using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Medical_Equipment_Rental_Management_System.Models
{
    public class MediRentContext : DbContext
    {
        public MediRentContext() : base("MediRentConnection")
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Equipment> Equipment { get; set; }

        public DbSet<RentalBooking> RentalBookings { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
    }
}