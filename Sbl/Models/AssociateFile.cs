using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class AssociateFile
    {
        public int Id { get; set; }

        //

        [Required(ErrorMessage = "Associate is required")]
        [Display(Name = "Associate")]
        public int AssociateId { get; set; }


        public virtual Associate Associate { get; set; }

        //

        [Display(Name = "File")]
        public byte[] DataFile { get; set; }


        [Display(Name = "File Content Type")]
        public string DataFileContentType { get; set; }


        [Display(Name = "File Name")]
        public string DataFileName { get; set; }


        [Display(Name = "File Description")]
        public string DataFileDescription { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiry Date")]
        public DateTime? DataFileExpiryDate { get; set; }

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