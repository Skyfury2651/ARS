using ARS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARS.Controllers
{
    public class ActionTicket
    {
        public Models.Transaction Transaction;
        public List<Ticket> Tickets;
        public Models.ApplicationUser User;
    }


    public class TicketController : Controller
    {
        private ApplicationDbContext _db;
        public TicketController()
        {
            _db = new ApplicationDbContext();
        }
        // GET: Ticket
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult CancelTicket(int id, int type, string confirmNumber = null, string blockingNumber = null)
        {
            if (string.IsNullOrEmpty(confirmNumber) == false || string.IsNullOrEmpty(blockingNumber) == false)
            {
                var ticket = _db.Tickets.Find(id);
                var transaction = ticket.Trasaction;
                var cancelNumber = Helper.RandomString(5);
                ApplicationDbContext context = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(User.Identity.GetUserId());
                ticket.status = (int)TicketStatus.DISABLE;
                ticket.cancelNumber = cancelNumber;
                double returnPrice = 0;
                if (ticket.Seat.status == (int)SeatStatus.Block)
                {
                    transaction.price = transaction.price - ticket.Seat.Flight.price;
                }
                else
                {
                    returnPrice = ticket.Seat.Flight.price;
                    if ((DateTime.Now - transaction.updatedAt).TotalDays < 14)
                    {
                        returnPrice = returnPrice * 50 / 100;
                    }
                    else
                    {
                        returnPrice = returnPrice * 80 / 100;
                    }
                    transaction.price = transaction.price - returnPrice;
                }
                transaction.updatedAt = DateTime.Now;
                currentUser.skyMiles = (int)Math.Round(currentUser.skyMiles - ticket.Seat.Flight.distance);
                currentUser.balance += returnPrice;
                userManager.Update(currentUser);
                var ticketActive = transaction.Tickets.Where(x => x.status == 1).Count();
                if (ticketActive == 0)
                {
                    transaction.status = (int)TransactionStatus.CANCEL;
                }

                _db.SaveChanges();

                return Json(new { code = 200 });
            }
            return Json(new { code = 422 });
        }


        public ActionResult ConfirmTicket(int id, string confirmNumber)
        {
            var ticket = _db.Tickets.Find(id);
            var transaction = ticket.Trasaction;
            
            if (transaction.Tickets.First().blockingNumber == confirmNumber)
            {
                string orderSeats = "";

                foreach (var item in transaction.Tickets)
                {
                    orderSeats += item.Seat.position + ',';
                }

                var fullUrl = this.Url.Action("PaymentWithPaypal", "Flight", new { id = transaction.Tickets.First().Seat.flightId, orderSeat = orderSeats }, this.Request.Url.Scheme);
                return Json(new { fullUrl, code = 201 });
            }

            return Json(new { code = 422 });
        }

        [CustomAuthorize]
        public ActionResult TicketsList()
        {
            var userId = User.Identity.GetUserId();
            var tickets = _db.Tickets.Where(x => x.userId == userId).OrderByDescending(x => x.id).ToList();
            return View(tickets);
        }

        public ActionResult RescheduleTicketCheck(int id, string confirmNumber)
        {
            var ticket = _db.Tickets.Find(id);
            var numberSeat = ticket.Trasaction.Tickets.Count();
            var firstTicket = _db.Tickets.Find(ticket.Trasaction.Tickets.Min(x => x.id));

            var fromCity = firstTicket.Seat.Flight.FromAirport.CityAirports.First().City.name;
            var toCity = firstTicket.Seat.Flight.ToAirport.CityAirports.First().City.name;

            var fromAirportId = firstTicket.Seat.Flight.fromAirportId;
            var toAirportId = firstTicket.Seat.Flight.toAirportId;
            var toAirport = firstTicket.Seat.Flight.ToAirport;
            var fromAirport = firstTicket.Seat.Flight.ToAirport;
            var oldTransaction = ticket.transactionId;
            if (ticket.flightType == 2)
            {
                numberSeat = numberSeat / 2;
            }
            if (ticket.confirmNumber == confirmNumber && ticket.Trasaction.status == (int)TransactionStatus.SUCCESS)
            {
                return Json(new
                {
                    status = "Success",
                    flightType = ticket.flightType,
                    fromCity = fromCity,
                    toCity = toCity,
                    NumberOfPassager = numberSeat,
                    fromAirportId = fromAirportId,
                    toAirportId = toAirportId,
                    oldTransaction = oldTransaction
                });
            }

            return Json(new { status = "Fail" });
        }

        public ActionResult RescheduleTicket(int id, int flightId, string orderSeat, string orderSeatReturn = null, int returnId = 0, int flightType = 1, int type = 1, string transactionIds = null, string Cancel = null)
        {
            var transaction = _db.Transaction.Find(id);
            transaction.status = (int)TransactionStatus.CANCEL;
            foreach (var ticket in transaction.Tickets)
            {
                ticket.status = (int)TicketStatus.DISABLE;
                ticket.Seat.status = (int)SeatStatus.Available;
            }

            return RedirectToAction("PaymentWithPaypal", "Flight", new { id = flightId, orderSeat = orderSeat, orderSeatReturn = orderSeatReturn, returnId = returnId, flightType = flightType, type = type });
        }
        public ActionResult TicketDetail(int id)
        {
            var ticket = _db.Tickets.Find(id);
            var transaction = ticket.Trasaction;
            var actionTicket = new ActionTicket();
            actionTicket.Transaction = transaction;
            List<Ticket> tickets = new List<Ticket> { };
            tickets.Add(ticket);
            actionTicket.Tickets = tickets;

            return View(actionTicket);
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {

            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string name, double price, int quanity)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = name,
                currency = "USD",
                price = (price / quanity).ToString(),
                quantity = quanity.ToString(),
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = price.ToString(),
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = price.ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<PayPal.Api.Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new PayPal.Api.Transaction()
            {
                description = "Transaction description",
                invoice_number = "RandomInvoince", //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

    }
}