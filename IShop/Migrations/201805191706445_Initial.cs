namespace IShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bills",
                c => new
                    {
                        OrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        DailyMenuID = c.Int(),
                        Count = c.Int(nullable: false),
                        Address = c.String(nullable: false),
                        FinalyPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        DateString = c.String(),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.DailyMenus", t => t.DailyMenuID)
                .Index(t => t.UserId)
                .Index(t => t.DailyMenuID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserResponces",
                c => new
                    {
                        UserResponceID = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Responce = c.String(),
                        Estimation = c.Int(nullable: false),
                        DailyMenuID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateString = c.String(),
                    })
                .PrimaryKey(t => t.UserResponceID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.DailyMenus", t => t.DailyMenuID, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.DailyMenuID);
            
            CreateTable(
                "dbo.DailyMenus",
                c => new
                    {
                        DailyMenuID = c.Int(nullable: false, identity: true),
                        DailyMenuName = c.String(nullable: false, maxLength: 50),
                        Calorie = c.Double(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DailyMenuID);
            
            CreateTable(
                "dbo.DailyMenuResponces",
                c => new
                    {
                        DailyMenuResponceID = c.Int(nullable: false, identity: true),
                        DailyMenuID = c.Int(),
                        Responce = c.String(),
                        Reference = c.String(maxLength: 60),
                        Date = c.DateTime(nullable: false),
                        DateString = c.String(),
                    })
                .PrimaryKey(t => t.DailyMenuResponceID)
                .ForeignKey("dbo.DailyMenus", t => t.DailyMenuID)
                .Index(t => t.DailyMenuID);
            
            CreateTable(
                "dbo.Dishes",
                c => new
                    {
                        DishID = c.Int(nullable: false, identity: true),
                        DishName = c.String(nullable: false, maxLength: 25),
                        CategoryID = c.Int(),
                        Calorie = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.DishID)
                .ForeignKey("dbo.DishCategories", t => t.CategoryID)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.DishCategories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        IngrediantID = c.Int(nullable: false, identity: true),
                        IngredientName = c.String(nullable: false, maxLength: 20),
                        Calorie = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IngrediantID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.DishDailyMenus",
                c => new
                    {
                        Dish_DishID = c.Int(nullable: false),
                        DailyMenu_DailyMenuID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Dish_DishID, t.DailyMenu_DailyMenuID })
                .ForeignKey("dbo.Dishes", t => t.Dish_DishID, cascadeDelete: true)
                .ForeignKey("dbo.DailyMenus", t => t.DailyMenu_DailyMenuID, cascadeDelete: true)
                .Index(t => t.Dish_DishID)
                .Index(t => t.DailyMenu_DailyMenuID);
            
            CreateTable(
                "dbo.IngredientDishes",
                c => new
                    {
                        Ingredient_IngrediantID = c.Int(nullable: false),
                        Dish_DishID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Ingredient_IngrediantID, t.Dish_DishID })
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_IngrediantID, cascadeDelete: true)
                .ForeignKey("dbo.Dishes", t => t.Dish_DishID, cascadeDelete: true)
                .Index(t => t.Ingredient_IngrediantID)
                .Index(t => t.Dish_DishID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Bills", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.UserResponces", "DailyMenuID", "dbo.DailyMenus");
            DropForeignKey("dbo.Orders", "DailyMenuID", "dbo.DailyMenus");
            DropForeignKey("dbo.IngredientDishes", "Dish_DishID", "dbo.Dishes");
            DropForeignKey("dbo.IngredientDishes", "Ingredient_IngrediantID", "dbo.Ingredients");
            DropForeignKey("dbo.DishDailyMenus", "DailyMenu_DailyMenuID", "dbo.DailyMenus");
            DropForeignKey("dbo.DishDailyMenus", "Dish_DishID", "dbo.Dishes");
            DropForeignKey("dbo.Dishes", "CategoryID", "dbo.DishCategories");
            DropForeignKey("dbo.DailyMenuResponces", "DailyMenuID", "dbo.DailyMenus");
            DropForeignKey("dbo.UserResponces", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.IngredientDishes", new[] { "Dish_DishID" });
            DropIndex("dbo.IngredientDishes", new[] { "Ingredient_IngrediantID" });
            DropIndex("dbo.DishDailyMenus", new[] { "DailyMenu_DailyMenuID" });
            DropIndex("dbo.DishDailyMenus", new[] { "Dish_DishID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Dishes", new[] { "CategoryID" });
            DropIndex("dbo.DailyMenuResponces", new[] { "DailyMenuID" });
            DropIndex("dbo.UserResponces", new[] { "DailyMenuID" });
            DropIndex("dbo.UserResponces", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Orders", new[] { "DailyMenuID" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Bills", new[] { "OrderID" });
            DropTable("dbo.IngredientDishes");
            DropTable("dbo.DishDailyMenus");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Ingredients");
            DropTable("dbo.DishCategories");
            DropTable("dbo.Dishes");
            DropTable("dbo.DailyMenuResponces");
            DropTable("dbo.DailyMenus");
            DropTable("dbo.UserResponces");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Orders");
            DropTable("dbo.Bills");
        }
    }
}
