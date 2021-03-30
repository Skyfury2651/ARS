﻿using ARS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARS.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var ExpiredTransactionList = db.Transaction.SqlQuery("SELECT TOP (1000) [id] ,[ticketId] ,[price],[type],[createdAt],[updatedAt],[status], DATEADD(MINUTE, 30, createdAt) as expiredAt"
            + " FROM [aspnet-ARS-20210315112621].[dbo].[Transactions]"
                + " WHERE updatedAt < DATEADD(SECOND, 15, createdAt) AND status != 1").ToList();
            return View();
        }
        public ActionResult CitiesJsonDeparture()
        {
            return Json(db.Cities.Select(x => new
            {
                Id = x.id,
                Name = x.name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult AirportsJsonDeparture(string cityId)
        {

            int Id = Convert.ToInt32(cityId);
            var states = db.CityAirports.Where(x => x.CityId == Id)
               .Select(x => new
               {
                   Id = x.AirportId,
                   Name = x.Airport.name
               })
               .ToList();

            return Json(states, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AirportsJsonArrival(string cityId)
        {

            int Id = Convert.ToInt32(cityId);
            var states = db.CityAirports.Where(x => x.CityId == Id)
               .Select(x => new
               {
                   Id = x.AirportId,
                   Name = x.Airport.name,
                   CityName = x.City.name,
                   NearBy = false
               })
               .ToList();

            if (states.Count == 0)
            {
                var city = db.Cities.Find(Id);
                var query = db.Cities.SqlQuery("Select * from (SELECT *, (((acos(sin((" + city.lattitude + "*pi()/180)) * sin(([lattitude]*pi()/180)) + cos((" + city.lattitude + "*pi()/180)) * cos(([lattitude]*pi()/180)) * cos(((" + city.longtitude + " - [longtitude]) * pi()/180)))) * 180/pi()) * 60 * 1.1515 * 1.609344) as distance FROM [aspnet-ARS-20210315112621].[dbo].[Cities]) as st WHERE st.distance <= 1000");

                var cityes = query.ToList();
                List<int> cityIds = new List<int>();
                foreach (var item in cityes)
                {
                    cityIds.Add(item.id);
                }

                 states = db.CityAirports.Where(data => cityIds.Contains(data.CityId)).Select(x => new
                 {
                     Id = x.AirportId,
                     Name = x.Airport.name,
                     CityName = x.City.name,
                     NearBy = true
                 }).ToList();
            }

            return Json(states, JsonRequestBehavior.AllowGet);
        }
        public double ToRadians(double degrees) => degrees * Math.PI / 180.0;
        public double distanceInMiles(double lon1d, double lat1d, double lon2d, double lat2d)
        {
            var lon1 = ToRadians(lon1d);
            var lat1 = ToRadians(lat1d);
            var lon2 = ToRadians(lon2d);
            var lat2 = ToRadians(lat2d);

            var deltaLon = lon2 - lon1;
            var c = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon));
            var earthRadius = 3958.76;
            var distInMiles = earthRadius * c;

            return distInMiles;
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}