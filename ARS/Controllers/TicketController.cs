using ARS.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARS.Controllers
{
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
        public ActionResult CancelTicket(int id, int type, string confirmNumber = null, string blockingNumber = null)
        {
            if (confirmNumber != null && blockingNumber != null)
            {

                return Json(new { code = 200 });
            }
            return Json(new { code = 422 });
        }
        public ActionResult ConfirmTicket(int id, string confirmNumber)
        {
            //var ticket = _db.Tickets.Find(id);
            //if (ticket.confirmNumber == confirmNumber)
            //{
            //    string orderSeats = "";
            //    foreach (var seat in ticket.TicketSeat)
            //    {
            //        orderSeats += seat.Seat.position + ',';
            //    }
            //    return RedirectToAction("PaymentWithPaypal", "Flight", new { id = id, orderSeats = orderSeats });
            //}

            return Json(new { code = 422 });
        }

        public ActionResult RescheduleTicket()
        {
            return View();
        }

        public ActionResult ViewTicketStatus()
        {
            return View();
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