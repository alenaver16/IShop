namespace IShop.Migrations
{
    using IShop.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IShop.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IShop.Models.ApplicationDbContext context)
        {
            Ingredient Ingredient1 = new Ingredient { IngrediantID = 1, IngredientName = "������", Calorie = 16 };
            Ingredient Ingredient2 = new Ingredient { IngrediantID = 2, IngredientName = "�������", Calorie = 28 };
            Ingredient Ingredient3 = new Ingredient { IngrediantID = 3, IngredientName = "����", Calorie = 324 };
            Ingredient Ingredient4 = new Ingredient { IngrediantID = 4, IngredientName = "������", Calorie = 124 };
            Ingredient Ingredient5 = new Ingredient { IngrediantID = 5, IngredientName = "��������� �����", Calorie = 400 };
            context.Ingredients.AddOrUpdate(
             Ingredient1, Ingredient2, Ingredient3, Ingredient4, Ingredient5
             );

            DishCategory DishCategory1 = new DishCategory { CategoryID = 1, CategoryName = "�����" };
            DishCategory DishCategory2 = new DishCategory { CategoryID = 2, CategoryName = "���" };
            context.DishCategories.AddOrUpdate(
             DishCategory1, DishCategory2
             );


            Dish Dish1 = new Dish { DishID = 1, DishName = "����� ������", Calorie = 178, CategoryID = 1, Ingredients = new[] { Ingredient1, Ingredient2, Ingredient5 } };
            Dish Dish2 = new Dish { DishID = 2, DishName = "����� ���������", Calorie = 298, CategoryID = 1, Ingredients = new[] { Ingredient1, Ingredient2, Ingredient3, Ingredient4, Ingredient5 } };
            context.Dishes.AddOrUpdate(
            Dish1, Dish2
             );

            DailyMenu DailyMenu1 = new DailyMenu { DailyMenuID = 1, DailyMenuName = "���  ��������� �������", Calorie = 476, Price = 500, Dishes = new[] { Dish1, Dish2 } };

            context.DailyMenus.AddOrUpdate(
             DailyMenu1
             );

            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // ������� ��� ����
            var role1 = new IdentityRole { Name = "user" };
            var role2 = new IdentityRole { Name = "manager" };

            // ��������� ���� � ��
            roleManager.Create(role1);
            roleManager.Create(role2);

            // ������� �������������
            var user = new ApplicationUser { Id = "2", FirstName = "����", LastName = "���������", Email = "vlad@gmail.com", UserName = "vlad@gmail.com" };
            string password = "Qwerty1!";
            var result = userManager.Create(user, password);

            var manager = new ApplicationUser { Id = "1", FirstName = "�����", LastName = "��������", Email = "manager@gmail.com", UserName = "manager@gmail.com" };
            string password2 = "Qwerty1!";
            var result2 = userManager.Create(manager, password2);

            // ���� �������� ������������ ������ �������
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, role1.Name);
            }
            if (result2.Succeeded)
            {
                // ��������� ��� ������������ ����
                userManager.AddToRole(manager.Id, role2.Name);

            }

            context.UserResponces.AddOrUpdate(
            new UserResponce { UserResponceID = 1, DailyMenuID = 1, UserId = "2", Estimation = 5, Responce = "����� ������� � ��������������� ������, ���� ���������� ���!", Date = DateTime.Now, DateString = DateTime.Now.ToShortDateString() }
            );

            context.DailyMenuResponces.AddOrUpdate(
            new DailyMenuResponce { DailyMenuResponceID = 1, DailyMenuID = 1, Responce = "��������� �������� ��� ������.", Reference = "https://fitbox.kiev.ua/#sp-portfolio-wrapper", Date = DateTime.Now, DateString = DateTime.Now.ToShortDateString() }
            );

            context.Orders.AddOrUpdate(
            new Order { OrderID = 1, DailyMenuID = 1, UserId = "1", Count = 1, Address = "�.�������, ��.��������������, �.40, ��.428", Date = DateTime.Now, DateString = DateTime.Now.ToShortDateString(), FinalyPrice = 500 }
            );

            context.Bills.AddOrUpdate(
            new Bill { OrderID = 1 }
            );

        }
    }
}
