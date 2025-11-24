using System;
using System.Collections;
using System.Collections.Generic;
using Default;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimator: MonoBehaviour
{
    private Player _player;
    [SerializeField] private Animator _animator;
    public Animator Animator => _animator;

    private Dictionary<EAnimationBool, int> boolHash;
    private Dictionary<EAnimationInt, int> intHash;
    private Dictionary<EAnimationFloat, int> floatHash;
    private Dictionary<EAnimationTrigger, int> triggerHash;

    private UnityAction<string> _OnTransitionEvent;
    public UnityAction<string> OnTransitionEvent => _OnTransitionEvent;

    private SkeletonMecanimRootMotion rootMotion;

    public void Awake() 
    {
        _player = gameObject.GetComponent<Player>();

        boolHash = new Dictionary<EAnimationBool, int>();
        intHash = new Dictionary<EAnimationInt, int>();
        floatHash = new Dictionary<EAnimationFloat, int>();
        triggerHash = new Dictionary<EAnimationTrigger, int>();
        rootMotion = transform.GetComponentInParentAndChild<SkeletonMecanimRootMotion>();

        Hashing();
    }

    public void Start() 
    {
    }

    public void FixedUpdate()
    {
        
    }

    public int GetHash(EAnimationBool key) => boolHash[key];
    public int GetHash(EAnimationTrigger key) => triggerHash[key];
    public int GetHash(EAnimationInt key) => intHash[key];
    public int GetHash(EAnimationFloat key) => floatHash[key];
    public int GetHash(string key) => Animator.StringToHash(key);
    
    private void Hashing()
    {
        foreach (EAnimationBool par in Enum.GetValues(typeof(EAnimationBool)))
            boolHash.Add(par, Animator.StringToHash(par.ToString()));

        foreach (EAnimationInt par in Enum.GetValues(typeof(EAnimationInt)))
            intHash.Add(par, Animator.StringToHash(par.ToString()));

        foreach (EAnimationFloat par in Enum.GetValues(typeof(EAnimationFloat)))
            floatHash.Add(par, Animator.StringToHash(par.ToString()));

        foreach (EAnimationTrigger par in Enum.GetValues(typeof(EAnimationTrigger)))
            triggerHash.Add(par, Animator.StringToHash(par.ToString()));
    }
 
    public void ResetTrigger(EAnimationTrigger key)
    {
        if(!triggerHash.ContainsKey(key)) return;

        _animator.ResetTrigger(triggerHash[key]);
    }

    public void SetTrigger(EAnimationTrigger key)
    {
        if(!triggerHash.ContainsKey(key)) return;

        _animator.SetTrigger(triggerHash[key]);
    }

    public void SetBool(EAnimationBool key, bool value)
    {
        if(!boolHash.ContainsKey(key)) return;

        _animator.SetBool(boolHash[key], value);
    }

    public void SetInteger(EAnimationInt key, int value)
    {
        if(!intHash.ContainsKey(key)) return;

        _animator.SetInteger(intHash[key], value);
    }

    public void SetFloat(EAnimationFloat key, float value)
    {
        if(!floatHash.ContainsKey(key)) return;

        _animator.SetFloat(floatHash[key], value);
    }

    private EAnimationTrigger recentTrigger = EAnimationTrigger.IdleOn;
    public void Trigger(EAnimationTrigger key)
    {
        if(!triggerHash.ContainsKey(key)) return;

        _animator.ResetTrigger(triggerHash[recentTrigger]);

        _animator.SetTrigger(triggerHash[key]);

        recentTrigger = key;
    }

    public void ActivateLeg()
    {
        _animator.SetLayerWeight(4, 1);
    }

    public void DeactivateLeg()
    {
        _animator.SetLayerWeight(4, 0);
    }

    public void WaitUntilAnimStart(string animName, UnityAction onEnd)
    {
        IEnumerator WaitTransit()
        {
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(animName));
            onEnd.Invoke();
        }
        GameManager.instance.StartCoroutineWrapper(WaitTransit());
    }

    public float GetCurrentClipLength(int layer)
    {
        return _animator.GetCurrentAnimatorClipInfo(layer)[0].clip.length;
    }

    public void SetRootmotionOffset(float x, float y)
    {
        if(rootMotion == null) return;

        rootMotion.rootMotionTranslateXPerY = x;
        rootMotion.rootMotionTranslateYPerX = y;
    }

    public Vector2 GetRootmotionOffset()
    {
        if(rootMotion == null) return Vector2.zero;

        return new Vector2(rootMotion.rootMotionTranslateXPerY, rootMotion.rootMotionTranslateYPerX);
    }

    public void AddRootmotionOffset(float x, float y)
    {
        if(rootMotion == null) return;

        rootMotion.rootMotionTranslateXPerY += x;
        rootMotion.rootMotionTranslateYPerX += y;

    }

    public void SetRootMotionScale(float x, float y)
    {
        if (rootMotion == null) return;

        rootMotion.rootMotionScaleX = x;
        rootMotion.rootMotionScaleY = y;
    }
    
    public Vector2 GetRootMotionScale()
    {
        if (rootMotion == null) return Vector2.zero;

        return new Vector2(rootMotion.rootMotionScaleX, rootMotion.rootMotionScaleY);
    }
}