namespace BeYourMarket.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class promail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ProEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ProEmail");
        }
    }
}
