namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "status", c => c.Int(nullable: false));
            AddColumn("dbo.Transactions", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "status");
            DropColumn("dbo.Tickets", "status");
        }
    }
}
