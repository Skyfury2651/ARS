namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFlightIdAtTransaction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "flightId", "dbo.Flights");
            DropIndex("dbo.Transactions", new[] { "flightId" });
            DropColumn("dbo.Transactions", "flightId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "flightId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "flightId");
            AddForeignKey("dbo.Transactions", "flightId", "dbo.Flights", "id", cascadeDelete: true);
        }
    }
}
