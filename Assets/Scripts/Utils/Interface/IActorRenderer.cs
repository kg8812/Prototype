using UnityEngine;

public interface IActorRenderer
{
    public Vector3 Pivot { get; }
    public Vector3 TopPivot { get; }

    public MeshRenderer MeshRenderer { get; }
    Vector3 GetPosition();
    void SetPosition(Vector3 position);
    void UpdateRenderer();

    public void Hide();
    public void Appear();
}