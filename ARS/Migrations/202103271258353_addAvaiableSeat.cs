namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAvaiableSeat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flights", "seatAvaiable", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "seatAvaiableBusiness", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "seatAvaiableFirst", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "seatAvaiableClub", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "seatAvaiableSmoking", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "seatAvaiableNonSmoking", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Flights", "seatAvaiableNonSmoking");
            DropColumn("dbo.Flights", "seatAvaiableSmoking");
            DropColumn("dbo.Flights", "seatAvaiableClub");
            DropColumn("dbo.Flights", "seatAvaiableFirst");
            DropColumn("dbo.Flights", "seatAvaiableBusiness");
            DropColumn("dbo.Flights", "seatAvaiable");
        }
    }
}
