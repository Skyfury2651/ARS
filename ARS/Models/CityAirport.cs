using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class CityAirport
    {
        [Key, Column(Order = 0)]
        public int CityId { get; set; }
        [Key, Column(Order = 1)]
        public int AirportId { get; set; }
        public virtual City City { get; set; }
        public virtual Airport Airport { get; set; }
    }
}