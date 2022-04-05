using System;

namespace DevUp.Infrastructure.System.Exceptions
{
    public class EnvironmentVariableNotDefinedException : InvalidOperationException
    {
        public string VaribleName { get; set; }

        public EnvironmentVariableNotDefinedException(string variableName)
            : this(variableName, $"Environment variable '{variableName}' is not defined")
        {
        }

        public EnvironmentVariableNotDefinedException(string variableName, string message) : base(message)
        {
            VaribleName = variableName;
        }
    }
}
