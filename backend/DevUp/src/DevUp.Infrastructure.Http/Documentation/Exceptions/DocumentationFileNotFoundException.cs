using DevUp.Infrastructure.Exceptions;

namespace DevUp.Infrastructure.Http.Documentation.Exceptions
{
    internal class DocumentationFileNotFoundException : InfrastructureException
    {
        public string Path { get; }

        public DocumentationFileNotFoundException(string path)
            : base($"Failed to locate documentation file at path: {path}.")
        {
            Path = path;
        }
    }
}
