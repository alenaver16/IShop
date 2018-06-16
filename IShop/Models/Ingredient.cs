using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class Ingredient
    {
        [Key]
        public int IngrediantID { get; set; }

        [MaxLength(20), MinLength(3)]
        [Required]
        [Display(Name = "Название ингридиента")]
        public string IngredientName { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Display(Name = "Калории")]
        public double Calorie { get; set; }

        public ICollection<Dish> Dishes { get; set; }

    }
}