using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Medical_Equipment_Rental_Management_System.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        [Column(TypeName = "decimal")]
        public decimal TotalAmount { get; set; }

        public string InvoiceStatus { get; set; }

        [ForeignKey("BookingId")]
        public virtual RentalBooking RentalBooking { get; set; }
    }
}