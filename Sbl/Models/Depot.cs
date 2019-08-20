using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Depot
    {
        public int Id { get; set; }

        //

        public virtual ICollection<Associate> Associates { get; set; }

        public virtual ICollection<RouteAllocation> RouteAllocations { get; set; }

        public virtual ICollection<RouteAmazon> RouteAmazons { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        //

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Telephone is required")]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }


        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }


        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; }


        [Required(ErrorMessage = "Postcode is required")]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }


        [Required(ErrorMessage = "Country is required")]
        [Display(Name = "Country")]
        public string Country { get; set; }

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