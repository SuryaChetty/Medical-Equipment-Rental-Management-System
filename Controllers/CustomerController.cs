
using Medical_Equipment_Rental_Management_System.Models;


using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MediRent.Controllers
{
    public class CustomerController : Controller
    {
        private MediRentContext db = new MediRentContext();

        // GET: Customer
        public ActionResult Index()
        {
            var customers = db.Customers
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CustomerId)
                .ToList();

            return View(customers);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedDate = DateTime.Now;
                customer.IsActive = true;

                db.Customers.Add(customer);
                db.SaveChanges();

                TempData["Success"] = "Customer added successfully.";

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Customer customer = db.Customers.Find(id);

            if (customer == null || !customer.IsActive)
                return HttpNotFound();

            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = db.Customers.Find(customer.CustomerId);

                if (existingCustomer == null || !existingCustomer.IsActive)
                    return HttpNotFound();

                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;

                db.SaveChanges();

                TempData["Success"] = "Customer updated successfully.";

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Customer customer = db.Customers.Find(id);

            if (customer == null || !customer.IsActive)
                return HttpNotFound();

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);

            if (customer == null || !customer.IsActive)
                return HttpNotFound();

            customer.IsActive = false;

            db.SaveChanges();

            TempData["Success"] = "Customer deactivated successfully.";

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
