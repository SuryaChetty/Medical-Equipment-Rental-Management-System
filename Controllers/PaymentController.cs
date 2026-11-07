
using Medical_Equipment_Rental_Management_System.Models;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using QRCoder;

namespace Medical_Equipment_Rental_Management_System.Controllers
{
    public class PaymentController : Controller
    {
        private MediRentContext db = new MediRentContext();

        // ==========================================
        // CUSTOMER: DISPLAY UPI QR CODE
        // GET: Payment/Create/5
        // ==========================================

        [HttpGet]
        public ActionResult Create(int id)
        {
            var booking = db.RentalBookings
                .Include(b => b.Customer)
                .Include(b => b.Equipment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return HttpNotFound();

            if (booking.BookingStatus != "Pending")
            {
                TempData["Error"] =
                    "Only pending bookings can be paid.";

                return RedirectToAction("Index", "RentalBooking");
            }

            bool alreadyPaid = db.Payments.Any(p =>
                p.BookingId == id &&
                p.PaymentStatus == "Success");

            if (alreadyPaid)
            {
                TempData["Error"] =
                    "This booking is already paid.";

                return RedirectToAction("Index", "RentalBooking");
            }

            // Read UPI details from Web.config

            string upiId =
                ConfigurationManager.AppSettings["UPIId"];

            string upiName =
                ConfigurationManager.AppSettings["UPIName"];

            if (string.IsNullOrWhiteSpace(upiId))
            {
                return Content(
                    "Please configure UPIId in Web.config.");
            }

            if (string.IsNullOrWhiteSpace(upiName))
                upiName = "Medical Equipment Rental";

            // Generate UPI payment URL

            string upiUri =
                "upi://pay?pa=" + Uri.EscapeDataString(upiId) +
                "&pn=" + Uri.EscapeDataString(upiName) +
                "&am=" + booking.TotalAmount.ToString(
                    "0.00",
                    System.Globalization.CultureInfo.InvariantCulture) +
                "&cu=INR" +
                "&tn=" + Uri.EscapeDataString(
                    "Booking " + booking.BookingId);

            // Generate QR Code

            using (QRCodeGenerator generator = new QRCodeGenerator())
            using (QRCodeData qrData = generator.CreateQrCode(
                upiUri,
                QRCodeGenerator.ECCLevel.Q))
            {
                PngByteQRCode qrCode = new PngByteQRCode(qrData);

                byte[] qrBytes = qrCode.GetGraphic(20);

                ViewBag.QrCodeBase64 =
                    Convert.ToBase64String(qrBytes);
            }

            ViewBag.UPIId = upiId;
            ViewBag.UPIName = upiName;

            return View(booking);
        }

        // ==========================================
        // CUSTOMER: SUBMIT UPI TRANSACTION ID
        // POST: Payment/Submit
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(
            int bookingId,
            string upiTransactionId)
        {
            var booking = db.RentalBookings
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
                return HttpNotFound();

            if (booking.BookingStatus != "Pending")
            {
                TempData["Error"] =
                    "This booking cannot be paid.";

                return RedirectToAction("Index", "RentalBooking");
            }

            if (string.IsNullOrWhiteSpace(upiTransactionId))
            {
                TempData["Error"] =
                    "Please enter the UPI transaction ID.";

                return RedirectToAction(
                    "Create",
                    new { id = bookingId });
            }

            upiTransactionId = upiTransactionId.Trim();

            if (upiTransactionId.Length > 100)
            {
                TempData["Error"] =
                    "Transaction ID is too long.";

                return RedirectToAction(
                    "Create",
                    new { id = bookingId });
            }

            // Check already paid

            bool alreadyPaid = db.Payments.Any(p =>
                p.BookingId == bookingId &&
                p.PaymentStatus == "Success");

            if (alreadyPaid)
            {
                TempData["Error"] =
                    "This booking is already paid.";

                return RedirectToAction(
                    "Index",
                    "RentalBooking");
            }

            // Check duplicate transaction ID

            bool duplicateTransaction = db.Payments.Any(p =>
                p.UpiTransactionId == upiTransactionId &&
                p.PaymentStatus != "Rejected");

            if (duplicateTransaction)
            {
                TempData["Error"] =
                    "This transaction ID has already been submitted.";

                return RedirectToAction(
                    "Create",
                    new { id = bookingId });
            }

            // Check pending submission

            bool pendingSubmission = db.Payments.Any(p =>
                p.BookingId == bookingId &&
                p.PaymentStatus == "PendingVerification");

            if (pendingSubmission)
            {
                TempData["Error"] =
                    "Your payment is already awaiting verification.";

                return RedirectToAction(
                    "Index",
                    "RentalBooking");
            }

            // Save payment details

            Payment payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalAmount,
                PaymentMethod = "UPI QR",
                PaymentStatus = "PendingVerification",
                UpiTransactionId = upiTransactionId,
                SubmittedDate = DateTime.Now
            };

            db.Payments.Add(payment);
            db.SaveChanges();

            TempData["Success"] =
                "Payment submitted successfully. Waiting for verification.";

            return RedirectToAction(
                "Index",
                "RentalBooking");
        }

