using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class Dish
    {
        [Key]
        public int DishID { get; set; }

        [MaxLength(25), MinLength(5)]
        [Required]
        [Display(Name = "Название блюда")]
        public string DishName { get; set; }

        public int? CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public DishCategory Category { get; set; }

        [Display(Name = "Калории")]
        public double Calorie { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; }
        public ICollection<DailyMenu> DailyMenus { get; set; }
    }
}