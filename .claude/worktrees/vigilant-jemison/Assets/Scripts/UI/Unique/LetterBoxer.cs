using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Managers;
using UnityEngine.Events;

namespace Apis.UI
{
    public class LetterBoxer : Singleton<LetterBoxer>
    {
        public Canvas letterCan;
        [SerializeField] private RectTransform letterTrans;
        [SerializeField] private Image[] letter;
        [SerializeField] private float letterSize = 100f;

        private bool toggleLetter = false;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            // _anim = GetComponent<Animator>();

            letter[0].GetComponent<RectTransform>().sizeDelta = new Vector2(0, letterSize);
            letter[1].GetComponent<RectTransform>().sizeDelta = new Vector2(0, letterSize);

            letterTrans.sizeDelta = new Vector2(0, 1080 + letterSize * 2);
        }

        private void Start()
        {
            letterCan.worldCamera = CameraManager.instance.UICam;
            
        }


        public void ToggleLetterBox(bool isOn, float duration = 1f, UnityAction onStarted = null, UnityAction onEnded = null)
        {
            if (isOn == toggleLetter) return;
            toggleLetter = isOn;

            DOTween.Sequence()
                .OnStart(() =>
                {
                    onStarted?.Invoke();
                })
                .Append(DOTween.To(
                    () => letterTrans.sizeDelta, 
                    x => letterTrans.sizeDelta = x,
                    new Vector2(0, 1080 + (toggleLetter ? 0 : letterSize * 2)), 
                    duration))
                .OnComplete(() =>
                {
                    onEnded?.Invoke();
                });
        }
    }
}