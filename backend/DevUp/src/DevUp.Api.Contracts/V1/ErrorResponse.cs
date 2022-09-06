using System.Text.Json.Serialization;

namespace DevUp.Api.Contracts.V1
{
    public class ErrorResponse
    {
        public string Code { get; }
        public IEnumerable<string> Errors { get; }

        public ErrorResponse(string code, string error)
            : this(code, new[] { error })
        {
        }

        [JsonConstructor]
        public ErrorResponse(string code, IEnumerable<string> errors)
        {
            Code = code;
            Errors = errors;
        }
    }
}
