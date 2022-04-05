namespace DevUp.Infrastructure.System
{
    public interface IEnvironmentVariableRetriever
    {
        public string GetVariable(string variableName);
    }
}
