using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class RenderView
    {
        public string DateId { get; set; }
        //public IEnumerable<Order> Orders { get; set; }
        public int Orders { get; set; }
    }

    public class RenderPie
    {
        public int MenuId { get; set; }
        //public IEnumerable<Order> Orders { get; set; }
        public string Orders { get; set; }
    }
}