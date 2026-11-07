namespace Medical_Equipment_Rental_Management_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRazorpayPaymentFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Payments", "PaymentMethod", c => c.String());
            AlterColumn("dbo.Payments", "PaymentStatus", c => c.String());
            AlterColumn("dbo.Payments", "RazorpayOrderId", c => c.String());
            AlterColumn("dbo.Payments", "RazorpayPaymentId", c => c.String());
            AlterColumn("dbo.Payments", "RazorpaySignature", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Payments", "RazorpaySignature", c => c.String(maxLength: 200));
            AlterColumn("dbo.Payments", "RazorpayPaymentId", c => c.String(maxLength: 100));
            AlterColumn("dbo.Payments", "RazorpayOrderId", c => c.String(maxLength: 100));
            AlterColumn("dbo.Payments", "PaymentStatus", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Payments", "PaymentMethod", c => c.String(maxLength: 50));
        }
    }
}
