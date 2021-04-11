using ARS.App_Start;
using ARS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARS.Controllers
{
    public class FlightSearchListModel
    {
        public List<Flight> departureFlights;
        public List<Flight> returnFlights;
    }
    public class FlightController : Controller
    {
        public ApplicationDbContext _db;
        private List<int> transactionListId;
        public FlightController()
        {
            _db = new ApplicationDbContext();
            transactionListId = new List<int> { };
        }
        // GET: Flight
        public ActionResult Index(FlightSearchModel flight)
        {
            var Year = flight.ReturnDate.Year;
            var request = flight;
            var result = new FlightSearchListModel();
            if (flight.DepartureDate.Year == 1)
            {
                return RedirectToAction("Index", "Home");
            }

            var AddDepartureDate = flight.DepartureDate.AddDays(3);
            var MinusDepartureDate = flight.DepartureDate.AddDays(-3);
            var dataDeparture = _db.Flights.Where(p => p.departureDate < AddDepartureDate)
                .Where(p => p.departureDate > MinusDepartureDate)
                .Where(p => p.toAirportId == flight.toAirportId)
                .Where(p => p.fromAirportId == flight.fromAirportId)
                .Where(p => p.seatAvaiable >= flight.NumberOfPassager);
            result.departureFlights = dataDeparture.ToList();
            if (Year != 1 && flight.flightType == 2)
            {
                var AddReturnDate = flight.ReturnDate.AddDays(3);
                var MinusReturnDate = flight.ReturnDate.AddDays(-3);
                var dataReturn = _db.Flights.Where(p => p.departureDate > MinusReturnDate)
                    .Where(p => p.departureDate < AddReturnDate)
               .Where(p => p.toAirportId == flight.fromAirportId)
               .Where(p => p.fromAirportId == flight.toAirportId)
               .Where(p => p.seatAvaiable >= flight.NumberOfPassager);
                result.returnFlights = dataReturn.ToList();
            }

            return View(result);
        }

        public ActionResult Place(int? id)
        {
            Flight flight = _db.Flights.Find(id);
            FlightPlaceModel flightModel = new FlightPlaceModel();
            flightModel.Flight = flight;

            var bookedSeats = _db.Seats.Where(x => x.flightId == id && x.status != 1).Select(x => new
            {
                classType = x.classType,
                flightId = x.flightId,
                Id = x.id,
                position = x.position,
                status = x.status
            }).ToList();

            int result = DateTime.Compare(DateTime.Now.AddDays(14), flight.departureDate);
            string relationship;
            var allowReservation = false;
            if (result < 0)
            {
                relationship = "is earlier than";
                allowReservation = true;
            }

            return Json(new { bookedSeats, allowReservation }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MakeReservation(int id, string orderSeat, string orderSeatReturn = null, int returnId = 0, int flightType = 1, int type = 1, string transactionIds = null, string Cancel = null)
        {
            var blockingNumber = Helper.RandomString(5);
            //add transaction
            string currentUserId = User.Identity.GetUserId();
            Models.Transaction transaction = null;
            var flight = _db.Flights.Find(id);
            var seats = orderSeat.Split(',');
            List<Seat> seatList = new List<Seat> { };
            List<Ticket> tickets = new List<Ticket> { };
            double totalPrice = 0;
            for (int i = 0; i < seats.Length; i++)
            {
                string position = seats[i];
                var seat = _db.Seats.Where(x => x.position == position & x.flightId == id).FirstOrDefault();

                if (seat != null)
                {
                    seatList.Add(seat);
                }
            }
            // check have transaction in request or not
            if (string.IsNullOrEmpty(transactionIds))
            {
                transaction = _db.Transaction.Add(new Models.Transaction
                {
                    status = (int)TransactionStatus.PENDING,
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                    type = (int)TransactionType.PAYPAL
                });
                // start loop Departure
                foreach (var item in seatList)
                {
                    item.status = (int)SeatStatus.Reservation;
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
                        transactionId = transaction.id,
                        flightType = flightType,
                        type = (int)TicketType.ADULT,
                        status = (int)TicketStatus.DISABLE,
                        blockingNumber = blockingNumber
                    });
                    tickets.Add(ticket);
                    flight.seatAvaiable = flight.seatAvaiable - 1;
                    _db.SaveChanges();
                }
                //// end loop seatList Departure
                if (flightType == 2)
                {
                    var seatListReturn = new List<Seat> { };
                    for (int i = 0; i < seats.Length; i++)
                    {
                        var returnSeats = orderSeatReturn.Split(',');
                        string returnPosition = returnSeats[i];
                        var seat = _db.Seats.Where(x => x.position == returnPosition & x.flightId == returnId).FirstOrDefault();

                        if (seat != null)
                        {
                            seatListReturn.Add(seat);
                        }
                    }
                    var flightReturn = _db.Flights.Find(returnId);
                    for (int i = 0; i < seatListReturn.Count; i++)
                    {
                        seatListReturn[i].status = (int)SeatStatus.Reservation;
                        double price = 0;
                        switch (seatListReturn[i].classType)
                        {
                            case 1:
                                price += (flightReturn.price * 90 / 100);
                                break;
                            case 2:
                                price += (flightReturn.price * 90 / 100);
                                break;
                            case 3:
                                price += (flightReturn.price * 90 / 100);
                                break;
                            case 4:
                                price += (flightReturn.price * 90 / 100);
                                break;
                            case 5:
                                price += (flightReturn.price * 90 / 100);
                                break;
                            default:
                                break;
                        }
                        totalPrice += price;

                        var ticket = _db.Tickets.Add(new Ticket
                        {
                            userId = currentUserId,
                            seatId = seatListReturn[i].id,
                            transactionId = transaction.id,
                            flightType = flightType,
                            type = (int)TicketType.ADULT,
                            status = (int)TicketStatus.DISABLE,
                            blockingNumber = blockingNumber
                        });
                        tickets.Add(ticket);

                        flightReturn.seatAvaiable = flightReturn.seatAvaiable - 1;
                        _db.SaveChanges();
                    }
                }
                transaction.price = totalPrice;

                _db.SaveChanges();
                if (string.IsNullOrEmpty(transactionIds))
                {
                    transactionIds += (transaction.id).ToString();
                }
                else
                {
                    transactionIds += "," + (transaction.id).ToString();
                }
                //add transaction
            }

            var actionTicket = new ActionTicket();
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var transactionIdList = transactionIds.Split(',').Select(Int32.Parse).ToList();

            //sending Email
            actionTicket.Tickets = tickets;
            sendTicketMail(currentUser, transaction.Tickets, "blocking", blockingNumber, transaction.price.ToString());
            actionTicket.Transaction = transaction;
            actionTicket.User = currentUser;
            //on successful payment, show success page to user.  
            return View("~/Views/Ticket/TicketDetail.cshtml", actionTicket);
        }

        [CustomAuthorize]
        public ActionResult PaymentWithPaypal(int id, string orderSeat, int oldTransaction = 0, string orderSeatReturn = null, int returnId = 0, int flightType = 1, int type = 1, string transactionIds = null, string Cancel = null)
        {
            try
            {
                //add transaction
                string currentUserId = User.Identity.GetUserId();
                Models.Transaction transaction = null;
                var flight = _db.Flights.Find(id);
                var seats = orderSeat.Split(',');
                if (oldTransaction != 0)
                {
                    var oldTransactionItem = _db.Transaction.Find(oldTransaction);
                    oldTransactionItem.status = (int)TransactionStatus.CANCEL;
                    _db.SaveChanges();
                    var userManager2 = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var currentUser2 = userManager2.FindById(User.Identity.GetUserId());

                    var tickets = _db.Tickets.Where(x => x.transactionId == oldTransaction).ToList();
                    foreach (var ticket in tickets)
                    {
                        currentUser2.skyMiles = (int)Math.Round(currentUser2.skyMiles - ticket.Seat.Flight.distance);
                        userManager2.Update(currentUser2);
                    }

                    foreach (var oldTicket in tickets)
                    {
                        oldTicket.status = (int)TicketStatus.DISABLE;
                        oldTicket.Seat.status = (int)SeatStatus.Available;
                        _db.SaveChanges();
                    }
                }
                List<Seat> seatList = new List<Seat> { };
                double totalPrice = 0;
                for (int i = 0; i < seats.Length; i++)
                {
                    string position = seats[i];
                    var seat = _db.Seats.Where(x => x.position == position & x.flightId == id).FirstOrDefault();

                    if (seat != null)
                    {
                        seatList.Add(seat);
                    }
                }
                // check have transaction in request or not
                if (string.IsNullOrEmpty(transactionIds))
                {
                    transaction = _db.Transaction.Add(new Models.Transaction
                    {
                        status = (int)TransactionStatus.PENDING,
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now,
                        type = (int)TransactionType.PAYPAL
                    });
                    // start loop Departure
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
                            transactionId = transaction.id,
                            flightType = flightType,
                            type = (int)TicketType.ADULT,
                            status = (int)TicketStatus.DISABLE
                        });

                        flight.seatAvaiable = flight.seatAvaiable - 1;
                        _db.SaveChanges();
                    }
                    //// end loop seatList Departure
                    if (flightType == 2)
                    {
                        var seatListReturn = new List<Seat> { };
                        for (int i = 0; i < seats.Length; i++)
                        {
                            var returnSeats = orderSeatReturn.Split(',');
                            string returnPosition = returnSeats[i];
                            var seat = _db.Seats.Where(x => x.position == returnPosition & x.flightId == returnId).FirstOrDefault();

                            if (seat != null)
                            {
                                seatListReturn.Add(seat);
                            }
                        }
                        var flightReturn = _db.Flights.Find(returnId);
                        for (int i = 0; i < seatListReturn.Count; i++)
                        {
                            seatListReturn[i].status = (int)SeatStatus.Block;
                            double price = 0;
                            switch (seatListReturn[i].classType)
                            {
                                case 1:
                                    price += (flightReturn.price * 90 / 100);
                                    break;
                                case 2:
                                    price += (flightReturn.price * 90 / 100);
                                    break;
                                case 3:
                                    price += (flightReturn.price * 90 / 100);
                                    break;
                                case 4:
                                    price += (flightReturn.price * 90 / 100);
                                    break;
                                case 5:
                                    price += (flightReturn.price * 90 / 100);
                                    break;
                                default:
                                    break;
                            }
                            totalPrice += price;

                            var ticket = _db.Tickets.Add(new Ticket
                            {
                                userId = currentUserId,
                                seatId = seatListReturn[i].id,
                                transactionId = transaction.id,
                                flightType = flightType,
                                type = (int)TicketType.ADULT,
                                status = (int)TicketStatus.DISABLE
                            });


                            flightReturn.seatAvaiable = flightReturn.seatAvaiable - 1;
                            _db.SaveChanges();
                        }
                    }
                    transaction.price = totalPrice;

                    _db.SaveChanges();
                    if (string.IsNullOrEmpty(transactionIds))
                    {
                        transactionIds += (transaction.id).ToString();
                    }
                    else
                    {
                        transactionIds += "," + (transaction.id).ToString();
                    }
                    //add transaction
                }
                ApplicationDbContext context = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(User.Identity.GetUserId());
                ////getting the apiContext  
                if (currentUser.balance < totalPrice)
                {
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
                }
                else
                {
                    currentUser.balance = currentUser.balance - totalPrice;
                    userManager.Update(currentUser);
                }
                var actionTicket = new ActionTicket();
               
                var transactionIdList = transactionIds.Split(',').Select(Int32.Parse).ToList();
                var confirmNumber = Helper.RandomString(5);
                foreach (var item in transactionIdList)
                {

                    List<Ticket> tickets = new List<Ticket> { };
                    var transactionItem = _db.Transaction.Find(item);
                    var ticketLists = _db.Tickets.Where(x => x.transactionId == transactionItem.id).ToList();
                    if (ticketLists.Count != 0)
                    {
                        actionTicket.Transaction = transactionItem;
                        transactionItem.status = (int)TransactionStatus.SUCCESS;
                        transactionItem.updatedAt = DateTime.Now;
                        foreach (var ticket in transactionItem.Tickets)
                        {
                            ticket.status = (int)TicketStatus.ACTIVE;
                            ticket.confirmNumber = confirmNumber;
                            ticket.Seat.status = (int)SeatStatus.Buyed;
                            currentUser.skyMiles = (int)Math.Round(currentUser.skyMiles + ticket.Seat.Flight.distance);
                            userManager.Update(currentUser);
                            tickets.Add(ticket);
                        }

                        _db.SaveChanges();
                        sendTicketMail(currentUser, transactionItem.Tickets, "confirm", confirmNumber, transactionItem.price.ToString());
                    }

                    actionTicket.Tickets = tickets;


                }
                //sending Email
                actionTicket.User = currentUser;
                //on successful payment, show success page to user.  
                return View("~/Views/Ticket/TicketDetail.cshtml", actionTicket);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void sendTicketMail(ApplicationUser currentUser, List<Ticket> tickets, string numberType, string number, string totalPrice, string subject = "ARS Airline Ticket Info")
        {
            string mail = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate/Ticket2.html")))
            {
                mail = reader.ReadToEnd();
            }

            string header = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate/HeaderMail2.html")))
            {
                header = reader.ReadToEnd();
            }
            header = header.Replace("{userFullName}", currentUser.lastName + " " + currentUser.firstName);
            header = header.Replace("{numberType}", numberType);
            header = header.Replace("{number}", number);
            header = header.Replace("{totalPrice}", totalPrice);

            string ticketsContent = string.Empty;
            foreach (var item in tickets)
            {
                string ticketBody = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate/TicketBody2.html")))
                {
                    ticketBody = reader.ReadToEnd();
                }
                var departureAirportName = item.Seat.Flight.FromAirport.CityAirports.First().Airport.name;
                ticketBody = ticketBody.Replace("{departureAirportName}", departureAirportName);

                var departureTime = item.Seat.Flight.departureDate.ToString("D");
                ticketBody = ticketBody.Replace("{departureTime}", departureTime);

                var planeCode = item.Seat.Flight.planeCode;
                ticketBody = ticketBody.Replace("{planeCode}", planeCode);

                var departureAirportCode = item.Seat.Flight.ToAirport.CityAirports.First().Airport.code;
                ticketBody = ticketBody.Replace("{departureAirportCode}", departureAirportCode);

                var duration = item.Seat.Flight.flyTime;
                ticketBody = ticketBody.Replace("{duration}", duration.ToString());

                var fromAirportCode = item.Seat.Flight.FromAirport.CityAirports.First().Airport.code;
                ticketBody = ticketBody.Replace("{fromAirportCode}", fromAirportCode);

                var toAirportCode = item.Seat.Flight.ToAirport.CityAirports.First().Airport.code;
                ticketBody = ticketBody.Replace("{toAirportCode}", toAirportCode);

                var classService = "Non - Smoking";
                if (item.Seat.classType == 1)
                {
                    classService = "Bussiness";
                }
                else if (item.Seat.classType == 2)
                {
                    classService = "First Class ";
                }
                else if (item.Seat.classType == 3)
                {
                    classService = "Club Class ";
                }
                else if (item.Seat.classType == 4)
                {
                    classService = "Smoking";
                }
                ticketBody = ticketBody.Replace("{classService}", classService);

                var seatPosition = item.Seat.position;
                ticketBody = ticketBody.Replace("{seatPosition}", "10000");

                var departureCityName = item.Seat.Flight.FromAirport.CityAirports.First().City.name;
                ticketBody = ticketBody.Replace("{departureCityName}", departureCityName);

                var departureDate = item.Seat.Flight.departureDate.ToString("D");
                ticketBody = ticketBody.Replace("{departureDate}", departureDate);

                var arrivalAirportName = item.Seat.Flight.ToAirport.CityAirports.First().Airport.name;
                ticketBody = ticketBody.Replace("{arrivalAirportName}", arrivalAirportName);

                var arrivalCityName = item.Seat.Flight.ToAirport.CityAirports.First().City.name;
                ticketBody = ticketBody.Replace("{arrivalCityName}", "10000");

                var arrivalTime = item.Seat.Flight.arrivalDate.ToString("t");
                ticketBody = ticketBody.Replace("{arrivalTime}", "10000");

                var arrivalDate = item.Seat.Flight.arrivalDate.ToString("D");
                ticketBody = ticketBody.Replace("{arrivalDate}", "10000");
                ticketsContent = ticketsContent + ticketBody;
            }

            mail = mail.Replace("{Heading}", header);
            mail = mail.Replace("{tickets}", ticketsContent);
            string body = string.Empty;
            bool IsSendEmail = SendEmail.EmailSend(currentUser.Email, subject, mail, true);
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