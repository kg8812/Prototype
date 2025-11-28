using UnityEngine;

public interface IMonoBehaviour
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public Vector3 Position { get; set; } // 중앙 포지션
}