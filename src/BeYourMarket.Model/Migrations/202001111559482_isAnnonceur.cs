namespace BeYourMarket.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isAnnonceur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsAnnonceurValid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsAnnonceurValid");
        }
    }
}
