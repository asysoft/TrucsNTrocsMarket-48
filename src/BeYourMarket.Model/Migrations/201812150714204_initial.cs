namespace BeYourMarket.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {          
            CreateTable(
                "dbo.PrepaidCards",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 128),
                        NumSerie = c.Int(nullable: false),
                        NumLot = c.Int(nullable: false),
                        DateGeneration = c.DateTime(nullable: false),
                        DateFinValidite = c.DateTime(nullable: false),
                        IsValid = c.Boolean(nullable: false),
                        IsActif = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            

            
        }
        
        public override void Down()
        {
 
            DropTable("dbo.PrepaidCards");
 
        }
    }
}
