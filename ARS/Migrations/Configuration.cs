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
            //this.seedCity(context);
            //this.seedCityAirport(context);
            this.seedFlightCase1(context);
            this.seedFlightsCase2(context);
            //this.seedSeat(context);
            this.seedStops(context);
        }
        public void seedSeat(ARS.Models.ApplicationDbContext context)
        {
            for (int i = 7; i < 18; i++)
            {
                int classType = 1;
                if (i > 14)
                {
                    classType = 4;
                }
                for (int j = 1; j < 7; j++)
                {
                    string position = j.ToString()  + Convert.ToChar(j + 64);
                    context.Seats.AddOrUpdate(x => x.id, new Models.Seat
                    {
                        id = 1 * i,
                        status = 1,
                        classType = classType,
                        position = position,
                        flightId = 4
                    });
                }
            }
        }
        public void seedFlightCase1(ARS.Models.ApplicationDbContext context)
        {
            context.Flights.AddOrUpdate(x => x.id, new Models.Flight
            {
                id = 1,
                planeCode = "NE-23",
                status = 1,
                haveStop = false,
                distance = 1000,
                departureDate = DateTime.Now.AddDays(15),
                arrivalDate = DateTime.Now.AddDays(16),
                fromAirportId = 558,
                toAirportId = 555,
                flyTime = 10,
                price = 100000,
                seatAvaiable = 100,
            });
            //context.Seats.AddOrUpdate(x => x.id, new Models.Seat
            //{
            //    id = 1,
            //    flightId = 1,
            //    status = 1,
            //    classType = (int) ARS.Models.SeatType.Bussiness,
            //});
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
            }, new Models.City
            {
                id = 8,
                name = "Changi",
                lattitude = 1.3450,
                longtitude = 103.9832,
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
            }, new Models.CityAirport
            {
                AirportId = 563,
                CityId = 8
            }
            );
        }
        public void seedFlightsCase2(ARS.Models.ApplicationDbContext context)
        {
            context.Flights.AddOrUpdate(x => x.id, new Models.Flight
            {
                id = 2,
                status = 1,
                haveStop = true,
                fromAirportId = 558,
                toAirportId = 16,
                distance = 5003,
                departureDate = DateTime.Parse("10/04/2021"),
                arrivalDate = DateTime.Parse("11/04/2021"),
                flyTime = 11,
                price = 100,
                planeCode = "HK-01",
            },

            new Models.Flight
            {
                id = 3,
                status = 1,
                haveStop = true,
                fromAirportId = 16,
                toAirportId = 558,
                distance = 5003,
                departureDate = DateTime.ParseExact("20/04/2021", "dd/MM/yyyy", null),
                arrivalDate = DateTime.ParseExact("21/04/2021", "dd/MM/yyyy", null),
                flyTime = 11,
                price = 150,
                planeCode = "HK-02",
            }
            );
        }
        public void seedStops(ARS.Models.ApplicationDbContext context)
        {
            context.Stops.AddOrUpdate(x => x.id, new Models.Stop
            {
                id = 1,
                flightId = 2,
                cityId = 8,
                stopOrder = 1
            }, new Models.Stop
            {
                id = 1,
                flightId = 3,
                cityId = 8,
                stopOrder = 1
            }
            );
        }
    }
}
