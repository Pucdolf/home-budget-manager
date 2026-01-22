using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Web.appMaps
{
    public class ControllerEndpoints : IEndpoint
    {
        public record CreateCategoryDto(string Name, string? Description);

        public void Map(IEndpointRouteBuilder app)
        {

            app.MapGet("/categories/list", async (HttpContext context, AppDbContext db, CategoryService categoryService) =>
            {
                var loginUser = context.Request.Cookies["logged_user"];
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == loginUser);

                if (user == null)
                {
                    return Results.Content("<div class='error'>B³¹d: U¿ytkownik nieznaleziony.</div>", "text/html");
                }

                var categories = categoryService.listAllUserCategories(user.Id);

                // 3. Zbuduj HTML
                var htmlBuilder = new System.Text.StringBuilder();
                htmlBuilder.Append("<select id='category' name='categoryId' required class='form-input' onchange='handleCategoryChange(this)'>");

                htmlBuilder.Append("<option value=''>Wybierz kategoriê</option>");
                foreach (var cat in categories)
                {
                    htmlBuilder.Append($"<option value='{cat.Id}'>{cat.Name}</option>");
                }

                htmlBuilder.Append("<option value='new-category'>Dodaj kategoriê</option>");
                htmlBuilder.Append("</select>");

                return Results.Content(htmlBuilder.ToString(), "text/html");
            });

            app.MapPost("/categories/add", async (CreateCategoryDto dto, HttpContext context, AppDbContext db, CategoryService catService) =>
            {
                var loginUser = context.Request.Cookies["logged_user"];
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == loginUser);

                if (user == null)
                {
                    return Results.Content("<div class='error'>B³¹d: U¿ytkownik nieznaleziony.</div>", "text/html");
                }

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Results.Json(new { success = false, message = "Nazwa wymagana" });

                catService.addCategory(user.Id, dto.Name, dto.Description);

                return Results.Json(new { success = true });
            });
        }
    }
}
