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
    public class DailyMenuResponcesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            var dailyMenuResponces = db.DailyMenuResponces.Include(d => d.DailyMenu);
            return View(dailyMenuResponces.ToList());
        }
        [Authorize(Roles = "user")]
        public ActionResult IndexUser()
        {
            var dailyMenuResponces = db.DailyMenuResponces.Include(d => d.DailyMenu);
            return View(dailyMenuResponces.ToList());
        }

        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DailyMenuResponce dailyMenuResponce = db.DailyMenuResponces.Find(id);
        //    if (dailyMenuResponce == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(dailyMenuResponce);
        //}

        [Authorize(Roles = "manager")]
        public ActionResult Create()
        {
            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName");
            DailyMenuResponce res = new DailyMenuResponce { Date = DateTime.Now };
            return View(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public ActionResult Create([Bind(Include = "DailyMenuResponceID,DailyMenuID,Responce,Reference,Date")] DailyMenuResponce dailyMenuResponce)
        {
            if (ModelState.IsValid)
            {
                db.DailyMenuResponces.Add(dailyMenuResponce);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName", dailyMenuResponce.DailyMenuID);
            return View(dailyMenuResponce);
        }


        [Authorize(Roles = "manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyMenuResponce dailyMenuResponce = db.DailyMenuResponces.Find(id);
            if (dailyMenuResponce == null)
            {
                return HttpNotFound();
            }
            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName", dailyMenuResponce.DailyMenuID);
            return View(dailyMenuResponce);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public ActionResult Edit([Bind(Include = "DailyMenuResponceID,DailyMenuID,Responce,Reference,Date")] DailyMenuResponce dailyMenuResponce)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dailyMenuResponce).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName", dailyMenuResponce.DailyMenuID);
            return View(dailyMenuResponce);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyMenuResponce dailyMenuResponce = db.DailyMenuResponces.Find(id);
            if (dailyMenuResponce == null)
            {
                return HttpNotFound();
            }
            return View(dailyMenuResponce);
        }

        [Authorize(Roles = "manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DailyMenuResponce dailyMenuResponce = db.DailyMenuResponces.Find(id);
            db.DailyMenuResponces.Remove(dailyMenuResponce);
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
