using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using Refit;

namespace DevUp.Api.Sdk
{
    public interface IIdentityApi
    {
        [Post("api/v1/register")]
        public Task<ApiResponse<IdentityResponse>> Register([Body] RegisterUserRequest request, CancellationToken cancellationToken);

        [Post("api/v1/login")]
        public Task<ApiResponse<IdentityResponse>> Login([Body] LoginUserRequest request, CancellationToken cancellationToken);

        [Post("api/v1/refresh")]
        public Task<ApiResponse<IdentityResponse>> Refresh([Body] RefreshUserRequest request, CancellationToken cancellationToken);
    }
}
