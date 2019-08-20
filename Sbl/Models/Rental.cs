using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Rental
    {
        public int Id { get; set; }

        //

        public virtual ICollection<Vehicle> Vehicles { get; set; }
        
        public virtual ICollection<RentalPriceLog> RentalPriceLogs { get; set; }

        //

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Weekly Rental Price")]
        public double RentalPrice { get; set; }


        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Weekly Insurance  Price")]
        public double InsurancePrice { get; set; }

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