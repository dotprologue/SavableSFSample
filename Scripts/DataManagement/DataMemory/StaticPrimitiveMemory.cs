using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SavableSFSample
{
    public class StaticPrimitiveMemory : IPrimitiveSaver, IPrimitiveLoader
    {
        private static readonly Dictionary<string, object> primitiveDictionary = new Dictionary<string, object>();

        public UniTask SaveAsync<T>(string path, T value, CancellationToken cancellationToken) where T : class
        {
            primitiveDictionary[path] = value;
            return UniTask.CompletedTask;
        }

        public UniTask<T> LoadAsync<T>(string path, CancellationToken cancellationToken) where T : class
        {
            if (!primitiveDictionary.ContainsKey(path))
            {
                throw new ArgumentException($"Data don't exist at '{path}'.");
            }
            var obj = primitiveDictionary[path];
            if (obj is T result)
            {
                return UniTask.FromResult(result);
            }
            else
            {
                throw new ArgumentException($"The type of the data is not '{typeof(T).FullName}'.");
            }
        }

        public bool Exist(string path)
        {
            return primitiveDictionary.ContainsKey(path);
        }
    }
}
