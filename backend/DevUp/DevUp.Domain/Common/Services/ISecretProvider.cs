using System;

namespace DevUp.Domain.Common.Services
{
    public interface ISecretProvider
    {
        public string Get(string key);
        public T Get<T>(string key, Func<string, T> castFunction);
    }
}
