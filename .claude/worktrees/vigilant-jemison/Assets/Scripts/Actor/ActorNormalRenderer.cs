using Sirenix.OdinInspector;
using UnityEngine;

public class ActorNormalRenderer : MonoBehaviour , IActorRenderer
{
    [Tooltip("캐릭터 가운데 위치 값")] [SerializeField]
    private Vector3 pivot;

    [LabelText("상단 위치")] [SerializeField] private Vector3 topPivot;

    public Vector3 Pivot => pivot;

    public Vector3 TopPivot => topPivot;

    public Vector3 GetPosition()
    {
        return transform.position + pivot;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position - pivot;
    }

    public void UpdateRenderer()
    {
    }

    public void Hide()
    {
    }

    public void Appear()
    {
    }
}
