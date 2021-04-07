using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class Ticket
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int seatId { get; set; }
        public int transactionId { get; set; }
        public int type { get; set; }
        public int flightType { get; set; }
        public string blockingNumber { get; set; }
        public string confirmNumber { get; set; }
        public string cancelNumber { get; set; }
        public int status { get; set; } = 0;// 0 - not Available ; 1 - Available 
        public virtual ApplicationUser User { get; set; }
        public virtual Seat Seat{ get; set; }
        public virtual Transaction Trasaction { get; set; }
    }
    public enum TicketType
    {
        ADULT = 1 , OLD = 2 , KID = 3
    }
    public enum TicketStatus
    {
        [Display(Description = "Disable")]
        DISABLE = 0,
        [Display(Description = "Active")]
        ACTIVE = 1,
    }
}