using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Example.Requests;
using DevUp.Api.Contracts.V1.Example.Responses;
using Refit;

namespace DevUp.Api.Sdk
{
    public interface IExampleApi
    {
        [Get(Route.Api.V1.Example.Url)]
        public Task<ApiResponse<IEnumerable<ExampleResponse>>> Get();

        [Post(Route.Api.V1.Example.Url)]
        public Task Post([Body] ExampleRequest request);

    }
}
