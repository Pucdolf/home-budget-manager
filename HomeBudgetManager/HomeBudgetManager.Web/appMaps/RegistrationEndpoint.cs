using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Web.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class RegistrationEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/registration", (HttpContext context, IWebHostEnvironment env) => {

                var filePath = Path.Combine(env.WebRootPath, "registration.html");

                var html = File.ReadAllText(filePath);

                return Results.Content(html, "text/html");
            });

        }
    }
}
