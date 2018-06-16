using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IShop.Models;
using Microsoft.AspNet.Identity;

namespace IShop.Controllers
{
    public class UserResponcesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            var userResponces = db.UserResponces.Include(u => u.ApplicationUser).Include(u => u.DailyMenu);
            return View(userResponces.ToList());
        }
        [Authorize(Roles = "user")]
        public ActionResult IndexUser()
        {
            var userResponces = db.UserResponces.Include(u => u.ApplicationUser).Include(u => u.DailyMenu);
            return View(userResponces.ToList());
        }

        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserResponce userResponce = db.UserResponces.Find(id);
            if (userResponce == null)
            {
                return HttpNotFound();
            }
            return View(userResponce);
        }

        [Authorize(Roles = "user")]
        public ActionResult Create(int id)
        {
            DailyMenu dailyMenu = db.DailyMenus.FirstOrDefault(h => h.DailyMenuID == id);
            UserResponce response = new UserResponce
            { 
                DailyMenuID = id,
                UserId = User.Identity.GetUserId(),
                Date = DateTime.Now,
                Estimation = 5,
                Responce = ""
            };

            return View(response);
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserResponceID,UserId,Responce,Estimation,DailyMenuID,Date")] UserResponce userResponce)
        {
            if (ModelState.IsValid)
            {
                db.UserResponces.Add(userResponce);
                db.SaveChanges();

                return RedirectToRoute(new
                {
                    controller = "DailyMenus",
                    action = "Details",
                    id = userResponce.DailyMenuID.ToString()
                });

            }

            return View(userResponce);
        }

        [Authorize(Roles = "user")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserResponce userResponce = db.UserResponces.Find(id);
            if (userResponce == null)
            {
                return HttpNotFound();
            }
            return View(userResponce);
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserResponceID,UserId,Responce,Estimation,DailyMenuID,Date")] UserResponce userResponce)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userResponce).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToRoute(new
                {
                    controller = "DailyMenus",
                    action = "Details",
                    id = userResponce.DailyMenuID.ToString()
                });
            }
            return View(userResponce);
        }

        [UserOrManager]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserResponce userResponce = db.UserResponces.Find(id);
            if (userResponce == null)
            {
                return HttpNotFound();
            }
            return View(userResponce);
        }

        [UserOrManager]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserResponce userResponce = db.UserResponces.Find(id);
            var pageId = userResponce.DailyMenuID.ToString();
            db.UserResponces.Remove(userResponce);
            db.SaveChanges();
            if (User.IsInRole("user"))
            {
                return RedirectToRoute(new
                {
                    controller = "DailyMenus",
                    action = "Details",
                    id = pageId
                });
            }
            else
            {
                return RedirectToAction("Index");
            }
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
