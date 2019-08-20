using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class InspectionCoord
    {
        public int Id { get; set; }

        //

        [Required]
        [Display(Name = "Inspection")]
        public int InspectionId { get; set; }


        public virtual Inspection Inspection { get; set; }

        //

        [Display(Name = "Location")]
        public string Location { get; set; }


        [Display(Name = "Coord X")]
        public string CoordX { get; set; }


        [Display(Name = "Coord Y")]
        public string CoordY { get; set; }


        [Display(Name = "Damage Key")]
        public string DamageKey { get; set; }

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