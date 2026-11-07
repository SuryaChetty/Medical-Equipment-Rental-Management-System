namespace Medical_Equipment_Rental_Management_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRazorpayFieldsToPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "UpiTransactionId", c => c.String(maxLength: 100));
            AddColumn("dbo.Payments", "SubmittedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Payments", "VerifiedDate", c => c.DateTime());
            AddColumn("dbo.Payments", "VerifiedBy", c => c.String(maxLength: 100));
            AlterColumn("dbo.Payments", "PaymentMethod", c => c.String(maxLength: 30));
            AlterColumn("dbo.Payments", "PaymentStatus", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Payments", "RazorpayOrderId", c => c.String(maxLength: 100));
            AlterColumn("dbo.Payments", "RazorpayPaymentId", c => c.String(maxLength: 100));
            AlterColumn("dbo.Payments", "RazorpaySignature", c => c.String(maxLength: 200));
            DropColumn("dbo.Payments", "PaymentDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "PaymentDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Payments", "RazorpaySignature", c => c.String());
            AlterColumn("dbo.Payments", "RazorpayPaymentId", c => c.String());
            AlterColumn("dbo.Payments", "RazorpayOrderId", c => c.String());
            AlterColumn("dbo.Payments", "PaymentStatus", c => c.String());
            AlterColumn("dbo.Payments", "PaymentMethod", c => c.String());
            DropColumn("dbo.Payments", "VerifiedBy");
            DropColumn("dbo.Payments", "VerifiedDate");
            DropColumn("dbo.Payments", "SubmittedDate");
            DropColumn("dbo.Payments", "UpiTransactionId");
        }
    }
}
