using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Web.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public interface IEndpoint
    {
        void Map(IEndpointRouteBuilder app);
    }
}
