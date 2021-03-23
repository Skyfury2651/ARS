using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class Flight
    {
        public int id { get; set; }
        public string planeCode { get; set; }
        public int fromAirportId { get; set; }
        public int toAirportId { get; set; }
        public int type { get; set; }
        public DateTime departureDate { get; set; }
        public DateTime arrivalDate { get; set; }
        public double flyTime { get; set; }
        public double price { get; set; }
        public double distance { get; set; }
        public bool haveStop { get; set; }
        public int status { get; set; }
        public virtual Airport FromAirport { get; set; }
        public virtual Airport ToAirport { get; set; }
    }
}