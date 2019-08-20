using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Associate
    {
        public int Id { get; set; }

        //

        [Display(Name = "Depot")]
        public int? DepotId { get; set; }


        public virtual Depot Depot { get; set; }

        //

        public virtual ICollection<Accident> Accidents { get; set; }

        public virtual ICollection<AssociateFile> AssociateFiles { get; set; }

        public virtual ICollection<AssociateReceipt> AssociateReceipts { get; set; }

        public virtual ICollection<AssociateRemittance> AssociateRemittances { get; set; }

        public virtual ICollection<Charge> Charges { get; set; }

        public virtual ICollection<ChargePcn> ChargePcns { get; set; }

        public virtual ICollection<Inspection> Inspections { get; set; }

        public virtual ICollection<RouteAllocation> RouteAllocations { get; set; }

        public virtual ICollection<SubRental> SubRentals { get; set; }

        //

        [Display(Name = "Induction")]
        public int? InductionId { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; }

        //

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Position")]
        public string Position { get; set; }


        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }


        [Required]
        [Display(Name = "City")]
        public string City { get; set; }


        [Required]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }


        [Required]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }


        [Display(Name = "Nationality")]
        public string Nationality { get; set; }


        [Display(Name = "National Insurance Number")]
        public string NationalInsuranceNumber { get; set; }


        [Display(Name = "UTR Number")]
        public string UTRNumber { get; set; }


        [Required]
        [Display(Name = "Next of Kin Name")]
        public string NextOfKinName { get; set; }


        [Required]
        [Display(Name = "Next of Kin Relationship")]
        public string NextOfKinRelationship { get; set; }


        [Required]
        [Display(Name = "Next of Kin Mobile")]
        public string NextOfKinMobile { get; set; }

        //

        [Display(Name = "Name of the Bank")]
        public string NameOfTheBank { get; set; }


        [Display(Name = "Sort Code")]
        public string SortCode { get; set; }


        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }


        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        //

        [Display(Name = "Photo")]
        public byte[] DataPhoto { get; set; }

        [Display(Name = "Photo Content Type")]
        public string DataPhotoContentType { get; set; }

        [Display(Name = "Photo Name")]
        public string DataPhotoName { get; set; }


        [Display(Name = "Driving Licence")]
        public byte[] DataDrivingLicence { get; set; }

        [Display(Name = "Driving Licence Content Type")]
        public string DataDrivingLicenceContentType { get; set; }

        [Display(Name = "Driving Licence Name")]
        public string DataDrivingLicenceName { get; set; }


        [Display(Name = "ID/Passport")]
        public byte[] DataPassport { get; set; }

        [Display(Name = "Passport Content Type")]
        public string DataPassportContentType { get; set; }

        [Display(Name = "Passport Name")]
        public string DataPassportName { get; set; }


        [Display(Name = "Proof of Address")]
        public byte[] DataProofOfAddress { get; set; }

        [Display(Name = "Proof of Address Content Type")]
        public string DataProofOfAddressContentType { get; set; }

        [Display(Name = "Proof of Address Name")]
        public string DataProofOfAddressName { get; set; }


        [Display(Name = "Nino")]
        public byte[] DataNino { get; set; }

        [Display(Name = "Nino Content Type")]
        public string DataNinoContentType { get; set; }

        [Display(Name = "Nino Name")]
        public string DataNinoName { get; set; }

        //

        [Display(Name = "Bio")]
        public string Bio { get; set; }

        //

        [Required]
        [Display(Name = "Application Status")]
        public string ApplicationStatus { get; set; }


        [Required]
        [Display(Name = "Associate Status")]
        public string AssociateStatus { get; set; }


        [Display(Name = "Is Employed")]
        public bool IsEmployed { get; set; }


        [Display(Name = "Is Third Party")]
        public bool IsThirdParty { get; set; }


        [Display(Name = "Own Vehicle")]
        public bool OwnVehicle { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }


        [Display(Name = "Induction Completed")]
        public bool InductionCompleted { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Induction Completed")]
        public DateTime? InductionCompletedDate { get; set; }


        [Display(Name = "First Ride Along")]
        public bool FirstRideAlong { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date First Ride Along")]
        public DateTime? FirstRideAlongDate { get; set; }


        [Display(Name = "Second Ride Along")]
        public bool SecondRideAlong { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Second Ride Along")]
        public DateTime? SecondRideAlongDate { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }


        [Display(Name = "Active")]
        public bool Active { get; set; }


        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }
    }
}