using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InductionStep5ViewModel
    {
        public int Id { get; set; }

        public string GuidId { get; set; }

        public string FullName { get; set; }

        //

        [Required(ErrorMessage = "Please sign your name to proceed")]
        [Display(Name = "Signature")]
        public string Step5Signature { get; set; }


        [Required(ErrorMessage = "Please add date to proceed")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Signed")]
        public DateTime? Step5DateSigned { get; set; }

    }
}