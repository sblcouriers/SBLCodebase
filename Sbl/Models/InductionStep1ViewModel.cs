using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InductionStep1ViewModel
    {
        public int Id { get; set; }

        public string GuidId { get; set; }

        public List<string> CountryList { get; set; }

        //

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }


        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Next of Kin Name")]
        public string NextOfKinName { get; set; }


        [Required]
        [Display(Name = "Next of Kin Relationship")]
        public string NextOfKinRelationship { get; set; }


        [Required]
        [Display(Name = "Next of Kin Mobile")]
        public string NextOfKinMobile { get; set; }


        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }


        [Required]
        [Display(Name = "City")]
        public string City { get; set; }


        [Display(Name = "County")]
        public string County { get; set; }


        [Required]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }


        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        //

        [Required(ErrorMessage = "Please sign your name to proceed")]
        [Display(Name = "Signature")]
        public string Step1Signature { get; set; }


        [Required(ErrorMessage = "Please add date to proceed")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step1DateSigned { get; set; }

    }
}