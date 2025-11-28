using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Apis
{
    public class AddressableUtil
    {
        public static AsyncOperationHandle<GameObject> InstantiateAsync(AssetReference assetRef)
        {
            var handler = Addressables.InstantiateAsync(assetRef);

            handler.Completed += obj =>
            {
                if (obj.Result != null) obj.Result.AddComponent<AddressCleanUp>();
            };

            return handler;
        }

        public static AsyncOperationHandle<GameObject> InstantiateAsync(string address)
        {
            var handler = Addressables.InstantiateAsync(address);

            handler.Completed += obj => { obj.Result.AddComponent<AddressCleanUp>(); };

            return handler;
        }
    }
}