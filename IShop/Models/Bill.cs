using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class Bill
    {
        [Key]
        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public Order Order { get; set; }
    }
}