namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTableData : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights");
            DropForeignKey("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights");
            DropIndex("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            DropIndex("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            CreateTable(
                "dbo.TicketSeats",
                c => new
                    {
                        SeatId = c.Int(nullable: false),
                        TicketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SeatId, t.TicketId });
            
            AddColumn("dbo.Seats", "TicketSeat_SeatId", c => c.Int());
            AddColumn("dbo.Seats", "TicketSeat_TicketId", c => c.Int());
            AddColumn("dbo.Tickets", "TicketSeat_SeatId", c => c.Int());
            AddColumn("dbo.Tickets", "TicketSeat_TicketId", c => c.Int());
            CreateIndex("dbo.Seats", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" });
            CreateIndex("dbo.Tickets", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" });
            AddForeignKey("dbo.Seats", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" }, "dbo.TicketSeats", new[] { "SeatId", "TicketId" });
            AddForeignKey("dbo.Tickets", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" }, "dbo.TicketSeats", new[] { "SeatId", "TicketId" });
            DropColumn("dbo.Flights", "TicketFlight_TicketId");
            DropColumn("dbo.Flights", "TicketFlight_FlightId");
            DropColumn("dbo.Tickets", "TicketFlight_TicketId");
            DropColumn("dbo.Tickets", "TicketFlight_FlightId");
            DropTable("dbo.TicketFlights");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TicketFlights",
                c => new
                    {
                        TicketId = c.Int(nullable: false),
                        FlightId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TicketId, t.FlightId });
            
            AddColumn("dbo.Tickets", "TicketFlight_FlightId", c => c.Int());
            AddColumn("dbo.Tickets", "TicketFlight_TicketId", c => c.Int());
            AddColumn("dbo.Flights", "TicketFlight_FlightId", c => c.Int());
            AddColumn("dbo.Flights", "TicketFlight_TicketId", c => c.Int());
            DropForeignKey("dbo.Tickets", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" }, "dbo.TicketSeats");
            DropForeignKey("dbo.Seats", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" }, "dbo.TicketSeats");
            DropIndex("dbo.Tickets", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" });
            DropIndex("dbo.Seats", new[] { "TicketSeat_SeatId", "TicketSeat_TicketId" });
            DropColumn("dbo.Tickets", "TicketSeat_TicketId");
            DropColumn("dbo.Tickets", "TicketSeat_SeatId");
            DropColumn("dbo.Seats", "TicketSeat_TicketId");
            DropColumn("dbo.Seats", "TicketSeat_SeatId");
            DropTable("dbo.TicketSeats");
            CreateIndex("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            CreateIndex("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" });
            AddForeignKey("dbo.Tickets", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights", new[] { "TicketId", "FlightId" });
            AddForeignKey("dbo.Flights", new[] { "TicketFlight_TicketId", "TicketFlight_FlightId" }, "dbo.TicketFlights", new[] { "TicketId", "FlightId" });
        }
    }
}
