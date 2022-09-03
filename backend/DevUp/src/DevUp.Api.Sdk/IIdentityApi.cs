using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using Refit;

namespace DevUp.Api.Sdk
{
    public interface IIdentityApi
    {
        [Post(Route.Api.V1.Identity.Register)]
        public Task<ApiResponse<IdentityResponse>> Register([Body] RegisterUserRequest request, CancellationToken cancellationToken);

        [Post(Route.Api.V1.Identity.Login)]
        public Task<ApiResponse<IdentityResponse>> Login([Body] LoginUserRequest request, CancellationToken cancellationToken);

        [Post(Route.Api.V1.Identity.Refresh)]
        public Task<ApiResponse<IdentityResponse>> Refresh([Body] RefreshUserRequest request, CancellationToken cancellationToken);
    }
}
