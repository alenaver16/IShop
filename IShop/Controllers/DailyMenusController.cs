using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using IShop.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;

namespace IShop.Controllers
{
    public class DailyMenusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            var dailyMenu = db.DailyMenus.Include(t => t.Orders).Include(t => t.Dishes).ToList();
            for (int i = 0; i < dailyMenu.Count; i++)
            {
                List<Dish> dishes = dailyMenu[i].Dishes.ToList();
            }

            return View(dailyMenu);
        }

        [Authorize(Roles = "user")]
        public ActionResult IndexUser()
        {
            var dailyMenu = db.DailyMenus.Include(t => t.Orders).Include(t => t.Dishes).ToList();
            for (int i = 0; i < dailyMenu.Count; i++)
            {
                List<Dish> dishes = dailyMenu[i].Dishes.ToList();
            }

            return View(dailyMenu);
        }

        [UserOrManager]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyMenu dailyMenu = db.DailyMenus.Include(d => d.Dishes).FirstOrDefault(d => d.DailyMenuID == id);
            List<Dish> dishes = dailyMenu.Dishes.ToList();
            List<UserResponce> responses = db.UserResponces.Include(u => u.ApplicationUser).Where(t => t.DailyMenuID == id).ToList();
            var userId = User.Identity.GetUserId();

            if (dailyMenu == null)
            {
                return HttpNotFound();
            }

            dynamic model = new ExpandoObject();
            model.DailyMenu = dailyMenu;
            model.Dishes = dishes;
            model.Responses = responses;
            model.Id = userId;

            return View(model);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Create()
        {
            ViewBag.Dishes = db.Dishes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public ActionResult Create(DailyMenu dailyMenu, int[] selectedDishes)
        {
            if (ModelState.IsValid)
            {
                if (selectedDishes != null)
                {
                    dailyMenu.Dishes = new List<Dish>();
                    foreach (Dish c in db.Dishes.Where(co => selectedDishes.Contains(co.DishID)))
                    {
                        dailyMenu.Dishes.Add(c);
                    }
                }

                db.DailyMenus.Add(dailyMenu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dailyMenu);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Edit(int? id)
        {
            DailyMenu dailyMenu = db.DailyMenus.Include(d => d.Dishes).FirstOrDefault(d => d.DailyMenuID == id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (dailyMenu == null)
            {
                return HttpNotFound();
            }
            ViewBag.Dishes = db.Dishes.ToList();
            dynamic model = new ExpandoObject();
            model.dailyMenu = dailyMenu;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public ActionResult Edit(DailyMenu dailyMenu, int[] selectedDishes)
        {
            if (ModelState.IsValid)
            {
                DailyMenu newDaiyMenu = db.DailyMenus.Include(d => d.Dishes).FirstOrDefault(d => d.DailyMenuID == dailyMenu.DailyMenuID);
                newDaiyMenu.DailyMenuName = dailyMenu.DailyMenuName;
                newDaiyMenu.Price = dailyMenu.Price;
                newDaiyMenu.Calorie = dailyMenu.Calorie;
                newDaiyMenu.Dishes.Clear();
                if (selectedDishes != null)
                {
                    foreach (var c in db.Dishes.Where(co => selectedDishes.Contains(co.DishID)))
                    {
                        newDaiyMenu.Dishes.Add(c);
                    }
                }

                db.Entry(newDaiyMenu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dailyMenu);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyMenu dailyMenu = db.DailyMenus.Find(id);
            if (dailyMenu == null)
            {
                return HttpNotFound();
            }
            return View(dailyMenu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            DailyMenu dailyMenu = db.DailyMenus.Find(id);
            db.DailyMenus.Remove(dailyMenu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult Order(int id)
        {
            string userId = User.Identity.GetUserId();
            DailyMenu dailyMenu = db.DailyMenus.FirstOrDefault(d => d.DailyMenuID == id);
            Order order = new Order { DailyMenuID = id, UserId = userId, Date = DateTime.Now, DateString = DateTime.Now.ToShortDateString(), FinalyPrice = dailyMenu.Price, Count = 1 };
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user")]
        public ActionResult Order(Order order)
        {
            order.FinalyPrice = order.FinalyPrice * order.Count;
            db.Orders.Add(order);
            db.SaveChanges(); 
            return RedirectToAction("Orders", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "manager")]
        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("DailyMenusPDF_" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(4);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }
        [Authorize(Roles = "manager")]
        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 24, 35, 12, 12 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            List<DailyMenu> dailyMenus = db.DailyMenus.Include(d => d.Dishes ).ToList<DailyMenu>();
            for (int i = 0; i < dailyMenus.Count; i++)
            {
                List<Dish> dishes = dailyMenus[i].Dishes.ToList();
            }
            BaseFont baseFont = BaseFont.CreateFont(HostingEnvironment.MapPath("/fonts/arial.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Меню", new Font(baseFont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "Меню");
            AddCellToHeader(tableLayout, "Блюда");
            AddCellToHeader(tableLayout, "Калории");
            AddCellToHeader(tableLayout, "Стоимость");

            ////Add body  

            foreach (var menu in dailyMenus)
            {
                string d = "";
                AddCellToBody(tableLayout, menu.DailyMenuName);
                foreach (var dish in menu.Dishes)
                {
                    d += dish.DishName.ToString() + "\r\n ";
                }
                AddCellToBody(tableLayout, d);
                AddCellToBody(tableLayout, menu.Calorie.ToString());
                AddCellToBody(tableLayout, menu.Price.ToString());
            }

            return tableLayout;
        }

        // Method to add single cell to the Header  
        [Authorize(Roles = "manager")]
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            BaseFont baseFont = BaseFont.CreateFont(HostingEnvironment.MapPath("/fonts/arial.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(baseFont, 8, 1, iTextSharp.text.BaseColor.YELLOW)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
            });
        }

        // Method to add single cell to the body  
        [Authorize(Roles = "manager")]
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            BaseFont baseFont = BaseFont.CreateFont(HostingEnvironment.MapPath("/fonts/arial.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(baseFont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }
    }
}
