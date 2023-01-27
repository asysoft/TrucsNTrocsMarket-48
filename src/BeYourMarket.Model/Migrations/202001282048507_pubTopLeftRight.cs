namespace BeYourMarket.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pubTopLeftRight : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.PubLocations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PageName = c.String(),
                        TopFileName = c.String(),
                        TopIsFree = c.Boolean(nullable: false),
                        TopFileNum = c.Int(nullable: false),
                        LeftFileName = c.String(),
                        LeftIsFree = c.Boolean(nullable: false),
                        LeftFileNum = c.Int(nullable: false),
                        RightFileName = c.String(),
                        RightIsFree = c.Boolean(nullable: false),
                        RightFileNum = c.Int(nullable: false),
                        LeftAspNetUser_Id = c.String(maxLength: 128),
                        RightAspNetUser_Id = c.String(maxLength: 128),
                        TopAspNetUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.LeftAspNetUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.RightAspNetUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.TopAspNetUser_Id)
                .Index(t => t.LeftAspNetUser_Id)
                .Index(t => t.RightAspNetUser_Id)
                .Index(t => t.TopAspNetUser_Id);
            
            
        }
        
        public override void Down()
        {

        }
    }
}
