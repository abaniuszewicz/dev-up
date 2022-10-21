using Humanizer;
using Microsoft.AspNetCore.Routing;

namespace DevUp.Api.Configuration
{
    public sealed class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value?.ToString().Kebaberize();
        }
    }
}
