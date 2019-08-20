using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class RouteCharge
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Associate")]
        public int AssociateId { get; set; }

        [Required]
        [Display(Name = "RouteId")]
        public int RouteId { get; set; }

        public virtual Associate Associate { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public double Amount { get; set; }


        [Display(Name = "Set as Credit")]
        public bool SetAsCredit { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }

    }//end 
}