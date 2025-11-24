using DG.Tweening;
using Default;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.UI;

namespace UI
{
    public class UI_CutScenePopup : UI_Popup
    {
        private float startTime;
        private const float skipDuration = 1.5f;
        public Coroutine skipKeyCoroutine;

        public override void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Skip)))
            {
                startTime = Time.unscaledTime;
                skipKeyCoroutine = GameManager.instance.StartCoroutineWrapper(SkipKeyBtn());
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Skip)))
            {
                startTime = Time.unscaledTime;
                skipKeyCoroutine = GameManager.instance.StartCoroutineWrapper(SkipKeyBtn());
            }
        }

        private IEnumerator SkipKeyBtn()
        {
            float time = Time.unscaledTime;
            while ((Gamepad.current != null &&
                    InputManager.GetButton(KeySettingManager.GetUIButton(Define.UIKey.Skip)) ||
                    InputManager.GetKey(KeySettingManager.GetUIKeyCode(Define.UIKey.Skip))) &&
                   startTime + skipDuration > time)
            {
                time = Time.unscaledTime;
                float ratio = (time - startTime) / skipDuration;
                skipBtnSprite.fillAmount = ratio;
                // Debug.Log($"skip key btn {ratio}");
                yield return null;
            }

            if (startTime + skipDuration <= Time.unscaledTime)
            {
                if (videoPlayer.isPlaying)
                {
                    StopVideo();
                }
            }
            else
            {
                skipBtnSprite.fillAmount = 0;
            }
        }

        enum Images
        {
            SkipBtnFill
        }


        public VideoPlayer videoPlayer;
        [HideInInspector] public Image skipBtnSprite;

        public UnityEvent OnVideoEnd = new();

        public override void Init()
        {
            base.Init();
            Bind<Image>(typeof(Images));
            skipBtnSprite = GetImage((int)Images.SkipBtnFill);
        }

        public void Init(string videoName)
        {
            string str = videoName;
            if (!videoName.Contains("Videos/"))
            {
                str = "Videos/" + videoName;
            }

            VideoClip clip = ResourceUtil.Load<VideoClip>(str);
            videoPlayer.clip = clip;
            PlayVideo();
        }

        Sequence seq;

        public void PlayVideo()
        {
            videoPlayer.Prepare();
            videoPlayer.Play();

            seq = DOTween.Sequence();
            seq.SetUpdate(true);
            seq.SetDelay((float)videoPlayer.clip.length);
            seq.AppendCallback(StopVideo);
        }

        private void StopVideo()
        {
            videoPlayer.Stop();
            GameManager.instance.StopCoroutineWrapper(skipKeyCoroutine);
            FadeManager.instance.Fading(() => { GameManager.UI.CloseUI(this); });

            if (seq.IsActive())
            {
                seq?.Kill();
            }
        }

        public override void TryActivated(bool force = false)
        {
            videoPlayer.clip = null;
            skipBtnSprite.fillAmount = 0;
            base.TryActivated(force);
        }


        protected override void Deactivated()
        {
            base.Deactivated();
            videoPlayer.Stop();
            videoPlayer.clip = null;

            OnVideoEnd.Invoke();

            OnVideoEnd.RemoveAllListeners();
            if (seq != null && seq.IsActive())
            {
                seq.Kill();
            }

            CancelInvoke();
        }
    }
}