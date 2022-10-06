using Humanizer;
using Microsoft.AspNetCore.Routing;

namespace DevUp.Api.Configuration
{
    internal class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value?.ToString().Kebaberize();
        }
    }
}
