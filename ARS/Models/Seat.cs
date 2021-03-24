using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class Seat
    {
        public int id { get; set; }
        public string position { get; set; }
        public int flightId { get; set; }
        public int classType { get; set; }
        public int status { get; set; }
        public virtual Flight Flight { get; set; }
    }
}