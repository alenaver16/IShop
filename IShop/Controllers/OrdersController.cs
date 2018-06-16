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
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using IShop.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace IShop.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.ApplicationUser).Include(o => o.Bill).Include(o => o.DailyMenu);
            return View(orders.ToList());
        }

        [Authorize(Roles = "manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.FirstOrDefault(d => d.OrderID == id);
            ApplicationUser user = db.Users.Where(d => d.Id == order.UserId).FirstOrDefault();
            DailyMenu dailyMenu = db.DailyMenus.Where(d => d.DailyMenuID == order.DailyMenuID).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }
            dynamic model = new ExpandoObject();
            model.Order = order;
            model.User = user.FullName;
            model.Menu = dailyMenu.DailyMenuName;
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName");
            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName");
            Order order = new Order { Date = DateTime.Now, DateString = DateTime.Now.ToShortDateString() };
            return View(order);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", order.UserId);
            ViewBag.OrderID = new SelectList(db.Bills, "OrderID", "OrderID", order.OrderID);
            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName", order.DailyMenuID);
            return View(order);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", order.UserId);
            ViewBag.OrderID = new SelectList(db.Bills, "OrderID", "OrderID", order.OrderID);
            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName", order.DailyMenuID);
            return View(order);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", order.UserId);
            ViewBag.OrderID = new SelectList(db.Bills, "OrderID", "OrderID", order.OrderID);
            ViewBag.DailyMenuID = new SelectList(db.DailyMenus, "DailyMenuID", "DailyMenuName", order.DailyMenuID);
            return View(order);
        }

        [Authorize(Roles = "manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Order order = db.Orders.Find(id);
            Order order = db.Orders.Include(o => o.ApplicationUser).Include(o => o.Bill).Include(o => o.DailyMenu).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [Authorize(Roles = "manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
        public ActionResult ReportChartPie()
        {
            var orders = db.Orders.Include(o => o.ApplicationUser).Include(o => o.DailyMenu)
                .OrderBy(o => o.DailyMenuID).GroupBy(o => o.DailyMenuID)
                .Select(o => new RenderPie { MenuId = o.Key.Value, Orders = o.ToList().Count().ToString() }).ToList();

            List<string> listMenuName = new List<string>();
            List<string> listCount = new List<string>();

            foreach (var item in orders)
            {
                int id = item.MenuId;
                DailyMenu dm = db.DailyMenus.FirstOrDefault(d => d.DailyMenuID == id);
                listMenuName.Add(dm.DailyMenuName);
                listCount.Add(item.Orders);
            }

            var myChart = new Chart(width: 600, height: 400, theme: ChartTheme.Blue)
            .AddTitle("Отчет о заказах меню")
            .AddSeries(
            name: "Отчет",
            chartType: "Pie",
            xValue: listMenuName,
            yValues: listCount)
            .Write();

            return null;
        }


        [Authorize(Roles = "manager")]
        public ActionResult ReportChart()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<RenderView> orders = db.Orders.GroupBy(o => o.DateString).Select(o => new RenderView { DateId = o.Key, Orders = (o.ToList()).Count() }).ToList();
            List<string> listDate = new List<string>();
            List<string> listCount = new List<string>();
            foreach (var item in orders)
            {
                listDate.Add(item.DateId);
                listCount.Add(item.Orders.ToString());
            }

            var myChart = new Chart(width: 600, height: 400, theme: ChartTheme.Blue)
            .AddTitle("Отчет о заказах")
            .AddSeries(
            name: "Отчет",
            xValue: listDate,
            yValues: listCount)
            .Write();

            return null;
        }

        [Authorize(Roles = "manager")]
        public ActionResult Report()
        {
            ViewBag.Order = db.Orders.Include(o => o.ApplicationUser).Include(o => o.DailyMenu).ToList();
            var data = (from ty in db.Orders.Include(o => o.ApplicationUser).Include(o => o.DailyMenu)
                        orderby ty.DateString
                        select ty);
            return View(data);
        }

        [Authorize(Roles = "manager")]
        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("OrdersPDF_" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(6);
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

            float[] headers = { 24, 24, 8, 12, 45, 24 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            List<Order> orders = db.Orders.Include(o => o.DailyMenu).Include(o => o.ApplicationUser).ToList<Order>();
            BaseFont baseFont = BaseFont.CreateFont(HostingEnvironment.MapPath("/fonts/arial.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Заказы", new Font(baseFont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "Пользователь");
            AddCellToHeader(tableLayout, "Меню");
            AddCellToHeader(tableLayout, "Количество");
            AddCellToHeader(tableLayout, "Конечная стоимость");
            AddCellToHeader(tableLayout, "Адрес доставки");
            AddCellToHeader(tableLayout, "Дата");

            ////Add body  

            foreach (var order in orders)
            {

                AddCellToBody(tableLayout, order.ApplicationUser.FirstName);
                AddCellToBody(tableLayout, order.DailyMenu.DailyMenuName);
                AddCellToBody(tableLayout, order.Count.ToString());
                AddCellToBody(tableLayout, order.FinalyPrice.ToString());
                AddCellToBody(tableLayout, order.Address);
                AddCellToBody(tableLayout, order.DateString);
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
