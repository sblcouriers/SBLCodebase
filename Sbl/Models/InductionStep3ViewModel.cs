using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InductionStep3ViewModel
    {
        public int Id { get; set; }

        public string GuidId { get; set; }

        public string FullName { get; set; }

        //

        [Required]
        [Display(Name = "Do you have any unspent criminal convictions?")]
        public bool UnspentCriminalConviction { get; set; }


        [Display(Name = "Criminal Conviction Details")]
        public string CriminalConvictionDetails { get; set; }


        [Required]
        [Display(Name = "Title")]
        public string CRTitle { get; set; }


        [Required]
        [Display(Name = "Surname")]
        public string CRSurname { get; set; }


        [Required]
        [Display(Name = "Forename")]
        public string CRForename { get; set; }


        [Required]
        [Display(Name = "Mother's Surname at Birth")]
        public string CRMotherSurnameBirth { get; set; }


        [Required]
        [Display(Name = "Mother's Forename at Birth")]
        public string CRMotherForenameBirth { get; set; }


        [Required]
        [Display(Name = "Maiden/Family Name if married")]
        public string CRMaidenFamilyName { get; set; }


        [Display(Name = "Other Name (1)")]
        public string CROtherName1 { get; set; }


        [Display(Name = "Other Name (2)")]
        public string CROtherName2 { get; set; }


        [Display(Name = "Other Name (3)")]
        public string CROtherName3 { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime? CRDateOfBirth { get; set; }


        [Required]
        [Display(Name = "Gender")]
        public string CRGender { get; set; }


        [Required]
        [Display(Name = "Town of Birth")]
        public string CRTownOfBirth { get; set; }


        [Required]
        [Display(Name = "Country of Birth")]
        public string CRCountryOfBirth { get; set; }


        [Required]
        [Display(Name = "Passport Number")]
        public string CRPassportNumber { get; set; }


        [Required]
        [Display(Name = "UK Passport")]
        public string CRIsUKPassport { get; set; }


        [Required]
        [Display(Name = "Country of Issue")]
        public string CRPassportCountryOfIssue { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Issue Date")]
        public DateTime? CRPassportIssueDate { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiry Date")]
        public DateTime? CRPassportExpiryDate { get; set; }


        [Required]
        [Display(Name = "National Insurance Number")]
        public string CRNationalInsuranceNumber { get; set; }


        [Required]
        [Display(Name = "Driver License")]
        public string CRDriverLicense { get; set; }


        [Required]
        [Display(Name = "Country of Issue")]
        public string CRDriverLicenseCountryOfIssue { get; set; }


        [Required(ErrorMessage = "Please sign your name to proceed")]
        [Display(Name = "Signature")]
        public string Step3Signature { get; set; }


        [Required(ErrorMessage = "Please add date to proceed")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step3DateSigned { get; set; }

    }
}