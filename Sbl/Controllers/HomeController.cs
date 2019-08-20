using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sbl.Helpers;
using Sbl.Models;

namespace Sbl.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return RedirectToAction("dashboard", "admin");
        }

        #region Download Associate Profile
        /// <summary>
        /// Download Associate Profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DownloadAssociateProfile(int id)
        {
            var associate = db.Associates.FirstOrDefault(x => x.Id == id);
            if (associate != null)
            {
                var associatefiles = (from x in db.AssociateFiles
                                      where x.AssociateId == associate.Id && x.Active == true && x.Deleted == false
                                      select new AssociateProfileViewModel.AssociateFile
                                      {
                                          Id = x.Id,
                                          DataFileName = x.DataFileName,
                                          DataFile = x.DataFile,
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
            else
            {
                return HttpNotFound();
            }
        }
        #endregion

        #region Download Route Allocation List DateWise in PDF
        /// <summary>
        /// Download Route Allocation List DateWise in PDF
        /// </summary>
        /// <param name="routeDate"></param>
        /// <returns></returns>
        public ActionResult DownloadRouteAllocationList(DateTime? routeDate)
        {
            var routeViewModel = new RouteAllocationViewModel();
            routeViewModel.RouteDate = routeDate;
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
            routeViewModel.Routes = routes;
            return View(routeViewModel);
        }
        #endregion
    }//end class
}//end namespace