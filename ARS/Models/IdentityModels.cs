using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ARS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userIdentityCode { get; set; }

        public int balance { get; set; }
        public string address { get; set; }
        public int sex { get; set; }
        public int age { get; set; }
        public string preferedCreditCardNumber { get; set; }
        public int skyMiles { get; set; }
        public int status { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public virtual DbSet<Airport> Airports { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<Stop> Stops { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<CityAirport> CityAirports { get; set; }
        public virtual DbSet<TicketSeat> TicketSeats { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}