using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlatformCreator : MonoBehaviour, IMonoBehaviour
{
    [LabelText("발판 크기")] public Vector2 size;
    private Vector2 topPivot = Vector2.zero;

    public Vector3 TopPivot
    {
        get => topPivot;
        set => topPivot = value;
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public virtual GameObject CreatePlatform(Vector2 position)
    {
        var platform = GameManager.Factory.Get(FactoryManager.FactoryType.Normal, "Platform", position);

        platform.transform.localScale = size;
        platform.transform.SetParent(transform);
        return platform;
    }

    public virtual void Return(GameObject platform)
    {
        GameManager.Factory.Return(platform);
    }
}