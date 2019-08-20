using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using Sbl.Models;
using System.Collections.Generic;

namespace Sbl.Controllers
{
    public class PdfController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        public ActionResult AssociateReceipt(int? AssociateId, DateTime? Date)
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
                                  RouteType1 = x.RouteType1,
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


 

                // extra deductions
                var extradeductions = (from x in getroutes
                                       where x.FuelChargePrice > 0
                                       select new AssociateRemittanceViewModel.ExtraDeduction
                                       {
                                           DeductionDate = x.RouteDate,
                                           Description = "Fuel Charge",
                                           DeductionAmount = x.FuelChargePrice
                                       }).ToList();


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
                //
                var totalCredits = sumSubTotal + sumCreditAmount;
                var totalDeductions = sumDeductionAmount + sumSubRentalAmount + sumExtraDeductionAmount;
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
                                  RouteType1 = x.RouteType1,
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



    }
}