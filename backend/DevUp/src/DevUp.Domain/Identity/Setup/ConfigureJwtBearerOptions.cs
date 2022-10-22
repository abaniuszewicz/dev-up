using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace DevUp.Domain.Identity.Setup
{
    internal sealed class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly AuthenticationOptions _authenticationOptions;

        public ConfigureJwtBearerOptions(IOptions<AuthenticationOptions> authenticationOptions)
        {
            _authenticationOptions = authenticationOptions.Value;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            Configure(options);
        }

        public void Configure(JwtBearerOptions options)
        {
            options.SaveToken = true;
            options.TokenValidationParameters = _authenticationOptions.GetTokenValidationParameters();
        }
    }
}
