using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class AssociateListViewModel
    {
        public int? DepotId { get; set; }

        public List<Associate> Associates { get; set; }
        public class Associate
        {
            public int AssociateId { get; set; }

            public string Name { get; set; }
            public string Email { get; set; }

            public string Address { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
            public string Mobile { get; set; }

            public DateTime? DateOfBirth { get; set; }
            public string Nationality { get; set; }
            public string NationalInsuranceNumber { get; set; }
            public string UTRNumber { get; set; }

            public string NextOfKinName { get; set; }
            public string NextOfKinRelationship { get; set; }
            public string NextOfKinMobile { get; set; }

            public string NameOfTheBank { get; set; }
            public string SortCode { get; set; }
            public string AccountNumber { get; set; }
            public string AccountName { get; set; }

            public byte[] DataPhoto { get; set; }
            public string DataPhotoContentType { get; set; }
            public string DataPhotoName { get; set; }

            public string AssociateStatus { get; set; }
        }



        public List<Depot> Depots { get; set; }
        public class Depot
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }



    }
}