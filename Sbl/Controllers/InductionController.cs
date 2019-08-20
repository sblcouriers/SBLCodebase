using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sbl.Models;
using System.Globalization;

namespace Sbl.Controllers
{
    public class InductionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            return View();
        }


 



        public ActionResult Start(string code)
        {
            string dbcode = "";

            if (db.Settings.Any())
            {
                dbcode = db.Settings.FirstOrDefault().InductionCode;

                if (code == dbcode)
                {
                    Induction induction = new Induction();
                    induction.GuidId = Guid.NewGuid().ToString();
                    induction.DateCreated = DateTime.Now;
                    induction.Active = false;
                    induction.Deleted = false;
                    db.Inductions.Add(induction);
                    db.SaveChanges();

                    return RedirectToAction("step1", new { guidid = induction.GuidId });
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            else
            {
                return RedirectToAction("index");
            }
        }



        // step 1

        public ActionResult Step1(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            IEnumerable<SelectListItem> countries = db.Countries.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Countries = countries;

            var viewModel = new InductionStep1ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                //
                FullName = induction.FullName,
                MobileNumber = induction.MobileNumber,
                Email = induction.Email,
                NextOfKinName = induction.NextOfKinName,
                NextOfKinRelationship = induction.NextOfKinRelationship,
                NextOfKinMobile = induction.NextOfKinMobile,
                Address = induction.Address,
                City = induction.City,
                County = induction.County,
                Postcode = induction.Postcode,
                Country = induction.Country,
                //
                Step1Signature = induction.Step1Signature,
                Step1DateSigned = induction.Step1DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step1(InductionStep1ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.FullName = viewModel.FullName;
                    induction.MobileNumber = viewModel.MobileNumber;
                    induction.Email = viewModel.Email;
                    induction.NextOfKinName = viewModel.NextOfKinName;
                    induction.NextOfKinRelationship = viewModel.NextOfKinRelationship;
                    induction.NextOfKinMobile = viewModel.NextOfKinMobile;
                    induction.Address = viewModel.Address;
                    induction.City = viewModel.City;
                    induction.County = viewModel.County;
                    induction.Postcode = viewModel.Postcode;
                    induction.Country = viewModel.Country;
                    //
                    induction.Step1Signature = viewModel.Step1Signature;
                    induction.Step1DateSigned = viewModel.Step1DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step1 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step2", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step1 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            return View();
        }









        // step 2

        public ActionResult Step2(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            IEnumerable<SelectListItem> countries = db.Countries.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Countries = countries;

            var viewModel = new InductionStep2ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                //
                DateOfBirth = induction.DateOfBirth,
                NationalInsuranceNumber = induction.NationalInsuranceNumber,
                Nationality = induction.Nationality,
                UTRNumber = induction.UTRNumber,
                OwnVehicle = induction.OwnVehicle,
                VehicleType = induction.VehicleType,
                VehicleInsuranceRenewalDate = induction.VehicleInsuranceRenewalDate,
                VehicleMOTDue = induction.VehicleMOTDue,
                NameOfTheBank = induction.NameOfTheBank,
                SortCode = induction.SortCode,
                AccountNumber = induction.AccountNumber,
                AccountName = induction.AccountName,
                //
                Step2Signature = induction.Step2Signature,
                Step2DateSigned = induction.Step2DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step2(InductionStep2ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.DateOfBirth = viewModel.DateOfBirth;
                    induction.NationalInsuranceNumber = viewModel.NationalInsuranceNumber;
                    induction.Nationality = viewModel.Nationality;
                    induction.UTRNumber = viewModel.UTRNumber;
                    induction.OwnVehicle = viewModel.OwnVehicle;
                    induction.VehicleType = viewModel.VehicleType;
                    induction.VehicleInsuranceRenewalDate = viewModel.VehicleInsuranceRenewalDate;
                    induction.VehicleMOTDue = viewModel.VehicleMOTDue;
                    induction.NameOfTheBank = viewModel.NameOfTheBank;
                    induction.SortCode = viewModel.SortCode;
                    induction.AccountNumber = viewModel.AccountNumber;
                    induction.AccountName = viewModel.AccountName;
                    //
                    induction.Step2Signature = viewModel.Step2Signature;
                    induction.Step2DateSigned = viewModel.Step2DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step2 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step3", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step2 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> countries = db.Countries.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Countries = countries;

            return View(viewModel);
        }







        // step 3

        public ActionResult Step3(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            IEnumerable<SelectListItem> countries = db.Countries.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Countries = countries;

            var viewModel = new InductionStep3ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                //
                UnspentCriminalConviction = induction.UnspentCriminalConviction,
                CriminalConvictionDetails = induction.CriminalConvictionDetails,
                CRTitle = induction.CRTitle,
                CRSurname = induction.CRSurname,
                CRForename = induction.CRForename,
                CRMotherSurnameBirth = induction.CRMotherSurnameBirth,
                CRMotherForenameBirth = induction.CRMotherForenameBirth,
                CRMaidenFamilyName = induction.CRMaidenFamilyName,
                CROtherName1 = induction.CROtherName1,
                CROtherName2 = induction.CROtherName2,
                CROtherName3 = induction.CROtherName3,
                CRDateOfBirth = induction.CRDateOfBirth,
                CRGender = induction.CRGender,
                CRTownOfBirth = induction.CRTownOfBirth,
                CRCountryOfBirth = induction.CRCountryOfBirth,
                CRPassportNumber = induction.CRPassportNumber,
                CRIsUKPassport = induction.CRIsUKPassport,
                CRPassportCountryOfIssue = induction.CRPassportCountryOfIssue,
                CRPassportIssueDate = induction.CRPassportIssueDate,
                CRPassportExpiryDate = induction.CRPassportExpiryDate,
                CRNationalInsuranceNumber = induction.CRNationalInsuranceNumber,
                CRDriverLicense = induction.CRDriverLicense,
                CRDriverLicenseCountryOfIssue = induction.CRDriverLicenseCountryOfIssue,
                //
                Step3Signature = induction.Step2Signature,
                Step3DateSigned = induction.Step2DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step3(InductionStep3ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.UnspentCriminalConviction = viewModel.UnspentCriminalConviction;
                    induction.CriminalConvictionDetails = viewModel.CriminalConvictionDetails;
                    induction.CRTitle = viewModel.CRTitle;
                    induction.CRSurname = viewModel.CRSurname;
                    induction.CRForename = viewModel.CRForename;
                    induction.CRMotherSurnameBirth = viewModel.CRMotherSurnameBirth;
                    induction.CRMotherForenameBirth = viewModel.CRMotherForenameBirth;
                    induction.CRMaidenFamilyName = viewModel.CRMaidenFamilyName;
                    induction.CROtherName1 = viewModel.CROtherName1;
                    induction.CROtherName2 = viewModel.CROtherName2;
                    induction.CROtherName3 = viewModel.CROtherName3;
                    induction.CRDateOfBirth = viewModel.CRDateOfBirth;
                    induction.CRGender = viewModel.CRGender;
                    induction.CRTownOfBirth = viewModel.CRTownOfBirth;
                    induction.CRCountryOfBirth = viewModel.CRCountryOfBirth;
                    induction.CRPassportNumber = viewModel.CRPassportNumber;
                    induction.CRIsUKPassport = viewModel.CRIsUKPassport;
                    induction.CRPassportCountryOfIssue = viewModel.CRPassportCountryOfIssue;
                    induction.CRPassportIssueDate = viewModel.CRPassportIssueDate;
                    induction.CRPassportExpiryDate = viewModel.CRPassportExpiryDate;
                    induction.CRNationalInsuranceNumber = viewModel.CRNationalInsuranceNumber;
                    induction.CRDriverLicense = viewModel.CRDriverLicense;
                    induction.CRDriverLicenseCountryOfIssue = viewModel.CRDriverLicenseCountryOfIssue;
                    //
                    induction.Step3Signature = viewModel.Step3Signature;
                    induction.Step3DateSigned = viewModel.Step3DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step3 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step4", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step3 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> countries = db.Countries.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Countries = countries;

            return View(viewModel);
        }











        // step 4

        public ActionResult Step4(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            IEnumerable<SelectListItem> countries = db.Countries.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Countries = countries;

            var viewModel = new InductionStep4ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                Address = induction.Address,
                City = induction.City,
                County = induction.County,
                Postcode = induction.Postcode,
                Country = induction.Country,
                //
                CRCurrentAddressNumberStreet = induction.CRCurrentAddressNumberStreet,
                CRCurrentAddressTownCity = induction.CRCurrentAddressTownCity,
                CRCurrentAddressCountyDistrict = induction.CRCurrentAddressCountyDistrict,
                CRCurrentAddressPostcode = induction.CRCurrentAddressPostcode,
                CRCurrentAddressDateMovedIn = induction.CRCurrentAddressDateMovedIn,
                CRCurrentAddressCountry = induction.CRCurrentAddressCountry,
                //
                CRPreviousAddress1NumberStreet = induction.CRPreviousAddress1NumberStreet,
                CRPreviousAddress1TownCity = induction.CRPreviousAddress1TownCity,
                CRPreviousAddress1CountyDistrict = induction.CRPreviousAddress1CountyDistrict,
                CRPreviousAddress1Postcode = induction.CRPreviousAddress1Postcode,
                CRPreviousAddress1DateMovedIn = induction.CRPreviousAddress1DateMovedIn,
                CRPreviousAddress1Country = induction.CRPreviousAddress1Country,
                //
                CRPreviousAddress2NumberStreet = induction.CRPreviousAddress2NumberStreet,
                CRPreviousAddress2TownCity = induction.CRPreviousAddress2TownCity,
                CRPreviousAddress2CountyDistrict = induction.CRPreviousAddress2CountyDistrict,
                CRPreviousAddress2Postcode = induction.CRPreviousAddress2Postcode,
                CRPreviousAddress2DateMovedIn = induction.CRPreviousAddress2DateMovedIn,
                CRPreviousAddress2Country = induction.CRPreviousAddress2Country,
                //
                CRPreviousAddress3NumberStreet = induction.CRPreviousAddress3NumberStreet,
                CRPreviousAddress3TownCity = induction.CRPreviousAddress3TownCity,
                CRPreviousAddress3CountyDistrict = induction.CRPreviousAddress3CountyDistrict,
                CRPreviousAddress3Postcode = induction.CRPreviousAddress3Postcode,
                CRPreviousAddress3DateMovedIn = induction.CRPreviousAddress3DateMovedIn,
                CRPreviousAddress3Country = induction.CRPreviousAddress3Country,
                //
                CRPreviousAddress4NumberStreet = induction.CRPreviousAddress4NumberStreet,
                CRPreviousAddress4TownCity = induction.CRPreviousAddress4TownCity,
                CRPreviousAddress4CountyDistrict = induction.CRPreviousAddress4CountyDistrict,
                CRPreviousAddress4Postcode = induction.CRPreviousAddress4Postcode,
                CRPreviousAddress4DateMovedIn = induction.CRPreviousAddress4DateMovedIn,
                CRPreviousAddress4Country = induction.CRPreviousAddress4Country,
                //
                CRPreviousAddress5NumberStreet = induction.CRPreviousAddress5NumberStreet,
                CRPreviousAddress5TownCity = induction.CRPreviousAddress5TownCity,
                CRPreviousAddress5CountyDistrict = induction.CRPreviousAddress5CountyDistrict,
                CRPreviousAddress5Postcode = induction.CRPreviousAddress5Postcode,
                CRPreviousAddress5DateMovedIn = induction.CRPreviousAddress5DateMovedIn,
                CRPreviousAddress5Country = induction.CRPreviousAddress5Country,
                //
                CRPreviousAddress6NumberStreet = induction.CRPreviousAddress6NumberStreet,
                CRPreviousAddress6TownCity = induction.CRPreviousAddress6TownCity,
                CRPreviousAddress6CountyDistrict = induction.CRPreviousAddress6CountyDistrict,
                CRPreviousAddress6Postcode = induction.CRPreviousAddress6Postcode,
                CRPreviousAddress6DateMovedIn = induction.CRPreviousAddress6DateMovedIn,
                CRPreviousAddress6Country = induction.CRPreviousAddress6Country,
                //
                Step4Signature = induction.Step4Signature,
                Step4DateSigned = induction.Step4DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step4(InductionStep4ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.CRCurrentAddressNumberStreet = viewModel.CRCurrentAddressNumberStreet;
                    induction.CRCurrentAddressTownCity = viewModel.CRCurrentAddressTownCity;
                    induction.CRCurrentAddressCountyDistrict = viewModel.CRCurrentAddressCountyDistrict;
                    induction.CRCurrentAddressPostcode = viewModel.CRCurrentAddressPostcode;
                    induction.CRCurrentAddressDateMovedIn = viewModel.CRCurrentAddressDateMovedIn;
                    induction.CRCurrentAddressCountry = viewModel.CRCurrentAddressCountry;
                    //
                    induction.CRPreviousAddress1NumberStreet = viewModel.CRPreviousAddress1NumberStreet;
                    induction.CRPreviousAddress1TownCity = viewModel.CRPreviousAddress1TownCity;
                    induction.CRPreviousAddress1CountyDistrict = viewModel.CRPreviousAddress1CountyDistrict;
                    induction.CRPreviousAddress1Postcode = viewModel.CRPreviousAddress1Postcode;
                    induction.CRPreviousAddress1DateMovedIn = viewModel.CRPreviousAddress1DateMovedIn;
                    induction.CRPreviousAddress1Country = viewModel.CRPreviousAddress1Country;
                    //
                    induction.CRPreviousAddress2NumberStreet = viewModel.CRPreviousAddress2NumberStreet;
                    induction.CRPreviousAddress2TownCity = viewModel.CRPreviousAddress2TownCity;
                    induction.CRPreviousAddress2CountyDistrict = viewModel.CRPreviousAddress2CountyDistrict;
                    induction.CRPreviousAddress2Postcode = viewModel.CRPreviousAddress2Postcode;
                    induction.CRPreviousAddress2DateMovedIn = viewModel.CRPreviousAddress2DateMovedIn;
                    induction.CRPreviousAddress2Country = viewModel.CRPreviousAddress2Country;
                    //
                    induction.CRPreviousAddress3NumberStreet = viewModel.CRPreviousAddress3NumberStreet;
                    induction.CRPreviousAddress3TownCity = viewModel.CRPreviousAddress3TownCity;
                    induction.CRPreviousAddress3CountyDistrict = viewModel.CRPreviousAddress3CountyDistrict;
                    induction.CRPreviousAddress3Postcode = viewModel.CRPreviousAddress3Postcode;
                    induction.CRPreviousAddress3DateMovedIn = viewModel.CRPreviousAddress3DateMovedIn;
                    induction.CRPreviousAddress3Country = viewModel.CRPreviousAddress3Country;
                    //
                    induction.CRPreviousAddress4NumberStreet = viewModel.CRPreviousAddress4NumberStreet;
                    induction.CRPreviousAddress4TownCity = viewModel.CRPreviousAddress4TownCity;
                    induction.CRPreviousAddress4CountyDistrict = viewModel.CRPreviousAddress4CountyDistrict;
                    induction.CRPreviousAddress4Postcode = viewModel.CRPreviousAddress4Postcode;
                    induction.CRPreviousAddress4DateMovedIn = viewModel.CRPreviousAddress4DateMovedIn;
                    induction.CRPreviousAddress4Country = viewModel.CRPreviousAddress4Country;
                    //
                    induction.CRPreviousAddress5NumberStreet = viewModel.CRPreviousAddress5NumberStreet;
                    induction.CRPreviousAddress5TownCity = viewModel.CRPreviousAddress5TownCity;
                    induction.CRPreviousAddress5CountyDistrict = viewModel.CRPreviousAddress5CountyDistrict;
                    induction.CRPreviousAddress5Postcode = viewModel.CRPreviousAddress5Postcode;
                    induction.CRPreviousAddress5DateMovedIn = viewModel.CRPreviousAddress5DateMovedIn;
                    induction.CRPreviousAddress5Country = viewModel.CRPreviousAddress5Country;
                    //
                    induction.CRPreviousAddress6NumberStreet = viewModel.CRPreviousAddress6NumberStreet;
                    induction.CRPreviousAddress6TownCity = viewModel.CRPreviousAddress6TownCity;
                    induction.CRPreviousAddress6CountyDistrict = viewModel.CRPreviousAddress6CountyDistrict;
                    induction.CRPreviousAddress6Postcode = viewModel.CRPreviousAddress6Postcode;
                    induction.CRPreviousAddress6DateMovedIn = viewModel.CRPreviousAddress6DateMovedIn;
                    induction.CRPreviousAddress6Country = viewModel.CRPreviousAddress6Country;
                    //
                    induction.Step4Signature = viewModel.Step4Signature;
                    induction.Step4DateSigned = viewModel.Step4DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step4 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step5", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step4 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            IEnumerable<SelectListItem> countries = db.Countries.Where(x => x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = false,
            });

            ViewBag.Countries = countries;

            return View(viewModel);
        }









        // step 5

        public ActionResult Step5(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            var viewModel = new InductionStep5ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                //
                Step5Signature = induction.Step5Signature,
                Step5DateSigned = induction.Step5DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step5(InductionStep5ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.Step5Signature = viewModel.Step5Signature;
                    induction.Step5DateSigned = viewModel.Step5DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step5 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step6", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step5 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            return View(viewModel);
        }









        // step 6

        public ActionResult Step6(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            var viewModel = new InductionStep6ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                //
                PhysicallyFit = induction.PhysicallyFit,
                NotTakingMedication = induction.NotTakingMedication,
                NoMedicalConditions = induction.NoMedicalConditions,
                ComplyWithGuidelines = induction.ComplyWithGuidelines,
                NoMentalOrPhysicalConditions = induction.NoMentalOrPhysicalConditions,
                InformSblChanges = induction.InformSblChanges,
                //
                Step6Signature = induction.Step6Signature,
                Step6DateSigned = induction.Step6DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step6(InductionStep6ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.PhysicallyFit = viewModel.PhysicallyFit;
                    induction.NotTakingMedication = viewModel.NotTakingMedication;
                    induction.NoMedicalConditions = viewModel.NoMedicalConditions;
                    induction.ComplyWithGuidelines = viewModel.ComplyWithGuidelines;
                    induction.NoMentalOrPhysicalConditions = viewModel.NoMentalOrPhysicalConditions;
                    induction.InformSblChanges = viewModel.InformSblChanges;
                    //
                    induction.Step6Signature = viewModel.Step6Signature;
                    induction.Step6DateSigned = viewModel.Step6DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step6 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step7", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step6 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            return View(viewModel);
        }











        // step 7

        public ActionResult Step7(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            var viewModel = new InductionStep7ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                //
                Step7Signature = induction.Step7Signature,
                Step7DateSigned = induction.Step7DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step7(InductionStep7ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.Step7Signature = viewModel.Step7Signature;
                    induction.Step7DateSigned = viewModel.Step7DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step7 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step8", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step7 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            return View(viewModel);
        }










        // step 8

        public ActionResult Step8(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            var viewModel = new InductionStep8ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                //
                Step8Signature = induction.Step8Signature,
                Step8DateSigned = induction.Step8DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step8(InductionStep8ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.Step8Signature = viewModel.Step8Signature;
                    induction.Step8DateSigned = viewModel.Step8DateSigned;
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step8 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("step9", new { guidid = viewModel.GuidId });
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step8 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            return View(viewModel);
        }









        // step 9

        public ActionResult Step9(string guidid)
        {
            if (!db.Inductions.Where(x => x.GuidId == guidid).Any())
            {
                return RedirectToAction("index");
            }

            var induction = db.Inductions.Where(x => x.GuidId == guidid).FirstOrDefault();

            var viewModel = new InductionStep9ViewModel
            {
                Id = induction.Id,
                GuidId = induction.GuidId,
                FullName = induction.FullName,
                //
                Step9Signature = induction.Step9Signature,
                Step9DateSigned = induction.Step9DateSigned
            };

            if (viewModel == null)
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }


        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step9(InductionStep9ViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var induction = db.Inductions.Where(x => x.Id == viewModel.Id).FirstOrDefault();

                    // db
                    induction.Step9Signature = viewModel.Step9Signature;
                    induction.Step9DateSigned = viewModel.Step9DateSigned;
                    //
                    induction.Status = "Pending";
                    induction.Active = true;
                    //
                    db.SaveChanges();

                    // log
                    Helpers.Logging.LogEntry("Induction: Step9 - InductionId: " + viewModel.Id, "", false);

                    return RedirectToAction("thankyou");
                }
                catch (Exception ex)
                {
                    // log
                    Helpers.Logging.LogEntry("Induction: Step9 - InductionId: " + viewModel.Id, ex.Message, true);
                }
            }

            return View(viewModel);
        }




        public ActionResult ThankYou()
        {
            return View();
        }






    }
}