using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;


public abstract partial class Actor
{
    protected MeshRenderer meshRenderer;

    Tweener blinkTweener;
    private IEnumerator _blinkCoroutine;

    public bool HitImmune => ImmunityController.IsImmune("HitImmunity");

    public Guid AddHitImmunity()
    {
        if (!ImmunityController.Contains("HitImmunity"))
        {
            ImmunityController.MakeNewType("HitImmunity");
        }
        return ImmunityController.AddCount("HitImmunity");
    }

    public void RemoveHitImmunity(Guid guid)
    {
        ImmunityController.MinusCount("HitImmunity",guid);
    }
    public void ForceRemoveHitImmunity()
    {
        ImmunityController.MakeCountToZero("HitImmunity");
    }
    /* 피격시 깜빡임 함수 
    Skeleton을 Child로 가진 액터만 사용 가능
    Skeleton의 meterial shader를 skeleton fill 모드로 설정해야 사용 가능 */
    public void InvokeBlink()
    {
        // blinkTweener?.Kill();
        // if(_blinkCoroutine != null) { StopCoroutine(_blinkCoroutine); }
        // _blinkCoroutine = Blink();
        //
        // if(gameObject.activeSelf)
        //     StartCoroutine(_blinkCoroutine);
    }

    //IEnumerator Blink()
    //{
        // if (meshRenderer == null) yield break;
        //
        // MaterialPropertyBlock mpblock = new MaterialPropertyBlock();
        // mpblock.SetFloat("_FillPhase", 0);
        //
        // 빙결 PropertyBlock와 겹쳐서 주석처리했습니다.
        
        // meshRenderer.SetPropertyBlock(mpblock);
        // blinkTweener = DOTween.To(()=>0, x=> { mpblock.SetFloat("_FillPhase", x); meshRenderer.SetPropertyBlock(mpblock); }, 0.8f, blinkFrequency/2f)
        //                 .SetLoops((int)Mathf.Round(blinkTime/blinkFrequency) * 2, LoopType.Yoyo);
        //yield return new WaitForSeconds(blinkTime);
        //?.Kill();
    //}

    protected Coroutine KnockBackCoroutine;

    private UnityEvent _onHide;
    private UnityEvent _onAppear;
    
    public UnityEvent OnHide => _onHide ??= new();
    public UnityEvent OnAppear => _onAppear ??= new();
    public void Hide()
    {
        if(!meshRenderer.enabled) return;
        meshRenderer.enabled = false;
        
        OnHide.Invoke();
    }

    public void Appear()
    {
        if(meshRenderer.enabled) return;
        meshRenderer.enabled = true;
        OnAppear.Invoke();
    }
}
