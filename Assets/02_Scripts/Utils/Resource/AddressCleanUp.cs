using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressCleanUp : MonoBehaviour
{
    private void OnDestroy()
    {
        Addressables.ReleaseInstance(gameObject);
    }
}