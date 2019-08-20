using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class ChargePcnInstalment
    {
        public int Id { get; set; }

        //

        [Required]
        [Display(Name = "Charge PCN")]
        public int ChargePcnId { get; set; }


        public virtual ChargePcn ChargePcn { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }


        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Amount")]
        public double Amount { get; set; }

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