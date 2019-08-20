using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class RentalPriceLog
    {
        public int Id { get; set; }

        //

        public int RentalId { get; set; }

        public virtual Rental Rental { get; set; }

        //

        public double RentalPrice { get; set; }


        public double InsurancePrice { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Changed")]
        public DateTime? DateChanged { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }


        public bool Active { get; set; }


        public bool Deleted { get; set; }
    }
}