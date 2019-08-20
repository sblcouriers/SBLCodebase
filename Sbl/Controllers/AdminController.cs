using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Sbl.Models;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Ajax.Utilities;
using Sbl.Helpers;
using Sbl.Models.BO;
using SelectPdf;
using WebGrease.Css.Extensions;

namespace Sbl.Controllers
{
    [Authorize(Roles = "master, admin, fleet, payroll, poc, recruitment,driver")]
    public class AdminController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();





        #region handbook




        public ActionResult Handbook(DateTime? Date, int? Days, int? DepotId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // get settings
            var settings = db.Settings.FirstOrDefault();


            // poc depot
            string pocDepot = "";
            if (User.IsInRole("poc"))
            {
                pocDepot = currentuser.Depot.Name;
            }


            // period list
            var periodroutes = db.VehicleHandbooks.Where(x => x.Active == true && x.Deleted == false).ToList();

            DateTime dateNow = DateTime.Now.Date;
            DateTime firstDate = DateTime.Now.Date;
            DateTime lastDate = DateTime.Now.Date;

            if (periodroutes.Count() > 0)
            {
                firstDate = periodroutes.OrderBy(x => x.BookDate).FirstOrDefault().BookDate.Value.Date;

                if (firstDate > DateTime.Now.Date)
                {
                    firstDate = DateTime.Now.Date.AddDays(-7);
                }

                lastDate = periodroutes.OrderByDescending(x => x.BookDate).FirstOrDefault().BookDate.Value.Date;

                if (lastDate < DateTime.Now.Date)
                {
                    lastDate = DateTime.Now.Date.AddDays(7);
                }
            }

            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }

            // period list
            List<HandbookViewModel.Period> periodList = new List<HandbookViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new HandbookViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate = DateTime.Now.Date;
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // get focus sunday
            if (focusDate.DayOfWeek != DayOfWeek.Sunday)
            {
                while (focusDate.DayOfWeek != DayOfWeek.Sunday)
                    focusDate = focusDate.AddDays(-1);
            }


            // days and period end date
            var periodDays = 28;
            if (Days.HasValue)
            {
                periodDays = Days.Value;
            }

            var periodEndDate = focusDate.AddDays(periodDays - 1);


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          orderby x.Name ascending
                          select new HandbookViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();


            // depotid
            var depotId = 0;
            if (currentuser.DepotId.HasValue)
            {
                depotId = currentuser.DepotId.Value;
            }
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }



            // loop 
            var vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList();
 
            var handbooks = db.VehicleHandbooks.Where(x => x.BookDate.Value >= focusDate && x.BookDate.Value <= periodEndDate && x.Active == true && x.Deleted == false).ToList();

            List<HandbookViewModel.Vehicle> vehicleList = new List<HandbookViewModel.Vehicle>();

            foreach (var vehicle in vehicles)
            {
                List<HandbookViewModel.Handbook> handbookList = new List<HandbookViewModel.Handbook>();

                int? associateId = 0;
                string associateName = "";
                int? associateDepotId = 0;
                string associateDepotName = "";

                for (int i = 0; i < periodDays; i++)
                {
                    var bookDate = focusDate.AddDays(i);

                    var handbook = handbooks.Where(x => x.VehicleId == vehicle.Id && x.BookDate == bookDate).FirstOrDefault();
                    
                    if (db.SubRentals.Where(x => x.VehicleId == vehicle.Id && !x.DateReturned.HasValue && x.Active == true && x.Deleted == false).OrderByDescending(x => x.DateRented).Any())
                    {
                        var subrental = db.SubRentals.Where(x => x.VehicleId == vehicle.Id && !x.DateReturned.HasValue && x.Active == true && x.Deleted == false).OrderByDescending(x => x.DateRented).FirstOrDefault();
                        associateId = subrental.Associate.Id;
                        associateName = subrental.Associate.Name;
                        associateDepotId = subrental.Associate.Depot.Id;
                        associateDepotName = subrental.Associate.Depot.Name;
                    }

                    if (handbook != null)
                    {
                        handbookList.Add(new HandbookViewModel.Handbook
                        {
                            VehicleId = vehicle.Id,
                            HandbookId = handbook.Id,
                            BookDate = bookDate,
                            Notes = handbook.Notes,
                            Status = handbook.Status
                        });
                    }
                    else
                    {
                        handbookList.Add(new HandbookViewModel.Handbook
                        {
                            VehicleId = vehicle.Id,
                            HandbookId = 0,
                            BookDate = bookDate,
                            Notes = "",
                            Status = ""
                        });
                    }
                }

                if (depotId == 0)
                {
                    vehicleList.Add(new HandbookViewModel.Vehicle
                    {
                        VehicleId = vehicle.Id,
                        AssociateName = associateName,
                        DepotName = associateDepotName,
                        Make = vehicle.Make,
                        Model = vehicle.Model,
                        Registration = vehicle.Registration,
                        Handbooks = handbookList
                    });
                }
                else
                {
                    if (depotId == associateDepotId)
                    {
                        vehicleList.Add(new HandbookViewModel.Vehicle
                        {
                            VehicleId = vehicle.Id,
                            AssociateName = associateName,
                            DepotName = associateDepotName,
                            Make = vehicle.Make,
                            Model = vehicle.Model,
                            Registration = vehicle.Registration,
                            Handbooks = handbookList
                        });
                    }

                }

            }

            var viewModel = new HandbookViewModel
            {
                POCDepot = pocDepot,
                DepotId = depotId,
                DateStart = focusDate,
                Days = periodDays,
                ScheduleEditDays = settings.ScheduleEditDays,
                //
                Vehicles = vehicleList,
                Periods = periodList,
                Depots = depots
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        public JsonResult HandbookEdit(int? vehicleid, DateTime? bookdate, string status, string notes)
        {
            try
            {
                var vehicle = db.Vehicles.Where(x => x.Id == vehicleid).FirstOrDefault();

                var handbooks = db.VehicleHandbooks.Where(x => x.VehicleId == vehicle.Id && x.BookDate == bookdate).ToList();

                foreach (var handbook in handbooks)
                {
                    db.VehicleHandbooks.Remove(handbook);
                    db.SaveChanges();
                }

                VehicleHandbook vehiclehandbook = new VehicleHandbook();

                vehiclehandbook.VehicleId = vehicleid;
                vehiclehandbook.BookDate = bookdate;
                vehiclehandbook.Status = status;
                vehiclehandbook.Notes = notes;
                vehiclehandbook.Active = true;
                vehiclehandbook.Deleted = false;
                vehiclehandbook.DateCreated = DateTime.Now;
                db.VehicleHandbooks.Add(vehiclehandbook);
                db.SaveChanges();

                var gethandbook = db.VehicleHandbooks.Where(x => x.Id == vehiclehandbook.Id).FirstOrDefault();
                var getvehicle = db.Vehicles.Where(x => x.Id == gethandbook.VehicleId).FirstOrDefault();

                return Json(new
                {
                    //
                    vehicleid = getvehicle.Id,
                    handbookid = gethandbook.Id,
                    description = getvehicle.Make + " - " + getvehicle.Model + " - " + getvehicle.Registration,
                    bookdate = String.Format("{0:yyyy-MM-dd}", gethandbook.BookDate),
                    status = gethandbook.Status
                    //
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult GetHandbookDetails(int? HandbookId)
        {
            try
            {
                if (db.VehicleHandbooks.Where(x => x.Id == HandbookId).Any())
                {
                    var handbook = db.VehicleHandbooks.Where(x => x.Id == HandbookId).FirstOrDefault();

                    return Json(new
                    {
                        //
                        HandbookId = HandbookId,
                        Notes = handbook.Notes,
                        Status = handbook.Status,
                        //
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        //
                        Status = 0,
                        //
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        
        public ActionResult HandbookList(DateTime? Date, int? VehicleId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // period list
            var periodroutes = db.VehicleHandbooks.Where(x => x.Active == true && x.Deleted == false).ToList();

            DateTime firstDate = DateTime.Now.Date;
            DateTime lastDate = DateTime.Now.Date;

            if (periodroutes.Count() > 0)
            {
                firstDate = periodroutes.OrderBy(x => x.BookDate).FirstOrDefault().BookDate.Value.Date;

                if (firstDate > DateTime.Now.Date)
                {
                    firstDate = DateTime.Now.Date.AddDays(-7);
                }

                lastDate = periodroutes.OrderByDescending(x => x.BookDate).FirstOrDefault().BookDate.Value.Date;

                if (lastDate < DateTime.Now.Date)
                {
                    lastDate = DateTime.Now.Date.AddDays(7);
                }
            }

            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }

            // period list
            List<HandbookListViewModel.Period> periodList = new List<HandbookListViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new HandbookListViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate = lastSaturday.AddDays(-6);
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // days and period end date
            var periodDays = 7;
            var periodEndDate = focusDate.AddDays(periodDays - 1);


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          select new HandbookListViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();


 
 


            var getvehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false);

 

            // init lists
            var associateReceipt = db.AssociateReceipts.Where(x => x.WeekStartDate == focusDate && x.Active == true && x.Deleted == false).ToList();
            var associateRemittance = db.AssociateRemittances.Where(x => x.WeekStartDate == focusDate && x.Active == true && x.Deleted == false).ToList();



            // vehicle list
            List<HandbookListViewModel.Vehicle> vehicleList = new List<HandbookListViewModel.Vehicle>();

            foreach (var vehicle in getvehicles)
            {
                var handbooks = periodroutes.Where(x => x.VehicleId == vehicle.Id && x.BookDate >= focusDate && x.BookDate <= periodEndDate && x.Active == true && x.Deleted == false).OrderBy(x => x.BookDate).ToList();

                List<HandbookListViewModel.Handbook> handbookList = new List<HandbookListViewModel.Handbook>();

                foreach (var handbook in handbooks)
                {
                    handbookList.Add(new HandbookListViewModel.Handbook
                    {
                        HandbookId = handbook.Id,
                        BookDate = handbook.BookDate,
                        Status = handbook.Status,
                        Notes = handbook.Notes,
                        VehicleId = handbook.VehicleId.Value
                        
                    });
                }

                //

                vehicleList.Add(new HandbookListViewModel.Vehicle
                {
                    VehicleId = vehicle.Id,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Registration = vehicle.Registration,
                    Handbooks = handbookList
                });
            }



            var viewModel = new HandbookListViewModel
            {
                VehicleId = VehicleId,
                DateStart = Date,
               
                //
                Vehicles = vehicleList,
                Periods = periodList,
                Depots = depots
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }



        #endregion





        #region time sheet




        public ActionResult TimeSheetList(DateTime? Date, int? DepotId, int? AssociateId, string Status)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // period list
            var periodroutes = db.RouteAllocations.Where(x => x.Active == true && x.Deleted == false).ToList();

            DateTime firstDate = DateTime.Now.Date;
            DateTime lastDate = DateTime.Now.Date;

            if (periodroutes.Count() > 0)
            {
                firstDate = periodroutes.OrderBy(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (firstDate > DateTime.Now.Date)
                {
                    firstDate = DateTime.Now.Date.AddDays(-7);
                }

                lastDate = periodroutes.OrderByDescending(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (lastDate < DateTime.Now.Date)
                {
                    lastDate = DateTime.Now.Date.AddDays(7);
                }
            }

            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }

            // period list
            List<TimeSheetListViewModel.Period> periodList = new List<TimeSheetListViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new TimeSheetListViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate = lastSaturday.AddDays(-6);
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // days and period end date
            var periodDays = 7;
            var periodEndDate = focusDate.AddDays(periodDays - 1);


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          select new TimeSheetListViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();



            // associates
            var associates = (from x in db.Associates
                              where x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false
                              orderby x.Name ascending
                              select new TimeSheetListViewModel.Associate
                              {
                                  Id = x.Id,
                                  Name = x.Name
                              }).ToList();


            // depotid
            var depotId = 0;
            if (currentuser.DepotId.HasValue)
            {
                depotId = currentuser.DepotId.Value;
            }
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }

            var status = "1";

            if (String.IsNullOrEmpty(Status))
            {
                status = "2";
            }
            else
            {
                status = Status;
            }

            // associate list
            //var getassociates = db.Associates.Where(x => x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false);
            var getassociates = db.Associates.Where(x => x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false);

            if (status == "2")
            {
                getassociates = getassociates.Where(x => x.AssociateStatus == "Active");
            }

            if (status == "3")
            {
                getassociates = getassociates.Where(x => x.AssociateStatus == "Deactivated");
            }

            // associateid
            var associateId = 0;
            if (AssociateId.HasValue)
            {
                associateId = AssociateId.Value;
            }

            // filter by associate id
            if (associateId > 0)
            {
                if (getassociates.Where(x => x.Id == associateId).Any())
                {
                    getassociates = getassociates.Where(x => x.Id == associateId);
                }
            }

            // filter by depot id
            if (getassociates.Where(x => x.DepotId == depotId).Any())
            {
                getassociates = getassociates.Where(x => x.DepotId == depotId);
            }




            // init lists
            var associateReceipt = db.AssociateReceipts.Where(x => x.WeekStartDate == focusDate && x.Active == true && x.Deleted == false).ToList();
            var associateRemittance = db.AssociateRemittances.Where(x => x.WeekStartDate == focusDate && x.Active == true && x.Deleted == false).ToList();



            // remittance list
            List<TimeSheetListViewModel.Remittance> remittanceList = new List<TimeSheetListViewModel.Remittance>();

            foreach (var associate in getassociates.OrderBy(x => x.Name))
            {
                var routes = periodroutes.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && (x.AllocationStatus == "ON" || x.AllocationStatus == "TRAINING") && x.Active == true && x.Deleted == false).OrderBy(x => x.RouteDate).ToList();

                List<TimeSheetListViewModel.Route> routeList = new List<TimeSheetListViewModel.Route>();

                foreach (var route in routes)
                {
                    routeList.Add(new TimeSheetListViewModel.Route
                    {
                        Id = route.Id,
                        RouteDate = route.RouteDate,
                        StartTime = route.StartTime,
                        EndTime = route.EndTime,
                        TotalTime = route.TotalTime
                    });
                }

                //

                remittanceList.Add(new TimeSheetListViewModel.Remittance
                {
                    AssociateId = associate.Id,
                    AssociateName = associate.Name,
                    AssociateDepotId = associate.DepotId,
                    //
                    Routes = routeList
                });
            }



            var viewModel = new TimeSheetListViewModel
            {
                AssociateId = associateId,
                DepotId = depotId,
                DateStart = Date,
                SelectedStatus = status,
                //
                Associates = associates,
                Periods = periodList,
                Depots = depots,
                Remittances = remittanceList
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }
      

        #endregion






        #region AssociateRemittanceList


        public ActionResult AssociateRemittanceList(DateTime? Date, int? DepotId, int? AssociateId, string Status)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // get settings
            var settings = db.Settings.FirstOrDefault();


            // period list
            var periodroutes = db.RouteAllocations.Where(x => x.Active == true && x.Deleted == false).ToList();

            DateTime firstDate = DateTime.Now.Date;
            DateTime lastDate = DateTime.Now.Date;

            if (periodroutes.Count() > 0)
            {
                firstDate = periodroutes.OrderBy(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (firstDate > DateTime.Now.Date)
                {
                    firstDate = DateTime.Now.Date.AddDays(-7);
                }

                lastDate = periodroutes.OrderByDescending(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (lastDate < DateTime.Now.Date)
                {
                    lastDate = DateTime.Now.Date.AddDays(7);
                }
            }

            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }

            // period list
            List<AssociateRemittanceListViewModel.Period> periodList = new List<AssociateRemittanceListViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new AssociateRemittanceListViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate = lastSaturday.AddDays(-6);
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // days and period end date
            var periodDays = 7;
            var periodEndDate = focusDate.AddDays(periodDays - 1);


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          select new AssociateRemittanceListViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();



            // associates
            var associates = (from x in db.Associates
                              where x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false
                              orderby x.Name ascending
                              select new AssociateRemittanceListViewModel.Associate
                              {
                                  Id = x.Id,
                                  Name = x.Name
                              }).ToList();


            // depotid
            var depotId = 0;
            if (currentuser.DepotId.HasValue)
            {
                depotId = currentuser.DepotId.Value;
            }
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }

            var status = "1";

            if (String.IsNullOrEmpty(Status))
            {
                status = "2";
            }
            else
            {
                status = Status;
            }

            // associate list
            //var getassociates = db.Associates.Where(x => x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false);
            var getassociates = db.Associates.Where(x => x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false).ToList();

            if (status == "2")
            {
                getassociates = getassociates.Where(x => x.AssociateStatus == "Active").ToList();
            }

            if (status == "3")
            {
                getassociates = getassociates.Where(x => x.AssociateStatus == "Deactivated").ToList();
            }

            // associateid
            var associateId = 0;
            if (AssociateId.HasValue)
            {
                associateId = AssociateId.Value;
            }

            // filter by associate id
            if (associateId > 0)
            {
                if (getassociates.Where(x => x.Id == associateId).Any())
                {
                    getassociates = getassociates.Where(x => x.Id == associateId).ToList();
                }
            }

            // filter by depot id
            if (getassociates.Where(x => x.DepotId == depotId).Any())
            {
                getassociates = getassociates.Where(x => x.DepotId == depotId).ToList();
            }




            // init lists
            var associateReceipt = db.AssociateReceipts.Where(x => x.WeekStartDate == focusDate && x.Active == true && x.Deleted == false).ToList();
            var associateRemittance = db.AssociateRemittances.Where(x => x.WeekStartDate == focusDate && x.Active == true && x.Deleted == false).ToList();


            // remittance list
            List<AssociateRemittanceListViewModel.Remittance> remittanceList = new List<AssociateRemittanceListViewModel.Remittance>();

            foreach (var associate in getassociates.OrderBy(x => x.Name))
            {


                /*

                //var getroutes = periodroutes.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && (x.AllocationStatus == "ON" || x.AllocationStatus == "TRAINING") && x.Active == true && x.Deleted == false).OrderBy(x => x.RouteDate).ToList();
                var getroutes = db.RouteAllocations.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && x.Active == true && x.Deleted == false);

                List<AssociateRemittanceListViewModel.Route> routeList = new List<AssociateRemittanceListViewModel.Route>();

                var routeCount = 0;

                foreach (var route in getroutes)
                {
                    routeList.Add(new AssociateRemittanceListViewModel.Route
                    {
                        Id = route.Id,
                        RouteDate = route.RouteDate,
                        RouteType1 = route.RouteType1,
                        RouteCode1 = route.RouteCode1,
                        Mileage = route.Mileage,
                        Note = route.Notes
                    });

                    routeCount++;
                }

 
                */


                var getroutes = periodroutes.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && (x.AllocationStatus == "ON" || x.AllocationStatus == "TRAINING") && x.Active == true && x.Deleted == false).OrderBy(x => x.RouteDate).ToList();
                //var getroutes = db.RouteAllocations.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && x.Active == true && x.Deleted == false);

                //List<AssociateRemittanceListViewModel.Route> routeList = new List<AssociateRemittanceListViewModel.Route>();



                // routes
                var routes = (from x in getroutes
                              select new AssociateRemittanceListViewModel.Route
                              {
                                  Id = x.Id,
                                  RouteDate = x.RouteDate,
                                  //RouteType1 = x.RouteType + (x.RouteType2 != "0" ? " / " + x.RouteType2 : "") + (x.RouteType3 != "0" ? " / " + x.RouteType3 : ""),
                                  //RouteCode = x.RouteCode + (x.RouteCode2 != "0" ? " / " + x.RouteCode2 : "") + (x.RouteCode3 != "0" ? " / " + x.RouteCode3 : ""),
                                  RouteType1 = x.RouteType1 == "SupportAd1" ||
                                               x.RouteType1 == "SupportAd2" ||
                                               x.RouteType1 == "SupportAd3" ? "Support" : x.RouteType1,
                                  RouteCode1 = x.RouteCode1,
                                  RouteExtra = (x.RouteType2 != "0" ?
                                                   x.RouteType2 == "SupportAd1" ||
                                                   x.RouteType2 == "SupportAd2" ||
                                                   x.RouteType2 == "SupportAd3" ? "Support" : x.RouteType2 : "") +
                                               (x.RouteType3 != "0" ? " / " +
                                                   x.RouteType3 == "SupportAd1" ||
                                                   x.RouteType3 == "SupportAd2" ||
                                                   x.RouteType3 == "SupportAd3" ? "Support" : x.RouteType3 : "") +
                                               (x.RouteType4 != "0" ? " / " +
                                                   x.RouteType4 == "SupportAd1" ||
                                                   x.RouteType4 == "SupportAd2" ||
                                                   x.RouteType4 == "SupportAd3" ? "Support" : x.RouteType4 : "") +
                                               (x.RouteType5 != "0" ? " / " +
                                                   x.RouteType5 == "SupportAd1" ||
                                                   x.RouteType5 == "SupportAd2" ||
                                                   x.RouteType5 == "SupportAd3" ? "Support" : x.RouteType5 : ""),
                                  //
                                  //RouteExtra = (x.RouteType2 != "0" ? x.RouteType2 + " (" + x.RouteCode2 + ")" : "") + (x.RouteType3 != "0" ? " / " + x.RouteType3 + " (" + x.RouteCode3 + ")" : ""),
                                  //Depot = x.Associate.Depot.Name,
                                  Depot = x.Depot.Name,
                                  Mileage = x.Mileage,
                                  Byod = x.BYODPrice,
                                  //RouteRate = x.RoutePrice1 + x.RoutePrice2 + x.RoutePrice3,
                                  RouteRate = x.RoutePrice1,
                                  RouteExtraRate = x.RoutePrice2 + x.RoutePrice3,
                                  FuelSupport = x.Mileage * settings.Mileage1MileSBL,
                                  SubTotal = (x.BYODPrice + x.RoutePrice1 + x.RoutePrice2 + x.RoutePrice3 + (x.Mileage * settings.Mileage1MileSBL)),
                                  //
                                  AllocationStatus = x.AllocationStatus,
                                  AuthPayroll = x.AuthPayroll,
                                  AuthPoc = x.AuthPoc,
                                  AuthAdmin = x.AuthAdmin,
                                  Note = x.Notes,
                              }).OrderBy(x => x.RouteDate).ToList();


                // get charges
                var getcharges = db.Charges.Where(x => x.AssociateId == associate.Id && x.Date >= focusDate && x.Date <= periodEndDate && x.Active == true && x.Deleted == false);


                // credits
                var credits = (from x in getcharges
                               where x.SetAsCredit == true
                               select new AssociateRemittanceListViewModel.Credit
                               {
                                   CreditDate = x.Date,
                                   Description = x.Description,
                                   CreditAmount = x.Amount
                               }).ToList();


                // deductions
                var deductions = (from x in getcharges
                                  where x.SetAsCredit == false
                                  select new AssociateRemittanceListViewModel.Deduction
                                  {
                                      DeductionDate = x.Date,
                                      Description = x.Description,
                                      DeductionAmount = x.Amount
                                  }).ToList();


                // subrentals

                var holidays = getroutes.Where(x => x.AllocationStatus == "HOLIDAY").ToList();

                var getsubrentals = db.SubRentals.Where(x =>
                    x.AssociateId == associate.Id &&
                    x.DateRented <= periodEndDate &&
                    //((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (!x.DateReturned.HasValue)) &&
                    ((x.DateReturned >= focusDate) || (!x.DateReturned.HasValue)) &&
                    x.Active == true &&
                    x.Deleted == false);

                var subrentals1 = (from x in getsubrentals
                                   select new AssociateRemittanceListViewModel.SubRental
                                   {
                                       DateRented = x.DateRented,
                                       DateReturned = x.DateReturned,
                                       VanName = x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                       VanRentalDays = 0,
                                       //
                                       VanRentalDescription = "",
                                       VanRentalPrice = x.VanRentalPrice,
                                       InsuranceDescription = "",
                                       InsurancePrice = x.InsurancePrice,
                                       GoodsInTransitDescription = "",
                                       GoodsInTransitPrice = x.GoodsInTransitPrice,
                                       PublicLiabilityDescription = "",
                                       PublicLiabilityPrice = x.PublicLiabilityPrice,
                                       SubRentalAmount = x.RentalPrice
                                   }).AsEnumerable().Select(s => new AssociateRemittanceListViewModel.SubRental
                                   {
                                       DateRented = s.DateRented,
                                       DateReturned = s.DateReturned,
                                       VanName = s.VanName,
                                       VanRentalDays = s.DateRented < focusDate && !s.DateReturned.HasValue ? (periodEndDate - focusDate).Days + 1 - (holidays.Where(y => y.RouteDate >= focusDate && y.RouteDate <= periodEndDate).Count()) :
                                                       s.DateRented >= focusDate && !s.DateReturned.HasValue ? (periodEndDate - s.DateRented).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= s.DateRented && y.RouteDate <= periodEndDate).Count()) :
                                                       s.DateRented < focusDate && s.DateReturned.HasValue ? s.DateReturned <= periodEndDate ? (s.DateReturned - focusDate).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= focusDate && y.RouteDate <= s.DateReturned).Count()) : (periodEndDate - focusDate).Days + 1 - (holidays.Where(y => y.RouteDate >= focusDate && y.RouteDate <= periodEndDate).Count()) :
                                                       s.DateRented >= focusDate && s.DateReturned.HasValue ? s.DateReturned <= periodEndDate ? (s.DateReturned - s.DateRented).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= s.DateRented && y.RouteDate <= s.DateReturned).Count()) : (periodEndDate - s.DateRented).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= s.DateRented && y.RouteDate <= periodEndDate).Count()) : 0,
                                       //
                                       VanRentalDescription = "Van Rental: " + s.VanName,
                                       VanRentalPrice = s.VanRentalPrice,
                                       InsuranceDescription = "Insurance: " + s.VanName,
                                       InsurancePrice = s.InsurancePrice,
                                       GoodsInTransitDescription = "Goods in Transit: " + s.VanName,
                                       GoodsInTransitPrice = s.GoodsInTransitPrice,
                                       PublicLiabilityDescription = "Public Liability: " + s.VanName,
                                       PublicLiabilityPrice = s.PublicLiabilityPrice,
                                       SubRentalAmount = s.SubRentalAmount
                                   }).ToList();


                var subrentals = (from x in subrentals1
                                  select new AssociateRemittanceListViewModel.SubRental
                                  {
                                      DateRented = x.DateRented,
                                      DateReturned = x.DateReturned,
                                      VanName = x.VanName,
                                      VanRentalDays = x.VanRentalDays,
                                      //
                                      VanRentalDescription = x.VanRentalDescription,
                                      VanRentalPrice = x.VanRentalPrice,
                                      InsuranceDescription = x.InsuranceDescription,
                                      InsurancePrice = x.InsurancePrice,
                                      GoodsInTransitDescription = x.GoodsInTransitDescription,
                                      GoodsInTransitPrice = x.GoodsInTransitPrice,
                                      PublicLiabilityDescription = x.PublicLiabilityDescription,
                                      PublicLiabilityPrice = x.PublicLiabilityPrice,
                                      SubRentalAmount = x.SubRentalAmount
                                  }).AsEnumerable().Select(s => new AssociateRemittanceListViewModel.SubRental
                                  {
                                      DateRented = s.DateRented,
                                      DateReturned = s.DateReturned,
                                      VanName = s.VanName,
                                      VanRentalDays = s.VanRentalDays,
                                      //
                                      VanRentalDescription = s.VanRentalDescription,
                                      VanRentalPrice = ((s.VanRentalPrice / 7) * s.VanRentalDays),
                                      InsuranceDescription = s.InsuranceDescription,
                                      InsurancePrice = ((s.InsurancePrice / 7) * s.VanRentalDays),
                                      GoodsInTransitDescription = s.GoodsInTransitDescription,
                                      GoodsInTransitPrice = ((s.GoodsInTransitPrice / 7) * s.VanRentalDays),
                                      PublicLiabilityDescription = s.PublicLiabilityDescription,
                                      PublicLiabilityPrice = ((s.PublicLiabilityPrice / 7) * s.VanRentalDays),
                                      SubRentalAmount = ((s.SubRentalAmount / 7) * s.VanRentalDays),
                                  }).ToList();
              

                // extra deductions
                var extradeductions = (from x in getroutes
                                       where x.FuelChargePrice > 0
                                       select new AssociateRemittanceListViewModel.ExtraDeduction
                                       {
                                           DeductionDate = x.RouteDate,
                                           Description = "Fuel Charge",
                                           DeductionAmount = x.FuelChargePrice
                                       }).ToList();


                // charge claims
                List<AssociateRemittanceListViewModel.ChargeClaim> chargeclaims = new List<AssociateRemittanceListViewModel.ChargeClaim>();
                var getchargeclaims = db.ChargeClaims.Where(x => x.AssociateId == associate.Id && x.Active == true && x.Deleted == false);
                foreach (var charge in getchargeclaims)
                {
                    int instalments = charge.NumberOfInstalments;
                    double amount = charge.Amount / charge.NumberOfInstalments;
                    for (int i = 0; i < instalments; i++)
                    {
                        int weekdate = 7 * charge.WeekFrequency * i;
                        string description = charge.Description + " (" + (charge.WeekFrequency == 1 ? "Every " + charge.WeekFrequency + " week" : "Every " + charge.WeekFrequency + " weeks") + ": " + (i + 1) + "/" + charge.NumberOfInstalments + ")";
                        DateTime instalmentdate = charge.DateFirstInstalment.AddDays(weekdate);
                        if (instalmentdate >= focusDate && instalmentdate <= periodEndDate)
                        {
                            chargeclaims.Add(new AssociateRemittanceListViewModel.ChargeClaim
                            {
                                Id = charge.Id,
                                ChargeClaimDate = instalmentdate,
                                ChargeClaimAmount = amount,
                                Description = description
                            });
                        }
                    }
                }


                // charge pcn
                List<AssociateRemittanceListViewModel.ChargePcn> chargepcns = new List<AssociateRemittanceListViewModel.ChargePcn>();
                var getchargepcns = db.ChargePcns.Where(x => x.AssociateId == associate.Id && x.Active == true && x.Deleted == false);
                foreach (var charge in getchargepcns)
                {
                    int instalments = charge.NumberOfInstalments;
                    double amount = (charge.PcnFee + charge.AdminFee) / charge.NumberOfInstalments;
                    for (int i = 0; i < instalments; i++)
                    {
                        int weekdate = 7 * charge.WeekFrequency * i;
                        string description = charge.Description + " (" + (charge.WeekFrequency == 1 ? "Every " + charge.WeekFrequency + " week" : "Every " + charge.WeekFrequency + " weeks") + ": " + (i + 1) + "/" + charge.NumberOfInstalments + ")";
                        DateTime instalmentdate = charge.DateFirstInstalment.AddDays(weekdate);
                        if (instalmentdate >= focusDate && instalmentdate <= periodEndDate)
                        {
                            chargepcns.Add(new AssociateRemittanceListViewModel.ChargePcn
                            {
                                Id = charge.Id,
                                ChargePcnDate = instalmentdate,
                                ChargePcnAmount = amount,
                                Description = description
                            });
                        }
                    }
                }


                //
                var sumMileage = routes.Sum(x => x.Mileage);
                var sumByod = routes.Sum(x => x.Byod);
                var sumRouteRate = routes.Sum(x => x.RouteRate);
                var sumRouteExtraRate = routes.Sum(x => x.RouteExtraRate);
                var sumFuelSupport = routes.Sum(x => x.FuelSupport);
                //
                var sumSubTotal = routes.Sum(x => x.SubTotal);
                var sumCreditAmount = credits.Sum(x => x.CreditAmount);
                //
                var sumDeductionAmount = deductions.Sum(x => x.DeductionAmount);
                var sumSubRentalAmount = subrentals.Sum(x => x.SubRentalAmount);
                var sumExtraDeductionAmount = extradeductions.Sum(x => x.DeductionAmount);
                var sumChargeClaimAmount = chargeclaims.Sum(x => x.ChargeClaimAmount);
                var sumChargePcnAmount = chargepcns.Sum(x => x.ChargePcnAmount);
                //
                var totalCredits = sumSubTotal + sumCreditAmount;
                var totalDeductions = sumDeductionAmount + sumSubRentalAmount + sumExtraDeductionAmount + sumChargeClaimAmount + sumChargePcnAmount;
                //
                var total = totalCredits - totalDeductions;
                 

                //


                var authPoc = routes.Any(x => x.AuthPoc == true) ? true : false;
                var authPayroll = routes.Any(x => x.AuthPayroll == true) ? true : false;
                var authAdmin = routes.Any(x => x.AuthAdmin == true) ? true : false;

                // counts

                var associateReceipt2 = associateReceipt.Where(x => x.AssociateId == associate.Id).ToList();
                var receiptSent = associateReceipt2.Any() ? associateReceipt2.Count() : 0;

                var associateRemittance2 = associateRemittance.Where(x => x.AssociateId == associate.Id).ToList();
                var remittanceSent = associateRemittance2.Any() ? associateRemittance2.Count() : 0;



                remittanceList.Add(new AssociateRemittanceListViewModel.Remittance
                {
                    AssociateId = associate.Id,
                    AssociateName = associate.Name,

                    AssociateDepotName = associate.Depot.Name,
                    AssociateDepotId = associate.DepotId,

                    RouteCount = routes.Count(),
                    RoutePay = sumSubTotal,
                    AdditionalPay = 0,
                    ByodPay = sumByod,
                    DpmoBonusPay = 0,
                    TotalMiles = sumMileage,
                    MileagePay = sumFuelSupport,
                    OtherPay = sumCreditAmount,

                    OtherDeductionsPay = sumDeductionAmount,
                    VanRentalPay = sumSubRentalAmount,
                    VanInsurancePay = 0,
                    DamageChargesPay = sumChargeClaimAmount,
                    VanFinesPay = sumChargePcnAmount,
                    CoFuelUsedPay = sumExtraDeductionAmount,
                    LoanPay = 0,

                    NetPay = total,
                    SecuritaxPay = 15,
                    GrossPay = total - 15,

                    //
                    AuthPoc = authPoc,
                    AuthPayroll = authPayroll,
                    AuthAdmin = authAdmin,
                    //
                    //Total = total,
                    //
                    ReceiptSent = receiptSent,
                    RemittanceSent = remittanceSent,
                    //
                    Routes = routes
                });


                #region 


                /*

                #region Calculate Totals

                // get settings
                var settings = db.Settings.FirstOrDefault();

                // this period
                double deductOwnVechileAmount = 0;


                if (associate.OwnVehicle)
                {
                deductOwnVechileAmount = WebConstant.OwnVechicleDeductAmount;//If the associate “OwnVehicle” is true then it should deduct £5 from daily route
                }

                // get charges
                var charges = (from x in db.Charges
                where x.AssociateId == associate.Id && x.Date >= focusDate && x.Date <= periodEndDate && x.Active == true && x.Deleted == false
                select new AssociateRemittanceViewModel.Charge
                {
                Date = x.Date,
                Description = x.Description,
                Amount = x.Amount,
                SetAsCredit = x.SetAsCredit
                }).ToList();

                // get subtotals & total
                List<AssociateRemittanceViewModel.Subtotal> sublist = new List<AssociateRemittanceViewModel.Subtotal>();
                var subtotals = db.RouteAllocations.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && x.Active == true && x.Deleted == false).ToList();

                #region Route Calculatation 
                foreach (var sub in subtotals)
                {
                double ad1Quantity = 0;
                double ad1Price = 0;
                double ad2Quantity = 0;
                double ad2Price = 0;
                double ad3Quantity = 0;
                double ad3Price = 0;
                double mileage = 0;
                double fuelSupport = 0;
                double deduct = 0;


                #region Route Type Calculatation
                if (sublist.Any(x => x.Description == sub.RouteType))
                {
                var sublistitem = sublist.FirstOrDefault(x => x.Description == sub.RouteType);
                if (sublistitem != null)
                {
                sublistitem.Quantity = sublistitem.Quantity + 1;
                sublistitem.Amount = sublistitem.Amount + sub.RoutePrice;
                }
                }
                else
                {
                sublist.Add(new AssociateRemittanceViewModel.Subtotal
                {
                Description = sub.RouteType1,
                Quantity = 1,
                Amount = sub.RoutePrice
                });
                }
                #endregion

                #region Ad1 Quantity Calculatation
                if (sub.Ad1Quantity > 0)
                {
                ad1Quantity = ad1Quantity + sub.Ad1Quantity;
                ad1Price = ad1Price + (sub.Ad1Quantity * sub.Ad1Price);
                }

                if (sublist.Any(x => x.Description == WebConstant.Ad1))
                {
                var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Ad1);
                if (sublistitem != null)
                {
                sublistitem.Quantity = sublistitem.Quantity + ad1Quantity;
                sublistitem.Amount = sublistitem.Amount + ad1Price;
                }
                }
                else
                {
                sublist.Add(new AssociateRemittanceViewModel.Subtotal
                {
                Description = WebConstant.Ad1,
                Quantity = ad1Quantity,
                Amount = ad1Price
                });
                }

                #endregion

                #region Ad2 Quantity Calculatation
                if (sub.Ad2Quantity > 0)
                {
                ad2Quantity = ad2Quantity + sub.Ad2Quantity;
                ad2Price = ad2Price + (sub.Ad2Quantity * sub.Ad2Price);
                }

                if (sublist.Any(x => x.Description == WebConstant.Ad2))
                {
                var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Ad2);
                if (sublistitem != null)
                {
                sublistitem.Quantity = sublistitem.Quantity + ad2Quantity;
                sublistitem.Amount = sublistitem.Amount + ad2Price;
                }
                }
                else
                {
                sublist.Add(new AssociateRemittanceViewModel.Subtotal
                {
                Description = WebConstant.Ad2,
                Quantity = ad2Quantity,
                Amount = ad2Price
                });
                }

                #endregion

                #region Ad3 Quantity Calculatation
                if (sub.Ad3Quantity > 0)
                {
                ad3Quantity = ad3Quantity + sub.Ad3Quantity;
                ad3Price = ad3Price + (sub.Ad3Quantity * sub.Ad3Price);
                }

                if (sublist.Any(x => x.Description == WebConstant.Ad3))
                {
                var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Ad3);
                if (sublistitem != null)
                {
                sublistitem.Quantity = sublistitem.Quantity + ad3Quantity;
                sublistitem.Amount = sublistitem.Amount + ad3Price;
                }
                }
                else
                {
                sublist.Add(new AssociateRemittanceViewModel.Subtotal
                {
                Description = WebConstant.Ad3,
                Quantity = ad3Quantity,
                Amount = ad3Price
                });
                }
                #endregion

                #region Litres & Mileage Calculatation
                if (sub.Mileage > 0)
                {
                mileage = mileage + sub.Mileage;
                fuelSupport = fuelSupport + ((sub.Mileage / settings.MilesPerLitre) * settings.DieselPrice);
                }

                if (sublist.Any(x => x.Description == WebConstant.LitresAndMileage))
                {
                var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.LitresAndMileage);
                if (sublistitem != null)
                {
                sublistitem.Quantity = sublistitem.Quantity + mileage;
                sublistitem.Amount = sublistitem.Amount + fuelSupport;
                }
                }
                else
                {
                sublist.Add(new AssociateRemittanceViewModel.Subtotal
                {
                Description = WebConstant.LitresAndMileage,
                Quantity = mileage,
                Amount = fuelSupport
                });
                }
                #endregion

                #region Deduct Calculatation
                var deductAmt = deductOwnVechileAmount + sub.Deduct;
                if (sublist.Any(x => x.Description == WebConstant.Deduct))
                {
                var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Deduct);
                if (sublistitem != null)
                {
                sublistitem.Quantity = 1;
                sublistitem.Amount = sublistitem.Amount + deductAmt;
                }
                }
                else
                {
                sublist.Add(new AssociateRemittanceViewModel.Subtotal
                {
                Description = WebConstant.Deduct,
                Quantity = 1,
                Amount = deductAmt
                });
                }
                #endregion
                }
                #endregion

                #region Vechicle Rent Related Calculation
                var subRentalCharges = 0.00;
                var associateVechileRental = new List<AssociateRemittanceViewModel.Subrental>();

                if (!associate.OwnVehicle)
                {
                var subRentals = associate.SubRentals.Where(x => ((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (x.DateReturned == null)) && x.Active && x.Deleted == false).ToList();

                foreach (var subRent in subRentals)
                {
                if (subRent != null)
                {
                subRentalCharges = subRentalCharges + subRent.RentalPrice;

                var rental = new AssociateRemittanceViewModel.Subrental();
                rental.Description = subRent.Vehicle.Make + " - " + subRent.Vehicle.Model + " - " + subRent.Vehicle.Registration;
                rental.Amount = subRent.RentalPrice;
                associateVechileRental.Add(rental);
                }
                }
                }
                #endregion

                var deductRow = sublist.FirstOrDefault(x => x.Description == WebConstant.Deduct);
                if (deductRow != null)
                {
                deductRow.Amount = -deductRow.Amount;
                }
                var routeAmt = sublist.Sum(x => x.Amount);

                var routeRental = associateVechileRental.Sum(x => x.Amount);
                var creditCharges = charges.Where(x => x.SetAsCredit).Sum(x => x.Amount);
                var routeCharges = charges.Where(x => x.SetAsCredit == false).Sum(x => x.Amount);

                // total
                // double total = sublist.Sum(x => x.Amount) + charges.Where(x => x.SetAsCredit == false).Sum(x => x.Amount) - subrentals.Sum(x => x.Amount) - charges.Where(x => x.SetAsCredit == true).Sum(x => x.Amount);
                double total = routeAmt + creditCharges - routeRental - routeCharges;
                #endregion

                */
    
            
                #endregion


            }



            var viewModel = new AssociateRemittanceListViewModel
            {
                AssociateId = associateId,
                DepotId = depotId,
                DateStart = Date,
                SelectedStatus = status,
                //
                Associates = associates,
                Periods = periodList,
                Depots = depots,
                Remittances = remittanceList
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }



        /*
        public ActionResult AssociateRemittanceOLD(int? AssociateId, DateTime? Date)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // get settings
            var settings = db.Settings.FirstOrDefault();


            // focus date
            var focusDate = Date.Value.Date;

            // days and period end date
            var periodEndDate = focusDate.AddDays(6);


            // get associate
            var associate = db.Associates.Where(x => x.Id == AssociateId && x.Active == true && x.Deleted == false).FirstOrDefault();


            // this period
            double deductOwnVechileAmount = 0;

            // get routes
            if (associate != null)
            {
                if (associate.OwnVehicle)
                {
                    deductOwnVechileAmount = WebConstant.OwnVechicleDeductAmount;//If the associate “OwnVehicle” is true then it should deduct £5 from daily route
                }

                var routes = (from x in associate.RouteAllocations
                              where x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && x.Active == true && x.Deleted == false
                              select new AssociateRemittanceViewModel.Route
                              {
                                  Id = x.Id,
                                  RouteDate = x.RouteDate,
                                  RouteType1 = x.RouteType1,
                                  RouteCode = x.RouteCode1,
                                  Depot = x.Depot.Name,
                                  Ad1Quantity = x.Ad1Quantity,
                                  Ad1Price = x.Ad1Price,
                                  Ad2Quantity = x.Ad2Quantity,
                                  Ad2Price = x.Ad2Price,
                                  Ad3Quantity = x.Ad3Quantity,
                                  Ad3Price = x.Ad3Price,
                                  Mileage = x.Mileage,
                                  Litres = x.Mileage / settings.MilesPerLitre,
                                  FuelSupport = (x.Mileage / settings.MilesPerLitre) * settings.DieselPrice,
                                  RouteRate = x.RoutePrice1,
                                  AllocationStatus = x.AllocationStatus,
                                  AuthPayroll = x.AuthPayroll,
                                  AuthPoc = x.AuthPoc,
                                  AuthAdmin = x.AuthAdmin,
                                  Deduct = x.Deduct + (x.Associate.OwnVehicle ? WebConstant.OwnVechicleDeductAmount : 0),
                                  RoutePrice = x.RoutePrice
                              }).OrderBy(x => x.RouteDate).ToList();


                // get charges
                var charges = (from x in associate.Charges
                               where x.Date >= focusDate && x.Date <= periodEndDate && x.Active == true && x.Deleted == false
                               select new AssociateRemittanceViewModel.Charge
                               {
                                   Date = x.Date,
                                   Description = x.Description,
                                   Amount = x.Amount,
                                   SetAsCredit = x.SetAsCredit
                               }).ToList();


                // get subtotals & total
                List<AssociateRemittanceViewModel.Subtotal> sublist = new List<AssociateRemittanceViewModel.Subtotal>();
                var subtotals = associate.RouteAllocations.Where(x => x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && x.Active == true && x.Deleted == false).ToList();

                #region Route Calculatation 
                foreach (var sub in subtotals)
                {
                    double ad1Quantity = 0;
                    double ad1Price = 0;
                    double ad2Quantity = 0;
                    double ad2Price = 0;
                    double ad3Quantity = 0;
                    double ad3Price = 0;
                    double mileage = 0;
                    double fuelSupport = 0;
                    double deduct = 0;


                    #region Route Type Calculatation
                    if (sublist.Any(x => x.Description == sub.RouteType))
                    {
                        var sublistitem = sublist.FirstOrDefault(x => x.Description == sub.RouteType);
                        if (sublistitem != null)
                        {
                            sublistitem.Quantity = sublistitem.Quantity + 1;
                            sublistitem.Amount = sublistitem.Amount + sub.RoutePrice;
                        }
                    }
                    else
                    {
                        sublist.Add(new AssociateRemittanceViewModel.Subtotal
                        {
                            Description = sub.RouteType1,
                            Quantity = 1,
                            Amount = sub.RoutePrice
                        });
                    }
                    #endregion

                    #region Ad1 Quantity Calculatation
                    if (sub.Ad1Quantity > 0)
                    {
                        ad1Quantity = ad1Quantity + sub.Ad1Quantity;
                        ad1Price = ad1Price + (sub.Ad1Quantity * sub.Ad1Price);
                    }

                    if (sublist.Any(x => x.Description == WebConstant.Ad1))
                    {
                        var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Ad1);
                        if (sublistitem != null)
                        {
                            sublistitem.Quantity = sublistitem.Quantity + ad1Quantity;
                            sublistitem.Amount = sublistitem.Amount + ad1Price;
                        }
                    }
                    else
                    {
                        sublist.Add(new AssociateRemittanceViewModel.Subtotal
                        {
                            Description = WebConstant.Ad1,
                            Quantity = ad1Quantity,
                            Amount = ad1Price
                        });
                    }

                    #endregion

                    #region Ad2 Quantity Calculatation
                    if (sub.Ad2Quantity > 0)
                    {
                        ad2Quantity = ad2Quantity + sub.Ad2Quantity;
                        ad2Price = ad2Price + (sub.Ad2Quantity * sub.Ad2Price);
                    }

                    if (sublist.Any(x => x.Description == WebConstant.Ad2))
                    {
                        var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Ad2);
                        if (sublistitem != null)
                        {
                            sublistitem.Quantity = sublistitem.Quantity + ad2Quantity;
                            sublistitem.Amount = sublistitem.Amount + ad2Price;
                        }
                    }
                    else
                    {
                        sublist.Add(new AssociateRemittanceViewModel.Subtotal
                        {
                            Description = WebConstant.Ad2,
                            Quantity = ad2Quantity,
                            Amount = ad2Price
                        });
                    }

                    #endregion

                    #region Ad3 Quantity Calculatation
                    if (sub.Ad3Quantity > 0)
                    {
                        ad3Quantity = ad3Quantity + sub.Ad3Quantity;
                        ad3Price = ad3Price + (sub.Ad3Quantity * sub.Ad3Price);
                    }

                    if (sublist.Any(x => x.Description == WebConstant.Ad3))
                    {
                        var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Ad3);
                        if (sublistitem != null)
                        {
                            sublistitem.Quantity = sublistitem.Quantity + ad3Quantity;
                            sublistitem.Amount = sublistitem.Amount + ad3Price;
                        }
                    }
                    else
                    {
                        sublist.Add(new AssociateRemittanceViewModel.Subtotal
                        {
                            Description = WebConstant.Ad3,
                            Quantity = ad3Quantity,
                            Amount = ad3Price
                        });
                    }
                    #endregion

                    #region Litres & Mileage Calculatation
                    if (sub.Mileage > 0)
                    {
                        mileage = mileage + sub.Mileage;
                        fuelSupport = fuelSupport + ((sub.Mileage / settings.MilesPerLitre) * settings.DieselPrice);
                    }

                    if (sublist.Any(x => x.Description == WebConstant.LitresAndMileage))
                    {
                        var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.LitresAndMileage);
                        if (sublistitem != null)
                        {
                            sublistitem.Quantity = sublistitem.Quantity + mileage;
                            sublistitem.Amount = sublistitem.Amount + fuelSupport;
                        }
                    }
                    else
                    {
                        sublist.Add(new AssociateRemittanceViewModel.Subtotal
                        {
                            Description = WebConstant.LitresAndMileage,
                            Quantity = mileage,
                            Amount = fuelSupport
                        });
                    }
                    #endregion

                    #region Deduct Calculatation
                    var deductAmt = deductOwnVechileAmount + sub.Deduct;
                    if (sublist.Any(x => x.Description == WebConstant.Deduct))
                    {
                        var sublistitem = sublist.FirstOrDefault(x => x.Description == WebConstant.Deduct);
                        if (sublistitem != null)
                        {
                            sublistitem.Quantity = 1;
                            sublistitem.Amount = sublistitem.Amount + deductAmt;
                        }
                    }
                    else
                    {
                        sublist.Add(new AssociateRemittanceViewModel.Subtotal
                        {
                            Description = WebConstant.Deduct,
                            Quantity = 1,
                            Amount = deductAmt
                        });
                    }
                    #endregion
                }
                #endregion

                #region Vechicle Rent Related Calculation
                var subRentalCharges = 0.00;
                var associateVechileRental = new List<AssociateRemittanceViewModel.Subrental>();

                if (!associate.OwnVehicle)
                {
                    var subRentals = associate.SubRentals.Where(x => ((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (x.DateReturned == null)) && x.Active && x.Deleted == false).ToList();

                    foreach (var subRent in subRentals)
                    {
                        if (subRent != null)
                        {
                            subRentalCharges = subRentalCharges + subRent.RentalPrice;

                            var rental = new AssociateRemittanceViewModel.Subrental();
                            rental.Description = subRent.Vehicle.Make + " - " + subRent.Vehicle.Model + " - " + subRent.Vehicle.Registration;
                            rental.Amount = subRent.RentalPrice;
                            associateVechileRental.Add(rental);
                        }
                    }
                }
                #endregion

                var deductRow = sublist.FirstOrDefault(x => x.Description == WebConstant.Deduct);
                if (deductRow != null)
                {
                    deductRow.Amount = -deductRow.Amount;
                }
                var routeAmt = sublist.Sum(x => x.Amount);

                var routeRental = associateVechileRental.Sum(x => x.Amount);
                var creditCharges = charges.Where(x => x.SetAsCredit).Sum(x => x.Amount);
                var routeCharges = charges.Where(x => x.SetAsCredit == false).Sum(x => x.Amount);

                // total
                // double total = sublist.Sum(x => x.Amount) + charges.Where(x => x.SetAsCredit == false).Sum(x => x.Amount) - subrentals.Sum(x => x.Amount) - charges.Where(x => x.SetAsCredit == true).Sum(x => x.Amount);
                double total = routeAmt + creditCharges - routeRental - routeCharges;

                //

                var viewModel = new AssociateRemittanceViewModel
                {
                    AssociateId = associate.Id,
                    //
                    SblRemittanceBusinessName = settings.SblRemittanceBusinessName,
                    SblRemittanceBusinessVatNumber = settings.SblRemittanceBusinessVatNumber,
                    SblRemittanceThankYouMessage = settings.SblRemittanceThankYouMessage,
                    SblRemittanceBusinessAddress = settings.SblRemittanceBusinessAddress,
                    SblRemittanceBusinessCity = settings.SblRemittanceBusinessCity,
                    SblRemittanceBusinessPostcode = settings.SblRemittanceBusinessPostcode,
                    //
                    AssociateName = associate.Name,
                    AssociateAddress = associate.Address,
                    AssociateCity = associate.City,
                    AssociatePostcode = associate.Postcode,
                    AssociatePhone = associate.Mobile,
                    AssociateEmail = associate.Email,
                    RemittanceDate = periodEndDate,
                    DueDate = periodEndDate.AddDays(28),
                    //
                    FromDate = focusDate,
                    //Periods = periods,
                    Routes = routes,
                    Charges = charges,
                    Subrentals = associateVechileRental,
                    Subtotals = sublist,
                    Total = total
                };

                if (viewModel == null)
                {
                    return HttpNotFound();
                }

                return View(viewModel);
            }
            return HttpNotFound();
        }
        */



        public static int CalculateDays(DateTime From, DateTime To)
        {
            DateTime from = new DateTime(From.Year, From.Month, From.Day);
            DateTime to = new DateTime(To.Year, To.Month, To.Day);
            TimeSpan span = to.Subtract(from);
            return span.Days;
        }



        public ActionResult AssociateRemittance(int? AssociateId, DateTime? Date)
        {

            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // get settings
            var settings = db.Settings.FirstOrDefault();


            // focus date
            var focusDate = Date.Value.Date;


            // week
            int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(focusDate);


            // days and period end date
            var periodEndDate = focusDate.AddDays(6);


            // get associate
            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();


            // own vehicle deduct price
            double deductOwnVechileAmount = 0;


            if (associate != null)
            {
                if (associate.OwnVehicle)
                {
                    deductOwnVechileAmount = settings.OwnVehicleDeductionPrice;
                }

                
                // get routes
                var getroutes = db.RouteAllocations.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && x.Active == true && x.Deleted == false);

                // routes
                var routes = (from x in getroutes
                               select new AssociateRemittanceViewModel.Route
                               {
                                   Id = x.Id,
                                   RouteDate = x.RouteDate,
                                   //RouteType1 = x.RouteType + (x.RouteType2 != "0" ? " / " + x.RouteType2 : "") + (x.RouteType3 != "0" ? " / " + x.RouteType3 : ""),
                                   //RouteCode = x.RouteCode + (x.RouteCode2 != "0" ? " / " + x.RouteCode2 : "") + (x.RouteCode3 != "0" ? " / " + x.RouteCode3 : ""),
                                   RouteType1 = x.RouteType1 == "SupportAd1" || 
                                                x.RouteType1 == "SupportAd2" || 
                                                x.RouteType1 == "SupportAd3" ? "Support" : x.RouteType1,
                                   RouteCode1 = x.RouteCode1,
                                   RouteExtra = (x.RouteType2 != "0" ? 
                                                    x.RouteType2 == "SupportAd1" ||
                                                    x.RouteType2 == "SupportAd2" ||
                                                    x.RouteType2 == "SupportAd3" ? "Support" : x.RouteType2 : "") + 
                                                (x.RouteType3 != "0" ? " / " + 
                                                    x.RouteType3 == "SupportAd1" ||
                                                    x.RouteType3 == "SupportAd2" ||
                                                    x.RouteType3 == "SupportAd3" ? "Support" : x.RouteType3 : "") +
                                                (x.RouteType4 != "0" ? " / " +
                                                    x.RouteType4 == "SupportAd1" ||
                                                    x.RouteType4 == "SupportAd2" ||
                                                    x.RouteType4 == "SupportAd3" ? "Support" : x.RouteType4 : "") +
                                                (x.RouteType5 != "0" ? " / " +
                                                    x.RouteType5 == "SupportAd1" ||
                                                    x.RouteType5 == "SupportAd2" ||
                                                    x.RouteType5 == "SupportAd3" ? "Support" : x.RouteType5 : ""),
                                   //
                                   //RouteExtra = (x.RouteType2 != "0" ? x.RouteType2 + " (" + x.RouteCode2 + ")" : "") + (x.RouteType3 != "0" ? " / " + x.RouteType3 + " (" + x.RouteCode3 + ")" : ""),
                                   //Depot = x.Associate.Depot.Name,
                                   Depot = x.Depot.Name,
                                   Mileage = x.Mileage,
                                   Byod = x.BYODPrice,
                                   //RouteRate = x.RoutePrice1 + x.RoutePrice2 + x.RoutePrice3,
                                   RouteRate = x.RoutePrice1,
                                   RouteExtraRate = x.RoutePrice2 + x.RoutePrice3,
                                   FuelSupport = x.Mileage * settings.Mileage1MileSBL,
                                   SubTotal = (x.BYODPrice + x.RoutePrice1 + x.RoutePrice2 + x.RoutePrice3 + (x.Mileage * settings.Mileage1MileSBL)),
                                   //
                                   AllocationStatus = x.AllocationStatus,
                                   AuthPayroll = x.AuthPayroll,
                                   AuthPoc = x.AuthPoc,
                                   AuthAdmin = x.AuthAdmin,
                               }).OrderBy(x => x.RouteDate).ToList();


                // get charges
                var getcharges = db.Charges.Where(x => x.AssociateId == associate.Id && x.Date >= focusDate && x.Date <= periodEndDate && x.Active == true && x.Deleted == false);

                // credits
                var credits = (from x in getcharges
                               where x.SetAsCredit == true
                               select new AssociateRemittanceViewModel.Credit
                               {
                                   CreditDate = x.Date,
                                   Description = x.Description,
                                   CreditAmount = x.Amount
                               }).ToList();


                // deductions
                var deductions = (from x in getcharges
                                  where x.SetAsCredit == false
                                  select new AssociateRemittanceViewModel.Deduction
                                  {
                                      DeductionDate = x.Date,
                                      Description = x.Description,
                                      DeductionAmount = x.Amount
                                  }).ToList();


                // subrentals

                var holidays = getroutes.Where(x => x.AllocationStatus == "HOLIDAY").ToList();

                var getsubrentals = db.SubRentals.Where(x =>
                    x.AssociateId == associate.Id &&
                    x.DateRented <= periodEndDate &&
                    //((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (!x.DateReturned.HasValue)) &&
                    ((x.DateReturned >= focusDate) || (!x.DateReturned.HasValue)) &&
                    x.Active == true &&
                    x.Deleted == false);

                var subrentals1 = (from x in getsubrentals
                                   select new AssociateRemittanceViewModel.SubRental
                                   {
                                       DateRented = x.DateRented,
                                       DateReturned = x.DateReturned,
                                       VanName = x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                       VanRentalDays = 0,
                                       //
                                       VanRentalDescription = "",
                                       VanRentalPrice = x.VanRentalPrice,
                                       InsuranceDescription = "",
                                       InsurancePrice = x.InsurancePrice,
                                       GoodsInTransitDescription = "",
                                       GoodsInTransitPrice = x.GoodsInTransitPrice,
                                       PublicLiabilityDescription = "",
                                       PublicLiabilityPrice = x.PublicLiabilityPrice,
                                       SubRentalAmount = x.RentalPrice
                                   }).AsEnumerable().Select(s => new AssociateRemittanceViewModel.SubRental
                                   {
                                       DateRented = s.DateRented,
                                       DateReturned = s.DateReturned,
                                       VanName = s.VanName,
                                       VanRentalDays = s.DateRented < focusDate && !s.DateReturned.HasValue ? (periodEndDate - focusDate).Days + 1 - (holidays.Where(y => y.RouteDate >= focusDate && y.RouteDate <= periodEndDate).Count()) :
                                                       s.DateRented >= focusDate && !s.DateReturned.HasValue ? (periodEndDate - s.DateRented).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= s.DateRented && y.RouteDate <= periodEndDate).Count()) :
                                                       s.DateRented < focusDate && s.DateReturned.HasValue ? s.DateReturned <= periodEndDate ? (s.DateReturned - focusDate).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= focusDate && y.RouteDate <= s.DateReturned).Count()) : (periodEndDate - focusDate).Days + 1 - (holidays.Where(y => y.RouteDate >= focusDate && y.RouteDate <= periodEndDate).Count()) :
                                                       s.DateRented >= focusDate && s.DateReturned.HasValue ? s.DateReturned <= periodEndDate ? (s.DateReturned - s.DateRented).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= s.DateRented && y.RouteDate <= s.DateReturned).Count()) : (periodEndDate - s.DateRented).Value.Days + 1 - (holidays.Where(y => y.RouteDate >= s.DateRented && y.RouteDate <= periodEndDate).Count()) : 0,
                                       //
                                       VanRentalDescription = "Van Rental: " + s.VanName,
                                       VanRentalPrice = s.VanRentalPrice,
                                       InsuranceDescription = "Insurance: " + s.VanName,
                                       InsurancePrice = s.InsurancePrice,
                                       GoodsInTransitDescription = "Goods in Transit: " + s.VanName,
                                       GoodsInTransitPrice = s.GoodsInTransitPrice,
                                       PublicLiabilityDescription = "Public Liability: " + s.VanName,
                                       PublicLiabilityPrice = s.PublicLiabilityPrice,
                                       SubRentalAmount = s.SubRentalAmount
                                   }).ToList();


                var subrentals = (from x in subrentals1
                                  select new AssociateRemittanceViewModel.SubRental
                                  {
                                      DateRented = x.DateRented,
                                      DateReturned = x.DateReturned,
                                      VanName = x.VanName,
                                      VanRentalDays = x.VanRentalDays,
                                      //
                                      VanRentalDescription = x.VanRentalDescription,
                                      VanRentalPrice = x.VanRentalPrice,
                                      InsuranceDescription = x.InsuranceDescription,
                                      InsurancePrice = x.InsurancePrice,
                                      GoodsInTransitDescription = x.GoodsInTransitDescription,
                                      GoodsInTransitPrice = x.GoodsInTransitPrice,
                                      PublicLiabilityDescription = x.PublicLiabilityDescription,
                                      PublicLiabilityPrice = x.PublicLiabilityPrice,
                                      SubRentalAmount = x.SubRentalAmount
                                  }).AsEnumerable().Select(s => new AssociateRemittanceViewModel.SubRental
                                  {
                                      DateRented = s.DateRented,
                                      DateReturned = s.DateReturned,
                                      VanName = s.VanName,
                                      VanRentalDays = s.VanRentalDays,
                                      //
                                      VanRentalDescription = s.VanRentalDescription,
                                      VanRentalPrice = ((s.VanRentalPrice / 7) * s.VanRentalDays),
                                      InsuranceDescription = s.InsuranceDescription,
                                      InsurancePrice = ((s.InsurancePrice / 7) * s.VanRentalDays),
                                      GoodsInTransitDescription = s.GoodsInTransitDescription,
                                      GoodsInTransitPrice = ((s.GoodsInTransitPrice / 7) * s.VanRentalDays),
                                      PublicLiabilityDescription = s.PublicLiabilityDescription,
                                      PublicLiabilityPrice = ((s.PublicLiabilityPrice / 7) * s.VanRentalDays),
                                      SubRentalAmount = ((s.SubRentalAmount / 7) * s.VanRentalDays),
                                  }).ToList();




                /*

                 
                int vanRentalDays = getroutes.Where(x => x.AllocationStatus != "HOLIDAY").Count();
                var getsubrentals = db.SubRentals.Where(x => x.AssociateId == associate.Id && ((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (!x.DateReturned.HasValue)) && x.Active == true && x.Deleted == false);

                var subrentals1 = new List<AssociateRemittanceViewModel.SubRental>();
                foreach (var x in getsubrentals)
                {
                    subrentals1.Add(new AssociateRemittanceViewModel.SubRental
                    {
                        VanRentalDays = vanRentalDays,
                        VanRentalDescription = "Van Rental: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                        VanRentalPrice = ((x.VanRentalPrice / 7) * vanRentalDays),
                        InsuranceDescription = "Insurance: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                        InsurancePrice = ((x.InsurancePrice / 7) * vanRentalDays),
                        GoodsInTransitDescription = "Goods in Transit: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                        GoodsInTransitPrice = ((x.GoodsInTransitPrice / 7) * vanRentalDays),
                        PublicLiabilityDescription = "Public Liability: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                        PublicLiabilityPrice = ((x.PublicLiabilityPrice / 7) * vanRentalDays),
                        SubRentalAmount = ((x.RentalPrice / 7) * vanRentalDays)
                    });
                }

                var subrentals = (from x in subrentals1.ToList()
                                  select new AssociateRemittanceViewModel.SubRental
                                  {
                                      VanRentalDays = x.VanRentalDays,
                                      VanRentalDescription = x.VanRentalDescription,
                                      VanRentalPrice = x.VanRentalPrice,
                                      InsuranceDescription = x.InsuranceDescription,
                                      InsurancePrice = x.InsurancePrice,
                                      GoodsInTransitDescription = x.GoodsInTransitDescription,
                                      GoodsInTransitPrice = x.GoodsInTransitPrice,
                                      PublicLiabilityDescription = x.PublicLiabilityDescription,
                                      PublicLiabilityPrice = x.PublicLiabilityPrice,
                                      SubRentalAmount = x.SubRentalAmount
                                  }).AsEnumerable().Select(s => new AssociateRemittanceViewModel.SubRental
                                  {
                                      VanRentalDays = s.VanRentalDays,
                                      VanRentalDescription = s.VanRentalDescription,
                                      VanRentalPrice = s.VanRentalPrice,
                                      InsuranceDescription = s.InsuranceDescription,
                                      InsurancePrice = s.InsurancePrice,
                                      GoodsInTransitDescription = s.GoodsInTransitDescription,
                                      GoodsInTransitPrice = s.GoodsInTransitPrice,
                                      PublicLiabilityDescription = s.PublicLiabilityDescription,
                                      PublicLiabilityPrice = s.PublicLiabilityPrice,
                                      SubRentalAmount = s.SubRentalAmount
                                  }).ToList();




                
                int vanRentalDays = getroutes.Where(x => x.AllocationStatus != "HOLIDAY").Count();

                var getsubrentals = db.SubRentals.Where(x => x.AssociateId == associate.Id && ((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (!x.DateReturned.HasValue)) && x.Active == true && x.Deleted == false);

                var subrentals = (from x in getsubrentals
                                  select new AssociateRemittanceViewModel.SubRental
                                  {
                                      VanRentalDays = vanRentalDays,
                                      VanRentalDescription = "Van Rental: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      VanRentalPrice = ((x.VanRentalPrice / 7) * vanRentalDays),
                                      InsuranceDescription = "Insurance: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      InsurancePrice = ((x.InsurancePrice / 7) * vanRentalDays),
                                      GoodsInTransitDescription = "Goods in Transit: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      GoodsInTransitPrice = ((x.GoodsInTransitPrice / 7) * vanRentalDays),
                                      PublicLiabilityDescription = "Public Liability: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      PublicLiabilityPrice = ((x.PublicLiabilityPrice / 7) * vanRentalDays),
                                      SubRentalAmount = ((x.RentalPrice / 7) * vanRentalDays)
                                  }).ToList();


                 


                 


            var getsubrentals = db.SubRentals.Where(x => x.AssociateId == associate.Id && ((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (!x.DateReturned.HasValue)) && x.Active == true && x.Deleted == false);
                
                var subrentals = new List<AssociateRemittanceViewModel.SubRental>();

                foreach (var rental in getsubrentals)
                {
                    string vanName = rental.Vehicle.Make + " - " + rental.Vehicle.Model + " - " + rental.Vehicle.Registration;
                    string rentalDates = "";
                    int rentalDays = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        DateTime checkDate = focusDate.AddDays(i);
                        if (getroutes.Where(x => x.RouteDate == checkDate && x.AllocationStatus == "HOLIDAY").Count() == 0)
                        {
                            if ((rental.DateRented <= checkDate && !rental.DateReturned.HasValue) || rental.DateReturned == checkDate)
                            {
                                rentalDates = rentalDates + String.Format("{0:dd/MM} ", checkDate.Date);
                                rentalDays = rentalDays + 1;
                            }
                        }
                    }

                    subrentals.Add(new AssociateRemittanceViewModel.SubRental
                    {
                        VanRentalDates = rentalDates,
                        VanRentalDays = rentalDays,
                        VanRentalDescription = "Van Rental: " + vanName,
                        VanRentalPrice = ((rental.VanRentalPrice / 7) * rentalDays),
                        InsuranceDescription = "Insurance: " + vanName,
                        InsurancePrice = ((rental.InsurancePrice / 7) * rentalDays),
                        GoodsInTransitDescription = "Goods in Transit: " + vanName,
                        GoodsInTransitPrice = ((rental.GoodsInTransitPrice / 7) * rentalDays),
                        PublicLiabilityDescription = "Public Liability: " + vanName,
                        PublicLiabilityPrice = ((rental.PublicLiabilityPrice / 7) * rentalDays),
                        SubRentalAmount = ((rental.RentalPrice / 7) * rentalDays)
                    });
                }

                subrentals = subrentals.ToList();




                //int vanRentalDays = getroutes.Where(x => x.AllocationStatus != "HOLIDAY").Count();
                //var subRentals = associate.SubRentals.Where(x => ((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (x.DateReturned == null)) && x.Active && x.Deleted == false).ToList();
                var subrentals2 = (from x in getsubrentals
                                  select new AssociateRemittanceViewModel.SubRental
                                  {
                                      VanRentalDays = vanRentalDays,
                                      VanRentalDescription = "Van Rental: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      VanRentalPrice = ((x.VanRentalPrice / 7) * vanRentalDays),
                                      InsuranceDescription = "Insurance: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      InsurancePrice = ((x.InsurancePrice / 7) * vanRentalDays),
                                      GoodsInTransitDescription = "Goods in Transit: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      GoodsInTransitPrice = ((x.GoodsInTransitPrice / 7) * vanRentalDays),
                                      PublicLiabilityDescription = "Public Liability: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      PublicLiabilityPrice = ((x.PublicLiabilityPrice / 7) * vanRentalDays),
                                      SubRentalAmount = ((x.RentalPrice / 7) * vanRentalDays)
                                  }).ToList();
                */


                // extra deductions
                var extradeductions = (from x in getroutes
                                       where x.FuelChargePrice > 0
                                       select new AssociateRemittanceViewModel.ExtraDeduction
                                       {
                                           DeductionDate = x.RouteDate,
                                           Description = "Fuel Charge",
                                           DeductionAmount = x.FuelChargePrice
                                       }).ToList();



                // charge claims
                List<AssociateRemittanceViewModel.ChargeClaim> chargeclaims = new List<AssociateRemittanceViewModel.ChargeClaim>();
                //var getchargeclaims = db.ChargeClaims.Where(x => x.AssociateId == associate.Id && x.DateFirstInstalment <= focusDate && x.DateLastInstalment <= periodEndDate && x.Active == true && x.Deleted == false);
                var getchargeclaims = db.ChargeClaims.Where(x => x.AssociateId == associate.Id && x.Active == true && x.Deleted == false);
                foreach (var charge in getchargeclaims)
                {
                    int instalments = charge.NumberOfInstalments;
                    double amount = charge.Amount / charge.NumberOfInstalments;
                    for (int i = 0; i < instalments; i++)
                    {
                        int weekdate = 7 * charge.WeekFrequency * i;
                        string description = charge.Description + " (" + (charge.WeekFrequency == 1 ? "Every " + charge.WeekFrequency + " week" : "Every " + charge.WeekFrequency + " weeks") + ": " + (i + 1) + "/" + charge.NumberOfInstalments + ")";
                        DateTime instalmentdate = charge.DateFirstInstalment.AddDays(weekdate);
                        if (instalmentdate >= focusDate && instalmentdate <= periodEndDate)
                        {
                            chargeclaims.Add(new AssociateRemittanceViewModel.ChargeClaim
                            {
                                Id = charge.Id,
                                ChargeClaimDate = instalmentdate,
                                ChargeClaimAmount = amount,
                                Description = description
                            });
                        }
                    }
                }


                // charge pcn
                List<AssociateRemittanceViewModel.ChargePcn> chargepcns = new List<AssociateRemittanceViewModel.ChargePcn>();
                var getchargepcns = db.ChargePcns.Where(x => x.AssociateId == associate.Id && x.Active == true && x.Deleted == false);
                foreach (var charge in getchargepcns)
                {
                    int instalments = charge.NumberOfInstalments;
                    double amount = (charge.PcnFee + charge.AdminFee) / charge.NumberOfInstalments;
                    for (int i = 0; i < instalments; i++)
                    {
                        int weekdate = 7 * charge.WeekFrequency * i;
                        string description = charge.Description + " (" + (charge.WeekFrequency == 1 ? "Every " + charge.WeekFrequency + " week" : "Every " + charge.WeekFrequency + " weeks") + ": " + (i + 1) + "/" + charge.NumberOfInstalments + ")";
                        DateTime instalmentdate = charge.DateFirstInstalment.AddDays(weekdate);
                        if (instalmentdate >= focusDate && instalmentdate <= periodEndDate)
                        {
                            chargepcns.Add(new AssociateRemittanceViewModel.ChargePcn
                            {
                                Id = charge.Id,
                                ChargePcnDate = instalmentdate,
                                ChargePcnAmount = amount,
                                Description = description
                            });
                        }
                    }
                }


                //
                var sumMileage = routes.Sum(x => x.Mileage);
                var sumByod = routes.Sum(x => x.Byod);
                var sumRouteRate = routes.Sum(x => x.RouteRate);
                var sumRouteExtraRate = routes.Sum(x => x.RouteExtraRate);
                var sumFuelSupport = routes.Sum(x => x.FuelSupport);
                //
                var sumSubTotal = routes.Sum(x => x.SubTotal);
                var sumCreditAmount = credits.Sum(x => x.CreditAmount);
                //
                var sumDeductionAmount = deductions.Sum(x => x.DeductionAmount);
                var sumSubRentalAmount = subrentals.Sum(x => x.SubRentalAmount);
                var sumExtraDeductionAmount = extradeductions.Sum(x => x.DeductionAmount);
                var sumChargeClaimAmount = chargeclaims.Sum(x => x.ChargeClaimAmount);
                var sumChargePcnAmount = chargepcns.Sum(x => x.ChargePcnAmount);
                //
                var totalCredits = sumSubTotal + sumCreditAmount;
                var totalDeductions = sumDeductionAmount + sumSubRentalAmount + sumExtraDeductionAmount + sumChargeClaimAmount + sumChargePcnAmount;
                //
                var total = totalCredits - totalDeductions;

                var viewModel = new AssociateRemittanceViewModel
                {
                    AssociateId = associate.Id,
                    WeekNumber = week,
                    //
                    SblRemittanceBusinessName = settings.SblRemittanceBusinessName,
                    SblRemittanceBusinessVatNumber = settings.SblRemittanceBusinessVatNumber,
                    SblRemittanceThankYouMessage = settings.SblRemittanceThankYouMessage,
                    SblRemittanceBusinessAddress = settings.SblRemittanceBusinessAddress,
                    SblRemittanceBusinessCity = settings.SblRemittanceBusinessCity,
                    SblRemittanceBusinessPostcode = settings.SblRemittanceBusinessPostcode,
                    //
                    AssociateName = associate.Name,
                    AssociateAddress = associate.Address,
                    AssociateCity = associate.City,
                    AssociatePostcode = associate.Postcode,
                    AssociatePhone = associate.Mobile,
                    AssociateEmail = associate.Email,
                    RemittanceDate = periodEndDate,
                    DueDate = periodEndDate.AddDays(28),
                    FromDate = focusDate,
                    //
                    SumMileage = sumMileage,
                    SumByod = sumByod,
                    SumRouteRate = sumRouteRate,
                    SumRouteExtraRate = sumRouteExtraRate,
                    SumFuelSupport = sumFuelSupport,
                    SumSubTotal = sumSubTotal,
                    Routes = routes,
                    //
                    SumCreditAmount = sumCreditAmount,
                    Credits = credits,
                    //
                    SumDeductionAmount = sumDeductionAmount,
                    Deductions = deductions,
                    //
                    SumSubRentalAmount = sumSubRentalAmount,
                    SubRentals = subrentals,
                    //
                    SumExtraDeductionAmount = sumExtraDeductionAmount,
                    ExtraDeductions = extradeductions,
                    //
                    SumChargeClaimAmount = sumChargeClaimAmount,
                    ChargeClaims = chargeclaims,
                    //
                    SumChargePcnAmount = sumChargePcnAmount,
                    ChargePcns = chargepcns,
                    //
                    TotalDeductions = totalDeductions,
                    TotalCredits = totalCredits,
                    Total = total,
                    //
                };

                if (viewModel == null)
                {
                    return HttpNotFound();
                }

                return View(viewModel);
            }

            return HttpNotFound();
        }



        public JsonResult SendEmailReceipt(int AssociateId, DateTime? DateStart)
        {
            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();

            bool isvalid = false;

            if (associate != null)
            {
                var settings = db.Settings.FirstOrDefault();

                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(DateStart.Value);

                // create pdf
                string url = "https://" + settings.DomainName + "/pdf/AssociateReceipt?associateid=" + associate.Id + "&date=" + String.Format("{0:yyyy-MM-dd}", DateStart);
                HtmlToPdf converter = new HtmlToPdf();

                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                converter.Options.WebPageWidth = 1350;
                //converter.Options.WebPageHeight = 0;
                converter.Options.WebPageFixedSize = false;
                converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;

                PdfDocument doc = converter.ConvertUrl(url);
                HtmlToPdfResult result = converter.ConversionResult;
                doc.DocumentInformation.Title = result.WebPageInformation.Title;
                doc.DocumentInformation.Subject = result.WebPageInformation.Description;
                doc.DocumentInformation.Keywords = result.WebPageInformation.Keywords;
                doc.DocumentInformation.Author = "SBL Couriers";
                doc.DocumentInformation.CreationDate = DateTime.Now;

                doc.ViewerPreferences.HideWindowUI = true;
                doc.ViewerPreferences.CenterWindow = true;

                byte[] pdfReceipt = doc.Save();
                doc.Close();

                // save in database
                AssociateReceipt receipt = new AssociateReceipt();
                receipt.AssociateId = associate.Id;
                receipt.Week = week;
                receipt.WeekStartDate = DateStart;
                receipt.IsSent = true;
                receipt.DateSent = DateTime.Now;
                receipt.DataFile = pdfReceipt;
                receipt.DataFileContentType = "application/pdf";
                receipt.DataFileName = "receipt.pdf";
                receipt.DataFileExtension = ".pdf";
                receipt.DateCreated = DateTime.Now;
                receipt.Active = true;
                receipt.Deleted = false;
                db.AssociateReceipts.Add(receipt);
                db.SaveChanges();

                // send email
                int days = 6;
                string subject = Functions.GetAssociateReceiptEmailSubject(AssociateId, DateStart, days);
                string body = Functions.GetAssociateReceiptEmailBody(AssociateId, DateStart, days);
                isvalid = EmailSender.SendEmailReceiptToAssociate(associate.Email, subject, body);
            }

            return Json(new
            {
                isvalid = isvalid
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SendEmailRemittance(int AssociateId, DateTime? DateStart)
        {
            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();

            bool isvalid = false;

            if (associate != null)
            {
                var settings = db.Settings.FirstOrDefault();

                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(DateStart.Value);

                // create pdf
                string url = "https://" + settings.DomainName + "/pdf/AssociateRemittance?associateid=" + associate.Id + "&date=" + String.Format("{0:yyyy-MM-dd}", DateStart);
                HtmlToPdf converter = new HtmlToPdf();

                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                converter.Options.WebPageWidth = 1350;
                //converter.Options.WebPageHeight = 0;
                converter.Options.WebPageFixedSize = false;
                converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;

                PdfDocument doc = converter.ConvertUrl(url);
                HtmlToPdfResult result = converter.ConversionResult;
                doc.DocumentInformation.Title = result.WebPageInformation.Title;
                doc.DocumentInformation.Subject = result.WebPageInformation.Description;
                doc.DocumentInformation.Keywords = result.WebPageInformation.Keywords;
                doc.DocumentInformation.Author = "SBL Couriers";
                doc.DocumentInformation.CreationDate = DateTime.Now;

                doc.ViewerPreferences.HideWindowUI = true;
                doc.ViewerPreferences.CenterWindow = true;

                byte[] pdfRemittance = doc.Save();
                doc.Close();

                // save in database
                AssociateRemittance newremittance = new AssociateRemittance();
                newremittance.AssociateId = associate.Id;
                newremittance.Week = week;
                newremittance.WeekStartDate = DateStart;
                newremittance.IsSent = true;
                newremittance.DateSent = DateTime.Now;
                newremittance.DataFile = pdfRemittance;
                newremittance.DataFileContentType = "application/pdf";
                newremittance.DataFileName = "remittance.pdf";
                newremittance.DataFileExtension = ".pdf";
                newremittance.DateCreated = DateTime.Now;
                newremittance.Active = true;
                newremittance.Deleted = false;
                db.AssociateRemittances.Add(newremittance);
                db.SaveChanges();

                // send email
                int days = 6;
                string subject = Functions.GetAssociateRemittanceEmailSubject(AssociateId, DateStart, days);
                string body = Functions.GetAssociateRemittanceEmailBody(AssociateId, DateStart, days);
                isvalid = EmailSender.SendEmailRemittanceToAssociate(newremittance.Id, associate.Email, subject, body);

            }

            return Json(new
            {
                isvalid = isvalid
            }, JsonRequestBehavior.AllowGet);
        }



        #endregion





        #region UpdateAuthLocks
        public JsonResult UpdateAuthPoc(int associateId, DateTime? date)
        {
            DateTime weekStartDate = date.Value;
            DateTime weekEndDate = date.Value.AddDays(6);
            var routeAllocations =
               db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false && x.AssociateId == associateId
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false && x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();
            if (routeAllocations.Count > 0)
            {
                bool isUpdateAuthPoc = false;
                var userid = User.Identity.GetUserId();

                if (User.IsInRole(WebConstant.SBLUserRole.POC))
                {
                    isUpdateAuthPoc = true;
                }

                if (isUpdateAuthPoc)
                {
                    foreach (var routeAllocation in routeAllocations)
                    {

                        if (isUpdateAuthPoc)
                        {
                            routeAllocation.AuthPoc = true;
                        }

                        db.RouteAllocations.Attach(routeAllocation);

                        if (isUpdateAuthPoc)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthPoc).IsModified = true;
                        }
                    }

                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAuthPayroll(int associateId, DateTime? date)
        {
            DateTime weekStartDate = date.Value;
            DateTime weekEndDate = date.Value.AddDays(6);
            var routeAllocations =
               db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false && x.AssociateId == associateId
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false && x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();
            if (routeAllocations.Count > 0)
            {
                // current user
                bool isUpdateAuthPayroll = false;
                var userid = User.Identity.GetUserId();

                if (User.IsInRole(WebConstant.SBLUserRole.Payroll))
                {
                    isUpdateAuthPayroll = true;
                }

                if (isUpdateAuthPayroll)
                {
                    foreach (var routeAllocation in routeAllocations)
                    {

                        if (isUpdateAuthPayroll)
                        {
                            routeAllocation.AuthPayroll = true;
                        }

                        db.RouteAllocations.Attach(routeAllocation);

                        if (isUpdateAuthPayroll)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthPayroll).IsModified = true;
                        }
                    }

                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAuthAdmin(int associateId, DateTime? date)
        {
            DateTime weekStartDate = date.Value;
            DateTime weekEndDate = date.Value.AddDays(6);
            var routeAllocations =
               db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false && x.AssociateId == associateId
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false && x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();
            if (routeAllocations.Count > 0)
            {
                // current user
                bool isUpdateAuthAdmin = false;
                var userid = User.Identity.GetUserId();

                if (User.IsInRole(WebConstant.SBLUserRole.Admin))
                {
                    isUpdateAuthAdmin = true;
                }

                if (isUpdateAuthAdmin)
                {
                    foreach (var routeAllocation in routeAllocations)
                    {

                        if (isUpdateAuthAdmin)
                        {
                            routeAllocation.AuthAdmin = true;
                        }

                        db.RouteAllocations.Attach(routeAllocation);

                        if (isUpdateAuthAdmin)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthAdmin).IsModified = true;
                        }
                    }

                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAuthPocRevert(int associateId, DateTime? date)
        {
            DateTime weekStartDate = date.Value;
            DateTime weekEndDate = date.Value.AddDays(6);
            var routeAllocations =
               db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false && x.AssociateId == associateId
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false && x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();
            if (routeAllocations.Count > 0)
            {
                bool isUpdateAuthPoc = false;
                var userid = User.Identity.GetUserId();

                if (User.IsInRole(WebConstant.SBLUserRole.Admin))
                {
                    isUpdateAuthPoc = true;
                }

                if (isUpdateAuthPoc)
                {
                    foreach (var routeAllocation in routeAllocations)
                    {

                        if (isUpdateAuthPoc)
                        {
                            routeAllocation.AuthPoc = false;
                        }

                        db.RouteAllocations.Attach(routeAllocation);

                        if (isUpdateAuthPoc)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthPoc).IsModified = true;
                        }
                    }

                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAuthPayrollRevert(int associateId, DateTime? date)
        {
            DateTime weekStartDate = date.Value;
            DateTime weekEndDate = date.Value.AddDays(6);
            var routeAllocations =
               db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false && x.AssociateId == associateId
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false && x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();
            if (routeAllocations.Count > 0)
            {
                // current user
                bool isUpdateAuthPayroll = false;
                var userid = User.Identity.GetUserId();

                if (User.IsInRole(WebConstant.SBLUserRole.Admin))
                {
                    isUpdateAuthPayroll = true;
                }

                if (isUpdateAuthPayroll)
                {
                    foreach (var routeAllocation in routeAllocations)
                    {

                        if (isUpdateAuthPayroll)
                        {
                            routeAllocation.AuthPayroll = false;
                        }

                        db.RouteAllocations.Attach(routeAllocation);

                        if (isUpdateAuthPayroll)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthPayroll).IsModified = true;
                        }
                    }

                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAuthAdminRevert(int associateId, DateTime? date)
        {
            DateTime weekStartDate = date.Value;
            DateTime weekEndDate = date.Value.AddDays(6);
            var routeAllocations =
               db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false && x.AssociateId == associateId
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false && x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();
            if (routeAllocations.Count > 0)
            {
                // current user
                bool isUpdateAuthAdmin = false;
                var userid = User.Identity.GetUserId();

                if (User.IsInRole(WebConstant.SBLUserRole.Admin))
                {
                    isUpdateAuthAdmin = true;
                }

                if (isUpdateAuthAdmin)
                {
                    foreach (var routeAllocation in routeAllocations)
                    {

                        if (isUpdateAuthAdmin)
                        {
                            routeAllocation.AuthAdmin = false;
                        }

                        db.RouteAllocations.Attach(routeAllocation);

                        if (isUpdateAuthAdmin)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthAdmin).IsModified = true;
                        }
                    }

                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }
        #endregion





        #region SendEmailToAssociate


        public JsonResult SendEmailToAllAssociates(int? DepotId, DateTime? DateStart)
        {
            var associates = db.Associates.Where(x => x.DepotId == DepotId  && x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false);

            foreach (var associate in associates)
            {
                bool isEmailSent = false;
                if (associate != null)
                {
                    var routeAllocations = db.RouteAllocations.Where(x => x.Active && x.Deleted == false && x.Associate.Active && x.Associate.Deleted == false && x.AssociateId.Value == associate.Id).OrderBy(x => x.RouteDate).ToList();

                    DateTime? weekStartDate = null;
                    DateTime? weekEndDate = null;
                    if (DateStart.HasValue)
                    {
                        weekStartDate = DateStart;
                        weekEndDate = DateStart.Value.AddDays(6);
                        routeAllocations = routeAllocations.Where(x => x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate).ToList();
                    }
                    StringBuilder weekelyRemittanceDetail = new StringBuilder();
                    string period = String.Format("{0:dd MMM yyyy} - {1:dd MMM yyyy}", weekStartDate, weekEndDate);
                    foreach (var route in routeAllocations)
                    {
                        var routedate = "N/A";
                        if (route.RouteDate != null)
                        {
                            routedate = route.RouteDate.Value.ToString("dd/MM/yyy");
                        }
                        weekelyRemittanceDetail.AppendFormat(
                            "<tr style='padding: 10px; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; color: #555555; font-weight: 400;' bgcolor='#f9f9f9'><td class='text-left' width='90' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{0}</td><td class='text-left' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{1}</td><td class='text-left' width='40' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{2}</td><td class='text-left' width='60' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{3}</td><td class='text-left' width='50' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{4}</td><td class='text-left' width='50' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{5}</td><td class='text-left' width='50' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{6}</td><td class='text-left' width='50' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{7}</td><td class='text-left' width='50' style='border: 1px solid #dddddd; padding: 10px; mso-table-lspace: 0 !important; mso-table-rspace: 0 !important; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' align='left'>{8}</td></tr>",
                            routedate, route.RouteType1, route.RouteCode1, route.Ad1Quantity,
                            route.Ad2Quantity, route.Ad3Quantity, route.Mileage, route.Fuel, route.Deduct);
                    }
                    string emailSubject = "SBL: Weekely Remittance Detail - " + period;
                    var emailbody = EmailTemplate.PrepareMailBodyWith("WeekelyRemittance.html",
                          WebConstant.EmailTemplate.EmailKey.WebSiteUrl, Utility.WebSiteUrl,
                          WebConstant.EmailTemplate.EmailKey.WeekelyRemittanceDetail, weekelyRemittanceDetail.ToString(),
                          WebConstant.EmailTemplate.EmailKey.Name, associate.Name,
                          WebConstant.EmailTemplate.EmailKey.Email, associate.Email,
                          WebConstant.EmailTemplate.EmailKey.WeekDateRange, period,
                          WebConstant.EmailTemplate.EmailKey.WeekNumber, ""
                          );
                    //isEmailSent = EmailSender.SendEmailReceiptToAssociate(associate.Email, emailSubject, emailbody);
                }
            }

            return Json(new
            {
                //IsEmailSent = isEmailSent
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion





        #region Schedule




        public ActionResult Schedule(DateTime? Date, int? Days, int? DepotId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // get settings
            var settings = db.Settings.FirstOrDefault();


            // poc depot
            string pocDepot = "";
            if (User.IsInRole("poc"))
            {
                pocDepot = currentuser.Depot.Name;
            }


            // period list
            var periodroutes = db.RouteAllocations.Where(x => x.Active == true && x.Deleted == false).ToList();

            DateTime dateNow = DateTime.Now.Date;
            DateTime firstDate = DateTime.Now.Date;
            DateTime lastDate = DateTime.Now.Date;

            if (periodroutes.Count() > 0)
            {
                firstDate = periodroutes.OrderBy(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (firstDate > DateTime.Now.Date)
                {
                    firstDate = DateTime.Now.Date.AddDays(-7);
                }

                lastDate = periodroutes.OrderByDescending(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (lastDate < DateTime.Now.Date)
                {
                    lastDate = DateTime.Now.Date.AddDays(7);
                }
            }

            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }

            // period list
            List<ScheduleViewModel.Period> periodList = new List<ScheduleViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new ScheduleViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate = DateTime.Now.Date;
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // get focus sunday
            if (focusDate.DayOfWeek != DayOfWeek.Sunday)
            {
                while (focusDate.DayOfWeek != DayOfWeek.Sunday)
                    focusDate = focusDate.AddDays(-1);
            }


            // days and period end date
            var periodDays = 28;
            if (Days.HasValue)
            {
                periodDays = Days.Value;
            }

            var periodEndDate = focusDate.AddDays(periodDays - 1);


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          orderby x.Name ascending
                          select new ScheduleViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();


            // depotid
            var depotId = 0;
            if (currentuser.DepotId.HasValue)
            {
                depotId = currentuser.DepotId.Value;
            }
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }

            // loop 
            var associates = db.Associates.Where(x => x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList();


            /*
            if (associates.Where(x => x.DepotId == depotId).Any())
            {
                associates = associates.Where(x => x.DepotId == depotId).ToList();
            }
            */







            int? activedrivers = associates.Where(x => x.DepotId == DepotId && x.Active == true).Count();

            //var routes = db.RouteAllocations.Where(x => x.DepotId == depotId && x.RouteDate.Value >= focusDate && x.RouteDate.Value <= periodEndDate && x.Active == true && x.Deleted == false).ToList();
            var routes = db.RouteAllocations.Where(x => x.RouteDate.Value >= focusDate && x.RouteDate.Value <= periodEndDate && x.Active == true && x.Deleted == false).ToList();

            List<ScheduleViewModel.Associate> associateList = new List<ScheduleViewModel.Associate>();

            //string test = "";

            foreach (var associate in associates)
            {
                List<ScheduleViewModel.Route> routeList = new List<ScheduleViewModel.Route>();

                bool showAssociate = false;

                for (int i = 0; i < periodDays; i++)
                {
                    var routeDate = focusDate.AddDays(i);

                    if (associate.DepotId == depotId ||
                        routes.Where(x => x.AssociateId == associate.Id && (x.RouteDate == dateNow.AddDays(-1) || x.RouteDate == dateNow || x.RouteDate == dateNow.AddDays(1)) && x.DepotId == depotId).Any() // show external driver only 1 day before to 1 day after working day
                        )
                    {
                        showAssociate = true;
                    }

                    if (showAssociate == true)
                    {
                        var route = routes.Where(x => x.AssociateId == associate.Id && x.RouteDate == routeDate).FirstOrDefault();
                        int routesOn = routes.Where(x => x.RouteDate == routeDate && x.DepotId == depotId && x.AllocationStatus == "ON").Count();
                        int routesOff = routes.Where(x => x.RouteDate == routeDate && x.DepotId == depotId && (x.AllocationStatus == "OFF" || x.AllocationStatus == "HOLIDAY" || x.AllocationStatus == "TRAINING")).Count();

                        if (route != null)
                        {

                            //test = test + "---1---xx-----" + i + "COUNT" + i + "-----" + associate.Id + " " + route.Id + " " + routeDate + " " + route.DepotId.Value + " " + route.Depot.Name + " " + route.AllocationStatus + " " + route.RouteType1 + " " + routesOn + " " + routesOff + " " + route.AuthPoc + " " + route.AuthPayroll + " " + route.AuthAdmin;
                            //test = test + "---1---xx-----" + i + "COUNT" + i + "-----" + associate.Id + " " + route.Id + " " + routeDate + " " + route.DepotId.Value;
                            //test = test + "---1---xx-----" + i + "COUNT" + i + "-----" + associate.Id + " " + route.Id;
                            //test = test + "---1---xx-----" + i + "COUNT" + i + "-----" + routeDate + " " + route.DepotId.Value;
                            //test = test + "---1---xx-----" + i + "COUNT" + i + "-----" + routeDate;
                            //test = test + "---1---xx-----" + i + "COUNT" + i + "-----" + route.DepotId.Value;

                            //ViewBag.Test = test;


                            /*
                            int routeDepotIdCrash = 1;
                            if (route.DepotId != null)
                            {
                                routeDepotIdCrash = route.DepotId.Value;
                            }
                              */
                                                       
                            routeList.Add(new ScheduleViewModel.Route
                            {
                                AssociateId = associate.Id,
                                RouteId = route.Id,
                                RouteDate = routeDate,
                                //RouteDepotId = route.DepotId.Value,
                                RouteDepotId = (!route.DepotId.HasValue ? 1 : route.DepotId.Value),
                                RouteDepotName = route.Depot.Name,
                                AllocationStatus = route.AllocationStatus,
                                RouteType1 = route.RouteType1,
                                RoutesOn = routesOn,
                                RoutesOff = routesOff,
                                AuthPoc = route.AuthPoc,
                                AuthPayroll = route.AuthPayroll,
                                AuthAdmin = route.AuthAdmin
                            });

                        }
                        else
                        {
                            //test = test + "---2---xx-----" + i + "COUNT" + i + "-----" + associate.Id + " " + "0" + " " + routeDate + " " + "0" + " " + "xxx" + " " + "xxx" + " " + "xxx" + " " + "0" + " " + "0" + " " + "false" + " " + "false" + " " + "false";

                            //ViewBag.Test = test;

                            routeList.Add(new ScheduleViewModel.Route
                            {
                                AssociateId = associate.Id,
                                RouteId = 0,
                                RouteDate = routeDate,
                                RouteDepotId = 0,
                                RouteDepotName = "",
                                AllocationStatus = "",
                                RouteType1 = "",
                                RoutesOn = 0,
                                RoutesOff = 0,
                                AuthPoc = false,
                                AuthPayroll = false,
                                AuthAdmin = false
                            });

                        }
                    }


                }

                if (showAssociate == true)
                {
                    associateList.Add(new ScheduleViewModel.Associate
                    {
                        AssociateId = associate.Id,
                        AssociateName = associate.Name,
                        AssociateMobile = associate.Mobile,
                        AssociateDepotId = associate.DepotId.Value,
                        AssociateDepotName = associate.Depot.Name,
                        AssociateVehicleRegistration = associate.SubRentals.Where(x => !x.DateReturned.HasValue && x.Active == true && x.Deleted == false).Any() ? associate.SubRentals.Where(x => !x.DateReturned.HasValue && x.Active == true && x.Deleted == false).OrderByDescending(o => o.DateRented).FirstOrDefault().Vehicle.Registration : "-",
                        Routes = routeList
                    });
                }

            }

            var viewModel = new ScheduleViewModel
            {
                POCDepot = pocDepot,
                DepotId = depotId,
                DateStart = focusDate,
                Days = periodDays,
                ActiveDrivers = activedrivers,
                ScheduleEditDays = settings.ScheduleEditDays,
                //
                Associates = associateList,
                Periods = periodList,
                Depots = depots
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }




        public ActionResult Schedule2(DateTime? Date, int? Days, int? DepotId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // get settings
            var settings = db.Settings.FirstOrDefault();


            // poc depot
            string pocDepot = "";
            if (User.IsInRole("poc"))
            {
                pocDepot = currentuser.Depot.Name;
            }


            // period list
            var periodroutes = db.RouteAllocations.Where(x => x.Active == true && x.Deleted == false).ToList();

            DateTime dateNow = DateTime.Now.Date;
            DateTime firstDate = DateTime.Now.Date;
            DateTime lastDate = DateTime.Now.Date;

            if (periodroutes.Count() > 0)
            {
                firstDate = periodroutes.OrderBy(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (firstDate > DateTime.Now.Date)
                {
                    firstDate = DateTime.Now.Date.AddDays(-7);
                }

                lastDate = periodroutes.OrderByDescending(x => x.RouteDate).FirstOrDefault().RouteDate.Value.Date;

                if (lastDate < DateTime.Now.Date)
                {
                    lastDate = DateTime.Now.Date.AddDays(7);
                }
            }

            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }

            // period list
            List<ScheduleViewModel.Period> periodList = new List<ScheduleViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new ScheduleViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate =DateTime.Now.Date;
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // get focus sunday
            if (focusDate.DayOfWeek != DayOfWeek.Sunday)
            {
                while (focusDate.DayOfWeek != DayOfWeek.Sunday)
                    focusDate = focusDate.AddDays(-1);
            }


            // days and period end date
            var periodDays = 28;
            if (Days.HasValue)
            {
                periodDays = Days.Value;
            }

            var periodEndDate = focusDate.AddDays(periodDays - 1);
 

            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          orderby x.Name ascending
                          select new ScheduleViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();


            // depotid
            var depotId = 0;
            if (currentuser.DepotId.HasValue)
            {
                depotId = currentuser.DepotId.Value;
            }
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }

            // loop 
            var associates = db.Associates.Where(x => x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList();


            /*
            if (associates.Where(x => x.DepotId == depotId).Any())
            {
                associates = associates.Where(x => x.DepotId == depotId).ToList();
            }
            */

            int? activedrivers = associates.Where(x => x.DepotId == DepotId && x.Active == true).Count();

            //var routes = db.RouteAllocations.Where(x => x.DepotId == depotId && x.RouteDate.Value >= focusDate && x.RouteDate.Value <= periodEndDate && x.Active == true && x.Deleted == false).ToList();
            var routes = db.RouteAllocations.Where(x => x.RouteDate.Value >= focusDate && x.RouteDate.Value <= periodEndDate && x.Active == true && x.Deleted == false).ToList();

            List<ScheduleViewModel.Associate> associateList = new List<ScheduleViewModel.Associate>();

            foreach (var associate in associates)
            {
                List<ScheduleViewModel.Route> routeList = new List<ScheduleViewModel.Route>();

                bool showAssociate = false;

                for (int i = 0; i < periodDays; i++)
                {
                    var routeDate = focusDate.AddDays(i);

                    if (associate.DepotId == depotId ||
                        routes.Where(x => x.AssociateId == associate.Id && (x.RouteDate == dateNow.AddDays(-1) || x.RouteDate == dateNow || x.RouteDate == dateNow.AddDays(1)) && x.DepotId == depotId).Any() // show external driver only 1 day before to 1 day after working day
                        //routes.Where(x => x.AssociateId == associate.Id && routeDate == dateNow && x.DepotId == depotId).Any() // show external driver when working during period selected
                        //routes.Where(x => x.AssociateId == associate.Id && (x.Associate.DepotId == depotId || x.DepotId == depotId)).Any()
                        )
                    {
                        showAssociate = true;
                    }

                    if (showAssociate == true)
                    {
                        var route = routes.Where(x => x.AssociateId == associate.Id && x.RouteDate == routeDate).FirstOrDefault();
                        int routesOn = routes.Where(x => x.RouteDate == routeDate && x.DepotId == depotId && x.AllocationStatus == "ON").Count();
                        int routesOff = routes.Where(x => x.RouteDate == routeDate && x.DepotId == depotId && (x.AllocationStatus == "OFF" || x.AllocationStatus == "HOLIDAY" || x.AllocationStatus == "TRAINING")).Count();

                        if (route != null)
                        {
                            routeList.Add(new ScheduleViewModel.Route
                            {
                                AssociateId = associate.Id,
                                RouteId = route.Id,
                                RouteDate = routeDate,
                                RouteDepotId = route.DepotId.Value,
                                RouteDepotName = route.Depot.Name,
                                AllocationStatus = route.AllocationStatus,
                                RouteType1 = route.RouteType1,
                                RoutesOn = routesOn,
                                RoutesOff = routesOff,
                                AuthPoc = route.AuthPoc,
                                AuthPayroll = route.AuthPayroll,
                                AuthAdmin = route.AuthAdmin
                            });
                        }
                        else
                        {
                            routeList.Add(new ScheduleViewModel.Route
                            {
                                AssociateId = associate.Id,
                                RouteId = 0,
                                RouteDate = routeDate,
                                RouteDepotId = 0,
                                RouteDepotName = "",
                                AllocationStatus = "",
                                RouteType1 = "",
                                RoutesOn = 0,
                                RoutesOff = 0,
                                AuthPoc = false,
                                AuthPayroll = false,
                                AuthAdmin = false
                            });
                        }
                    }


                }

                if (showAssociate == true)
                {
                    associateList.Add(new ScheduleViewModel.Associate
                    {
                        AssociateId = associate.Id,
                        AssociateName = associate.Name,
                        AssociateMobile = associate.Mobile,
                        AssociateDepotId = associate.DepotId.Value,
                        AssociateDepotName = associate.Depot.Name,
                        AssociateVehicleRegistration = associate.SubRentals.Where(x => !x.DateReturned.HasValue && x.Active == true && x.Deleted == false).Any() ? associate.SubRentals.Where(x => !x.DateReturned.HasValue && x.Active == true && x.Deleted == false).OrderByDescending(o => o.DateRented).FirstOrDefault().Vehicle.Registration : "-",
                        Routes = routeList
                    });
                }

            }
 
            var viewModel = new ScheduleViewModel
            {
                POCDepot = pocDepot,
                DepotId = depotId,
                DateStart = focusDate,
                Days = periodDays,
                ActiveDrivers = activedrivers,
                ScheduleEditDays = settings.ScheduleEditDays,
                //
                Associates = associateList,
                Periods = periodList,
                Depots = depots
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        
        public JsonResult UpdateAllOffDateDepotId(DateTime? RouteDate, int? RouteDepotId)
        {
            if (RouteDate.HasValue && RouteDepotId.HasValue)
            {
                var associates = db.Associates.Where(x => x.AssociateStatus == "Active" && x.ApplicationStatus == "Approved" && x.DepotId == RouteDepotId && x.Active == true && x.Deleted == false).ToList();

                foreach (var associate in associates)
                {
                    if (!db.RouteAllocations.Where(x => x.AssociateId == associate.Id && x.RouteDate == RouteDate).Any())
                    {
                        var routeAllocation = new RouteAllocation();
                        routeAllocation.AssociateId = associate.Id;
                        routeAllocation.RouteDate = RouteDate;
                        routeAllocation.DepotId = RouteDepotId;
                        routeAllocation.AllocationStatus = "OFF";
                        routeAllocation.Status = "Pending";
                        routeAllocation.DateCreated = DateTime.Now;
                        routeAllocation.Active = true;
                        routeAllocation.Deleted = false;
                        db.RouteAllocations.Add(routeAllocation);
                        db.SaveChanges();
                    }
                }
            }

            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }




        #endregion





        #region ScheduleEdit


        public JsonResult ScheduleEdit(int? associateId, DateTime? routeDate, int? routeDepotId, string allocationStatus)
        {
            try
            {
                var associate = db.Associates.Where(x => x.Id == associateId).FirstOrDefault();

                // delete routes
                var routes = db.RouteAllocations.Where(x => x.AssociateId == associate.Id && x.RouteDate == routeDate).ToList();
                foreach (var route in routes)
                {
                    db.RouteAllocations.Remove(route);
                    db.SaveChanges();
                }

                if (allocationStatus == "REMOVE")
                {
                    return Json(new
                    {
                        status = 1,
                        //
                        associateid = associate.Id,
                        associatename = associate.Name,
                        associatedepotid = associate.DepotId,
                        associatedepotname = associate.Depot.Name,
                        routeid = 0,
                        routedate = String.Format("{0:yyyy-MM-dd}", routeDate),
                        routedepotid = 0,
                        routedepotname = "",
                        allocationstatus = "",
                        //
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int daysworked = 0;

                    // check if worked 6 days in a row
                    if (allocationStatus == "ON")
                    {
                        for (DateTime? date = routeDate.Value.AddDays(-6); date <= routeDate; date = date.Value.AddDays(1))
                        {
                            if (db.RouteAllocations.Where(x => x.AssociateId == associateId && x.RouteDate == date && x.AllocationStatus == "ON" && x.Active == true && x.Deleted == false).Any())
                            {
                                daysworked++;
                            }
                        }
                    }

                    if (daysworked == 6)
                    {
                        return Json(new
                        {
                            status = 2,
                            daysworked = daysworked
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // add route
                        var routeAllocation = new RouteAllocation();
                        routeAllocation.AssociateId = associateId;
                        routeAllocation.RouteDate = routeDate;
                        routeAllocation.DepotId = routeDepotId;
                        routeAllocation.AllocationStatus = allocationStatus;
                        routeAllocation.Active = true;
                        routeAllocation.Deleted = false;
                        routeAllocation.Status = "Pending";
                        routeAllocation.DateCreated = DateTime.Now;
                        db.RouteAllocations.Add(routeAllocation);
                        db.SaveChanges();

                        var getroute = db.RouteAllocations.Where(x => x.Id == routeAllocation.Id).FirstOrDefault();
                        var getassociate = db.Associates.Where(x => x.Id == getroute.AssociateId).FirstOrDefault();
                        var depot = db.Depots.Where(x => x.Id == getroute.DepotId).FirstOrDefault();

                        return Json(new
                        {
                            status = 1,
                            //
                            associateid = getassociate.Id,
                            associatename = getassociate.Name,
                            associatedepotid = getassociate.DepotId,
                            associatedepotname = getassociate.Depot.Name,
                            routeid = getroute.Id,
                            routedate = String.Format("{0:yyyy-MM-dd}", getroute.RouteDate),
                            routedepotid = getroute.DepotId,
                            routedepotname = getroute.Depot.Name,
                            allocationstatus = getroute.AllocationStatus,
                            depotname = depot.Name,
                            authadmin = getroute.AuthAdmin,
                            routetype1 = getroute.RouteType1,
                            routetype2 = getroute.RouteType2,
                            routetype3 = getroute.RouteType3,
                            routetype4 = getroute.RouteType4,
                            routetype5 = getroute.RouteType5
                            //
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }

        }
        
        
        #endregion
        



        
        #region RouteEdit
        public JsonResult RouteEdit(int? AssociateId, DateTime? RouteDate, int? RouteId, string RouteCode1, string RouteType1, string RouteCode2, string RouteType2, string RouteCode3, string RouteType3, string RouteCode4, string RouteType4, string RouteCode5, string RouteType5, int? RouteDepotId, double Mileage, double FuelCharge, double Byod, double CongestionCharge, double LatePayment, string StartTime, string EndTime, double TotalTime, string Notes)
        {
            try
            {
                // check if worked 6 days in a row

                bool hasWorked = false;
                
                /*
                for (int i = 1; i <= 6; i++)
                {
                    DateTime date = RouteDate.Value.AddDays(-i);

                    if (db.RouteAllocations.Where(x => x.AssociateId == AssociateId && x.RouteDate == date && x.AllocationStatus != "ON" && x.Active == true && x.Deleted == false).Any())
                    {
                        hasWorked = false;
                    }
                }
                */


                if (hasWorked == true)
                {
                    return Json(new
                    {
                        status = 2,
                        hasWorked = hasWorked
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (RouteId == 0)
                    {
                        // add

                        RouteAllocation route = new RouteAllocation();
                        // 
                        route.AssociateId = AssociateId;
                        route.RouteDate = RouteDate;
                        route.Status = "Pending";
                        route.AllocationStatus = "ON";
                        route.DateCreated = DateTime.Now;
                        route.Active = true;
                        route.Deleted = false;
                        //
                        route.RouteCode1 = RouteCode1;
                        route.RouteType1 = RouteType1;
                        route.RoutePrice1 = Helpers.Prices.GetPrice("SBL", RouteType1);
                        //
                        route.RouteCode2 = RouteCode2;
                        route.RouteType2 = RouteType2;
                        route.RoutePrice2 = Helpers.Prices.GetPrice("SBL", RouteType2);
                        //
                        route.RouteCode3 = RouteCode3;
                        route.RouteType3 = RouteType3;
                        route.RoutePrice3 = Helpers.Prices.GetPrice("SBL", RouteType3);
                        //
                        route.RouteCode4 = RouteCode4;
                        route.RouteType4 = RouteType4;
                        route.RoutePrice4 = Helpers.Prices.GetPrice("SBL", RouteType4);
                        //
                        route.RouteCode5 = RouteCode5;
                        route.RouteType5 = RouteType5;
                        route.RoutePrice5 = Helpers.Prices.GetPrice("SBL", RouteType5);
                        //
                        route.DepotId = RouteDepotId;

                        //

                        route.Mileage = Mileage;
                        route.FuelChargePrice = FuelCharge;

                        route.BYODPrice = Byod;

                        /*
                        if (Byod > 0)
                        {
                            //route.BYODPrice = Helpers.Prices.GetPrice("SBL", "Byod");
                        }
                        else
                        {
                            route.BYODPrice = 0;
                        }
                        */

                        if (CongestionCharge > 0)
                        {
                            route.CongestionChargeQuantity = 1;
                            route.CongestionChargePrice = Helpers.Prices.GetPrice("SBL", "CongestionCharge");
                        }
                        else
                        {
                            route.CongestionChargeQuantity = 0;
                            route.CongestionChargePrice = 0;
                        }

                        if (LatePayment > 0)
                        {
                            route.LatePaymentQuantity = LatePayment;
                            route.LatePaymentPrice = Helpers.Prices.GetPrice("SBL", "LatePayment") * LatePayment;
                        }
                        else
                        {
                            route.LatePaymentQuantity = 0;
                            route.LatePaymentPrice = 0;
                        }

                        //

                        route.StartTime = StartTime;
                        route.EndTime = EndTime;
                        route.TotalTime = TotalTime;

                        route.Notes = Notes;

                        db.RouteAllocations.Add(route);
                        db.SaveChanges();

                        var getroute = db.RouteAllocations.Where(x => x.Id == route.Id).FirstOrDefault();
                        var getassociate = db.Associates.Where(x => x.Id == getroute.AssociateId).FirstOrDefault();
                        var depot = db.Depots.Where(x => x.Id == getroute.DepotId).FirstOrDefault();

                        return Json(new
                        {
                            status = 1,
                            //
                            associateid = getassociate.Id,
                            associatename = getassociate.Name,
                            associatedepotid = getassociate.DepotId,
                            associatedepotname = getassociate.Depot.Name,
                            routeid = getroute.Id,
                            routedate = String.Format("{0:yyyy-MM-dd}", getroute.RouteDate),
                            routedepotid = getroute.DepotId,
                            routedepotname = getroute.Depot.Name,
                            allocationstatus = getroute.AllocationStatus,
                            //
                            depotname = depot.Name,
                            authadmin = getroute.AuthAdmin,
                            routetype1 = getroute.RouteType1,
                            routetype2 = getroute.RouteType2,
                            routetype3 = getroute.RouteType3,
                            routetype4 = getroute.RouteType4,
                            routetype5 = getroute.RouteType5
                            //
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // edit

                        var route = db.RouteAllocations.Where(x => x.Id == RouteId).FirstOrDefault();
                        //
                        route.RouteCode1 = RouteCode1;
                        route.RouteType1 = RouteType1;
                        route.RoutePrice1 = Helpers.Prices.GetPrice("SBL", RouteType1);
                        //
                        route.RouteCode2 = RouteCode2;
                        route.RouteType2 = RouteType2;
                        route.RoutePrice2 = Helpers.Prices.GetPrice("SBL", RouteType2);
                        //
                        route.RouteCode3 = RouteCode3;
                        route.RouteType3 = RouteType3;
                        route.RoutePrice3 = Helpers.Prices.GetPrice("SBL", RouteType3);
                        //
                        route.RouteCode4 = RouteCode4;
                        route.RouteType4 = RouteType4;
                        route.RoutePrice4 = Helpers.Prices.GetPrice("SBL", RouteType4);
                        //
                        route.RouteCode5 = RouteCode5;
                        route.RouteType5 = RouteType5;
                        route.RoutePrice5 = Helpers.Prices.GetPrice("SBL", RouteType5);
                        //
                        route.DepotId = RouteDepotId;

                        //

                        route.Mileage = Mileage;
                        route.FuelChargePrice = FuelCharge;

                        route.BYODPrice = Byod;


                        /*
                        if (Byod > 0)
                        {
                            route.BYODPrice = Helpers.Prices.GetPrice("SBL", "Byod");
                        }
                        else
                        {
                            route.BYODPrice = 0;
                        }
                        */

                        if (CongestionCharge > 0)
                        {
                            route.CongestionChargeQuantity = 1;
                            route.CongestionChargePrice = Helpers.Prices.GetPrice("SBL", "CongestionCharge");
                        }
                        else
                        {
                            route.CongestionChargeQuantity = 0;
                            route.CongestionChargePrice = 0;
                        }

                        if (LatePayment > 0)
                        {
                            route.LatePaymentQuantity = LatePayment;
                            route.LatePaymentPrice = Helpers.Prices.GetPrice("SBL", "LatePayment") * LatePayment;
                        }
                        else
                        {
                            route.LatePaymentQuantity = 0;
                            route.LatePaymentPrice = 0;
                        }

                        //

                        route.StartTime = StartTime;
                        route.EndTime = EndTime;
                        route.TotalTime = TotalTime;

                        route.Notes = Notes;
                        route.AllocationStatus = "ON";
                        db.SaveChanges();

                        var getroute = db.RouteAllocations.Where(x => x.Id == route.Id).FirstOrDefault();
                        var getassociate = db.Associates.Where(x => x.Id == getroute.AssociateId).FirstOrDefault();
                        var depot = db.Depots.Where(x => x.Id == getroute.DepotId).FirstOrDefault();

                        return Json(new
                        {
                            status = 1,
                            //
                            associateid = getassociate.Id,
                            associatename = getassociate.Name,
                            associatedepotid = getassociate.DepotId,
                            associatedepotname = getassociate.Depot.Name,
                            routeid = getroute.Id,
                            routedate = String.Format("{0:yyyy-MM-dd}", getroute.RouteDate),
                            routedepotid = getroute.DepotId,
                            routedepotname = getroute.Depot.Name,
                            allocationstatus = getroute.AllocationStatus,
                            //
                            depotname = depot.Name,
                            authadmin = getroute.AuthAdmin,
                            routetype1 = getroute.RouteType1,
                            routetype2 = getroute.RouteType2,
                            routetype3 = getroute.RouteType3,
                            routetype4 = getroute.RouteType4,
                            routetype5 = getroute.RouteType5
                            //
                        }, JsonRequestBehavior.AllowGet);
                    }


                    // remove duplicate route
                    if (db.RouteAllocations.Where(x => x.AssociateId == AssociateId && x.RouteDate.Value == RouteDate && x.Active == true && x.Deleted == false).Count() > 1)
                    {
                        var extra = db.RouteAllocations.Where(x => x.AssociateId == AssociateId && x.RouteDate.Value == RouteDate && x.Active == true && x.Deleted == false).OrderByDescending(x => x.DateCreated).FirstOrDefault();
                        extra.Active = true;
                        extra.Deleted = false;
                        db.SaveChanges();

                        Log log = new Log();
                        log.Message = "duplicated route allocation removed";
                        log.Method = "";
                        log.DateCreated = DateTime.Now;
                        log.IsError = false;
                        db.Logs.Add(log);
                        db.SaveChanges();
                    }


                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion





        #region Get BYOD Price
        public JsonResult GetByodPrice()
        {
            try
            {
                var byodprice = db.Settings.FirstOrDefault().BYODSbl;
               
                return Json(new
                {
                    byodprice = byodprice,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    byodprice = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion




        #region Get Congestion Charge Price
        public JsonResult GetCongestionChargePrice()
        {
            try
            {
                var congestionchargeprice = db.Settings.FirstOrDefault().CongestionChargeSbl;

                return Json(new
                {
                    congestionchargeprice = congestionchargeprice,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    congestionchargeprice = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion




        #region ServiceConfirmation



        public ActionResult ServiceConfirmation(DateTime? Date, int? Days, int? DepotId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);


            // period list

            DateTime firstDate = DateTime.Now.Date.AddMonths(-2);
            DateTime lastDate = DateTime.Now.Date.AddMonths(2);


            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }


            // period list
            List<ServiceConfirmationViewModel.Period> periodList = new List<ServiceConfirmationViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new ServiceConfirmationViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate = DateTime.Now.Date;
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // get focus sunday
            if (focusDate.DayOfWeek != DayOfWeek.Sunday)
            {
                while (focusDate.DayOfWeek != DayOfWeek.Sunday)
                    focusDate = focusDate.AddDays(-1);
            }


            // days and period end date
            var periodDays = 7;
            if (Days.HasValue)
            {
                //periodDays = Days.Value;
            }

            var periodEndDate = focusDate.AddDays(periodDays - 1);


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          orderby x.Name ascending
                          select new ServiceConfirmationViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();


            // depotid
            var depotId = 0;
            if (currentuser.DepotId.HasValue)
            {
                depotId = currentuser.DepotId.Value;
            }
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }


            // this period
            var hasWritePermision = false;
            if (User.IsInRole(WebConstant.SBLUserRole.Master) || User.IsInRole(WebConstant.SBLUserRole.Admin))
            {
                hasWritePermision = true;
            }
            else
            {
                if (currentuser != null && currentuser.DepotId == depotId)
                {
                    hasWritePermision = true;
                }
                else
                {
                    hasWritePermision = false;
                }
            }

            ViewBag.HasWritePermision = hasWritePermision;



            var routeAllocationList = new List<ServiceConfirmationViewModel.Total>();


            #region week variables

            // amazon week
            double amazonWeekFullAndHalfQuantity = 0;
            double amazonWeekFullAndHalfPrice = 0;
            //
            double amazonWeekFullQuantity = 0;
            double amazonWeekFullPrice = 0;
            double amazonWeekHalfQuantity = 0;
            double amazonWeekHalfPrice = 0;
            double amazonWeekRemoteDebriefQuantity = 0;
            double amazonWeekRemoteDebriefPrice = 0;
            double amazonWeekNurseryRoutesLevel1Quantity = 0;
            double amazonWeekNurseryRoutesLevel1Price = 0;
            double amazonWeekNurseryRoutesLevel2Quantity = 0;
            double amazonWeekNurseryRoutesLevel2Price = 0;
            double amazonWeekRescue2HoursQuantity = 0;
            double amazonWeekRescue2HoursPrice = 0;
            double amazonWeekRescue4HoursQuantity = 0;
            double amazonWeekRescue4HoursPrice = 0;
            double amazonWeekRescue6HoursQuantity = 0;
            double amazonWeekRescue6HoursPrice = 0;
            double amazonWeekReDelivery2HoursQuantity = 0;
            double amazonWeekReDelivery2HoursPrice = 0;
            double amazonWeekReDelivery4HoursQuantity = 0;
            double amazonWeekReDelivery4HoursPrice = 0;
            double amazonWeekReDelivery6HoursQuantity = 0;
            double amazonWeekReDelivery6HoursPrice = 0;
            double amazonWeekMissort2HoursQuantity = 0;
            double amazonWeekMissort2HoursPrice = 0;
            double amazonWeekMissort4HoursQuantity = 0;
            double amazonWeekMissort4HoursPrice = 0;
            double amazonWeekMissort6HoursQuantity = 0;
            double amazonWeekMissort6HoursPrice = 0;
            double amazonWeekSameDayQuantity = 0;
            double amazonWeekSameDayPrice = 0;
            double amazonWeekTrainingDayQuantity = 0;
            double amazonWeekTrainingDayPrice = 0;
            double amazonWeekRideAlongQuantity = 0;
            double amazonWeekRideAlongPrice = 0;

            double amazonWeekSupportAd1Quantity = 0;
            double amazonWeekSupportAd1Price = 0;
            double amazonWeekSupportAd2Quantity = 0;
            double amazonWeekSupportAd2Price = 0;
            double amazonWeekSupportAd3Quantity = 0;
            double amazonWeekSupportAd3Price = 0;
            double amazonWeekLeadDriverQuantity = 0;
            double amazonWeekLeadDriverPrice = 0;
            double amazonWeekLargeVanQuantity = 0;
            double amazonWeekLargeVanPrice = 0;

            double amazonWeekCongestionChargeQuantity = 0;
            double amazonWeekCongestionChargePrice = 0;
            double amazonWeekLatePaymentQuantity = 0;
            double amazonWeekLatePaymentPrice = 0;

            double amazonWeekFuel = 0;
            double amazonWeekMileage = 0;
            double amazonWeekDeduct = 0;

            // sbl week
            double sblWeekFullAndHalfQuantity = 0;
            double sblWeekFullAndHalfPrice = 0;
            //
            double sblWeekFullQuantity = 0;
            double sblWeekFullPrice = 0;
            double sblWeekHalfQuantity = 0;
            double sblWeekHalfPrice = 0;
            double sblWeekRemoteDebriefQuantity = 0;
            double sblWeekRemoteDebriefPrice = 0;
            double sblWeekNurseryRoutesLevel1Quantity = 0;
            double sblWeekNurseryRoutesLevel1Price = 0;
            double sblWeekNurseryRoutesLevel2Quantity = 0;
            double sblWeekNurseryRoutesLevel2Price = 0;
            double sblWeekRescue2HoursQuantity = 0;
            double sblWeekRescue2HoursPrice = 0;
            double sblWeekRescue4HoursQuantity = 0;
            double sblWeekRescue4HoursPrice = 0;
            double sblWeekRescue6HoursQuantity = 0;
            double sblWeekRescue6HoursPrice = 0;
            double sblWeekReDelivery2HoursQuantity = 0;
            double sblWeekReDelivery2HoursPrice = 0;
            double sblWeekReDelivery4HoursQuantity = 0;
            double sblWeekReDelivery4HoursPrice = 0;
            double sblWeekReDelivery6HoursQuantity = 0;
            double sblWeekReDelivery6HoursPrice = 0;
            double sblWeekMissort2HoursQuantity = 0;
            double sblWeekMissort2HoursPrice = 0;
            double sblWeekMissort4HoursQuantity = 0;
            double sblWeekMissort4HoursPrice = 0;
            double sblWeekMissort6HoursQuantity = 0;
            double sblWeekMissort6HoursPrice = 0;
            double sblWeekSameDayQuantity = 0;
            double sblWeekSameDayPrice = 0;
            double sblWeekTrainingDayQuantity = 0;
            double sblWeekTrainingDayPrice = 0;
            double sblWeekRideAlongQuantity = 0;
            double sblWeekRideAlongPrice = 0;

            double sblWeekSupportAd1Quantity = 0;
            double sblWeekSupportAd1Price = 0;
            double sblWeekSupportAd2Quantity = 0;
            double sblWeekSupportAd2Price = 0;
            double sblWeekSupportAd3Quantity = 0;
            double sblWeekSupportAd3Price = 0;
            double sblWeekLeadDriverQuantity = 0;
            double sblWeekLeadDriverPrice = 0;
            double sblWeekLargeVanQuantity = 0;
            double sblWeekLargeVanPrice = 0;

            double sblWeekCongestionChargeQuantity = 0;
            double sblWeekCongestionChargePrice = 0;
            double sblWeekLatePaymentQuantity = 0;
            double sblWeekLatePaymentPrice = 0;

            double sblWeekFuel = 0;
            double sblWeekMileage = 0;
            double sblWeekDeduct = 0;

            // diff week
            double diffWeekFullAndHalfQuantity = 0;
            double diffWeekFullAndHalfPrice = 0;
            //
            double diffWeekFullQuantity = 0;
            double diffWeekFullPrice = 0;
            double diffWeekHalfQuantity = 0;
            double diffWeekHalfPrice = 0;
            double diffWeekRemoteDebriefQuantity = 0;
            double diffWeekRemoteDebriefPrice = 0;
            double diffWeekNurseryRoutesLevel1Quantity = 0;
            double diffWeekNurseryRoutesLevel1Price = 0;
            double diffWeekNurseryRoutesLevel2Quantity = 0;
            double diffWeekNurseryRoutesLevel2Price = 0;
            double diffWeekRescue2HoursQuantity = 0;
            double diffWeekRescue2HoursPrice = 0;
            double diffWeekRescue4HoursQuantity = 0;
            double diffWeekRescue4HoursPrice = 0;
            double diffWeekRescue6HoursQuantity = 0;
            double diffWeekRescue6HoursPrice = 0;
            double diffWeekReDelivery2HoursQuantity = 0;
            double diffWeekReDelivery2HoursPrice = 0;
            double diffWeekReDelivery4HoursQuantity = 0;
            double diffWeekReDelivery4HoursPrice = 0;
            double diffWeekReDelivery6HoursQuantity = 0;
            double diffWeekReDelivery6HoursPrice = 0;
            double diffWeekMissort2HoursQuantity = 0;
            double diffWeekMissort2HoursPrice = 0;
            double diffWeekMissort4HoursQuantity = 0;
            double diffWeekMissort4HoursPrice = 0;
            double diffWeekMissort6HoursQuantity = 0;
            double diffWeekMissort6HoursPrice = 0;
            double diffWeekSameDayQuantity = 0;
            double diffWeekSameDayPrice = 0;
            double diffWeekTrainingDayQuantity = 0;
            double diffWeekTrainingDayPrice = 0;
            double diffWeekRideAlongQuantity = 0;
            double diffWeekRideAlongPrice = 0;

            double diffWeekSupportAd1Quantity = 0;
            double diffWeekSupportAd1Price = 0;
            double diffWeekSupportAd2Quantity = 0;
            double diffWeekSupportAd2Price = 0;
            double diffWeekSupportAd3Quantity = 0;
            double diffWeekSupportAd3Price = 0;
            double diffWeekLeadDriverQuantity = 0;
            double diffWeekLeadDriverPrice = 0;
            double diffWeekLargeVanQuantity = 0;
            double diffWeekLargeVanPrice = 0;

            double diffWeekCongestionChargeQuantity = 0;
            double diffWeekCongestionChargePrice = 0;
            double diffWeekLatePaymentQuantity = 0;
            double diffWeekLatePaymentPrice = 0;

            double diffWeekFuel = 0;
            double diffWeekMileage = 0;
            double diffWeekDeduct = 0;

            #endregion


            #region week grand totals

            double amazonWeekTotalQuantity = 0;
            double amazonWeekTotalPrice = 0;
            double sblWeekTotalQuantity = 0;
            double sblWeekTotalPrice = 0;
            double diffWeekTotalQuantity = 0;
            double diffWeekTotalPrice = 0;

            #endregion


            #region Get Detail of Route Per Day 
            foreach (DateTime day in Utility.EachDay(focusDate, periodEndDate))
            {
                //var dateRoute = ;
                int routeAmazonId = 0;

                var routeDate = day.Date;

                var amazonRoute = db.RouteAmazons.Where(x => x.RouteDate == routeDate && x.Active == true && x.Deleted == false);

                if (depotId != 0)
                {
                    amazonRoute = amazonRoute.Where(x => x.DepotId == depotId);
                }


                #region amazon day variables

                double amazonDayFullAndHalfQuantity = 0;
                double amazonDayFullAndHalfPrice = 0;
                //
                double amazonDayFullQuantity = 0;
                double amazonDayFullPrice = 0;
                double amazonDayHalfQuantity = 0;
                double amazonDayHalfPrice = 0;
                double amazonDayRemoteDebriefQuantity = 0;
                double amazonDayRemoteDebriefPrice = 0;
                double amazonDayNurseryRoutesLevel1Quantity = 0;
                double amazonDayNurseryRoutesLevel1Price = 0;
                double amazonDayNurseryRoutesLevel2Quantity = 0;
                double amazonDayNurseryRoutesLevel2Price = 0;
                double amazonDayRescue2HoursQuantity = 0;
                double amazonDayRescue2HoursPrice = 0;
                double amazonDayRescue4HoursQuantity = 0;
                double amazonDayRescue4HoursPrice = 0;
                double amazonDayRescue6HoursQuantity = 0;
                double amazonDayRescue6HoursPrice = 0;
                double amazonDayReDelivery2HoursQuantity = 0;
                double amazonDayReDelivery2HoursPrice = 0;
                double amazonDayReDelivery4HoursQuantity = 0;
                double amazonDayReDelivery4HoursPrice = 0;
                double amazonDayReDelivery6HoursQuantity = 0;
                double amazonDayReDelivery6HoursPrice = 0;
                double amazonDayMissort2HoursQuantity = 0;
                double amazonDayMissort2HoursPrice = 0;
                double amazonDayMissort4HoursQuantity = 0;
                double amazonDayMissort4HoursPrice = 0;
                double amazonDayMissort6HoursQuantity = 0;
                double amazonDayMissort6HoursPrice = 0;
                double amazonDaySameDayQuantity = 0;
                double amazonDaySameDayPrice = 0;
                double amazonDayTrainingDayQuantity = 0;
                double amazonDayTrainingDayPrice = 0;
                double amazonDayRideAlongQuantity = 0;
                double amazonDayRideAlongPrice = 0;

                double amazonDaySupportAd1Quantity = 0;
                double amazonDaySupportAd1Price = 0;
                double amazonDaySupportAd2Quantity = 0;
                double amazonDaySupportAd2Price = 0;
                double amazonDaySupportAd3Quantity = 0;
                double amazonDaySupportAd3Price = 0;
                double amazonDayLeadDriverQuantity = 0;
                double amazonDayLeadDriverPrice = 0;
                double amazonDayLargeVanQuantity = 0;
                double amazonDayLargeVanPrice = 0;

                double amazonDayCongestionChargeQuantity = 0;
                double amazonDayCongestionChargePrice = 0;
                double amazonDayLatePaymentQuantity = 0;
                double amazonDayLatePaymentPrice = 0;

                double amazonDayFuel = 0;
                double amazonDayMileage = 0;
                double amazonDayDeduct = 0;

                #endregion


                #region amazon day sum

                if (amazonRoute.Any())
                {
                    var amazonDayRoute = amazonRoute.FirstOrDefault();
                    // set route id
                    if (amazonDayRoute != null)
                    {
                        routeAmazonId = amazonDayRoute.Id;

                        //
                        amazonDayFullAndHalfQuantity = amazonDayRoute.FullQuantity + amazonDayRoute.HalfQuantity;
                        amazonDayFullAndHalfPrice = (amazonDayRoute.FullQuantity * amazonDayRoute.FullPrice) + (amazonDayRoute.HalfQuantity * amazonDayRoute.HalfPrice);
                        //

                        amazonDayFullQuantity = amazonDayRoute.FullQuantity;
                        amazonDayFullPrice = amazonDayRoute.FullQuantity * amazonDayRoute.FullPrice;

                        amazonDayHalfQuantity = amazonDayRoute.HalfQuantity;
                        amazonDayHalfPrice = amazonDayRoute.HalfQuantity * amazonDayRoute.HalfPrice;

                        amazonDayRemoteDebriefQuantity = amazonDayRoute.RemoteDebriefQuantity;
                        amazonDayRemoteDebriefPrice = amazonDayRoute.RemoteDebriefQuantity * amazonDayRoute.RemoteDebriefPrice;

                        amazonDayNurseryRoutesLevel1Quantity = amazonDayRoute.NurseryRoutesLevel1Quantity;
                        amazonDayNurseryRoutesLevel1Price = amazonDayRoute.NurseryRoutesLevel1Quantity * amazonDayRoute.NurseryRoutesLevel1Price;

                        amazonDayNurseryRoutesLevel2Quantity = amazonDayRoute.NurseryRoutesLevel2Quantity;
                        amazonDayNurseryRoutesLevel2Price = amazonDayRoute.NurseryRoutesLevel2Quantity * amazonDayRoute.NurseryRoutesLevel2Price;

                        amazonDayRescue2HoursQuantity = amazonDayRoute.Rescue2HoursQuantity;
                        amazonDayRescue2HoursPrice = amazonDayRoute.Rescue2HoursQuantity * amazonDayRoute.Rescue2HoursPrice;

                        amazonDayRescue4HoursQuantity = amazonDayRoute.Rescue4HoursQuantity;
                        amazonDayRescue4HoursPrice = amazonDayRoute.Rescue4HoursQuantity * amazonDayRoute.Rescue4HoursPrice;

                        amazonDayRescue6HoursQuantity = amazonDayRoute.Rescue6HoursQuantity;
                        amazonDayRescue6HoursPrice = amazonDayRoute.Rescue6HoursQuantity * amazonDayRoute.Rescue6HoursPrice;

                        amazonDayReDelivery2HoursQuantity = amazonDayRoute.ReDelivery2HoursQuantity;
                        amazonDayReDelivery2HoursPrice = amazonDayRoute.ReDelivery2HoursQuantity * amazonDayRoute.ReDelivery2HoursPrice;

                        amazonDayReDelivery4HoursQuantity = amazonDayRoute.ReDelivery4HoursQuantity;
                        amazonDayReDelivery4HoursPrice = amazonDayRoute.ReDelivery4HoursQuantity * amazonDayRoute.ReDelivery4HoursPrice;

                        amazonDayReDelivery6HoursQuantity = amazonDayRoute.ReDelivery6HoursQuantity;
                        amazonDayReDelivery6HoursPrice = amazonDayRoute.ReDelivery6HoursQuantity * amazonDayRoute.ReDelivery6HoursPrice;

                        amazonDayMissort2HoursQuantity = amazonDayRoute.Missort2HoursQuantity;
                        amazonDayMissort2HoursPrice = amazonDayRoute.Missort2HoursQuantity * amazonDayRoute.Missort2HoursPrice;

                        amazonDayMissort4HoursQuantity = amazonDayRoute.Missort4HoursQuantity;
                        amazonDayMissort4HoursPrice = amazonDayRoute.Missort4HoursQuantity * amazonDayRoute.Missort4HoursPrice;

                        amazonDayMissort6HoursQuantity = amazonDayRoute.Missort6HoursQuantity;
                        amazonDayMissort6HoursPrice = amazonDayRoute.Missort6HoursQuantity * amazonDayRoute.Missort6HoursPrice;

                        amazonDaySameDayQuantity = amazonDayRoute.SameDayQuantity;
                        amazonDaySameDayPrice = amazonDayRoute.SameDayQuantity * amazonDayRoute.SameDayPrice;

                        amazonDayTrainingDayQuantity = amazonDayRoute.TrainingDayQuantity;
                        amazonDayTrainingDayPrice = amazonDayRoute.TrainingDayQuantity * amazonDayRoute.TrainingDayPrice;

                        amazonDayRideAlongQuantity = amazonDayRoute.RideAlongQuantity;
                        amazonDayRideAlongPrice = amazonDayRoute.RideAlongQuantity * amazonDayRoute.RideAlongPrice;

                        amazonDaySupportAd1Quantity = amazonDayRoute.SupportAd1Quantity;
                        amazonDaySupportAd1Price = amazonDayRoute.SupportAd1Quantity * amazonDayRoute.SupportAd1Price;

                        amazonDaySupportAd2Quantity = amazonDayRoute.SupportAd2Quantity;
                        amazonDaySupportAd2Price = amazonDayRoute.SupportAd2Quantity * amazonDayRoute.SupportAd2Price;

                        amazonDaySupportAd3Quantity = amazonDayRoute.SupportAd3Quantity;
                        amazonDaySupportAd3Price = amazonDayRoute.SupportAd3Quantity * amazonDayRoute.SupportAd3Price;

                        amazonDayLeadDriverQuantity = amazonDayRoute.LeadDriverQuantity;
                        amazonDayLeadDriverPrice = amazonDayRoute.LeadDriverQuantity * amazonDayRoute.LeadDriverPrice;

                        amazonDayLargeVanQuantity = amazonDayRoute.LargeVanQuantity;
                        amazonDayLargeVanPrice = amazonDayRoute.LargeVanQuantity * amazonDayRoute.LargeVanPrice;

                        amazonDayCongestionChargeQuantity = amazonDayRoute.CongestionChargeQuantity;
                        amazonDayCongestionChargePrice = amazonDayRoute.CongestionChargeQuantity * amazonDayRoute.CongestionChargePrice;

                        amazonDayLatePaymentQuantity = amazonDayRoute.LatePaymentQuantity;
                        amazonDayLatePaymentPrice = amazonDayRoute.LatePaymentQuantity * amazonDayRoute.LatePaymentPrice;

                        amazonDayFuel = amazonDayRoute.Fuel;
                        amazonDayMileage = amazonDayRoute.Mileage;
                        amazonDayDeduct = amazonDayRoute.Deduct;
                    }
                }

                #endregion


                #region amazon week sum

                if (amazonRoute.Any())
                {
                    var amazonWeekRoutes = amazonRoute;

                    foreach (var route in amazonWeekRoutes)
                    {
                        amazonWeekFullAndHalfQuantity = amazonWeekFullAndHalfQuantity + (route.FullQuantity + route.HalfQuantity);
                        amazonWeekFullAndHalfPrice = amazonWeekFullAndHalfPrice + (route.FullQuantity * route.FullPrice) + (route.HalfQuantity * route.HalfPrice);

                        //

                        amazonWeekFullQuantity = amazonWeekFullQuantity + route.FullQuantity;
                        amazonWeekFullPrice = amazonWeekFullPrice + (route.FullQuantity * route.FullPrice);

                        amazonWeekHalfQuantity = amazonWeekHalfQuantity + route.HalfQuantity;
                        amazonWeekHalfPrice = amazonWeekHalfPrice + (route.HalfQuantity * route.HalfPrice);

                        amazonWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity + route.RemoteDebriefQuantity;
                        amazonWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice + (route.RemoteDebriefQuantity * route.RemoteDebriefPrice);

                        amazonWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity + route.NurseryRoutesLevel1Quantity;
                        amazonWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price + (route.NurseryRoutesLevel1Quantity * route.NurseryRoutesLevel1Price);

                        amazonWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity + route.NurseryRoutesLevel2Quantity;
                        amazonWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price + (route.NurseryRoutesLevel2Quantity * route.NurseryRoutesLevel2Price);

                        amazonWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity + route.Rescue2HoursQuantity;
                        amazonWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice + (route.Rescue2HoursQuantity * route.Rescue2HoursPrice);

                        amazonWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity + route.Rescue4HoursQuantity;
                        amazonWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice + (route.Rescue4HoursQuantity * route.Rescue4HoursPrice);

                        amazonWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity + route.Rescue6HoursQuantity;
                        amazonWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice + (route.Rescue6HoursQuantity * route.Rescue6HoursPrice);

                        amazonWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity + route.ReDelivery2HoursQuantity;
                        amazonWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice + (route.ReDelivery2HoursQuantity * route.ReDelivery2HoursPrice);

                        amazonWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity + route.ReDelivery4HoursQuantity;
                        amazonWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice + (route.ReDelivery4HoursQuantity * route.ReDelivery4HoursPrice);

                        amazonWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity + route.ReDelivery6HoursQuantity;
                        amazonWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice + (route.ReDelivery6HoursQuantity * route.ReDelivery6HoursPrice);

                        amazonWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity + route.Missort2HoursQuantity;
                        amazonWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice + (route.Missort2HoursQuantity * route.Missort2HoursPrice);

                        amazonWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity + route.Missort4HoursQuantity;
                        amazonWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice + (route.Missort4HoursQuantity * route.Missort4HoursPrice);

                        amazonWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity + route.Missort6HoursQuantity;
                        amazonWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice + (route.Missort6HoursQuantity * route.Missort6HoursPrice);

                        amazonWeekSameDayQuantity = amazonWeekSameDayQuantity + route.SameDayQuantity;
                        amazonWeekSameDayPrice = amazonWeekSameDayPrice + (route.SameDayQuantity * route.SameDayPrice);

                        amazonWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity + route.TrainingDayQuantity;
                        amazonWeekTrainingDayPrice = amazonWeekTrainingDayPrice + (route.TrainingDayQuantity * route.TrainingDayPrice);

                        amazonWeekRideAlongQuantity = amazonWeekRideAlongQuantity + route.RideAlongQuantity;
                        amazonWeekRideAlongPrice = amazonWeekRideAlongPrice + (route.RideAlongQuantity * route.RideAlongPrice);

                        amazonWeekSupportAd1Quantity = amazonWeekSupportAd1Quantity + route.SupportAd1Quantity;
                        amazonWeekSupportAd1Price = amazonWeekSupportAd1Price + (route.SupportAd1Quantity * route.SupportAd1Price);

                        amazonWeekSupportAd2Quantity = amazonWeekSupportAd2Quantity + route.SupportAd2Quantity;
                        amazonWeekSupportAd2Price = amazonWeekSupportAd2Price + (route.SupportAd2Quantity * route.SupportAd2Price);

                        amazonWeekSupportAd3Quantity = amazonWeekSupportAd3Quantity + route.SupportAd3Quantity;
                        amazonWeekSupportAd3Price = amazonWeekSupportAd3Price + (route.SupportAd3Quantity * route.SupportAd3Price);

                        amazonWeekLeadDriverQuantity = amazonWeekLeadDriverQuantity + route.LeadDriverQuantity;
                        amazonWeekLeadDriverPrice = amazonWeekLeadDriverPrice + (route.LeadDriverQuantity * route.LeadDriverPrice);

                        amazonWeekLargeVanQuantity = amazonWeekLargeVanQuantity + route.LargeVanQuantity;
                        amazonWeekLargeVanPrice = amazonWeekLargeVanPrice + (route.LargeVanQuantity * route.LargeVanPrice);

                        amazonWeekCongestionChargeQuantity = amazonWeekCongestionChargeQuantity + route.Ad1Quantity;
                        amazonWeekCongestionChargePrice = amazonWeekCongestionChargePrice + (route.CongestionChargeQuantity * route.CongestionChargePrice);

                        amazonWeekLatePaymentQuantity = amazonWeekLatePaymentQuantity + route.LatePaymentQuantity;
                        amazonWeekLatePaymentPrice = amazonWeekLatePaymentPrice + (route.LatePaymentQuantity * route.LatePaymentPrice);

                        amazonWeekFuel = amazonWeekFuel + route.Fuel;
                        amazonWeekMileage = amazonWeekMileage + route.Mileage;
                        amazonWeekDeduct = amazonWeekDeduct + route.Deduct;
                    }
                }
                #endregion


                #region sbl day variables

                // sbl day
                double sblDayFullAndHalfQuantity = 0;
                double sblDayFullAndHalfPrice = 0;
                //
                double sblDayFullQuantity = 0;
                double sblDayFullPrice = 0;
                double sblDayHalfQuantity = 0;
                double sblDayHalfPrice = 0;
                double sblDayRemoteDebriefQuantity = 0;
                double sblDayRemoteDebriefPrice = 0;
                double sblDayNurseryRoutesLevel1Quantity = 0;
                double sblDayNurseryRoutesLevel1Price = 0;
                double sblDayNurseryRoutesLevel2Quantity = 0;
                double sblDayNurseryRoutesLevel2Price = 0;
                double sblDayRescue2HoursQuantity = 0;
                double sblDayRescue2HoursPrice = 0;
                double sblDayRescue4HoursQuantity = 0;
                double sblDayRescue4HoursPrice = 0;
                double sblDayRescue6HoursQuantity = 0;
                double sblDayRescue6HoursPrice = 0;
                double sblDayReDelivery2HoursQuantity = 0;
                double sblDayReDelivery2HoursPrice = 0;
                double sblDayReDelivery4HoursQuantity = 0;
                double sblDayReDelivery4HoursPrice = 0;
                double sblDayReDelivery6HoursQuantity = 0;
                double sblDayReDelivery6HoursPrice = 0;
                double sblDayMissort2HoursQuantity = 0;
                double sblDayMissort2HoursPrice = 0;
                double sblDayMissort4HoursQuantity = 0;
                double sblDayMissort4HoursPrice = 0;
                double sblDayMissort6HoursQuantity = 0;
                double sblDayMissort6HoursPrice = 0;
                double sblDaySameDayQuantity = 0;
                double sblDaySameDayPrice = 0;
                double sblDayTrainingDayQuantity = 0;
                double sblDayTrainingDayPrice = 0;
                double sblDayRideAlongQuantity = 0;
                double sblDayRideAlongPrice = 0;

                double sblDaySupportAd1Quantity = 0;
                double sblDaySupportAd1Price = 0;
                double sblDaySupportAd2Quantity = 0;
                double sblDaySupportAd2Price = 0;
                double sblDaySupportAd3Quantity = 0;
                double sblDaySupportAd3Price = 0;
                double sblDayLeadDriverQuantity = 0;
                double sblDayLeadDriverPrice = 0;
                double sblDayLargeVanQuantity = 0;
                double sblDayLargeVanPrice = 0;

                double sblDayCongestionChargeQuantity = 0;
                double sblDayCongestionChargePrice = 0;
                double sblDayLatePaymentQuantity = 0;
                double sblDayLatePaymentPrice = 0;

                double sblDayFuel = 0;
                double sblDayMileage = 0;
                double sblDayDeduct = 0;


                #endregion


                #region sblDayRoutes1

                var sblRoutes = db.RouteAllocations.Where(x => x.RouteDate == routeDate && x.Active && x.Deleted == false);

                if (depotId != 0)
                {
                    sblRoutes = sblRoutes.Where(x => x.DepotId == depotId);
                }

                if (sblRoutes.Any())
                {
                    var sblDayRoutes = sblRoutes;

                    foreach (var route in sblDayRoutes)
                    {

                        //
                        // 1
                        //

                        if (route.RouteType1 == "Full")
                        {
                            sblDayFullQuantity++;
                            //sblDayFullPrice = sblDayFullPrice + (sblDayFullQuantity * route.RoutePrice1);
                            sblDayFullPrice = sblDayFullPrice + route.RoutePrice1;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 1;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayFullQuantity * route.RoutePrice1);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Half")
                        {
                            sblDayHalfQuantity++;
                            //sblDayHalfPrice = sblDayHalfPrice + (sblDayHalfQuantity * route.RoutePrice1);
                            sblDayHalfPrice = sblDayHalfPrice + route.RoutePrice1;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 0.5;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayHalfQuantity * route.RoutePrice1);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "RemoteDebrief")
                        {
                            sblDayRemoteDebriefQuantity++;
                            //sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + (sblDayRemoteDebriefQuantity * route.RoutePrice1);
                            sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "NurseryRoutesLevel1")
                        {
                            sblDayNurseryRoutesLevel1Quantity++;
                            //sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + (sblDayNurseryRoutesLevel1Quantity * route.RoutePrice1);
                            sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "NurseryRoutesLevel2")
                        {
                            sblDayNurseryRoutesLevel2Quantity++;
                            //sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + (sblDayNurseryRoutesLevel2Quantity * route.RoutePrice1);
                            sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Rescue2Hours")
                        {
                            sblDayRescue2HoursQuantity++;
                            //sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + (sblDayRescue2HoursQuantity * route.RoutePrice1);
                            sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Rescue4Hours")
                        {
                            sblDayRescue4HoursQuantity++;
                            //sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + (sblDayRescue4HoursQuantity * route.RoutePrice1);
                            sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Rescue6Hours")
                        {
                            sblDayRescue6HoursQuantity++;
                            //sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + (sblDayRescue6HoursQuantity * route.RoutePrice1);
                            sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "ReDelivery2Hours")
                        {
                            sblDayReDelivery2HoursQuantity++;
                            //sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + (sblDayReDelivery2HoursQuantity * route.RoutePrice1);
                            sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "ReDelivery4Hours")
                        {
                            sblDayReDelivery4HoursQuantity++;
                            //sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + (sblDayReDelivery4HoursQuantity * route.RoutePrice1);
                            sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "ReDelivery6Hours")
                        {
                            sblDayReDelivery6HoursQuantity++;
                            //sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + (sblDayReDelivery6HoursQuantity * route.RoutePrice1);
                            sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Missort2Hours")
                        {
                            sblDayMissort2HoursQuantity++;
                            //sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + (sblDayMissort2HoursQuantity * route.RoutePrice1);
                            sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Missort4Hours")
                        {
                            sblDayMissort4HoursQuantity++;
                            //sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + (sblDayMissort4HoursQuantity * route.RoutePrice1);
                            sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Missort6Hours")
                        {
                            sblDayMissort6HoursQuantity++;
                            //sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + (sblDayMissort6HoursQuantity * route.RoutePrice1);
                            sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "SameDay")
                        {
                            sblDaySameDayQuantity++;
                            //sblDaySameDayPrice = sblDaySameDayPrice + (sblDaySameDayQuantity * route.RoutePrice1);
                            sblDaySameDayPrice = sblDaySameDayPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "TrainingDay")
                        {
                            sblDayTrainingDayQuantity++;
                            //sblDayTrainingDayPrice = sblDayTrainingDayPrice + (sblDayTrainingDayQuantity * route.RoutePrice1);
                            sblDayTrainingDayPrice = sblDayTrainingDayPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "RideAlong")
                        {
                            sblDayRideAlongQuantity++;
                            //sblDayRideAlongPrice = sblDayRideAlongPrice + (sblDayRideAlongQuantity * route.RoutePrice1);
                            sblDayRideAlongPrice = sblDayRideAlongPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "SupportAd1")
                        {
                            sblDaySupportAd1Quantity++;
                            sblDaySupportAd1Price = sblDaySupportAd1Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "SupportAd2")
                        {
                            sblDaySupportAd2Quantity++;
                            sblDaySupportAd2Price = sblDaySupportAd2Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "SupportAd3")
                        {
                            sblDaySupportAd3Quantity++;
                            sblDaySupportAd3Price = sblDaySupportAd3Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "LeadDriver")
                        {
                            sblDayLeadDriverQuantity++;
                            sblDayLeadDriverPrice = sblDayLeadDriverPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "LargeVan")
                        {
                            sblDayLargeVanQuantity++;
                            sblDayLargeVanPrice = sblDayLargeVanPrice + route.RoutePrice1;
                        }


                        //
                        // 2
                        //

                        if (route.RouteType2 == "Full")
                        {
                            sblDayFullQuantity++;
                            //sblDayFullPrice = sblDayFullPrice + (sblDayFullQuantity * route.RoutePrice2);
                            sblDayFullPrice = sblDayFullPrice + route.RoutePrice2;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 1;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayFullQuantity * route.RoutePrice2);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Half")
                        {
                            sblDayHalfQuantity++;
                            //sblDayHalfPrice = sblDayHalfPrice + (sblDayHalfQuantity * route.RoutePrice2);
                            sblDayHalfPrice = sblDayHalfPrice + route.RoutePrice2;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 0.5;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayHalfQuantity * route.RoutePrice2);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "RemoteDebrief")
                        {
                            sblDayRemoteDebriefQuantity++;
                            //sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + (sblDayRemoteDebriefQuantity * route.RoutePrice2);
                            sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "NurseryRoutesLevel1")
                        {
                            sblDayNurseryRoutesLevel1Quantity++;
                            //sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + (sblDayNurseryRoutesLevel1Quantity * route.RoutePrice2);
                            sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "NurseryRoutesLevel2")
                        {
                            sblDayNurseryRoutesLevel2Quantity++;
                            //sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + (sblDayNurseryRoutesLevel2Quantity * route.RoutePrice2);
                            sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Rescue2Hours")
                        {
                            sblDayRescue2HoursQuantity++;
                            //sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + (sblDayRescue2HoursQuantity * route.RoutePrice2);
                            sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Rescue4Hours")
                        {
                            sblDayRescue4HoursQuantity++;
                            //sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + (sblDayRescue4HoursQuantity * route.RoutePrice2);
                            sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Rescue6Hours")
                        {
                            sblDayRescue6HoursQuantity++;
                            //sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + (sblDayRescue6HoursQuantity * route.RoutePrice2);
                            sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "ReDelivery2Hours")
                        {
                            sblDayReDelivery2HoursQuantity++;
                            //sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + (sblDayReDelivery2HoursQuantity * route.RoutePrice2);
                            sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "ReDelivery4Hours")
                        {
                            sblDayReDelivery4HoursQuantity++;
                            //sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + (sblDayReDelivery4HoursQuantity * route.RoutePrice2);
                            sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "ReDelivery6Hours")
                        {
                            sblDayReDelivery6HoursQuantity++;
                            //sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + (sblDayReDelivery6HoursQuantity * route.RoutePrice2);
                            sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Missort2Hours")
                        {
                            sblDayMissort2HoursQuantity++;
                            //sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + (sblDayMissort2HoursQuantity * route.RoutePrice2);
                            sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Missort4Hours")
                        {
                            sblDayMissort4HoursQuantity++;
                            //sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + (sblDayMissort4HoursQuantity * route.RoutePrice2);
                            sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Missort6Hours")
                        {
                            sblDayMissort6HoursQuantity++;
                            //sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + (sblDayMissort6HoursQuantity * route.RoutePrice2);
                            sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SameDay")
                        {
                            sblDaySameDayQuantity++;
                            //sblDaySameDayPrice = sblDaySameDayPrice + (sblDaySameDayQuantity * route.RoutePrice2);
                            sblDaySameDayPrice = sblDaySameDayPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "TrainingDay")
                        {
                            sblDayTrainingDayQuantity++;
                            //sblDayTrainingDayPrice = sblDayTrainingDayPrice + (sblDayTrainingDayQuantity * route.RoutePrice2);
                            sblDayTrainingDayPrice = sblDayTrainingDayPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "RideAlong")
                        {
                            sblDayRideAlongQuantity++;
                            //sblDayRideAlongPrice = sblDayRideAlongPrice + (sblDayRideAlongQuantity * route.RoutePrice2);
                            sblDayRideAlongPrice = sblDayRideAlongPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SupportAd1")
                        {
                            sblDaySupportAd1Quantity++;
                            sblDaySupportAd1Price = sblDaySupportAd1Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SupportAd2")
                        {
                            sblDaySupportAd2Quantity++;
                            sblDaySupportAd2Price = sblDaySupportAd2Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SupportAd3")
                        {
                            sblDaySupportAd3Quantity++;
                            sblDaySupportAd3Price = sblDaySupportAd3Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "LeadDriver")
                        {
                            sblDayLeadDriverQuantity++;
                            sblDayLeadDriverPrice = sblDayLeadDriverPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "LargeVan")
                        {
                            sblDayLargeVanQuantity++;
                            sblDayLargeVanPrice = sblDayLargeVanPrice + route.RoutePrice2;
                        }




                        //
                        // 3
                        //

                        if (route.RouteType3 == "Full")
                        {
                            sblDayFullQuantity++;
                            //sblDayFullPrice = sblDayFullPrice + (sblDayFullQuantity * route.RoutePrice3);
                            sblDayFullPrice = sblDayFullPrice + route.RoutePrice3;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 1;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayFullQuantity * route.RoutePrice3);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Half")
                        {
                            sblDayHalfQuantity++;
                            //sblDayHalfPrice = sblDayHalfPrice + (sblDayHalfQuantity * route.RoutePrice3);
                            sblDayHalfPrice = sblDayHalfPrice + route.RoutePrice3;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 0.5;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayHalfQuantity * route.RoutePrice3);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "RemoteDebrief")
                        {
                            sblDayRemoteDebriefQuantity++;
                            //sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + (sblDayRemoteDebriefQuantity * route.RoutePrice3);
                            sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "NurseryRoutesLevel1")
                        {
                            sblDayNurseryRoutesLevel1Quantity++;
                            //sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + (sblDayNurseryRoutesLevel1Quantity * route.RoutePrice3);
                            sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "NurseryRoutesLevel2")
                        {
                            sblDayNurseryRoutesLevel2Quantity++;
                            //sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + (sblDayNurseryRoutesLevel2Quantity * route.RoutePrice3);
                            sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Rescue2Hours")
                        {
                            sblDayRescue2HoursQuantity++;
                            //sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + (sblDayRescue2HoursQuantity * route.RoutePrice3);
                            sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Rescue4Hours")
                        {
                            sblDayRescue4HoursQuantity++;
                            //sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + (sblDayRescue4HoursQuantity * route.RoutePrice3);
                            sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Rescue6Hours")
                        {
                            sblDayRescue6HoursQuantity++;
                            //sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + (sblDayRescue6HoursQuantity * route.RoutePrice3);
                            sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "ReDelivery2Hours")
                        {
                            sblDayReDelivery2HoursQuantity++;
                            //sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + (sblDayReDelivery2HoursQuantity * route.RoutePrice3);
                            sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "ReDelivery4Hours")
                        {
                            sblDayReDelivery4HoursQuantity++;
                            //sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + (sblDayReDelivery4HoursQuantity * route.RoutePrice3);
                            sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "ReDelivery6Hours")
                        {
                            sblDayReDelivery6HoursQuantity++;
                            //sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + (sblDayReDelivery6HoursQuantity * route.RoutePrice3);
                            sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Missort2Hours")
                        {
                            sblDayMissort2HoursQuantity++;
                            //sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + (sblDayMissort2HoursQuantity * route.RoutePrice3);
                            sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Missort4Hours")
                        {
                            sblDayMissort4HoursQuantity++;
                            //sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + (sblDayMissort4HoursQuantity * route.RoutePrice3);
                            sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Missort6Hours")
                        {
                            sblDayMissort6HoursQuantity++;
                            //sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + (sblDayMissort6HoursQuantity * route.RoutePrice3);
                            sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SameDay")
                        {
                            sblDaySameDayQuantity++;
                            //sblDaySameDayPrice = sblDaySameDayPrice + (sblDaySameDayQuantity * route.RoutePrice3);
                            sblDaySameDayPrice = sblDaySameDayPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "TrainingDay")
                        {
                            sblDayTrainingDayQuantity++;
                            //sblDayTrainingDayPrice = sblDayTrainingDayPrice + (sblDayTrainingDayQuantity * route.RoutePrice3);
                            sblDayTrainingDayPrice = sblDayTrainingDayPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "RideAlong")
                        {
                            sblDayRideAlongQuantity++;
                            //sblDayRideAlongPrice = sblDayRideAlongPrice + (sblDayRideAlongQuantity * route.RoutePrice3);
                            sblDayRideAlongPrice = sblDayRideAlongPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SupportAd1")
                        {
                            sblDaySupportAd1Quantity++;
                            sblDaySupportAd1Price = sblDaySupportAd1Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SupportAd2")
                        {
                            sblDaySupportAd2Quantity++;
                            sblDaySupportAd2Price = sblDaySupportAd2Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SupportAd3")
                        {
                            sblDaySupportAd3Quantity++;
                            sblDaySupportAd3Price = sblDaySupportAd3Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "LeadDriver")
                        {
                            sblDayLeadDriverQuantity++;
                            sblDayLeadDriverPrice = sblDayLeadDriverPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "LargeVan")
                        {
                            sblDayLargeVanQuantity++;
                            sblDayLargeVanPrice = sblDayLargeVanPrice + route.RoutePrice3;
                        }




                        //
                        // 4
                        //

                        if (route.RouteType4 == "Full")
                        {
                            sblDayFullQuantity++;
                            //sblDayFullPrice = sblDayFullPrice + (sblDayFullQuantity * route.RoutePrice4);
                            sblDayFullPrice = sblDayFullPrice + route.RoutePrice4;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 1;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayFullQuantity * route.RoutePrice4);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Half")
                        {
                            sblDayHalfQuantity++;
                            //sblDayHalfPrice = sblDayHalfPrice + (sblDayHalfQuantity * route.RoutePrice4);
                            sblDayHalfPrice = sblDayHalfPrice + route.RoutePrice4;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 0.5;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayHalfQuantity * route.RoutePrice4);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "RemoteDebrief")
                        {
                            sblDayRemoteDebriefQuantity++;
                            //sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + (sblDayRemoteDebriefQuantity * route.RoutePrice4);
                            sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "NurseryRoutesLevel1")
                        {
                            sblDayNurseryRoutesLevel1Quantity++;
                            //sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + (sblDayNurseryRoutesLevel1Quantity * route.RoutePrice4);
                            sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "NurseryRoutesLevel2")
                        {
                            sblDayNurseryRoutesLevel2Quantity++;
                            //sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + (sblDayNurseryRoutesLevel2Quantity * route.RoutePrice4);
                            sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Rescue2Hours")
                        {
                            sblDayRescue2HoursQuantity++;
                            //sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + (sblDayRescue2HoursQuantity * route.RoutePrice4);
                            sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Rescue4Hours")
                        {
                            sblDayRescue4HoursQuantity++;
                            //sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + (sblDayRescue4HoursQuantity * route.RoutePrice4);
                            sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Rescue6Hours")
                        {
                            sblDayRescue6HoursQuantity++;
                            //sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + (sblDayRescue6HoursQuantity * route.RoutePrice4);
                            sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "ReDelivery2Hours")
                        {
                            sblDayReDelivery2HoursQuantity++;
                            //sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + (sblDayReDelivery2HoursQuantity * route.RoutePrice4);
                            sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "ReDelivery4Hours")
                        {
                            sblDayReDelivery4HoursQuantity++;
                            //sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + (sblDayReDelivery4HoursQuantity * route.RoutePrice4);
                            sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "ReDelivery6Hours")
                        {
                            sblDayReDelivery6HoursQuantity++;
                            //sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + (sblDayReDelivery6HoursQuantity * route.RoutePrice4);
                            sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Missort2Hours")
                        {
                            sblDayMissort2HoursQuantity++;
                            //sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + (sblDayMissort2HoursQuantity * route.RoutePrice4);
                            sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Missort4Hours")
                        {
                            sblDayMissort4HoursQuantity++;
                            //sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + (sblDayMissort4HoursQuantity * route.RoutePrice4);
                            sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Missort6Hours")
                        {
                            sblDayMissort6HoursQuantity++;
                            //sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + (sblDayMissort6HoursQuantity * route.RoutePrice4);
                            sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SameDay")
                        {
                            sblDaySameDayQuantity++;
                            //sblDaySameDayPrice = sblDaySameDayPrice + (sblDaySameDayQuantity * route.RoutePrice4);
                            sblDaySameDayPrice = sblDaySameDayPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "TrainingDay")
                        {
                            sblDayTrainingDayQuantity++;
                            //sblDayTrainingDayPrice = sblDayTrainingDayPrice + (sblDayTrainingDayQuantity * route.RoutePrice4);
                            sblDayTrainingDayPrice = sblDayTrainingDayPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "RideAlong")
                        {
                            sblDayRideAlongQuantity++;
                            //sblDayRideAlongPrice = sblDayRideAlongPrice + (sblDayRideAlongQuantity * route.RoutePrice4);
                            sblDayRideAlongPrice = sblDayRideAlongPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SupportAd1")
                        {
                            sblDaySupportAd1Quantity++;
                            sblDaySupportAd1Price = sblDaySupportAd1Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SupportAd2")
                        {
                            sblDaySupportAd2Quantity++;
                            sblDaySupportAd2Price = sblDaySupportAd2Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SupportAd3")
                        {
                            sblDaySupportAd3Quantity++;
                            sblDaySupportAd3Price = sblDaySupportAd3Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "LeadDriver")
                        {
                            sblDayLeadDriverQuantity++;
                            sblDayLeadDriverPrice = sblDayLeadDriverPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "LargeVan")
                        {
                            sblDayLargeVanQuantity++;
                            sblDayLargeVanPrice = sblDayLargeVanPrice + route.RoutePrice4;
                        }



                        //
                        // 5
                        //

                        if (route.RouteType5 == "Full")
                        {
                            sblDayFullQuantity++;
                            //sblDayFullPrice = sblDayFullPrice + (sblDayFullQuantity * route.RoutePrice5);
                            sblDayFullPrice = sblDayFullPrice + route.RoutePrice5;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 1;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayFullQuantity * route.RoutePrice5);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Half")
                        {
                            sblDayHalfQuantity++;
                            //sblDayHalfPrice = sblDayHalfPrice + (sblDayHalfQuantity * route.RoutePrice5);
                            sblDayHalfPrice = sblDayHalfPrice + route.RoutePrice5;

                            // full and half
                            sblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity + 0.5;
                            //sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + (sblDayHalfQuantity * route.RoutePrice5);
                            sblDayFullAndHalfPrice = sblDayFullAndHalfPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "RemoteDebrief")
                        {
                            sblDayRemoteDebriefQuantity++;
                            //sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + (sblDayRemoteDebriefQuantity * route.RoutePrice5);
                            sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "NurseryRoutesLevel1")
                        {
                            sblDayNurseryRoutesLevel1Quantity++;
                            //sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + (sblDayNurseryRoutesLevel1Quantity * route.RoutePrice5);
                            sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "NurseryRoutesLevel2")
                        {
                            sblDayNurseryRoutesLevel2Quantity++;
                            //sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + (sblDayNurseryRoutesLevel2Quantity * route.RoutePrice5);
                            sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Rescue2Hours")
                        {
                            sblDayRescue2HoursQuantity++;
                            //sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + (sblDayRescue2HoursQuantity * route.RoutePrice5);
                            sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Rescue4Hours")
                        {
                            sblDayRescue4HoursQuantity++;
                            //sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + (sblDayRescue4HoursQuantity * route.RoutePrice5);
                            sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Rescue6Hours")
                        {
                            sblDayRescue6HoursQuantity++;
                            //sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + (sblDayRescue6HoursQuantity * route.RoutePrice5);
                            sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "ReDelivery2Hours")
                        {
                            sblDayReDelivery2HoursQuantity++;
                            //sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + (sblDayReDelivery2HoursQuantity * route.RoutePrice5);
                            sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "ReDelivery4Hours")
                        {
                            sblDayReDelivery4HoursQuantity++;
                            //sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + (sblDayReDelivery4HoursQuantity * route.RoutePrice5);
                            sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "ReDelivery6Hours")
                        {
                            sblDayReDelivery6HoursQuantity++;
                            //sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + (sblDayReDelivery6HoursQuantity * route.RoutePrice5);
                            sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Missort2Hours")
                        {
                            sblDayMissort2HoursQuantity++;
                            //sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + (sblDayMissort2HoursQuantity * route.RoutePrice5);
                            sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Missort4Hours")
                        {
                            sblDayMissort4HoursQuantity++;
                            //sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + (sblDayMissort4HoursQuantity * route.RoutePrice5);
                            sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Missort6Hours")
                        {
                            sblDayMissort6HoursQuantity++;
                            //sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + (sblDayMissort6HoursQuantity * route.RoutePrice5);
                            sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "SameDay")
                        {
                            sblDaySameDayQuantity++;
                            //sblDaySameDayPrice = sblDaySameDayPrice + (sblDaySameDayQuantity * route.RoutePrice5);
                            sblDaySameDayPrice = sblDaySameDayPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "TrainingDay")
                        {
                            sblDayTrainingDayQuantity++;
                            //sblDayTrainingDayPrice = sblDayTrainingDayPrice + (sblDayTrainingDayQuantity * route.RoutePrice5);
                            sblDayTrainingDayPrice = sblDayTrainingDayPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "RideAlong")
                        {
                            sblDayRideAlongQuantity++;
                            //sblDayRideAlongPrice = sblDayRideAlongPrice + (sblDayRideAlongQuantity * route.RoutePrice5);
                            sblDayRideAlongPrice = sblDayRideAlongPrice + route.RoutePrice5;
                        }


                        if (route.RouteType5 == "SupportAd1")
                        {
                            sblDaySupportAd1Quantity++;
                            sblDaySupportAd1Price = sblDaySupportAd1Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "SupportAd2")
                        {
                            sblDaySupportAd2Quantity++;
                            sblDaySupportAd2Price = sblDaySupportAd2Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "SupportAd3")
                        {
                            sblDaySupportAd3Quantity++;
                            sblDaySupportAd3Price = sblDaySupportAd3Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "LeadDriver")
                        {
                            sblDayLeadDriverQuantity++;
                            sblDayLeadDriverPrice = sblDayLeadDriverPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "LargeVan")
                        {
                            sblDayLargeVanQuantity++;
                            sblDayLargeVanPrice = sblDayLargeVanPrice + route.RoutePrice5;
                        }
                        
                        sblDayCongestionChargeQuantity = sblDayCongestionChargeQuantity + route.CongestionChargeQuantity;
                        sblDayCongestionChargePrice = sblDayCongestionChargePrice + (sblDayCongestionChargeQuantity * route.CongestionChargePrice);

                        sblDayLatePaymentQuantity = sblDayLatePaymentQuantity + route.LatePaymentQuantity;
                        sblDayLatePaymentPrice = sblDayLatePaymentPrice + (sblDayLatePaymentQuantity * route.LatePaymentPrice);

                        sblDayFuel = sblDayFuel + route.Fuel;
                        sblDayMileage = sblDayMileage + route.Mileage;
                        sblDayDeduct = sblDayDeduct + route.Deduct;


                    }
                }
                #endregion


                #region sblWeekRoutes1


                if (sblRoutes.Any())
                {
                    var sblWeekRoutes = sblRoutes;

                    foreach (var route in sblWeekRoutes)
                    {


                        //
                        // 1
                        //

                        if (route.RouteType1 == "Full")
                        {
                            sblWeekFullQuantity++;
                            //sblWeekFullPrice = sblWeekFullPrice + (sblWeekFullQuantity * route.RoutePrice1);
                            sblWeekFullPrice = sblWeekFullPrice + route.RoutePrice1;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 1;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice1);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Half")
                        {
                            sblWeekHalfQuantity++;
                            //sblWeekHalfPrice = sblWeekHalfPrice + (sblWeekHalfQuantity * route.RoutePrice1);
                            sblWeekHalfPrice = sblWeekHalfPrice + route.RoutePrice1;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 0.5;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice1);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "RemoteDebrief")
                        {
                            sblWeekRemoteDebriefQuantity++;
                            //sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + (sblWeekRemoteDebriefQuantity * route.RoutePrice1);
                            sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "NurseryRoutesLevel1")
                        {
                            sblWeekNurseryRoutesLevel1Quantity++;
                            //sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + (sblWeekNurseryRoutesLevel1Quantity * route.RoutePrice1);
                            sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "NurseryRoutesLevel2")
                        {
                            sblWeekNurseryRoutesLevel2Quantity++;
                            //sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + (sblWeekNurseryRoutesLevel2Quantity * route.RoutePrice1);
                            sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Rescue2Hours")
                        {
                            sblWeekRescue2HoursQuantity++;
                            //sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + (sblWeekRescue2HoursQuantity * route.RoutePrice1);
                            sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Rescue4Hours")
                        {
                            sblWeekRescue4HoursQuantity++;
                            //sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + (sblWeekRescue4HoursQuantity * route.RoutePrice1);
                            sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Rescue6Hours")
                        {
                            sblWeekRescue6HoursQuantity++;
                            //sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + (sblWeekRescue6HoursQuantity * route.RoutePrice1);
                            sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "ReDelivery2Hours")
                        {
                            sblWeekReDelivery2HoursQuantity++;
                            //sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + (sblWeekReDelivery2HoursQuantity * route.RoutePrice1);
                            sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "ReDelivery4Hours")
                        {
                            sblWeekReDelivery4HoursQuantity++;
                            //sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + (sblWeekReDelivery4HoursQuantity * route.RoutePrice1);
                            sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "ReDelivery6Hours")
                        {
                            sblWeekReDelivery6HoursQuantity++;
                            //sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + (sblWeekReDelivery6HoursQuantity * route.RoutePrice1);
                            sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Missort2Hours")
                        {
                            sblWeekMissort2HoursQuantity++;
                            //sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + (sblWeekMissort2HoursQuantity * route.RoutePrice1);
                            sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Missort4Hours")
                        {
                            sblWeekMissort4HoursQuantity++;
                            //sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + (sblWeekMissort4HoursQuantity * route.RoutePrice1);
                            sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "Missort6Hours")
                        {
                            sblWeekMissort6HoursQuantity++;
                            //sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + (sblWeekMissort6HoursQuantity * route.RoutePrice1);
                            sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "SameDay")
                        {
                            sblWeekSameDayQuantity++;
                            //sblWeekSameDayPrice = sblWeekSameDayPrice + (sblWeekSameDayQuantity * route.RoutePrice1);
                            sblWeekSameDayPrice = sblWeekSameDayPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "TrainingDay")
                        {
                            sblWeekTrainingDayQuantity++;
                            //sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + (sblWeekTrainingDayQuantity * route.RoutePrice1);
                            sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "RideAlong")
                        {
                            sblWeekRideAlongQuantity++;
                            //sblWeekRideAlongPrice = sblWeekRideAlongPrice + (sblWeekRideAlongQuantity * route.RoutePrice1);
                            sblWeekRideAlongPrice = sblWeekRideAlongPrice + route.RoutePrice1;
                        }
                        

                        if (route.RouteType1 == "SupportAd1")
                        {
                            sblWeekSupportAd1Quantity++;
                            sblWeekSupportAd1Price = sblWeekSupportAd1Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "SupportAd2")
                        {
                            sblWeekSupportAd2Quantity++;
                            sblWeekSupportAd2Price = sblWeekSupportAd2Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "SupportAd3")
                        {
                            sblWeekSupportAd3Quantity++;
                            sblWeekSupportAd3Price = sblWeekSupportAd3Price + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "LeadDriver")
                        {
                            sblWeekLeadDriverQuantity++;
                            sblWeekLeadDriverPrice = sblWeekLeadDriverPrice + route.RoutePrice1;
                        }

                        if (route.RouteType1 == "LargeVan")
                        {
                            sblWeekLargeVanQuantity++;
                            sblWeekLargeVanPrice = sblWeekLargeVanPrice + route.RoutePrice1;
                        }



                        //
                        // 2
                        //

                        if (route.RouteType2 == "Full")
                        {
                            sblWeekFullQuantity++;
                            //sblWeekFullPrice = sblWeekFullPrice + (sblWeekFullQuantity * route.RoutePrice2);
                            sblWeekFullPrice = sblWeekFullPrice + route.RoutePrice2;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 1;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice2);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Half")
                        {
                            sblWeekHalfQuantity++;
                            //sblWeekHalfPrice = sblWeekHalfPrice + (sblWeekHalfQuantity * route.RoutePrice2);
                            sblWeekHalfPrice = sblWeekHalfPrice + route.RoutePrice2;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 0.5;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice2);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "RemoteDebrief")
                        {
                            sblWeekRemoteDebriefQuantity++;
                            //sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + (sblWeekRemoteDebriefQuantity * route.RoutePrice2);
                            sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "NurseryRoutesLevel1")
                        {
                            sblWeekNurseryRoutesLevel1Quantity++;
                            //sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + (sblWeekNurseryRoutesLevel1Quantity * route.RoutePrice2);
                            sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "NurseryRoutesLevel2")
                        {
                            sblWeekNurseryRoutesLevel2Quantity++;
                            //sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + (sblWeekNurseryRoutesLevel2Quantity * route.RoutePrice2);
                            sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Rescue2Hours")
                        {
                            sblWeekRescue2HoursQuantity++;
                            //sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + (sblWeekRescue2HoursQuantity * route.RoutePrice2);
                            sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Rescue4Hours")
                        {
                            sblWeekRescue4HoursQuantity++;
                            //sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + (sblWeekRescue4HoursQuantity * route.RoutePrice2);
                            sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Rescue6Hours")
                        {
                            sblWeekRescue6HoursQuantity++;
                            //sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + (sblWeekRescue6HoursQuantity * route.RoutePrice2);
                            sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "ReDelivery2Hours")
                        {
                            sblWeekReDelivery2HoursQuantity++;
                            //sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + (sblWeekReDelivery2HoursQuantity * route.RoutePrice2);
                            sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "ReDelivery4Hours")
                        {
                            sblWeekReDelivery4HoursQuantity++;
                            //sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + (sblWeekReDelivery4HoursQuantity * route.RoutePrice2);
                            sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "ReDelivery6Hours")
                        {
                            sblWeekReDelivery6HoursQuantity++;
                            //sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + (sblWeekReDelivery6HoursQuantity * route.RoutePrice2);
                            sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Missort2Hours")
                        {
                            sblWeekMissort2HoursQuantity++;
                            //sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + (sblWeekMissort2HoursQuantity * route.RoutePrice2);
                            sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Missort4Hours")
                        {
                            sblWeekMissort4HoursQuantity++;
                            //sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + (sblWeekMissort4HoursQuantity * route.RoutePrice2);
                            sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "Missort6Hours")
                        {
                            sblWeekMissort6HoursQuantity++;
                            //sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + (sblWeekMissort6HoursQuantity * route.RoutePrice2);
                            sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SameDay")
                        {
                            sblWeekSameDayQuantity++;
                            //sblWeekSameDayPrice = sblWeekSameDayPrice + (sblWeekSameDayQuantity * route.RoutePrice2);
                            sblWeekSameDayPrice = sblWeekSameDayPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "TrainingDay")
                        {
                            sblWeekTrainingDayQuantity++;
                            //sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + (sblWeekTrainingDayQuantity * route.RoutePrice2);
                            sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "RideAlong")
                        {
                            sblWeekRideAlongQuantity++;
                            //sblWeekRideAlongPrice = sblWeekRideAlongPrice + (sblWeekRideAlongQuantity * route.RoutePrice2);
                            sblWeekRideAlongPrice = sblWeekRideAlongPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SupportAd1")
                        {
                            sblWeekSupportAd1Quantity++;
                            sblWeekSupportAd1Price = sblWeekSupportAd1Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SupportAd2")
                        {
                            sblWeekSupportAd2Quantity++;
                            sblWeekSupportAd2Price = sblWeekSupportAd2Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "SupportAd3")
                        {
                            sblWeekSupportAd3Quantity++;
                            sblWeekSupportAd3Price = sblWeekSupportAd3Price + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "LeadDriver")
                        {
                            sblWeekLeadDriverQuantity++;
                            sblWeekLeadDriverPrice = sblWeekLeadDriverPrice + route.RoutePrice2;
                        }

                        if (route.RouteType2 == "LargeVan")
                        {
                            sblWeekLargeVanQuantity++;
                            sblWeekLargeVanPrice = sblWeekLargeVanPrice + route.RoutePrice2;
                        }





                        //
                        // 3
                        //

                        if (route.RouteType3 == "Full")
                        {
                            sblWeekFullQuantity++;
                            //sblWeekFullPrice = sblWeekFullPrice + (sblWeekFullQuantity * route.RoutePrice3);
                            sblWeekFullPrice = sblWeekFullPrice + route.RoutePrice3;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 1;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice3);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Half")
                        {
                            sblWeekHalfQuantity++;
                            //sblWeekHalfPrice = sblWeekHalfPrice + (sblWeekHalfQuantity * route.RoutePrice3);
                            sblWeekHalfPrice = sblWeekHalfPrice + route.RoutePrice3;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 0.5;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice3);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "RemoteDebrief")
                        {
                            sblWeekRemoteDebriefQuantity++;
                            //sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + (sblWeekRemoteDebriefQuantity * route.RoutePrice3);
                            sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "NurseryRoutesLevel1")
                        {
                            sblWeekNurseryRoutesLevel1Quantity++;
                            //sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + (sblWeekNurseryRoutesLevel1Quantity * route.RoutePrice3);
                            sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "NurseryRoutesLevel2")
                        {
                            sblWeekNurseryRoutesLevel2Quantity++;
                            //sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + (sblWeekNurseryRoutesLevel2Quantity * route.RoutePrice3);
                            sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Rescue2Hours")
                        {
                            sblWeekRescue2HoursQuantity++;
                            //sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + (sblWeekRescue2HoursQuantity * route.RoutePrice3);
                            sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Rescue4Hours")
                        {
                            sblWeekRescue4HoursQuantity++;
                            //sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + (sblWeekRescue4HoursQuantity * route.RoutePrice3);
                            sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Rescue6Hours")
                        {
                            sblWeekRescue6HoursQuantity++;
                            //sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + (sblWeekRescue6HoursQuantity * route.RoutePrice3);
                            sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "ReDelivery2Hours")
                        {
                            sblWeekReDelivery2HoursQuantity++;
                            //sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + (sblWeekReDelivery2HoursQuantity * route.RoutePrice3);
                            sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "ReDelivery4Hours")
                        {
                            sblWeekReDelivery4HoursQuantity++;
                            //sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + (sblWeekReDelivery4HoursQuantity * route.RoutePrice3);
                            sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "ReDelivery6Hours")
                        {
                            sblWeekReDelivery6HoursQuantity++;
                            //sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + (sblWeekReDelivery6HoursQuantity * route.RoutePrice3);
                            sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Missort2Hours")
                        {
                            sblWeekMissort2HoursQuantity++;
                            //sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + (sblWeekMissort2HoursQuantity * route.RoutePrice3);
                            sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Missort4Hours")
                        {
                            sblWeekMissort4HoursQuantity++;
                            //sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + (sblWeekMissort4HoursQuantity * route.RoutePrice3);
                            sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "Missort6Hours")
                        {
                            sblWeekMissort6HoursQuantity++;
                            //sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + (sblWeekMissort6HoursQuantity * route.RoutePrice3);
                            sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SameDay")
                        {
                            sblWeekSameDayQuantity++;
                            //sblWeekSameDayPrice = sblWeekSameDayPrice + (sblWeekSameDayQuantity * route.RoutePrice3);
                            sblWeekSameDayPrice = sblWeekSameDayPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "TrainingDay")
                        {
                            sblWeekTrainingDayQuantity++;
                            //sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + (sblWeekTrainingDayQuantity * route.RoutePrice3);
                            sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "RideAlong")
                        {
                            sblWeekRideAlongQuantity++;
                            //sblWeekRideAlongPrice = sblWeekRideAlongPrice + (sblWeekRideAlongQuantity * route.RoutePrice3);
                            sblWeekRideAlongPrice = sblWeekRideAlongPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SupportAd1")
                        {
                            sblWeekSupportAd1Quantity++;
                            sblWeekSupportAd1Price = sblWeekSupportAd1Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SupportAd2")
                        {
                            sblWeekSupportAd2Quantity++;
                            sblWeekSupportAd2Price = sblWeekSupportAd2Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "SupportAd3")
                        {
                            sblWeekSupportAd3Quantity++;
                            sblWeekSupportAd3Price = sblWeekSupportAd3Price + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "LeadDriver")
                        {
                            sblWeekLeadDriverQuantity++;
                            sblWeekLeadDriverPrice = sblWeekLeadDriverPrice + route.RoutePrice3;
                        }

                        if (route.RouteType3 == "LargeVan")
                        {
                            sblWeekLargeVanQuantity++;
                            sblWeekLargeVanPrice = sblWeekLargeVanPrice + route.RoutePrice3;
                        }






                        //
                        // 4
                        //

                        if (route.RouteType4 == "Full")
                        {
                            sblWeekFullQuantity++;
                            //sblWeekFullPrice = sblWeekFullPrice + (sblWeekFullQuantity * route.RoutePrice4);
                            sblWeekFullPrice = sblWeekFullPrice + route.RoutePrice4;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 1;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice4);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Half")
                        {
                            sblWeekHalfQuantity++;
                            //sblWeekHalfPrice = sblWeekHalfPrice + (sblWeekHalfQuantity * route.RoutePrice4);
                            sblWeekHalfPrice = sblWeekHalfPrice + route.RoutePrice4;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 0.5;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice4);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "RemoteDebrief")
                        {
                            sblWeekRemoteDebriefQuantity++;
                            //sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + (sblWeekRemoteDebriefQuantity * route.RoutePrice4);
                            sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "NurseryRoutesLevel1")
                        {
                            sblWeekNurseryRoutesLevel1Quantity++;
                            //sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + (sblWeekNurseryRoutesLevel1Quantity * route.RoutePrice4);
                            sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "NurseryRoutesLevel2")
                        {
                            sblWeekNurseryRoutesLevel2Quantity++;
                            //sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + (sblWeekNurseryRoutesLevel2Quantity * route.RoutePrice4);
                            sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Rescue2Hours")
                        {
                            sblWeekRescue2HoursQuantity++;
                            //sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + (sblWeekRescue2HoursQuantity * route.RoutePrice4);
                            sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Rescue4Hours")
                        {
                            sblWeekRescue4HoursQuantity++;
                            //sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + (sblWeekRescue4HoursQuantity * route.RoutePrice4);
                            sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Rescue6Hours")
                        {
                            sblWeekRescue6HoursQuantity++;
                            //sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + (sblWeekRescue6HoursQuantity * route.RoutePrice4);
                            sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "ReDelivery2Hours")
                        {
                            sblWeekReDelivery2HoursQuantity++;
                            //sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + (sblWeekReDelivery2HoursQuantity * route.RoutePrice4);
                            sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "ReDelivery4Hours")
                        {
                            sblWeekReDelivery4HoursQuantity++;
                            //sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + (sblWeekReDelivery4HoursQuantity * route.RoutePrice4);
                            sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "ReDelivery6Hours")
                        {
                            sblWeekReDelivery6HoursQuantity++;
                            //sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + (sblWeekReDelivery6HoursQuantity * route.RoutePrice4);
                            sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Missort2Hours")
                        {
                            sblWeekMissort2HoursQuantity++;
                            //sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + (sblWeekMissort2HoursQuantity * route.RoutePrice4);
                            sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Missort4Hours")
                        {
                            sblWeekMissort4HoursQuantity++;
                            //sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + (sblWeekMissort4HoursQuantity * route.RoutePrice4);
                            sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "Missort6Hours")
                        {
                            sblWeekMissort6HoursQuantity++;
                            //sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + (sblWeekMissort6HoursQuantity * route.RoutePrice4);
                            sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SameDay")
                        {
                            sblWeekSameDayQuantity++;
                            //sblWeekSameDayPrice = sblWeekSameDayPrice + (sblWeekSameDayQuantity * route.RoutePrice4);
                            sblWeekSameDayPrice = sblWeekSameDayPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "TrainingDay")
                        {
                            sblWeekTrainingDayQuantity++;
                            //sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + (sblWeekTrainingDayQuantity * route.RoutePrice4);
                            sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "RideAlong")
                        {
                            sblWeekRideAlongQuantity++;
                            //sblWeekRideAlongPrice = sblWeekRideAlongPrice + (sblWeekRideAlongQuantity * route.RoutePrice4);
                            sblWeekRideAlongPrice = sblWeekRideAlongPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SupportAd1")
                        {
                            sblWeekSupportAd1Quantity++;
                            sblWeekSupportAd1Price = sblWeekSupportAd1Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SupportAd2")
                        {
                            sblWeekSupportAd2Quantity++;
                            sblWeekSupportAd2Price = sblWeekSupportAd2Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "SupportAd3")
                        {
                            sblWeekSupportAd3Quantity++;
                            sblWeekSupportAd3Price = sblWeekSupportAd3Price + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "LeadDriver")
                        {
                            sblWeekLeadDriverQuantity++;
                            sblWeekLeadDriverPrice = sblWeekLeadDriverPrice + route.RoutePrice4;
                        }

                        if (route.RouteType4 == "LargeVan")
                        {
                            sblWeekLargeVanQuantity++;
                            sblWeekLargeVanPrice = sblWeekLargeVanPrice + route.RoutePrice4;
                        }






                        //
                        // 5
                        //

                        if (route.RouteType5 == "Full")
                        {
                            sblWeekFullQuantity++;
                            //sblWeekFullPrice = sblWeekFullPrice + (sblWeekFullQuantity * route.RoutePrice5);
                            sblWeekFullPrice = sblWeekFullPrice + route.RoutePrice5;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 1;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice5);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Half")
                        {
                            sblWeekHalfQuantity++;
                            //sblWeekHalfPrice = sblWeekHalfPrice + (sblWeekHalfQuantity * route.RoutePrice5);
                            sblWeekHalfPrice = sblWeekHalfPrice + route.RoutePrice5;

                            // full and half
                            sblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity + 0.5;
                            //sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + (sblWeekFullQuantity * route.RoutePrice5);
                            sblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "RemoteDebrief")
                        {
                            sblWeekRemoteDebriefQuantity++;
                            //sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + (sblWeekRemoteDebriefQuantity * route.RoutePrice5);
                            sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "NurseryRoutesLevel1")
                        {
                            sblWeekNurseryRoutesLevel1Quantity++;
                            //sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + (sblWeekNurseryRoutesLevel1Quantity * route.RoutePrice5);
                            sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "NurseryRoutesLevel2")
                        {
                            sblWeekNurseryRoutesLevel2Quantity++;
                            //sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + (sblWeekNurseryRoutesLevel2Quantity * route.RoutePrice5);
                            sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Rescue2Hours")
                        {
                            sblWeekRescue2HoursQuantity++;
                            //sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + (sblWeekRescue2HoursQuantity * route.RoutePrice5);
                            sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Rescue4Hours")
                        {
                            sblWeekRescue4HoursQuantity++;
                            //sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + (sblWeekRescue4HoursQuantity * route.RoutePrice5);
                            sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Rescue6Hours")
                        {
                            sblWeekRescue6HoursQuantity++;
                            //sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + (sblWeekRescue6HoursQuantity * route.RoutePrice5);
                            sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "ReDelivery2Hours")
                        {
                            sblWeekReDelivery2HoursQuantity++;
                            //sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + (sblWeekReDelivery2HoursQuantity * route.RoutePrice5);
                            sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "ReDelivery4Hours")
                        {
                            sblWeekReDelivery4HoursQuantity++;
                            //sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + (sblWeekReDelivery4HoursQuantity * route.RoutePrice5);
                            sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "ReDelivery6Hours")
                        {
                            sblWeekReDelivery6HoursQuantity++;
                            //sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + (sblWeekReDelivery6HoursQuantity * route.RoutePrice5);
                            sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Missort2Hours")
                        {
                            sblWeekMissort2HoursQuantity++;
                            //sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + (sblWeekMissort2HoursQuantity * route.RoutePrice5);
                            sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Missort4Hours")
                        {
                            sblWeekMissort4HoursQuantity++;
                            //sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + (sblWeekMissort4HoursQuantity * route.RoutePrice5);
                            sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "Missort6Hours")
                        {
                            sblWeekMissort6HoursQuantity++;
                            //sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + (sblWeekMissort6HoursQuantity * route.RoutePrice5);
                            sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "SameDay")
                        {
                            sblWeekSameDayQuantity++;
                            //sblWeekSameDayPrice = sblWeekSameDayPrice + (sblWeekSameDayQuantity * route.RoutePrice5);
                            sblWeekSameDayPrice = sblWeekSameDayPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "TrainingDay")
                        {
                            sblWeekTrainingDayQuantity++;
                            //sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + (sblWeekTrainingDayQuantity * route.RoutePrice5);
                            sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "RideAlong")
                        {
                            sblWeekRideAlongQuantity++;
                            //sblWeekRideAlongPrice = sblWeekRideAlongPrice + (sblWeekRideAlongQuantity * route.RoutePrice5);
                            sblWeekRideAlongPrice = sblWeekRideAlongPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "SupportAd1")
                        {
                            sblWeekSupportAd1Quantity++;
                            sblWeekSupportAd1Price = sblWeekSupportAd1Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "SupportAd2")
                        {
                            sblWeekSupportAd2Quantity++;
                            sblWeekSupportAd2Price = sblWeekSupportAd2Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "SupportAd3")
                        {
                            sblWeekSupportAd3Quantity++;
                            sblWeekSupportAd3Price = sblWeekSupportAd3Price + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "LeadDriver")
                        {
                            sblWeekLeadDriverQuantity++;
                            sblWeekLeadDriverPrice = sblWeekLeadDriverPrice + route.RoutePrice5;
                        }

                        if (route.RouteType5 == "LargeVan")
                        {
                            sblWeekLargeVanQuantity++;
                            sblWeekLargeVanPrice = sblWeekLargeVanPrice + route.RoutePrice5;
                        }
                        

                        sblWeekCongestionChargeQuantity = sblWeekCongestionChargeQuantity + route.Ad1Quantity;
                        sblWeekCongestionChargePrice = sblWeekCongestionChargePrice + (sblWeekCongestionChargeQuantity * route.CongestionChargePrice);

                        sblWeekLatePaymentQuantity = sblWeekLatePaymentQuantity + route.LatePaymentQuantity;
                        sblWeekLatePaymentPrice = sblWeekLatePaymentPrice + (sblWeekLatePaymentQuantity * route.LatePaymentPrice);

                        sblWeekFuel = sblWeekFuel + route.Fuel;
                        sblWeekMileage = sblWeekMileage + route.Mileage;
                        sblWeekDeduct = sblWeekDeduct + route.Deduct;


                    }
                }
                #endregion


                #region diff day
                // diff day
                double diffDayFullAndHalfQuantity = amazonDayFullAndHalfQuantity - sblDayFullAndHalfQuantity;
                double diffDayFullAndHalfPrice = amazonDayFullAndHalfPrice - sblDayFullAndHalfPrice;
                //
                double diffDayFullQuantity = amazonDayFullQuantity - sblDayFullQuantity;
                double diffDayFullPrice = amazonDayFullPrice - sblDayFullPrice;
                double diffDayHalfQuantity = amazonDayHalfQuantity - sblDayHalfQuantity;
                double diffDayHalfPrice = amazonDayHalfPrice - sblDayHalfPrice;
                double diffDayRemoteDebriefQuantity = amazonDayRemoteDebriefQuantity - sblDayRemoteDebriefQuantity;
                double diffDayRemoteDebriefPrice = amazonDayRemoteDebriefPrice - sblDayRemoteDebriefPrice;
                double diffDayNurseryRoutesLevel1Quantity = amazonDayNurseryRoutesLevel1Quantity - sblDayNurseryRoutesLevel1Quantity;
                double diffDayNurseryRoutesLevel1Price = amazonDayNurseryRoutesLevel1Price - sblDayNurseryRoutesLevel1Price;
                double diffDayNurseryRoutesLevel2Quantity = amazonDayNurseryRoutesLevel2Quantity - sblDayNurseryRoutesLevel2Quantity;
                double diffDayNurseryRoutesLevel2Price = amazonDayNurseryRoutesLevel2Price - sblDayNurseryRoutesLevel2Price;
                double diffDayRescue2HoursQuantity = amazonDayRescue2HoursQuantity - sblDayRescue2HoursQuantity;
                double diffDayRescue2HoursPrice = amazonDayRescue2HoursPrice - sblDayRescue2HoursPrice;
                double diffDayRescue4HoursQuantity = amazonDayRescue4HoursQuantity - sblDayRescue4HoursQuantity;
                double diffDayRescue4HoursPrice = amazonDayRescue4HoursPrice - sblDayRescue4HoursPrice;
                double diffDayRescue6HoursQuantity = amazonDayRescue6HoursQuantity - sblDayRescue6HoursQuantity;
                double diffDayRescue6HoursPrice = amazonDayRescue6HoursPrice - sblDayRescue6HoursPrice;
                double diffDayReDelivery2HoursQuantity = amazonDayReDelivery2HoursQuantity - sblDayReDelivery2HoursQuantity;
                double diffDayReDelivery2HoursPrice = amazonDayReDelivery2HoursPrice - sblDayReDelivery2HoursPrice;
                double diffDayReDelivery4HoursQuantity = amazonDayReDelivery4HoursQuantity - sblDayReDelivery4HoursQuantity;
                double diffDayReDelivery4HoursPrice = amazonDayReDelivery4HoursPrice - sblDayReDelivery4HoursPrice;
                double diffDayReDelivery6HoursQuantity = amazonDayReDelivery6HoursQuantity - sblDayReDelivery6HoursQuantity;
                double diffDayReDelivery6HoursPrice = amazonDayReDelivery6HoursPrice - sblDayReDelivery6HoursPrice;
                double diffDayMissort2HoursQuantity = amazonDayMissort2HoursQuantity - sblDayMissort2HoursQuantity;
                double diffDayMissort2HoursPrice = amazonDayMissort2HoursPrice - sblDayMissort2HoursPrice;
                double diffDayMissort4HoursQuantity = amazonDayMissort4HoursQuantity - sblDayMissort4HoursQuantity;
                double diffDayMissort4HoursPrice = amazonDayMissort4HoursPrice - sblDayMissort4HoursPrice;
                double diffDayMissort6HoursQuantity = amazonDayMissort6HoursQuantity - sblDayMissort6HoursQuantity;
                double diffDayMissort6HoursPrice = amazonDayMissort6HoursPrice - sblDayMissort6HoursPrice;
                double diffDaySameDayQuantity = amazonDaySameDayQuantity - sblDaySameDayQuantity;
                double diffDaySameDayPrice = amazonDaySameDayPrice - sblDaySameDayPrice;
                double diffDayTrainingDayQuantity = amazonDayTrainingDayQuantity - sblDayTrainingDayQuantity;
                double diffDayTrainingDayPrice = amazonDayTrainingDayPrice - sblDayTrainingDayPrice;
                double diffDayRideAlongQuantity = amazonDayRideAlongQuantity - sblDayRideAlongQuantity;
                double diffDayRideAlongPrice = amazonDayRideAlongPrice - sblDayRideAlongPrice;

                double diffDaySupportAd1Quantity = amazonDaySupportAd1Quantity - sblDaySupportAd1Quantity;
                double diffDaySupportAd1Price = amazonDaySupportAd1Price - sblDaySupportAd1Price;
                double diffDaySupportAd2Quantity = amazonDaySupportAd2Quantity - sblDaySupportAd2Quantity;
                double diffDaySupportAd2Price = amazonDaySupportAd2Price - sblDaySupportAd2Price;
                double diffDaySupportAd3Quantity = amazonDaySupportAd3Quantity - sblDaySupportAd3Quantity;
                double diffDaySupportAd3Price = amazonDaySupportAd3Price - sblDaySupportAd3Price;
                double diffDayLeadDriverQuantity = amazonDayLeadDriverQuantity - sblDayLeadDriverQuantity;
                double diffDayLeadDriverPrice = amazonDayLeadDriverPrice - sblDayLeadDriverPrice;
                double diffDayLargeVanQuantity = amazonDayLargeVanQuantity - sblDayLargeVanQuantity;
                double diffDayLargeVanPrice = amazonDayLargeVanPrice - sblDayLargeVanPrice;

                double diffDayCongestionChargeQuantity = amazonDayCongestionChargeQuantity - sblDayCongestionChargeQuantity;
                double diffDayCongestionChargePrice = amazonDayCongestionChargePrice - sblDayCongestionChargePrice;
                double diffDayLatePaymentQuantity = amazonDayLatePaymentQuantity - sblDayLatePaymentQuantity;
                double diffDayLatePaymentPrice = amazonDayLatePaymentPrice - sblDayLatePaymentPrice;

                double diffDayFuel = amazonDayFuel - sblDayFuel;
                double diffDayMileage = amazonDayMileage - sblDayMileage;
                double diffDayDeduct = amazonDayDeduct - sblDayDeduct;


                // diff week
                diffWeekFullAndHalfQuantity = amazonWeekFullAndHalfQuantity - sblWeekFullAndHalfQuantity;
                diffWeekFullAndHalfPrice = amazonWeekFullAndHalfPrice - sblWeekFullAndHalfPrice;
                //
                diffWeekFullQuantity = amazonWeekFullQuantity - sblWeekFullQuantity;
                diffWeekFullPrice = amazonWeekFullPrice - sblWeekFullPrice;
                diffWeekHalfQuantity = amazonWeekHalfQuantity - sblWeekHalfQuantity;
                diffWeekHalfPrice = amazonWeekHalfPrice - sblWeekHalfPrice;
                diffWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity - sblWeekRemoteDebriefQuantity;
                diffWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice - sblWeekRemoteDebriefPrice;
                diffWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity - sblWeekNurseryRoutesLevel1Quantity;
                diffWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price - sblWeekNurseryRoutesLevel1Price;
                diffWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity - sblWeekNurseryRoutesLevel2Quantity;
                diffWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price - sblWeekNurseryRoutesLevel2Price;
                diffWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity - sblWeekRescue2HoursQuantity;
                diffWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice - sblWeekRescue2HoursPrice;
                diffWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity - sblWeekRescue4HoursQuantity;
                diffWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice - sblWeekRescue4HoursPrice;
                diffWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity - sblWeekRescue6HoursQuantity;
                diffWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice - sblWeekRescue6HoursPrice;
                diffWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity - sblWeekReDelivery2HoursQuantity;
                diffWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice - sblWeekReDelivery2HoursPrice;
                diffWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity - sblWeekReDelivery4HoursQuantity;
                diffWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice - sblWeekReDelivery4HoursPrice;
                diffWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity - sblWeekReDelivery6HoursQuantity;
                diffWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice - sblWeekReDelivery6HoursPrice;
                diffWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity - sblWeekMissort2HoursQuantity;
                diffWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice - sblWeekMissort2HoursPrice;
                diffWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity - sblWeekMissort4HoursQuantity;
                diffWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice - sblWeekMissort4HoursPrice;
                diffWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity - sblWeekMissort6HoursQuantity;
                diffWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice - sblWeekMissort6HoursPrice;
                diffWeekSameDayQuantity = amazonWeekSameDayQuantity - sblWeekSameDayQuantity;
                diffWeekSameDayPrice = amazonWeekSameDayPrice - sblWeekSameDayPrice;
                diffWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity - sblWeekTrainingDayQuantity;
                diffWeekTrainingDayPrice = amazonWeekTrainingDayPrice - sblWeekTrainingDayPrice;
                diffWeekRideAlongQuantity = amazonWeekRideAlongQuantity - sblWeekRideAlongQuantity;
                diffWeekRideAlongPrice = amazonWeekRideAlongPrice - sblWeekRideAlongPrice;

                diffWeekSupportAd1Quantity = amazonWeekSupportAd1Quantity - sblWeekSupportAd1Quantity;
                diffWeekSupportAd1Price = amazonWeekSupportAd1Price - sblWeekSupportAd1Price;
                diffWeekSupportAd2Quantity = amazonWeekSupportAd2Quantity - sblWeekSupportAd2Quantity;
                diffWeekSupportAd2Price = amazonWeekSupportAd2Price - sblWeekSupportAd2Price;
                diffWeekSupportAd3Quantity = amazonWeekSupportAd3Quantity - sblWeekSupportAd3Quantity;
                diffWeekSupportAd3Price = amazonWeekSupportAd3Price - sblWeekSupportAd3Price;
                diffWeekLeadDriverQuantity = amazonWeekLeadDriverQuantity - sblWeekLeadDriverQuantity;
                diffWeekLeadDriverPrice = amazonWeekLeadDriverPrice - sblWeekLeadDriverPrice;
                diffWeekLargeVanQuantity = amazonWeekLargeVanQuantity - sblWeekLargeVanQuantity;
                diffWeekLargeVanPrice = amazonWeekLargeVanPrice - sblWeekLargeVanPrice;

                diffWeekCongestionChargeQuantity = amazonWeekCongestionChargeQuantity - sblWeekCongestionChargeQuantity;
                diffWeekCongestionChargePrice = amazonWeekCongestionChargePrice - sblWeekCongestionChargePrice;
                diffWeekLatePaymentQuantity = amazonWeekLatePaymentQuantity - sblWeekLatePaymentQuantity;
                diffWeekLatePaymentPrice = amazonWeekLatePaymentPrice - sblWeekLatePaymentPrice;

                diffWeekFuel = amazonWeekFuel - sblWeekFuel;
                diffWeekMileage = amazonWeekMileage - sblWeekMileage;
                diffWeekDeduct = amazonWeekDeduct - sblWeekDeduct;
                #endregion


                #region calculate grand totals

                amazonWeekTotalQuantity =
                    +amazonWeekFullQuantity
                    + amazonWeekHalfQuantity
                    + amazonWeekRemoteDebriefQuantity
                    + amazonWeekNurseryRoutesLevel1Quantity
                    + amazonWeekNurseryRoutesLevel2Quantity
                    + amazonWeekRescue2HoursQuantity
                    + amazonWeekRescue4HoursQuantity
                    + amazonWeekRescue6HoursQuantity
                    + amazonWeekReDelivery2HoursQuantity
                    + amazonWeekReDelivery4HoursQuantity
                    + amazonWeekReDelivery6HoursQuantity
                    + amazonWeekMissort2HoursQuantity
                    + amazonWeekMissort4HoursQuantity
                    + amazonWeekMissort6HoursQuantity
                    + amazonWeekSameDayQuantity
                    + amazonWeekTrainingDayQuantity
                    + amazonWeekRideAlongQuantity

                    + amazonWeekSupportAd1Quantity
                    + amazonWeekSupportAd2Quantity
                    + amazonWeekSupportAd3Quantity
                    + amazonWeekLeadDriverQuantity
                    + amazonWeekLargeVanQuantity

                    + amazonWeekCongestionChargeQuantity
                    + amazonWeekLatePaymentQuantity;

                amazonWeekTotalPrice =
                    +amazonWeekFullPrice
                    + amazonWeekHalfPrice
                    + amazonWeekRemoteDebriefPrice
                    + amazonWeekNurseryRoutesLevel1Price
                    + amazonWeekNurseryRoutesLevel2Price
                    + amazonWeekRescue2HoursPrice
                    + amazonWeekRescue4HoursPrice
                    + amazonWeekRescue6HoursPrice
                    + amazonWeekReDelivery2HoursPrice
                    + amazonWeekReDelivery4HoursPrice
                    + amazonWeekReDelivery6HoursPrice
                    + amazonWeekMissort2HoursPrice
                    + amazonWeekMissort4HoursPrice
                    + amazonWeekMissort6HoursPrice
                    + amazonWeekSameDayPrice
                    + amazonWeekTrainingDayPrice
                    + amazonWeekRideAlongPrice

                    + amazonWeekSupportAd1Price
                    + amazonWeekSupportAd2Price
                    + amazonWeekSupportAd3Price
                    + amazonWeekLeadDriverPrice
                    + amazonWeekLargeVanPrice

                    + amazonWeekCongestionChargePrice
                    + amazonWeekLatePaymentPrice;

                sblWeekTotalQuantity =
                    +sblWeekFullQuantity
                    + sblWeekHalfQuantity
                    + sblWeekRemoteDebriefQuantity
                    + sblWeekNurseryRoutesLevel1Quantity
                    + sblWeekNurseryRoutesLevel2Quantity
                    + sblWeekRescue2HoursQuantity
                    + sblWeekRescue4HoursQuantity
                    + sblWeekRescue6HoursQuantity
                    + sblWeekReDelivery2HoursQuantity
                    + sblWeekReDelivery4HoursQuantity
                    + sblWeekReDelivery6HoursQuantity
                    + sblWeekMissort2HoursQuantity
                    + sblWeekMissort4HoursQuantity
                    + sblWeekMissort6HoursQuantity
                    + sblWeekSameDayQuantity
                    + sblWeekTrainingDayQuantity
                    + sblWeekRideAlongQuantity

                    + sblWeekSupportAd1Quantity
                    + sblWeekSupportAd2Quantity
                    + sblWeekSupportAd3Quantity
                    + sblWeekLeadDriverQuantity
                    + sblWeekLargeVanQuantity

                    + sblWeekCongestionChargeQuantity
                    + sblWeekLatePaymentQuantity;

                sblWeekTotalPrice =
                    +sblWeekFullPrice
                    + sblWeekHalfPrice
                    + sblWeekRemoteDebriefPrice
                    + sblWeekNurseryRoutesLevel1Price
                    + sblWeekNurseryRoutesLevel2Price
                    + sblWeekRescue2HoursPrice
                    + sblWeekRescue4HoursPrice
                    + sblWeekRescue6HoursPrice
                    + sblWeekReDelivery2HoursPrice
                    + sblWeekReDelivery4HoursPrice
                    + sblWeekReDelivery6HoursPrice
                    + sblWeekMissort2HoursPrice
                    + sblWeekMissort4HoursPrice
                    + sblWeekMissort6HoursPrice
                    + sblWeekSameDayPrice
                    + sblWeekTrainingDayPrice
                    + sblWeekRideAlongPrice

                    + sblWeekSupportAd1Price
                    + sblWeekSupportAd2Price
                    + sblWeekSupportAd3Price
                    + sblWeekLeadDriverPrice
                    + sblWeekLargeVanPrice

                    + sblWeekCongestionChargePrice
                    + sblWeekLatePaymentPrice;


                diffWeekTotalQuantity =
                    +diffWeekFullQuantity
                    + diffWeekHalfQuantity
                    + diffWeekRemoteDebriefQuantity
                    + diffWeekNurseryRoutesLevel1Quantity
                    + diffWeekNurseryRoutesLevel2Quantity
                    + diffWeekRescue2HoursQuantity
                    + diffWeekRescue4HoursQuantity
                    + diffWeekRescue6HoursQuantity
                    + diffWeekReDelivery2HoursQuantity
                    + diffWeekReDelivery4HoursQuantity
                    + diffWeekReDelivery6HoursQuantity
                    + diffWeekMissort2HoursQuantity
                    + diffWeekMissort4HoursQuantity
                    + diffWeekMissort6HoursQuantity
                    + diffWeekSameDayQuantity
                    + diffWeekTrainingDayQuantity
                    + diffWeekRideAlongQuantity

                    + diffWeekSupportAd1Quantity
                    + diffWeekSupportAd2Quantity
                    + diffWeekSupportAd3Quantity
                    + diffWeekLeadDriverQuantity
                    + diffWeekLargeVanQuantity

                    + diffWeekCongestionChargeQuantity
                    + diffWeekLatePaymentQuantity;

                diffWeekTotalPrice =
                    +diffWeekFullPrice
                    + diffWeekHalfPrice
                    + diffWeekRemoteDebriefPrice
                    + diffWeekNurseryRoutesLevel1Price
                    + diffWeekNurseryRoutesLevel2Price
                    + diffWeekRescue2HoursPrice
                    + diffWeekRescue4HoursPrice
                    + diffWeekRescue6HoursPrice
                    + diffWeekReDelivery2HoursPrice
                    + diffWeekReDelivery4HoursPrice
                    + diffWeekReDelivery6HoursPrice
                    + diffWeekMissort2HoursPrice
                    + diffWeekMissort4HoursPrice
                    + diffWeekMissort6HoursPrice
                    + diffWeekSameDayPrice
                    + diffWeekTrainingDayPrice
                    + diffWeekRideAlongPrice

                    + diffWeekSupportAd1Price
                    + diffWeekSupportAd2Price
                    + diffWeekSupportAd3Price
                    + diffWeekLeadDriverPrice
                    + diffWeekLargeVanPrice

                    + diffWeekCongestionChargePrice
                    + diffWeekLatePaymentPrice;
                #endregion


                // totals list 

                routeAllocationList.Add(new ServiceConfirmationViewModel.Total
                {
                    RouteAmazonId = routeAmazonId,
                    RouteDate = routeDate,
                    DepotId = depotId,
                    //
                    AmazonDayFullAndHalfQuantity = amazonDayFullAndHalfQuantity,
                    AmazonDayFullAndHalfPrice = amazonDayFullAndHalfPrice,
                    //
                    AmazonDayFullQuantity = amazonDayFullQuantity,
                    AmazonDayFullPrice = amazonDayFullPrice,
                    AmazonDayHalfQuantity = amazonDayHalfQuantity,
                    AmazonDayHalfPrice = amazonDayHalfPrice,
                    AmazonDayRemoteDebriefQuantity = amazonDayRemoteDebriefQuantity,
                    AmazonDayRemoteDebriefPrice = amazonDayRemoteDebriefPrice,
                    AmazonDayNurseryRoutesLevel1Quantity = amazonDayNurseryRoutesLevel1Quantity,
                    AmazonDayNurseryRoutesLevel1Price = amazonDayNurseryRoutesLevel1Price,
                    AmazonDayNurseryRoutesLevel2Quantity = amazonDayNurseryRoutesLevel2Quantity,
                    AmazonDayNurseryRoutesLevel2Price = amazonDayNurseryRoutesLevel2Price,
                    AmazonDayRescue2HoursQuantity = amazonDayRescue2HoursQuantity,
                    AmazonDayRescue2HoursPrice = amazonDayRescue2HoursPrice,
                    AmazonDayRescue4HoursQuantity = amazonDayRescue4HoursQuantity,
                    AmazonDayRescue4HoursPrice = amazonDayRescue4HoursPrice,
                    AmazonDayRescue6HoursQuantity = amazonDayRescue6HoursQuantity,
                    AmazonDayRescue6HoursPrice = amazonDayRescue6HoursPrice,
                    AmazonDayReDelivery2HoursQuantity = amazonDayReDelivery2HoursQuantity,
                    AmazonDayReDelivery2HoursPrice = amazonDayReDelivery2HoursPrice,
                    AmazonDayReDelivery4HoursQuantity = amazonDayReDelivery4HoursQuantity,
                    AmazonDayReDelivery4HoursPrice = amazonDayReDelivery4HoursPrice,
                    AmazonDayReDelivery6HoursQuantity = amazonDayReDelivery6HoursQuantity,
                    AmazonDayReDelivery6HoursPrice = amazonDayReDelivery6HoursPrice,
                    AmazonDayMissort2HoursQuantity = amazonDayMissort2HoursQuantity,
                    AmazonDayMissort2HoursPrice = amazonDayMissort2HoursPrice,
                    AmazonDayMissort4HoursQuantity = amazonDayMissort4HoursQuantity,
                    AmazonDayMissort4HoursPrice = amazonDayMissort4HoursPrice,
                    AmazonDayMissort6HoursQuantity = amazonDayMissort6HoursQuantity,
                    AmazonDayMissort6HoursPrice = amazonDayMissort6HoursPrice,
                    AmazonDaySameDayQuantity = amazonDaySameDayQuantity,
                    AmazonDaySameDayPrice = amazonDaySameDayPrice,
                    AmazonDayTrainingDayQuantity = amazonDayTrainingDayQuantity,
                    AmazonDayTrainingDayPrice = amazonDayTrainingDayPrice,
                    AmazonDayRideAlongQuantity = amazonDayRideAlongQuantity,
                    AmazonDayRideAlongPrice = amazonDayRideAlongPrice,

                    AmazonDaySupportAd1Quantity = amazonDaySupportAd1Quantity,
                    AmazonDaySupportAd1Price = amazonDaySupportAd1Price,
                    AmazonDaySupportAd2Quantity = amazonDaySupportAd2Quantity,
                    AmazonDaySupportAd2Price = amazonDaySupportAd2Price,
                    AmazonDaySupportAd3Quantity = amazonDaySupportAd3Quantity,
                    AmazonDaySupportAd3Price = amazonDaySupportAd3Price,
                    AmazonDayLeadDriverQuantity = amazonDayLeadDriverQuantity,
                    AmazonDayLeadDriverPrice = amazonDayLeadDriverPrice,
                    AmazonDayLargeVanQuantity = amazonDayLargeVanQuantity,
                    AmazonDayLargeVanPrice = amazonDayLargeVanPrice,

                    AmazonDayCongestionChargeQuantity = amazonDayCongestionChargeQuantity,
                    AmazonDayCongestionChargePrice = amazonDayCongestionChargePrice,
                    AmazonDayLatePaymentQuantity = amazonDayLatePaymentQuantity,
                    AmazonDayLatePaymentPrice = amazonDayLatePaymentPrice,

                    AmazonDayFuel = amazonDayFuel,
                    AmazonDayMileage = amazonDayMileage,
                    AmazonDayDeduct = amazonDayDeduct,
                    //
                    AmazonWeekFullAndHalfQuantity = amazonWeekFullAndHalfQuantity,
                    AmazonWeekFullAndHalfPrice = amazonWeekFullAndHalfPrice,
                    //
                    AmazonWeekFullQuantity = amazonWeekFullQuantity,
                    AmazonWeekFullPrice = amazonWeekFullPrice,
                    AmazonWeekHalfQuantity = amazonWeekHalfQuantity,
                    AmazonWeekHalfPrice = amazonWeekHalfPrice,
                    AmazonWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity,
                    AmazonWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice,
                    AmazonWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity,
                    AmazonWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price,
                    AmazonWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity,
                    AmazonWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price,
                    AmazonWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity,
                    AmazonWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice,
                    AmazonWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity,
                    AmazonWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice,
                    AmazonWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity,
                    AmazonWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice,
                    AmazonWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity,
                    AmazonWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice,
                    AmazonWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity,
                    AmazonWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice,
                    AmazonWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity,
                    AmazonWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice,
                    AmazonWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity,
                    AmazonWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice,
                    AmazonWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity,
                    AmazonWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice,
                    AmazonWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity,
                    AmazonWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice,
                    AmazonWeekSameDayQuantity = amazonWeekSameDayQuantity,
                    AmazonWeekSameDayPrice = amazonWeekSameDayPrice,
                    AmazonWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity,
                    AmazonWeekTrainingDayPrice = amazonWeekTrainingDayPrice,
                    AmazonWeekRideAlongQuantity = amazonWeekRideAlongQuantity,
                    AmazonWeekRideAlongPrice = amazonWeekRideAlongPrice,

                    AmazonWeekSupportAd1Quantity = amazonWeekSupportAd1Quantity,
                    AmazonWeekSupportAd1Price = amazonWeekSupportAd1Price,
                    AmazonWeekSupportAd2Quantity = amazonWeekSupportAd2Quantity,
                    AmazonWeekSupportAd2Price = amazonWeekSupportAd2Price,
                    AmazonWeekSupportAd3Quantity = amazonWeekSupportAd3Quantity,
                    AmazonWeekSupportAd3Price = amazonWeekSupportAd3Price,
                    AmazonWeekLeadDriverQuantity = amazonWeekLeadDriverQuantity,
                    AmazonWeekLeadDriverPrice = amazonWeekLeadDriverPrice,
                    AmazonWeekLargeVanQuantity = amazonWeekLargeVanQuantity,
                    AmazonWeekLargeVanPrice = amazonWeekLargeVanPrice,

                    AmazonWeekCongestionChargeQuantity = amazonWeekCongestionChargeQuantity,
                    AmazonWeekCongestionChargePrice = amazonWeekCongestionChargePrice,
                    AmazonWeekLatePaymentQuantity = amazonWeekLatePaymentQuantity,
                    AmazonWeekLatePaymentPrice = amazonWeekLatePaymentPrice,

                    AmazonWeekFuel = amazonWeekFuel,
                    AmazonWeekMileage = amazonWeekMileage,
                    AmazonWeekDeduct = amazonWeekDeduct,
                    //
                    SblDayFullAndHalfQuantity = sblDayFullAndHalfQuantity,
                    SblDayFullAndHalfPrice = sblDayFullAndHalfPrice,
                    //
                    SblDayFullQuantity = sblDayFullQuantity,
                    SblDayFullPrice = sblDayFullPrice,
                    SblDayHalfQuantity = sblDayHalfQuantity,
                    SblDayHalfPrice = sblDayHalfPrice,
                    SblDayRemoteDebriefQuantity = sblDayRemoteDebriefQuantity,
                    SblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice,
                    SblDayNurseryRoutesLevel1Quantity = sblDayNurseryRoutesLevel1Quantity,
                    SblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price,
                    SblDayNurseryRoutesLevel2Quantity = sblDayNurseryRoutesLevel2Quantity,
                    SblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price,
                    SblDayRescue2HoursQuantity = sblDayRescue2HoursQuantity,
                    SblDayRescue2HoursPrice = sblDayRescue2HoursPrice,
                    SblDayRescue4HoursQuantity = sblDayRescue4HoursQuantity,
                    SblDayRescue4HoursPrice = sblDayRescue4HoursPrice,
                    SblDayRescue6HoursQuantity = sblDayRescue6HoursQuantity,
                    SblDayRescue6HoursPrice = sblDayRescue6HoursPrice,
                    SblDayReDelivery2HoursQuantity = sblDayReDelivery2HoursQuantity,
                    SblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice,
                    SblDayReDelivery4HoursQuantity = sblDayReDelivery4HoursQuantity,
                    SblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice,
                    SblDayReDelivery6HoursQuantity = sblDayReDelivery6HoursQuantity,
                    SblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice,
                    SblDayMissort2HoursQuantity = sblDayMissort2HoursQuantity,
                    SblDayMissort2HoursPrice = sblDayMissort2HoursPrice,
                    SblDayMissort4HoursQuantity = sblDayMissort4HoursQuantity,
                    SblDayMissort4HoursPrice = sblDayMissort4HoursPrice,
                    SblDayMissort6HoursQuantity = sblDayMissort6HoursQuantity,
                    SblDayMissort6HoursPrice = sblDayMissort6HoursPrice,
                    SblDaySameDayQuantity = sblDaySameDayQuantity,
                    SblDaySameDayPrice = sblDaySameDayPrice,
                    SblDayTrainingDayQuantity = sblDayTrainingDayQuantity,
                    SblDayTrainingDayPrice = sblDayTrainingDayPrice,
                    SblDayRideAlongQuantity = sblDayRideAlongQuantity,
                    SblDayRideAlongPrice = sblDayRideAlongPrice,

                    SblDaySupportAd1Quantity = sblDaySupportAd1Quantity,
                    SblDaySupportAd1Price = sblDaySupportAd1Price,
                    SblDaySupportAd2Quantity = sblDaySupportAd2Quantity,
                    SblDaySupportAd2Price = sblDaySupportAd2Price,
                    SblDaySupportAd3Quantity = sblDaySupportAd3Quantity,
                    SblDaySupportAd3Price = sblDaySupportAd3Price,
                    SblDayLeadDriverQuantity = sblDayLeadDriverQuantity,
                    SblDayLeadDriverPrice = sblDayLeadDriverPrice,
                    SblDayLargeVanQuantity = sblDayLargeVanQuantity,
                    SblDayLargeVanPrice = sblDayLargeVanPrice,

                    SblDayCongestionChargeQuantity = sblDayCongestionChargeQuantity,
                    SblDayCongestionChargePrice = sblDayCongestionChargePrice,
                    SblDayLatePaymentQuantity = sblDayLatePaymentQuantity,
                    SblDayLatePaymentPrice = sblDayLatePaymentPrice,

                    SblDayFuel = sblDayFuel,
                    SblDayMileage = sblDayMileage,
                    SblDayDeduct = sblDayDeduct,
                    //
                    SblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity,
                    SblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice,
                    //
                    SblWeekFullQuantity = sblWeekFullQuantity,
                    SblWeekFullPrice = sblWeekFullPrice,
                    SblWeekHalfQuantity = sblWeekHalfQuantity,
                    SblWeekHalfPrice = sblWeekHalfPrice,
                    SblWeekRemoteDebriefQuantity = sblWeekRemoteDebriefQuantity,
                    SblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice,
                    SblWeekNurseryRoutesLevel1Quantity = sblWeekNurseryRoutesLevel1Quantity,
                    SblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price,
                    SblWeekNurseryRoutesLevel2Quantity = sblWeekNurseryRoutesLevel2Quantity,
                    SblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price,
                    SblWeekRescue2HoursQuantity = sblWeekRescue2HoursQuantity,
                    SblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice,
                    SblWeekRescue4HoursQuantity = sblWeekRescue4HoursQuantity,
                    SblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice,
                    SblWeekRescue6HoursQuantity = sblWeekRescue6HoursQuantity,
                    SblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice,
                    SblWeekReDelivery2HoursQuantity = sblWeekReDelivery2HoursQuantity,
                    SblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice,
                    SblWeekReDelivery4HoursQuantity = sblWeekReDelivery4HoursQuantity,
                    SblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice,
                    SblWeekReDelivery6HoursQuantity = sblWeekReDelivery6HoursQuantity,
                    SblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice,
                    SblWeekMissort2HoursQuantity = sblWeekMissort2HoursQuantity,
                    SblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice,
                    SblWeekMissort4HoursQuantity = sblWeekMissort4HoursQuantity,
                    SblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice,
                    SblWeekMissort6HoursQuantity = sblWeekMissort6HoursQuantity,
                    SblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice,
                    SblWeekSameDayQuantity = sblWeekSameDayQuantity,
                    SblWeekSameDayPrice = sblWeekSameDayPrice,
                    SblWeekTrainingDayQuantity = sblWeekTrainingDayQuantity,
                    SblWeekTrainingDayPrice = sblWeekTrainingDayPrice,
                    SblWeekRideAlongQuantity = sblWeekRideAlongQuantity,
                    SblWeekRideAlongPrice = sblWeekRideAlongPrice,

                    SblWeekSupportAd1Quantity = sblWeekSupportAd1Quantity,
                    SblWeekSupportAd1Price = sblWeekSupportAd1Price,
                    SblWeekSupportAd2Quantity = sblWeekSupportAd2Quantity,
                    SblWeekSupportAd2Price = sblWeekSupportAd2Price,
                    SblWeekSupportAd3Quantity = sblWeekSupportAd3Quantity,
                    SblWeekSupportAd3Price = sblWeekSupportAd3Price,
                    SblWeekLeadDriverQuantity = sblWeekLeadDriverQuantity,
                    SblWeekLeadDriverPrice = sblWeekLeadDriverPrice,
                    SblWeekLargeVanQuantity = sblWeekLargeVanQuantity,
                    SblWeekLargeVanPrice = sblWeekLargeVanPrice,

                    SblWeekCongestionChargeQuantity = sblWeekCongestionChargeQuantity,
                    SblWeekCongestionChargePrice = sblWeekCongestionChargePrice,
                    SblWeekLatePaymentQuantity = sblWeekLatePaymentQuantity,
                    SblWeekLatePaymentPrice = sblWeekLatePaymentPrice,

                    SblWeekFuel = sblWeekFuel,
                    SblWeekMileage = sblWeekMileage,
                    SblWeekDeduct = sblWeekDeduct,
                    //
                    DiffDayFullAndHalfQuantity = diffDayFullAndHalfQuantity,
                    DiffDayFullAndHalfPrice = diffDayFullAndHalfPrice,
                    //
                    DiffDayFullQuantity = diffDayFullQuantity,
                    DiffDayFullPrice = diffDayFullPrice,
                    DiffDayHalfQuantity = diffDayHalfQuantity,
                    DiffDayHalfPrice = diffDayHalfPrice,
                    DiffDayRemoteDebriefQuantity = diffDayRemoteDebriefQuantity,
                    DiffDayRemoteDebriefPrice = diffDayRemoteDebriefPrice,
                    DiffDayNurseryRoutesLevel1Quantity = diffDayNurseryRoutesLevel1Quantity,
                    DiffDayNurseryRoutesLevel1Price = diffDayNurseryRoutesLevel1Price,
                    DiffDayNurseryRoutesLevel2Quantity = diffDayNurseryRoutesLevel2Quantity,
                    DiffDayNurseryRoutesLevel2Price = diffDayNurseryRoutesLevel2Price,
                    DiffDayRescue2HoursQuantity = diffDayRescue2HoursQuantity,
                    DiffDayRescue2HoursPrice = diffDayRescue2HoursPrice,
                    DiffDayRescue4HoursQuantity = diffDayRescue4HoursQuantity,
                    DiffDayRescue4HoursPrice = diffDayRescue4HoursPrice,
                    DiffDayRescue6HoursQuantity = diffDayRescue6HoursQuantity,
                    DiffDayRescue6HoursPrice = diffDayRescue6HoursPrice,
                    DiffDayReDelivery2HoursQuantity = diffDayReDelivery2HoursQuantity,
                    DiffDayReDelivery2HoursPrice = diffDayReDelivery2HoursPrice,
                    DiffDayReDelivery4HoursQuantity = diffDayReDelivery4HoursQuantity,
                    DiffDayReDelivery4HoursPrice = diffDayReDelivery4HoursPrice,
                    DiffDayReDelivery6HoursQuantity = diffDayReDelivery6HoursQuantity,
                    DiffDayReDelivery6HoursPrice = diffDayReDelivery6HoursPrice,
                    DiffDayMissort2HoursQuantity = diffDayMissort2HoursQuantity,
                    DiffDayMissort2HoursPrice = diffDayMissort2HoursPrice,
                    DiffDayMissort4HoursQuantity = diffDayMissort4HoursQuantity,
                    DiffDayMissort4HoursPrice = diffDayMissort4HoursPrice,
                    DiffDayMissort6HoursQuantity = diffDayMissort6HoursQuantity,
                    DiffDayMissort6HoursPrice = diffDayMissort6HoursPrice,
                    DiffDaySameDayQuantity = diffDaySameDayQuantity,
                    DiffDaySameDayPrice = diffDaySameDayPrice,
                    DiffDayTrainingDayQuantity = diffDayTrainingDayQuantity,
                    DiffDayTrainingDayPrice = diffDayTrainingDayPrice,
                    DiffDayRideAlongQuantity = diffDayRideAlongQuantity,
                    DiffDayRideAlongPrice = diffDayRideAlongPrice,

                    DiffDaySupportAd1Quantity = diffDaySupportAd1Quantity,
                    DiffDaySupportAd1Price = diffDaySupportAd1Price,
                    DiffDaySupportAd2Quantity = diffDaySupportAd2Quantity,
                    DiffDaySupportAd2Price = diffDaySupportAd2Price,
                    DiffDaySupportAd3Quantity = diffDaySupportAd3Quantity,
                    DiffDaySupportAd3Price = diffDaySupportAd3Price,
                    DiffDayLeadDriverQuantity = diffDayLeadDriverQuantity,
                    DiffDayLeadDriverPrice = diffDayLeadDriverPrice,
                    DiffDayLargeVanQuantity = diffDayLargeVanQuantity,
                    DiffDayLargeVanPrice = diffDayLargeVanPrice,

                    DiffDayCongestionChargeQuantity = diffDayCongestionChargeQuantity,
                    DiffDayCongestionChargePrice = diffDayCongestionChargePrice,
                    DiffDayLatePaymentQuantity = diffDayLatePaymentQuantity,
                    DiffDayLatePaymentPrice = diffDayLatePaymentPrice,

                    DiffDayFuel = diffDayFuel,
                    DiffDayMileage = diffDayMileage,
                    DiffDayDeduct = diffDayDeduct,
                    //
                    DiffWeekFullAndHalfQuantity = diffWeekFullAndHalfQuantity,
                    DiffWeekFullAndHalfPrice = diffWeekFullAndHalfPrice,
                    //
                    DiffWeekFullQuantity = diffWeekFullQuantity,
                    DiffWeekFullPrice = diffWeekFullPrice,
                    DiffWeekHalfQuantity = diffWeekHalfQuantity,
                    DiffWeekHalfPrice = diffWeekHalfPrice,
                    DiffWeekRemoteDebriefQuantity = diffWeekRemoteDebriefQuantity,
                    DiffWeekRemoteDebriefPrice = diffWeekRemoteDebriefPrice,
                    DiffWeekNurseryRoutesLevel1Quantity = diffWeekNurseryRoutesLevel1Quantity,
                    DiffWeekNurseryRoutesLevel1Price = diffWeekNurseryRoutesLevel1Price,
                    DiffWeekNurseryRoutesLevel2Quantity = diffWeekNurseryRoutesLevel2Quantity,
                    DiffWeekNurseryRoutesLevel2Price = diffWeekNurseryRoutesLevel2Price,
                    DiffWeekRescue2HoursQuantity = diffWeekRescue2HoursQuantity,
                    DiffWeekRescue2HoursPrice = diffWeekRescue2HoursPrice,
                    DiffWeekRescue4HoursQuantity = diffWeekRescue4HoursQuantity,
                    DiffWeekRescue4HoursPrice = diffWeekRescue4HoursPrice,
                    DiffWeekRescue6HoursQuantity = diffWeekRescue6HoursQuantity,
                    DiffWeekRescue6HoursPrice = diffWeekRescue6HoursPrice,
                    DiffWeekReDelivery2HoursQuantity = diffWeekReDelivery2HoursQuantity,
                    DiffWeekReDelivery2HoursPrice = diffWeekReDelivery2HoursPrice,
                    DiffWeekReDelivery4HoursQuantity = diffWeekReDelivery4HoursQuantity,
                    DiffWeekReDelivery4HoursPrice = diffWeekReDelivery4HoursPrice,
                    DiffWeekReDelivery6HoursQuantity = diffWeekReDelivery6HoursQuantity,
                    DiffWeekReDelivery6HoursPrice = diffWeekReDelivery6HoursPrice,
                    DiffWeekMissort2HoursQuantity = diffWeekMissort2HoursQuantity,
                    DiffWeekMissort2HoursPrice = diffWeekMissort2HoursPrice,
                    DiffWeekMissort4HoursQuantity = diffWeekMissort4HoursQuantity,
                    DiffWeekMissort4HoursPrice = diffWeekMissort4HoursPrice,
                    DiffWeekMissort6HoursQuantity = diffWeekMissort6HoursQuantity,
                    DiffWeekMissort6HoursPrice = diffWeekMissort6HoursPrice,
                    DiffWeekSameDayQuantity = diffWeekSameDayQuantity,
                    DiffWeekSameDayPrice = diffWeekSameDayPrice,
                    DiffWeekTrainingDayQuantity = diffWeekTrainingDayQuantity,
                    DiffWeekTrainingDayPrice = diffWeekTrainingDayPrice,
                    DiffWeekRideAlongQuantity = diffWeekRideAlongQuantity,
                    DiffWeekRideAlongPrice = diffWeekRideAlongPrice,

                    DiffWeekSupportAd1Quantity = diffWeekSupportAd1Quantity,
                    DiffWeekSupportAd1Price = diffWeekSupportAd1Price,
                    DiffWeekSupportAd2Quantity = diffWeekSupportAd2Quantity,
                    DiffWeekSupportAd2Price = diffWeekSupportAd2Price,
                    DiffWeekSupportAd3Quantity = diffWeekSupportAd3Quantity,
                    DiffWeekSupportAd3Price = diffWeekSupportAd3Price,
                    DiffWeekLeadDriverQuantity = diffWeekLeadDriverQuantity,
                    DiffWeekLeadDriverPrice = diffWeekLeadDriverPrice,
                    DiffWeekLargeVanQuantity = diffWeekLargeVanQuantity,
                    DiffWeekLargeVanPrice = diffWeekLargeVanPrice,

                    DiffWeekCongestionChargeQuantity = diffWeekCongestionChargeQuantity,
                    DiffWeekCongestionChargePrice = diffWeekCongestionChargePrice,
                    DiffWeekLatePaymentQuantity = diffWeekLatePaymentQuantity,
                    DiffWeekLatePaymentPrice = diffWeekLatePaymentPrice,

                    DiffWeekFuel = diffWeekFuel,
                    DiffWeekMileage = diffWeekMileage,
                    DiffWeekDeduct = diffWeekDeduct,
                    //
                    AmazonWeekTotalQuantity = amazonWeekTotalQuantity,
                    AmazonWeekTotalPrice = amazonWeekTotalPrice,
                    SblWeekTotalQuantity = sblWeekTotalQuantity,
                    SblWeekTotalPrice = sblWeekTotalPrice,
                    DiffWeekTotalQuantity = diffWeekTotalQuantity,
                    DiffWeekTotalPrice = diffWeekTotalPrice
                });

                // end loop

            }
            #endregion

            var viewModel = new ServiceConfirmationViewModel
            {
                DepotId = depotId,
                DateStart = focusDate,
                Days = periodDays,
                //
                Periods = periodList,
                Depots = depots,
                //ThisWeek = dateWeek,
                //ThisDay = DateTime.Now.Date,
                HasWritePermision = hasWritePermision,

                Totals = routeAllocationList,

                //
                AmazonWeekFullAndHalfQuantity = amazonWeekFullAndHalfQuantity,
                AmazonWeekFullAndHalfPrice = amazonWeekFullAndHalfPrice,
                //
                AmazonWeekFullQuantity = amazonWeekFullQuantity,
                AmazonWeekFullPrice = amazonWeekFullPrice,
                AmazonWeekHalfQuantity = amazonWeekHalfQuantity,
                AmazonWeekHalfPrice = amazonWeekHalfPrice,
                AmazonWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity,
                AmazonWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice,
                AmazonWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity,
                AmazonWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price,
                AmazonWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity,
                AmazonWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price,
                AmazonWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity,
                AmazonWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice,
                AmazonWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity,
                AmazonWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice,
                AmazonWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity,
                AmazonWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice,
                AmazonWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity,
                AmazonWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice,
                AmazonWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity,
                AmazonWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice,
                AmazonWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity,
                AmazonWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice,
                AmazonWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity,
                AmazonWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice,
                AmazonWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity,
                AmazonWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice,
                AmazonWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity,
                AmazonWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice,
                AmazonWeekSameDayQuantity = amazonWeekSameDayQuantity,
                AmazonWeekSameDayPrice = amazonWeekSameDayPrice,
                AmazonWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity,
                AmazonWeekTrainingDayPrice = amazonWeekTrainingDayPrice,
                AmazonWeekRideAlongQuantity = amazonWeekRideAlongQuantity,
                AmazonWeekRideAlongPrice = amazonWeekRideAlongPrice,

                AmazonWeekSupportAd1Quantity = amazonWeekSupportAd1Quantity,
                AmazonWeekSupportAd1Price = amazonWeekSupportAd1Price,
                AmazonWeekSupportAd2Quantity = amazonWeekSupportAd2Quantity,
                AmazonWeekSupportAd2Price = amazonWeekSupportAd2Price,
                AmazonWeekSupportAd3Quantity = amazonWeekSupportAd3Quantity,
                AmazonWeekSupportAd3Price = amazonWeekSupportAd3Price,
                AmazonWeekLeadDriverQuantity = amazonWeekLeadDriverQuantity,
                AmazonWeekLeadDriverPrice = amazonWeekLeadDriverPrice,
                AmazonWeekLargeVanQuantity = amazonWeekLargeVanQuantity,
                AmazonWeekLargeVanPrice = amazonWeekLargeVanPrice,

                AmazonWeekCongestionChargeQuantity = amazonWeekCongestionChargeQuantity,
                AmazonWeekCongestionChargePrice = amazonWeekCongestionChargePrice,
                AmazonWeekLatePaymentQuantity = amazonWeekLatePaymentQuantity,
                AmazonWeekLatePaymentPrice = amazonWeekLatePaymentPrice,

                AmazonWeekFuel = amazonWeekFuel,
                AmazonWeekMileage = amazonWeekMileage,
                AmazonWeekDeduct = amazonWeekDeduct,
                //
                SblWeekFullAndHalfQuantity = sblWeekFullAndHalfQuantity,
                SblWeekFullAndHalfPrice = sblWeekFullAndHalfPrice,
                //
                SblWeekFullQuantity = sblWeekFullQuantity,
                SblWeekFullPrice = sblWeekFullPrice,
                SblWeekHalfQuantity = sblWeekHalfQuantity,
                SblWeekHalfPrice = sblWeekHalfPrice,
                SblWeekRemoteDebriefQuantity = sblWeekRemoteDebriefQuantity,
                SblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice,
                SblWeekNurseryRoutesLevel1Quantity = sblWeekNurseryRoutesLevel1Quantity,
                SblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price,
                SblWeekNurseryRoutesLevel2Quantity = sblWeekNurseryRoutesLevel2Quantity,
                SblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price,
                SblWeekRescue2HoursQuantity = sblWeekRescue2HoursQuantity,
                SblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice,
                SblWeekRescue4HoursQuantity = sblWeekRescue4HoursQuantity,
                SblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice,
                SblWeekRescue6HoursQuantity = sblWeekRescue6HoursQuantity,
                SblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice,
                SblWeekReDelivery2HoursQuantity = sblWeekReDelivery2HoursQuantity,
                SblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice,
                SblWeekReDelivery4HoursQuantity = sblWeekReDelivery4HoursQuantity,
                SblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice,
                SblWeekReDelivery6HoursQuantity = sblWeekReDelivery6HoursQuantity,
                SblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice,
                SblWeekMissort2HoursQuantity = sblWeekMissort2HoursQuantity,
                SblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice,
                SblWeekMissort4HoursQuantity = sblWeekMissort4HoursQuantity,
                SblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice,
                SblWeekMissort6HoursQuantity = sblWeekMissort6HoursQuantity,
                SblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice,
                SblWeekSameDayQuantity = sblWeekSameDayQuantity,
                SblWeekSameDayPrice = sblWeekSameDayPrice,
                SblWeekTrainingDayQuantity = sblWeekTrainingDayQuantity,
                SblWeekTrainingDayPrice = sblWeekTrainingDayPrice,
                SblWeekRideAlongQuantity = sblWeekRideAlongQuantity,
                SblWeekRideAlongPrice = sblWeekRideAlongPrice,

                SblWeekSupportAd1Quantity = sblWeekSupportAd1Quantity,
                SblWeekSupportAd1Price = sblWeekSupportAd1Price,
                SblWeekSupportAd2Quantity = sblWeekSupportAd2Quantity,
                SblWeekSupportAd2Price = sblWeekSupportAd2Price,
                SblWeekSupportAd3Quantity = sblWeekSupportAd3Quantity,
                SblWeekSupportAd3Price = sblWeekSupportAd3Price,
                SblWeekLeadDriverQuantity = sblWeekLeadDriverQuantity,
                SblWeekLeadDriverPrice = sblWeekLeadDriverPrice,
                SblWeekLargeVanQuantity = sblWeekLargeVanQuantity,
                SblWeekLargeVanPrice = sblWeekLargeVanPrice,

                SblWeekCongestionChargeQuantity = sblWeekCongestionChargeQuantity,
                SblWeekCongestionChargePrice = sblWeekCongestionChargePrice,
                SblWeekLatePaymentQuantity = sblWeekLatePaymentQuantity,
                SblWeekLatePaymentPrice = sblWeekLatePaymentPrice,

                SblWeekFuel = sblWeekFuel,
                SblWeekMileage = sblWeekMileage,
                SblWeekDeduct = sblWeekDeduct,
                //
                DiffWeekFullAndHalfQuantity = diffWeekFullAndHalfQuantity,
                DiffWeekFullAndHalfPrice = diffWeekFullAndHalfPrice,
                //
                DiffWeekFullQuantity = diffWeekFullQuantity,
                DiffWeekFullPrice = diffWeekFullPrice,
                DiffWeekHalfQuantity = diffWeekHalfQuantity,
                DiffWeekHalfPrice = diffWeekHalfPrice,
                DiffWeekRemoteDebriefQuantity = diffWeekRemoteDebriefQuantity,
                DiffWeekRemoteDebriefPrice = diffWeekRemoteDebriefPrice,
                DiffWeekNurseryRoutesLevel1Quantity = diffWeekNurseryRoutesLevel1Quantity,
                DiffWeekNurseryRoutesLevel1Price = diffWeekNurseryRoutesLevel1Price,
                DiffWeekNurseryRoutesLevel2Quantity = diffWeekNurseryRoutesLevel2Quantity,
                DiffWeekNurseryRoutesLevel2Price = diffWeekNurseryRoutesLevel2Price,
                DiffWeekRescue2HoursQuantity = diffWeekRescue2HoursQuantity,
                DiffWeekRescue2HoursPrice = diffWeekRescue2HoursPrice,
                DiffWeekRescue4HoursQuantity = diffWeekRescue4HoursQuantity,
                DiffWeekRescue4HoursPrice = diffWeekRescue4HoursPrice,
                DiffWeekRescue6HoursQuantity = diffWeekRescue6HoursQuantity,
                DiffWeekRescue6HoursPrice = diffWeekRescue6HoursPrice,
                DiffWeekReDelivery2HoursQuantity = diffWeekReDelivery2HoursQuantity,
                DiffWeekReDelivery2HoursPrice = diffWeekReDelivery2HoursPrice,
                DiffWeekReDelivery4HoursQuantity = diffWeekReDelivery4HoursQuantity,
                DiffWeekReDelivery4HoursPrice = diffWeekReDelivery4HoursPrice,
                DiffWeekReDelivery6HoursQuantity = diffWeekReDelivery6HoursQuantity,
                DiffWeekReDelivery6HoursPrice = diffWeekReDelivery6HoursPrice,
                DiffWeekMissort2HoursQuantity = diffWeekMissort2HoursQuantity,
                DiffWeekMissort2HoursPrice = diffWeekMissort2HoursPrice,
                DiffWeekMissort4HoursQuantity = diffWeekMissort4HoursQuantity,
                DiffWeekMissort4HoursPrice = diffWeekMissort4HoursPrice,
                DiffWeekMissort6HoursQuantity = diffWeekMissort6HoursQuantity,
                DiffWeekMissort6HoursPrice = diffWeekMissort6HoursPrice,
                DiffWeekSameDayQuantity = diffWeekSameDayQuantity,
                DiffWeekSameDayPrice = diffWeekSameDayPrice,
                DiffWeekTrainingDayQuantity = diffWeekTrainingDayQuantity,
                DiffWeekTrainingDayPrice = diffWeekTrainingDayPrice,
                DiffWeekRideAlongQuantity = diffWeekRideAlongQuantity,
                DiffWeekRideAlongPrice = diffWeekRideAlongPrice,

                DiffWeekSupportAd1Quantity = diffWeekSupportAd1Quantity,
                DiffWeekSupportAd1Price = diffWeekSupportAd1Price,
                DiffWeekSupportAd2Quantity = diffWeekSupportAd2Quantity,
                DiffWeekSupportAd2Price = diffWeekSupportAd2Price,
                DiffWeekSupportAd3Quantity = diffWeekSupportAd3Quantity,
                DiffWeekSupportAd3Price = diffWeekSupportAd3Price,
                DiffWeekLeadDriverQuantity = diffWeekLeadDriverQuantity,
                DiffWeekLeadDriverPrice = diffWeekLeadDriverPrice,
                DiffWeekLargeVanQuantity = diffWeekLargeVanQuantity,
                DiffWeekLargeVanPrice = diffWeekLargeVanPrice,

                DiffWeekCongestionChargeQuantity = diffWeekCongestionChargeQuantity,
                DiffWeekCongestionChargePrice = diffWeekCongestionChargePrice,
                DiffWeekLatePaymentQuantity = diffWeekLatePaymentQuantity,
                DiffWeekLatePaymentPrice = diffWeekLatePaymentPrice,

                DiffWeekFuel = diffWeekFuel,
                DiffWeekMileage = diffWeekMileage,
                DiffWeekDeduct = diffWeekDeduct,
                //
                AmazonWeekTotalQuantity = amazonWeekTotalQuantity,
                AmazonWeekTotalPrice = amazonWeekTotalPrice,
                SblWeekTotalQuantity = sblWeekTotalQuantity,
                SblWeekTotalPrice = sblWeekTotalPrice,
                DiffWeekTotalQuantity = diffWeekTotalQuantity,
                DiffWeekTotalPrice = diffWeekTotalPrice

                //
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


         

        public ActionResult ServiceConfirmation222(DateTime? Date, int? Days, int? DepotId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);
         

            // period list

            DateTime firstDate = DateTime.Now.Date.AddMonths(-2);
            DateTime lastDate = DateTime.Now.Date.AddMonths(2);


            // get prev sunday
            DateTime firstSunday = firstDate;
            if (firstDate.DayOfWeek != DayOfWeek.Sunday)
            {
                firstSunday = firstDate.AddDays(-1);
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(-1);
            }

            // get last saturday
            DateTime lastSaturday = lastDate;
            if (lastDate.DayOfWeek != DayOfWeek.Saturday)
            {
                lastSaturday = lastDate.AddDays(1);
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);
            }


            // period list
            List<ServiceConfirmationViewModel.Period> periodList = new List<ServiceConfirmationViewModel.Period>();
            for (DateTime date = firstSunday; date <= lastSaturday; date = date.AddDays(7))
            {
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(date);
                periodList.Add(new ServiceConfirmationViewModel.Period
                {
                    DateStart = date.Date,
                    DateEnd = date.Date.AddDays(6),
                    Year = date.Year,
                    Week = week
                });
            }


            // focus date
            var focusDate = DateTime.Now.Date;
            if (Date.HasValue)
            {
                focusDate = Date.Value.Date;
            }

            // get focus sunday
            if (focusDate.DayOfWeek != DayOfWeek.Sunday)
            {
                while (focusDate.DayOfWeek != DayOfWeek.Sunday)
                    focusDate = focusDate.AddDays(-1);
            }


            // days and period end date
            var periodDays = 7;
            if (Days.HasValue)
            {
                //periodDays = Days.Value;
            }

            var periodEndDate = focusDate.AddDays(periodDays - 1);


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          orderby x.Name ascending
                          select new ServiceConfirmationViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();


            // depotid
            var depotId = 0;
            if (currentuser.DepotId.HasValue)
            {
                depotId = currentuser.DepotId.Value;
            }
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }


            // this period
            var hasWritePermision = false;
            if (User.IsInRole(WebConstant.SBLUserRole.Master) || User.IsInRole(WebConstant.SBLUserRole.Admin))
            {
                hasWritePermision = true;
            }
            else
            {
                if (currentuser != null && currentuser.DepotId == depotId)
                {
                    hasWritePermision = true;
                }
                else
                {
                    hasWritePermision = false;
                }
            }

            ViewBag.HasWritePermision = hasWritePermision;



            //var day1 = Helpers.Functions.GetServiceConfirmationDate(depotId, focusDate);
            //var day2 = Helpers.Functions.GetServiceConfirmationDate(depotId, focusDate.AddDays(1));



            var viewModel = new ServiceConfirmationViewModel
            {
                //
                DepotId = depotId,
                DateStart = focusDate,
                Days = periodDays,
                //
                Periods = periodList,
                Depots = depots,
                HasWritePermision = hasWritePermision
                //
                //Day1 = day1

                //
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }



        public JsonResult ServiceConfirmationGetDays222(int DepotId, DateTime Date)
        {
            try
            {


               // var day1 = Helpers.Functions.GetServiceConfirmationDate(DepotId, Date);
                //var day2 = Helpers.Functions.GetServiceConfirmationDate(DepotId, Date.AddDays(1));


                return Json(new
                {
                  //  day1 = day1,
                    //day2 = day2,
                    status = 1
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }


         


        public JsonResult GetAmazonRouteDetails(int? RouteAmazonId)
        {
            try
            {
                if (db.RouteAmazons.Where(x => x.Id == RouteAmazonId && x.Active == true && x.Deleted == false).Any())
                {
                    var route = db.RouteAmazons.Where(x => x.Id == RouteAmazonId && x.Active == true && x.Deleted == false).FirstOrDefault();
                    
                    // Full
                    // Half
                    // RemoteDebrief
                    // NurseryRoutesLevel1
                    // NurseryRoutesLevel2
                    // Rescue2Hours
                    // Rescue4Hours
                    // Rescue6Hours
                    // ReDelivery2Hours
                    // ReDelivery4Hours
                    // ReDelivery6Hours
                    // Missort2Hours
                    // Missort4Hours
                    // Missort6Hours
                    // SameDay
                    // TrainingDay
                    // RideAlong

                    // SupportAd1
                    // SupportAd2
                    // SupportAd3
                    // LeadDriver
                    // LargeVan

                    // CongestionCharge
                    // LatePayment
                    // Mileage

                    return Json(new
                    {
                        Full = route.FullQuantity,
                        Half = route.HalfQuantity,
                        RemoteDebrief = route.RemoteDebriefQuantity,
                        NurseryRoutesLevel1 = route.NurseryRoutesLevel1Quantity,
                        NurseryRoutesLevel2 = route.NurseryRoutesLevel2Quantity,
                        Rescue2Hours = route.Rescue2HoursQuantity,
                        Rescue4Hours = route.Rescue4HoursQuantity,
                        Rescue6Hours = route.Rescue6HoursQuantity,
                        ReDelivery2Hours = route.ReDelivery2HoursQuantity,
                        ReDelivery4Hours = route.ReDelivery4HoursQuantity,
                        ReDelivery6Hours = route.ReDelivery6HoursQuantity,
                        Missort2Hours = route.Missort2HoursQuantity,
                        Missort4Hours = route.Missort4HoursQuantity,
                        Missort6Hours = route.Missort6HoursQuantity,
                        SameDay = route.SameDayQuantity,
                        TrainingDay = route.TrainingDayQuantity,
                        RideAlong = route.RideAlongQuantity,

                        SupportAd1 = route.SupportAd1Quantity,
                        SupportAd2 = route.SupportAd2Quantity,
                        SupportAd3 = route.SupportAd3Quantity,
                        LeadDriver = route.LeadDriverQuantity,
                        LargeVan = route.LargeVanQuantity,

                        CongestionCharge = route.CongestionChargeQuantity,
                        LatePayment = route.LatePaymentQuantity,

                        //Fuel = route.Fuel,
                        Mileage = route.Mileage,
                        //Deduct = route.Deduct,
                        status = 2
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult UpdateAmazonRouteDetails(DateTime? RouteDate, int? DepotId, int? RouteAmazonId, double Full, double Half, double RemoteDebrief, double NurseryRoutesLevel1, double NurseryRoutesLevel2, double Rescue2Hours, double Rescue4Hours, double Rescue6Hours, double ReDelivery2Hours, double ReDelivery4Hours, double ReDelivery6Hours, double Missort2Hours, double Missort4Hours, double Missort6Hours, double SameDay, double TrainingDay, double RideAlong, double LargeVan, double CongestionCharge, double LatePayment, double Mileage)
        {
            var prices = db.Settings.FirstOrDefault();

            //if (routeAmazonId == 0 && quantity > 0 && !db.RouteAmazons.Where(x => x.RouteDate == routeDate && x.DepotId == currentuser.DepotId && x.Active == true && x.Deleted == false).Any())
            if (RouteAmazonId == 0)
            {
                RouteAmazon route = new RouteAmazon();
                route.RouteDate = RouteDate;
                route.DepotId = DepotId;
                //
                route.FullQuantity = Full;
                route.FullPrice = prices.FullRouteAmazon;
                route.HalfQuantity = Half;
                route.HalfPrice = prices.HalfRouteAmazon;
                route.RemoteDebriefQuantity = RemoteDebrief;
                route.RemoteDebriefPrice = prices.RemoteDebriefAmazon;
                route.NurseryRoutesLevel1Quantity = NurseryRoutesLevel1;
                route.NurseryRoutesLevel1Price = prices.NurseryRoutesLevel1Amazon;
                route.NurseryRoutesLevel2Quantity = NurseryRoutesLevel2;
                route.NurseryRoutesLevel2Price = prices.NurseryRoutesLevel2Amazon;
                route.Rescue2HoursQuantity = Rescue2Hours;
                route.Rescue2HoursPrice = prices.Rescue2HoursAmazon;
                route.Rescue4HoursQuantity = Rescue4Hours;
                route.Rescue4HoursPrice = prices.Rescue4HoursAmazon;
                route.Rescue6HoursQuantity = Rescue6Hours;
                route.Rescue6HoursPrice = prices.Rescue6HoursAmazon;
                route.ReDelivery2HoursQuantity = ReDelivery2Hours;
                route.ReDelivery2HoursPrice = prices.ReDelivery2HoursAmazon;
                route.ReDelivery4HoursQuantity = ReDelivery4Hours;
                route.ReDelivery4HoursPrice = prices.ReDelivery4HoursAmazon;
                route.ReDelivery6HoursQuantity = ReDelivery6Hours;
                route.ReDelivery6HoursPrice = prices.ReDelivery6HoursAmazon;
                route.Missort2HoursQuantity = Missort2Hours;
                route.Missort2HoursPrice = prices.Missort2HoursAmazon;
                route.Missort4HoursQuantity = Missort4Hours;
                route.Missort4HoursPrice = prices.Missort4HoursAmazon;
                route.Missort6HoursQuantity = Missort6Hours;
                route.Missort6HoursPrice = prices.Missort6HoursAmazon;
                route.SameDayQuantity = SameDay;
                route.SameDayPrice = prices.SamedayAmazon;
                route.TrainingDayQuantity = TrainingDay;
                route.TrainingDayPrice = prices.TrainingDayAmazon;
                route.RideAlongQuantity = RideAlong;
                route.RideAlongPrice = prices.RideAlongAmazon;

                //route.SupportAd1Quantity = SupportAd1;
                //route.SupportAd1Price = prices.SupportAd1Amazon;
                //route.SupportAd2Quantity = SupportAd2;
                //route.SupportAd2Price = prices.SupportAd2Amazon;
                //route.SupportAd3Quantity = SupportAd3;
                //route.SupportAd3Price = prices.SupportAd3Amazon;
                //route.LeadDriverQuantity = LeadDriver;
                //route.LeadDriverPrice = prices.LeadDriverAmazon;
                route.LargeVanQuantity = LargeVan;
                route.LargeVanPrice = prices.LargeVanAmazon;

                route.CongestionChargeQuantity = CongestionCharge;
                route.CongestionChargePrice = prices.CongestionChargeAmazon;
                route.LatePaymentQuantity = LatePayment;
                route.LatePaymentPrice = prices.LatePaymentAmazon;

                route.Mileage = Mileage;
                //
                route.Fuel = 0;
                route.Deduct = 0;
                //
                route.DateCreated = DateTime.Now;
                route.Active = true;
                route.Deleted = false;
                db.RouteAmazons.Add(route);
                db.SaveChanges();
            }
            else
            {
                var route = db.RouteAmazons.FirstOrDefault(x => x.Id == RouteAmazonId);

                if (route != null)
                {
                    route.FullQuantity = Full;
                    route.FullPrice = Full == 0 ? 0 : prices.FullRouteAmazon;
                    route.HalfQuantity = Half;
                    route.HalfPrice = Half == 0 ? 0 : prices.HalfRouteAmazon;
                    route.RemoteDebriefQuantity = RemoteDebrief;
                    route.RemoteDebriefPrice = RemoteDebrief == 0 ? 0 : prices.RemoteDebriefAmazon;
                    route.NurseryRoutesLevel1Quantity = NurseryRoutesLevel1;
                    route.NurseryRoutesLevel1Price = NurseryRoutesLevel1 == 0 ? 0 : prices.NurseryRoutesLevel1Amazon;
                    route.NurseryRoutesLevel2Quantity = NurseryRoutesLevel2;
                    route.NurseryRoutesLevel2Price = NurseryRoutesLevel2 == 0 ? 0 : prices.NurseryRoutesLevel2Amazon;
                    route.Rescue2HoursQuantity = Rescue2Hours;
                    route.Rescue2HoursPrice = Rescue2Hours == 0 ? 0 : prices.Rescue2HoursAmazon;
                    route.Rescue4HoursQuantity = Rescue4Hours;
                    route.Rescue4HoursPrice = Rescue4Hours == 0 ? 0 : prices.Rescue4HoursAmazon;
                    route.Rescue6HoursQuantity = Rescue6Hours;
                    route.Rescue6HoursPrice = Rescue6Hours == 0 ? 0 : prices.Rescue6HoursAmazon;
                    route.ReDelivery2HoursQuantity = ReDelivery2Hours;
                    route.ReDelivery2HoursPrice = ReDelivery2Hours == 0 ? 0 : prices.ReDelivery2HoursAmazon;
                    route.ReDelivery4HoursQuantity = ReDelivery4Hours;
                    route.ReDelivery4HoursPrice = ReDelivery4Hours == 0 ? 0 : prices.ReDelivery4HoursAmazon;
                    route.ReDelivery6HoursQuantity = ReDelivery6Hours;
                    route.ReDelivery6HoursPrice = ReDelivery6Hours == 0 ? 0 : prices.ReDelivery6HoursAmazon;
                    route.Missort2HoursQuantity = Missort2Hours;
                    route.Missort2HoursPrice = Missort2Hours == 0 ? 0 : prices.Missort2HoursAmazon;
                    route.Missort4HoursQuantity = Missort4Hours;
                    route.Missort4HoursPrice = Missort4Hours == 0 ? 0 : prices.Missort4HoursAmazon;
                    route.Missort6HoursQuantity = Missort6Hours;
                    route.Missort6HoursPrice = Missort6Hours == 0 ? 0 : prices.Missort6HoursAmazon;
                    route.SameDayQuantity = SameDay;
                    route.SameDayPrice = SameDay == 0 ? 0 : prices.SamedayAmazon;
                    route.TrainingDayQuantity = TrainingDay;
                    route.TrainingDayPrice = TrainingDay == 0 ? 0 : prices.TrainingDayAmazon;
                    route.RideAlongQuantity = RideAlong;
                    route.RideAlongPrice = RideAlong == 0 ? 0 : prices.RideAlongAmazon;

                    //route.SupportAd1Quantity = SupportAd1;
                    //route.SupportAd1Price = SupportAd1 == 0 ? 0 : prices.SupportAd1Amazon;
                    //route.SupportAd2Quantity = SupportAd2;
                    //route.SupportAd2Price = SupportAd2 == 0 ? 0 : prices.SupportAd2Amazon;
                    //route.SupportAd3Quantity = SupportAd3;
                    //route.SupportAd3Price = SupportAd3 == 0 ? 0 : prices.SupportAd3Amazon;
                    //route.LeadDriverQuantity = LeadDriver;
                    //route.LeadDriverPrice = LeadDriver == 0 ? 0 : prices.LeadDriverAmazon;
                    route.LargeVanQuantity = LargeVan;
                    route.LargeVanPrice = LargeVan == 0 ? 0 : prices.LargeVanAmazon;

                    route.CongestionChargeQuantity = CongestionCharge;
                    route.CongestionChargePrice = CongestionCharge == 0 ? 0 : prices.CongestionChargeAmazon;
                    route.LatePaymentQuantity = LatePayment;
                    route.LatePaymentPrice = LatePayment == 0 ? 0 : prices.LatePaymentAmazon;
                    route.Mileage = Mileage;
                    //
                    route.Fuel = 0;
                    route.Deduct = 0;
                    //
                    db.SaveChanges();
                }
            }

            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }



        #endregion





        #region Associate


        public ActionResult AssociateList(int? DepotId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();


            // depotid
            var depotId = 0;
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }


            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          select new AssociateListViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();

            var getassociates = db.Associates.Where(x => x.Active == true && x.Deleted == false);

            // filter by depot id
            if (depotId > 0)
            {
                getassociates = getassociates.Where(x => x.DepotId == depotId);
            }

            var associates = (from x in getassociates
                              orderby x.Name ascending
                              select new AssociateListViewModel.Associate
                              {
                                  AssociateId = x.Id,
                                  Name = x.Name,
                                  Email = x.Email,
                                  Address = x.Address,
                                  City = x.City,
                                  Postcode = x.Postcode,
                                  Mobile = x.Mobile,
                                  DateOfBirth = x.DateOfBirth,
                                  Nationality = x.Nationality,
                                  NationalInsuranceNumber = x.NationalInsuranceNumber,
                                  UTRNumber = x.UTRNumber,
                                  NextOfKinName = x.NextOfKinName,
                                  NextOfKinRelationship = x.NextOfKinRelationship,
                                  NextOfKinMobile = x.NextOfKinMobile,
                                  NameOfTheBank = x.NameOfTheBank,
                                  SortCode = x.SortCode,
                                  AccountNumber = x.AccountNumber,
                                  AccountName = x.AccountName,
                                  DataPhoto = x.DataPhoto,
                                  DataPhotoContentType = x.DataPhotoContentType,
                                  DataPhotoName = x.DataPhotoName,
                                  AssociateStatus = x.AssociateStatus
                              }).ToList();


            var viewModel = new AssociateListViewModel
            {
                DepotId = depotId,
                Depots = depots,
                Associates = associates
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        public ActionResult AssociateCreate()
        {
            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Depots = depots;

            return View();
        }


        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult AssociateCreate(Associate associate, HttpPostedFileBase PhotoUpload, HttpPostedFileBase DrivingLicenceUpload, HttpPostedFileBase PassportUpload, HttpPostedFileBase ProofOfAddressUpload, HttpPostedFileBase NinoUpload)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);


                #region Add Associate In User Table

                /*

                var applicationUser = new ApplicationUser();
                applicationUser.Email = associate.Email;
                applicationUser.UserName = associate.Email;
                applicationUser.PhoneNumber = associate.Mobile;
                applicationUser.DepotId = associate.DepotId;
                applicationUser.LockoutEnabled = true;
                applicationUser.EmailConfirmed = true;
                applicationUser.PhoneNumberConfirmed = true;
                db.Users.Add(applicationUser);
                // db.SaveChanges();

                var role = new IdentityUserRole();
                role.RoleId = WebConstant.SBLUserRoleId.DriverRoleId;
                role.UserId = applicationUser.Id;
                applicationUser.Roles.Add(role);
                // db.SaveChanges();

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                userManager.Create(applicationUser, "vijendra");


                var user = new ApplicationUser {
                    UserName = associate.Email,
                    Email = associate.Email,
                    DepotId = associate.DepotId,
                };
                var result = UserManager.Create(user, associate.Mobile);
                //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                //var result = userManager.Create(user, associate.Mobile);


            */


                #endregion





                if (PhotoUpload != null && PhotoUpload.ContentLength > 0)
                {
                    Int32 length = PhotoUpload.ContentLength;
                    byte[] temp = new byte[length];
                    PhotoUpload.InputStream.Read(temp, 0, length);

                    associate.DataPhoto = temp;
                    associate.DataPhotoContentType = PhotoUpload.ContentType;
                    associate.DataPhotoName = PhotoUpload.FileName;
                }

                if (DrivingLicenceUpload != null && DrivingLicenceUpload.ContentLength > 0)
                {
                    Int32 length = DrivingLicenceUpload.ContentLength;
                    byte[] temp = new byte[length];
                    DrivingLicenceUpload.InputStream.Read(temp, 0, length);

                    associate.DataDrivingLicence = temp;
                    associate.DataDrivingLicenceContentType = DrivingLicenceUpload.ContentType;
                    associate.DataDrivingLicenceName = DrivingLicenceUpload.FileName;
                }

                if (PassportUpload != null && PassportUpload.ContentLength > 0)
                {
                    Int32 length = PassportUpload.ContentLength;
                    byte[] temp = new byte[length];
                    PassportUpload.InputStream.Read(temp, 0, length);

                    associate.DataPassport = temp;
                    associate.DataPassportContentType = PassportUpload.ContentType;
                    associate.DataPassportName = PassportUpload.FileName;
                }

                if (ProofOfAddressUpload != null && ProofOfAddressUpload.ContentLength > 0)
                {
                    Int32 length = ProofOfAddressUpload.ContentLength;
                    byte[] temp = new byte[length];
                    ProofOfAddressUpload.InputStream.Read(temp, 0, length);

                    associate.DataProofOfAddress = temp;
                    associate.DataProofOfAddressContentType = ProofOfAddressUpload.ContentType;
                    associate.DataProofOfAddressName = ProofOfAddressUpload.FileName;
                }

                if (NinoUpload != null && NinoUpload.ContentLength > 0)
                {
                    Int32 length = NinoUpload.ContentLength;
                    byte[] temp = new byte[length];
                    NinoUpload.InputStream.Read(temp, 0, length);

                    associate.DataNino = temp;
                    associate.DataNinoContentType = NinoUpload.ContentType;
                    associate.DataNinoName = NinoUpload.FileName;
                }





                // db
                associate.DateCreated = DateTime.Now;
                associate.UserId = "";
                associate.Active = true;
                associate.Deleted = false;
                db.Associates.Add(associate);
                db.SaveChanges();

                // log
                Helpers.Logging.LogEntry("Admin: AssociateCreate - User: " + currentuser.Id, "", false);

                return RedirectToAction("associatelist");










                try
                {



                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: AssociateCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Depots = depots;

            return View();
        }


        public ActionResult AssociateEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Associate associate = db.Associates.Find(id);

            if (associate == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == associate.DepotId,
            });

            ViewBag.Depots = depots;

            return View(associate);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult AssociateEdit(Associate associate, HttpPostedFileBase PhotoUpload, HttpPostedFileBase DrivingLicenceUpload, HttpPostedFileBase PassportUpload, HttpPostedFileBase ProofOfAddressUpload, HttpPostedFileBase NinoUpload)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Associates.Where(x => x.Id == associate.Id).FirstOrDefault();

                    original.DepotId = associate.DepotId;

                    original.Name = associate.Name;
                    original.Email = associate.Email;
                    original.Position = associate.Position;
                    original.Address = associate.Address;
                    original.City = associate.City;
                    original.Postcode = associate.Postcode;
                    original.Mobile = associate.Mobile;
                    original.NextOfKinName = associate.NextOfKinName;
                    original.NextOfKinRelationship = associate.NextOfKinRelationship;
                    original.NextOfKinMobile = associate.NextOfKinMobile;






                    if (PhotoUpload != null && PhotoUpload.ContentLength > 0)
                    {
                        Int32 length = PhotoUpload.ContentLength;
                        byte[] temp = new byte[length];
                        PhotoUpload.InputStream.Read(temp, 0, length);

                        original.DataPhoto = temp;
                        original.DataPhotoContentType = PhotoUpload.ContentType;
                        original.DataPhotoName = PhotoUpload.FileName;
                    }
                    else
                    {
                        original.DataPhoto = original.DataPhoto;
                        original.DataPhotoContentType = original.DataPhotoContentType;
                        original.DataPhotoName = original.DataPhotoName;
                    }



                    if (DrivingLicenceUpload != null && DrivingLicenceUpload.ContentLength > 0)
                    {
                        Int32 length = DrivingLicenceUpload.ContentLength;
                        byte[] temp = new byte[length];
                        DrivingLicenceUpload.InputStream.Read(temp, 0, length);

                        original.DataDrivingLicence = temp;
                        original.DataDrivingLicenceContentType = DrivingLicenceUpload.ContentType;
                        original.DataDrivingLicenceName = DrivingLicenceUpload.FileName;
                    }
                    else
                    {
                        original.DataDrivingLicence = original.DataDrivingLicence;
                        original.DataDrivingLicenceContentType = original.DataDrivingLicenceContentType;
                        original.DataDrivingLicenceName = original.DataDrivingLicenceName;
                    }




                    if (PassportUpload != null && PassportUpload.ContentLength > 0)
                    {
                        Int32 length = PassportUpload.ContentLength;
                        byte[] temp = new byte[length];
                        PassportUpload.InputStream.Read(temp, 0, length);

                        original.DataPassport = temp;
                        original.DataPassportContentType = PassportUpload.ContentType;
                        original.DataPassportName = PassportUpload.FileName;
                    }
                    else
                    {
                        original.DataPassport = original.DataPassport;
                        original.DataPassportContentType = original.DataPassportContentType;
                        original.DataPassportName = original.DataPassportName;
                    }




                    if (ProofOfAddressUpload != null && ProofOfAddressUpload.ContentLength > 0)
                    {
                        Int32 length = ProofOfAddressUpload.ContentLength;
                        byte[] temp = new byte[length];
                        ProofOfAddressUpload.InputStream.Read(temp, 0, length);

                        original.DataProofOfAddress = temp;
                        original.DataProofOfAddressContentType = ProofOfAddressUpload.ContentType;
                        original.DataProofOfAddressName = ProofOfAddressUpload.FileName;
                    }
                    else
                    {
                        original.DataProofOfAddress = original.DataProofOfAddress;
                        original.DataProofOfAddressContentType = original.DataProofOfAddressContentType;
                        original.DataProofOfAddressName = original.DataProofOfAddressName;
                    }




                    if (NinoUpload != null && NinoUpload.ContentLength > 0)
                    {
                        Int32 length = NinoUpload.ContentLength;
                        byte[] temp = new byte[length];
                        NinoUpload.InputStream.Read(temp, 0, length);

                        original.DataNino = temp;
                        original.DataNinoContentType = NinoUpload.ContentType;
                        original.DataNinoName = NinoUpload.FileName;
                    }
                    else
                    {
                        original.DataNino = original.DataNino;
                        original.DataNinoContentType = original.DataNinoContentType;
                        original.DataNinoName = original.DataNinoName;
                    }




                    original.Bio = associate.Bio;
                    original.ApplicationStatus = associate.ApplicationStatus;
                    original.AssociateStatus = associate.AssociateStatus;
                    original.IsEmployed = associate.IsEmployed;
                    original.IsThirdParty = associate.IsThirdParty;
                    original.OwnVehicle = associate.OwnVehicle;
                    original.StartDate = associate.StartDate;
                    original.InductionCompleted = associate.InductionCompleted;
                    original.InductionCompletedDate = associate.InductionCompletedDate;
                    original.FirstRideAlong = associate.FirstRideAlong;
                    original.FirstRideAlongDate = associate.FirstRideAlongDate;
                    original.SecondRideAlong = associate.SecondRideAlong;
                    original.SecondRideAlongDate = associate.SecondRideAlongDate;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: AssociateEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("associatelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: AssociateEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == associate.DepotId,
            });

            ViewBag.Depots = depots;

            return View(associate);
        }


        public ActionResult AssociateFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Associate associate = db.Associates.Find(id);

            if (associate == null)
            {
                return HttpNotFound();
            }

            return View(associate);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult AssociateFile(int? Id, string Description, DateTime? ExpiryDate, HttpPostedFileBase FileUpload)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Associates.Where(x => x.Id == Id).FirstOrDefault();

                    if (FileUpload != null && FileUpload.ContentLength > 0)
                    {
                        Int32 length = FileUpload.ContentLength;
                        byte[] temp = new byte[length];
                        FileUpload.InputStream.Read(temp, 0, length);

                        AssociateFile associatefile = new AssociateFile();
                        associatefile.AssociateId = original.Id;

                        associatefile.DataFile = temp;
                        associatefile.DataFileContentType = FileUpload.ContentType;
                        associatefile.DataFileName = FileUpload.FileName;
                        associatefile.DataFileDescription = Description;
                        associatefile.DataFileExpiryDate = ExpiryDate;

                        associatefile.DateCreated = DateTime.Now;
                        associatefile.Active = true;
                        associatefile.Deleted = false;
                        db.AssociateFiles.Add(associatefile);
                        db.SaveChanges();
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: AssociateFile - User: " + currentuser.Id, "", false);

                    return RedirectToAction("associatefile", new { id = original.Id });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: AssociateFile - User: " + currentuser.Id, ex.Message, true);
                }
            }

            Associate associate = db.Associates.Find(Id);

            return View(associate);
        }


        public ActionResult AssociateFileList()
        {
            return View(db.AssociateFiles.Where(x => x.Active == true && x.Deleted == false).OrderByDescending(x => x.DataFileExpiryDate).ToList());
        }


        public JsonResult AssociateFileRemove(int? AssociateFileId)
        {
            var associatefile = db.AssociateFiles.Where(x => x.Id == AssociateFileId).FirstOrDefault();
            associatefile.Deleted = true;
            db.SaveChanges();

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public FileResult AssociateFileDownload(int id)
        {
            var associatefile = db.AssociateFiles.FirstOrDefault(x => x.Id == id);
            if (associatefile != null)
            {

                return File(associatefile.DataFile, associatefile.DataFileContentType, associatefile.DataFileName);
            }
            return null;
        }


        public ActionResult AssociateProfile(int? id)
        {
            var associate = db.Associates.Where(x => x.Id == id).FirstOrDefault();

            var associatefiles = (from x in db.AssociateFiles
                                  where x.AssociateId == associate.Id && x.Active == true && x.Deleted == false
                                  select new AssociateProfileViewModel.AssociateFile
                                  {
                                      Id = x.Id,
                                      DataFileName = x.DataFileName,
                                      DataFileDescription = x.DataFileDescription,
                                      DataFileExpiryDate = x.DataFileExpiryDate
                                  }).ToList();

            var associateremittances = (from x in db.RouteAllocations
                                        where x.AssociateId == associate.Id && x.Active == true && x.Deleted == false
                                        orderby x.RouteDate descending
                                        select new AssociateProfileViewModel.AssociateRemittance
                                        {
                                            Id = x.Id,
                                            Date = x.RouteDate,
                                            Status = x.Status
                                        }).ToList();

            var viewModel = new AssociateProfileViewModel
            {
                Id = associate.Id,
                Name = associate.Name,
                Email = associate.Email,
                Position = associate.Position,
                Address = associate.Address,
                City = associate.City,
                Postcode = associate.Postcode,
                Mobile = associate.Mobile,
                NextOfKinName = associate.NextOfKinName,
                NextOfKinRelationship = associate.NextOfKinRelationship,
                NextOfKinMobile = associate.NextOfKinMobile,
                DataPhoto = associate.DataPhoto,
                DataPhotoContentType = associate.DataPhotoContentType,
                Bio = associate.Bio,
                AssociateFiles = associatefiles,
                AssociateRemittances = associateremittances
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        #endregion


         


        #region subrental


        public ActionResult SubRentalList(int? DepotId, int? AssociateId, int? VehicleId, string VehicleMake, string VehicleModel, string Status)
        {
            var depotId = 0;
            if (DepotId.HasValue)
            {
                depotId = DepotId.Value;
            }

            var associateId = 0;
            if (AssociateId.HasValue)
            {
                associateId = AssociateId.Value;
            }

            var vehicleId = 0;
            if (VehicleId.HasValue)
            {
                vehicleId = VehicleId.Value;
            }

            var vehicleMake = "";
            if (!String.IsNullOrEmpty(VehicleMake))
            {
                vehicleMake = VehicleMake;
            }

            var vehicleModel = "";
            if (!String.IsNullOrEmpty(VehicleModel))
            {
                vehicleModel = VehicleModel;
            }

            var status = Status;

            //

            var getsubrentals = db.SubRentals.Where(x => x.Active == true && x.Deleted == false).ToList();

            if (depotId > 0)
            {
                getsubrentals = getsubrentals.Where(x => x.Associate.DepotId == depotId).ToList();
            }

            if (associateId > 0)
            {
                getsubrentals = getsubrentals.Where(x => x.AssociateId == associateId).ToList();
            }

            if (vehicleId > 0)
            {
                getsubrentals = getsubrentals.Where(x => x.VehicleId == vehicleId).ToList();
            }

            if (vehicleMake != "")
            {
                getsubrentals = getsubrentals.Where(x => x.Vehicle.Make == vehicleMake).ToList();
            }

            if (vehicleModel != "")
            {
                getsubrentals = getsubrentals.Where(x => x.Vehicle.Model == vehicleModel).ToList();
            }

            if (status == "1")
            {
                getsubrentals = getsubrentals.Where(x => !x.DateReturned.HasValue).ToList();
            }

            if (status == "2")
            {
                getsubrentals = getsubrentals.Where(x => x.DateReturned.HasValue).ToList();
            }

            //

            var subrentals = (from x in getsubrentals
                              orderby x.DateRented descending
                              select new SubRentalListViewModel.SubRental
                              {
                                  Id = x.Id,
                                  AssociateId = x.Id,
                                  AssociateName = x.Associate.Name,
                                  VehicleMake = x.Vehicle.Make,
                                  VehicleModel = x.Vehicle.Model,
                                  VehicleRegistration = x.Vehicle.Registration,
                                  VehicleId = x.VehicleId,
                                  MileageStart = x.MileageStart,
                                  MileageEnd = x.MileageEnd,
                                  VanRentalPrice = x.VanRentalPrice,
                                  InsurancePrice = x.InsurancePrice,
                                  GoodsInTransitPrice = x.GoodsInTransitPrice,
                                  PublicLiabilityPrice = x.PublicLiabilityPrice,
                                  RentalPrice = x.RentalPrice,
                                  DateRented = x.DateRented,
                                  DateReturned = x.DateReturned,
                                  Status = x.Status,
                                  DateCreated = x.DateCreated,
                                  Active = x.Active,
                                  Deleted = x.Deleted
                              }).ToList();

            // depots
            var depots = (from x in db.Depots
                          where x.Active == true && x.Deleted == false
                          orderby x.Name ascending
                          select new SubRentalListViewModel.Depot
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();

            // associates
            var associates = (from x in db.Associates
                              where x.Active == true && x.Deleted == false
                              orderby x.Name ascending
                              select new SubRentalListViewModel.Associate
                              {
                                  Id = x.Id,
                                  Name = x.Name
                              }).ToList();

            // vehicles
            var vehicles = (from x in db.Vehicles
                            where x.Active == true && x.Deleted == false
                            orderby x.Registration ascending
                            select new SubRentalListViewModel.Vehicle
                            {
                                Id = x.Id,
                                VehicleMake = x.Make,
                                VehicleModel = x.Model,
                                VehicleRegistration = x.Registration
                            }).ToList();

            var viewModel = new SubRentalListViewModel
            {
                SelectedDepotId = depotId,
                SelectedAssociateId = associateId,
                SelectedVehicleId = vehicleId,
                SelectedVehicleMake = VehicleMake,
                SelectedVehicleModel = vehicleModel,
                SelectedStatus = status,
                //
                SubRentals = subrentals,
                //
                Depots = depots,
                Associates = associates,
                Vehicles = vehicles
                //
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        public ActionResult SubRentalList2()
        {
            return View(db.SubRentals.Where(x => x.Active == true && x.Deleted == false).ToList());
        }


        public ActionResult SubRentalCreate()
        {
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => 
                (x.AssociateStatus == "Active" && 
                    x.ApplicationStatus == "Approved" && 
                    x.OwnVehicle == false && 
                    !x.SubRentals.Where(y => !y.DateReturned.HasValue && y.Active == true && y.Deleted == false).Any() &&
                    x.Active && 
                    x.Deleted == false)).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            var datenow = DateTime.Now.Date;

            var subrentals = db.SubRentals.Where(x => 
                x.DateRented <= datenow && 
                !x.DateReturned.HasValue && 
                x.Active == true && 
                x.Deleted == false);

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => 
                !subrentals.Any(y => y.VehicleId == x.Id) && 
                x.Status != "Repair" && 
                x.Status != "Returned" && 
                x.Active == true && 
                x.Deleted == false).OrderBy(x => x.Make).ThenBy(x => x.Model).ThenBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Vehicles = vehicles;


            // get settings
            var settings = db.Settings.FirstOrDefault();
            ViewBag.VanRentalPriceSettings = settings.VanRentalPrice;
            ViewBag.InsurancePriceSettings = settings.InsurancePrice;
            ViewBag.GoodsInTransitPriceSettings = settings.GoodsInTransitPrice;
            ViewBag.PublicLiabilityPriceSettings = settings.PublicLiabilityPrice;

            return View();
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult SubRentalCreate(SubRental subrental)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    // update vehicle status
                    if (subrental.Vehicle != null)
                    {
                        subrental.Vehicle.Status = "Rented";
                    }
                    else
                    {
                        var vechicle = db.Vehicles.FirstOrDefault(x => x.Id == subrental.VehicleId);
                        if (vechicle != null)
                        {
                            subrental.Vehicle = vechicle;
                            subrental.Vehicle.Status = "Rented";
                        }
                    }
                    subrental.RentalPrice = subrental.VanRentalPrice + subrental.InsurancePrice + subrental.GoodsInTransitPrice + subrental.PublicLiabilityPrice;
                    subrental.Status = "Rented";
                    subrental.DateCreated = DateTime.Now;
                    subrental.Active = true;
                    subrental.Deleted = false;
                    db.SubRentals.Add(subrental);
                    db.SaveChanges();

                    var subRentalId = subrental.Id;
                    if (subRentalId > 0)
                    {
                        VehicleRentalPrice vehicleRental = new VehicleRentalPrice();
                        vehicleRental.SubRentalId = subRentalId;
                        vehicleRental.VanRentalPrice = subrental.VanRentalPrice;
                        vehicleRental.InsurancePrice = subrental.InsurancePrice;
                        vehicleRental.GoodsInTransitPrice = subrental.GoodsInTransitPrice;
                        vehicleRental.PublicLiabilityPrice = subrental.PublicLiabilityPrice;
                        vehicleRental.RentalPrice = subrental.VanRentalPrice + subrental.InsurancePrice + subrental.GoodsInTransitPrice + subrental.PublicLiabilityPrice;
                        vehicleRental.DateCreated = DateTime.Now;
                        vehicleRental.Active = true;
                        vehicleRental.Deleted = false;
                        db.VehicleRentalPrices.Add(vehicleRental);
                        db.SaveChanges();
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: SubRentalCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("subrentallist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: SubRentalCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            //

            IEnumerable<SelectListItem> associates = db.Associates.Where(x =>
            (x.AssociateStatus == "Active" &&
                x.ApplicationStatus == "Approved" &&
                x.OwnVehicle == false &&
                !x.SubRentals.Where(y => !y.DateReturned.HasValue && y.Active == true && y.Deleted == false).Any() &&
                x.Active &&
                x.Deleted == false)).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = false,
                });

            ViewBag.Associates = associates;

            var datenow = DateTime.Now.Date;

            var subrentals = db.SubRentals.Where(x =>
                x.DateRented <= datenow &&
                !x.DateReturned.HasValue &&
                x.Active == true &&
                x.Deleted == false);

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x =>
                !subrentals.Any(y => y.VehicleId == x.Id) &&
                x.Status != "Repair" &&
                x.Status != "Returned" &&
                x.Active == true &&
                x.Deleted == false).OrderBy(x => x.Make).ThenBy(x => x.Model).ThenBy(x => x.Registration).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Make + " - " + x.Model + " - " + x.Registration,
                    Value = x.Id.ToString(),
                    Selected = false,
                });

            ViewBag.Vehicles = vehicles;

            //

            return View();
        }


        public ActionResult SubRentalEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SubRental subrental = db.SubRentals.Find(id);

            if (subrental == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x =>
            (x.Id == subrental.AssociateId) ||
            (x.AssociateStatus == "Active" &&
                x.ApplicationStatus == "Approved" &&
                x.OwnVehicle == false &&
                !x.SubRentals.Where(y => !y.DateReturned.HasValue && y.Active == true && y.Deleted == false).Any() &&
                x.Active &&
                x.Deleted == false)).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = x.Id == subrental.AssociateId,
                });

            ViewBag.Associates = associates;

            var datenow = DateTime.Now.Date;

            var subrentals = db.SubRentals.Where(x => 
                x.DateRented <= datenow && 
                !x.DateReturned.HasValue && 
                x.Active == true && 
                x.Deleted == false);

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => 
                x.Id == subrental.VehicleId || 
                (!subrentals.Any(y => y.VehicleId == x.Id) && 
                x.Status != "Repair" && 
                x.Status != "Returned" && 
                x.Active == true && 
                x.Deleted == false)).OrderBy(x => x.Make).ThenBy(x => x.Model).ThenBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = x.Id == subrental.VehicleId,
            });

            ViewBag.Vehicles = vehicles;

            return View(subrental);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult SubRentalEdit(SubRental subrental)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    var original = db.SubRentals.FirstOrDefault(x => x.Id == subrental.Id);
                    if (original != null)
                    {
                        var lastRentalPrice = original.RentalPrice;

                        original.AssociateId = subrental.AssociateId;
                        original.VehicleId = subrental.VehicleId;

                        original.VanRentalPrice = subrental.VanRentalPrice;
                        original.InsurancePrice = subrental.InsurancePrice;
                        original.GoodsInTransitPrice = subrental.GoodsInTransitPrice;
                        original.PublicLiabilityPrice = subrental.PublicLiabilityPrice;
                        original.RentalPrice = subrental.VanRentalPrice + subrental.InsurancePrice + subrental.GoodsInTransitPrice + subrental.PublicLiabilityPrice;

                        original.MileageStart = subrental.MileageStart;
                        original.DateRented = subrental.DateRented;

                        original.MileageEnd = original.MileageEnd;
                        original.DateReturned = original.DateReturned;

                        // update vehicle status
                        original.Vehicle.Status = subrental.DateReturned.HasValue ? "Available" : "Rented";
                        original.Status = subrental.DateReturned.HasValue ? "Available" : "Rented";

                        original.Active = original.Active;
                        original.DateCreated = original.DateCreated;
                        original.Deleted = original.Deleted;
                        db.SaveChanges();

                        var subRentalId = subrental.Id;
                        if (subRentalId > 0 && lastRentalPrice != subrental.RentalPrice)
                        {
                            VehicleRentalPrice vehicleRental = new VehicleRentalPrice();
                            vehicleRental.SubRentalId = subRentalId;
                            vehicleRental.VanRentalPrice = subrental.VanRentalPrice;
                            vehicleRental.InsurancePrice = subrental.InsurancePrice;
                            vehicleRental.GoodsInTransitPrice = subrental.GoodsInTransitPrice;
                            vehicleRental.PublicLiabilityPrice = subrental.PublicLiabilityPrice;
                            vehicleRental.RentalPrice = subrental.VanRentalPrice + subrental.InsurancePrice + subrental.GoodsInTransitPrice + subrental.PublicLiabilityPrice;
                            vehicleRental.DateCreated = DateTime.Now;
                            vehicleRental.Active = true;
                            vehicleRental.Deleted = false;
                            db.VehicleRentalPrices.Add(vehicleRental);
                            db.SaveChanges();
                        }
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: SubRentalEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("subrentallist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: SubRentalEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            //

            IEnumerable<SelectListItem> associates = db.Associates.Where(x =>
            (x.Id == subrental.AssociateId) ||
            (x.AssociateStatus == "Active" &&
                x.ApplicationStatus == "Approved" &&
                x.OwnVehicle == false &&
                !x.SubRentals.Where(y => !y.DateReturned.HasValue && y.Active == true && y.Deleted == false).Any() &&
                x.Active &&
                x.Deleted == false)).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = x.Id == subrental.AssociateId,
                });

            ViewBag.Associates = associates;

            var datenow = DateTime.Now.Date;

            var subrentals = db.SubRentals.Where(x =>
                x.DateRented <= datenow &&
                !x.DateReturned.HasValue &&
                x.Active == true &&
                x.Deleted == false);

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x =>
                x.Id == subrental.VehicleId ||
                (!subrentals.Any(y => y.VehicleId == x.Id) &&
                x.Status != "Repair" &&
                x.Status != "Returned" &&
                x.Active == true &&
                x.Deleted == false)).OrderBy(x => x.Make).ThenBy(x => x.Model).ThenBy(x => x.Registration).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Make + " - " + x.Model + " - " + x.Registration,
                    Value = x.Id.ToString(),
                    Selected = x.Id == subrental.VehicleId,
                });

            ViewBag.Vehicles = vehicles;

            // 

            return View(subrental);
        }


        public ActionResult SubRentalReturn(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SubRental subrental = db.SubRentals.Find(id);

            if (subrental == null)
            {
                return HttpNotFound();
            }

            return View(subrental);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult SubRentalReturn(SubRental subrental)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    var original = db.SubRentals.FirstOrDefault(x => x.Id == subrental.Id);
                    if (original != null)
                    {
                        var lastRentalPrice = original.RentalPrice;

                        original.AssociateId = original.AssociateId;
                        original.VehicleId = original.VehicleId;

                        original.VanRentalPrice = original.VanRentalPrice;
                        original.InsurancePrice = original.InsurancePrice;
                        original.GoodsInTransitPrice = original.GoodsInTransitPrice;
                        original.PublicLiabilityPrice = original.PublicLiabilityPrice;
                        original.RentalPrice = original.VanRentalPrice + original.InsurancePrice + original.GoodsInTransitPrice + original.PublicLiabilityPrice;

                        original.MileageStart = original.MileageStart;
                        original.DateRented = original.DateRented;
                        //
                        original.MileageEnd = subrental.MileageEnd;
                        original.DateReturned = subrental.DateReturned;
                        //
                        original.Vehicle.Status = "Available";
                        original.Status = "Returned";
                        //
                        original.Active = original.Active;
                        original.DateCreated = original.DateCreated;
                        original.Deleted = original.Deleted;
                        db.SaveChanges();
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: SubRentalReturn - User: " + currentuser.Id, "", false);

                    return RedirectToAction("subrentallist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: SubRentalReturn - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(subrental);
        }


        public ActionResult SubRentalCancelReturn(int? id)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            try
            {
                SubRental subrental = db.SubRentals.Find(id);
                subrental.MileageEnd = 0;
                subrental.DateReturned = null;
                subrental.Status = "Rented";
                db.SaveChanges();

                // log
                Helpers.Logging.LogEntry("Admin: SubRentalCancelReturn - User: " + currentuser.Id, "", false);

                return RedirectToAction("subrentallist");
            }
            catch (Exception ex)
            {
                // log
                Helpers.Logging.LogEntry("Admin: SubRentalCancelReturn - User: " + currentuser.Id, ex.Message, true);
            }

            return RedirectToAction("subrentallist");
        }


        #endregion


























        #region Dashboard Page Methods
        public ActionResult Dashboard()
        {
            var associates = (from x in db.Associates
                              where x.Active == true && x.Deleted == false
                              select new DashboardViewModel.Associate
                              {
                                  Id = x.Id,
                                  Name = x.Name
                              }).ToList();

            var depots = (from x in db.Depots
                              where x.Active == true && x.Deleted == false
                              orderby x.Name ascending
                              select new DashboardViewModel.Depot
                              {
                                  Id = x.Id,
                                  Name = x.Name,
                                  ActiveDrivers = db.Associates.Where(y => y.DepotId == x.Id && y.ApplicationStatus == "Approved" && y.AssociateStatus == "Active" && y.Active == true && y.Deleted == false).Count()
                              }).ToList();

            var viewModel = new DashboardViewModel
            {
                Associates = associates,
                Depots = depots
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }

        public JsonResult GetDashboardProfit(int Period)
        {
            // get settings
            var settings = db.Settings.FirstOrDefault();

            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            // period

            var dateNow = DateTime.Now.Date;
            var dateFrom = dateNow;
            var dateTo = dateNow;

            if (Period == 2)
            {
                dateFrom = dateNow.AddDays(-7);
                dateTo = dateNow;
            }

            if (Period == 3)
            {
                dateFrom = dateNow.AddDays(-28);
                dateTo = dateNow;
            }

            if (Period == 4)
            {
                dateFrom = dateNow.AddDays(-60);
                dateTo = dateNow;
            }

            if (Period == 5)
            {
                dateFrom = dateNow.AddDays(-365);
                dateTo = dateNow;
            }

            // variables
            double amzIncome = 0;

            #region Charge Calculation
            var charges = db.Charges.Where(x => x.Active && x.Deleted == false && x.Date >= dateFrom && x.Date <= dateNow).ToList();
            #endregion

            var sblTotalList = db.RouteAllocations.Where(x => x.RouteDate >= dateFrom && x.RouteDate <= dateNow && x.Active == true && x.Deleted == false).ToList();

            // if poc filter by depot
            if (currentuser != null && currentuser.DepotId.HasValue)
            {
                sblTotalList = sblTotalList.Where(x => x.DepotId == currentuser.DepotId).ToList();
            }
            var sblRouteChargesTotal = Utility.TotalRouteCharges(sblTotalList, settings);
            //var associateVechileRental = Utility.RentalCharges(sblTotalList);
            var associateVechileRental = 0;

            var routeChargeAmt = charges.Where(x => x.SetAsCredit == false).Sum(x => x.Amount);
            var routeChargeCreditAmt = charges.Where(x => x.SetAsCredit).Sum(x => x.Amount);
            //var vechicleRentAmt = associateVechileRental.Sum(x => x.Amount);
            var vechicleRentAmt = 0;

            var sblTotal = sblRouteChargesTotal + routeChargeCreditAmt - routeChargeAmt - vechicleRentAmt;

            var incomeFromAmz = db.RouteAmazons.Where(x => x.RouteDate >= dateFrom && x.RouteDate <= dateNow && x.Active == true && x.Deleted == false);

            // if poc filter by depot
            if (currentuser != null && currentuser.DepotId.HasValue)
            {
                incomeFromAmz = incomeFromAmz.Where(x => x.DepotId == currentuser.DepotId);
            }

            if (incomeFromAmz.Any())
            {
                amzIncome = incomeFromAmz.Sum(x =>
                                (x.FullQuantity * x.FullPrice) +
                                (x.HalfQuantity * x.HalfPrice) +
                                (x.RemoteDebriefQuantity * x.RemoteDebriefPrice) +
                                (x.NurseryRoutesLevel1Quantity * x.NurseryRoutesLevel1Price) +
                                (x.NurseryRoutesLevel2Quantity * x.NurseryRoutesLevel2Price) +
                                (x.Rescue2HoursQuantity * x.Rescue2HoursPrice) +
                                (x.Rescue4HoursQuantity * x.Rescue4HoursPrice) +
                                (x.Rescue6HoursQuantity * x.Rescue6HoursPrice) +
                                (x.ReDelivery2HoursQuantity * x.ReDelivery2HoursPrice) +
                                (x.ReDelivery4HoursQuantity * x.ReDelivery4HoursPrice) +
                                (x.ReDelivery6HoursQuantity * x.ReDelivery6HoursPrice) +
                                (x.Missort2HoursQuantity * x.Missort2HoursPrice) +
                                (x.Missort4HoursQuantity * x.Missort4HoursPrice) +
                                (x.Missort6HoursQuantity * x.Missort6HoursPrice) +
                                (x.SameDayQuantity * x.SameDayPrice) +
                                (x.TrainingDayQuantity * x.TrainingDayPrice) +
                                (x.RideAlongQuantity * x.RideAlongPrice) +

                                (x.SupportAd1Quantity * x.SupportAd1Price) +
                                (x.SupportAd2Quantity * x.SupportAd2Price) +
                                (x.SupportAd3Quantity * x.SupportAd3Price) +
                                (x.LeadDriverQuantity * x.LeadDriverPrice) +
                                (x.LargeVanQuantity * x.LargeVanPrice) +

                                ((x.Mileage / settings.MilesPerLitre) * settings.DieselPrice));
            }

            double profit = 0.00;
            if (sblTotal >= 0)
            {
                profit = sblTotal - amzIncome;
            }
            else
            {
                profit = amzIncome + sblTotal;
            }
            return Json(new
            {
                income = amzIncome,
                outgoing = sblTotal,
                profit = profit
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardVehicleStatus()
        {
            var vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).ToList();
            int rented = vehicles.Count(x => x.Status == "Rented");
            int available = vehicles.Count(x => x.Status == "Available");
            int repair = vehicles.Count(x => x.Status == "Repair");
            int returned = vehicles.Count(x => x.Status == "Returned");
            var totalVehicles = vehicles.Count();
            return Json(new
            {
                rented = rented,
                available = available,
                repair = repair,
                returned = returned,
                totalVehicles = totalVehicles
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardOverview(DateTime? FromDate)
        {
            // get settings
            var settings = db.Settings.FirstOrDefault();

            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            // period
            var dateNow = DateTime.Now.Date;

            var currentWeek = dateNow.GetWeekNumberOfYear();
            var startWeekNo = 1;
            // check from date
            if (FromDate.HasValue)
            {
                startWeekNo = FromDate.Value.GetWeekNumberOfYear();
            }
            List<Overview> overviews = new List<Overview>();

            for (int weekNo = startWeekNo; weekNo <= currentWeek; weekNo++)
            {
                var weekStartDate = weekNo.FirstDateOfWeek();
                var weekEndDate = weekStartDate.LastDayOfWeek();

                var sblTotalList = db.RouteAllocations.Where(x => x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate && x.Active == true && x.Deleted == false).ToList();
                var charges = db.Charges.Where(x => x.Active && x.Deleted == false && x.Date >= weekStartDate && x.Date <= weekEndDate).ToList();

                // if poc filter by depot
                if (currentuser != null && currentuser.DepotId.HasValue)
                {
                    sblTotalList = sblTotalList.Where(x => x.DepotId == currentuser.DepotId).ToList();
                }

                var sblTotal = Utility.TotalRouteCharges(sblTotalList, settings);
                //var associateVechileRental = Utility.RentalCharges(sblTotalList);
                var associateVechileRental = 0;

                var routeChargeAmt = charges.Where(x => x.SetAsCredit == false).Sum(x => x.Amount);
                var routeCreditAmt = charges.Where(x => x.SetAsCredit).Sum(x => x.Amount);
                //var vechicleRentAmt = associateVechileRental.Sum(x => x.Amount);
                var vechicleRentAmt = 0;

                sblTotal = sblTotal + routeCreditAmt - routeChargeAmt - vechicleRentAmt;

                var amazonTotalAmount = db.RouteAmazons.Where(x => x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate && x.Active == true && x.Deleted == false).ToList();

                // if poc filter by depot
                if (currentuser != null && currentuser.DepotId.HasValue)
                {
                    amazonTotalAmount = amazonTotalAmount.Where(x => x.DepotId == currentuser.DepotId).ToList();
                }

                double amazonTotal = 0;
                if (amazonTotalAmount.Any())
                {
                    amazonTotal = amazonTotalAmount.Sum(x =>
                                    (x.FullQuantity * x.FullPrice) +
                                    (x.HalfQuantity * x.HalfPrice) +
                                    (x.RemoteDebriefQuantity * x.RemoteDebriefPrice) +
                                    (x.NurseryRoutesLevel1Quantity * x.NurseryRoutesLevel1Price) +
                                    (x.NurseryRoutesLevel2Quantity * x.NurseryRoutesLevel2Price) +
                                    (x.Rescue2HoursQuantity * x.Rescue2HoursPrice) +
                                    (x.Rescue4HoursQuantity * x.Rescue4HoursPrice) +
                                    (x.Rescue6HoursQuantity * x.Rescue6HoursPrice) +
                                    (x.ReDelivery2HoursQuantity * x.ReDelivery2HoursPrice) +
                                    (x.ReDelivery4HoursQuantity * x.ReDelivery4HoursPrice) +
                                    (x.ReDelivery6HoursQuantity * x.ReDelivery6HoursPrice) +
                                    (x.Missort2HoursQuantity * x.Missort2HoursPrice) +
                                    (x.Missort4HoursQuantity * x.Missort4HoursPrice) +
                                    (x.Missort6HoursQuantity * x.Missort6HoursPrice) +
                                    (x.SameDayQuantity * x.SameDayPrice) +
                                    (x.TrainingDayQuantity * x.TrainingDayPrice) +
                                    (x.RideAlongQuantity * x.RideAlongPrice) +
                                
                                    (x.SupportAd1Quantity * x.SupportAd1Price) +
                                    (x.SupportAd2Quantity * x.SupportAd2Price) +
                                    (x.SupportAd3Quantity * x.SupportAd3Price) +
                                    (x.LeadDriverQuantity * x.LeadDriverPrice) +
                                    (x.LargeVanQuantity * x.LargeVanPrice) +

                                    ((x.Mileage / settings.MilesPerLitre) * settings.DieselPrice));
                }
                double diff = 0.0;
                if (sblTotal >= 0)
                {
                    diff = sblTotal - amazonTotal;
                }
                else
                {
                    diff = amazonTotal + sblTotal;
                }

                string status = " - ";

                if (diff > 0)
                {
                    status = "Profit";
                }

                if (diff < 0)
                {
                    status = "Loss";
                }

                string period = String.Format("{0:dd MMM yyyy} - {1:dd MMM yyyy}", weekStartDate, weekEndDate);

                // add to list
                overviews.Add(new Overview
                {
                    Id = weekNo,
                    Period = period,
                    SblTotal = sblTotal,
                    AmazonTotal = amazonTotal,
                    Diff = diff,
                    Status = status
                });
            }
            return Json(new
            {
                overviews = overviews.OrderByDescending(x => x.Id)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardStats()
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);
            var routeList = db.RouteAllocations.Where(x => x.Active == true && x.Deleted == false).ToList();
            var driverList = db.Associates.Where(x => x.Active && x.Deleted == false && x.DepotId >= 0 && x.ApplicationStatus == "Approved" && x.AssociateStatus == "Active").ToList();


            double averageRoutePerWeek = 0.0F;
            if (!User.IsInRole(WebConstant.SBLUserRole.Master) && !User.IsInRole(WebConstant.SBLUserRole.Admin))
            {
                if (currentuser != null && currentuser.DepotId.HasValue)
                {
                    routeList = routeList.Where(x => x.DepotId == currentuser.DepotId).ToList();
                    driverList = driverList.Where(x => x.DepotId == currentuser.DepotId).ToList();
                }
                else
                {
                    routeList = new List<RouteAllocation>();
                    driverList = new List<Associate>();
                }
            }

            #region Pending inductions
            double inductions = 0;

            if (db.Inductions.Where(x => x.Status == "Pending" && x.Active == true && x.Deleted == false).Any())
            {
                inductions = db.Inductions.Where(x => x.Status == "Pending" && x.Active == true && x.Deleted == false).Count();
            }
            #endregion

            #region Average Route Per Week
            if (routeList.Any())
            {
                var count = routeList.Count();
                var firstOrDefault = routeList.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var first = firstOrDefault.RouteDate;
                    var routeAllocation = routeList.OrderByDescending(x => x.RouteDate).FirstOrDefault();
                    if (routeAllocation != null)
                    {
                        var last = routeAllocation.RouteDate;
                        var timeSpan = last - first;
                        if (timeSpan != null)
                        {
                            var weeks = timeSpan.Value.TotalDays / 7;
                            averageRoutePerWeek = count / weeks;
                        }
                    }
                }
            }
            #endregion;

            return Json(new
            {
                routes = averageRoutePerWeek.ToString("0.##"),
                pendingInductions = inductions,
                allocatedDrivers = driverList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardDriversDepot()
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

            var dateNow = DateTime.Now.Date;

            double drivers = 0;

            var drivers1 = db.RouteAllocations.Where(x => x.RouteDate >= dateNow && x.Active == true && x.Deleted == false);

            // if poc filter by depot
            if (currentuser.DepotId.HasValue)
            {
                drivers1 = drivers1.Where(x => x.DepotId == currentuser.DepotId);
            }

            if (drivers1.Any())
            {
                drivers = drivers1.Count();
            }

            return Json(new
            {
                drivers = drivers
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardInductionPending()
        {
            double inductions = 0;

            if (db.Inductions.Where(x => x.Status == "Pending" && x.Active == true && x.Deleted == false).Any())
            {
                inductions = db.Inductions.Where(x => x.Status == "Pending" && x.Active == true && x.Deleted == false).Count();
            }

            return Json(new
            {
                inductions = inductions
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardRoutesWeek()
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            double routes = 0;

            var routelist1 = db.RouteAllocations.Where(x => x.Active == true && x.Deleted == false);

            // if poc filter by depot
            if (currentuser != null && currentuser.DepotId.HasValue)
            {
                routelist1 = routelist1.Where(x => x.DepotId == currentuser.DepotId);
            }

            if (routelist1.Any())
            {
                var routelist = routelist1;

                var count = routelist.Count();
                var firstOrDefault = routelist.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var first = firstOrDefault.RouteDate;
                    var routeAllocation = routelist.OrderByDescending(x => x.RouteDate).FirstOrDefault();
                    if (routeAllocation != null)
                    {
                        var last = routeAllocation.RouteDate;
                        var timeSpan = last - first;
                        if (timeSpan != null)
                        {
                            var weeks = timeSpan.Value.TotalDays / 7;
                            routes = Math.Ceiling(count / weeks);
                        }
                    }
                }
            }

            return Json(new
            {
                routes = routes
            }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetInductionNotes(int InductionId)
        {
            var notes = (from x in db.InductionNotes
                         where x.InductionId == InductionId && x.Active == true && x.Deleted == false
                         orderby x.DateNote descending
                         select new
                         {
                             Id = x.Id,
                             DateNote = x.DateNote,
                             Description = x.Description,
                             Author = x.Author,
                         }).ToList();

            return Json(new
            {
                status = 1,
                notes = notes
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertInductionNote(int InductionId, string Description)
        {
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            InductionNote note = new InductionNote();
            note.InductionId = InductionId;
            note.Description = Description;
            note.DateNote = DateTime.Now;
            note.Author = currentuser.Email;
            note.DateCreated = DateTime.Now;
            note.Active = true;
            note.Deleted = false;
            db.InductionNotes.Add(note);
            db.SaveChanges();

            return Json(new
            {
                result = 1
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetInductionDocs(int InductionId)
        {
            var docs = (from x in db.Inductions
                        where x.Id == InductionId && x.Active == true && x.Deleted == false
                        select new
                        {
                            Id = x.Id,

                            DataFileDrivingLicense = x.DataFileDrivingLicense,
                            DataFileExtensionDrivingLicense = x.DataFileExtensionDrivingLicense,
                            IsValidDrivingLicense = x.IsValidDrivingLicense,

                            DataFileNationalInsuranceNumber = x.DataFileNationalInsuranceNumber,
                            DataFileExtensionNationalInsuranceNumber = x.DataFileExtensionNationalInsuranceNumber,
                            IsValidNationalInsuranceNumber = x.IsValidNationalInsuranceNumber,

                            DataFileProofAddress = x.DataFileProofAddress,
                            DataFileExtensionProofAddress = x.DataFileExtensionProofAddress,
                            IsValidProofAddress = x.IsValidProofAddress,

                            DataFileProofId = x.DataFileProofId,
                            DataFileExtensionProofId = x.DataFileExtensionProofId,
                            IsValidProofId = x.IsValidProofId
                        }).FirstOrDefault();

            return Json(new
            {
                status = 1,
                Id = docs.Id,
                //
                DataFileDrivingLicense = docs.DataFileDrivingLicense,
                DataFileExtensionDrivingLicense = docs.DataFileExtensionDrivingLicense,
                IsValidDrivingLicense = docs.IsValidDrivingLicense,
                //
                DataFileNationalInsuranceNumber = docs.DataFileNationalInsuranceNumber,
                DataFileExtensionNationalInsuranceNumber = docs.DataFileExtensionNationalInsuranceNumber,
                IsValidNationalInsuranceNumber = docs.IsValidNationalInsuranceNumber,
                //
                DataFileProofAddress = docs.DataFileProofAddress,
                DataFileExtensionProofAddress = docs.DataFileExtensionProofAddress,
                IsValidProofAddress = docs.IsValidProofAddress,
                //
                DataFileProofId = docs.DataFileProofId,
                DataFileExtensionProofId = docs.DataFileExtensionProofId,
                IsValidProofId = docs.IsValidProofId
                //
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public FileResult DownloadInductionDrivingLicense(int InductionId)
        {
            var induction = db.Inductions.Where(x => x.Id == InductionId).FirstOrDefault();

            if (induction.IsValidDrivingLicense == true)
            {
                string fileName = "Driving_License_" + induction.FullName + induction.DataFileExtensionDrivingLicense;

                return File(induction.DataFileDrivingLicense, induction.DataFileContentTypeDrivingLicense, fileName);
            }

            return null;
        }



        [HttpPost]
        public JsonResult UploadDrivingLicense(int InductionId)
        {
            string message = "Could not process your request.";

            int result = 0;

            try
            {
                result = 0;

                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        Int32 length = fileContent.ContentLength;
                        byte[] temp = new byte[length];
                        fileContent.InputStream.Read(temp, 0, length);

                        var induction = db.Inductions.Where(x => x.Id == InductionId).FirstOrDefault();
                        induction.DataFileDrivingLicense = temp;
                        induction.DataFileContentTypeDrivingLicense = fileContent.ContentType;
                        induction.DataFileNameDrivingLicense = fileContent.FileName;
                        string extension = Path.GetExtension(fileContent.FileName);
                        induction.DataFileExtensionDrivingLicense = extension;
                        induction.IsValidDrivingLicense = true;
                        db.SaveChanges();

                        result = 2;
                    }
                }
            }
            catch (Exception)
            {
                result = -1;
            }

            return Json(new
            {
                result = result,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }




        public List<Allocation> Allocations { get; set; }
        public class Allocation
        {
            public int Id { get; set; }
            public string Week { get; set; }
            public DateTime Date { get; set; }
            public int Routes { get; set; }
            public double FullQuantity { get; set; }
            public double FullPrice { get; set; }
            public double HalfQuantity { get; set; }
            public double HalfPrice { get; set; }
            public double RemoteDebriefQuantity { get; set; }
            public double RemoteDebriefPrice { get; set; }
            public double NurseryRoutesLevel1Quantity { get; set; }
            public double NurseryRoutesLevel1Price { get; set; }
            public double NurseryRoutesLevel2Quantity { get; set; }
            public double NurseryRoutesLevel2Price { get; set; }
            public double Rescue2HoursQuantity { get; set; }
            public double Rescue2HoursPrice { get; set; }
            public double Rescue4HoursQuantity { get; set; }
            public double Rescue4HoursPrice { get; set; }
            public double Rescue6HoursQuantity { get; set; }
            public double Rescue6HoursPrice { get; set; }
            public double ReDelivery2HoursQuantity { get; set; }
            public double ReDelivery2HoursPrice { get; set; }
            public double ReDelivery4HoursQuantity { get; set; }
            public double ReDelivery4HoursPrice { get; set; }
            public double ReDelivery6HoursQuantity { get; set; }
            public double ReDelivery6HoursPrice { get; set; }
            public double Missort2HoursQuantity { get; set; }
            public double Missort2HoursPrice { get; set; }
            public double Missort4HoursQuantity { get; set; }
            public double Missort4HoursPrice { get; set; }
            public double Missort6HoursQuantity { get; set; }
            public double Missort6HoursPrice { get; set; }
            public double SameDayQuantity { get; set; }
            public double SameDayPrice { get; set; }
            public double TrainingDayQuantity { get; set; }
            public double TrainingDayPrice { get; set; }
            public double RideAlongQuantity { get; set; }
            public double RideAlongPrice { get; set; }

            public double SupportAd1Quantity { get; set; }
            public double SupportAd1Price { get; set; }
            public double SupportAd2Quantity { get; set; }
            public double SupportAd2Price { get; set; }
            public double SupportAd3Quantity { get; set; }
            public double SupportAd3Price { get; set; }
            public double LeadDriverQuantity { get; set; }
            public double LeadDriverPrice { get; set; }
            public double LargeVanQuantity { get; set; }
            public double LargeVanPrice { get; set; }

            public double CongestionChargeQuantity { get; set; }
            public double CongestionChargePrice { get; set; }
            public double LatePaymentQuantity { get; set; }
            public double LatePaymentPrice { get; set; }


            public double Fuel { get; set; }
            public double Mileage { get; set; }
            public double Deduct { get; set; }
        }


        public JsonResult GetDashboardRouteAllocations(DateTime? FromDate)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            // period
            var dateNow = DateTime.Now.Date;
            var dateFrom = dateNow;
            var dateTo = dateNow;

            // check from date
            if (FromDate.HasValue)
            {
                dateFrom = FromDate.Value;
            }


            // init
            int count = 1;
            List<Allocation> allocations = new List<Allocation>();


            // loop dates
            foreach (DateTime day in Utility.EachDay(dateFrom, dateTo))
            {
                int weekNumber = day.GetWeekNumberOfYear();
                string dayName = day.ToString("dddd");
                var filterDate = day.Date;
                int routeCount = 0;

                var sblDayRoutes1 = db.RouteAllocations.Where(x => x.RouteDate == filterDate && x.Active == true && x.Deleted == false);

                // if poc filter by depot
                if (currentuser.DepotId.HasValue)
                {
                    sblDayRoutes1 = sblDayRoutes1.Where(x => x.DepotId == currentuser.DepotId);
                }

                if (sblDayRoutes1.Any())
                {
                    var sblDayRoutes = sblDayRoutes1;
                    double sblDayFullQuantity = 0;
                    double sblDayFullPrice = 0;
                    double sblDayHalfQuantity = 0;
                    double sblDayHalfPrice = 0;
                    double sblDayRemoteDebriefQuantity = 0;
                    double sblDayRemoteDebriefPrice = 0;
                    double sblDayNurseryRoutesLevel1Quantity = 0;
                    double sblDayNurseryRoutesLevel1Price = 0;
                    double sblDayNurseryRoutesLevel2Quantity = 0;
                    double sblDayNurseryRoutesLevel2Price = 0;
                    double sblDayRescue2HoursQuantity = 0;
                    double sblDayRescue2HoursPrice = 0;
                    double sblDayRescue4HoursQuantity = 0;
                    double sblDayRescue4HoursPrice = 0;
                    double sblDayRescue6HoursQuantity = 0;
                    double sblDayRescue6HoursPrice = 0;
                    double sblDayReDelivery2HoursQuantity = 0;
                    double sblDayReDelivery2HoursPrice = 0;
                    double sblDayReDelivery4HoursQuantity = 0;
                    double sblDayReDelivery4HoursPrice = 0;
                    double sblDayReDelivery6HoursQuantity = 0;
                    double sblDayReDelivery6HoursPrice = 0;
                    double sblDayMissort2HoursQuantity = 0;
                    double sblDayMissort2HoursPrice = 0;
                    double sblDayMissort4HoursQuantity = 0;
                    double sblDayMissort4HoursPrice = 0;
                    double sblDayMissort6HoursQuantity = 0;
                    double sblDayMissort6HoursPrice = 0;
                    double sblDaySameDayQuantity = 0;
                    double sblDaySameDayPrice = 0;
                    double sblDayTrainingDayQuantity = 0;
                    double sblDayTrainingDayPrice = 0;
                    double sblDayRideAlongQuantity = 0;
                    double sblDayRideAlongPrice = 0;

                    double sblDaySupportAd1Quantity = 0;
                    double sblDaySupportAd1Price = 0;
                    double sblDaySupportAd2Quantity = 0;
                    double sblDaySupportAd2Price = 0;
                    double sblDaySupportAd3Quantity = 0;
                    double sblDaySupportAd3Price = 0;

                    double sblDayLeadDriverQuantity = 0;
                    double sblDayLeadDriverPrice = 0;
                    double sblDayLargeVanQuantity = 0;
                    double sblDayLargeVanPrice = 0;

                    double sblDayCongestionChargeQuantity = 0;
                    double sblDayCongestionChargePrice = 0;
                    double sblDayLatePaymentQuantity = 0;
                    double sblDayLatePaymentPrice = 0;

                    double sblDayFuel = 0;
                    double sblDayMileage = 0;
                    double sblDayDeduct = 0;

                    #region routeDetail
                    foreach (var route in sblDayRoutes)
                    {
                        if (route.RouteType1 == "Full")
                        {
                            sblDayFullQuantity++;
                            sblDayFullPrice = sblDayFullPrice + (sblDayFullQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "Half")
                        {
                            sblDayHalfQuantity++;
                            sblDayHalfPrice = sblDayHalfPrice + (sblDayHalfQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "RemoteDebrief")
                        {
                            sblDayRemoteDebriefQuantity++;
                            sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + (sblDayRemoteDebriefQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "NurseryRoutesLevel1")
                        {
                            sblDayNurseryRoutesLevel1Quantity++;
                            sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + (sblDayNurseryRoutesLevel1Quantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "NurseryRoutesLevel2")
                        {
                            sblDayNurseryRoutesLevel2Quantity++;
                            sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + (sblDayNurseryRoutesLevel2Quantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "Rescue2Hours")
                        {
                            sblDayRescue2HoursQuantity++;
                            sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + (sblDayRescue2HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "Rescue4Hours")
                        {
                            sblDayRescue4HoursQuantity++;
                            sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + (sblDayRescue4HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "Rescue6Hours")
                        {
                            sblDayRescue6HoursQuantity++;
                            sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + (sblDayRescue6HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "ReDelivery2Hours")
                        {
                            sblDayReDelivery2HoursQuantity++;
                            sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + (sblDayReDelivery2HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "ReDelivery4Hours")
                        {
                            sblDayReDelivery4HoursQuantity++;
                            sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + (sblDayReDelivery4HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "ReDelivery6Hours")
                        {
                            sblDayReDelivery6HoursQuantity++;
                            sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + (sblDayReDelivery6HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "Missort2Hours")
                        {
                            sblDayMissort2HoursQuantity++;
                            sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + (sblDayMissort2HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "Missort4Hours")
                        {
                            sblDayMissort4HoursQuantity++;
                            sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + (sblDayMissort4HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "Missort6Hours")
                        {
                            sblDayMissort6HoursQuantity++;
                            sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + (sblDayMissort6HoursQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "SameDay")
                        {
                            sblDaySameDayQuantity++;
                            sblDaySameDayPrice = sblDaySameDayPrice + (sblDaySameDayQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "TrainingDay")
                        {
                            sblDayTrainingDayQuantity++;
                            sblDayTrainingDayPrice = sblDayTrainingDayPrice + (sblDayTrainingDayQuantity * route.RoutePrice1);
                        }

                        if (route.RouteType1 == "RideAlong")
                        {
                            sblDayRideAlongQuantity++;
                            sblDayRideAlongPrice = sblDayRideAlongPrice + (sblDayRideAlongQuantity * route.RoutePrice1);
                        }



                        sblDayFuel = sblDayFuel + route.Fuel;
                        sblDayMileage = sblDayMileage + route.Mileage;
                        sblDayDeduct = sblDayDeduct + route.Deduct;

                        routeCount++;
                    }
                    #endregion

                    // add to list
                    allocations.Add(new Allocation
                    {
                        Id = count,
                        Week = weekNumber + "/" + dayName,
                        Date = filterDate,
                        Routes = routeCount,
                        FullQuantity = sblDayFullQuantity,
                        FullPrice = sblDayFullPrice,
                        HalfQuantity = sblDayHalfQuantity,
                        HalfPrice = sblDayHalfPrice,
                        RemoteDebriefQuantity = sblDayRemoteDebriefQuantity,
                        RemoteDebriefPrice = sblDayRemoteDebriefPrice,
                        NurseryRoutesLevel1Quantity = sblDayNurseryRoutesLevel1Quantity,
                        NurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price,
                        NurseryRoutesLevel2Quantity = sblDayNurseryRoutesLevel2Quantity,
                        NurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price,
                        Rescue2HoursQuantity = sblDayRescue2HoursQuantity,
                        Rescue2HoursPrice = sblDayRescue2HoursPrice,
                        Rescue4HoursQuantity = sblDayRescue4HoursQuantity,
                        Rescue4HoursPrice = sblDayRescue4HoursPrice,
                        Rescue6HoursQuantity = sblDayRescue6HoursQuantity,
                        Rescue6HoursPrice = sblDayRescue6HoursPrice,
                        ReDelivery2HoursQuantity = sblDayReDelivery2HoursQuantity,
                        ReDelivery2HoursPrice = sblDayReDelivery2HoursPrice,
                        ReDelivery4HoursQuantity = sblDayReDelivery4HoursQuantity,
                        ReDelivery4HoursPrice = sblDayReDelivery4HoursPrice,
                        ReDelivery6HoursQuantity = sblDayReDelivery6HoursQuantity,
                        ReDelivery6HoursPrice = sblDayReDelivery6HoursPrice,
                        Missort2HoursQuantity = sblDayMissort2HoursQuantity,
                        Missort2HoursPrice = sblDayMissort2HoursPrice,
                        Missort4HoursQuantity = sblDayMissort4HoursQuantity,
                        Missort4HoursPrice = sblDayMissort4HoursPrice,
                        Missort6HoursQuantity = sblDayMissort6HoursQuantity,
                        Missort6HoursPrice = sblDayMissort6HoursPrice,
                        SameDayQuantity = sblDaySameDayQuantity,
                        SameDayPrice = sblDaySameDayPrice,
                        TrainingDayQuantity = sblDayTrainingDayQuantity,
                        TrainingDayPrice = sblDayTrainingDayPrice,
                        RideAlongQuantity = sblDayRideAlongQuantity,
                        RideAlongPrice = sblDayRideAlongPrice,

                        Fuel = sblDayFuel,
                        Mileage = sblDayMileage,
                        Deduct = sblDayDeduct
                    });

                    count++;
                }
            }

            return Json(new
            {
                allocations = allocations
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        







        public ActionResult VehicleList()
        {
            return View(db.Vehicles.Where(x => x.Active == true && x.Deleted == false).ToList());
        }

        public ActionResult VehicleCreate()
        {
            IEnumerable<SelectListItem> rentals = db.Rentals.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Rentals = rentals;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult VehicleCreate(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    vehicle.Status = "Available";
                    vehicle.DateCreated = DateTime.Now;
                    vehicle.Active = true;
                    vehicle.Deleted = false;
                    db.Vehicles.Add(vehicle);
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("vehiclelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> rentals = db.Rentals.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Rentals = rentals;

            return View();
        }

        public ActionResult VehicleEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Vehicle vehicle = db.Vehicles.Find(id);

            if (vehicle == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> rentals = db.Rentals.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == vehicle.RentalId,
            });

            ViewBag.Rentals = rentals;

            return View(vehicle);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult VehicleEdit(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    var original = db.Vehicles.FirstOrDefault(x => x.Id == vehicle.Id);
                    if (original != null)
                    {
                        original.RentalId = vehicle.RentalId;
                        original.Make = vehicle.Make;
                        original.Model = vehicle.Model;
                        original.Registration = vehicle.Registration;
                        original.MileageRented = vehicle.MileageRented;
                        original.DateRented = vehicle.DateRented;
                        original.RentalPrice = original.RentalPrice; // only admin can change, so get from original here
                        original.NextServiceDate = vehicle.NextServiceDate;
                        //
                        original.MileageReturned = original.MileageReturned;
                        original.DateReturned = original.DateReturned;
                        original.Status = original.Status;
                        //
                        original.Active = original.Active;
                        original.DateCreated = original.DateCreated;
                        original.Deleted = original.Deleted;
                        db.SaveChanges();
                    }


                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("vehiclelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> rentals = db.Rentals.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == vehicle.RentalId,
            });

            ViewBag.Rentals = rentals;

            return View(vehicle);
        }

        public ActionResult VehicleFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Vehicle vehicle = db.Vehicles.Find(id);

            if (vehicle == null)
            {
                return HttpNotFound();
            }

            return View(vehicle);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult VehicleFile(int? Id, string Description, DateTime? ExpiryDate, HttpPostedFileBase FileUpload)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    var original = db.Vehicles.Where(x => x.Id == Id).FirstOrDefault();

                    if (FileUpload != null && FileUpload.ContentLength > 0)
                    {
                        Int32 length = FileUpload.ContentLength;
                        byte[] temp = new byte[length];
                        FileUpload.InputStream.Read(temp, 0, length);

                        VehicleFile vehiclefile = new VehicleFile();
                        vehiclefile.VehicleId = original.Id;

                        vehiclefile.DataFile = temp;
                        vehiclefile.DataFileContentType = FileUpload.ContentType;
                        vehiclefile.DataFileName = FileUpload.FileName;
                        vehiclefile.DataFileDescription = Description;
                        vehiclefile.DataFileExpiryDate = ExpiryDate;

                        vehiclefile.DateCreated = DateTime.Now;
                        vehiclefile.Active = true;
                        vehiclefile.Deleted = false;
                        db.VehicleFiles.Add(vehiclefile);
                        db.SaveChanges();
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleFile - User: " + currentuser.Id, "", false);

                    return RedirectToAction("vehiclefile", new { id = original.Id });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleFile - User: " + currentuser.Id, ex.Message, true);
                }
            }

            Vehicle vehicle = db.Vehicles.Find(Id);

            return View(vehicle);
        }

        public JsonResult VehicleFileRemove(int? VehicleFileId)
        {
            var vehiclefile = db.VehicleFiles.Where(x => x.Id == VehicleFileId).FirstOrDefault();
            vehiclefile.Deleted = true;
            db.SaveChanges();

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult VehicleFileDownload(int id)
        {
            var vehiclefile = db.VehicleFiles.Where(x => x.Id == id).FirstOrDefault();

            return File(vehiclefile.DataFile, vehiclefile.DataFileContentType, vehiclefile.DataFileName);
        }

        public ActionResult VehicleServiceList()
        {
            return View(db.VehicleServices.Where(x => x.Active == true && x.Deleted == false).ToList());
        }

        public ActionResult VehicleServiceCreate()
        {
            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Vehicles = vehicles;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult VehicleServiceCreate(VehicleService vehicleservice)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    vehicleservice.DateCreated = DateTime.Now;
                    vehicleservice.Active = true;
                    vehicleservice.Deleted = false;
                    db.VehicleServices.Add(vehicleservice);
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleServiceCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("vehicleservicelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleServiceCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Vehicles = vehicles;

            return View();
        }

        public ActionResult VehicleServiceEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VehicleService vehicleservice = db.VehicleServices.Find(id);

            if (vehicleservice == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = x.Id == vehicleservice.VehicleId,
            });

            ViewBag.Vehicles = vehicles;

            return View(vehicleservice);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult VehicleServiceEdit(VehicleService vehicleservice)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.VehicleServices.Where(x => x.Id == vehicleservice.Id).FirstOrDefault();

                    original.VehicleId = vehicleservice.VehicleId;
                    original.ServiceDate = vehicleservice.ServiceDate;
                    original.Description = vehicleservice.Description;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleServiceEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("vehicleservicelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleServiceEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = x.Id == vehicleservice.VehicleId,
            });

            ViewBag.Vehicles = vehicles;

            return View(vehicleservice);
        }

        
        public ActionResult VehicleReturn(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Vehicle vehicle = db.Vehicles.Find(id);

            if (vehicle == null)
            {
                return HttpNotFound();
            }

            return View(vehicle);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult VehicleReturn(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    var original = db.Vehicles.FirstOrDefault(x => x.Id == vehicle.Id);
                    if (original != null)
                    {
                        original.RentalId = original.RentalId;
                        original.Make = original.Make;
                        original.Model = original.Model;
                        original.Registration = original.Registration;
                        original.MileageRented = original.MileageRented;
                        original.DateRented = original.DateRented;
                        original.RentalPrice = original.RentalPrice; // only admin can change, so get from original here
                        original.NextServiceDate = original.NextServiceDate;
                        //
                        original.MileageReturned = vehicle.MileageReturned;
                        original.DateReturned = vehicle.DateReturned;
                        original.Status = "Returned";
                        //
                        original.Active = original.Active;
                        original.DateCreated = original.DateCreated;
                        original.Deleted = original.Deleted;
                        db.SaveChanges();
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleReturn - User: " + currentuser.Id, "", false);

                    return RedirectToAction("vehiclelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: VehicleReturn - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(vehicle);
        }




        public ActionResult VehicleCancelReturn(int? id)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            try
            {
                Vehicle vehicle = db.Vehicles.Find(id);
                vehicle.Status = "Rented";
                db.SaveChanges();

                // log
                Helpers.Logging.LogEntry("Admin: VehicleCancelReturn - User: " + currentuser.Id, "", false);

                return RedirectToAction("vehiclelist");
            }
            catch (Exception ex)
            {
                // log
                Helpers.Logging.LogEntry("Admin: VehicleCancelReturn - User: " + currentuser.Id, ex.Message, true);
            }

            return RedirectToAction("vehiclelist");
        }



















        public ActionResult InspectionList()
        {
            return View(db.Inspections.Where(x => x.Active == true && x.Deleted == false).ToList());
        }

        public ActionResult InspectionCreate()
        {
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.SubRentals.Where(y => y.Status == "Rented" && y.Active == true && x.Deleted == false).Any() && x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult InspectionCreate(Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    inspection.DateCreated = DateTime.Now;
                    inspection.Active = true;
                    inspection.Deleted = false;
                    db.Inspections.Add(inspection);
                    db.SaveChanges();

                    // update vehicle status
                    var vehicle = db.Vehicles.Where(x => x.Id == inspection.VehicleId).FirstOrDefault().Status = "Rented";
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: InspectionCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("inspectionlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: InspectionCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.SubRentals.Where(y => y.Status == "Rented" && y.Active == true && x.Deleted == false).Any() && x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            return View();
        }

        public ActionResult InspectionEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Inspection inspection = db.Inspections.Find(id);

            if (inspection == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.SubRentals.Where(y => y.Status == "Rented" && y.Active == true && x.Deleted == false).Any() && x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == inspection.AssociateId,
            });

            ViewBag.Associates = associates;

            return View(inspection);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult InspectionEdit(Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Inspections.Where(x => x.Id == inspection.Id).FirstOrDefault();

                    original.VehicleId = inspection.VehicleId;
                    original.AssociateId = inspection.AssociateId;

                    original.InspectionDueDate = inspection.InspectionDueDate;
                    original.DateOut = inspection.DateOut;
                    original.OdometerOut = inspection.OdometerOut;
                    original.FuelLevelOut = inspection.FuelLevelOut;

                    original.ConditionSide1Out = inspection.ConditionSide1Out;
                    original.ConditionSide2Out = inspection.ConditionSide2Out;
                    original.ConditionFrontOut = inspection.ConditionFrontOut;
                    original.ConditionBackOut = inspection.ConditionBackOut;
                    original.ConditionWindshieldOut = inspection.ConditionWindshieldOut;

                    original.AdditionalDamageCommentsOut = inspection.AdditionalDamageCommentsOut;

                    original.TyreOutNSMM1 = inspection.TyreOutNSMM1;
                    original.TyreOutNSPSI1 = inspection.TyreOutNSPSI1;
                    original.TyreOutNSMake1 = inspection.TyreOutNSMake1;
                    original.TyreOutNSMM2 = inspection.TyreOutNSMM2;
                    original.TyreOutNSPSI2 = inspection.TyreOutNSPSI2;
                    original.TyreOutNSMake2 = inspection.TyreOutNSMake2;

                    original.TyreOutOSMM1 = inspection.TyreOutOSMM1;
                    original.TyreOutOSPSI1 = inspection.TyreOutOSPSI1;
                    original.TyreOutOSMake1 = inspection.TyreOutOSMake1;
                    original.TyreOutOSMM2 = inspection.TyreOutOSMM2;
                    original.TyreOutOSPSI2 = inspection.TyreOutOSPSI2;
                    original.TyreOutOSMake2 = inspection.TyreOutOSMake2;

                    original.TyreOutSpareTyreMM = inspection.TyreOutSpareTyreMM;
                    original.TyreOutSpareTyrePSI = inspection.TyreOutSpareTyrePSI;
                    original.TyreOutSpareTyreMake = inspection.TyreOutSpareTyreMake;

                    original.LightsIndicatorsHornOut = inspection.LightsIndicatorsHornOut;
                    original.WindscreenWipersWasherOut = inspection.WindscreenWipersWasherOut;
                    original.RadioOut = inspection.RadioOut;
                    original.DriversHandbookOut = inspection.DriversHandbookOut;
                    original.CoolantLevelOut = inspection.CoolantLevelOut;
                    original.EngineOilLevelOut = inspection.EngineOilLevelOut;
                    original.BrakeFluidLevelOut = inspection.BrakeFluidLevelOut;
                    original.TyrePressuresCheckedOut = inspection.TyrePressuresCheckedOut;
                    original.SeatbeltConditionAndOperationOut = inspection.SeatbeltConditionAndOperationOut;
                    original.CigaretteLighterOut = inspection.CigaretteLighterOut;
                    original.JackAndToolsPresentOut = inspection.JackAndToolsPresentOut;
                    original.HandbrakeOperationOut = inspection.HandbrakeOperationOut;
                    original.InternalConditionCleanlinessOut = inspection.InternalConditionCleanlinessOut;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    var vehicle = db.Vehicles.Where(x => x.Id == inspection.VehicleId).FirstOrDefault().Status = "Rented";
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: InspectionEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("inspectionlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: InspectionEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.SubRentals.Where(y => y.Status == "Rented" && y.Active == true && x.Deleted == false).Any() && x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == inspection.AssociateId,
            });

            ViewBag.Associates = associates;

            return View(inspection);
        }

        public JsonResult GetAssociateVehicle(int? AssociateId)
        {
            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();

            var vehicle = associate.SubRentals.Where(x => x.Status == "Rented" && x.Active == true && x.Deleted == false).FirstOrDefault().Vehicle;

            string vehiclename = vehicle.Make + " " + vehicle.Model + " " + vehicle.Registration;

            return Json(new
            {
                vehiclename = vehiclename,
                vehicleId = vehicle.Id
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InspectionIn(int? id)
        {
            var inspection = db.Inspections.Where(x => x.Id == id).FirstOrDefault();

            var viewModel = new InspectionInViewModel
            {
                Id = inspection.Id,
                VehicleId = inspection.VehicleId,

                DateSignedSblIn = inspection.DateSignedSblIn,
                SignatureSblIn = inspection.SignatureSblIn,

                DateSignedAssociateIn = inspection.DateSignedAssociateIn,
                SignatureAssociateIn = inspection.SignatureAssociateIn,

                DateIn = inspection.DateIn,
                OdometerIn = inspection.OdometerIn,
                FuelLevelIn = inspection.FuelLevelIn,

                ConditionSide1In = inspection.ConditionSide1In,
                ConditionSide2In = inspection.ConditionSide2In,
                ConditionFrontIn = inspection.ConditionFrontIn,
                ConditionBackIn = inspection.ConditionBackIn,
                ConditionWindshieldIn = inspection.ConditionWindshieldIn,

                AdditionalDamageCommentsIn = inspection.AdditionalDamageCommentsIn,

                TyreInNSMM1 = inspection.TyreInNSMM1,
                TyreInNSPSI1 = inspection.TyreInNSPSI1,
                TyreInNSMake1 = inspection.TyreInNSMake1,
                TyreInNSMM2 = inspection.TyreInNSMM2,
                TyreInNSPSI2 = inspection.TyreInNSPSI2,
                TyreInNSMake2 = inspection.TyreInNSMake2,

                TyreInOSMM1 = inspection.TyreInOSMM1,
                TyreInOSPSI1 = inspection.TyreInOSPSI1,
                TyreInOSMake1 = inspection.TyreInOSMake1,
                TyreInOSMM2 = inspection.TyreInOSMM2,
                TyreInOSPSI2 = inspection.TyreInOSPSI2,
                TyreInOSMake2 = inspection.TyreInOSMake2,

                TyreInSpareTyreMM = inspection.TyreInSpareTyreMM,
                TyreInSpareTyrePSI = inspection.TyreInSpareTyrePSI,
                TyreInSpareTyreMake = inspection.TyreInSpareTyreMake,

                LightsIndicatorsHornIn = inspection.LightsIndicatorsHornIn,
                WindscreenWipersWasherIn = inspection.WindscreenWipersWasherIn,
                RadioIn = inspection.RadioIn,
                DriversHandbookIn = inspection.DriversHandbookIn,
                CoolantLevelIn = inspection.CoolantLevelIn,
                EngineOilLevelIn = inspection.EngineOilLevelIn,
                BrakeFluidLevelIn = inspection.BrakeFluidLevelIn,
                TyrePressuresCheckedIn = inspection.TyrePressuresCheckedIn,
                SeatbeltConditionAndOperationIn = inspection.SeatbeltConditionAndOperationIn,
                CigaretteLighterIn = inspection.CigaretteLighterIn,
                JackAndToolsPresentIn = inspection.JackAndToolsPresentIn,
                HandbrakeOperationIn = inspection.HandbrakeOperationIn,
                InternalConditionCleanlinessIn = inspection.InternalConditionCleanlinessIn
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult InspectionIn(InspectionInViewModel inspection)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Inspections.Where(x => x.Id == inspection.Id).FirstOrDefault();

                    original.DateIn = inspection.DateIn;
                    original.OdometerIn = inspection.OdometerIn;
                    original.FuelLevelIn = inspection.FuelLevelIn;
                    original.AdditionalDamageCommentsIn = inspection.AdditionalDamageCommentsIn;

                    original.TyreInNSMM1 = inspection.TyreInNSMM1;
                    original.TyreInNSPSI1 = inspection.TyreInNSPSI1;
                    original.TyreInNSMake1 = inspection.TyreInNSMake1;
                    original.TyreInNSMM2 = inspection.TyreInNSMM2;
                    original.TyreInNSPSI2 = inspection.TyreInNSPSI2;
                    original.TyreInNSMake2 = inspection.TyreInNSMake2;

                    original.TyreInOSMM1 = inspection.TyreInOSMM1;
                    original.TyreInOSPSI1 = inspection.TyreInOSPSI1;
                    original.TyreInOSMake1 = inspection.TyreInOSMake1;
                    original.TyreInOSMM2 = inspection.TyreInOSMM2;
                    original.TyreInOSPSI2 = inspection.TyreInOSPSI2;
                    original.TyreInOSMake2 = inspection.TyreInOSMake2;

                    original.TyreInSpareTyreMM = inspection.TyreInSpareTyreMM;
                    original.TyreInSpareTyrePSI = inspection.TyreInSpareTyrePSI;
                    original.TyreInSpareTyreMake = inspection.TyreInSpareTyreMake;

                    original.LightsIndicatorsHornIn = inspection.LightsIndicatorsHornIn;
                    original.WindscreenWipersWasherIn = inspection.WindscreenWipersWasherIn;
                    original.RadioIn = inspection.RadioIn;
                    original.DriversHandbookIn = inspection.DriversHandbookIn;
                    original.CoolantLevelIn = inspection.CoolantLevelIn;
                    original.EngineOilLevelIn = inspection.EngineOilLevelIn;
                    original.BrakeFluidLevelIn = inspection.BrakeFluidLevelIn;
                    original.TyrePressuresCheckedIn = inspection.TyrePressuresCheckedIn;
                    original.SeatbeltConditionAndOperationIn = inspection.SeatbeltConditionAndOperationIn;
                    original.CigaretteLighterIn = inspection.CigaretteLighterIn;
                    original.JackAndToolsPresentIn = inspection.JackAndToolsPresentIn;
                    original.HandbrakeOperationIn = inspection.HandbrakeOperationIn;
                    original.InternalConditionCleanlinessIn = inspection.InternalConditionCleanlinessIn;

                    original.ConditionSide1In = inspection.ConditionSide1In;
                    original.ConditionSide2In = inspection.ConditionSide2In;
                    original.ConditionWindshieldIn = inspection.ConditionWindshieldIn;
                    original.ConditionBackIn = inspection.ConditionBackIn;
                    original.ConditionFrontIn = inspection.ConditionFrontIn;

                    db.SaveChanges();

                    var vehicle = db.Vehicles.Where(x => x.Id == inspection.VehicleId).FirstOrDefault().Status = "Available";
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: InspectionIn - User: " + currentuser.Id, "", false);

                    return RedirectToAction("inspectionlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: InspectionIn - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(inspection);
        }

        //
        // route allocation
        //
        public JsonResult RouteAllocationList(DateTime routeDate)
        {
            var routes = (from x in db.RouteAllocations
                          where x.RouteDate == routeDate
                          && x.Active == true
                          && x.Deleted == false
                          select new RouteAllocationViewModel.Route
                          {
                              Id = x.Id,
                              RouteDate = x.RouteDate,
                              DriverId = x.Associate.Id,
                              DriverName = x.Associate.Name,
                              DepotId = x.Depot.Id,
                              DepotName = x.Depot.Name,
                              RouteCode1 = x.RouteCode1,
                              RouteType1 = x.RouteType1,
                              //
                              RoutePrice1 = x.RoutePrice1,
                              Ad1Quantity = x.Ad1Quantity,
                              Ad1Price = x.Ad1Price,
                              Ad2Quantity = x.Ad2Quantity,
                              Ad2Price = x.Ad2Price,
                              Ad3Quantity = x.Ad3Quantity,
                              Ad3Price = x.Ad3Price,
                              //
                              Fuel = x.Fuel,
                              Mileage = x.Mileage,
                              Deduct = x.Deduct,
                              Notes = x.Notes
                          }).ToList();
            return Json(new
            {
                routes = routes
            }, JsonRequestBehavior.AllowGet);
        }


        /*


        public ActionResult RouteAllocation(DateTime? RouteDate, int? DepotId)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

            // this period
            var dateRoute = DateTime.Now.Date;
            var dateFrom = dateRoute.AddDays(-(int)dateRoute.DayOfWeek + (int)DayOfWeek.Sunday);
            var dateTo = dateFrom.AddDays(6);
            var dateWeek = dateFrom.GetWeekNumberOfYear();


            // check from date
            if (RouteDate.HasValue)
            {
                dateRoute = RouteDate.Value.Date;
                dateFrom = dateRoute.AddDays(-(int)dateRoute.DayOfWeek + (int)DayOfWeek.Sunday);
                dateTo = dateFrom.AddDays(6);
                dateWeek = dateFrom.GetWeekNumberOfYear();
            }
            var hasWritePermision = false;
            if (User.IsInRole(WebConstant.SBLUserRole.Master) || User.IsInRole(WebConstant.SBLUserRole.Admin))
            {
                hasWritePermision = true;
            }
            else
            {
                if (currentuser != null && currentuser.DepotId == DepotId)
                {
                    hasWritePermision = true;
                }
                else
                {
                    hasWritePermision = false;
                }
            }
            int? currentUserDepotId = null;
            if (User.IsInRole(WebConstant.SBLUserRole.Master) || User.IsInRole(WebConstant.SBLUserRole.Admin))
            {
                if (DepotId.HasValue)
                {
                    currentUserDepotId = DepotId;
                }
            }
            else
            {
                if (currentuser != null)
                {
                    if (DepotId.HasValue && DepotId > 0)
                    {
                        if (DepotId == currentuser.DepotId)
                        {
                            currentUserDepotId = currentuser.DepotId;
                        }
                    }
                }
            }
            if (currentUserDepotId != null && currentUserDepotId.Value > 0)
            {
                var routeAmazons = db.RouteAmazons.FirstOrDefault(x => x.RouteDate == dateRoute && x.DepotId == currentUserDepotId);
                if (routeAmazons == null || routeAmazons.Id <= 0)
                {
                    RouteAmazon route = new RouteAmazon();
                    route.RouteDate = dateRoute;
                    route.DepotId = currentUserDepotId;
                    route.DateCreated = DateTime.Now;
                    route.Active = true;
                    route.Deleted = false;
                    db.RouteAmazons.Add(route);
                    db.SaveChanges();
                }
            }

            var routes = (from x in db.RouteAllocations
                          where x.RouteDate == dateRoute
                          && x.Active == true
                          && x.Deleted == false
                          && x.DepotId == DepotId
                          select new RouteAllocationViewModel.Route
                          {
                              Id = x.Id,
                              RouteDate = x.RouteDate,
                              DriverId = x.Associate.Id,
                              DriverName = x.Associate.Name,
                              DepotId = x.Depot.Id,
                              DepotName = x.Depot.Name,
                              RouteCode1 = x.RouteCode1,
                              RouteType1 = x.RouteType1,
                              //
                              RoutePrice1 = x.RoutePrice1,
                              Ad1Quantity = x.Ad1Quantity,
                              Ad1Price = x.Ad1Price,
                              Ad2Quantity = x.Ad2Quantity,
                              Ad2Price = x.Ad2Price,
                              Ad3Quantity = x.Ad3Quantity,
                              Ad3Price = x.Ad3Price,
                              //
                              Fuel = x.Fuel,
                              Mileage = x.Mileage,
                              Deduct = x.Deduct,
                              Notes = x.Notes
                          }).ToList();



            // depots
            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Depots = depots;

            // amazon route id
            int RouteAmazonId = 0;

            #region amazon day
            // amazon day
            double amazonDayFullQuantity = 0;
            double amazonDayFullPrice = 0;
            double amazonDayHalfQuantity = 0;
            double amazonDayHalfPrice = 0;
            double amazonDayRemoteDebriefQuantity = 0;
            double amazonDayRemoteDebriefPrice = 0;
            double amazonDayNurseryRoutesLevel1Quantity = 0;
            double amazonDayNurseryRoutesLevel1Price = 0;
            double amazonDayNurseryRoutesLevel2Quantity = 0;
            double amazonDayNurseryRoutesLevel2Price = 0;
            double amazonDayRescue2HoursQuantity = 0;
            double amazonDayRescue2HoursPrice = 0;
            double amazonDayRescue4HoursQuantity = 0;
            double amazonDayRescue4HoursPrice = 0;
            double amazonDayRescue6HoursQuantity = 0;
            double amazonDayRescue6HoursPrice = 0;
            double amazonDayReDelivery2HoursQuantity = 0;
            double amazonDayReDelivery2HoursPrice = 0;
            double amazonDayReDelivery4HoursQuantity = 0;
            double amazonDayReDelivery4HoursPrice = 0;
            double amazonDayReDelivery6HoursQuantity = 0;
            double amazonDayReDelivery6HoursPrice = 0;
            double amazonDayMissort2HoursQuantity = 0;
            double amazonDayMissort2HoursPrice = 0;
            double amazonDayMissort4HoursQuantity = 0;
            double amazonDayMissort4HoursPrice = 0;
            double amazonDayMissort6HoursQuantity = 0;
            double amazonDayMissort6HoursPrice = 0;
            double amazonDaySameDayQuantity = 0;
            double amazonDaySameDayPrice = 0;
            double amazonDayTrainingDayQuantity = 0;
            double amazonDayTrainingDayPrice = 0;
            double amazonDayRideAlongQuantity = 0;
            double amazonDayRideAlongPrice = 0;
            double amazonDayAd1Quantity = 0;
            double amazonDayAd1Price = 0;
            double amazonDayAd2Quantity = 0;
            double amazonDayAd2Price = 0;
            double amazonDayAd3Quantity = 0;
            double amazonDayAd3Price = 0;
            double amazonDayFuel = 0;
            double amazonDayMileage = 0;
            double amazonDayDeduct = 0;

            // var amazonDayRoute1 = db.RouteAmazons.Where(x => x.RouteDate == dateRoute && x.Active == true && x.Deleted == false);
            var amazonDayRoute1 = db.RouteAmazons.Where(x => x.RouteDate == dateRoute && x.Active == true && x.Deleted == false && x.DepotId == DepotId);


            if (amazonDayRoute1.Any())
            {
                //var amazonDayRoute = db.RouteAmazons.Where(x => x.RouteDate == dateRoute && x.Active == true && x.Deleted == false).FirstOrDefault();

                var amazonDayRoute = amazonDayRoute1.FirstOrDefault();

                // set route id
                RouteAmazonId = amazonDayRoute.Id;

                amazonDayFullQuantity = amazonDayRoute.FullQuantity;
                amazonDayFullPrice = amazonDayRoute.FullQuantity * amazonDayRoute.FullPrice;

                amazonDayHalfQuantity = amazonDayRoute.HalfQuantity;
                amazonDayHalfPrice = amazonDayRoute.HalfQuantity * amazonDayRoute.HalfPrice;

                amazonDayRemoteDebriefQuantity = amazonDayRoute.RemoteDebriefQuantity;
                amazonDayRemoteDebriefPrice = amazonDayRoute.RemoteDebriefQuantity * amazonDayRoute.RemoteDebriefPrice;

                amazonDayNurseryRoutesLevel1Quantity = amazonDayRoute.NurseryRoutesLevel1Quantity;
                amazonDayNurseryRoutesLevel1Price = amazonDayRoute.NurseryRoutesLevel1Quantity * amazonDayRoute.NurseryRoutesLevel1Price;

                amazonDayNurseryRoutesLevel2Quantity = amazonDayRoute.NurseryRoutesLevel2Quantity;
                amazonDayNurseryRoutesLevel2Price = amazonDayRoute.NurseryRoutesLevel2Quantity * amazonDayRoute.NurseryRoutesLevel2Price;

                amazonDayRescue2HoursQuantity = amazonDayRoute.Rescue2HoursQuantity;
                amazonDayRescue2HoursPrice = amazonDayRoute.Rescue2HoursQuantity * amazonDayRoute.Rescue2HoursPrice;

                amazonDayRescue4HoursQuantity = amazonDayRoute.Rescue4HoursQuantity;
                amazonDayRescue4HoursPrice = amazonDayRoute.Rescue4HoursQuantity * amazonDayRoute.Rescue4HoursPrice;

                amazonDayRescue6HoursQuantity = amazonDayRoute.Rescue6HoursQuantity;
                amazonDayRescue6HoursPrice = amazonDayRoute.Rescue6HoursQuantity * amazonDayRoute.Rescue6HoursPrice;

                amazonDayReDelivery2HoursQuantity = amazonDayRoute.ReDelivery2HoursQuantity;
                amazonDayReDelivery2HoursPrice = amazonDayRoute.ReDelivery2HoursQuantity * amazonDayRoute.ReDelivery2HoursPrice;

                amazonDayReDelivery4HoursQuantity = amazonDayRoute.ReDelivery4HoursQuantity;
                amazonDayReDelivery4HoursPrice = amazonDayRoute.ReDelivery4HoursQuantity * amazonDayRoute.ReDelivery4HoursPrice;

                amazonDayReDelivery6HoursQuantity = amazonDayRoute.ReDelivery6HoursQuantity;
                amazonDayReDelivery6HoursPrice = amazonDayRoute.ReDelivery6HoursQuantity * amazonDayRoute.ReDelivery6HoursPrice;

                amazonDayMissort2HoursQuantity = amazonDayRoute.Missort2HoursQuantity;
                amazonDayMissort2HoursPrice = amazonDayRoute.Missort2HoursQuantity * amazonDayRoute.Missort2HoursPrice;

                amazonDayMissort4HoursQuantity = amazonDayRoute.Missort4HoursQuantity;
                amazonDayMissort4HoursPrice = amazonDayRoute.Missort4HoursQuantity * amazonDayRoute.Missort4HoursPrice;

                amazonDayMissort6HoursQuantity = amazonDayRoute.Missort6HoursQuantity;
                amazonDayMissort6HoursPrice = amazonDayRoute.Missort6HoursQuantity * amazonDayRoute.Missort6HoursPrice;

                amazonDaySameDayQuantity = amazonDayRoute.SameDayQuantity;
                amazonDaySameDayPrice = amazonDayRoute.SameDayQuantity * amazonDayRoute.SameDayPrice;

                amazonDayTrainingDayQuantity = amazonDayRoute.TrainingDayQuantity;
                amazonDayTrainingDayPrice = amazonDayRoute.TrainingDayQuantity * amazonDayRoute.TrainingDayPrice;

                amazonDayRideAlongQuantity = amazonDayRoute.RideAlongQuantity;
                amazonDayRideAlongPrice = amazonDayRoute.RideAlongQuantity * amazonDayRoute.RideAlongPrice;

                amazonDayAd1Quantity = amazonDayRoute.Ad1Quantity;
                amazonDayAd1Price = amazonDayRoute.Ad1Quantity * amazonDayRoute.Ad1Price;

                amazonDayAd2Quantity = amazonDayRoute.Ad2Quantity;
                amazonDayAd2Price = amazonDayRoute.Ad2Quantity * amazonDayRoute.Ad2Price;

                amazonDayAd3Quantity = amazonDayRoute.Ad3Quantity;
                amazonDayAd3Price = amazonDayRoute.Ad3Quantity * amazonDayRoute.Ad3Price;

                amazonDayFuel = amazonDayRoute.Fuel;
                amazonDayMileage = amazonDayRoute.Mileage;
                amazonDayDeduct = amazonDayRoute.Deduct;
            }
            #endregion

            #region amazon week
            // amazon week
            double amazonWeekFullQuantity = 0;
            double amazonWeekFullPrice = 0;
            double amazonWeekHalfQuantity = 0;
            double amazonWeekHalfPrice = 0;
            double amazonWeekRemoteDebriefQuantity = 0;
            double amazonWeekRemoteDebriefPrice = 0;
            double amazonWeekNurseryRoutesLevel1Quantity = 0;
            double amazonWeekNurseryRoutesLevel1Price = 0;
            double amazonWeekNurseryRoutesLevel2Quantity = 0;
            double amazonWeekNurseryRoutesLevel2Price = 0;
            double amazonWeekRescue2HoursQuantity = 0;
            double amazonWeekRescue2HoursPrice = 0;
            double amazonWeekRescue4HoursQuantity = 0;
            double amazonWeekRescue4HoursPrice = 0;
            double amazonWeekRescue6HoursQuantity = 0;
            double amazonWeekRescue6HoursPrice = 0;
            double amazonWeekReDelivery2HoursQuantity = 0;
            double amazonWeekReDelivery2HoursPrice = 0;
            double amazonWeekReDelivery4HoursQuantity = 0;
            double amazonWeekReDelivery4HoursPrice = 0;
            double amazonWeekReDelivery6HoursQuantity = 0;
            double amazonWeekReDelivery6HoursPrice = 0;
            double amazonWeekMissort2HoursQuantity = 0;
            double amazonWeekMissort2HoursPrice = 0;
            double amazonWeekMissort4HoursQuantity = 0;
            double amazonWeekMissort4HoursPrice = 0;
            double amazonWeekMissort6HoursQuantity = 0;
            double amazonWeekMissort6HoursPrice = 0;
            double amazonWeekSameDayQuantity = 0;
            double amazonWeekSameDayPrice = 0;
            double amazonWeekTrainingDayQuantity = 0;
            double amazonWeekTrainingDayPrice = 0;
            double amazonWeekRideAlongQuantity = 0;
            double amazonWeekRideAlongPrice = 0;
            double amazonWeekAd1Quantity = 0;
            double amazonWeekAd1Price = 0;
            double amazonWeekAd2Quantity = 0;
            double amazonWeekAd2Price = 0;
            double amazonWeekAd3Quantity = 0;
            double amazonWeekAd3Price = 0;
            double amazonWeekFuel = 0;
            double amazonWeekMileage = 0;
            double amazonWeekDeduct = 0;

            var amazonWeekRoute1 = db.RouteAmazons.Where(x => x.RouteDate >= dateFrom && x.RouteDate <= dateTo && x.Active == true && x.Deleted == false && x.DepotId == DepotId);

            if (amazonWeekRoute1.Any())
            {
                var amazonWeekRoutes = amazonWeekRoute1;

                foreach (var route in amazonWeekRoutes)
                {
                    amazonWeekFullQuantity = amazonWeekFullQuantity + route.FullQuantity;
                    amazonWeekFullPrice = amazonWeekFullPrice + (route.FullQuantity * route.FullPrice);

                    amazonWeekHalfQuantity = amazonWeekHalfQuantity + route.HalfQuantity;
                    amazonWeekHalfPrice = amazonWeekHalfPrice + (route.HalfQuantity * route.HalfPrice);

                    amazonWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity + route.RemoteDebriefQuantity;
                    amazonWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice + (route.RemoteDebriefQuantity * route.RemoteDebriefPrice);

                    amazonWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity + route.NurseryRoutesLevel1Quantity;
                    amazonWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price + (route.NurseryRoutesLevel1Quantity * route.NurseryRoutesLevel1Price);

                    amazonWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity + route.NurseryRoutesLevel2Quantity;
                    amazonWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price + (route.NurseryRoutesLevel2Quantity * route.NurseryRoutesLevel2Price);

                    amazonWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity + route.Rescue2HoursQuantity;
                    amazonWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice + (route.Rescue2HoursQuantity * route.Rescue2HoursPrice);

                    amazonWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity + route.Rescue4HoursQuantity;
                    amazonWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice + (route.Rescue4HoursQuantity * route.Rescue4HoursPrice);

                    amazonWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity + route.Rescue6HoursQuantity;
                    amazonWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice + (route.Rescue6HoursQuantity * route.Rescue6HoursPrice);

                    amazonWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity + route.ReDelivery2HoursQuantity;
                    amazonWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice + (route.ReDelivery2HoursQuantity * route.ReDelivery2HoursPrice);

                    amazonWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity + route.ReDelivery4HoursQuantity;
                    amazonWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice + (route.ReDelivery4HoursQuantity * route.ReDelivery4HoursPrice);

                    amazonWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity + route.ReDelivery6HoursQuantity;
                    amazonWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice + (route.ReDelivery6HoursQuantity * route.ReDelivery6HoursPrice);

                    amazonWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity + route.Missort2HoursQuantity;
                    amazonWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice + (route.Missort2HoursQuantity * route.Missort2HoursPrice);

                    amazonWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity + route.Missort4HoursQuantity;
                    amazonWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice + (route.Missort4HoursQuantity * route.Missort4HoursPrice);

                    amazonWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity + route.Missort6HoursQuantity;
                    amazonWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice + (route.Missort6HoursQuantity * route.Missort6HoursPrice);

                    amazonWeekSameDayQuantity = amazonWeekSameDayQuantity + route.SameDayQuantity;
                    amazonWeekSameDayPrice = amazonWeekSameDayPrice + (route.SameDayQuantity * route.SameDayPrice);

                    amazonWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity + route.TrainingDayQuantity;
                    amazonWeekTrainingDayPrice = amazonWeekTrainingDayPrice + (route.TrainingDayQuantity * route.TrainingDayPrice);

                    amazonWeekRideAlongQuantity = amazonWeekRideAlongQuantity + route.RideAlongQuantity;
                    amazonWeekRideAlongPrice = amazonWeekRideAlongPrice + (route.RideAlongQuantity * route.RideAlongPrice);

                    amazonWeekAd1Quantity = amazonWeekAd1Quantity + route.Ad1Quantity;
                    amazonWeekAd1Price = amazonWeekAd1Price + (route.Ad1Quantity * route.Ad1Price);

                    amazonWeekAd2Quantity = amazonWeekAd2Quantity + route.Ad2Quantity;
                    amazonWeekAd2Price = amazonWeekAd2Price + (route.Ad2Quantity * route.Ad2Price);

                    amazonWeekAd3Quantity = amazonWeekAd3Quantity + route.Ad3Quantity;
                    amazonWeekAd3Price = amazonWeekAd3Price + (route.Ad3Quantity * route.Ad3Price);

                    amazonWeekFuel = amazonWeekFuel + route.Fuel;
                    amazonWeekMileage = amazonWeekMileage + route.Mileage;
                    amazonWeekDeduct = amazonWeekDeduct + route.Deduct;
                }
            }
            #endregion

            #region sblDayRoutes1
            // sbl day
            double sblDayFullQuantity = 0;
            double sblDayFullPrice = 0;
            double sblDayHalfQuantity = 0;
            double sblDayHalfPrice = 0;
            double sblDayRemoteDebriefQuantity = 0;
            double sblDayRemoteDebriefPrice = 0;
            double sblDayNurseryRoutesLevel1Quantity = 0;
            double sblDayNurseryRoutesLevel1Price = 0;
            double sblDayNurseryRoutesLevel2Quantity = 0;
            double sblDayNurseryRoutesLevel2Price = 0;
            double sblDayRescue2HoursQuantity = 0;
            double sblDayRescue2HoursPrice = 0;
            double sblDayRescue4HoursQuantity = 0;
            double sblDayRescue4HoursPrice = 0;
            double sblDayRescue6HoursQuantity = 0;
            double sblDayRescue6HoursPrice = 0;
            double sblDayReDelivery2HoursQuantity = 0;
            double sblDayReDelivery2HoursPrice = 0;
            double sblDayReDelivery4HoursQuantity = 0;
            double sblDayReDelivery4HoursPrice = 0;
            double sblDayReDelivery6HoursQuantity = 0;
            double sblDayReDelivery6HoursPrice = 0;
            double sblDayMissort2HoursQuantity = 0;
            double sblDayMissort2HoursPrice = 0;
            double sblDayMissort4HoursQuantity = 0;
            double sblDayMissort4HoursPrice = 0;
            double sblDayMissort6HoursQuantity = 0;
            double sblDayMissort6HoursPrice = 0;
            double sblDaySameDayQuantity = 0;
            double sblDaySameDayPrice = 0;
            double sblDayTrainingDayQuantity = 0;
            double sblDayTrainingDayPrice = 0;
            double sblDayRideAlongQuantity = 0;
            double sblDayRideAlongPrice = 0;
            double sblDayAd1Quantity = 0;
            double sblDayAd1Price = 0;
            double sblDayAd2Quantity = 0;
            double sblDayAd2Price = 0;
            double sblDayAd3Quantity = 0;
            double sblDayAd3Price = 0;
            double sblDayFuel = 0;
            double sblDayMileage = 0;
            double sblDayDeduct = 0;

            var sblDayRoutes1 = db.RouteAllocations.Where(x => x.RouteDate == dateRoute && x.Active == true && x.Deleted == false && x.DepotId == DepotId);


            if (sblDayRoutes1.Any())
            {
                var sblDayRoutes = sblDayRoutes1;

                foreach (var route in sblDayRoutes)
                {
                    if (route.RouteType1 == "Full")
                    {
                        sblDayFullQuantity++;
                        sblDayFullPrice = sblDayFullPrice + (sblDayFullQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Half")
                    {
                        sblDayHalfQuantity++;
                        sblDayHalfPrice = sblDayHalfPrice + (sblDayHalfQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "RemoteDebrief")
                    {
                        sblDayRemoteDebriefQuantity++;
                        sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + (sblDayRemoteDebriefQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "NurseryRoutesLevel1")
                    {
                        sblDayNurseryRoutesLevel1Quantity++;
                        sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + (sblDayNurseryRoutesLevel1Quantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "NurseryRoutesLevel2")
                    {
                        sblDayNurseryRoutesLevel2Quantity++;
                        sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + (sblDayNurseryRoutesLevel2Quantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Rescue2Hours")
                    {
                        sblDayRescue2HoursQuantity++;
                        sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + (sblDayRescue2HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Rescue4Hours")
                    {
                        sblDayRescue4HoursQuantity++;
                        sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + (sblDayRescue4HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Rescue6Hours")
                    {
                        sblDayRescue6HoursQuantity++;
                        sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + (sblDayRescue6HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "ReDelivery2Hours")
                    {
                        sblDayReDelivery2HoursQuantity++;
                        sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + (sblDayReDelivery2HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "ReDelivery4Hours")
                    {
                        sblDayReDelivery4HoursQuantity++;
                        sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + (sblDayReDelivery4HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "ReDelivery6Hours")
                    {
                        sblDayReDelivery6HoursQuantity++;
                        sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + (sblDayReDelivery6HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Missort2Hours")
                    {
                        sblDayMissort2HoursQuantity++;
                        sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + (sblDayMissort2HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Missort4Hours")
                    {
                        sblDayMissort4HoursQuantity++;
                        sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + (sblDayMissort4HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Missort6Hours")
                    {
                        sblDayMissort6HoursQuantity++;
                        sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + (sblDayMissort6HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "SameDay")
                    {
                        sblDaySameDayQuantity++;
                        sblDaySameDayPrice = sblDaySameDayPrice + (sblDaySameDayQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "TrainingDay")
                    {
                        sblDayTrainingDayQuantity++;
                        sblDayTrainingDayPrice = sblDayTrainingDayPrice + (sblDayTrainingDayQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "RideAlong")
                    {
                        sblDayRideAlongQuantity++;
                        sblDayRideAlongPrice = sblDayRideAlongPrice + (sblDayRideAlongQuantity * route.RoutePrice1);
                    }

                    sblDayAd1Quantity = sblDayAd1Quantity + route.Ad1Quantity;
                    sblDayAd1Price = sblDayAd1Price + (sblDayAd1Quantity * route.Ad1Price);

                    sblDayAd2Quantity = sblDayAd2Quantity + route.Ad2Quantity;
                    sblDayAd2Price = sblDayAd2Price + (sblDayAd2Quantity * route.Ad2Price);

                    sblDayAd3Quantity = sblDayAd3Quantity + route.Ad3Quantity;
                    sblDayAd3Price = sblDayAd3Price + (sblDayAd3Quantity * route.Ad3Price);

                    sblDayFuel = sblDayFuel + route.Fuel;
                    sblDayMileage = sblDayMileage + route.Mileage;
                    sblDayDeduct = sblDayDeduct + route.Deduct;
                }
            }
            #endregion

            #region sblWeekRoutes1
            // sbl week
            double sblWeekFullQuantity = 0;
            double sblWeekFullPrice = 0;
            double sblWeekHalfQuantity = 0;
            double sblWeekHalfPrice = 0;
            double sblWeekRemoteDebriefQuantity = 0;
            double sblWeekRemoteDebriefPrice = 0;
            double sblWeekNurseryRoutesLevel1Quantity = 0;
            double sblWeekNurseryRoutesLevel1Price = 0;
            double sblWeekNurseryRoutesLevel2Quantity = 0;
            double sblWeekNurseryRoutesLevel2Price = 0;
            double sblWeekRescue2HoursQuantity = 0;
            double sblWeekRescue2HoursPrice = 0;
            double sblWeekRescue4HoursQuantity = 0;
            double sblWeekRescue4HoursPrice = 0;
            double sblWeekRescue6HoursQuantity = 0;
            double sblWeekRescue6HoursPrice = 0;
            double sblWeekReDelivery2HoursQuantity = 0;
            double sblWeekReDelivery2HoursPrice = 0;
            double sblWeekReDelivery4HoursQuantity = 0;
            double sblWeekReDelivery4HoursPrice = 0;
            double sblWeekReDelivery6HoursQuantity = 0;
            double sblWeekReDelivery6HoursPrice = 0;
            double sblWeekMissort2HoursQuantity = 0;
            double sblWeekMissort2HoursPrice = 0;
            double sblWeekMissort4HoursQuantity = 0;
            double sblWeekMissort4HoursPrice = 0;
            double sblWeekMissort6HoursQuantity = 0;
            double sblWeekMissort6HoursPrice = 0;
            double sblWeekSameDayQuantity = 0;
            double sblWeekSameDayPrice = 0;
            double sblWeekTrainingDayQuantity = 0;
            double sblWeekTrainingDayPrice = 0;
            double sblWeekRideAlongQuantity = 0;
            double sblWeekRideAlongPrice = 0;
            double sblWeekAd1Quantity = 0;
            double sblWeekAd1Price = 0;
            double sblWeekAd2Quantity = 0;
            double sblWeekAd2Price = 0;
            double sblWeekAd3Quantity = 0;
            double sblWeekAd3Price = 0;
            double sblWeekFuel = 0;
            double sblWeekMileage = 0;
            double sblWeekDeduct = 0;

            var sblWeekRoutes1 = db.RouteAllocations.Where(x => x.RouteDate >= dateFrom && x.RouteDate <= dateTo && x.Active == true && x.Deleted == false && x.DepotId == DepotId);


            if (sblWeekRoutes1.Any())
            {
                var sblWeekRoutes = sblWeekRoutes1;

                foreach (var route in sblWeekRoutes)
                {
                    if (route.RouteType1 == "Full")
                    {
                        sblWeekFullQuantity++;
                        sblWeekFullPrice = sblWeekFullPrice + (sblWeekFullQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Half")
                    {
                        sblWeekHalfQuantity++;
                        sblWeekHalfPrice = sblWeekHalfPrice + (sblWeekHalfQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "RemoteDebrief")
                    {
                        sblWeekRemoteDebriefQuantity++;
                        sblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice + (sblWeekRemoteDebriefQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "NurseryRoutesLevel1")
                    {
                        sblWeekNurseryRoutesLevel1Quantity++;
                        sblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price + (sblWeekNurseryRoutesLevel1Quantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "NurseryRoutesLevel2")
                    {
                        sblWeekNurseryRoutesLevel2Quantity++;
                        sblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price + (sblWeekNurseryRoutesLevel2Quantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Rescue2Hours")
                    {
                        sblWeekRescue2HoursQuantity++;
                        sblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice + (sblWeekRescue2HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Rescue4Hours")
                    {
                        sblWeekRescue4HoursQuantity++;
                        sblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice + (sblWeekRescue4HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Rescue6Hours")
                    {
                        sblWeekRescue6HoursQuantity++;
                        sblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice + (sblWeekRescue6HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "ReDelivery2Hours")
                    {
                        sblWeekReDelivery2HoursQuantity++;
                        sblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice + (sblWeekReDelivery2HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "ReDelivery4Hours")
                    {
                        sblWeekReDelivery4HoursQuantity++;
                        sblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice + (sblWeekReDelivery4HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "ReDelivery6Hours")
                    {
                        sblWeekReDelivery6HoursQuantity++;
                        sblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice + (sblWeekReDelivery6HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Missort2Hours")
                    {
                        sblWeekMissort2HoursQuantity++;
                        sblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice + (sblWeekMissort2HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Missort4Hours")
                    {
                        sblWeekMissort4HoursQuantity++;
                        sblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice + (sblWeekMissort4HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "Missort6Hours")
                    {
                        sblWeekMissort6HoursQuantity++;
                        sblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice + (sblWeekMissort6HoursQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "SameDay")
                    {
                        sblWeekSameDayQuantity++;
                        sblWeekSameDayPrice = sblWeekSameDayPrice + (sblWeekSameDayQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "TrainingDay")
                    {
                        sblWeekTrainingDayQuantity++;
                        sblWeekTrainingDayPrice = sblWeekTrainingDayPrice + (sblWeekTrainingDayQuantity * route.RoutePrice1);
                    }

                    if (route.RouteType1 == "RideAlong")
                    {
                        sblWeekRideAlongQuantity++;
                        sblWeekRideAlongPrice = sblWeekRideAlongPrice + (sblWeekRideAlongQuantity * route.RoutePrice1);
                    }

                    sblWeekAd1Quantity = sblWeekAd1Quantity + route.Ad1Quantity;
                    sblWeekAd1Price = sblWeekAd1Price + (sblWeekAd1Quantity * route.Ad1Price);

                    sblWeekAd2Quantity = sblWeekAd2Quantity + route.Ad2Quantity;
                    sblWeekAd2Price = sblWeekAd2Price + (sblWeekAd2Quantity * route.Ad2Price);

                    sblWeekAd3Quantity = sblWeekAd3Quantity + route.Ad3Quantity;
                    sblWeekAd3Price = sblWeekAd3Price + (sblWeekAd3Quantity * route.Ad3Price);

                    sblWeekFuel = sblWeekFuel + route.Fuel;
                    sblWeekMileage = sblWeekMileage + route.Mileage;
                    sblWeekDeduct = sblWeekDeduct + route.Deduct;
                }
            }
            #endregion

            #region diff day
            // diff day
            double diffDayFullQuantity = amazonDayFullQuantity - sblDayFullQuantity;
            double diffDayFullPrice = amazonDayFullPrice - sblDayFullPrice;
            double diffDayHalfQuantity = amazonDayHalfQuantity - sblDayHalfQuantity;
            double diffDayHalfPrice = amazonDayHalfPrice - sblDayHalfPrice;
            double diffDayRemoteDebriefQuantity = amazonDayRemoteDebriefQuantity - sblDayRemoteDebriefQuantity;
            double diffDayRemoteDebriefPrice = amazonDayRemoteDebriefPrice - sblDayRemoteDebriefPrice;
            double diffDayNurseryRoutesLevel1Quantity = amazonDayNurseryRoutesLevel1Quantity - sblDayNurseryRoutesLevel1Quantity;
            double diffDayNurseryRoutesLevel1Price = amazonDayNurseryRoutesLevel1Price - sblDayNurseryRoutesLevel1Price;
            double diffDayNurseryRoutesLevel2Quantity = amazonDayNurseryRoutesLevel2Quantity - sblDayNurseryRoutesLevel2Quantity;
            double diffDayNurseryRoutesLevel2Price = amazonDayNurseryRoutesLevel2Price - sblDayNurseryRoutesLevel2Price;
            double diffDayRescue2HoursQuantity = amazonDayRescue2HoursQuantity - sblDayRescue2HoursQuantity;
            double diffDayRescue2HoursPrice = amazonDayRescue2HoursPrice - sblDayRescue2HoursPrice;
            double diffDayRescue4HoursQuantity = amazonDayRescue4HoursQuantity - sblDayRescue4HoursQuantity;
            double diffDayRescue4HoursPrice = amazonDayRescue4HoursPrice - sblDayRescue4HoursPrice;
            double diffDayRescue6HoursQuantity = amazonDayRescue6HoursQuantity - sblDayRescue6HoursQuantity;
            double diffDayRescue6HoursPrice = amazonDayRescue6HoursPrice - sblDayRescue6HoursPrice;
            double diffDayReDelivery2HoursQuantity = amazonDayReDelivery2HoursQuantity - sblDayReDelivery2HoursQuantity;
            double diffDayReDelivery2HoursPrice = amazonDayReDelivery2HoursPrice - sblDayReDelivery2HoursPrice;
            double diffDayReDelivery4HoursQuantity = amazonDayReDelivery4HoursQuantity - sblDayReDelivery4HoursQuantity;
            double diffDayReDelivery4HoursPrice = amazonDayReDelivery4HoursPrice - sblDayReDelivery4HoursPrice;
            double diffDayReDelivery6HoursQuantity = amazonDayReDelivery6HoursQuantity - sblDayReDelivery6HoursQuantity;
            double diffDayReDelivery6HoursPrice = amazonDayReDelivery6HoursPrice - sblDayReDelivery6HoursPrice;
            double diffDayMissort2HoursQuantity = amazonDayMissort2HoursQuantity - sblDayMissort2HoursQuantity;
            double diffDayMissort2HoursPrice = amazonDayMissort2HoursPrice - sblDayMissort2HoursPrice;
            double diffDayMissort4HoursQuantity = amazonDayMissort4HoursQuantity - sblDayMissort4HoursQuantity;
            double diffDayMissort4HoursPrice = amazonDayMissort4HoursPrice - sblDayMissort4HoursPrice;
            double diffDayMissort6HoursQuantity = amazonDayMissort6HoursQuantity - sblDayMissort6HoursQuantity;
            double diffDayMissort6HoursPrice = amazonDayMissort6HoursPrice - sblDayMissort6HoursPrice;
            double diffDaySameDayQuantity = amazonDaySameDayQuantity - sblDaySameDayQuantity;
            double diffDaySameDayPrice = amazonDaySameDayPrice - sblDaySameDayPrice;
            double diffDayTrainingDayQuantity = amazonDayTrainingDayQuantity - sblDayTrainingDayQuantity;
            double diffDayTrainingDayPrice = amazonDayTrainingDayPrice - sblDayTrainingDayPrice;
            double diffDayRideAlongQuantity = amazonDayRideAlongQuantity - sblDayRideAlongQuantity;
            double diffDayRideAlongPrice = amazonDayRideAlongPrice - sblDayRideAlongPrice;
            double diffDayAd1Quantity = amazonDayAd1Quantity - sblDayAd1Quantity;
            double diffDayAd1Price = amazonDayAd1Price - sblDayAd1Price;
            double diffDayAd2Quantity = amazonDayAd2Quantity - sblDayAd2Quantity;
            double diffDayAd2Price = amazonDayAd2Price - sblDayAd2Price;
            double diffDayAd3Quantity = amazonDayAd3Quantity - sblDayAd3Quantity;
            double diffDayAd3Price = amazonDayAd3Price - sblDayAd3Price;
            double diffDayFuel = amazonDayFuel - sblDayFuel;
            double diffDayMileage = amazonDayMileage - sblDayMileage;
            double diffDayDeduct = amazonDayDeduct - sblDayDeduct;


            // diff week
            double diffWeekFullQuantity = amazonWeekFullQuantity - sblWeekFullQuantity;
            double diffWeekFullPrice = amazonWeekFullPrice - sblWeekFullPrice;
            double diffWeekHalfQuantity = amazonWeekHalfQuantity - sblWeekHalfQuantity;
            double diffWeekHalfPrice = amazonWeekHalfPrice - sblWeekHalfPrice;
            double diffWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity - sblWeekRemoteDebriefQuantity;
            double diffWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice - sblWeekRemoteDebriefPrice;
            double diffWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity - sblWeekNurseryRoutesLevel1Quantity;
            double diffWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price - sblWeekNurseryRoutesLevel1Price;
            double diffWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity - sblWeekNurseryRoutesLevel2Quantity;
            double diffWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price - sblWeekNurseryRoutesLevel2Price;
            double diffWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity - sblWeekRescue2HoursQuantity;
            double diffWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice - sblWeekRescue2HoursPrice;
            double diffWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity - sblWeekRescue4HoursQuantity;
            double diffWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice - sblWeekRescue4HoursPrice;
            double diffWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity - sblWeekRescue6HoursQuantity;
            double diffWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice - sblWeekRescue6HoursPrice;
            double diffWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity - sblWeekReDelivery2HoursQuantity;
            double diffWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice - sblWeekReDelivery2HoursPrice;
            double diffWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity - sblWeekReDelivery4HoursQuantity;
            double diffWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice - sblWeekReDelivery4HoursPrice;
            double diffWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity - sblWeekReDelivery6HoursQuantity;
            double diffWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice - sblWeekReDelivery6HoursPrice;
            double diffWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity - sblWeekMissort2HoursQuantity;
            double diffWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice - sblWeekMissort2HoursPrice;
            double diffWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity - sblWeekMissort4HoursQuantity;
            double diffWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice - sblWeekMissort4HoursPrice;
            double diffWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity - sblWeekMissort6HoursQuantity;
            double diffWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice - sblWeekMissort6HoursPrice;
            double diffWeekSameDayQuantity = amazonWeekSameDayQuantity - sblWeekSameDayQuantity;
            double diffWeekSameDayPrice = amazonWeekSameDayPrice - sblWeekSameDayPrice;
            double diffWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity - sblWeekTrainingDayQuantity;
            double diffWeekTrainingDayPrice = amazonWeekTrainingDayPrice - sblWeekTrainingDayPrice;
            double diffWeekRideAlongQuantity = amazonWeekRideAlongQuantity - sblWeekRideAlongQuantity;
            double diffWeekRideAlongPrice = amazonWeekRideAlongPrice - sblWeekRideAlongPrice;
            double diffWeekAd1Quantity = amazonWeekAd1Quantity - sblWeekAd1Quantity;
            double diffWeekAd1Price = amazonWeekAd1Price - sblWeekAd1Price;
            double diffWeekAd2Quantity = amazonWeekAd2Quantity - sblWeekAd2Quantity;
            double diffWeekAd2Price = amazonWeekAd2Price - sblWeekAd2Price;
            double diffWeekAd3Quantity = amazonWeekAd3Quantity - sblWeekAd3Quantity;
            double diffWeekAd3Price = amazonWeekAd3Price - sblWeekAd3Price;
            double diffWeekFuel = amazonWeekFuel - sblWeekFuel;
            double diffWeekMileage = amazonWeekMileage - sblWeekMileage;
            double diffWeekDeduct = amazonWeekDeduct - sblWeekDeduct;
            #endregion

            #region calculate totals
            // calculate totals

            double amazonWeekTotalQuantity =
                +amazonWeekFullQuantity
                + amazonWeekHalfQuantity
                + amazonWeekRemoteDebriefQuantity
                + amazonWeekNurseryRoutesLevel1Quantity
                + amazonWeekNurseryRoutesLevel2Quantity
                + amazonWeekRescue2HoursQuantity
                + amazonWeekRescue4HoursQuantity
                + amazonWeekRescue6HoursQuantity
                + amazonWeekReDelivery2HoursQuantity
                + amazonWeekReDelivery4HoursQuantity
                + amazonWeekReDelivery6HoursQuantity
                + amazonWeekMissort2HoursQuantity
                + amazonWeekMissort4HoursQuantity
                + amazonWeekMissort6HoursQuantity
                + amazonWeekSameDayQuantity
                + amazonWeekTrainingDayQuantity
                + amazonWeekRideAlongQuantity
                + amazonWeekAd1Quantity
                + amazonWeekAd2Quantity
                + amazonWeekAd3Quantity;

            double amazonWeekTotalPrice =
                +amazonWeekFullPrice
                + amazonWeekHalfPrice
                + amazonWeekRemoteDebriefPrice
                + amazonWeekNurseryRoutesLevel1Price
                + amazonWeekNurseryRoutesLevel2Price
                + amazonWeekRescue2HoursPrice
                + amazonWeekRescue4HoursPrice
                + amazonWeekRescue6HoursPrice
                + amazonWeekReDelivery2HoursPrice
                + amazonWeekReDelivery4HoursPrice
                + amazonWeekReDelivery6HoursPrice
                + amazonWeekMissort2HoursPrice
                + amazonWeekMissort4HoursPrice
                + amazonWeekMissort6HoursPrice
                + amazonWeekSameDayPrice
                + amazonWeekTrainingDayPrice
                + amazonWeekRideAlongPrice
                + amazonWeekAd1Price
                + amazonWeekAd2Price
                + amazonWeekAd3Price;

            double sblWeekTotalQuantity =
                +sblWeekFullQuantity
                + sblWeekHalfQuantity
                + sblWeekRemoteDebriefQuantity
                + sblWeekNurseryRoutesLevel1Quantity
                + sblWeekNurseryRoutesLevel2Quantity
                + sblWeekRescue2HoursQuantity
                + sblWeekRescue4HoursQuantity
                + sblWeekRescue6HoursQuantity
                + sblWeekReDelivery2HoursQuantity
                + sblWeekReDelivery4HoursQuantity
                + sblWeekReDelivery6HoursQuantity
                + sblWeekMissort2HoursQuantity
                + sblWeekMissort4HoursQuantity
                + sblWeekMissort6HoursQuantity
                + sblWeekSameDayQuantity
                + sblWeekTrainingDayQuantity
                + sblWeekRideAlongQuantity
                + sblWeekAd1Quantity
                + sblWeekAd2Quantity
                + sblWeekAd3Quantity;

            double sblWeekTotalPrice =
                +sblWeekFullPrice
                + sblWeekHalfPrice
                + sblWeekRemoteDebriefPrice
                + sblWeekNurseryRoutesLevel1Price
                + sblWeekNurseryRoutesLevel2Price
                + sblWeekRescue2HoursPrice
                + sblWeekRescue4HoursPrice
                + sblWeekRescue6HoursPrice
                + sblWeekReDelivery2HoursPrice
                + sblWeekReDelivery4HoursPrice
                + sblWeekReDelivery6HoursPrice
                + sblWeekMissort2HoursPrice
                + sblWeekMissort4HoursPrice
                + sblWeekMissort6HoursPrice
                + sblWeekSameDayPrice
                + sblWeekTrainingDayPrice
                + sblWeekRideAlongPrice
                + sblWeekAd1Price
                + sblWeekAd2Price
                + sblWeekAd3Price;


            double diffWeekTotalQuantity =
                +diffWeekFullQuantity
                + diffWeekHalfQuantity
                + diffWeekRemoteDebriefQuantity
                + diffWeekNurseryRoutesLevel1Quantity
                + diffWeekNurseryRoutesLevel2Quantity
                + diffWeekRescue2HoursQuantity
                + diffWeekRescue4HoursQuantity
                + diffWeekRescue6HoursQuantity
                + diffWeekReDelivery2HoursQuantity
                + diffWeekReDelivery4HoursQuantity
                + diffWeekReDelivery6HoursQuantity
                + diffWeekMissort2HoursQuantity
                + diffWeekMissort4HoursQuantity
                + diffWeekMissort6HoursQuantity
                + diffWeekSameDayQuantity
                + diffWeekTrainingDayQuantity
                + diffWeekRideAlongQuantity
                + diffWeekAd1Quantity
                + diffWeekAd2Quantity
                + diffWeekAd3Quantity;

            double diffWeekTotalPrice =
                +diffWeekFullPrice
                + diffWeekHalfPrice
                + diffWeekRemoteDebriefPrice
                + diffWeekNurseryRoutesLevel1Price
                + diffWeekNurseryRoutesLevel2Price
                + diffWeekRescue2HoursPrice
                + diffWeekRescue4HoursPrice
                + diffWeekRescue6HoursPrice
                + diffWeekReDelivery2HoursPrice
                + diffWeekReDelivery4HoursPrice
                + diffWeekReDelivery6HoursPrice
                + diffWeekMissort2HoursPrice
                + diffWeekMissort4HoursPrice
                + diffWeekMissort6HoursPrice
                + diffWeekSameDayPrice
                + diffWeekTrainingDayPrice
                + diffWeekRideAlongPrice
                + diffWeekAd1Price
                + diffWeekAd2Price
                + diffWeekAd3Price;
            #endregion

            #region Route Allocation View Model
            var viewModel = new RouteAllocationViewModel
            {
                RouteDate = dateRoute,
                ThisWeek = dateWeek,
                ThisDay = DateTime.Now.Date,
                //
                Routes = routes,
                //
                RouteAmazonId = RouteAmazonId,
                //
                AmazonDayFullQuantity = amazonDayFullQuantity,
                AmazonDayFullPrice = amazonDayFullPrice,
                AmazonDayHalfQuantity = amazonDayHalfQuantity,
                AmazonDayHalfPrice = amazonDayHalfPrice,
                AmazonDayRemoteDebriefQuantity = amazonDayRemoteDebriefQuantity,
                AmazonDayRemoteDebriefPrice = amazonDayRemoteDebriefPrice,
                AmazonDayNurseryRoutesLevel1Quantity = amazonDayNurseryRoutesLevel1Quantity,
                AmazonDayNurseryRoutesLevel1Price = amazonDayNurseryRoutesLevel1Price,
                AmazonDayNurseryRoutesLevel2Quantity = amazonDayNurseryRoutesLevel2Quantity,
                AmazonDayNurseryRoutesLevel2Price = amazonDayNurseryRoutesLevel2Price,
                AmazonDayRescue2HoursQuantity = amazonDayRescue2HoursQuantity,
                AmazonDayRescue2HoursPrice = amazonDayRescue2HoursPrice,
                AmazonDayRescue4HoursQuantity = amazonDayRescue4HoursQuantity,
                AmazonDayRescue4HoursPrice = amazonDayRescue4HoursPrice,
                AmazonDayRescue6HoursQuantity = amazonDayRescue6HoursQuantity,
                AmazonDayRescue6HoursPrice = amazonDayRescue6HoursPrice,
                AmazonDayReDelivery2HoursQuantity = amazonDayReDelivery2HoursQuantity,
                AmazonDayReDelivery2HoursPrice = amazonDayReDelivery2HoursPrice,
                AmazonDayReDelivery4HoursQuantity = amazonDayReDelivery4HoursQuantity,
                AmazonDayReDelivery4HoursPrice = amazonDayReDelivery4HoursPrice,
                AmazonDayReDelivery6HoursQuantity = amazonDayReDelivery6HoursQuantity,
                AmazonDayReDelivery6HoursPrice = amazonDayReDelivery6HoursPrice,
                AmazonDayMissort2HoursQuantity = amazonDayMissort2HoursQuantity,
                AmazonDayMissort2HoursPrice = amazonDayMissort2HoursPrice,
                AmazonDayMissort4HoursQuantity = amazonDayMissort4HoursQuantity,
                AmazonDayMissort4HoursPrice = amazonDayMissort4HoursPrice,
                AmazonDayMissort6HoursQuantity = amazonDayMissort6HoursQuantity,
                AmazonDayMissort6HoursPrice = amazonDayMissort6HoursPrice,
                AmazonDaySameDayQuantity = amazonDaySameDayQuantity,
                AmazonDaySameDayPrice = amazonDaySameDayPrice,
                AmazonDayTrainingDayQuantity = amazonDayTrainingDayQuantity,
                AmazonDayTrainingDayPrice = amazonDayTrainingDayPrice,
                AmazonDayRideAlongQuantity = amazonDayRideAlongQuantity,
                AmazonDayRideAlongPrice = amazonDayRideAlongPrice,
                AmazonDayAd1Quantity = amazonDayAd1Quantity,
                AmazonDayAd1Price = amazonDayAd1Price,
                AmazonDayAd2Quantity = amazonDayAd2Quantity,
                AmazonDayAd2Price = amazonDayAd2Price,
                AmazonDayAd3Quantity = amazonDayAd3Quantity,
                AmazonDayAd3Price = amazonDayAd3Price,
                AmazonDayFuel = amazonDayFuel,
                AmazonDayMileage = amazonDayMileage,
                AmazonDayDeduct = amazonDayDeduct,
                //
                AmazonWeekFullQuantity = amazonWeekFullQuantity,
                AmazonWeekFullPrice = amazonWeekFullPrice,
                AmazonWeekHalfQuantity = amazonWeekHalfQuantity,
                AmazonWeekHalfPrice = amazonWeekHalfPrice,
                AmazonWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity,
                AmazonWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice,
                AmazonWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity,
                AmazonWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price,
                AmazonWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity,
                AmazonWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price,
                AmazonWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity,
                AmazonWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice,
                AmazonWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity,
                AmazonWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice,
                AmazonWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity,
                AmazonWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice,
                AmazonWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity,
                AmazonWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice,
                AmazonWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity,
                AmazonWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice,
                AmazonWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity,
                AmazonWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice,
                AmazonWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity,
                AmazonWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice,
                AmazonWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity,
                AmazonWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice,
                AmazonWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity,
                AmazonWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice,
                AmazonWeekSameDayQuantity = amazonWeekSameDayQuantity,
                AmazonWeekSameDayPrice = amazonWeekSameDayPrice,
                AmazonWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity,
                AmazonWeekTrainingDayPrice = amazonWeekTrainingDayPrice,
                AmazonWeekRideAlongQuantity = amazonWeekRideAlongQuantity,
                AmazonWeekRideAlongPrice = amazonWeekRideAlongPrice,
                AmazonWeekAd1Quantity = amazonWeekAd1Quantity,
                AmazonWeekAd1Price = amazonWeekAd1Price,
                AmazonWeekAd2Quantity = amazonWeekAd2Quantity,
                AmazonWeekAd2Price = amazonWeekAd2Price,
                AmazonWeekAd3Quantity = amazonWeekAd3Quantity,
                AmazonWeekAd3Price = amazonWeekAd3Price,
                AmazonWeekFuel = amazonWeekFuel,
                AmazonWeekMileage = amazonWeekMileage,
                AmazonWeekDeduct = amazonWeekDeduct,
                //
                SblDayFullQuantity = sblDayFullQuantity,
                SblDayFullPrice = sblDayFullPrice,
                SblDayHalfQuantity = sblDayHalfQuantity,
                SblDayHalfPrice = sblDayHalfPrice,
                SblDayRemoteDebriefQuantity = sblDayRemoteDebriefQuantity,
                SblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice,
                SblDayNurseryRoutesLevel1Quantity = sblDayNurseryRoutesLevel1Quantity,
                SblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price,
                SblDayNurseryRoutesLevel2Quantity = sblDayNurseryRoutesLevel2Quantity,
                SblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price,
                SblDayRescue2HoursQuantity = sblDayRescue2HoursQuantity,
                SblDayRescue2HoursPrice = sblDayRescue2HoursPrice,
                SblDayRescue4HoursQuantity = sblDayRescue4HoursQuantity,
                SblDayRescue4HoursPrice = sblDayRescue4HoursPrice,
                SblDayRescue6HoursQuantity = sblDayRescue6HoursQuantity,
                SblDayRescue6HoursPrice = sblDayRescue6HoursPrice,
                SblDayReDelivery2HoursQuantity = sblDayReDelivery2HoursQuantity,
                SblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice,
                SblDayReDelivery4HoursQuantity = sblDayReDelivery4HoursQuantity,
                SblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice,
                SblDayReDelivery6HoursQuantity = sblDayReDelivery6HoursQuantity,
                SblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice,
                SblDayMissort2HoursQuantity = sblDayMissort2HoursQuantity,
                SblDayMissort2HoursPrice = sblDayMissort2HoursPrice,
                SblDayMissort4HoursQuantity = sblDayMissort4HoursQuantity,
                SblDayMissort4HoursPrice = sblDayMissort4HoursPrice,
                SblDayMissort6HoursQuantity = sblDayMissort6HoursQuantity,
                SblDayMissort6HoursPrice = sblDayMissort6HoursPrice,
                SblDaySameDayQuantity = sblDaySameDayQuantity,
                SblDaySameDayPrice = sblDaySameDayPrice,
                SblDayTrainingDayQuantity = sblDayTrainingDayQuantity,
                SblDayTrainingDayPrice = sblDayTrainingDayPrice,
                SblDayRideAlongQuantity = sblDayRideAlongQuantity,
                SblDayRideAlongPrice = sblDayRideAlongPrice,
                SblDayAd1Quantity = sblDayAd1Quantity,
                SblDayAd1Price = sblDayAd1Price,
                SblDayAd2Quantity = sblDayAd2Quantity,
                SblDayAd2Price = sblDayAd2Price,
                SblDayAd3Quantity = sblDayAd3Quantity,
                SblDayAd3Price = sblDayAd3Price,
                SblDayFuel = sblDayFuel,
                SblDayMileage = sblDayMileage,
                SblDayDeduct = sblDayDeduct,
                //
                SblWeekFullQuantity = sblWeekFullQuantity,
                SblWeekFullPrice = sblWeekFullPrice,
                SblWeekHalfQuantity = sblWeekHalfQuantity,
                SblWeekHalfPrice = sblWeekHalfPrice,
                SblWeekRemoteDebriefQuantity = sblWeekRemoteDebriefQuantity,
                SblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice,
                SblWeekNurseryRoutesLevel1Quantity = sblWeekNurseryRoutesLevel1Quantity,
                SblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price,
                SblWeekNurseryRoutesLevel2Quantity = sblWeekNurseryRoutesLevel2Quantity,
                SblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price,
                SblWeekRescue2HoursQuantity = sblWeekRescue2HoursQuantity,
                SblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice,
                SblWeekRescue4HoursQuantity = sblWeekRescue4HoursQuantity,
                SblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice,
                SblWeekRescue6HoursQuantity = sblWeekRescue6HoursQuantity,
                SblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice,
                SblWeekReDelivery2HoursQuantity = sblWeekReDelivery2HoursQuantity,
                SblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice,
                SblWeekReDelivery4HoursQuantity = sblWeekReDelivery4HoursQuantity,
                SblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice,
                SblWeekReDelivery6HoursQuantity = sblWeekReDelivery6HoursQuantity,
                SblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice,
                SblWeekMissort2HoursQuantity = sblWeekMissort2HoursQuantity,
                SblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice,
                SblWeekMissort4HoursQuantity = sblWeekMissort4HoursQuantity,
                SblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice,
                SblWeekMissort6HoursQuantity = sblWeekMissort6HoursQuantity,
                SblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice,
                SblWeekSameDayQuantity = sblWeekSameDayQuantity,
                SblWeekSameDayPrice = sblWeekSameDayPrice,
                SblWeekTrainingDayQuantity = sblWeekTrainingDayQuantity,
                SblWeekTrainingDayPrice = sblWeekTrainingDayPrice,
                SblWeekRideAlongQuantity = sblWeekRideAlongQuantity,
                SblWeekRideAlongPrice = sblWeekRideAlongPrice,
                SblWeekAd1Quantity = sblWeekAd1Quantity,
                SblWeekAd1Price = sblWeekAd1Price,
                SblWeekAd2Quantity = sblWeekAd2Quantity,
                SblWeekAd2Price = sblWeekAd2Price,
                SblWeekAd3Quantity = sblWeekAd3Quantity,
                SblWeekAd3Price = sblWeekAd3Price,
                SblWeekFuel = sblWeekFuel,
                SblWeekMileage = sblWeekMileage,
                SblWeekDeduct = sblWeekDeduct,
                //
                DiffDayFullQuantity = diffDayFullQuantity,
                DiffDayFullPrice = diffDayFullPrice,
                DiffDayHalfQuantity = diffDayHalfQuantity,
                DiffDayHalfPrice = diffDayHalfPrice,
                DiffDayRemoteDebriefQuantity = diffDayRemoteDebriefQuantity,
                DiffDayRemoteDebriefPrice = diffDayRemoteDebriefPrice,
                DiffDayNurseryRoutesLevel1Quantity = diffDayNurseryRoutesLevel1Quantity,
                DiffDayNurseryRoutesLevel1Price = diffDayNurseryRoutesLevel1Price,
                DiffDayNurseryRoutesLevel2Quantity = diffDayNurseryRoutesLevel2Quantity,
                DiffDayNurseryRoutesLevel2Price = diffDayNurseryRoutesLevel2Price,
                DiffDayRescue2HoursQuantity = diffDayRescue2HoursQuantity,
                DiffDayRescue2HoursPrice = diffDayRescue2HoursPrice,
                DiffDayRescue4HoursQuantity = diffDayRescue4HoursQuantity,
                DiffDayRescue4HoursPrice = diffDayRescue4HoursPrice,
                DiffDayRescue6HoursQuantity = diffDayRescue6HoursQuantity,
                DiffDayRescue6HoursPrice = diffDayRescue6HoursPrice,
                DiffDayReDelivery2HoursQuantity = diffDayReDelivery2HoursQuantity,
                DiffDayReDelivery2HoursPrice = diffDayReDelivery2HoursPrice,
                DiffDayReDelivery4HoursQuantity = diffDayReDelivery4HoursQuantity,
                DiffDayReDelivery4HoursPrice = diffDayReDelivery4HoursPrice,
                DiffDayReDelivery6HoursQuantity = diffDayReDelivery6HoursQuantity,
                DiffDayReDelivery6HoursPrice = diffDayReDelivery6HoursPrice,
                DiffDayMissort2HoursQuantity = diffDayMissort2HoursQuantity,
                DiffDayMissort2HoursPrice = diffDayMissort2HoursPrice,
                DiffDayMissort4HoursQuantity = diffDayMissort4HoursQuantity,
                DiffDayMissort4HoursPrice = diffDayMissort4HoursPrice,
                DiffDayMissort6HoursQuantity = diffDayMissort6HoursQuantity,
                DiffDayMissort6HoursPrice = diffDayMissort6HoursPrice,
                DiffDaySameDayQuantity = diffDaySameDayQuantity,
                DiffDaySameDayPrice = diffDaySameDayPrice,
                DiffDayTrainingDayQuantity = diffDayTrainingDayQuantity,
                DiffDayTrainingDayPrice = diffDayTrainingDayPrice,
                DiffDayRideAlongQuantity = diffDayRideAlongQuantity,
                DiffDayRideAlongPrice = diffDayRideAlongPrice,
                DiffDayAd1Quantity = diffDayAd1Quantity,
                DiffDayAd1Price = diffDayAd1Price,
                DiffDayAd2Quantity = diffDayAd2Quantity,
                DiffDayAd2Price = diffDayAd2Price,
                DiffDayAd3Quantity = diffDayAd3Quantity,
                DiffDayAd3Price = diffDayAd3Price,
                DiffDayFuel = diffDayFuel,
                DiffDayMileage = diffDayMileage,
                DiffDayDeduct = diffDayDeduct,
                //
                DiffWeekFullQuantity = diffWeekFullQuantity,
                DiffWeekFullPrice = diffWeekFullPrice,
                DiffWeekHalfQuantity = diffWeekHalfQuantity,
                DiffWeekHalfPrice = diffWeekHalfPrice,
                DiffWeekRemoteDebriefQuantity = diffWeekRemoteDebriefQuantity,
                DiffWeekRemoteDebriefPrice = diffWeekRemoteDebriefPrice,
                DiffWeekNurseryRoutesLevel1Quantity = diffWeekNurseryRoutesLevel1Quantity,
                DiffWeekNurseryRoutesLevel1Price = diffWeekNurseryRoutesLevel1Price,
                DiffWeekNurseryRoutesLevel2Quantity = diffWeekNurseryRoutesLevel2Quantity,
                DiffWeekNurseryRoutesLevel2Price = diffWeekNurseryRoutesLevel2Price,
                DiffWeekRescue2HoursQuantity = diffWeekRescue2HoursQuantity,
                DiffWeekRescue2HoursPrice = diffWeekRescue2HoursPrice,
                DiffWeekRescue4HoursQuantity = diffWeekRescue4HoursQuantity,
                DiffWeekRescue4HoursPrice = diffWeekRescue4HoursPrice,
                DiffWeekRescue6HoursQuantity = diffWeekRescue6HoursQuantity,
                DiffWeekRescue6HoursPrice = diffWeekRescue6HoursPrice,
                DiffWeekReDelivery2HoursQuantity = diffWeekReDelivery2HoursQuantity,
                DiffWeekReDelivery2HoursPrice = diffWeekReDelivery2HoursPrice,
                DiffWeekReDelivery4HoursQuantity = diffWeekReDelivery4HoursQuantity,
                DiffWeekReDelivery4HoursPrice = diffWeekReDelivery4HoursPrice,
                DiffWeekReDelivery6HoursQuantity = diffWeekReDelivery6HoursQuantity,
                DiffWeekReDelivery6HoursPrice = diffWeekReDelivery6HoursPrice,
                DiffWeekMissort2HoursQuantity = diffWeekMissort2HoursQuantity,
                DiffWeekMissort2HoursPrice = diffWeekMissort2HoursPrice,
                DiffWeekMissort4HoursQuantity = diffWeekMissort4HoursQuantity,
                DiffWeekMissort4HoursPrice = diffWeekMissort4HoursPrice,
                DiffWeekMissort6HoursQuantity = diffWeekMissort6HoursQuantity,
                DiffWeekMissort6HoursPrice = diffWeekMissort6HoursPrice,
                DiffWeekSameDayQuantity = diffWeekSameDayQuantity,
                DiffWeekSameDayPrice = diffWeekSameDayPrice,
                DiffWeekTrainingDayQuantity = diffWeekTrainingDayQuantity,
                DiffWeekTrainingDayPrice = diffWeekTrainingDayPrice,
                DiffWeekRideAlongQuantity = diffWeekRideAlongQuantity,
                DiffWeekRideAlongPrice = diffWeekRideAlongPrice,
                DiffWeekAd1Quantity = diffWeekAd1Quantity,
                DiffWeekAd1Price = diffWeekAd1Price,
                DiffWeekAd2Quantity = diffWeekAd2Quantity,
                DiffWeekAd2Price = diffWeekAd2Price,
                DiffWeekAd3Quantity = diffWeekAd3Quantity,
                DiffWeekAd3Price = diffWeekAd3Price,
                DiffWeekFuel = diffWeekFuel,
                DiffWeekMileage = diffWeekMileage,
                DiffWeekDeduct = diffWeekDeduct,
                //
                AmazonWeekTotalQuantity = amazonWeekTotalQuantity,
                AmazonWeekTotalPrice = amazonWeekTotalPrice,
                SblWeekTotalQuantity = sblWeekTotalQuantity,
                SblWeekTotalPrice = sblWeekTotalPrice,
                DiffWeekTotalQuantity = diffWeekTotalQuantity,
                DiffWeekTotalPrice = diffWeekTotalPrice,
                HasWritePermision = hasWritePermision
                //
            };
            #endregion

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }

        */





        public JsonResult GetRouteDrivers()
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

            var drivers = (from x in db.Associates
                           where x.ApplicationStatus == "Approved" && x.AssociateStatus == "Active" && x.Active == true && x.Deleted == false
                           orderby x.Name
                           select new
                           {
                               Id = x.Id,
                               Name = x.Name,
                               DepotId = x.DepotId
                           }).ToList();

            return Json(new
            {
                drivers = drivers
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddRoute(int AssociateId, int? DepotId, DateTime? RouteDate, string RouteCode1, string RouteType1, double Ad1, double Ad2, double Ad3, double Fuel, double Deduct, double Mileage, string Notes)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);


            var prices = db.Settings.FirstOrDefault();
            var associate = db.Associates.FirstOrDefault(x => x.Id == AssociateId);

            RouteAllocation route = new RouteAllocation();
            route.AssociateId = AssociateId;
            route.RouteDate = RouteDate;
            route.RouteCode1 = RouteCode1;

            int? currentUserDepotId = null;
            if (User.IsInRole(WebConstant.SBLUserRole.Master) || User.IsInRole(WebConstant.SBLUserRole.Admin))
            {
                if (DepotId.HasValue)
                {
                    currentUserDepotId = DepotId;
                }
            }
            else
            {
                if (currentuser != null)
                {
                    if (currentuser.DepotId.HasValue)
                    {
                        currentUserDepotId = currentuser.DepotId.Value;
                    }
                    else
                    {
                        if (associate != null && associate.DepotId.HasValue)
                        {
                            currentUserDepotId = associate.DepotId.Value;
                        }
                    }
                }

            }
            if (prices != null)
            {
                if (RouteType1 == "Full")
                {
                    route.RoutePrice1 = prices.FullRouteSBL;
                }

                if (RouteType1 == "Half")
                {
                    route.RoutePrice1 = prices.HalfRouteSBL;
                }

                if (RouteType1 == "RemoteDebrief")
                {
                    route.RoutePrice1 = prices.RemoteDebriefSBL;
                }

                if (RouteType1 == "NurseryRoutesLevel1")
                {
                    route.RoutePrice1 = prices.NurseryRoutesLevel1SBL;
                }

                if (RouteType1 == "NurseryRoutesLevel2")
                {
                    route.RoutePrice1 = prices.NurseryRoutesLevel2SBL;
                }

                if (RouteType1 == "Rescue2Hours")
                {
                    route.RoutePrice1 = prices.Rescue2HoursSBL;
                }

                if (RouteType1 == "Rescue4Hours")
                {
                    route.RoutePrice1 = prices.Rescue4HoursSBL;
                }

                if (RouteType1 == "Rescue6Hours")
                {
                    route.RoutePrice1 = prices.Rescue6HoursSBL;
                }

                if (RouteType1 == "ReDelivery2Hours")
                {
                    route.RoutePrice1 = prices.ReDelivery2HoursSBL;
                }

                if (RouteType1 == "ReDelivery4Hours")
                {
                    route.RoutePrice1 = prices.ReDelivery4HoursSBL;
                }

                if (RouteType1 == "ReDelivery6Hours")
                {
                    route.RoutePrice1 = prices.ReDelivery6HoursSBL;
                }

                if (RouteType1 == "Missort2HoursSBL")
                {
                    route.RoutePrice1 = prices.Missort2HoursSBL;
                }

                if (RouteType1 == "Missort4HoursSBL")
                {
                    route.RoutePrice1 = prices.Missort4HoursSBL;
                }

                if (RouteType1 == "Missort6HoursSBL")
                {
                    route.RoutePrice1 = prices.Missort6HoursSBL;
                }

                if (RouteType1 == "SameDay")
                {
                    route.RoutePrice1 = prices.SamedaySBL;
                }

                if (RouteType1 == "TrainingDay")
                {
                    route.RoutePrice1 = prices.TrainingDaySBL;
                }

                if (RouteType1 == "RideAlong")
                {
                    route.RoutePrice1 = prices.RideAlongSBL;
                }
            }
            route.RouteType1 = RouteType1;


            double result1;
            route.Ad1Quantity = double.TryParse(Ad1.ToString(), out result1) ? Ad1 : 0;
            route.Ad1Price = prices.Ad1Sbl;

            double result2;
            route.Ad2Quantity = double.TryParse(Ad2.ToString(), out result2) ? Ad2 : 0;
            route.Ad2Price = prices.Ad2Sbl;

            double result3;
            route.Ad3Quantity = double.TryParse(Ad3.ToString(), out result3) ? Ad3 : 0;
            route.Ad3Price = prices.Ad3Sbl;

            double result4;
            route.Fuel = double.TryParse(Fuel.ToString(), out result4) ? Fuel : 0;

            double result5;
            route.Deduct = double.TryParse(Deduct.ToString(), out result5) ? Deduct : 0;

            double result6;
            route.Mileage = double.TryParse(Mileage.ToString(), out result6) ? Mileage : 0;

            route.Notes = Notes;

            string depotname = "-";
            if (currentUserDepotId > 0)
            {
                depotname = db.Depots.FirstOrDefault(x => x.Id == currentUserDepotId.Value).Name;
            }
            else
            {
                if (associate.DepotId.HasValue)
                {
                    depotname = associate.Depot.Name;
                }
            }

            route.DepotId = currentUserDepotId;
            route.Status = "Pending";
            route.DateCreated = DateTime.Now;
            route.Active = true;
            route.Deleted = false;
            db.RouteAllocations.Add(route);
            db.SaveChanges();

            var getroute = db.RouteAllocations.FirstOrDefault(x => x.Id == route.Id);

            return Json(new
            {
                Id = getroute.Id,
                DriverName = getroute.Associate.Name,
                DepotName = depotname,
                RouteCode = getroute.RouteCode1,
                RouteType1 = getroute.RouteType1,
                DepotId = getroute.DepotId,
                Ad1 = getroute.Ad1Quantity,
                Ad2 = getroute.Ad2Quantity,
                Ad3 = getroute.Ad3Quantity,
                Fuel = getroute.Fuel,
                Deduct = getroute.Deduct,
                Mileage = getroute.Mileage,
                Notes = getroute.Notes
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRoute(int routeAllocationId, double ad1, double ad2, double ad3, double fuel, double deduct, double mileage, string notes, string routeCode1, string routeType1, int depotId)
        {
            // current user
            var route = db.RouteAllocations.FirstOrDefault(x => x.Id == routeAllocationId);
            if (route != null)
            {
                double ad1Qty;
                route.Ad1Quantity = double.TryParse(ad1.ToString(CultureInfo.InvariantCulture), out ad1Qty) ? ad1 : 0;

                double ad2Qty;
                route.Ad2Quantity = double.TryParse(ad2.ToString(CultureInfo.InvariantCulture), out ad2Qty) ? ad2 : 0;

                double ad3Qty;
                route.Ad3Quantity = double.TryParse(ad3.ToString(CultureInfo.InvariantCulture), out ad3Qty) ? ad3 : 0;

                double fuelQty;
                route.Fuel = double.TryParse(fuel.ToString(CultureInfo.InvariantCulture), out fuelQty) ? fuel : 0;

                double deductAmt;
                route.Deduct = double.TryParse(deduct.ToString(CultureInfo.InvariantCulture), out deductAmt) ? deduct : 0;

                double vmileage;
                route.Mileage = double.TryParse(mileage.ToString(CultureInfo.InvariantCulture), out vmileage) ? mileage : 0;

                route.RouteCode1 = routeCode1;
                route.RouteType1 = routeType1;
                if (User.IsInRole(WebConstant.SBLUserRole.Master) || User.IsInRole(WebConstant.SBLUserRole.Admin))
                {
                    if (depotId > 0)
                    {
                        route.DepotId = depotId;
                    }
                }
                route.Notes = notes;
                db.SaveChanges();
            }
            var getroute = db.RouteAllocations.FirstOrDefault(x => x.Id == route.Id);

            return Json(new
            {
                Id = getroute.Id,
                DriverName = getroute.Associate.Name,
                DepotName = getroute.Depot.Name,
                RouteCode1 = getroute.RouteCode1,
                RouteType1 = getroute.RouteType1,
                Ad1 = getroute.Ad1Quantity,
                Ad2 = getroute.Ad2Quantity,
                Ad3 = getroute.Ad3Quantity,
                Fuel = getroute.Fuel,
                Deduct = getroute.Deduct,
                Mileage = getroute.Mileage,
                Notes = getroute.Notes
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveRoute(int RouteId)
        {
            var route = db.RouteAllocations.FirstOrDefault(x => x.Id == RouteId);
            route.Deleted = true;
            db.SaveChanges();
            return Json(new
            {
                Id = route.Id
            }, JsonRequestBehavior.AllowGet);
        }
        //
        // schedule
        //



        //
        // accident
        //
        public ActionResult AccidentList()
        {
            return View(db.Accidents.Where(x => x.Active == true && x.Deleted == false).ToList());
        }


        public ActionResult AccidentCreate()
        {
            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Vehicles = vehicles;

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            return View();
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult AccidentCreate(Accident accident)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    accident.DateCreated = DateTime.Now;
                    accident.Active = true;
                    accident.Deleted = false;
                    db.Accidents.Add(accident);
                    db.SaveChanges();

                    // files
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase fileUpload = Request.Files.Get(i);

                        if (fileUpload.ContentLength > 0)
                        {
                            Int32 length = fileUpload.ContentLength;
                            byte[] temp = new byte[length];
                            fileUpload.InputStream.Read(temp, 0, length);

                            AccidentFile accidentfile = new AccidentFile();
                            accidentfile.AccidentId = accident.Id;
                            accidentfile.DataFile = temp;
                            accidentfile.DataFileContentType = fileUpload.ContentType;
                            accidentfile.DataFileName = fileUpload.FileName;
                            accidentfile.DateCreated = DateTime.Now;
                            accidentfile.Active = true;
                            accidentfile.Deleted = false;
                            db.AccidentFiles.Add(accidentfile);
                            db.SaveChanges();
                        }
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: AccidentCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("accidentlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: AccidentCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Vehicles = vehicles;

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            return View();
        }

        public ActionResult AccidentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Accident accident = db.Accidents.Find(id);

            if (accident == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = x.Id == accident.VehicleId,
            });

            ViewBag.Vehicles = vehicles;

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == accident.AssociateId,
            });

            ViewBag.Associates = associates;

            return View(accident);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult AccidentEdit(Accident accident)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Accidents.Where(x => x.Id == accident.Id).FirstOrDefault();

                    original.VehicleId = accident.VehicleId;
                    original.AssociateId = accident.AssociateId;
                    original.Date = accident.Date;
                    original.Status = accident.Status;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // files
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase fileUpload = Request.Files.Get(i);

                        if (fileUpload.ContentLength > 0)
                        {
                            Int32 length = fileUpload.ContentLength;
                            byte[] temp = new byte[length];
                            fileUpload.InputStream.Read(temp, 0, length);

                            AccidentFile accidentfile = new AccidentFile();
                            accidentfile.AccidentId = accident.Id;
                            accidentfile.DataFile = temp;
                            accidentfile.DataFileContentType = fileUpload.ContentType;
                            accidentfile.DataFileName = fileUpload.FileName;
                            accidentfile.DateCreated = DateTime.Now;
                            accidentfile.Active = true;
                            accidentfile.Deleted = false;
                            db.AccidentFiles.Add(accidentfile);
                            db.SaveChanges();
                        }
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: AccidentEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("accidentlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: AccidentEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> vehicles = db.Vehicles.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Registration).ToList().Select(x => new SelectListItem()
            {
                Text = x.Make + " - " + x.Model + " - " + x.Registration,
                Value = x.Id.ToString(),
                Selected = x.Id == accident.VehicleId,
            });

            ViewBag.Vehicles = vehicles;

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == accident.AssociateId,
            });

            ViewBag.Associates = associates;

            return View(accident);
        }

        public JsonResult AccidentFileRemove(int? AccidentFileId)
        {
            var accidentfile = db.AccidentFiles.Where(x => x.Id == AccidentFileId).FirstOrDefault();
            accidentfile.Deleted = true;
            db.SaveChanges();

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult AccidentFileDownload(int id)
        {
            var accidentfile = db.AccidentFiles.Where(x => x.Id == id).FirstOrDefault();

            return File(accidentfile.DataFile, accidentfile.DataFileContentType, accidentfile.DataFileName);
        }
        //
        // induction
        //
        public ActionResult InductionList()
        {
            return View(db.Inductions.Where(x => x.Active == true && x.Deleted == false).ToList());
        }

        public ActionResult InductionView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Induction induction = db.Inductions.Find(id);

            if (induction == null)
            {
                return HttpNotFound();
            }

            return View(induction);
        }

        public ActionResult InductionEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Induction induction = db.Inductions.Find(id);

            if (induction == null)
            {
                return HttpNotFound();
            }

            return View(induction);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult InductionEdit(Induction induction)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    var original = db.Inductions.FirstOrDefault(x => x.Id == induction.Id);

                    original.FullName = induction.FullName;
                    original.MobileNumber = induction.MobileNumber;
                    original.Email = induction.Email;
                    original.Status = induction.Status;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // save to associates
                    if (!db.Associates.Where(x => x.InductionId == original.Id && x.Active == true && x.Deleted == false).Any())
                    {
                        if (induction.Status == "Approved")
                        {
                            Associate associate = new Associate();
                            associate.InductionId = induction.Id;
                            //
                            associate.Name = original.FullName;
                            associate.Email = original.Email;
                            associate.Position = "Associate";
                            associate.Address = original.Address;
                            associate.City = original.City;
                            associate.Postcode = original.Postcode;
                            associate.Mobile = original.MobileNumber;

                            associate.DateOfBirth = original.DateOfBirth;
                            associate.Nationality = original.Nationality;
                            associate.NationalInsuranceNumber = original.NationalInsuranceNumber;
                            associate.UTRNumber = original.UTRNumber;

                            associate.NextOfKinName = original.NextOfKinName;
                            associate.NextOfKinRelationship = original.NextOfKinRelationship;
                            associate.NextOfKinMobile = original.NextOfKinMobile;

                            associate.NameOfTheBank = original.NameOfTheBank;
                            associate.SortCode = original.SortCode;
                            associate.AccountNumber = original.AccountNumber;
                            associate.AccountName = original.AccountName;

                            //associate.Bio = "";
                            associate.ApplicationStatus = "Approved";
                            associate.AssociateStatus = "Inactive";
                            associate.IsEmployed = false;
                            associate.IsThirdParty = false;
                            associate.OwnVehicle = false;
                            //associate.StartDate = null;
                            associate.InductionCompleted = true;
                            //associate.InductionCompletedDate = null;
                            associate.FirstRideAlong = false;
                            //associate.FirstRideAlongDate = null;
                            associate.SecondRideAlong = false;
                            //associate.SecondRideAlongDate = null;
                            associate.Active = true;
                            associate.DateCreated = DateTime.Now;
                            associate.Deleted = false;
                            //
                            db.Associates.Add(associate);
                            db.SaveChanges();
                        }
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: InductionEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("inductionlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: InductionEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(induction);
        }

        public ActionResult InductionCode()
        {
            if (!db.Settings.Any())
            {
                Setting setting = new Setting();
                setting.InductionCode = "SBL";
                setting.WebAppName = "SBL";
                setting.ScheduleEditDays = 2;
                db.Settings.Add(setting);
                db.SaveChanges();
            }

            var getsetting = db.Settings.FirstOrDefault();

            return View(getsetting);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult InductionCode(Setting setting)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Settings.Where(x => x.Id == setting.Id).FirstOrDefault();

                    original.WebAppName = original.WebAppName;
                    original.InductionCode = setting.InductionCode;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: InductionCode - User: " + currentuser.Id, "", false);

                    return RedirectToAction("dashboard");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: InductionCode - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(setting);
        }

        public ActionResult InductionCalendar()
        {
            // var inductionCalendarData = db.InductionCalendars.Where(x => x.Active == true && x.Deleted == false).ToList();
            return View();
        }

        public JsonResult AddUpdateInductionCalendar(int id, string location, double timeStamp)
        {
            if (id > 0)
            {
                InductionCalendar s = new InductionCalendar { Id = id, Location = location };
                db.InductionCalendars.Attach(s);
                db.Entry(s).Property(u => u.Location).IsModified = true;
                s.Location = location;
                db.SaveChanges();
            }
            else
            {
                InductionCalendar calendar = new InductionCalendar();
                calendar.DateTime = Utility.ConvertFromUnixTimestamp(timeStamp);
                calendar.Location = location;
                calendar.Active = true;
                calendar.Deleted = false;
                calendar.DateCreated = DateTime.Now;
                db.InductionCalendars.Add(calendar);
                db.SaveChanges();
                id = calendar.Id;
            }
            return Json(new
            {
                id = id
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteInductionCalendar(int id)
        {
            InductionCalendar s = new InductionCalendar { Id = id, Deleted = true };
            db.InductionCalendars.Attach(s);
            db.Entry(s).Property(u => u.Location).IsModified = true;
            s.Deleted = true;
            db.SaveChanges();
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InductionCalendarData()
        {
            var inductionCalendarData = db.InductionCalendars.Where(x => x.Active && x.Deleted == false).ToList();
            var calanderDataList = from e in inductionCalendarData

                                   select new
                                   {
                                       id = e.Id,
                                       title = e.Location,
                                       description = e.Location,// + " at "+ e.DateTime.ToString("dd/MM/yyyy hh:mm tt"),
                                       start = e.DateTime.ToString("s"),
                                       end = e.DateTime.ToString("s"),
                                       allDay = false

                                   };

            var rows = calanderDataList.ToArray();

            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        //
        // price list
        //
        public ActionResult PriceList()
        {
            if (!db.Settings.Any())
            {
                Setting setting = new Setting();
                setting.InductionCode = "SBL";
                setting.WebAppName = "SBL";
                setting.ScheduleEditDays = 2;
                db.Settings.Add(setting);
                db.SaveChanges();
            }

            var getsetting = db.Settings.FirstOrDefault();

            return View(getsetting);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult PriceList(Setting setting)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Settings.Where(x => x.Id == setting.Id).FirstOrDefault();

                    original.FullRouteAmazon = setting.FullRouteAmazon;
                    original.FullRouteSBL = setting.FullRouteSBL;
                    original.HalfRouteAmazon = setting.HalfRouteAmazon;
                    original.HalfRouteSBL = setting.HalfRouteSBL;
                    original.RemoteDebriefAmazon = setting.RemoteDebriefAmazon;
                    original.RemoteDebriefSBL = setting.RemoteDebriefSBL;

                    original.NurseryRoutesLevel1Amazon = setting.NurseryRoutesLevel1Amazon;
                    original.NurseryRoutesLevel1SBL = setting.NurseryRoutesLevel1SBL;
                    original.NurseryRoutesLevel2Amazon = setting.NurseryRoutesLevel2Amazon;
                    original.NurseryRoutesLevel2SBL = setting.NurseryRoutesLevel2SBL;

                    original.Rescue2HoursAmazon = setting.Rescue2HoursAmazon;
                    original.Rescue2HoursSBL = setting.Rescue2HoursSBL;
                    original.Rescue4HoursAmazon = setting.Rescue4HoursAmazon;
                    original.Rescue4HoursSBL = setting.Rescue4HoursSBL;
                    original.Rescue6HoursAmazon = setting.Rescue6HoursAmazon;
                    original.Rescue6HoursSBL = setting.Rescue6HoursSBL;

                    original.ReDelivery2HoursAmazon = setting.ReDelivery2HoursAmazon;
                    original.ReDelivery2HoursSBL = setting.ReDelivery2HoursSBL;
                    original.ReDelivery4HoursAmazon = setting.ReDelivery4HoursAmazon;
                    original.ReDelivery4HoursSBL = setting.ReDelivery4HoursSBL;
                    original.ReDelivery6HoursAmazon = setting.ReDelivery6HoursAmazon;
                    original.ReDelivery6HoursSBL = setting.ReDelivery6HoursSBL;

                    original.Missort2HoursAmazon = setting.Missort2HoursAmazon;
                    original.Missort2HoursSBL = setting.Missort2HoursSBL;
                    original.Missort4HoursAmazon = setting.Missort4HoursAmazon;
                    original.Missort4HoursSBL = setting.Missort4HoursSBL;
                    original.Missort6HoursAmazon = setting.Missort6HoursAmazon;
                    original.Missort6HoursSBL = setting.Missort6HoursSBL;

                    original.SamedayAmazon = setting.SamedayAmazon;
                    original.SamedaySBL = setting.SamedaySBL;
                    original.TrainingDayAmazon = setting.TrainingDayAmazon;
                    original.TrainingDaySBL = setting.TrainingDaySBL;
                    original.RideAlongAmazon = setting.RideAlongAmazon;
                    original.RideAlongSBL = setting.RideAlongSBL;

                    original.SupportAd1Amazon = setting.SupportAd1Amazon;
                    original.SupportAd1Sbl = setting.SupportAd1Sbl;
                    original.SupportAd2Amazon = setting.SupportAd2Amazon;
                    original.SupportAd2Sbl = setting.SupportAd2Sbl;
                    original.SupportAd3Amazon = setting.SupportAd3Amazon;
                    original.SupportAd3Sbl = setting.SupportAd3Sbl;

                    original.LeadDriverAmazon = setting.LeadDriverAmazon;
                    original.LeadDriverSbl = setting.LeadDriverSbl;
                    original.LargeVanAmazon = setting.LargeVanAmazon;
                    original.LargeVanSbl = setting.LargeVanSbl;

                    original.CongestionChargeAmazon = setting.CongestionChargeAmazon;
                    original.CongestionChargeSbl = setting.CongestionChargeSbl;
                    original.LatePaymentAmazon = setting.LatePaymentAmazon;
                    original.LatePaymentSbl = setting.LatePaymentSbl;

                    original.Mileage1MileAmazon = setting.Mileage1MileAmazon;
                    original.Mileage1MileSBL = setting.Mileage1MileSBL;
                    original.BYODAmazon = setting.BYODAmazon;
                    original.BYODSbl = setting.BYODSbl;

                    original.VanRentalPrice = setting.VanRentalPrice;
                    original.InsurancePrice = setting.InsurancePrice;
                    original.GoodsInTransitPrice = setting.GoodsInTransitPrice;
                    original.PublicLiabilityPrice = setting.PublicLiabilityPrice;

                    //

                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: PriceList - User: " + currentuser.Id, "", false);

                    return RedirectToAction("dashboard");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: PriceList - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(setting);
        }
        //
        // invoice info
        //
        public ActionResult RemittanceInfo()
        {
            if (!db.Settings.Any())
            {
                Setting setting = new Setting();
                setting.InductionCode = "SBL";
                setting.WebAppName = "SBL";
                setting.ScheduleEditDays = 2;
                db.Settings.Add(setting);
                db.SaveChanges();
            }

            var getsetting = db.Settings.FirstOrDefault();

            return View(getsetting);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult RemittanceInfo(Setting setting)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Settings.Where(x => x.Id == setting.Id).FirstOrDefault();

                    original.SblRemittanceBusinessName = setting.SblRemittanceBusinessName;
                    original.SblRemittanceBusinessVatNumber = setting.SblRemittanceBusinessVatNumber;
                    original.SblRemittanceThankYouMessage = setting.SblRemittanceThankYouMessage;
                    original.SblRemittanceBusinessAddress = setting.SblRemittanceBusinessAddress;
                    original.SblRemittanceBusinessCity = setting.SblRemittanceBusinessCity;
                    original.SblRemittanceBusinessPostcode = setting.SblRemittanceBusinessPostcode;

                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: RemittanceInfo - User: " + currentuser.Id, "", false);

                    return RedirectToAction("dashboard");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: RemittanceInfo - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(setting);
        }

        //
        // depot
        //
        public ActionResult DepotList()
        {
            return View(db.Depots.Where(x => x.Active == true && x.Deleted == false).ToList());
        }

        public ActionResult DepotCreate()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult DepotCreate(Depot depot)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    depot.DateCreated = DateTime.Now;
                    depot.Active = true;
                    depot.Deleted = false;
                    db.Depots.Add(depot);
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: DepotCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("depotlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: DepotCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View();
        }

        public ActionResult DepotEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Depot depot = db.Depots.Find(id);

            if (depot == null)
            {
                return HttpNotFound();
            }

            return View(depot);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult DepotEdit(Depot depot)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Depots.Where(x => x.Id == depot.Id).FirstOrDefault();

                    original.Name = depot.Name;
                    original.Telephone = depot.Telephone;
                    original.Address = depot.Address;
                    original.City = depot.City;
                    original.Postcode = depot.Postcode;
                    original.Country = depot.Country;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: DepotEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("depotlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: DepotEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(depot);
        }





        //
        // charge credit
        //
        public ActionResult ChargeList(int? associateId, int? depotId, int? weekNumber)
        {
            var charges = db.Charges.Where(x => x.Active == true && x.Deleted == false).ToList();
            if (associateId != null && associateId > 0)
            {
                charges = charges.Where(x => x.AssociateId == associateId).ToList();
            }
            if (depotId != null && depotId > 0)
            {
                charges = charges.Where(x => x.Associate.DepotId == depotId).ToList();
            }
            if (weekNumber != null && weekNumber > 0)
            {
                var weekStartDate = weekNumber.Value.FirstDateOfWeek();
                var weekEndDate = weekStartDate.LastDayOfWeek();
                charges = charges.Where(x => x.Date >= weekStartDate && x.Date <= weekEndDate).ToList();
            }
            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Depots = depots;
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Associates = associates;
            return View(charges);
        }

        public ActionResult ChargeCreate()
        {
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ChargeCreate(Charge charge)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    charge.DateCreated = DateTime.Now;
                    charge.Active = true;
                    charge.Deleted = false;
                    db.Charges.Add(charge);
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("chargelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            return View();
        }

        public ActionResult ChargeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Charge charge = db.Charges.Find(id);

            if (charge == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == charge.AssociateId,
            });

            ViewBag.Associates = associates;

            return View(charge);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ChargeEdit(Charge charge)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Charges.Where(x => x.Id == charge.Id).FirstOrDefault();

                    original.AssociateId = charge.AssociateId;
                    original.Description = charge.Description;
                    original.Date = charge.Date;
                    original.Amount = charge.Amount;
                    original.SetAsCredit = charge.SetAsCredit;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("chargelist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == charge.AssociateId,
            });

            ViewBag.Associates = associates;

            return View(charge);
        }

        public JsonResult ChargeDelete(int ChargeId)
        {
            var charge = db.Charges.Where(x => x.Id == ChargeId).FirstOrDefault();
            db.Charges.Remove(charge);
            db.SaveChanges();

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }








        //
        // charge pcn
        //
        public ActionResult ChargePcnList(int? associateId, int? depotId, int? weekNumber)
        {
            var charges = db.ChargePcns.Where(x => x.Active == true && x.Deleted == false).ToList();
            if (associateId != null && associateId > 0)
            {
                charges = charges.Where(x => x.AssociateId == associateId).ToList();
            }
            if (depotId != null && depotId > 0)
            {
                charges = charges.Where(x => x.Associate.DepotId == depotId).ToList();
            }
            if (weekNumber != null && weekNumber > 0)
            {
                var weekStartDate = weekNumber.Value.FirstDateOfWeek();
                var weekEndDate = weekStartDate.LastDayOfWeek();
                charges = charges.Where(x => x.Date >= weekStartDate && x.Date <= weekEndDate).ToList();
            }
            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Depots = depots;
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Associates = associates;
            return View(charges);
        }

        public ActionResult ChargePcnCreate()
        {
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = false
                });
            }
            ViewBag.Weeks = weeks;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ChargePcnCreate(ChargePcn chargepcn)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    chargepcn.DateLastInstalment = chargepcn.DateFirstInstalment.AddDays(7 * chargepcn.WeekFrequency);
                    chargepcn.DateCreated = DateTime.Now;
                    chargepcn.Active = true;
                    chargepcn.Deleted = false;
                    db.ChargePcns.Add(chargepcn);
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: ChargePcnCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("ChargePcnList");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: ChargePcnCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = false
                });
            }
            ViewBag.Weeks = weeks;

            return View();
        }

        public ActionResult ChargePcnEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ChargePcn chargepcn = db.ChargePcns.Find(id);

            if (chargepcn == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == chargepcn.AssociateId,
            });

            ViewBag.Associates = associates;

            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = false
                });
            }
            ViewBag.Weeks = weeks;

            return View(chargepcn);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ChargePcnEdit(ChargePcn chargepcn)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.ChargePcns.Where(x => x.Id == chargepcn.Id).FirstOrDefault();

                    original.AssociateId = chargepcn.AssociateId;
                    original.Date = chargepcn.Date;
                    original.Description = chargepcn.Description;
                    original.PcnFee = chargepcn.PcnFee;
                    original.AdminFee = chargepcn.AdminFee;
                    original.NumberOfInstalments = chargepcn.NumberOfInstalments;
                    original.WeekFrequency = chargepcn.WeekFrequency;
                    original.DateFirstInstalment = chargepcn.DateFirstInstalment;
                    original.DateLastInstalment = chargepcn.DateFirstInstalment.AddDays(7 * chargepcn.WeekFrequency);

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: ChargePcnEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("ChargePcnList");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: ChargePcnEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == chargepcn.AssociateId,
            });

            ViewBag.Associates = associates;

            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = false
                });
            }
            ViewBag.Weeks = weeks;

            return View(chargepcn);
        }

        public JsonResult ChargePcnDelete(int ChargePcnId)
        {
            var charge = db.ChargePcns.Where(x => x.Id == ChargePcnId).FirstOrDefault();
            db.ChargePcns.Remove(charge);
            db.SaveChanges();

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }










        //
        // charge claim
        //
        public ActionResult ChargeClaimList(int? associateId, int? depotId, int? weekNumber)
        {
            var charges = db.ChargeClaims.Where(x => x.Active == true && x.Deleted == false).ToList();
            if (associateId != null && associateId > 0)
            {
                charges = charges.Where(x => x.AssociateId == associateId).ToList();
            }
            if (depotId != null && depotId > 0)
            {
                charges = charges.Where(x => x.Associate.DepotId == depotId).ToList();
            }
            if (weekNumber != null && weekNumber > 0)
            {
                var weekStartDate = weekNumber.Value.FirstDateOfWeek();
                var weekEndDate = weekStartDate.LastDayOfWeek();
                charges = charges.Where(x => x.Date >= weekStartDate && x.Date <= weekEndDate).ToList();
            }
            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Depots = depots;
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Associates = associates;
            return View(charges);
        }

        public ActionResult ChargeClaimCreate()
        {
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;
            
            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = false
                });
            }
            ViewBag.Weeks = weeks;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ChargeClaimCreate(ChargeClaim chargeclaim)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    chargeclaim.DateLastInstalment = chargeclaim.DateFirstInstalment.AddDays(7 * chargeclaim.WeekFrequency);
                    chargeclaim.DateCreated = DateTime.Now;
                    chargeclaim.Active = true;
                    chargeclaim.Deleted = false;
                    db.ChargeClaims.Add(chargeclaim);
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeClaimCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("ChargeClaimList");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeClaimCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Associates = associates;

            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = false
                });
            }
            ViewBag.Weeks = weeks;

            return View();
        }

        public ActionResult ChargeClaimEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ChargeClaim chargeclaim = db.ChargeClaims.Find(id);

            if (chargeclaim == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == chargeclaim.AssociateId
            });

            ViewBag.Associates = associates;

            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = i == chargeclaim.WeekFrequency
                });
            }
            ViewBag.Weeks = weeks;

            return View(chargeclaim);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ChargeClaimEdit(ChargeClaim chargeclaim)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.ChargeClaims.Where(x => x.Id == chargeclaim.Id).FirstOrDefault();

                    original.AssociateId = chargeclaim.AssociateId;
                    original.Date = chargeclaim.Date;
                    original.Description = chargeclaim.Description;
                    original.Amount = chargeclaim.Amount;
                    original.NumberOfInstalments = chargeclaim.NumberOfInstalments;
                    original.WeekFrequency = chargeclaim.WeekFrequency;
                    original.DateFirstInstalment = chargeclaim.DateFirstInstalment;
                    original.DateLastInstalment = chargeclaim.DateFirstInstalment.AddDays(7 * chargeclaim.WeekFrequency);

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeClaimEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("ChargeClaimList");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: ChargeClaimEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == chargeclaim.AssociateId,
            });

            ViewBag.Associates = associates;

            List<SelectListItem> weeks = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                weeks.Add(new SelectListItem
                {
                    Text = (i == 1 ? "Every " + i + " week" : "Every " + i + " weeks"),
                    Value = i.ToString(),
                    Selected = i == chargeclaim.WeekFrequency
                });
            }
            ViewBag.Weeks = weeks;

            return View(chargeclaim);
        }

        public JsonResult ChargeClaimDelete(int ChargeClaimId)
        {
            var charge = db.ChargeClaims.Where(x => x.Id == ChargeClaimId).FirstOrDefault();
            db.ChargeClaims.Remove(charge);
            db.SaveChanges();

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }











        public ActionResult ScheduleEditDays()
        {
            if (!db.Settings.Any())
            {
                Setting setting = new Setting();
                setting.InductionCode = "SBL";
                setting.WebAppName = "SBL";
                setting.ScheduleEditDays = 2;
                db.Settings.Add(setting);
                db.SaveChanges();
            }

            var getsetting = db.Settings.FirstOrDefault();

            return View(getsetting);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ScheduleEditDays(Setting setting)
        {
            if (ModelState.IsValid)
            {
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Settings.Where(x => x.Id == setting.Id).FirstOrDefault();

                    original.ScheduleEditDays = setting.ScheduleEditDays;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: ScheduleEditDays - User: " + currentuser.Id, "", false);

                    return RedirectToAction("dashboard");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: ScheduleEditDays - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(setting);
        }






        //
        // rental company
        //
        public ActionResult RentalList()
        {
            return View(db.Rentals.Where(x => x.Active == true && x.Deleted == false).ToList());
        }
        public ActionResult RentalCreate()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult RentalCreate(Rental rental)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    DateTime datetimenow = DateTime.Now;

                    // db
                    rental.DateCreated = datetimenow;
                    rental.Active = true;
                    rental.Deleted = false;
                    db.Rentals.Add(rental);
                    db.SaveChanges();

                    // log price
                    RentalPriceLog log = new RentalPriceLog();
                    log.RentalId = rental.Id;
                    log.RentalPrice = rental.RentalPrice;
                    log.InsurancePrice = rental.InsurancePrice;
                    log.DateChanged = datetimenow;
                    log.DateCreated = DateTime.Now;
                    log.Active = true;
                    log.Deleted = false;
                    db.RentalPriceLogs.Add(log);
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: RentalCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("rentallist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: RentalCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View();
        }


        public ActionResult RentalEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Rental rental = db.Rentals.Find(id);

            if (rental == null)
            {
                return HttpNotFound();
            }

            return View(rental);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult RentalEdit(Rental rental)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    DateTime datetimenow = DateTime.Now;

                    var original = db.Rentals.FirstOrDefault(x => x.Id == rental.Id);

                    original.Name = rental.Name;
                    original.RentalPrice = rental.RentalPrice;
                    original.InsurancePrice = rental.InsurancePrice;

                    original.Active = original.Active;
                    original.DateCreated = original.DateCreated;
                    original.Deleted = original.Deleted;
                    db.SaveChanges();
                    
                    // log price
                    RentalPriceLog log = new RentalPriceLog();
                    log.RentalId = original.Id;
                    log.RentalPrice = rental.RentalPrice;
                    log.InsurancePrice = rental.InsurancePrice;
                    log.DateChanged = datetimenow;
                    log.DateCreated = DateTime.Now;
                    log.Active = true;
                    log.Deleted = false;
                    db.RentalPriceLogs.Add(log);
                    db.SaveChanges();
                    
                    // log
                    Helpers.Logging.LogEntry("Admin: RentalEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("rentallist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: RentalEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(rental);
        }


































        public ActionResult UserList()
        {
            return View(db.Users.OrderBy(x => x.UserName).ToList());
        }


        public ActionResult UserEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == user.DepotId,
            });

            ViewBag.Depots = depots;

            return View(user);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult UserEdit(ApplicationUser user, string TempDir)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    var original = db.Users.FirstOrDefault(x => x.Id == user.Id);

                    if (original != null)
                    {
                        original.Email = user.Email;
                        original.DepotId = user.DepotId;
                        db.SaveChanges();
                    }
                    // log
                    Helpers.Logging.LogEntry("Admin: UserEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("userlist");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: UserEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == user.DepotId,
            });

            ViewBag.Depots = depots;

            return View(user);
        }
        //
        // roles
        //

        public virtual ActionResult RoleList()
        {
            return View(db.Roles.ToList());
        }


        public ActionResult RoleCreate()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RoleCreate(string Name)
        {
            // current user
            var userid = User.Identity.GetUserId();
            var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

                if (!roleManager.RoleExists(Name))
                {
                    var role = new IdentityRole();
                    role.Name = Name;
                    roleManager.Create(role);
                }

                // log
                Helpers.Logging.LogEntry("Admin: RoleCreate - User: " + currentuser.Id, "", false);
            }
            catch (Exception ex)
            {
                // log
                Helpers.Logging.LogEntry("Admin: RoleCreate - User: " + currentuser.Id, ex.Message, true);
            }

            return RedirectToAction("rolelist");
        }


        public ActionResult RoleAssign()
        {
            IEnumerable<SelectListItem> users = db.Users.OrderBy(x => x.UserName).ToList().Select(x => new SelectListItem()
            {
                Text = x.UserName,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Users = users;

            //

            IEnumerable<SelectListItem> roles = db.Roles.OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Name,
                Selected = false,
            });

            ViewBag.Roles = roles;

            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult RoleAssign(string UserId, string Name)
        {
            return RedirectToAction("rolelist");
        }

        public ActionResult SettingEdit()
        {
            if (!db.Settings.Any())
            {
                Setting newsetting = new Setting();
                newsetting.WebAppName = "SBL";
                db.Settings.Add(newsetting);
                db.SaveChanges();
            }
            Setting setting = db.Settings.FirstOrDefault();
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SettingEdit(Setting setting)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Settings.FirstOrDefault();

                    original.WebAppName = setting.WebAppName;

                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Admin: SettingEdit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("dashboard");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: SettingEdit - User: " + currentuser.Id, ex.Message, true);
                }
            }

            return View(setting);
        }

        #region Download Assoicate Profile In PDF
        /// <summary>
        /// Download Assoicate Profile In PDF
        /// </summary>
        /// <param name="associateId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DownloadProfileDetail(int? associateId)
        {
            var associate = db.Associates.FirstOrDefault(x => x.Id == associateId.Value);
            if (associate != null)
            {
                var url = ConfigurationManager.AppSettings["WebSiteUrl"] + "/home/DownloadAssociateProfile/" + associate.Id;
                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                PdfPageSize pageSize = PdfPageSize.Letter;
                PdfPageOrientation pdfOrientation = PdfPageOrientation.Portrait;

                int webPageWidth = 1920;

                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = string.Format("{0}.pdf", associate.Name);
                return fileResult;
            }
            return null;
        }
        #endregion





        #region Find Inspection File
        /// <summary>
        ///  Find Inspection File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult InspectionFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inspection inspection = db.Inspections.Find(id);

            if (inspection == null)
            {
                return HttpNotFound();
            }
            return View(inspection);
        }
        #endregion

        #region Add Inspection File
        /// <summary>
        ///  Add Inspection File
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="expiryDate"></param>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult InspectionFile(int? Id, string description, DateTime? expiryDate, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.FirstOrDefault(x => x.Id == userid);

                try
                {
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        Int32 length = fileUpload.ContentLength;
                        byte[] temp = new byte[length];
                        fileUpload.InputStream.Read(temp, 0, length);

                        InspectionFile inspectionFile = new InspectionFile();
                        inspectionFile.InspectionId = Id.Value;

                        inspectionFile.DataFile = temp;
                        inspectionFile.DataFileContentType = fileUpload.ContentType;
                        inspectionFile.DataFileName = fileUpload.FileName;
                        inspectionFile.DataFileDescription = description;
                        inspectionFile.DataFileExpiryDate = expiryDate;

                        inspectionFile.DateCreated = DateTime.Now;
                        inspectionFile.Active = true;
                        inspectionFile.Deleted = false;
                        db.InspectionFiles.Add(inspectionFile);
                        db.SaveChanges();
                    }

                    // log
                    Helpers.Logging.LogEntry("Admin: Inspection File - User: " + currentuser.Id, "", false);

                    return RedirectToAction("InspectionFile", new { id = Id.Value });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: Inspection File - User: " + currentuser.Id, ex.Message, true);
                }
            }
            Inspection inspection = db.Inspections.Find(Id);

            return View(inspection);
        }
        #endregion

        #region Inspection File List
        /// <summary>
        /// Inspection File List
        /// </summary>
        /// <returns></returns>
        public ActionResult InspectionFileList()
        {
            var inspectionFiles =
                db.InspectionFiles.Where(x => x.Active == true && x.Deleted == false)
                    .OrderByDescending(x => x.DataFileExpiryDate)
                    .ToList();
            return View(inspectionFiles);
        }
        #endregion

        #region Remove Inspection File
        /// <summary>
        ///  Remove Inspection File
        /// </summary>
        /// <param name="inspectionFileId"></param>
        /// <returns></returns>
        public JsonResult InspectionFileRemove(int? inspectionFileId)
        {
            var inspectionfile = db.InspectionFiles.FirstOrDefault(x => x.Id == inspectionFileId);
            if (inspectionfile != null)
            {
                inspectionfile.Deleted = true;
                db.SaveChanges();
            }
            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Download Inspection File
        /// <summary>
        ///  Download Inspection File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult InspectionFileDownload(int id)
        {
            var inspectionfile = db.InspectionFiles.FirstOrDefault(x => x.Id == id);
            if (inspectionfile != null)
            {
                return File(inspectionfile.DataFile, inspectionfile.DataFileContentType, inspectionfile.DataFileName);
            }
            return null;
        }
        #endregion

        #region Finance List
        /// <summary>
        /// List of Finance
        /// </summary>
        /// <returns></returns>
        public ActionResult FinanceList()
        {
            return View(db.Finances.Where(x => x.Active == true && x.Deleted == false).ToList());
        }
        #endregion

        #region View Of Add Finance
        /// <summary>
        ///  View Of Add Finance
        /// </summary>
        /// <returns></returns>
        public ActionResult FinanceCreate()
        {
            return View();
        }
        #endregion

        #region Add Finance Detail to Database
        /// <summary>
        ///  Add Finance Detail to Database
        /// </summary>
        /// <param name="finance">object of Finance </param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult FinanceCreate(Finance finance)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    // db
                    finance.DateCreated = DateTime.Now;
                    finance.Active = true;
                    finance.Deleted = false;
                    db.Finances.Add(finance);
                    db.SaveChanges();
                    // log
                    Helpers.Logging.LogEntry("Admin: FinanceCreate - User: " + currentuser.Id, "", false);

                    return RedirectToAction("FinanceList");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: FinanceCreate - User: " + currentuser.Id, ex.Message, true);
                }
            }
            return View();
        }
        #endregion

        #region Update Finance Detail
        /// <summary>
        ///  Update Finance Detail
        /// </summary>
        /// <param name="id">Id of the Finance</param>
        /// <returns></returns>
        public ActionResult FinanceEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Finance finance = db.Finances.Find(id);

            if (finance == null)
            {
                return HttpNotFound();
            }
            return View(finance);
        }
        #endregion

        #region Update Finance Detail
        /// <summary>
        ///  Update Finance Detail
        /// </summary>
        /// <param name="finance"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult FinanceEdit(Finance finance)
        {
            if (ModelState.IsValid)
            {
                // current user
                var userid = User.Identity.GetUserId();
                var currentuser = db.Users.Where(x => x.Id == userid).FirstOrDefault();

                try
                {
                    var original = db.Finances.Where(x => x.Id == finance.Id).FirstOrDefault();
                    if (original != null)
                    {
                        original.Description = finance.Description;
                        original.Amount = finance.Amount;
                        original.SetAsCredit = finance.SetAsCredit;

                        original.Active = original.Active;
                        original.DateCreated = original.DateCreated;
                        original.Deleted = original.Deleted;
                        db.SaveChanges();
                    }
                    // log
                    Helpers.Logging.LogEntry("Admin: Finance Edit - User: " + currentuser.Id, "", false);

                    return RedirectToAction("FinanceList");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Admin: Finance Edit - User: " + currentuser.Id, ex.Message, true);
                }
            }
            return View(finance);
        }
        #endregion

        #region Delete Finance Detail
        /// <summary>
        /// Delete Finance Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteFinance(int id)
        {
            if (id > 0)
            {
                var finance = db.Finances.Where(x => x.Id == id).FirstOrDefault();
                if (finance != null)
                {
                    finance.Deleted = true;
                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region View Associate File
        /// <summary>
        /// View Associate File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult ViewAssociateFile(int id)
        {
            var associatefile = db.AssociateFiles.FirstOrDefault(x => x.Id == id);
            if (associatefile != null)
            {
                return Utility.GetFileStreamResult(associatefile.DataFile, associatefile.DataFileContentType);
            }
            return null;
        }
        #endregion

        #region View Inspection File
        /// <summary>
        ///  View Inspection File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult ViewInspectionFile(int id)
        {
            var inspectionsFiles = db.InspectionFiles.FirstOrDefault(x => x.Id == id);
            if (inspectionsFiles != null)
            {
                return Utility.GetFileStreamResult(inspectionsFiles.DataFile, inspectionsFiles.DataFileContentType);
            }
            return null;
        }
        #endregion

        #region View Accident File
        /// <summary>
        ///  View Accident File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult ViewAccidentFile(int id)
        {
            var accidentFile = db.AccidentFiles.FirstOrDefault(x => x.Id == id);
            if (accidentFile != null)
            {
                return Utility.GetFileStreamResult(accidentFile.DataFile, accidentFile.DataFileContentType);
            }
            return null;
        }
        #endregion
















        #region Associate Remittance Detail List
        /// <summary>
        /// Associate Remittance Detail List
        /// </summary>
        /// <param name="depotId"></param>
        /// <param name="weekNumber"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ActionResult AssociateRemittanceDetailList(int? depotId, int? weekNumber, int? associateId)
        {

            var routeAllocations = db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();

            // current user
            var userid = User.Identity.GetUserId();
            if (User.IsInRole(WebConstant.SBLUserRole.Driver))
            {
                routeAllocations = routeAllocations.Where(x => x.Associate.UserId == userid).ToList();
            }

            if (associateId.HasValue && associateId > 0)
            {
                routeAllocations = routeAllocations.Where(x => x.Associate?.Id == associateId.Value).ToList();
            }
            if (depotId.HasValue)
            {
                routeAllocations = routeAllocations.Where(x => x.Associate?.DepotId == depotId.Value).ToList();
            }



            DateTime? weekStartDate = null;
            DateTime? weekEndDate = null;
            if (weekNumber != null && weekNumber > 0)
            {
                weekStartDate = weekNumber.Value.FirstDateOfWeek();
                weekEndDate = weekStartDate.Value.LastDayOfWeek();

                routeAllocations = routeAllocations.Where(x => x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate).ToList();
            }



            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Depots = depots;
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Associates = associates;






            List<AssociateRemittanceViewModel> associateRouteList = new List<AssociateRemittanceViewModel>();

            foreach (var routeAllocation in routeAllocations)
            {
                bool addAssociateToList = false;
                var routeWeekNumber = 0;
                if (routeAllocation.RouteDate != null)
                    routeWeekNumber = routeAllocation.RouteDate.Value.GetWeekNumberOfYear();
                AssociateRemittanceViewModel associateRemittance = null;
                if (routeAllocation.AssociateId != null)
                {
                    var routeAssociateId = routeAllocation.AssociateId.Value;
                    if (associateRouteList.Count > 0)
                    {
                        associateRemittance = associateRouteList.FirstOrDefault(x => x.AssociateId == routeAssociateId && x.WeekNumber == routeWeekNumber);
                    }
                    if (associateRemittance == null)
                    {
                        associateRemittance = new AssociateRemittanceViewModel();
                        associateRemittance.WeekNumber = routeWeekNumber;
                        associateRemittance.AssociateName = routeAllocation.Associate.Name;
                        associateRemittance.AssociateEmail = routeAllocation.Associate.Email;
                        associateRemittance.AssociateId = routeAssociateId;
                        associateRemittance.AuthPoc = routeAllocation.AuthPoc;
                        associateRemittance.AuthPayroll = routeAllocation.AuthPayroll;
                        associateRemittance.AuthAdmin = routeAllocation.AuthAdmin;
                        addAssociateToList = true;
                    }
                    var route = new AssociateRemittanceViewModel.Route();
                    //route.Ad1Quantity = routeAllocation.Ad1Quantity;
                    //route.Ad2Quantity = routeAllocation.Ad2Quantity;
                    //route.Ad3Quantity = routeAllocation.Ad3Quantity;
                    route.FuelSupport = routeAllocation.Fuel;
                    //route.Deduct = routeAllocation.Deduct;
                    route.Mileage = routeAllocation.Mileage;
                    route.RouteCode1 = routeAllocation.RouteCode1;
                    route.RouteType1 = routeAllocation.RouteType1;
                    route.RouteDate = routeAllocation.RouteDate;
                    route.Id = routeAllocation.Id;
                    route.AuthPayroll = routeAllocation.AuthPayroll;
                    route.AuthPoc = routeAllocation.AuthPoc;
                    route.AuthAdmin = routeAllocation.AuthAdmin;
                    route.Depot = routeAllocation.Depot?.Name;
                    if (associateRemittance.Routes == null)
                    {
                        associateRemittance.Routes = new List<AssociateRemittanceViewModel.Route>();
                    }
                    associateRemittance.Routes.Add(route);
                    if (addAssociateToList)
                    {
                        associateRouteList.Add(associateRemittance);
                    }
                }
            }
            // associateRouteList = associateRouteList.OrderBy(x => x.Routes.Max(c => c.RouteRate)).ToList();
            associateRouteList = associateRouteList.OrderBy(x => x.WeekNumber).ToList();
            return View(associateRouteList);







        }
        #endregion









        #region Associate Remittance Detail List
        /// <summary>
        /// Associate Remittance Detail List
        /// </summary>
        /// <param name="depotId"></param>
        /// <param name="weekNumber"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ActionResult AssociateRemittanceDetailList2(int? depotId, int? weekNumber, int? associateId)
        {

            var routeAllocations = db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();

            // current user
            var userid = User.Identity.GetUserId();
            if (User.IsInRole(WebConstant.SBLUserRole.Driver))
            {
                routeAllocations = routeAllocations.Where(x => x.Associate.UserId == userid).ToList();
            }

            if (associateId.HasValue && associateId > 0)
            {
                routeAllocations = routeAllocations.Where(x => x.Associate?.Id == associateId.Value).ToList();
            }
            if (depotId.HasValue)
            {
                routeAllocations = routeAllocations.Where(x => x.Associate?.DepotId == depotId.Value).ToList();
            }



            DateTime? weekStartDate = null;
            DateTime? weekEndDate = null;
            if (weekNumber != null && weekNumber > 0)
            {
                weekStartDate = weekNumber.Value.FirstDateOfWeek();
                weekEndDate = weekStartDate.Value.LastDayOfWeek();

                routeAllocations = routeAllocations.Where(x => x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate).ToList();
            }



            IEnumerable<SelectListItem> depots = db.Depots.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Depots = depots;
            IEnumerable<SelectListItem> associates = db.Associates.Where(x => x.Active && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });
            ViewBag.Associates = associates;

            List<AssociateRemittanceViewModel> associateRouteList = new List<AssociateRemittanceViewModel>();
            foreach (var routeAllocation in routeAllocations)
            {
                bool addAssociateToList = false;
                var routeWeekNumber = 0;
                if (routeAllocation.RouteDate != null)
                    routeWeekNumber = routeAllocation.RouteDate.Value.GetWeekNumberOfYear();
                AssociateRemittanceViewModel associateRemittance = null;
                if (routeAllocation.AssociateId != null)
                {
                    var routeAssociateId = routeAllocation.AssociateId.Value;
                    if (associateRouteList.Count > 0)
                    {
                        associateRemittance = associateRouteList.FirstOrDefault(x => x.AssociateId == routeAssociateId && x.WeekNumber == routeWeekNumber);
                    }
                    if (associateRemittance == null)
                    {
                        associateRemittance = new AssociateRemittanceViewModel();
                        associateRemittance.WeekNumber = routeWeekNumber;
                        associateRemittance.AssociateName = routeAllocation.Associate.Name;
                        associateRemittance.AssociateEmail = routeAllocation.Associate.Email;
                        associateRemittance.AssociateId = routeAssociateId;
                        associateRemittance.AuthPoc = routeAllocation.AuthPoc;
                        associateRemittance.AuthPayroll = routeAllocation.AuthPayroll;
                        associateRemittance.AuthAdmin = routeAllocation.AuthAdmin;
                        addAssociateToList = true;
                    }
                    var route = new AssociateRemittanceViewModel.Route();
                    //route.Ad1Quantity = routeAllocation.Ad1Quantity;
                    //route.Ad2Quantity = routeAllocation.Ad2Quantity;
                    //route.Ad3Quantity = routeAllocation.Ad3Quantity;
                    route.FuelSupport = routeAllocation.Fuel;
                    //route.Deduct = routeAllocation.Deduct;
                    route.Mileage = routeAllocation.Mileage;
                    route.RouteCode1 = routeAllocation.RouteCode1;
                    route.RouteType1 = routeAllocation.RouteType1;
                    route.RouteDate = routeAllocation.RouteDate;
                    route.Id = routeAllocation.Id;
                    route.AuthPayroll = routeAllocation.AuthPayroll;
                    route.AuthPoc = routeAllocation.AuthPoc;
                    route.AuthAdmin = routeAllocation.AuthAdmin;
                    route.Depot = routeAllocation.Depot?.Name;
                    if (associateRemittance.Routes == null)
                    {
                        associateRemittance.Routes = new List<AssociateRemittanceViewModel.Route>();
                    }
                    associateRemittance.Routes.Add(route);
                    if (addAssociateToList)
                    {
                        associateRouteList.Add(associateRemittance);
                    }
                }
            }
            // associateRouteList = associateRouteList.OrderBy(x => x.Routes.Max(c => c.RouteRate)).ToList();
            associateRouteList = associateRouteList.OrderBy(x => x.WeekNumber).ToList();
            return View(associateRouteList);







        }
        #endregion



















        #region Download Route Allocation List DateWise in PDF
        /// <summary>
        /// Download Route Allocation List DateWise in PDF
        /// </summary>
        /// <param name="routeDate"></param>
        /// <returns></returns>

        public FileResult DownloadRouteAllocationList(DateTime routeDate)
        {
            var url = ConfigurationManager.AppSettings["WebSiteUrl"] + "/home/DownloadRouteAllocationList?routeDate=" + routeDate.Date.ToString("dd MMM yyyy");
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            PdfPageSize pageSize = PdfPageSize.Letter;
            PdfPageOrientation pdfOrientation = PdfPageOrientation.Portrait;

            int webPageWidth = 1920;

            int webPageHeight = 0;

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl(url);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = string.Format("route_{0}.pdf", routeDate.ToString("dd-MMM-yyyy"));

            return fileResult;
        }
        #endregion


        /*

        #region Update Auth Poc Or Auth Payroll in Route Allocation
        /// <summary>
        ///  Update Auth Poc Or Auth Payroll in Route Allocation
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="weekNumber"></param>
        /// <returns></returns>
        public JsonResult UpdateAuthPocOrPayroll(int associateId, int weekNumber)
        {
            DateTime weekStartDate = weekNumber.FirstDateOfWeek();
            DateTime weekEndDate = weekStartDate.LastDayOfWeek();
            var routeAllocations =
               db.RouteAllocations.Where(
                                           x => x.Active && x.Deleted == false && x.AssociateId == associateId
                                           && x.Associate.Active
                                           && x.Associate.Deleted == false && x.RouteDate >= weekStartDate && x.RouteDate <= weekEndDate)
                                           .OrderBy(x => x.RouteDate)
                                           .ToList();
            if (routeAllocations.Count > 0)
            {
                // current user
                bool isUpdateAuthPoc = false;
                bool isUpdateAuthPayroll = false;
                var userid = User.Identity.GetUserId();

                if (User.IsInRole(WebConstant.SBLUserRole.POC))
                {
                    isUpdateAuthPoc = true;
                    isUpdateAuthPayroll = false;
                }
                else if (User.IsInRole(WebConstant.SBLUserRole.Payroll))
                {
                    isUpdateAuthPayroll = true;
                    isUpdateAuthPoc = false;
                }

                if (isUpdateAuthPoc || isUpdateAuthPayroll)
                {
                    foreach (var routeAllocation in routeAllocations)
                    {

                        if (isUpdateAuthPoc)
                        {
                            routeAllocation.AuthPoc = true;
                        }
                        else if (isUpdateAuthPayroll)
                        {
                            routeAllocation.AuthPayroll = true;
                        }

                        db.RouteAllocations.Attach(routeAllocation);

                        if (isUpdateAuthPoc)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthPoc).IsModified = true;
                        }
                        else if (isUpdateAuthPayroll)
                        {
                            db.Entry(routeAllocation).Property(x => x.AuthPayroll).IsModified = true;
                        }
                    }

                    db.SaveChanges();
                }
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        */

 


         



        #region Find the List of the Depot
        public JsonResult GetDepots(int? associateDepotId)
        {
            var depots = (from x in db.Depots
                          where x.Id != associateDepotId && x.Active == true && x.Deleted == false
                          select new
                          {
                              Id = x.Id,
                              Name = x.Name
                          }).ToList();

            return Json(new
            {
                depots = depots
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region Get Route Fields
        public JsonResult GetRouteDetails(int? RouteId)
        {
            try
            {
                var depots = (from x in db.Depots
                              where x.Active == true && x.Deleted == false
                              select new
                              {
                                  Id = x.Id,
                                  Name = x.Name
                              }).ToList();

                if (db.RouteAllocations.Where(x => x.Id == RouteId).Any())
                {
                    var route = db.RouteAllocations.Where(x => x.Id == RouteId).FirstOrDefault();

                    return Json(new
                    {
                        RouteId = RouteId,
                        //
                        RouteCode1 = route.RouteCode1,
                        RouteType1 = route.RouteType1,
                        RouteCode2 = route.RouteCode2,
                        RouteType2 = route.RouteType2,
                        RouteCode3 = route.RouteCode3,
                        RouteType3 = route.RouteType3,
                        Depot = route.DepotId,
                        Mileage = route.Mileage,
                        FuelCharge = route.FuelChargePrice,
                        Byod = route.BYODPrice,
                        CongestionCharge = route.CongestionChargePrice,
                        LatePayment = route.LatePaymentQuantity,
                        StartTime = route.StartTime,
                        EndTime = route.EndTime,
                        TotalTime = route.TotalTime,
                        Notes = route.Notes,
                        //
                        status = 1,
                        depots = depots
                        //
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = 0,
                        depots = depots
                        //
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion



        public JsonResult DeleteUser(string UserId)
        {
            var user = db.Users.Where(x => x.Id == UserId).FirstOrDefault();
            db.Users.Remove(user);
            db.SaveChanges();

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }











    }//end class
}//end namespace
