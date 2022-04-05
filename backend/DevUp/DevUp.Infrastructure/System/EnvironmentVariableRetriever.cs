using System;
using DevUp.Infrastructure.System.Exceptions;

namespace DevUp.Infrastructure.System
{
    internal class EnvironmentVariableRetriever : IEnvironmentVariableRetriever
    {
        public string GetVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName)
                ?? throw new EnvironmentVariableNotDefinedException(variableName);
        }
    }
}
