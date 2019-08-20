using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InductionStep6ViewModel
    {
        public int Id { get; set; }

        public string GuidId { get; set; }

        public string FullName { get; set; }

        //

        [Required]
        [Display(Name = "I am physically fit and able to fulfil the demands of a multi-drop courier")]
        public bool PhysicallyFit { get; set; }


        [Required]
        [Display(Name = "I am not taking any medication that will hinder my ability to drive or manipulate consignments in a safe manner")]
        public bool NotTakingMedication { get; set; }


        [Required]
        [Display(Name = "I have no medical conditions that will hinder my ability to drive or manipulate consignments in a safe manner")]
        public bool NoMedicalConditions { get; set; }


        [Required]
        [Display(Name = "I will comply with all manual handling guidelines and not place myself in Danger by using unsafe working practices")]
        public bool ComplyWithGuidelines { get; set; }


        [Required]
        [Display(Name = "I have no mental or physical conditions that not be reported to the DVLA which could affect my ability to drive safely")]
        public bool NoMentalOrPhysicalConditions { get; set; }


        [Required]
        [Display(Name = "I will inform SBL if my ability to safely drive or handle consignments changes in the future")]
        public bool InformSblChanges { get; set; }


        [Required(ErrorMessage = "Please sign your name to proceed")]
        [Display(Name = "Signature")]
        public string Step6Signature { get; set; }


        [Required(ErrorMessage = "Please add date to proceed")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step6DateSigned { get; set; }


    }
}