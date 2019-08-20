using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class ScheduleViewModel
    {

        public int? DepotId { get; set; }

        public DateTime? DateStart { get; set; }

        public int? Days { get; set; }

        public int? ActiveDrivers { get; set; }

        public string POCDepot { get; set; }


        public int? ScheduleEditDays { get; set; }


        //


        public List<Associate> Associates { get; set; }

        public class Associate
        {
            public int AssociateId { get; set; }
            public string AssociateName { get; set; }
            public string AssociateMobile { get; set; }
            public int AssociateDepotId { get; set; }
            public string AssociateDepotName { get; set; }
            public string AssociateVehicleRegistration { get; set; }
            //
            public List<Route> Routes { get; set; }
        }

        public class Route
        {
            public int RouteId { get; set; }
            public int AssociateId { get; set; }
            public DateTime? RouteDate { get; set; }
            public int RouteDepotId { get; set; }
            public string RouteDepotName { get; set; }
            public string AllocationStatus { get; set; }

            public string RouteType1 { get; set; }

            public int RoutesOn { get; set; }
            public int RoutesOff { get; set; }

            public bool AuthPoc { get; set; }
            public bool AuthPayroll { get; set; }
            public bool AuthAdmin { get; set; }

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



    }
}