using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class DailyMenu
    {
        [Key]
        public int DailyMenuID { get; set; }

        [MaxLength(50), MinLength(5)]
        [Required]
        [Display(Name = "Название меню")]
        public string DailyMenuName { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Display(Name = "Калории")]
        public double Calorie { get; set; }

        [Required]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Display(Name = "Стоимость")]
        public decimal Price { get; set; }

        public ICollection<Dish> Dishes { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<DailyMenuResponce> DailyMenuResponces { get; set; }

        public ICollection<UserResponce> UserResponces { get; set; }
    }
}