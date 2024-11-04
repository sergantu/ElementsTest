using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Infrastructure.AssetManagement
{
    public class AssetProvider
    {
        private readonly IObjectResolver _objectResolver;
        private readonly Dictionary<string, AsyncOperationHandle> _completedCache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();

        public AssetProvider(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void Construct() => Addressables.InitializeAsync();

        public async UniTask<T> Load<T>(AssetReference assetReference)
                where T : class
        {
            if (_completedCache.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completedHandle)) {
                return completedHandle.Result as T;
            }

            return await RunWithCacheOnComplete(Addressables.LoadAssetAsync<T>(assetReference), assetReference.AssetGUID);
        }

        public async UniTask<T> Load<T>(string address)
                where T : class
        {
            if (_completedCache.TryGetValue(address, out AsyncOperationHandle completedHandle)) {
                return completedHandle.Result as T;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            return await RunWithCacheOnComplete(handle, address);
        }

        public async UniTask<GameObject> Instantiate(string path, Vector3 at)
        {
            GameObject test = await Addressables.InstantiateAsync(path, at, Quaternion.identity);
            _objectResolver.InjectGameObject(test);
            return test;
        }

        public async UniTask<GameObject> Instantiate(string path, Transform at)
        {
            GameObject obj = await Addressables.InstantiateAsync(path, at);
            _objectResolver.InjectGameObject(obj);
            return obj;
        }

        public async UniTask<GameObject> Instantiate(string path, Vector3 position, Transform parent)
        {
            GameObject obj = await Addressables.InstantiateAsync(path, position, Quaternion.identity, parent);
            _objectResolver.InjectGameObject(obj);
            return obj;
        }

        public virtual async UniTask<GameObject> Instantiate(string path)
        {
            GameObject obj = await Addressables.InstantiateAsync(path);
            _objectResolver.InjectGameObject(obj);
            return obj;
        }

        public async UniTask<string> LoadTextAsset(string path)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(path);
            await handle.Task;

            string result = handle.Status == AsyncOperationStatus.Succeeded ? handle.Result.text : String.Empty;
            Addressables.Release(handle);
            return result;
        }

        public void Cleanup()
        {
            foreach (var resourcesHandles in _handles.Values) {
                foreach (var handle in resourcesHandles) {
                    Addressables.Release(handle);
                }
            }

            _completedCache.Clear();
            _handles.Clear();
        }

        private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey)
                where T : class
        {
            handle.Completed += completedHandle => { _completedCache[cacheKey] = completedHandle; };
            AddHandle(cacheKey, handle);
            return await handle.Task;
        }

        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle)
                where T : class
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles)) {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }

            resourceHandles.Add(handle);
        }
    }
}