        // ==========================================
        // VIEW ALL PAYMENTS
        // GET: Payment/Index
        // ==========================================

        [HttpGet]
        public ActionResult Index()
        {
            var payments = db.Payments
                .Include(p => p.RentalBooking.Customer)
                .Include(p => p.RentalBooking.Equipment)
                .OrderByDescending(p => p.SubmittedDate)
                .ToList();

            return View(payments);
        }

        // ==========================================
        // APPROVE PAYMENT AND GENERATE INVOICE
        // POST: Payment/Approve/5
        // ==========================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Approve(int id)
        //{
        //    Payment payment = db.Payments
        //        .Include(p => p.RentalBooking)
        //        .FirstOrDefault(p => p.PaymentId == id);

        //    if (payment == null)
        //        return HttpNotFound();

        //    if (payment.PaymentStatus != "PendingVerification")
        //    {
        //        TempData["Error"] =
        //            "This payment is not awaiting verification.";

        //        return RedirectToAction("Index");
        //    }

        //    if (payment.RentalBooking == null ||
        //        payment.RentalBooking.BookingStatus != "Pending")
        //    {
        //        TempData["Error"] =
        //            "The related booking is not pending.";

        //        return RedirectToAction("Index");
        //    }

        //    // STEP 1: APPROVE PAYMENT

        //    payment.PaymentStatus = "Success";
        //    payment.VerifiedDate = DateTime.Now;
        //    payment.VerifiedBy = "Verified";

        //    // STEP 2: CONFIRM BOOKING

        //    payment.RentalBooking.BookingStatus = "Confirmed";

        //    // STEP 3: CHECK EXISTING INVOICE

        //    bool invoiceExists = db.Invoices.Any(
        //        i => i.BookingId == payment.BookingId
        //    );

        //    // STEP 4: GENERATE INVOICE

        //    if (!invoiceExists)
        //    {
        //        Invoice invoice = new Invoice
        //        {
        //            BookingId = payment.BookingId,

        //            InvoiceNumber = "INV-" +
        //                DateTime.Now.ToString("yyyyMMdd") +
        //                "-" + payment.BookingId,

        //            InvoiceDate = DateTime.Now,

        //            TotalAmount = payment.Amount,

        //            InvoiceStatus = "Generated"
        //        };

        //        db.Invoices.Add(invoice);
        //    }

        //    // STEP 5: SAVE PAYMENT, BOOKING AND INVOICE

        //    db.SaveChanges();

        //    TempData["Success"] =
        //        "Payment approved, booking confirmed and invoice generated successfully.";

        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            // 1. Get payment details
            Payment payment = db.Payments
                .Include(p => p.RentalBooking)
                .FirstOrDefault(p => p.PaymentId == id);

            if (payment == null)
            {
                return HttpNotFound();
            }

            // 2. Check payment status
            if (payment.PaymentStatus != "PendingVerification")
            {
                TempData["Error"] = "Payment is not awaiting verification.";
                return RedirectToAction("Index");
            }

            if (payment.RentalBooking == null ||
                payment.RentalBooking.BookingStatus != "Pending")
            {
                TempData["Error"] = "Booking is not pending.";
                return RedirectToAction("Index");
            }

            // 3. Approve payment
            payment.PaymentStatus = "Success";
            payment.VerifiedDate = DateTime.Now;
            payment.VerifiedBy = "Verified";

            // 4. Confirm booking
            payment.RentalBooking.BookingStatus = "Confirmed";

            // 5. Automatically generate invoice
            bool invoiceExists = db.Invoices.Any(
                i => i.BookingId == payment.BookingId
            );

            if (!invoiceExists)
            {
                Invoice invoice = new Invoice
                {
                    BookingId = payment.BookingId,
                    InvoiceNumber = "INV-" +
                        DateTime.Now.ToString("yyyyMMddHHmmss") +
                        "-" + payment.BookingId,

                    InvoiceDate = DateTime.Now,
                    TotalAmount = payment.Amount,
                    InvoiceStatus = "Generated"
                };

                db.Invoices.Add(invoice);
            }

            // 6. Save all changes to database
            db.SaveChanges();

            TempData["Success"] =
                "Payment approved and invoice generated successfully.";

            return RedirectToAction("Index");
        }
        // ==========================================
        // REJECT PAYMENT
        // POST: Payment/Reject/5
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id)
        {
            Payment payment = db.Payments
                .FirstOrDefault(p => p.PaymentId == id);

            if (payment == null)
                return HttpNotFound();

            if (payment.PaymentStatus != "PendingVerification")
            {
                TempData["Error"] =
                    "This payment is not awaiting verification.";

                return RedirectToAction("Index");
            }

            payment.PaymentStatus = "Rejected";
            payment.VerifiedDate = DateTime.Now;
            payment.VerifiedBy = "Verified";

            // Booking remains Pending for another payment attempt.

            db.SaveChanges();

            TempData["Success"] =
                "Payment rejected.";

            return RedirectToAction("Index");
        }

        // ==========================================
        // DISPOSE DATABASE CONTEXT
        // ==========================================

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
