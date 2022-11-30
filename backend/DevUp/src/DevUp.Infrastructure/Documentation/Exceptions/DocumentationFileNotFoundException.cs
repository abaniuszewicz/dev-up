using DevUp.Infrastructure.Exceptions;

namespace DevUp.Infrastructure.Documentation.Exceptions
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
