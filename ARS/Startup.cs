using ARS.Models;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.IO;
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

[assembly: OwinStartupAttribute(typeof(ARS.Startup))]
namespace ARS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate(
                () => ScheduleSeatAsync(), Cron.Minutely);
            //RecurringJob.AddOrUpdate(
            //    () => ScheduleTransaction(), Cron.Daily);
        }

        public async System.Threading.Tasks.Task ScheduleTransaction()
        {
            ApplicationDbContext _db = new ApplicationDbContext();
            var notPaidTransaction = await _db.Transaction.SqlQuery("SELECT TOP (1000) [id], [price], [type], [status], [createdAt] FROM [aspnet - ARS - 20210315112621].[dbo].[Transactions] WHERE status = 0").ToListAsync();
            foreach (var transaction in notPaidTransaction)
            {
                foreach (var ticket in transaction.Tickets)
                {
                    var flight = _db.Flights.Find(ticket.Seat.flightId);
                    var data = _db.Transaction.SqlQuery("SELECT TOP (1000) [id], [price], [type], [status], [createdAt] FROM [aspnet - ARS - 20210315112621].[dbo].[Transactions] WHERE status = 0");
                    //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    //var currentUser = userManager.FindById(ticket.userId);
                    //string body = string.Empty;
                    //using (StreamReader reader = new StreamReader("~/Views/MailDelay.html"))
                    //{
                    //    body = reader.ReadToEnd();
                    //}
                    //body = body.Replace("{typeNumber}", "confirm");
                    //body = body.Replace("{number}", ticket.Seat.flightId.ToString());
                    //body = body.Replace("{UserName}", currentUser.lastName + currentUser.firstName);
                    //bool IsSendEmail = SendEmail.EmailSend(currentUser.Email, "Your blocking number is ", body, true);
                }
            }
            

            //var ExpiredTransactionList = await _db.Transaction.SqlQuery("SELECT TOP (1000) [id] ,[price],[type],[createdAt],[updatedAt],[status], DATEADD(MINUTE, 15, [updatedAt]) as expiredAt"
            //+ " FROM [aspnet-ARS-20210315112621].[dbo].[Transactions]"
            //    + " WHERE GETDATE() < DATEADD(MINUTE, 15, [updatedAt]) AND status = 0").ToListAsync();
            //foreach (var item in ExpiredTransactionList)
            //{
            //    foreach (var ticket in item.Tickets)
            //    {
            //        ticket.Seat.status = (int)SeatStatus.Available;
            //        ticket.Seat.Flight.seatAvaiable = ticket.Seat.Flight.seatAvaiable + 1;
            //        ticket.status = (int)TicketStatus.DISABLE;
            //        ticket.User.skyMiles = (int)Math.Round(ticket.User.skyMiles - ticket.Seat.Flight.distance);
            //    }
            //    item.status = (int)TransactionStatus.CANCEL;
            //    await _db.SaveChangesAsync();
            //}
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
    }
}
