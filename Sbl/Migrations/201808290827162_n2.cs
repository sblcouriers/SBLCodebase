namespace Sbl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class n2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RouteCharges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssociateId = c.Int(nullable: false),
                        RouteId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        SetAsCredit = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Associates", t => t.AssociateId, cascadeDelete: true)
                .Index(t => t.AssociateId);
            
            CreateTable(
                "dbo.VehicleRentalPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubRentalId = c.Int(nullable: false),
                        RentalPrice = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubRentals", t => t.SubRentalId, cascadeDelete: true)
                .Index(t => t.SubRentalId);
            
            AddColumn("dbo.Associates", "UserId", c => c.String());
            AddColumn("dbo.RouteAllocations", "AuthPoc", c => c.Boolean(nullable: false));
            AddColumn("dbo.RouteAllocations", "AuthPayroll", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VehicleRentalPrices", "SubRentalId", "dbo.SubRentals");
            DropForeignKey("dbo.RouteCharges", "AssociateId", "dbo.Associates");
            DropIndex("dbo.VehicleRentalPrices", new[] { "SubRentalId" });
            DropIndex("dbo.RouteCharges", new[] { "AssociateId" });
            DropColumn("dbo.RouteAllocations", "AuthPayroll");
            DropColumn("dbo.RouteAllocations", "AuthPoc");
            DropColumn("dbo.Associates", "UserId");
            DropTable("dbo.VehicleRentalPrices");
            DropTable("dbo.RouteCharges");
        }
    }
}
