using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Application.PipelineBehaviors.Exceptions
{
    internal sealed class InputValidationException : DomainDataValidationException
    {
        public InputValidationException(string error) 
            : base(error)
        {
        }

        public InputValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}
