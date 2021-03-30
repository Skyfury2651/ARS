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
    public enum SeatType 
    {
        Bussiness = 1, FirstClass = 2, ClubClass = 3 , Smoking = 4 , NonSmoking = 5
    }
    public enum SeatStatus
    {
        Available = 1, Block = 0 , Buyed = 2
    }
}