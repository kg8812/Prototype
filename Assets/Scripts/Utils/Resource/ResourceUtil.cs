using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apis;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Default
{
    public static class ResourceUtil
    {
        public static T Load<T>(string path) where T : Object
        {
            T resource;
            try
            {
                if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    var temp = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
                    resource = temp.GetComponent<T>();
                }
                else
                {
                    resource = Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
                }
            }
            catch
            {
                //Debug.Log("Resource not found: " + path);
                return null;
            }

            return resource;
        }

        public static T Load<T>(AssetReference assetRef) where T : Object
        {
            T resource;
            try
            {
                if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    var temp = Addressables.LoadAssetAsync<GameObject>(assetRef).WaitForCompletion();
                    resource = temp.GetComponent<T>();
                }
                else
                {
                    resource = Addressables.LoadAssetAsync<T>(assetRef).WaitForCompletion();
                }
            }
            catch
            {
                Debug.Log("Resource not found: " + assetRef);
                return null;
            }

            return resource;
        }

        public static T[] LoadAll<T>(string path) where T : Object
        {
            T[] resources;

            try
            {
                if (typeof(T).IsSubclassOf(typeof(Component)))
                    resources = Addressables.LoadAssetsAsync<GameObject>(path, _ => { }).WaitForCompletion()
                        .Select(x => x.GetComponent<T>()).Where(x => x != null).ToArray();
                else
                    resources = Addressables.LoadAssetsAsync<T>(path, _ => { }).WaitForCompletion().ToArray();
            }
            catch
            {
                Debug.Log("Label not found");
                return null;
            }

            return resources;
        }

        public static void LoadAsync<T>(string key, Action<T> onSuccess, Action onFail = null) where T : class
        {
            Addressables.LoadAssetAsync<T>(key).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    onSuccess?.Invoke(handle.Result);
                }
                else
                {
                    Debug.LogError($"[AddressableLoader] Failed to load asset with key: {key}");
                    onFail?.Invoke();
                }
            };
        }

        public static void LoadAllAsync<T>(string label, UnityAction<IList<T>> completeAction) where T : Object
        {
            Addressables.LoadAssetsAsync<T>(label, _ => { }).Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                    Debug.LogError($"addressable loadAllAsync '{label}' is failed.");
                completeAction.Invoke(handle.Result);
            };
        }

        public static GameObject Instantiate(string path, Transform parent = null)
        {
            var async = AddressableUtil.InstantiateAsync(path);
            var go = async.WaitForCompletion();
            if (go == null) return null;

            if (parent != null) go.transform.SetParent(parent);

            var index = go.name.IndexOf("(Clone)");
            if (index > 0)
                go.name = go.name[..index];
            return go;
        }

        public static async Task<GameObject> InstantiateAsync(string path, Transform parent = null)
        {
            var handle = AddressableUtil.InstantiateAsync(path);
            var go = await handle.Task;

            if (go == null) return null;

            if (parent != null)
                go.transform.SetParent(parent);

            var index = go.name.IndexOf("(Clone)");
            if (index > 0)
                go.name = go.name[..index];

            return go;
        }

        public static void Release<T>(T resource) where T : Object
        {
            if (resource != null) Addressables.Release(resource);
        }
    }
}