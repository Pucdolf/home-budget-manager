using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Web.appMaps;
using HomeBudgetManager.Web.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace HomeBudgetManager.Web
{
    public static class EndpointExtensions
    {
        public static void MapAllEndpoints(this WebApplication app)
        {
            var endpointTypes = typeof(Program).Assembly.GetTypes()
                .Where(t => typeof(IEndpoint).IsAssignableFrom(t)
                && !t.IsInterface && !t.IsAbstract);

            foreach(var type in endpointTypes)
            {
                var instance = Activator.CreateInstance(type) as IEndpoint;

                instance?.Map(app);
            }
        }
    }




}