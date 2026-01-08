using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Web.appMaps
{
    public class ControllerEndpoints : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {

            app.MapGet("/list-categories", async (HttpContext context, AppDbContext db, CategoryService categoryService) =>
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
                htmlBuilder.Append("<select id='category' name='categoryId' required class='form-input'>");

                htmlBuilder.Append("<option value=''>Wybierz kategoriê</option>");
                foreach (var cat in categories)
                {
                    htmlBuilder.Append($"<option value='{cat.Id}'>{cat.Name}</option>");
                }

                htmlBuilder.Append("</select>");

                return Results.Content(htmlBuilder.ToString(), "text/html");
            });
        }
    }
}
