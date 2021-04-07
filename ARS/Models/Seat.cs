using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public virtual List<Ticket> Ticket { get; set; }
    }
    public enum SeatType 
    {
        Bussiness = 1, FirstClass = 2, ClubClass = 3 , Smoking = 4 , NonSmoking = 5
    }
    public enum SeatStatus
    {
        [Display(Description = "Available")]
        Available = 1,
        [Display(Description = "Blocked")]
        Block = 0 ,
        [Display(Description = "Paid")]
        Buyed = 2,
        [Display(Description = "Reservation")]
        Reservation = 3 
    }
}