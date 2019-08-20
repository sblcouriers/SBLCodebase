using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class AssociateProfileViewModel
    {
        public int Id { get; set; }


        public string Name { get; set; }


        public string Email { get; set; }


        public string Position { get; set; }


        public string Address { get; set; }


        public string City { get; set; }


        public string Postcode { get; set; }


        public string Mobile { get; set; }


        [Display(Name = "Next of Kin Name")]
        public string NextOfKinName { get; set; }


        [Display(Name = "Next of Kin Relationship")]
        public string NextOfKinRelationship { get; set; }


        [Display(Name = "Next of Kin Mobile")]
        public string NextOfKinMobile { get; set; }


        public byte[] DataPhoto { get; set; }


        public string DataPhotoContentType { get; set; }


        public string Bio { get; set; }


        public List<AssociateFile> AssociateFiles { get; set; }
        public class AssociateFile
        {
            public int Id { get; set; }
            public string DataFileName { get; set; }
            public byte[] DataFile { get; set; }
            public string DataFileDescription { get; set; }
            public DateTime? DataFileExpiryDate { get; set; }
        }
        public List<AssociateRemittance> AssociateRemittances { get; set; }
        public class AssociateRemittance
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public string Status { get; set; }
        }
    }
}