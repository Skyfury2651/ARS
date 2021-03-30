using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public double longtitude { get; set; }
        public double lattitude { get; set; }
        public string country { get; set; }
        public virtual ICollection<CityAirport> CityAirports{ get; set; }
    }
}