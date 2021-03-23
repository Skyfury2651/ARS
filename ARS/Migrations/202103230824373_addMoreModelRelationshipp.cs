namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMoreModelRelationshipp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        longtitude = c.Double(nullable: false),
                        lattitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Flights",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        planeCode = c.String(),
                        fromAirportId = c.Int(nullable: false),
                        toAirportId = c.Int(nullable: false),
                        type = c.Int(nullable: false),
                        departureDate = c.DateTime(nullable: false),
                        arrivalDate = c.DateTime(nullable: false),
                        flyTime = c.Double(nullable: false),
                        price = c.Double(nullable: false),
                        distance = c.Double(nullable: false),
                        haveStop = c.Boolean(nullable: false),
                        status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Airports", t => t.fromAirportId, cascadeDelete: false)
                .ForeignKey("dbo.Airports", t => t.toAirportId, cascadeDelete: false)
                .Index(t => t.fromAirportId)
                .Index(t => t.toAirportId);
            
            CreateTable(
                "dbo.Seats",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        position = c.String(),
                        flightId = c.Int(nullable: false),
                        classType = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Flights", t => t.flightId, cascadeDelete: true)
                .Index(t => t.flightId);
            
            CreateTable(
                "dbo.Stops",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        flightId = c.Int(nullable: false),
                        cityId = c.Int(nullable: false),
                        stopOrder = c.Int(nullable: false),
                        Airport_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Airports", t => t.Airport_id)
                .ForeignKey("dbo.Cities", t => t.cityId, cascadeDelete: true)
                .Index(t => t.cityId)
                .Index(t => t.Airport_id);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(maxLength: 128),
                        flightId = c.Int(nullable: false),
                        seatId = c.Int(nullable: false),
                        type = c.Int(nullable: false),
                        blockingNumber = c.String(),
                        confirmNumber = c.String(),
                        cancelNumber = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Flights", t => t.flightId, cascadeDelete: true)
                .ForeignKey("dbo.Seats", t => t.seatId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.userId)
                .Index(t => t.userId)
                .Index(t => t.flightId)
                .Index(t => t.seatId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ticketId = c.Int(nullable: false),
                        flightId = c.Int(nullable: false),
                        price = c.Double(nullable: false),
                        type = c.Int(nullable: false),
                        createdAt = c.DateTime(nullable: false),
                        updatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Flights", t => t.flightId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.ticketId, cascadeDelete: false)
                .Index(t => t.ticketId)
                .Index(t => t.flightId);
            
            CreateTable(
                "dbo.CityAirports",
                c => new
                    {
                        City_id = c.Int(nullable: false),
                        Airport_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.City_id, t.Airport_id })
                .ForeignKey("dbo.Cities", t => t.City_id, cascadeDelete: true)
                .ForeignKey("dbo.Airports", t => t.Airport_id, cascadeDelete: true)
                .Index(t => t.City_id)
                .Index(t => t.Airport_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "ticketId", "dbo.Tickets");
            DropForeignKey("dbo.Transactions", "flightId", "dbo.Flights");
            DropForeignKey("dbo.Tickets", "userId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "seatId", "dbo.Seats");
            DropForeignKey("dbo.Tickets", "flightId", "dbo.Flights");
            DropForeignKey("dbo.Stops", "cityId", "dbo.Cities");
            DropForeignKey("dbo.Stops", "Airport_id", "dbo.Airports");
            DropForeignKey("dbo.Seats", "flightId", "dbo.Flights");
            DropForeignKey("dbo.Flights", "toAirportId", "dbo.Airports");
            DropForeignKey("dbo.Flights", "fromAirportId", "dbo.Airports");
            DropForeignKey("dbo.CityAirports", "Airport_id", "dbo.Airports");
            DropForeignKey("dbo.CityAirports", "City_id", "dbo.Cities");
            DropIndex("dbo.CityAirports", new[] { "Airport_id" });
            DropIndex("dbo.CityAirports", new[] { "City_id" });
            DropIndex("dbo.Transactions", new[] { "flightId" });
            DropIndex("dbo.Transactions", new[] { "ticketId" });
            DropIndex("dbo.Tickets", new[] { "seatId" });
            DropIndex("dbo.Tickets", new[] { "flightId" });
            DropIndex("dbo.Tickets", new[] { "userId" });
            DropIndex("dbo.Stops", new[] { "Airport_id" });
            DropIndex("dbo.Stops", new[] { "cityId" });
            DropIndex("dbo.Seats", new[] { "flightId" });
            DropIndex("dbo.Flights", new[] { "toAirportId" });
            DropIndex("dbo.Flights", new[] { "fromAirportId" });
            DropTable("dbo.CityAirports");
            DropTable("dbo.Transactions");
            DropTable("dbo.Tickets");
            DropTable("dbo.Stops");
            DropTable("dbo.Seats");
            DropTable("dbo.Flights");
            DropTable("dbo.Cities");
        }
    }
}
