using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SavableSFSample
{
    //Store assets that are mainly provided to decoders for unity object type
    public class AssetStorage : IAssetStorage, IAssetProvider, IAssetSerializer, IAssetDeserializer
    {
        private readonly IAssetLoader assetLoader;
        private readonly Dictionary<Type, Dictionary<string, UnityEngine.Object>> nameToAssetDictionary = new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();
        private readonly Dictionary<Type, Dictionary<string, IEnumerable<string>>> pathToNamesDictionary = new Dictionary<Type, Dictionary<string, IEnumerable<string>>>();

        public AssetStorage(IAssetLoader assetLoader)
        {
            this.assetLoader = assetLoader ?? throw new ArgumentNullException(nameof(assetLoader));
        }

        public async UniTask LoadAssetsAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object
        {
            //If there are not any assets of the same type, add that type to the dictionary
            if (!pathToNamesDictionary.ContainsKey(typeof(T)))
            {
                nameToAssetDictionary.Add(typeof(T), new Dictionary<string, UnityEngine.Object>());
                pathToNamesDictionary.Add(typeof(T), new Dictionary<string, IEnumerable<string>>());
            }
            //Make sure that assets with the same path have not been loaded
            if (pathToNamesDictionary[typeof(T)].ContainsKey(assetPath))
            {
                throw new ArgumentException($"Assets at '{assetPath}' have been loaded already.");
            }
            //Load assets
            var assets = await assetLoader.LoadAssetsAsync<T>(assetPath, cancellationToken);
            //Make sure that there are not any asset with the same name
            foreach (var asset in assets)
            {
                if (nameToAssetDictionary[typeof(T)].ContainsKey(asset.name))
                {
                    throw new ArgumentException($"Asset '{asset.name}' exists already.");
                }
            }
            //Register the loaded assets
            pathToNamesDictionary[typeof(T)].Add(assetPath, assets.Select(a => a.name));
            foreach (var asset in assets)
            {
                nameToAssetDictionary[typeof(T)].Add(asset.name, asset);
            }
        }

        public async UniTask UnloadAssetsAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object
        {
            //Make sure that assets at the asset path exist
            if (!pathToNamesDictionary.ContainsKey(typeof(T)) || !pathToNamesDictionary[typeof(T)].ContainsKey(assetPath))
            {
                throw new ArgumentException($"Assets at '{assetPath}' have not been loaded.");
            }
            //Remove assets
            foreach (var name in pathToNamesDictionary[typeof(T)][assetPath])
            {
                nameToAssetDictionary[typeof(T)].Remove(name);
            }
            pathToNamesDictionary[typeof(T)].Remove(assetPath);
            //Unload assets
            await assetLoader.UnloadAssetsAsync<T>(assetPath, cancellationToken);
        }

        public IEnumerable<string> GetPathAll<T>() where T : UnityEngine.Object
        {
            if (pathToNamesDictionary.ContainsKey(typeof(T)))
            {
                return pathToNamesDictionary[typeof(T)].Keys.ToArray();
            }
            else
            {
                return new string[] { };
            }
        }

        public T GetAsset<T>(string assetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }
            //Make sure that the asset exists
            if (!nameToAssetDictionary.ContainsKey(typeof(T)) || !nameToAssetDictionary[typeof(T)].ContainsKey(assetName))
            {
                throw new ArgumentException($"Asset '{assetName}' has not been loaded.");
            }
            return (T)nameToAssetDictionary[typeof(T)][assetName];
        }

        public string Serialize<T>(T input) where T : UnityEngine.Object
        {
            return input.name;
        }

        public T Deserialize<T>(string input) where T : UnityEngine.Object
        {
            return GetAsset<T>(input);
        }
    }
}