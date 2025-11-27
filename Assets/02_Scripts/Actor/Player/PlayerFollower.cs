using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    private Player _player;
    private Transform thisTrans;

    private void Awake()
    {
        thisTrans = transform;
        DontDestroyOnLoad(gameObject);
        GameManager.instance.playerRegistered.AddListener(player => { _player = player; });
    }

    private void LateUpdate()
    {
        if (_player != null) thisTrans.position = _player.Position;
    }
}