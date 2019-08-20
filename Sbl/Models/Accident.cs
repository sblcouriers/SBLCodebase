using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Accident
    {
        public int Id { get; set; }

        //

        [Required(ErrorMessage = "Associate is required")]
        [Display(Name = "Associate")]
        public int AssociateId { get; set; }


        public virtual Associate Associate { get; set; }

        //

        [Required(ErrorMessage = "Vehicle is required")]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }


        public virtual Vehicle Vehicle { get; set; }

        //

        public virtual ICollection<AccidentFile> AccidentFiles { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Status is required")]
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