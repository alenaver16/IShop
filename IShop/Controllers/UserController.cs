using IShop.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace IShop.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            var users = db.Users.Where(u => u.Email != "manager@gmail.com").ToList();
            return View(users);
        }

        [Authorize(Roles = "manager")]
        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("UsersPDF_" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(3);
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

            float[] headers = { 45, 45, 35 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            List<ApplicationUser> users = db.Users.ToList<ApplicationUser>();
            BaseFont baseFont = BaseFont.CreateFont(HostingEnvironment.MapPath("/fonts/arial.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            tableLayout.AddCell(new PdfPCell(new Phrase("Пользователи", new Font(baseFont, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) {
                Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "FirstName");
            AddCellToHeader(tableLayout, "LastName");
            AddCellToHeader(tableLayout, "Email");

            ////Add body  

            foreach (var user in users)
            {
                AddCellToBody(tableLayout, user.FirstName);
                AddCellToBody(tableLayout, user.LastName);
                AddCellToBody(tableLayout, user.Email);
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
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
            });
        }

        // Method to add single cell to the body  
        [Authorize(Roles = "manager")]
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            BaseFont baseFont = BaseFont.CreateFont(HostingEnvironment.MapPath("/fonts/arial.ttf"), BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(baseFont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
             {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }
    }
}