using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int? DailyMenuID { get; set; }
        [ForeignKey("DailyMenuID")]
        public DailyMenu DailyMenu { get; set; }

        [Range(1, 10)]
        [Display(Name = "Количество")]
        public int Count { get; set; }

        [Required]
        [Display(Name = "Адрес доставки")]
        public string Address { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Display(Name = "Конечная стоимость")]
        public decimal FinalyPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        [Display(Name = "Дата заказа")]
        public DateTime Date { get; set; }
        [Display(Name = "Дата заказа")]
        public string DateString { get; set; }

        public Bill Bill { get; set; }
    }
}