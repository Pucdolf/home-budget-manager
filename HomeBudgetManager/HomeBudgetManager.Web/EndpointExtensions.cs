using HomeBudgetManager.Web.appMaps;

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