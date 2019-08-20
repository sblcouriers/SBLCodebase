using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Models
{
    public class DashboardViewModel
    {
        public List<Associate> Associates { get; set; }
        public class Associate
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }




        public List<Depot> Depots { get; set; }
        public class Depot
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ActiveDrivers { get; set; }
        }


    }
}