namespace DevUp.Application.Exceptions
{
    public abstract class ApplicationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public ApplicationException(string error)
            : this(new[] { error })
        {
        }

        public ApplicationException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}
