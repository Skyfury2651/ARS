namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeRelationFlightTicket : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "flightId", "dbo.Flights");
            DropIndex("dbo.Tickets", new[] { "flightId" });
            CreateTable(
                "dbo.TicketFlights",
                c => new
                    {
                        TicketId = c.Int(nullable: false),
                        FlightId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TicketId, t.FlightId });
            
            AddColumn("dbo.Flights", "TicketFlight_TicketId", c => c.Int());
            AddColumn("dbo.Flights", "TicketFlight_FlightId", c => c.Int());
            AddColumn("dbo.Tickets", "TicketFlight_TicketId", c => c.Int());
            AddColumn("dbo.Tickets", "TicketFlight_FlightId", c => c.Int());
            CreateIndex("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            CreateIndex("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            AddForeignKey("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights", new[] { "TicketId", "FlightId" });
            AddForeignKey("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights", new[] { "TicketId", "FlightId" });
            DropColumn("dbo.Tickets", "flightId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "flightId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights");
            DropForeignKey("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights");
            DropIndex("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            DropIndex("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            DropColumn("dbo.Tickets", "TicketFlight_FlightId");
            DropColumn("dbo.Tickets", "TicketFlight_TicketId");
            DropColumn("dbo.Flights", "TicketFlight_FlightId");
            DropColumn("dbo.Flights", "TicketFlight_TicketId");
            DropTable("dbo.TicketFlights");
            CreateIndex("dbo.Tickets", "flightId");
            AddForeignKey("dbo.Tickets", "flightId", "dbo.Flights", "id", cascadeDelete: true);
        }
    }
}
