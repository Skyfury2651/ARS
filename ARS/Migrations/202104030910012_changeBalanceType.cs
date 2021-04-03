namespace ARS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeBalanceType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "balance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "balance", c => c.Int(nullable: false));
        }
    }
}
