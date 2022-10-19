using ApplicationException = DevUp.Application.Exceptions.ApplicationException;

namespace DevUp.Application.PipelineBehaviors.Exceptions
{
    public sealed class ApplicationValidationException : ApplicationException
    {
        public ApplicationValidationException(string error) 
            : base(error)
        {
        }

        public ApplicationValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}
