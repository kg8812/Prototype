using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    //싱글톤용 클래스입니다. 싱글톤 필요한 객체 상속 받으시면 됩니다.

    private static T _instance;

    private static bool spawned;

    public static T instance
    {
        get
        {
            _instance ??= FindAnyObjectByType<T>();

            if (_instance == null && !spawned) _instance = new GameObject($"{typeof(T).Name}").AddComponent<T>();

            if (_instance.name == "SpawnPoint") Debug.Log(_instance.name);

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        spawned = true;
        if (_instance == null)
            _instance = this as T;
        else if (_instance != this) Destroy(gameObject);
    }
}