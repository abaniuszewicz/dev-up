using System;
using DevUp.Domain.Common.Exceptions;
using DevUp.Domain.Common.Services;

namespace DevUp.Infrastructure.System
{
    public class EnvSecretProvider : ISecretProvider
    {
        public string Get(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(value))
                throw new SecretNotFoundException(key);

            return value;
        }

        public T Get<T>(string key, Func<string, T> transformFunction)
        {
            var value = Get(key);

            try
            {
                return transformFunction(value);
            }
            catch
            {
                throw new SecretCastException<T>(key);
            }
        }
    }
}
