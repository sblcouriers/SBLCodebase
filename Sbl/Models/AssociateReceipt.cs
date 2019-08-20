using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class AssociateReceipt
    {
        public int Id { get; set; }

        //

        [Display(Name = "Associate")]
        public int AssociateId { get; set; }


        public virtual Associate Associate { get; set; }

        //

        [Display(Name = "Week")]
        public int Week { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Week Start Date")]
        public DateTime? WeekStartDate { get; set; }


        [Display(Name = "Is Sent")]
        public bool IsSent { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Sent")]
        public DateTime? DateSent { get; set; }

        //

        public byte[] DataFile { get; set; }
        public string DataFileContentType { get; set; }
        public string DataFileName { get; set; }
        public string DataFileExtension { get; set; }

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