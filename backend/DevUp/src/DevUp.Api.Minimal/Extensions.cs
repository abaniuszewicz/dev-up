using DevUp.Api.Minimal.V1;

namespace DevUp.Api.Minimal
{
    public static class Extensions
    {
        public static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
        {
            var endpointTypes = typeof(IMinimalApiMarker).Assembly.DefinedTypes
                .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpoints).IsAssignableFrom(t));

            foreach (var endpointType in endpointTypes)
            {
                endpointType.GetMethod(nameof(IEndpoints.DefineEndpoints))!
                    .Invoke(null, new object[] { app });
            }

            return app;
        }
    }
}
