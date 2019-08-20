using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class TimeSheetListViewModel
    {
        public int? AssociateId { get; set; }

        public int? DepotId { get; set; }

        public DateTime? DateStart { get; set; }

        public string SelectedStatus { get; set; }

        //

        public List<Remittance> Remittances { get; set; }
        public class Remittance
        {
            public int AssociateId { get; set; }
            public int? AssociateDepotId { get; set; }
            public string AssociateName { get; set; }

            public bool AuthPoc { get; set; }
            public bool AuthPayroll { get; set; }
            public bool AuthAdmin { get; set; }

            public double Total { get; set; }

            public int ReceiptSent { get; set; }
            public int RemittanceSent { get; set; }


            public List<Route> Routes { get; set; }
        }


        public class Route
        {
            public int Id { get; set; }
            public DateTime? RouteDate { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public double TotalTime { get; set; }

        }






        public List<Period> Periods { get; set; }
        public class Period
        {
            public int Year { get; set; }
            public int Week { get; set; }
            public DateTime? DateStart { get; set; }
            public DateTime? DateEnd { get; set; }
        }


        public List<Depot> Depots { get; set; }
        public class Depot
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }



        public List<Associate> Associates { get; set; }
        public class Associate
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


    }
}