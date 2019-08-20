﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class ChargePcn
    {
        public int Id { get; set; }

        //

        [Required]
        [Display(Name = "Associate")]
        public int AssociateId { get; set; }


        public virtual Associate Associate { get; set; }

        //

        public virtual ICollection<ChargePcnFile> ChargePcnFiles { get; set; }


        public virtual ICollection<ChargePcnInstalment> ChargePcnInstalments { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }


        [Required]
        [Display(Name = "Remittance Description")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "PCN Fee")]
        public double PcnFee { get; set; }


        [Required]
        [Display(Name = "Admin Fee")]
        public double AdminFee { get; set; }


        [Display(Name = "Number of Instalments")]
        public int NumberOfInstalments { get; set; }


        [Display(Name = "Week Frequency")]
        public int WeekFrequency { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date First Instalment")]
        public DateTime DateFirstInstalment { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Last Instalment")]
        public DateTime DateLastInstalment { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }


        [Display(Name = "Active")]
        public bool Active { get; set; }


        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }
    }//end 
}