using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;

namespace HomeBudgetManager.Tests
{
    public class CategoryServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public CategoryServiceTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Fact]
        public void AddCategory_ShouldPreventDuplicates_CaseInsensitive()
        {
            using var context = new AppDbContext(_contextOptions);
            
            // Add User
            var user = new DBUser { Login = "user1", Email = "u1@test.com", Password = "pwd" };
            context.Users.Add(user);
            context.SaveChanges();
            int userId = user.Id;

            var service = new CategoryService(context);

            // Add first category
            var result1 = service.addCategory(userId, "Prezenty", "Opis");
            Assert.Equal("Poprawnie dodano kategorię", result1);

            // Try add duplicate (same case)
            var result2 = service.addCategory(userId, "Prezenty", "Inny opis");
            Assert.Equal("Posiadasz już kategorię o tej samej nazwie", result2);

            // Try add duplicate (different case)
            var result3 = service.addCategory(userId, "PREZENTY", "Inny opis");
            Assert.Equal("Posiadasz już kategorię o tej samej nazwie", result3);
        }

        [Fact]
        public void AddCategory_ShouldAllowSameNameForDifferentUser()
        {
             using var context = new AppDbContext(_contextOptions);
            
            var user1 = new DBUser { Login = "user1", Email = "u1@test.com", Password = "pwd" };
            var user2 = new DBUser { Login = "user2", Email = "u2@test.com", Password = "pwd" };
            context.Users.AddRange(user1, user2);
            context.SaveChanges();
            
            var service = new CategoryService(context);
            int userId1 = user1.Id;
            int userId2 = user2.Id;

            service.addCategory(userId1, "Prezenty", "Opis");
            
            var result = service.addCategory(userId2, "Prezenty", "Opis");
            Assert.Equal("Poprawnie dodano kategorię", result);
        }

        [Fact]
        public void AddCategory_ShouldPreventDuplicateIfGlobalCategoryExists()
        {
            using var context = new AppDbContext(_contextOptions);
            
            var user = new DBUser { Login = "user1", Email = "u1@test.com", Password = "pwd" };
            context.Users.Add(user);
            
            // Manually add a global category
            context.Categories.Add(new DBCategory { Name = "Jedzenie", UserId = null });
            context.SaveChanges();

            var service = new CategoryService(context);
            int userId = user.Id;

            var result = service.addCategory(userId, "jedzenie", "Opis");
            Assert.Equal("Posiadasz już kategorię o tej samej nazwie", result);
        }
    }
}
