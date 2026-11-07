using Medical_Equipment_Rental_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Medical_Equipment_Rental_Management_System.Controllers
{
    public class EquipmentController : Controller
    {
        private MediRentContext db = new MediRentContext();

        // GET: Equipment
        public ActionResult Index()
        {
            var equipment = db.Equipment
                .Where(e => e.IsActive)
                .OrderByDescending(e => e.EquipmentId)
                .ToList();

            return View(equipment);
        }

        // GET: Equipment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                equipment.IsActive = true;

                db.Equipment.Add(equipment);
                db.SaveChanges();

                TempData["Success"] = "Equipment added successfully.";

                return RedirectToAction("Index");
            }

            return View(equipment);
        }

        // GET: Equipment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Equipment equipment = db.Equipment.Find(id);

            if (equipment == null || !equipment.IsActive)
                return HttpNotFound();

            return View(equipment);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Equipment.Find(equipment.EquipmentId);

                if (existing == null || !existing.IsActive)
                    return HttpNotFound();

                existing.EquipmentName = equipment.EquipmentName;
                existing.Category = equipment.Category;
                existing.Description = equipment.Description;
                existing.PricePerDay = equipment.PricePerDay;
                existing.Stock = equipment.Stock;
                existing.ImageUrl = equipment.ImageUrl;

                db.SaveChanges();

                TempData["Success"] = "Equipment updated successfully.";

                return RedirectToAction("Index");
            }

            return View(equipment);
        }

        // GET: Equipment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Equipment equipment = db.Equipment.Find(id);

            if (equipment == null || !equipment.IsActive)
                return HttpNotFound();

            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipment.Find(id);

            if (equipment == null || !equipment.IsActive)
                return HttpNotFound();

            equipment.IsActive = false;

            db.SaveChanges();

            TempData["Success"] = "Equipment deactivated successfully.";

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