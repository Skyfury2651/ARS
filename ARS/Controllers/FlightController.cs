using ARS.App_Start;
using ARS.Models;
using Microsoft.AspNet.Identity;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARS.Controllers
{
    public class FlightController : Controller
    {
        public ApplicationDbContext _db;

        public FlightController()
        {
            _db = new ApplicationDbContext();
        }
        // GET: Flight
        public ActionResult Index(FlightSearchModel flight)
        {
            var request = flight;
            //_db.Flights.Where(x => x.departureDate in);
            var AddDepartureDate = flight.DepartureDate.AddDays(3);
            var MinusDepartureDate = flight.DepartureDate.AddDays(-3);
            var data = _db.Flights.Where(p => p.departureDate < AddDepartureDate)
                .Where(p => p.departureDate > MinusDepartureDate)
                .Where(p => p.toAirportId == flight.toAirportId)
                .Where(p => p.fromAirportId == flight.fromAirportId)
                .Where(p => p.seatAvaiable >= flight.NumberOfPassager)
               .ToList();
            return View(data);
        }
        public ActionResult Place(int id)
        {
            Flight flight = _db.Flights.Find(id);
            FlightPlaceModel flightModel = new FlightPlaceModel();
            flightModel.Flight = flight;

            var bookedSeats = _db.Seats.Where(x => x.flightId == id && x.status != 1).ToList();
            flightModel.bookedSeat = bookedSeats;

            return View(flightModel);
        }
        public ActionResult PaymentWithPaypal(int id, string orderSeat, int flightType = 1, int type = 1, string transactionIds = null, string Cancel = null)
        {
            string currentUserId = User.Identity.GetUserId();

            var flight = _db.Flights.Find(id);
            var seats = orderSeat.Split(',');
            List<Seat> seatList = new List<Seat> { };
            double totalPrice = 0;
            for (int i = 0; i < seats.Length; i++)
            {
                string position = seats[i];
                var seat = _db.Seats.Where(x => x.position == position).FirstOrDefault();

                if (seat != null)
                {
                    seatList.Add(seat);
                }
            }
            if (string.IsNullOrEmpty(transactionIds))
            {
                foreach (var item in seatList)
                {
                    item.status = (int)SeatStatus.Block;
                    double price = 0;
                    switch (item.classType)
                    {
                        case 1:
                            price += (flight.price * 90 / 100);
                            break;
                        case 2:
                            price += (flight.price * 90 / 100);
                            break;
                        case 3:
                            price += (flight.price * 90 / 100);
                            break;
                        case 4:
                            price += (flight.price * 90 / 100);
                            break;
                        case 5:
                            price += (flight.price * 90 / 100);
                            break;
                        default:
                            break;
                    }
                    totalPrice += price;

                    var ticket = _db.Tickets.Add(new Ticket
                    {
                        userId = currentUserId,
                        seatId = item.id,
                        flightType = flightType,
                        type = (int)TicketType.ADULT,
                        status = (int)TicketStatus.DISABLE,
                        flightId = flight.id
                    });

                    var transaction = _db.Transaction.Add(new Models.Transaction
                    {
                        ticketId = ticket.id,
                        status = (int)TransactionStatus.PENDING,
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now,
                        price = price,
                        type = (int)TransactionType.PAYPAL
                    });
                    _db.SaveChanges();
                    if (string.IsNullOrEmpty(transactionIds))
                    {
                        transactionIds += (transaction.id).ToString();
                    }
                    else
                    {
                        transactionIds += "," + (transaction.id).ToString();
                    }

                }
                var Testtransaction = transactionIds;
            }
            ////getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Flight/PaymentWithPayPal/" + id + "?orderSeat=" + orderSeat + "&flightType=" + flightType + "&type=" + type + "&transactionIds=" + transactionIds + "&";
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, flight.planeCode, totalPrice, seatList.Count);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    var sessionPaymentId = Session[guid];

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //System.Diagnostics.Debug.WriteLine(ExecutePayment.state.ToLower());
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return null;
                        //return View("FailureView");
                    }
                }
            }
            catch (PayPal.PaymentsException ex)
            {
                Console.WriteLine(ex.Message);
            }
            var transactionIdList = transactionIds.Split(',').Select(Int32.Parse).ToList();
            foreach (var item in transactionIdList)
            {
                var transaction = _db.Transaction.Find(item);
                transaction.status = (int)TransactionStatus.SUCCESS;
                transaction.Ticket.status = (int)TicketStatus.ACTIVE;
                transaction.Ticket.Seat.status = (int)SeatStatus.Buyed;
                _db.SaveChanges();
            }
            // Xu ly chuoi transactionIds de lay duoc transactions va tickets


            //on successful payment, show success page to user.  
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
    public class FlightPlaceModel
    {
        public Flight Flight;
        public List<Seat> bookedSeat;
    }
    public class FlightSearchModel
    {
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int toAirportId { get; set; }
        public int fromAirportId { get; set; }
        public int NumberOfPassager { get; set; }
        public int flightType { get; set; }
    }
}