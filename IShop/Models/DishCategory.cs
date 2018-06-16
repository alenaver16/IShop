using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class DishCategory
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(20), MinLength(3)]
        [Display(Name = "Категория блюда")]
        public string CategoryName { get; set; }

        public ICollection<Dish> Dishes { get; set; }
    }
}