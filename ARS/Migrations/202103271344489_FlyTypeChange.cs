namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FlyTypeChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "flightType", c => c.Int(nullable: false));
            DropColumn("dbo.Flights", "type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Flights", "type", c => c.Int(nullable: false));
            DropColumn("dbo.Tickets", "flightType");
        }
    }
}
