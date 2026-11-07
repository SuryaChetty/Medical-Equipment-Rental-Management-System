namespace Medical_Equipment_Rental_Management_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRazorpaySignatureToPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "RazorpaySignature", c => c.String(maxLength: 200));
            AlterColumn("dbo.Payments", "PaymentMethod", c => c.String(maxLength: 50));
            AlterColumn("dbo.Payments", "PaymentStatus", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Payments", "RazorpayOrderId", c => c.String(maxLength: 100));
            AlterColumn("dbo.Payments", "RazorpayPaymentId", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Payments", "RazorpayPaymentId", c => c.String());
            AlterColumn("dbo.Payments", "RazorpayOrderId", c => c.String());
            AlterColumn("dbo.Payments", "PaymentStatus", c => c.String());
            AlterColumn("dbo.Payments", "PaymentMethod", c => c.String());
            DropColumn("dbo.Payments", "RazorpaySignature");
        }
    }
}
