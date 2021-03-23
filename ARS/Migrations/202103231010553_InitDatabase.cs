namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Airports",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        code = c.String(),
                        iso_country = c.String(),
                        municipality = c.String(),
                        lattitude = c.Double(nullable: false),
                        longtitude = c.Double(nullable: false),
                        status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.CityAirports",
                c => new
                    {
                        CityId = c.Int(nullable: false),
                        AirportId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CityId, t.AirportId })
                .ForeignKey("dbo.Airports", t => t.AirportId, cascadeDelete: true)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId)
                .Index(t => t.AirportId);
            
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
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                .ForeignKey("dbo.Flights", t => t.flightId, cascadeDelete: false)
                .Index(t => t.flightId);
            
            CreateTable(
                "dbo.Stops",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        flightId = c.Int(nullable: false),
                        cityId = c.Int(nullable: false),
                        stopOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Cities", t => t.cityId, cascadeDelete: true)
                .ForeignKey("dbo.Flights", t => t.flightId, cascadeDelete: true)
                .Index(t => t.flightId)
                .Index(t => t.cityId);
            
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
                .ForeignKey("dbo.Seats", t => t.seatId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.userId)
                .Index(t => t.userId)
                .Index(t => t.flightId)
                .Index(t => t.seatId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        firstName = c.String(),
                        lastName = c.String(),
                        balance = c.Int(nullable: false),
                        address = c.String(),
                        sex = c.Int(nullable: false),
                        age = c.Int(nullable: false),
                        preferedCreditCardNumber = c.Int(nullable: false),
                        skyMiles = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "ticketId", "dbo.Tickets");
            DropForeignKey("dbo.Transactions", "flightId", "dbo.Flights");
            DropForeignKey("dbo.Tickets", "userId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "seatId", "dbo.Seats");
            DropForeignKey("dbo.Tickets", "flightId", "dbo.Flights");
            DropForeignKey("dbo.Stops", "flightId", "dbo.Flights");
            DropForeignKey("dbo.Stops", "cityId", "dbo.Cities");
            DropForeignKey("dbo.Seats", "flightId", "dbo.Flights");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Flights", "toAirportId", "dbo.Airports");
            DropForeignKey("dbo.Flights", "fromAirportId", "dbo.Airports");
            DropForeignKey("dbo.CityAirports", "CityId", "dbo.Cities");
            DropForeignKey("dbo.CityAirports", "AirportId", "dbo.Airports");
            DropIndex("dbo.Transactions", new[] { "flightId" });
            DropIndex("dbo.Transactions", new[] { "ticketId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Tickets", new[] { "seatId" });
            DropIndex("dbo.Tickets", new[] { "flightId" });
            DropIndex("dbo.Tickets", new[] { "userId" });
            DropIndex("dbo.Stops", new[] { "cityId" });
            DropIndex("dbo.Stops", new[] { "flightId" });
            DropIndex("dbo.Seats", new[] { "flightId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Flights", new[] { "toAirportId" });
            DropIndex("dbo.Flights", new[] { "fromAirportId" });
            DropIndex("dbo.CityAirports", new[] { "AirportId" });
            DropIndex("dbo.CityAirports", new[] { "CityId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Tickets");
            DropTable("dbo.Stops");
            DropTable("dbo.Seats");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Flights");
            DropTable("dbo.Cities");
            DropTable("dbo.CityAirports");
            DropTable("dbo.Airports");
        }
    }
}
