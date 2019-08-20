using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InductionStep4ViewModel
    {
        public int Id { get; set; }

        public string GuidId { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        //

        [Required]
        [Display(Name = "Number/Street")]
        public string CRCurrentAddressNumberStreet { get; set; }


        [Required]
        [Display(Name = "Town/City")]
        public string CRCurrentAddressTownCity { get; set; }


        [Required]
        [Display(Name = "County/District")]
        public string CRCurrentAddressCountyDistrict { get; set; }


        [Required]
        [Display(Name = "Postcode")]
        public string CRCurrentAddressPostcode { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRCurrentAddressDateMovedIn { get; set; }


        [Required]
        [Display(Name = "Country")]
        public string CRCurrentAddressCountry { get; set; }



        [Display(Name = "Number/Street")]
        public string CRPreviousAddress1NumberStreet { get; set; }


        [Display(Name = "Town/City")]
        public string CRPreviousAddress1TownCity { get; set; }


        [Display(Name = "County/District")]
        public string CRPreviousAddress1CountyDistrict { get; set; }


        [Display(Name = "Postcode")]
        public string CRPreviousAddress1Postcode { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRPreviousAddress1DateMovedIn { get; set; }


        [Display(Name = "Country")]
        public string CRPreviousAddress1Country { get; set; }



        [Display(Name = "Number/Street")]
        public string CRPreviousAddress2NumberStreet { get; set; }


        [Display(Name = "Town/City")]
        public string CRPreviousAddress2TownCity { get; set; }


        [Display(Name = "County/District")]
        public string CRPreviousAddress2CountyDistrict { get; set; }


        [Display(Name = "Postcode")]
        public string CRPreviousAddress2Postcode { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRPreviousAddress2DateMovedIn { get; set; }


        [Display(Name = "Country")]
        public string CRPreviousAddress2Country { get; set; }



        [Display(Name = "Number/Street")]
        public string CRPreviousAddress3NumberStreet { get; set; }


        [Display(Name = "Town/City")]
        public string CRPreviousAddress3TownCity { get; set; }


        [Display(Name = "County/District")]
        public string CRPreviousAddress3CountyDistrict { get; set; }


        [Display(Name = "Postcode")]
        public string CRPreviousAddress3Postcode { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRPreviousAddress3DateMovedIn { get; set; }


        [Display(Name = "Country")]
        public string CRPreviousAddress3Country { get; set; }



        [Display(Name = "Number/Street")]
        public string CRPreviousAddress4NumberStreet { get; set; }


        [Display(Name = "Town/City")]
        public string CRPreviousAddress4TownCity { get; set; }


        [Display(Name = "County/District")]
        public string CRPreviousAddress4CountyDistrict { get; set; }


        [Display(Name = "Postcode")]
        public string CRPreviousAddress4Postcode { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRPreviousAddress4DateMovedIn { get; set; }


        [Display(Name = "Country")]
        public string CRPreviousAddress4Country { get; set; }



        [Display(Name = "Number/Street")]
        public string CRPreviousAddress5NumberStreet { get; set; }


        [Display(Name = "Town/City")]
        public string CRPreviousAddress5TownCity { get; set; }


        [Display(Name = "County/District")]
        public string CRPreviousAddress5CountyDistrict { get; set; }


        [Display(Name = "Postcode")]
        public string CRPreviousAddress5Postcode { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRPreviousAddress5DateMovedIn { get; set; }


        [Display(Name = "Country")]
        public string CRPreviousAddress5Country { get; set; }



        [Display(Name = "Number/Street")]
        public string CRPreviousAddress6NumberStreet { get; set; }


        [Display(Name = "Town/City")]
        public string CRPreviousAddress6TownCity { get; set; }


        [Display(Name = "County/District")]
        public string CRPreviousAddress6CountyDistrict { get; set; }


        [Display(Name = "Postcode")]
        public string CRPreviousAddress6Postcode { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRPreviousAddress6DateMovedIn { get; set; }


        [Display(Name = "Country")]
        public string CRPreviousAddress6Country { get; set; }



        [Required(ErrorMessage = "Please sign your name to proceed")]
        [Display(Name = "Signature")]
        public string Step4Signature { get; set; }


        [Required(ErrorMessage = "Please add date to proceed")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step4DateSigned { get; set; }



    }
}