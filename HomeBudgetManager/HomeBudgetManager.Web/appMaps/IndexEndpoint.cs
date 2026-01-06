using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class IndexEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/index", (HttpContext context, IWebHostEnvironment env) => {

                var filePath = Path.Combine(env.WebRootPath, "index.html");

                return Results.Content(File.ReadAllText(filePath), "text/html");
            });
        }
    }
}
