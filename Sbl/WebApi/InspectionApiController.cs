using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Sbl.Helpers;
using Sbl.Models;
using Sbl.Models.BO;
namespace Sbl.WebApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class InspectionApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region Find Inspections List
        /// <summary>
        /// Find Inspections List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // api/<controller>/FindInspections
        public IHttpActionResult FindInspectionList(int pageNo = 1, int pageLength = 15, string searchQuery = null)
        {
            var inspectionList = new List<ClsInspection>();
            var inspectionModel = new InspectionModel();
            inspectionModel.PageNo = pageNo;
            inspectionModel.PageLength = pageLength;

            int startIndex = (pageNo - 1) * pageLength;
            var inspections = new List<Inspection>();
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                inspectionModel.TotalRecords = db.Inspections.Count(x => x.Active == true && x.Deleted == false && x.Associate.Name.Contains(searchQuery));
                inspections = db.Inspections.Where(x => x.Active && x.Deleted == false && x.Associate.Name.Contains(searchQuery)).OrderByDescending(inspection => inspection.Id)
                       .Skip(startIndex)
                       .Take(inspectionModel.PageLength).ToList();
            }
            else
            {
                inspectionModel.TotalRecords = db.Inspections.Count(x => x.Active == true && x.Deleted == false);
                inspections = db.Inspections.Where(x => x.Active && x.Deleted == false).OrderByDescending(inspection => inspection.Id)
                       .Skip(startIndex)
                       .Take(inspectionModel.PageLength).ToList();
            }
            foreach (var inspection in inspections)
            {
                var objInspection = new ClsInspection();
                objInspection.AssociateName = inspection.Associate.Name;
                objInspection.AssociateEmail = inspection.Associate.Email;
                objInspection.VehicleName = string.Format("{0} - {1} - {2}", inspection.Vehicle.Make, inspection.Vehicle.Model, inspection.Vehicle.Registration);
                objInspection.VehicleId = inspection.VehicleId;
                objInspection.AssociateId = inspection.AssociateId;
                objInspection.Id = inspection.Id;
                objInspection.InspectionDueDate = inspection.InspectionDueDate;
                inspectionList.Add(objInspection);
            }

            inspectionModel.InspectionList = inspectionList;


            return Ok(inspectionModel);
        }
        #endregion

        #region Find Inspections List
        /// <summary>
        /// Find Inspections List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // api/<controller>/FindInspections
        public IHttpActionResult FindInspections()
        {
            var inspections = db.Inspections.Where(x => x.Active == true && x.Deleted == false).ToList();
            var inspectionList = new List<ClsInspection>();
            foreach (var inspection in inspections)
            {
                var objInspection = new ClsInspection();
                objInspection.AssociateName = inspection.Associate.Name;
                objInspection.AssociateEmail = inspection.Associate.Email;
                objInspection.VehicleName = string.Format("{0} - {1} - {2}", inspection.Vehicle.Make, inspection.Vehicle.Model, inspection.Vehicle.Registration);
                objInspection.VehicleId = inspection.VehicleId;
                objInspection.AssociateId = inspection.AssociateId;
                objInspection.Id = inspection.Id;
                objInspection.InspectionDueDate = inspection.InspectionDueDate;
                inspectionList.Add(objInspection);
            }
            if (inspectionList.Count <= 0)
            {
                return NotFound();
            }
            return Ok(inspectionList);
        }
        #endregion

        #region Find Inspection Detail
        /// <summary>
        ///  Find Inspection Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Inspection))]
        // api/<controller>/FindInspection/1
        [HttpGet]
        public IHttpActionResult FindInspection(int id)
        {
            var inspection = db.Inspections.FirstOrDefault(x => x.Active == true && x.Deleted == false && x.Id == id);
            if (inspection == null)
            {
                return NotFound();
            }

            return Ok(inspection);
        }
        #endregion

        #region Add Inspection
        /// <summary>
        /// Add Inspection
        /// </summary>
        /// <param name="inspection"></param>
        /// <returns></returns>
        // api/<controller>/AddInspection/inspection object
        [HttpPost]
        [ResponseType(typeof(ResponseOutput))]
        public ResponseOutput AddInspection(Inspection inspection)
        {
            var responseOutput = new ResponseOutput();
            responseOutput.IsSuccess = false;
            int inspectionId = 0;
            try
            {
                inspection.DateCreated = DateTime.Now;
                inspection.Active = true;
                inspection.Deleted = false;
                db.Inspections.Add(inspection);
                db.SaveChanges();
                inspectionId = inspection.Id;
                // update vehicle status
                var firstOrDefault = db.Vehicles.FirstOrDefault(x => x.Id == inspection.VehicleId);
                if (firstOrDefault != null)
                {
                    var vehicle = firstOrDefault.Status = "Rented";
                    db.SaveChanges();
                }
                responseOutput.ErrorMessage = "";
                responseOutput.IsSuccess = true;
                responseOutput.Id = inspectionId;
            }
            catch (Exception exception)
            {
                responseOutput.ErrorMessage = exception.Message;
                responseOutput.IsSuccess = false;
                responseOutput.Id = inspectionId;
            }
            return responseOutput;
        }
        #endregion

        #region Update Inspections
        /// <summary>
        ///  Update Inspections
        /// </summary>
        /// <param name="inspection"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(ResponseOutput))]
        public ResponseOutput UpdateInspection(Inspection inspection)
        {
            var responseOutput = new ResponseOutput();
            responseOutput.IsSuccess = false;
            int inspectionId = 0;
            if (inspection != null && inspection.Id > 0)
            {
                try
                {
                    var original = db.Inspections.FirstOrDefault(x => x.Id == inspection.Id);

                    if (original != null)
                    {
                        inspectionId = original.Id;
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

                        original.SignatureAssociateOut = inspection.SignatureAssociateOut;
                        original.SignatureSblOut = inspection.SignatureSblOut;

                        original.Active = original.Active;
                        original.DateCreated = original.DateCreated;
                        original.Deleted = original.Deleted;
                    }
                    db.SaveChanges();

                    // update vehicle status
                    var firstOrDefault = db.Vehicles.FirstOrDefault(x => x.Id == inspection.VehicleId);
                    if (firstOrDefault != null)
                    {
                        var vehicle = firstOrDefault.Status = "Rented";
                        db.SaveChanges();
                    }
                    responseOutput.ErrorMessage = "";
                    responseOutput.IsSuccess = true;
                    responseOutput.Id = inspectionId;

                }
                catch (Exception ex)
                {
                    responseOutput.ErrorMessage = ex.Message;
                    responseOutput.IsSuccess = false;
                    responseOutput.Id = inspectionId;
                }
            }
            else
            {
                responseOutput.ErrorMessage = "Object did not have the data";
            }
            return responseOutput;
        }
        #endregion

        #region Update Inspections IN
        /// <summary>
        ///   Update Inspections IN
        /// </summary>
        /// <param name="inspection"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(ResponseOutput))]
        public ResponseOutput UpdateInspectionIn(Inspection inspection)
        {
            var responseOutput = new ResponseOutput();
            responseOutput.IsSuccess = false;
            int inspectionId = 0;
            if (inspection != null && inspection.Id > 0)
            {
                try
                {
                    var original = db.Inspections.FirstOrDefault(x => x.Id == inspection.Id);

                    if (original != null)
                    {
                        inspectionId = original.Id;
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
                        original.SignatureSblIn = inspection.SignatureSblIn;
                        original.SignatureAssociateIn = inspection.SignatureAssociateIn;
                    }
                    db.SaveChanges();

                    // update vehicle status
                    var firstOrDefault = db.Vehicles.FirstOrDefault(x => x.Id == inspection.VehicleId);
                    if (firstOrDefault != null)
                    {
                        var vehicle = firstOrDefault.Status = "Available";
                        db.SaveChanges();
                    }
                    responseOutput.ErrorMessage = "";
                    responseOutput.IsSuccess = true;
                    responseOutput.Id = inspectionId;

                }
                catch (Exception ex)
                {
                    responseOutput.ErrorMessage = ex.Message;
                    responseOutput.IsSuccess = false;
                    responseOutput.Id = inspectionId;
                }
            }
            else
            {
                responseOutput.ErrorMessage = "Object did not have the data";
            }
            return responseOutput;
        }
        #endregion

        #region Add/Update Inspection
        /// <summary>
        /// Add/Update Inspection
        /// </summary>
        /// <param name="inspection"></param>
        /// <returns></returns>
        // api/<controller>/AddInspection/inspection object
        [HttpPost]
        [ResponseType(typeof(ResponseOutput))]
        public ResponseOutput AddUpdateInspection(Inspection inspection)
        {
            var responseOutput = new ResponseOutput();
            responseOutput.IsSuccess = false;
            int inspectionId = 0;
            if (inspection != null)
            {
                try
                {
                    if (inspection.Id <= 0)
                    {
                        inspection.DateCreated = DateTime.Now;
                        inspection.Active = true;
                        inspection.Deleted = false;
                        db.Inspections.Add(inspection);
                        db.SaveChanges();
                        inspectionId = inspection.Id;
                    }
                    else
                    {
                        var original = db.Inspections.FirstOrDefault(x => x.Id == inspection.Id);
                        if (original != null)
                        {
                            inspectionId = original.Id;
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
                        }
                        db.SaveChanges();
                    }
                    if (inspectionId > 0)
                    {
                        // update vehicle status
                        var firstOrDefault = db.Vehicles.FirstOrDefault(x => x.Id == inspection.VehicleId);
                        if (firstOrDefault != null)
                        {
                            var vehicle = firstOrDefault.Status = "Rented";
                            db.SaveChanges();
                        }
                    }
                    responseOutput.ErrorMessage = "";
                    responseOutput.IsSuccess = true;
                    responseOutput.Id = inspectionId;
                }
                catch (Exception exception)
                {
                    responseOutput.ErrorMessage = exception.Message;
                    responseOutput.IsSuccess = false;
                    responseOutput.Id = inspectionId;

                }
            }
            else
            {
                responseOutput.ErrorMessage = "Object did not have the data";
            }
            return responseOutput;
        }
        #endregion

        #region Find Associates
        /// <summary>
        ///  Find Associates
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(SelectListResponse))]
        // api/<controller>/FindAssociates/
        [HttpGet]
        public IHttpActionResult FindAssociates()
        {
            List<SelectListResponse> associates = db.Associates.Where(x => x.SubRentals.Any(y => y.Status == "Rented" && y.Active == true && x.Deleted == false) && x.Active == true && x.Deleted == false).OrderBy(x => x.Name).ToList().Select(x => new SelectListResponse()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();

            if (associates.Count <= 0)
            {
                return NotFound();
            }
            return Ok(associates);
        }
        #endregion

        #region Find Tyre Type
        /// <summary>
        ///  Find Tyre Type
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(SelectListResponse))]
        // api/<controller>/FindTyreTypes/
        [HttpGet]
        public IHttpActionResult FindTyreTypes()
        {
            var tyreTypeList = DropdownListBinder.FindTyreTypeList();
            if (tyreTypeList.Count <= 0)
            {
                return NotFound();
            }
            return Ok(tyreTypeList);
        }
        #endregion

        #region Login
        [HttpPost]
        public LoginResponse Login(LoginViewModel viewModel)
        {
            var loginResponse = new LoginResponse();
            loginResponse.IsLogin = false;
            loginResponse.Email = viewModel.Email;
            loginResponse.UserName = viewModel.Email;
            loginResponse.Name = viewModel.Email;

            if ((viewModel.Email == "admin@admin.com" && viewModel.Password == "admin") || (viewModel.Email == "master@master.com" && viewModel.Password == "test")
                || (viewModel.Email == "fleet@fleet.com" && viewModel.Password == "test") || (viewModel.Email == "payroll@payroll.com" && viewModel.Password == "test")
                || (viewModel.Email == "poc@poc.com" && viewModel.Password == "test") || (viewModel.Email == "recruitment@recruitment.com" && viewModel.Password == "test"))
            {
                loginResponse.UserId = 1;
                loginResponse.AuthToken = viewModel.Email + "-Sbl";
                loginResponse.IsLogin = true;
                if (viewModel.Email == "admin@admin.com")
                {
                    loginResponse.Role = "admin";
                }
                else if (viewModel.Email == "master@master.com")
                {
                    loginResponse.Role = "master";
                }
                else if (viewModel.Email == "fleet@fleet.com")
                {
                    loginResponse.Role = "fleet";
                }
                else if (viewModel.Email == "payroll@payroll.com")
                {
                    loginResponse.Role = "payroll";
                }
                else if (viewModel.Email == "poc@poc.com")
                {
                    loginResponse.Role = "poc";
                }
                else if (viewModel.Email == "recruitment@recruitment.com")
                {
                    loginResponse.Role = "recruitment";
                }
                else
                {
                    loginResponse.Role = "fleet";
                }
            }
            return loginResponse;
        }
        #endregion

        #region Find Associate Vehicle Detail
        /// <summary>
        ///  Find Associate Vehicle Detail
        /// </summary>
        /// <param name="id">Pass the AssociateId to get the Associate Vechicle</param>
        /// <returns></returns>
        [ResponseType(typeof(VechicleResponse))]
        // api/<controller>/FindAssociateVehicle/1
        [HttpGet]
        public VechicleResponse FindAssociateVehicle(int id)
        {
            var associate = db.Associates.FirstOrDefault(x => x.Id == id);
            var vechicleResponse = new VechicleResponse();
            if (associate != null)
            {
                var firstOrDefault = associate.SubRentals.FirstOrDefault(x => x.Status == "Rented" && x.Active == true && x.Deleted == false);
                if (firstOrDefault != null)
                {
                    var vehicle = firstOrDefault.Vehicle;
                    string vehiclename = string.Format("{0} - {1} - {2}", vehicle.Make, vehicle.Model, vehicle.Registration);
                    vechicleResponse.VehicleId = vehicle.Id;
                    vechicleResponse.AssociateId = firstOrDefault.AssociateId;
                    vechicleResponse.VehicleName = vehiclename;
                }
            }
            return vechicleResponse;
        }
        #endregion

        #region Find Inspection Files
        /// <summary>
        /// Find Inspection Files
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult FindInspectionFiles(int id)
        {
            var inspectionFiles = db.InspectionFiles.Where(x => x.Active == true && x.Deleted == false && x.InspectionId == id);

            return Ok(inspectionFiles);
        }
        #endregion

        #region Find Inspection File Detail
        /// <summary>
        /// Find Inspection File Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult FindInspectionFile(int id)
        {
            var inspectionFile = db.InspectionFiles.Where(x => x.Active == true && x.Deleted == false && x.Id == id);

            return Ok(inspectionFile);
        }
        #endregion

        #region Add Update Inspection File
        /// <summary>
        ///  Add Update Inspection File
        /// </summary>
        /// <param name="inspectionFile"></param>
        /// <returns></returns>
        [ResponseType(typeof(ResponseOutput))]
        // api/<controller>/FindInspection/1
        [HttpPost]
        public ResponseOutput AddUpdateInspectionFile(InspectionFile inspectionFile)
        {
            var responseOutput = new ResponseOutput();
            responseOutput.IsSuccess = false;
            if (inspectionFile != null)
            {
                try
                {
                    if (inspectionFile.Id > 0)
                    {
                        //Specify the fields that should not be updated.
                        db.Entry(inspectionFile).Property(x => x.Active).IsModified = false;
                        db.Entry(inspectionFile).Property(x => x.Deleted).IsModified = false;
                        db.Entry(inspectionFile).Property(x => x.DateCreated).IsModified = false;
                        db.SaveChanges();
                    }
                    else
                    {
                        inspectionFile.DateCreated = DateTime.Now;
                        inspectionFile.Active = true;
                        inspectionFile.Deleted = false;
                        db.InspectionFiles.Add(inspectionFile);
                        db.SaveChanges();
                    }
                    responseOutput.ErrorMessage = "";
                    responseOutput.IsSuccess = true;
                    responseOutput.Id = inspectionFile.Id;

                }
                catch (Exception ex)
                {
                    responseOutput.ErrorMessage = ex.Message;
                    responseOutput.IsSuccess = false;
                    responseOutput.Id = inspectionFile.Id;
                }
            }
            else
            {
                responseOutput.ErrorMessage = "Object did not have the data";
            }
            return responseOutput;

        }
        #endregion

        #region Delete Inspection Files
        /// <summary>
        ///  Delete Inspection Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ResponseOutput))]
        [HttpGet]
        public ResponseOutput DeleteInspectionFile(int id)
        {
            var responseOutput = new ResponseOutput();
            responseOutput.IsSuccess = false;
            if (id > 0)
            {
                try
                {
                    var inspectionfile = db.InspectionFiles.FirstOrDefault(x => x.Id == id);
                    if (inspectionfile != null)
                    {
                        inspectionfile.Deleted = true;
                        db.SaveChanges();
                    }
                    responseOutput.ErrorMessage = "";
                    responseOutput.IsSuccess = true;
                    responseOutput.Id = id;

                }
                catch (Exception ex)
                {
                    responseOutput.ErrorMessage = ex.Message;
                    responseOutput.IsSuccess = false;
                    responseOutput.Id = 0;
                }
            }
            else
            {
                responseOutput.ErrorMessage = "Inspection file Id is 0";
            }
            return responseOutput;

        }
        #endregion

        #region Find Inspection File Description List
        /// <summary>
        ///  Find Inspection File Description List
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(SelectListResponse))]
        [HttpGet]
        public IHttpActionResult FindInspectionFileDescriptionList()
        {
            var fileDescriptionList = DropdownListBinder.FindInspectionFileDescriptionList();
            if (fileDescriptionList.Count <= 0)
            {
                return NotFound();
            }
            return Ok(fileDescriptionList);
        }
        #endregion
    }//end controller
}//end namespace