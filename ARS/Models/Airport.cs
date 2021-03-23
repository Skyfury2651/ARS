using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class Airport
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string iso_country { get; set; }
        public string municipality { get; set; }
        public double lattitude { get; set; }
        public double longtitude { get; set; }
        public int status { get; set; }
        public virtual ICollection<CityAirport> CityAirports { get; set; }
    }
}