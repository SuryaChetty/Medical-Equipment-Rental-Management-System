
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Medical_Equipment_Rental_Management_System.Models;

namespace Medical_Equipment_Rental_Management_System.Controllers
{
    public class InvoiceController : Controller
    {
        private MediRentContext db = new MediRentContext();

        // GET: Invoice
        public ActionResult Index()
        {
            var invoices = db.Invoices
                .Include(i => i.RentalBooking)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            return View(invoices);
        }

        // GET: Invoice/Details/5
        public ActionResult Details(int id)
        {
            var invoice = db.Invoices
                .Include(i => i.RentalBooking)
                .FirstOrDefault(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            return View(invoice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
