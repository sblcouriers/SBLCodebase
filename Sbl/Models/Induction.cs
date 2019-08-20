using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Induction
    {
        public int Id { get; set; }

        public string GuidId { get; set; }


        //

        public virtual ICollection<InductionNote> InductionNotes { get; set; }

        //


        // step 1

        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }


        [Display(Name = "Email")]
        public string Email { get; set; }


        [Display(Name = "Next of Kin Name")]
        public string NextOfKinName { get; set; }


        [Display(Name = "Next of Kin Relationship")]
        public string NextOfKinRelationship { get; set; }


        [Display(Name = "Next of Kin Mobile")]
        public string NextOfKinMobile { get; set; }


        [Display(Name = "Address")]
        public string Address { get; set; }


        [Display(Name = "City")]
        public string City { get; set; }


        [Display(Name = "County")]
        public string County { get; set; }


        [Display(Name = "Postcode")]
        public string Postcode { get; set; }


        [Display(Name = "Country")]
        public string Country { get; set; }


        [Display(Name = "Signature")]
        public string Step1Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step1DateSigned { get; set; }



        // step 2

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }


        [Display(Name = "National Insurance Number")]
        public string NationalInsuranceNumber { get; set; }


        [Display(Name = "Nationality")]
        public string Nationality { get; set; }


        [Display(Name = "UTR Number")]
        public string UTRNumber { get; set; }


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


        [Display(Name = "Name of the Bank")]
        public string NameOfTheBank { get; set; }


        [Display(Name = "Sort Code")]
        public string SortCode { get; set; }


        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }


        [Display(Name = "Account Name")]
        public string AccountName { get; set; }


        [Display(Name = "Signature")]
        public string Step2Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step2DateSigned { get; set; }



        // step 3

        [Display(Name = "Do you have any unspent criminal convictions?")]
        public bool UnspentCriminalConviction { get; set; }


        [Display(Name = "Criminal Conviction Details")]
        public string CriminalConvictionDetails { get; set; }


        [Display(Name = "Title")]
        public string CRTitle { get; set; }


        [Display(Name = "Criminal Conviction Details")]
        public string CRSurname { get; set; }


        [Display(Name = "Criminal Conviction Details")]
        public string CRForename { get; set; }


        [Display(Name = "Mother's Surname at Birth")]
        public string CRMotherSurnameBirth { get; set; }


        [Display(Name = "Mother's Forename at Birth")]
        public string CRMotherForenameBirth { get; set; }


        [Display(Name = "Maiden/Family Name if married")]
        public string CRMaidenFamilyName { get; set; }


        [Display(Name = "Other Name (1)")]
        public string CROtherName1 { get; set; }


        [Display(Name = "Other Name (2)")]
        public string CROtherName2 { get; set; }


        [Display(Name = "Other Name (3)")]
        public string CROtherName3 { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime? CRDateOfBirth { get; set; }


        [Display(Name = "Gender")]
        public string CRGender { get; set; }


        [Display(Name = "Town of Birth")]
        public string CRTownOfBirth { get; set; }


        [Display(Name = "Country of Birth")]
        public string CRCountryOfBirth { get; set; }


        [Display(Name = "Passport Number")]
        public string CRPassportNumber { get; set; }


        [Display(Name = "UK Passport")]
        public string CRIsUKPassport { get; set; }


        [Display(Name = "Country of Issue")]
        public string CRPassportCountryOfIssue { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Issue Date")]
        public DateTime? CRPassportIssueDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiry Date")]
        public DateTime? CRPassportExpiryDate { get; set; }


        [Display(Name = "National Insurance Number")]
        public string CRNationalInsuranceNumber { get; set; }


        [Display(Name = "Driver License")]
        public string CRDriverLicense { get; set; }


        [Display(Name = "Country of Issue")]
        public string CRDriverLicenseCountryOfIssue { get; set; }


        [Display(Name = "Signature")]
        public string Step3Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step3DateSigned { get; set; }



        // step 4

        [Display(Name = "Number/Street")]
        public string CRCurrentAddressNumberStreet { get; set; }


        [Display(Name = "Town/City")]
        public string CRCurrentAddressTownCity { get; set; }


        [Display(Name = "County/District")]
        public string CRCurrentAddressCountyDistrict { get; set; }


        [Display(Name = "Postcode")]
        public string CRCurrentAddressPostcode { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Moved In")]
        public DateTime? CRCurrentAddressDateMovedIn { get; set; }


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


        [Display(Name = "Signature")]
        public string Step4Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step4DateSigned { get; set; }



        // step 5

        [Display(Name = "Signature")]
        public string Step5Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step5DateSigned { get; set; }


        
        // step 6

        [Display(Name = "I am physically fit and able to fulfil the demands of a multi-drop courier")]
        public bool PhysicallyFit { get; set; }


        [Display(Name = "I am not taking any medication that will hinder my ability to drive or manipulate consignments in a safe manner")]
        public bool NotTakingMedication { get; set; }


        [Display(Name = "I have no medical conditions that will hinder my ability to drive or manipulate consignments in a safe manner")]
        public bool NoMedicalConditions { get; set; }


        [Display(Name = "I will comply with all manual handling guidelines and not place myself in Danger by using unsafe working practices")]
        public bool ComplyWithGuidelines { get; set; }


        [Display(Name = "I have no mental or physical conditions that not be reported to the DVLA which could affect my ability to drive safely")]
        public bool NoMentalOrPhysicalConditions { get; set; }


        [Display(Name = "I will inform SBL if my ability to safely drive or handle consignments changes in the future")]
        public bool InformSblChanges { get; set; }


        [Display(Name = "Signature")]
        public string Step6Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step6DateSigned { get; set; }



        // step 7

        [Display(Name = "Signature")]
        public string Step7Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step7DateSigned { get; set; }



        // step 8

        [Display(Name = "Signature")]
        public string Step8Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step8DateSigned { get; set; }



        // step 9

        [Display(Name = "Signature")]
        public string Step9Signature { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step9DateSigned { get; set; }



        //

        [Display(Name = "Status")]
        public string Status { get; set; }

        //







        //

        public byte[] DataFileSignature { get; set; }
        public string DataFileContentTypeSignature { get; set; }
        public string DataFileNameSignature { get; set; }
        public string DataFileExtensionSignature { get; set; }


        public byte[] DataFileDrivingLicense { get; set; }
        public string DataFileContentTypeDrivingLicense { get; set; }
        public string DataFileNameDrivingLicense { get; set; }
        public string DataFileExtensionDrivingLicense { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiryDateDrivingLicense { get; set; }
        public bool IsValidDrivingLicense { get; set; }


        public byte[] DataFileProofId { get; set; }
        public string DataFileContentTypeProofId { get; set; }
        public string DataFileNameProofId { get; set; }
        public string DataFileExtensionProofId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiryDateProofId { get; set; }
        public bool IsValidProofId { get; set; }


        public byte[] DataFileNationalInsuranceNumber { get; set; }
        public string DataFileContentTypeNationalInsuranceNumber { get; set; }
        public string DataFileNameNationalInsuranceNumber { get; set; }
        public string DataFileExtensionNationalInsuranceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public bool IsValidNationalInsuranceNumber { get; set; }


        public byte[] DataFileProofAddress { get; set; }
        public string DataFileContentTypeProofAddress { get; set; }
        public string DataFileNameProofAddress { get; set; }
        public string DataFileExtensionProofAddress { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiryDateProofAddress { get; set; }
        public bool IsValidProofAddress { get; set; }


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