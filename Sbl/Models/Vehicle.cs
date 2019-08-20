using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        //

        [Required(ErrorMessage = "Rental Company is required")]
        [Display(Name = "Rental Company")]
        public int RentalId { get; set; }


        public virtual Rental Rental { get; set; }

        //

        public virtual ICollection<SubRental> SubRentals { get; set; }

        public virtual ICollection<VehicleFile> VehicleFiles { get; set; }

        public virtual ICollection<VehicleHandbook> VehicleHandbooks { get; set; }

        public virtual ICollection<VehicleService> VehicleServices { get; set; }

        public virtual ICollection<Inspection> Inspections { get; set; }

        public virtual ICollection<Accident> Accidents { get; set; }

        //

        [Required(ErrorMessage = "Make is required")]
        [Display(Name = "Make")]
        public string Make { get; set; }


        [Required(ErrorMessage = "Model is required")]
        [Display(Name = "Model")]
        public string Model { get; set; }


        [Required(ErrorMessage = "Registration is required")]
        [Display(Name = "Registration")]
        public string Registration { get; set; }


        [Required(ErrorMessage = "Mileage is required")]
        [Display(Name = "Mileage")]
        public double MileageRented { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date Rented is required")]
        [Display(Name = "Date Rented")]
        public DateTime DateRented { get; set; }


        [Display(Name = "Rental Price")]
        public double RentalPrice { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Next Service Date")]
        public DateTime? NextServiceDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Returned")]
        public DateTime? DateReturned { get; set; }


        [Display(Name = "Mileage")]
        public double MileageReturned { get; set; }


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