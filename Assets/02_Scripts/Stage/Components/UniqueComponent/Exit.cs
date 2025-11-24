using System;
using UnityEngine;

namespace Apis.Components
{
    public class Exit : MonoBehaviour, IOnInteract
    {
        private void Awake()
        {
        }

        public Func<bool> InteractCheckEvent { get; set; }

        public void OnInteract()
        {
            GameManager.Scene.SceneLoad(Define.SceneNames.MainWorldSceneName);
        }
    }
}