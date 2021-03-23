using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class Stop
    {
        public int id { get; set; }
        public int flightId { get; set; }
        public int cityId { get; set; }
        public int stopOrder { get; set; }
        public virtual Airport Airport { get; set; }
        public virtual City City { get; set; }
    }
}