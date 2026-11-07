
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medical_Equipment_Rental_Management_System.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        [StringLength(30)]
        public string PaymentMethod { get; set; }

        // PendingVerification, Success, Rejected
        [Required]
        [StringLength(30)]
        public string PaymentStatus { get; set; }

        [StringLength(100)]
        public string UpiTransactionId { get; set; }

        public DateTime SubmittedDate { get; set; }

        public DateTime? VerifiedDate { get; set; }

        [StringLength(100)]
        public string VerifiedBy { get; set; }

        // Razorpay details
        [StringLength(100)]
        public string RazorpayOrderId { get; set; }

        [StringLength(100)]
        public string RazorpayPaymentId { get; set; }

        [StringLength(200)]
        public string RazorpaySignature { get; set; }

        [ForeignKey("BookingId")]
        public virtual RentalBooking RentalBooking { get; set; }
    }
}
