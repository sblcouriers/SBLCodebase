using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Models.BO
{
    public class Overview
    {
        public int Id { get; set; }
        public string Period { get; set; }
        public double SblTotal { get; set; }
        public double AmazonTotal { get; set; }
        public string Status { get; set; }
        public double Diff { get; set; }
    }
}