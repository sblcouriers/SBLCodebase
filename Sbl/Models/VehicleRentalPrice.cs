using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class VehicleRentalPrice
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "SubRental is required")]
        [Display(Name = "SubRental")]
        public int SubRentalId { get; set; }
        
        public virtual SubRental SubRental { get; set; }



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
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }

    }//end class
}//end namespace