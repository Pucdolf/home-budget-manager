using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core
{
    internal class CategoryManager
    {
        private static List<Category> defaultCategories;

        CategoryManager()
        {
            initializeDefaultCategories();
        }

        // Clears categories container and then sets default categories to that container
        public static void initializeDefaultCategories()
            {
                defaultCategories.Clear();    

                Category groceriesCat = new(1, "Groceries", "groceries description");
                Category giftCat = new(2, "Gift", "gift description");
                Category billCat = new(3, "Bills", "bills description");
            
                defaultCategories.Add(groceriesCat);
                defaultCategories.Add(giftCat);
                defaultCategories.Add(billCat);
        }

        public static List<Category> getDefaultCategories()
        {
            return defaultCategories;
        }
        public static Category createCategory(String name, String description)
        {
            return new Category(name, description);    
        }
        public static bool validateCategory(Category category)
        {
            return false;
        }

    }
}
