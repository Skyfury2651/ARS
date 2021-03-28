using ARS.Models;
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
            return View();
        }
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