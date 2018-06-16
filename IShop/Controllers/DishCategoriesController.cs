using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IShop.Models;

namespace IShop.Controllers
{
    public class DishCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            return View(db.DishCategories.ToList());
        }

        [Authorize(Roles = "manager")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] DishCategory dishCategory)
        {
            if (ModelState.IsValid)
            {
                db.DishCategories.Add(dishCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dishCategory);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishCategory dishCategory = db.DishCategories.Find(id);
            if (dishCategory == null)
            {
                return HttpNotFound();
            }
            return View(dishCategory);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] DishCategory dishCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dishCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dishCategory);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishCategory dishCategory = db.DishCategories.Find(id);
            if (dishCategory == null)
            {
                return HttpNotFound();
            }
            return View(dishCategory);
        }

        [Authorize(Roles = "manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DishCategory dishCategory = db.DishCategories.Find(id);
            db.DishCategories.Remove(dishCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
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
