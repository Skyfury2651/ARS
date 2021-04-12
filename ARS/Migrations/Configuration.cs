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
            this.seedFlightsCase2(context);
            this.seedSeat(context);
            this.seedStops(context);
        }
        public void seedSeat(ARS.Models.ApplicationDbContext context)
        {
            int seatId = 1;
            int seatStatus = 2;
            for (int k = 1; k < 8; k++)
            {
                //int notAvailable = 0;
                for (int i = 1; i < 5; i++)
                {
                    int classType = (int)Models.SeatType.FirstClass;
                    if (i > 2)
                    {
                        classType = (int)Models.SeatType.ClubClass;
                        seatStatus = 0;
                    }

                    for (int j = 1; j < 7; j++)
                    {
                        string position = i.ToString() + Convert.ToChar(j + 64);
                        context.Seats.AddOrUpdate(x => x.id, new Models.Seat
                        {
                            id = seatId,
                            status = seatStatus,
                            classType = classType,
                            position = position,
                            flightId = k
                        });
                        seatId += 1;
                    }
                }
                seatStatus = 1;
                for (int i = 5; i < 19; i++)
                {

                    int classType = (int)Models.SeatType.NonSmoking;
                    if (i > 7)
                    {
                        classType = (int)Models.SeatType.Bussiness;
                    }
                    if (i > 13)
                    {
                        classType = (int)Models.SeatType.Smoking;
                    }
                    for (int j = 1; j < 7; j++)
                    {
                        string position = i.ToString() + Convert.ToChar(j + 64);
                        context.Seats.AddOrUpdate(x => x.id, new Models.Seat
                        {
                            id = seatId,
                            status = seatStatus,
                            classType = classType,
                            position = position,
                            flightId = k
                        });
                        seatId += 1;
                    }
                }
            }
        }
        public void seedCity(ARS.Models.ApplicationDbContext context)
        {
            context.Cities.AddOrUpdate(x => x.id, new Models.City
            {
                id = 1,
                name = "DaNang",
                lattitude = 21.028693,
                longtitude = 105.841616,
                country = "Viet Nam",
            }, new Models.City
            {
                id = 2,
                name = "Ha Noi",
                lattitude = 21.028693,
                longtitude = 105.841616,
                country = "Viet Nam",
            }, new Models.City
            {
                id = 3,
                name = "HoChiMinh",
                lattitude = 21.028693,
                longtitude = 105.841616,
                country = "Viet Nam",
            }, new Models.City
            {
                id = 4,
                name = "DuongDong",
                lattitude = 9.930741,
                longtitude = 76.267348,
                country = "India",
            }, new Models.City
            {
                id = 5,
                name = "Kochi",
                lattitude = 9.930740,
                longtitude = 76.267348,
                country = "India",
            }, new Models.City
            {
                id = 6,
                name = "Calicut",
                lattitude = 9.930739,
                longtitude = 76.267348,
                country = "India",
            }, new Models.City
            {
                id = 7,
                name = "AnDoCity",
                lattitude = 9.930741,
                longtitude = 76.267348,
                country = "India",
            }, new Models.City
            {
                id = 8,
                name = "Changi",
                lattitude = 1.3450,
                longtitude = 103.9832,
                country = "Singapore",
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
            context.Flights.AddOrUpdate(x => x.id,
                new Models.Flight
                {
                    id = 1,
                    planeCode = "NE-23",
                    status = 1,
                    haveStop = false,
                    distance = 1000,
                    departureDate = DateTime.ParseExact("10/04/2021", "dd/MM/yyyy", null),
                    arrivalDate = DateTime.ParseExact("11/04/2021", "dd/MM/yyyy", null),
                    fromAirportId = 558,
                    toAirportId = 555,
                    flyTime = 10,
                    price = 80,
                    seatAvaiable = 60,
                },
                new Models.Flight
                {
                    id = 2,
                    status = 1,
                    haveStop = true,
                    fromAirportId = 558,
                    toAirportId = 16,
                    distance = 5003,
                    departureDate = DateTime.ParseExact("10/04/2021", "dd/MM/yyyy", null),
                    arrivalDate = DateTime.ParseExact("11/04/2021", "dd/MM/yyyy", null),
                    flyTime = 11,
                    price = 100,
                    planeCode = "HK-01",
                    seatAvaiable = 100,
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
                    seatAvaiable = 100,
                },
                 new Models.Flight
                 {
                     id = 4,
                     status = 1,
                     haveStop = false,
                     fromAirportId = 558,
                     toAirportId = 16,
                     distance = 5003,
                     departureDate = DateTime.ParseExact("10/04/2021", "dd/MM/yyyy", null),
                     arrivalDate = DateTime.ParseExact("11/04/2021", "dd/MM/yyyy", null),
                     flyTime = 11,
                     price = 90,
                     planeCode = "HK-03",
                     seatAvaiable = 100,
                 },
                new Models.Flight
                {
                    id = 5,
                    status = 1,
                    haveStop = false,
                    fromAirportId = 16,
                    toAirportId = 558,
                    distance = 5003,
                    departureDate = DateTime.ParseExact("20/04/2021", "dd/MM/yyyy", null),
                    arrivalDate = DateTime.ParseExact("21/04/2021", "dd/MM/yyyy", null),
                    flyTime = 11,
                    price = 90,
                    planeCode = "HK-04",
                    seatAvaiable = 100,
                },
                new Models.Flight
                {
                    id = 6,
                    planeCode = "NE-23-1",
                    status = 1,
                    haveStop = false,
                    distance = 1000,
                    departureDate = DateTime.ParseExact("20/04/2021", "dd/MM/yyyy", null),
                    arrivalDate = DateTime.ParseExact("22/04/2021", "dd/MM/yyyy", null),
                    fromAirportId = 558,
                    toAirportId = 555,
                    flyTime = 10,
                    price = 100,
                    seatAvaiable = 60,
                },
                new Models.Flight
                {
                    id = 7,
                    planeCode = "NE-23-2",
                    status = 1,
                    haveStop = false,
                    distance = 1000,
                    departureDate = DateTime.ParseExact("15/06/2021", "dd/MM/yyyy", null),
                    arrivalDate = DateTime.ParseExact("16/06/2021", "dd/MM/yyyy", null),
                    fromAirportId = 558,
                    toAirportId = 555,
                    flyTime = 10,
                    price = 200,
                    seatAvaiable = 60,
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
                id = 2,
                flightId = 3,
                cityId = 8,
                stopOrder = 1
            }
            );
        }

        public void seedAirport(ARS.Models.ApplicationDbContext context)
        {
            context.Airports.AddOrUpdate(x => x.id, new Models.Airport
            {
                id = 15,
                code = "IN",
                iso_country = "IN",
                municipality = "Kochi",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "INS Garuda / Willingdon Island Naval Air Station"

            }, new Models.Airport
            {
                id = 16,
                code = "IN",
                iso_country = "IN",
                municipality = "Kochi",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "Cochin International Airport",
            }, new Models.Airport
            {
                id = 17,
                code = "IN",
                iso_country = "IN",
                municipality = "Kochi",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "Calicut International Airport",
            }, new Models.Airport
            {
                id = 555,
                code = "VN",
                iso_country = "VN",
                municipality = "Da Nang",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "Nuoc Man (Marble Mountain) Air Facility",
            }, new Models.Airport
            {
                id = 556,
                code = "VN",
                iso_country = "VN",
                municipality = "Da Nang",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "Red Beach Airfield",
            }, new Models.Airport
            {
                id = 558,
                code = "VN",
                iso_country = "VN",
                municipality = "Nam Tu Liem, Hanoi",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "Hoa Lac Airport",
            }, new Models.Airport
            {
                id = 559,
                code = "VN",
                iso_country = "VN",
                municipality = "Nam Tu Liem, Hanoi",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "International Airport NoiBai",
            }, new Models.Airport
            {
                id = 560,
                code = "VN",
                iso_country = "VN",
                municipality = "Nam Tu Liem, Hanoi",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "Bach Mai Airport",
            }, new Models.Airport
            {
                id = 563,
                code = "SG",
                iso_country = "SG",
                municipality = "Nam Tu Liem, Hanoi",
                lattitude = 0,
                longtitude = 0,
                status = 1,
                name = "Changi Airport",
            }
            );
        }
    }
}