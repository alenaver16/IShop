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

namespace IShop.Controllers
{
    public class DishesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            var dishes = db.Dishes.Include(d => d.Category).Include(d => d.Ingredients).ToList();
            for (int i = 0; i < dishes.Count; i++)
            {
                List<Ingredient> ingrediants = dishes[i].Ingredients.ToList();
            }
            return View(dishes);
        }

        [UserOrManager]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dish dish = db.Dishes.Include(d => d.Ingredients).Include(d => d.DailyMenus).FirstOrDefault(d => d.DishID == id);
            DishCategory dishCategory = db.DishCategories.Where(d => d.CategoryID == dish.CategoryID).FirstOrDefault();
            List<Ingredient> ingredients = dish.Ingredients.ToList();
            List<DailyMenu> menus = dish.DailyMenus.ToList();

            if (dish == null)
            {
                return HttpNotFound();
            }

            dynamic model = new ExpandoObject();
            model.Dish = dish;
            model.Category = dishCategory.CategoryName;
            model.Ingredients = ingredients;
            model.Menus = menus;
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Create()
        {
            ViewBag.Ingredients = db.Ingredients.ToList();
            ViewBag.CategoryID = new SelectList(db.DishCategories, "CategoryID", "CategoryName");
            return View();
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dish dish, int[] selectedIngredients)
        {
            if (ModelState.IsValid)
            {
                if (selectedIngredients != null)
                {
                    dish.Ingredients = new List<Ingredient>();
                    foreach (Ingredient c in db.Ingredients.Where(co => selectedIngredients.Contains(co.IngrediantID)))
                    {
                        dish.Ingredients.Add(c);
                        //dish.Calorie += c.Calorie;
                    }
                }
                db.Dishes.Add(dish);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.DishCategories, "CategoryID", "CategoryName", dish.CategoryID);
            return View(dish);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Edit(int? id)
        {
            Dish dish = db.Dishes.Include(d => d.Ingredients).FirstOrDefault(d => d.DishID == id);
            if (dish == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CategoryID = new SelectList(db.DishCategories, "CategoryID", "CategoryName", dish.CategoryID);
            ViewBag.Ingredients = db.Ingredients.ToList();
            dynamic model = new ExpandoObject();
            model.Dish = dish;
            return View(model);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Dish dish, int[] selectedIngredients)
        {
            if (ModelState.IsValid)
            {
                Dish newDish = db.Dishes.Include(d => d.Ingredients).FirstOrDefault(d => d.DishID == dish.DishID);
                newDish.DishName = dish.DishName;
                newDish.Calorie = dish.Calorie;
                newDish.Ingredients.Clear();
                if (selectedIngredients != null)
                {
                    foreach (var c in db.Ingredients.Where(co => selectedIngredients.Contains(co.IngrediantID)))
                    {
                        newDish.Ingredients.Add(c);
                    }
                }
                db.Entry(newDish).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.DishCategories, "CategoryID", "CategoryName", dish.CategoryID);
            return View(dish);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dish dish = db.Dishes.Find(id);
            if (dish == null)
            {
                return HttpNotFound();
            }
            return View(dish);
        }

        [Authorize(Roles = "manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dish dish = db.Dishes.Find(id);
            db.Dishes.Remove(dish);
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

        [Authorize(Roles = "manager")]
        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("DishesPDF_" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
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

            float[] headers = { 24, 24, 24, 24 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            List<Dish> dishes  = db.Dishes.Include(d => d.Ingredients).Include(d => d.Category).ToList<Dish>();
            for (int i = 0; i < dishes.Count; i++)
            {
                List<Ingredient> ingredients = dishes[i].Ingredients.ToList();
            }
            BaseFont baseFont = BaseFont.CreateFont(HostingEnvironment.MapPath("/fonts/arial.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Блюда", new Font(baseFont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "Блюдо");
            AddCellToHeader(tableLayout, "Катеория блюда");
            AddCellToHeader(tableLayout, "Ингридиенты");
            AddCellToHeader(tableLayout, "Калории");


            ////Add body  

            foreach (var dish in dishes)
            {
                string d = "";
                AddCellToBody(tableLayout, dish.DishName);
                AddCellToBody(tableLayout, dish.Category.CategoryName.ToString());
                foreach (var ingredient in dish.Ingredients)
                {
                    d += ingredient.IngredientName.ToString() + "\r\n ";
                }
                AddCellToBody(tableLayout, d);
                AddCellToBody(tableLayout, dish.Calorie.ToString());
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
