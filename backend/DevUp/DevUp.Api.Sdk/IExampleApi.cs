﻿using DevUp.Api.Contracts.V1.Example.Requests;
using DevUp.Api.Contracts.V1.Example.Responses;
using Refit;

namespace DevUp.Api.Sdk
{
    public interface IExampleApi
    {
        [Get("api/v1/example")]
        public Task<ApiResponse<IEnumerable<ExampleResponse>>> Get();

        [Post("api/v1/example")]
        public Task Post([Body] ExampleRequest request);

    }
}
