using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class UserResponce
    {
        [Key]
        public int UserResponceID { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        //[MinLength(3)]
        [Display(Name = "Отзыв")]
        public string Responce { get; set; }

        [Range(1,5)]
        [Display(Name = "Оценка")]
        public int Estimation { get; set; }

        public int DailyMenuID { get; set; }
        [ForeignKey("DailyMenuID")]
        public DailyMenu DailyMenu { get; set; }

        //[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        [Display(Name = "Дата публикации")]
        public DateTime Date { get; set; }
        public string DateString { get; set; }

    }
}