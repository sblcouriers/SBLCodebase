using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class VehicleHandbook
    {
        public int Id { get; set; }

        //

        [Display(Name = "Vehicle")]
        public int? VehicleId { get; set; }


        public virtual Vehicle Vehicle { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Book Date")]
        public DateTime? BookDate { get; set; }

        //

        [Display(Name = "Notes")]
        public string Notes { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; }

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