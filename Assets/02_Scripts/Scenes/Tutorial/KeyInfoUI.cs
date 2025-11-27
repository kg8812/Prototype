using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    public class KeyInfoUI : MonoBehaviour
    {
        [SerializeField] private Canvas canv;
        public Image[] uis;
        public TextMeshProUGUI[] texts;

        private readonly float duration = 0.6f;

        private void Start()
        {
            canv.worldCamera = CameraManager.instance.UICam;
        }

        public void ShowKeyUI(int id)
        {
            uis[id].DOFade(1, duration);
        }

        public void ShowKeyUIText(int id)
        {
            texts[id].DOFade(1, duration);
        }
    }
}