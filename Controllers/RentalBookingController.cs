
using Medical_Equipment_Rental_Management_System.Models;

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MediRent.Controllers
{
    public class RentalBookingController : Controller
    {
        private MediRentContext db = new MediRentContext();

        private const string Pending = "Pending";
        private const string Confirmed = "Confirmed";
        private const string Cancelled = "Cancelled";
        private const string Completed = "Completed";

        // GET: RentalBooking
        public ActionResult Index()
        {
            var bookings = db.RentalBookings
                .Include(b => b.Customer)
                .Include(b => b.Equipment)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(bookings);
        }

        // GET: RentalBooking/Create
        public ActionResult Create()
        {
            LoadDropdowns();

            return View();
        }

        // POST: RentalBooking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RentalBooking booking)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(booking.CustomerId, booking.EquipmentId);
                return View(booking);
            }

            var customer = db.Customers
                .FirstOrDefault(c => c.CustomerId == booking.CustomerId
                                  && c.IsActive);

            var equipment = db.Equipment
                .FirstOrDefault(e => e.EquipmentId == booking.EquipmentId
                                  && e.IsActive);

            if (customer == null)
            {
                ModelState.AddModelError("CustomerId",
                    "Please select an active customer.");
            }

            if (equipment == null)
            {
                ModelState.AddModelError("EquipmentId",
                    "Please select active equipment.");
            }

            if (customer == null || equipment == null)
            {
                LoadDropdowns(booking.CustomerId, booking.EquipmentId);
                return View(booking);
            }

            if (booking.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("StartDate",
                    "Start date cannot be in the past.");
            }

            if (booking.EndDate.Date < booking.StartDate.Date)
            {
                ModelState.AddModelError("EndDate",
                    "End date must be on or after the start date.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropdowns(booking.CustomerId, booking.EquipmentId);
                return View(booking);
            }

            int alreadyBooked = GetBookedQuantity(
                booking.EquipmentId,
                booking.StartDate,
                booking.EndDate,
                null);

            if (alreadyBooked + booking.Quantity > equipment.Stock)
            {
                ModelState.AddModelError("Quantity",
                    "Not enough equipment is available for these dates.");

                LoadDropdowns(booking.CustomerId, booking.EquipmentId);
                return View(booking);
            }

            int rentalDays =
                (booking.EndDate.Date - booking.StartDate.Date).Days + 1;

            booking.TotalAmount =
                equipment.PricePerDay * rentalDays * booking.Quantity;

            booking.BookingStatus = Pending;
            booking.BookingDate = DateTime.Now;

            db.RentalBookings.Add(booking);
            db.SaveChanges();

            TempData["Success"] = "Rental booking created successfully.";

            return RedirectToAction("Index");
        }

        // GET: RentalBooking/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var booking = db.RentalBookings
                .Include(b => b.Customer)
                .Include(b => b.Equipment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return HttpNotFound();

            return View(booking);
        }

        // GET: RentalBooking/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var booking = db.RentalBookings.Find(id);

            if (booking == null)
                return HttpNotFound();

            if (booking.BookingStatus == Cancelled ||
                booking.BookingStatus == Completed)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "Cancelled or completed bookings cannot be edited.");
            }

            LoadDropdowns(booking.CustomerId, booking.EquipmentId);

            return View(booking);
        }

        // POST: RentalBooking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RentalBooking booking)
        {
            var existing = db.RentalBookings.Find(booking.BookingId);

            if (existing == null)
                return HttpNotFound();

            if (existing.BookingStatus == Cancelled ||
                existing.BookingStatus == Completed)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "This booking cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropdowns(booking.CustomerId, booking.EquipmentId);
                return View(booking);
            }

            var customer = db.Customers
                .FirstOrDefault(c => c.CustomerId == booking.CustomerId
                                  && c.IsActive);

            var equipment = db.Equipment
                .FirstOrDefault(e => e.EquipmentId == booking.EquipmentId
                                  && e.IsActive);

            if (customer == null)
                ModelState.AddModelError("CustomerId", "Select an active customer.");

            if (equipment == null)
                ModelState.AddModelError("EquipmentId", "Select active equipment.");

            if (booking.StartDate.Date < DateTime.Today)
                ModelState.AddModelError("StartDate",
                    "Start date cannot be in the past.");

            if (booking.EndDate.Date < booking.StartDate.Date)
                ModelState.AddModelError("EndDate",
                    "End date must be on or after the start date.");

            if (!ModelState.IsValid)
            {
                LoadDropdowns(booking.CustomerId, booking.EquipmentId);
                return View(booking);
            }

            int alreadyBooked = GetBookedQuantity(
                booking.EquipmentId,
                booking.StartDate,
                booking.EndDate,
                booking.BookingId);

            if (alreadyBooked + booking.Quantity > equipment.Stock)
            {
                ModelState.AddModelError("Quantity",
                    "Not enough equipment is available for these dates.");

                LoadDropdowns(booking.CustomerId, booking.EquipmentId);
                return View(booking);
            }

            int rentalDays =
                (booking.EndDate.Date - booking.StartDate.Date).Days + 1;

            existing.CustomerId = booking.CustomerId;
            existing.EquipmentId = booking.EquipmentId;
            existing.StartDate = booking.StartDate;
            existing.EndDate = booking.EndDate;
            existing.Quantity = booking.Quantity;
            existing.TotalAmount =
                equipment.PricePerDay * rentalDays * booking.Quantity;

            db.SaveChanges();

            TempData["Success"] = "Booking updated successfully.";

            return RedirectToAction("Index");
        }

        // POST: RentalBooking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var booking = db.RentalBookings.Find(id);

            if (booking == null)
                return HttpNotFound();

            if (booking.BookingStatus == Completed)
            {
                TempData["Error"] =
                    "Completed bookings cannot be cancelled.";
                return RedirectToAction("Index");
            }

            if (booking.BookingStatus == Cancelled)
            {
                TempData["Error"] = "This booking is already cancelled.";
                return RedirectToAction("Index");
            }

            booking.BookingStatus = Cancelled;

            db.SaveChanges();

            TempData["Success"] = "Booking cancelled successfully.";

            return RedirectToAction("Index");
        }

        // Calculate overlapping booked quantity
        private int GetBookedQuantity(
            int equipmentId,
            DateTime startDate,
            DateTime endDate,
            int? excludeBookingId)
        {
            var query = db.RentalBookings.Where(b =>
                b.EquipmentId == equipmentId &&
                (b.BookingStatus == Pending ||
                 b.BookingStatus == Confirmed) &&
                DbFunctions.TruncateTime(b.StartDate) <= endDate.Date &&
                DbFunctions.TruncateTime(b.EndDate) >= startDate.Date);

            if (excludeBookingId.HasValue)
            {
                query = query.Where(
                    b => b.BookingId != excludeBookingId.Value);
            }

            return query.Select(b => (int?)b.Quantity).Sum() ?? 0;
        }

        private void LoadDropdowns(
    int? selectedCustomer = null,
    int? selectedEquipment = null)
        {
            ViewBag.CustomerId = new SelectList(
                db.Customers.Where(c => c.IsActive).ToList(),
                "CustomerId",
                "CustomerName",
                selectedCustomer);

            ViewBag.EquipmentId = new SelectList(
                db.Equipment
                    .Where(e => e.IsActive)
                    .ToList()
                    .Select(e => new
                    {
                        e.EquipmentId,
                        DisplayName = e.EquipmentName +
                            " - ₹" + e.PricePerDay + "/day"
                    }),
                "EquipmentId",
                "DisplayName",
                selectedEquipment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
