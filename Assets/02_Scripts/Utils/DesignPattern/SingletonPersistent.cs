using UnityEngine;

public class SingletonPersistent<T> : Singleton<T> where T : Component
{
    protected override void Awake()
    {
        base.Awake();
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}