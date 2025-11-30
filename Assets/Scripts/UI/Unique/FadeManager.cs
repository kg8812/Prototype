using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameStateSpace;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class FadeManager : SingletonPersistent<FadeManager>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Canvas canvas;


        public UnityEvent fadeIn;
        private readonly Queue<UnityAction> _fadeEndQueue = new();

        private readonly Queue<UnityAction> _fadeMiddleQueue = new();

        private Guid _fadeStateGuid;


        private Queue<Coroutine> _loadingQueue;

        private Tween fadingTween;
        private bool isLoading;

        public bool IsFadingIn { get; private set; }

        public bool IsFadingOut { get; private set; }

        public Queue<Coroutine> LoadingQueue => _loadingQueue ??= new Queue<Coroutine>();

        protected override void Awake()
        {
            base.Awake();
            fadeIn = new UnityEvent();
            IsFadingIn = false;
            IsFadingOut = false;
        }

        public void Fading(UnityAction middleAction, UnityAction endAction = null, float fadeDuration = 1f,
            bool isFadeIn = true, bool isFadeOut = true, bool enterDefaultState = true)
        {
            if (IsFadingOut) FadeOuted();

            _fadeEndQueue.Enqueue(endAction);

            if (isLoading)
            {
                middleAction?.Invoke();
                return;
            }

            _fadeMiddleQueue.Enqueue(middleAction);


            if (IsFadingIn) return;

            IsFadingIn = true;
            IsFadingOut = false;

            GameManager.PreventControl = true;

            if (enterDefaultState)
            {
                if (_fadeStateGuid != Guid.Empty)
                {
                    // Debug.LogError("fade guid가 empty가 아니데 try on 중");
                    var preGuid = _fadeStateGuid;
                    _fadeStateGuid = GameManager.instance.TryOnGameState(GameStateType.DefaultState);
                    GameManager.instance.TryOffGameState(GameStateType.DefaultState, preGuid);
                }
                else
                {
                    _fadeStateGuid = GameManager.instance.TryOnGameState(GameStateType.DefaultState);
                }
            }


            if (isFadeIn)
            {
                canvasGroup.alpha = 0;
                canvas.enabled = true;
                canvasGroup.DOFade(1, fadeDuration).SetUpdate(true).OnComplete(() =>
                {
                    // Debug.Log("fade");
                    GameManager.instance.StartCoroutine(FadeCoroutine(isFadeOut, fadeDuration));
                });
            }
            else
            {
                canvasGroup.alpha = 1;
                canvas.enabled = true;
                GameManager.instance.StartCoroutine(FadeCoroutine(isFadeOut, fadeDuration));
            }
        }

        private IEnumerator FadeCoroutine(bool isFadeOut, float fadeDuration)
        {
            IsFadingIn = false;
            if (isLoading) yield break;
            isLoading = true;

            while (_fadeMiddleQueue.Count > 0) _fadeMiddleQueue.Dequeue()?.Invoke();
            fadeIn.Invoke();
            fadeIn.RemoveAllListeners();

            for (var i = 0; i < 3; i++) yield return new WaitForEndOfFrame();

            while (LoadingQueue.Count > 0)
            {
                var loadingCoroutine = LoadingQueue.Dequeue();
                if (loadingCoroutine != null) yield return loadingCoroutine;
            }

            HandleFadeOut(isFadeOut, fadeDuration);
            isLoading = false;
            GameManager.PreventControl = false;
        }

        private void HandleFadeOut(bool isFadeOut, float duration)
        {
            if (GameManager.Scene.CurSceneData.isPlayerMustExist && _fadeStateGuid != Guid.Empty)
            {
                GameManager.instance.TryOffGameState(GameStateType.DefaultState, _fadeStateGuid);
                _fadeStateGuid = Guid.Empty;
            }

            if (isFadeOut)
            {
                fadingTween = canvasGroup.DOFade(0, duration).SetUpdate(true).OnComplete(FadeOuted);
                IsFadingOut = true;
            }
            else
            {
                canvasGroup.alpha = 0;
                FadeOuted();
            }
        }


        private void FadeOuted()
        {
            fadingTween?.Kill();
            canvas.enabled = false;
            IsFadingOut = false;
            canvasGroup.alpha = 0;

            while (_fadeEndQueue.Count > 0) _fadeEndQueue.Dequeue()?.Invoke();
            _fadeMiddleQueue.Clear();
        }
    }
}