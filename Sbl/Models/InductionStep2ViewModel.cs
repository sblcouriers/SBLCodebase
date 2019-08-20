using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InductionStep2ViewModel
    {
        public int Id { get; set; }

        public string GuidId { get; set; }

        public string FullName { get; set; }

        //

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }


        [Required]
        [Display(Name = "National Insurance Number")]
        public string NationalInsuranceNumber { get; set; }


        [Required]
        [Display(Name = "Nationality")]
        public string Nationality { get; set; }


        [Required]
        [Display(Name = "UTR Number")]
        public string UTRNumber { get; set; }


        [Required]
        [Display(Name = "Own Vehicle")]
        public bool OwnVehicle { get; set; }


        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Vehicle Insurance Renewal Date")]
        public DateTime? VehicleInsuranceRenewalDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Vehicle MOT Due")]
        public DateTime? VehicleMOTDue { get; set; }


        [Required]
        [Display(Name = "Name of the Bank")]
        public string NameOfTheBank { get; set; }


        [Required]
        [Display(Name = "Sort Code")]
        public string SortCode { get; set; }


        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }


        [Required]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }


        [Required(ErrorMessage = "Please sign your name to proceed")]
        [Display(Name = "Signature")]
        public string Step2Signature { get; set; }


        [Required(ErrorMessage = "Please add date to proceed")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step2DateSigned { get; set; }

    }
}