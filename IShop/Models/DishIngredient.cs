using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class DishIngredient
    {
        public int? DishID { get; set; }
        [ForeignKey("DishID")]
        public Dish Dish{ get; set; }

        public int? IngredientID { get; set; }
        [ForeignKey("IngredientID")]
        public Ingredient Ingredient { get; set; }
    }
}