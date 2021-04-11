using ARS.Models;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ARS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            RecurringJob.AddOrUpdate(
                () => ScheduleSeatAsync(), Cron.Hourly);
            RecurringJob.AddOrUpdate(
                () => ScheduleTransaction(), Cron.Daily);
            RecurringJob.AddOrUpdate(
                () => NotInTimePayment(), Cron.Daily);
        }
        public async System.Threading.Tasks.Task ScheduleSeatAsync()
        {
            ApplicationDbContext _db = new ApplicationDbContext();
            var ExpiredTransactionList = await _db.Database.SqlQuery<int>("SELECT Transactions.id FROM Tickets,Transactions,Seats WHERE tickets.transactionId = transactionId AND tickets.seatId = Seats.id AND Seats.status = 0 AND Transactions.status = 0 AND GETDATE() > DATEADD(SECOND, 30, [Transactions].updatedAt) GROUP BY [Transactions].id").ToListAsync();
            foreach (var item in ExpiredTransactionList)
            {
                var transaction = _db.Transaction.Find(item);
                foreach (var ticket in transaction.Tickets)
                {
                    ticket.Seat.status = (int)SeatStatus.Available;
                    ticket.Seat.Flight.seatAvaiable = ticket.Seat.Flight.seatAvaiable + 1;
                    ticket.status = (int)TicketStatus.DISABLE;
                    ticket.User.skyMiles = (int)Math.Round(ticket.User.skyMiles - ticket.Seat.Flight.distance);
                }
                transaction.status = (int)TransactionStatus.CANCEL;
                await _db.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task ScheduleTransaction()
        {
            System.Diagnostics.Debug.WriteLine("ScheduleTransaction");
            ApplicationDbContext _db = new ApplicationDbContext();
            var notPaidTransaction = await _db.Transaction.SqlQuery("SELECT * FROM ( SELECT TOP(1000) Transactions.id as id , departureDate , Transactions.status , Transactions.updatedAt , DATEDIFF(DAY,[Transactions].[updatedAt], Flights.departureDate) as DayChange FROM[aspnet - ARS - 20210315112621].[dbo].[Transactions]"
            + " INNER JOIN[aspnet - ARS - 20210315112621].[dbo].[Tickets] ON[Tickets].[transactionId] = Transactions.id"

            + " INNER JOIN[aspnet - ARS - 20210315112621].[dbo].[Seats] ON[Seats].[id] = seatId"

            + " INNER JOIN[aspnet - ARS - 20210315112621].[dbo].[Flights] ON[Flights].[id] = flightId)  AS data"

            + " WHERE data.DayChange = 21").ToListAsync();

            foreach (var item in notPaidTransaction)
            {
                var fullyTransaction = _db.Transaction.Find(item.id);
                var userId = fullyTransaction.Tickets.First().userId;
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(userId);

                sendTicketMail(currentUser, fullyTransaction.Tickets, "Cancel", "", fullyTransaction.price.ToString(), "ARS Airline ticket reminder");
            }
        }

        public async System.Threading.Tasks.Task NotInTimePayment()
        {
            System.Diagnostics.Debug.WriteLine("ScheduleTransaction");
            ApplicationDbContext _db = new ApplicationDbContext();
            var notPaidTransaction = await _db.Transaction.SqlQuery("SELECT * FROM ( SELECT TOP(1000) Transactions.id as id , departureDate , Transactions.status , Transactions.updatedAt , DATEDIFF(DAY,[Transactions].[updatedAt], Flights.departureDate) as DayChange FROM[aspnet - ARS - 20210315112621].[dbo].[Transactions]"
            + " INNER JOIN[aspnet - ARS - 20210315112621].[dbo].[Tickets] ON[Tickets].[transactionId] = Transactions.id"

            + " INNER JOIN[aspnet - ARS - 20210315112621].[dbo].[Seats] ON[Seats].[id] = seatId"

            + " INNER JOIN[aspnet - ARS - 20210315112621].[dbo].[Flights] ON[Flights].[id] = flightId)  AS data"

            + " WHERE data.DayChange < 15").ToListAsync();

            foreach (var item in notPaidTransaction)
            {
                var cancelNumber = Helper.RandomString(5);
                var fullyTransaction = _db.Transaction.Find(item.id);
                fullyTransaction.status = (int)TransactionStatus.CANCEL;
                _db.SaveChanges();
                var userManager2 = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser2 = userManager2.FindById(User.Identity.GetUserId());

                foreach (var ticket in fullyTransaction.Tickets)
                {
                    currentUser2.skyMiles = (int)Math.Round(currentUser2.skyMiles - ticket.Seat.Flight.distance);
                    userManager2.Update(currentUser2);
                }

                foreach (var oldTicket in fullyTransaction.Tickets)
                {
                    oldTicket.status = (int)TicketStatus.DISABLE;
                    oldTicket.Seat.status = (int)SeatStatus.Available;
                    oldTicket.cancelNumber = cancelNumber;
                    _db.SaveChanges();
                }


                var userId = fullyTransaction.Tickets.First().userId;
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(userId);

                sendTicketMail(currentUser, fullyTransaction.Tickets, "Cancel", cancelNumber, fullyTransaction.price.ToString(), "ARS Airline ticket canceled");
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

    }
}
