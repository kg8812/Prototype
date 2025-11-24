using System;
using Managers;
using UnityEngine;

namespace Apis.Components
{
    public class SpawnPoint : MonoBehaviour
    {
        public SectorManager.SectorType sectorType;
        static CustomQueue<Vector2> _spawnPos;
        public static CustomQueue<Vector2> spawnPos => _spawnPos ??= new();
        
        private bool isListen;
        private Player p;
        private static Action _afterSpawned;
        public bool isTest;
        
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
            if (!isTest && GameManager.SectorMag.CurrentSectors.ContainsKey(sectorType))
            {
                int sectorId = GameManager.SectorMag.CurrentSectors[sectorType];
                GameManager.SectorMag.LoadSector(sectorId, () =>
                {
                    Spawn();
                    GameManager.SectorMag.loadedSectors[sectorId].Enter();
                });
            }
            else
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            isListen = false;
            p = GameManager.instance.Player;
            if (p != null)
            {
                ResetPlayer(p);
            }
            else
            {
                if (GameManager.Scene.CurSceneData.isPlayerMustExist)
                {
                    isListen = true;
                    GameManager.instance.OnPlayerCreated.AddListener(ResetPlayer);
                    UI_CharacterSelect s =
                        GameManager.UI.CreateUI("UI_CharacterSelect", UIType.Scene) as UI_CharacterSelect;

                    if (s != null) s.ToggleCanClose(false);
                }
            }
            
            _afterSpawned?.Invoke();
            CameraManager.instance.InitPlayerCamPosition();
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


        private void OnDestroy()
        {
            if (isListen)
            {
                // 원래는 씬마다 하나씩 있어서 해제함
                // -> 한씬으로 합쳐짐.
                // GameManager.instance.playerRegistered.RemoveListener(ResetPlayer);
            }
        }
    }
}