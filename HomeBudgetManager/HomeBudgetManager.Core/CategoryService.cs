using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core
{
    public class CategoryService
    {
        private readonly AppDbContext db;

        public CategoryService(AppDbContext db) {
            this.db = db;
        }

        private bool userHasCategory(int userId, string name) {
            return db.Categories.Any(c => (c.UserId == userId || c.UserId == null) && c.Name.ToLower() == name.ToLower());
        }

        // metoda dodająca nową transakcję dla użytkownika. Zwraca informację o powodzeniu
        public string addCategory(int userId, string name, string? description)
        {
            if (userHasCategory(userId, name))
            {
                return "Posiadasz już kategorię o tej samej nazwie";
            }

            try
            {
                var newCategory = new DBCategory { Name = name, Description = description, UserId = userId };
                db.Categories.Add(newCategory);
                db.SaveChanges();
                return "Poprawnie dodano kategorię";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string deleteCategory(int userId, int categoryId)
        {

            var category = db.Categories.FirstOrDefault(c => c.UserId == userId);

            if (category == null) {

                return "Błąd: nie można usunąć kategorii. Nie znaleziono w bazie";
            }

            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                return "Pomyślnie usunięto kategorię";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public List<DBCategory> listAllUserCategories(int userId)
        {
            return db.Categories.Where(c => c.UserId == userId || c.UserId == null).OrderByDescending(c => c.UserId).ToList();
        }

        public string modifyCategory(int userId, int categoryId, string newName, string newDescription)
        {
            var category = db.Categories.FirstOrDefault(c => c.UserId == userId && c.Id == categoryId);

            if (category == null)
            {
                return "Błąd: użytkownik nie posiada takiej kategorii";
            }

            category.Name = newName;
            category.Description = newDescription;
            db.SaveChanges();

            return "Pomyślnie zedytowano kategorię";
        }

        public string addDefaultCategories()
        {
            var zakupy = new DBCategory { UserId = null, Name = "Zakupy spożywcze", Description = "Opłaty za codzienne zakupy domowe" };
            var rachunki = new DBCategory { UserId = null, Name = "Rachunki", Description = "Opłaty za wodę, gaz, prąd itp." };
            var transport = new DBCategory { UserId = null, Name = "Transport", Description = "Opłaty za komunikację miejską lub paliwo" };
            var finanse = new DBCategory { UserId = null, Name = "Finanse", Description = "Kategoria dla finansów" };
            var rozrywka = new DBCategory {UserId = null, Name = "Rozrywka", Description = "Kategoria dla rozrywki" };
            var inne = new DBCategory { UserId = null, Name = "Inne" , Description = "Kategoria dla innych wydatków"};

            List<DBCategory> categories = new List<DBCategory> { zakupy, rachunki, transport, finanse, rozrywka, inne };

            try
            {

                foreach (DBCategory category in categories)
                {
                    if (!db.Categories.Any(c => c.UserId == null && c.Name == category.Name))
                    {
                        db.Categories.Add(category);
                        Console.WriteLine("Dodano domyślną kategorię: ", category.Name);
                    } else
                    {
                        Console.WriteLine("Domyślna kategoria ", category.Name);
                    }
                }

                db.SaveChanges();
                return "Pomyślnie dodano domyślne transakcje";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }
    }
}
