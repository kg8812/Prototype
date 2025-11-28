using Sirenix.OdinInspector;
using UnityEngine;

public abstract class PlatformObject : MonoBehaviour
{
    public bool applyToOnlyPlayer;

    [HideIf("applyToOnlyPlayer")] public LayerMask layers;

    protected bool CheckAvailable(GameObject other)
    {
        IPlayer iPlayer = null;

        if (other.transform.parent != null) iPlayer = other.transform.parent.GetComponent<IPlayer>();
        iPlayer ??= other.GetComponent<IPlayer>();
        return (applyToOnlyPlayer && iPlayer != null) || ((1 << other.layer) & layers) != 0;
    }
}