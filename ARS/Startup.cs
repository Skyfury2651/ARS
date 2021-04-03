using ARS.Models;
using Hangfire;
using Microsoft.Owin;
using Owin;
using System;

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
