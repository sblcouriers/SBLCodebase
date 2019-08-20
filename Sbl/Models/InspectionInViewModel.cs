using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InspectionInViewModel
    {
        public int Id { get; set; }

        //

        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date In")]
        public DateTime? DateIn { get; set; }


        [Display(Name = "Odometer In")]
        public string OdometerIn { get; set; }


        [Display(Name = "Fuel Level In")]
        public string FuelLevelIn { get; set; }

        //

        [Display(Name = "Condition Side 1 In")]
        public string ConditionSide1In { get; set; }


        [Display(Name = "Condition Side 2 In")]
        public string ConditionSide2In { get; set; }


        [Display(Name = "Condition Front In")]
        public string ConditionFrontIn { get; set; }


        [Display(Name = "Condition Back In")]
        public string ConditionBackIn { get; set; }


        [Display(Name = "Condition Windshield In")]
        public string ConditionWindshieldIn { get; set; }

        //

        [Display(Name = "Additional Damage Comments In")]
        public string AdditionalDamageCommentsIn { get; set; }

        //

        [Display(Name = "N/S mm (1)")]
        public string TyreInNSMM1 { get; set; }


        [Display(Name = "N/S psi (1)")]
        public string TyreInNSPSI1 { get; set; }


        [Display(Name = "N/S Make (1)")]
        public string TyreInNSMake1 { get; set; }

        //

        [Display(Name = "N/S mm (2)")]
        public string TyreInNSMM2 { get; set; }


        [Display(Name = "N/S psi (2)")]
        public string TyreInNSPSI2 { get; set; }


        [Display(Name = "N/S Make (2)")]
        public string TyreInNSMake2 { get; set; }

        //

        [Display(Name = "O/S mm (1)")]
        public string TyreInOSMM1 { get; set; }


        [Display(Name = "O/S psi (1)")]
        public string TyreInOSPSI1 { get; set; }


        [Display(Name = "O/S Make (1)")]
        public string TyreInOSMake1 { get; set; }

        //

        [Display(Name = "O/S mm (2)")]
        public string TyreInOSMM2 { get; set; }


        [Display(Name = "O/S psi (2)")]
        public string TyreInOSPSI2 { get; set; }


        [Display(Name = "O/S Make (2)")]
        public string TyreInOSMake2 { get; set; }

        //

        [Display(Name = "Spare Type mm")]
        public string TyreInSpareTyreMM { get; set; }


        [Display(Name = "Spare Tyre psi")]
        public string TyreInSpareTyrePSI { get; set; }


        [Display(Name = "Spare Tyre Make")]
        public string TyreInSpareTyreMake { get; set; }

        //

        [Display(Name = "Lights/indicators/horn")]
        public bool LightsIndicatorsHornIn { get; set; }


        [Display(Name = "Windscreen wipers/washer")]
        public bool WindscreenWipersWasherIn { get; set; }


        [Display(Name = "Radio")]
        public bool RadioIn { get; set; }


        [Display(Name = "Drivers handbook")]
        public bool DriversHandbookIn { get; set; }


        [Display(Name = "Coolant level")]
        public bool CoolantLevelIn { get; set; }


        [Display(Name = "Engine oil level")]
        public bool EngineOilLevelIn { get; set; }


        [Display(Name = "Brake fluid level")]
        public bool BrakeFluidLevelIn { get; set; }


        [Display(Name = "Tyre pressures checked")]
        public bool TyrePressuresCheckedIn { get; set; }


        [Display(Name = "Seatbelt condition and operation")]
        public bool SeatbeltConditionAndOperationIn { get; set; }


        [Display(Name = "Cigarette lighter")]
        public bool CigaretteLighterIn { get; set; }


        [Display(Name = "Jack and tools present")]
        public bool JackAndToolsPresentIn { get; set; }


        [Display(Name = "Handbrake operation")]
        public bool HandbrakeOperationIn { get; set; }


        [Display(Name = "Internal condition / cleanliness")]
        public bool InternalConditionCleanlinessIn { get; set; }

        //

        [Display(Name = "Signature SBL")]
        public string SignatureSblIn { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed SBL")]
        public DateTime? DateSignedSblIn { get; set; }

        //

        [Display(Name = "Signature Associate")]
        public string SignatureAssociateIn { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed Associate")]
        public DateTime? DateSignedAssociateIn { get; set; }

    }
}