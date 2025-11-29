using System;
using Apis;
using Managers;
using UnityEngine;

namespace chamwhy.Components
{
    public class SpawnPoint : MonoBehaviour
    {
        static CustomQueue<Vector2> _spawnPos;
        public static CustomQueue<Vector2> spawnPos => _spawnPos ??= new();
        
        private Player p;
        private static Action _afterSpawned;
        
        public static event Action AfterSpawned
        {
            add
            {
                _afterSpawned -= value;
                _afterSpawned += value;
            }
            remove => _afterSpawned -= value;
        }

        private void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            p = GameManager.instance.Player;
            if (p != null)
            {
                ResetPlayer(p);
            }
            else
            {
                if (GameManager.Scene.CurSceneData.isPlayerMustExist)
                {
                    Player.CreatePlayerByType(PlayerType.Player1);
                    GameManager.instance.OnPlayerCreated.AddListener(ResetPlayer);
                }
            }
            
            _afterSpawned?.Invoke();
        }

        public void ResetPlayer(Player player)
        {
            GameManager.instance.OnPlayerCreated.RemoveListener(ResetPlayer);
            Vector2 pos = transform.position;
            while (spawnPos.Count > 0)
            {
                pos = spawnPos.Dequeue();
            }
            player.transform.position = pos;
            player.gameObject.SetActive(true);
            player.CorrectingPlayerPosture();
        }
    }
}