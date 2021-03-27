using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class Ticket
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int flightId { get; set; }
        public int seatId { get; set; }
        public int type { get; set; }
        public int flightType { get; set; }
        public string blockingNumber { get; set; }
        public string confirmNumber { get; set; }
        public string cancelNumber { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Flight Flight { get; set; }
        public virtual Seat Seat { get; set; }
    }
}