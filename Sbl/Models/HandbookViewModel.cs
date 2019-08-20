using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Models
{
    public class HandbookViewModel
    {

        public int? DepotId { get; set; }

        public DateTime? DateStart { get; set; }

        public int? Days { get; set; }

        public int? ActiveDrivers { get; set; }

        public string POCDepot { get; set; }


        public int? ScheduleEditDays { get; set; }


        public bool IsRented { get; set; }


        //


        public List<Vehicle> Vehicles { get; set; }

        public class Vehicle
        {
            public int VehicleId { get; set; }
            public string AssociateName { get; set; }
            public string DepotName { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Registration { get; set; }
            //
            public List<Handbook> Handbooks { get; set; }
        }

        public class Handbook
        {
            public int HandbookId { get; set; }
            public int VehicleId { get; set; }
            public DateTime? BookDate { get; set; }
            public string Notes { get; set; }
            public string Status { get; set; }
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