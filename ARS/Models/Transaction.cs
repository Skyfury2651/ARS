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
        public double price { get; set; }
        public int type { get; set; }
        public int status { get; set; } = 0; // 0 - pending ; 1 - success ; 2 - cancel
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
    public enum TransactionStatus
    {
        PENDING = 0 , SUCCESS = 1 , CANCEL = 2
    }

    public enum TransactionType
    {
        PAYPAL = 0 , VISA = 1 , LOCALBANKING = 2, GOOGLEPAY = 3 
    }
}