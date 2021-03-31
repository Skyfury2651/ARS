using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ARS.Models
{
    public class TicketSeat
    {
        [Key, Column(Order = 0)]
        public int SeatId { get; set; }
        [Key, Column(Order = 1)]
        public int TicketId { get; set; }
        public virtual List<Seat> Seats { get; set; }
        public virtual List<Ticket> Tickets { get; set; }
    }
}