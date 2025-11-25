using UnityEngine;

namespace Scenes.Lobby
{
    public class PlayerSpawner: MonoBehaviour
    {
        [SerializeField] private Player _playerPrefab;
        private void Start()
        {
            GameManager.instance.playerRegistered.AddListener(_ => PlayerSpawn());
            if (GameManager.instance.Player != null)
            {
                PlayerSpawn();
            }
        }

        private void PlayerSpawn()
        {
            GameManager.instance.ControllingEntity.transform.position = transform.position;
            GameManager.instance.ControllingEntity.Rb.linearVelocity = Vector2.zero;
        }
    }
}