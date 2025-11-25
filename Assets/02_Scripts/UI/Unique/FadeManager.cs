using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
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
        

        private bool _isFadingIn = false;
        private bool _isFadingOut = false;

        private Guid _fadeStateGuid;

        public bool IsFadingIn => _isFadingIn;
        public bool IsFadingOut => _isFadingOut;
        
        private Queue<UnityAction> _fadeMiddleQueue = new();
        private Queue<UnityAction> _fadeEndQueue = new();
        
        Queue<Coroutine> _loadingQueue;
        public Queue<Coroutine> LoadingQueue => _loadingQueue ??= new();
        protected override void Awake()
        {
            base.Awake();
            fadeIn = new();
            _isFadingIn = false;
            _isFadingOut = false;
        }

        Tween fadingTween;
        private bool isLoading = false;
        
        public void Fading(UnityAction middleAction, UnityAction endAction = null, float fadeDuration = 1f, bool isFadeIn = true, bool isFadeOut = true, bool enterDefaultState = true)
        {
            if (_isFadingOut)
            {
                FadeOuted();
            }
            
            _fadeEndQueue.Enqueue(endAction);

            if (isLoading)
            {
                middleAction?.Invoke();
                return;
            }
            
            _fadeMiddleQueue.Enqueue(middleAction);
            

            if (_isFadingIn) return;
            
            _isFadingIn = true;
            _isFadingOut = false;
            
            GameManager.PreventControl = true;

            if (enterDefaultState)
            {
                if (_fadeStateGuid != Guid.Empty)
                {
                    // Debug.LogError("fade guid가 empty가 아니데 try on 중");
                    Guid preGuid = _fadeStateGuid;
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

        IEnumerator FadeCoroutine(bool isFadeOut,float fadeDuration)
        {
            _isFadingIn = false;
            if (isLoading) yield break;
            isLoading = true;
            
            while (_fadeMiddleQueue.Count > 0)
            {
                _fadeMiddleQueue.Dequeue()?.Invoke();
            }
            fadeIn.Invoke();
            fadeIn.RemoveAllListeners();

            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            
            while (LoadingQueue.Count > 0)
            {
                var loadingCoroutine = LoadingQueue.Dequeue();
                if (loadingCoroutine != null)
                {
                    yield return loadingCoroutine;
                }
            }

            // 컨파이너 진입 안할 시 페이딩이 풀리지 않아서 일단 최대 5초로 임시처리해놨습니다.
            
            float curTime = 0;
            
            while (GameManager.instance.Player != null  && curTime < 5)
            {
                curTime += 0.03f;
                yield return new WaitForSecondsRealtime(0.03f);
            }
            
            HandleFadeOut(isFadeOut, fadeDuration);
            isLoading = false;
            GameManager.PreventControl = false;
        }
        private void HandleFadeOut(bool isFadeOut, float duration)
        {
            if (GameManager.Scene.CurSceneData.isPlayerMustExist && _fadeStateGuid != Guid.Empty)
            {
                GameManager.instance.TryOffGameState(GameStateType.DefaultState,_fadeStateGuid);
                _fadeStateGuid = Guid.Empty;
            } 
            
            if (isFadeOut)
            {
                fadingTween = canvasGroup.DOFade(0, duration).SetUpdate(true).OnComplete(FadeOuted);
                _isFadingOut = true;
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
            _isFadingOut = false;
            canvasGroup.alpha = 0;
            
            while (_fadeEndQueue.Count > 0)
            {
                _fadeEndQueue.Dequeue()?.Invoke();
            }
            _fadeMiddleQueue.Clear();
        }
    }
}