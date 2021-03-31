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
            var ExpiredTransactionList = await _db.Transaction.SqlQuery("SELECT TOP (1000) [id] ,[ticketId] ,[price],[type],[createdAt],[updatedAt],[status], DATEADD(MINUTE, 30, createdAt) as expiredAt"
            + " FROM [aspnet-ARS-20210315112621].[dbo].[Transactions]"
                + " WHERE updatedAt < DATEADD(MINUTE, 15, createdAt) AND status != 1 AND status != 2").ToListAsync();
            foreach (var item in ExpiredTransactionList)
            {
                item.Ticket.Seat.status = (int) SeatStatus.Available;
                item.Ticket.status = (int)TicketStatus.DISABLE;
                item.status = (int)TransactionStatus.CANCEL;
                await _db.SaveChangesAsync();
            }
        }
    }
}
