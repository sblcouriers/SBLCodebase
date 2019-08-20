using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Log
    {
        public int Id { get; set; }

        //

        [Display(Name = "Method")]
        public string Method { get; set; }


        [Display(Name = "Message")]
        public string Message { get; set; }


        [Display(Name = "Is Error")]
        public bool IsError { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
    }
}