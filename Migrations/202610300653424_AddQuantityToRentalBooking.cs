namespace Medical_Equipment_Rental_Management_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuantityToRentalBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RentalBookings", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.RentalBookings", "BookingStatus", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RentalBookings", "BookingStatus", c => c.String());
            DropColumn("dbo.RentalBookings", "Quantity");
        }
    }
}
