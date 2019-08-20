using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class SubRental
    {
        public int Id { get; set; }
        

        [Required(ErrorMessage = "Associate is required")]
        [Display(Name = "Associate")]
        public int AssociateId { get; set; }


        public virtual Associate Associate { get; set; }


        [Required(ErrorMessage = "Vehicle is required")]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }


        public virtual Vehicle Vehicle { get; set; }

        [Required(ErrorMessage = "Mileage is required")]
        [Display(Name = "Mileage Start")]
        public double MileageStart { get; set; }


        [Display(Name = "Mileage End")]
        public double MileageEnd { get; set; }


        [Display(Name = "Van Rental Price")]
        public double VanRentalPrice { get; set; }

        [Display(Name = "Insurance Price")]
        public double InsurancePrice { get; set; }

        [Display(Name = "Goods in Transit Price")]
        public double GoodsInTransitPrice { get; set; }

        [Display(Name = "Public Liability Price")]
        public double PublicLiabilityPrice { get; set; }

        [Display(Name = "Rental Price")]
        public double RentalPrice { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date Rented is required")]
        [Display(Name = "Date Rented")]
        public DateTime DateRented { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Returned")]
        public DateTime? DateReturned { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }
    }
}