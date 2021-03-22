namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableAirport : DbMigration
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
            
            AddColumn("dbo.AspNetUsers", "firstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "lastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "sex", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "age", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "preferedCreditCardNumber", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "skyMiles", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "status");
            DropColumn("dbo.AspNetUsers", "skyMiles");
            DropColumn("dbo.AspNetUsers", "preferedCreditCardNumber");
            DropColumn("dbo.AspNetUsers", "age");
            DropColumn("dbo.AspNetUsers", "sex");
            DropColumn("dbo.AspNetUsers", "lastName");
            DropColumn("dbo.AspNetUsers", "firstName");
            DropTable("dbo.Airports");
        }
    }
}
