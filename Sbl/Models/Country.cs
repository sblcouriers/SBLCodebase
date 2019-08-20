using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Country
    {
        public int Id { get; set; }

        //

        [Display(Name = "Name")]
        public string Name { get; set; }


        [Display(Name = "Code")]
        public string Code { get; set; }


        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

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