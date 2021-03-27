namespace ARS.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ARS.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ARS.Models.ApplicationDbContext context)
        {
            this.seedCity(context);
            this.seedCityAirport(context);
        }
        public void seedFlight()
        {

        }
        public void seedCity(ARS.Models.ApplicationDbContext context)
        {
            context.Cities.AddOrUpdate(x => x.id, new Models.City
            {
                id = 1,
                name = "DaNang",
                lattitude = 21.028693,
                longtitude = 105.841616,
            }, new Models.City
            {
                id = 2,
                name = "Ha Noi",
                lattitude = 21.028693,
                longtitude = 105.841616,
            }, new Models.City
            {
                id = 3,
                name = "HoChiMinh",
                lattitude = 21.028693,
                longtitude = 105.841616,
            }, new Models.City
            {
                id = 4,
                name = "DuongDong",
                lattitude = 9.930741,
                longtitude = 76.267348,
            }, new Models.City
            {
                id = 5,
                name = "Kochi",
                lattitude = 9.930740,
                longtitude = 76.267348,
            }, new Models.City
            {
                id = 6,
                name = "Calicut",
                lattitude = 9.930739,
                longtitude = 76.267348,
            }, new Models.City
            {
                id = 7,
                name = "AnDoCity",
                lattitude = 9.930741,
                longtitude = 76.267348,
            }
            );
        }

        public void seedCityAirport(ARS.Models.ApplicationDbContext context)
        {
            context.CityAirports.AddOrUpdate(new Models.CityAirport
            {
                AirportId = 558,
                CityId = 2
            }, new Models.CityAirport
            {
                AirportId = 559,
                CityId = 2
            }, new Models.CityAirport
            {
                AirportId = 560,
                CityId = 2
            }, new Models.CityAirport
            {
                AirportId = 555,
                CityId = 1
            }, new Models.CityAirport
            {
                AirportId = 556,
                CityId = 1
            }, new Models.CityAirport
            {
                AirportId = 15,
                CityId = 5
            }, new Models.CityAirport
            {
                AirportId = 16,
                CityId = 5
            }, new Models.CityAirport
            {
                AirportId = 17,
                CityId = 6
            }
            );
        }
    }
}
