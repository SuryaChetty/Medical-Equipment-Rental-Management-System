namespace Medical_Equipment_Rental_Management_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Address = c.String(nullable: false, maxLength: 250),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        EquipmentId = c.Int(nullable: false, identity: true),
                        EquipmentName = c.String(nullable: false, maxLength: 100),
                        Category = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        PricePerDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Stock = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EquipmentId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        InvoiceNumber = c.String(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceStatus = c.String(),
                    })
                .PrimaryKey(t => t.InvoiceId)
                .ForeignKey("dbo.RentalBookings", t => t.BookingId, cascadeDelete: true)
                .Index(t => t.BookingId);
            
            CreateTable(
                "dbo.RentalBookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        EquipmentId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookingStatus = c.String(),
                        BookingDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Equipments", t => t.EquipmentId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.EquipmentId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(),
                        PaymentStatus = c.String(),
                        RazorpayOrderId = c.String(),
                        RazorpayPaymentId = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.RentalBookings", t => t.BookingId, cascadeDelete: true)
                .Index(t => t.BookingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "BookingId", "dbo.RentalBookings");
            DropForeignKey("dbo.Invoices", "BookingId", "dbo.RentalBookings");
            DropForeignKey("dbo.RentalBookings", "EquipmentId", "dbo.Equipments");
            DropForeignKey("dbo.RentalBookings", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Payments", new[] { "BookingId" });
            DropIndex("dbo.RentalBookings", new[] { "EquipmentId" });
            DropIndex("dbo.RentalBookings", new[] { "CustomerId" });
            DropIndex("dbo.Invoices", new[] { "BookingId" });
            DropTable("dbo.Payments");
            DropTable("dbo.RentalBookings");
            DropTable("dbo.Invoices");
            DropTable("dbo.Equipments");
            DropTable("dbo.Customers");
        }
    }
}
