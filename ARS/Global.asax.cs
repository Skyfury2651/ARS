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
            //sendMailDaily();
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            RecurringJob.AddOrUpdate(
                () => ScheduleSeatAsync(), Cron.Minutely);
            RecurringJob.AddOrUpdate(
                () => ScheduleTransaction(), Cron.Daily);
            RecurringJob.AddOrUpdate(
                () => NotInTimePayment(), Cron.Daily);
        }
        public async System.Threading.Tasks.Task ScheduleSeatAsync()
        {
            ApplicationDbContext _db = new ApplicationDbContext();
            var ExpiredTransactionList = await _db.Transaction.SqlQuery("SELECT TOP (1000) [id] ,[price],[type],[createdAt],[updatedAt],[status], DATEADD(MINUTE, 15, [updatedAt]) as expiredAt"
            + " FROM [aspnet-ARS-20210315112621].[dbo].[Transactions]"
                + " WHERE GETDATE() < DATEADD(MINUTE, 15, [updatedAt]) AND status = 0").ToListAsync();
            foreach (var item in ExpiredTransactionList)
            {
                foreach (var ticket in item.Tickets)
                {
                    ticket.Seat.status = (int)SeatStatus.Available;
                    ticket.Seat.Flight.seatAvaiable = ticket.Seat.Flight.seatAvaiable + 1;
                    ticket.status = (int)TicketStatus.DISABLE;
                    ticket.User.skyMiles = (int)Math.Round(ticket.User.skyMiles - ticket.Seat.Flight.distance);
                }
                item.status = (int)TransactionStatus.CANCEL;
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

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate.html")))
                {
                    body = reader.ReadToEnd();
                }
                //body = body.Replace("{typeNumber}", "confirm");
                //body = body.Replace("{number}", confirmNumber);
                //body = body.Replace("{UserName}", currentUser.lastName + currentUser.firstName);
                bool IsSendEmail = SendEmail.EmailSend(currentUser.Email, "Your confirm number", body, true);
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
                    _db.SaveChanges();
                }
            }
        }
    }
}
