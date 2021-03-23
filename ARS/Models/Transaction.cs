using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class Transaction
    {
        public int id { get; set; }
        public int ticketId { get; set; }
        public int flightId { get; set; }
        public double price { get; set; }
        public int type { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual Flight Flight { get; set; }
    }
}