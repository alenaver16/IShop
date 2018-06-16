using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class DailyMenuResponce
    {
        [Key]
        public int DailyMenuResponceID { get; set; }

        public int? DailyMenuID { get; set; }
        [ForeignKey("DailyMenuID")]
        public DailyMenu DailyMenu { get; set; }

        [MinLength(5)]
        [Display(Name = "Отзыв")]
        public string Responce { get; set; }

        [MaxLength(60), MinLength(5)]
        [Display(Name = "Ссылка на источник")]
        [Url]
        public string Reference { get; set; }

        //[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        [Display(Name = "Дата публикации")]
        public DateTime Date { get; set; }
        public string DateString { get; set; }
    }
}