using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IShop.Models
{
    public class UserOrderHelper
    {
        private DailyMenu DailyMenu;
        private DateTime DateTime;

        public UserOrderHelper(DailyMenu dailyMenu, DateTime dateTime)
        {
            this.DailyMenu = dailyMenu;
            this.DateTime = dateTime;
        }

        public void setDailyMenu(DailyMenu dailyMenu)
        {
            this.DailyMenu = dailyMenu;
        }

        public DailyMenu getDailyMenu()
        {
            return DailyMenu;
        }

        public void setDateTime(DateTime DateTime)
        {
            this.DateTime = DateTime;
        }

        public DateTime getDateTime()
        {
            return DateTime;
        }
    }
}