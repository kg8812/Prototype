using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Player
{
    #region 피격 기획 변수
    /* 점멸 쉐이더 */
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("점멸 시간")]
    [SerializeField] private float blinkDuration = 2;
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("점멸 빈도")]
    [SerializeField] private float blinkFrequency = 4;
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("점멸 밝기(미적용)")]
    [SerializeField] private float blinkBrightness;

    /* 피격 흔들림 */
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("진동 시간")]
    [SerializeField] private float shakeDuration = 2;
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("진동 빈도")]
    [SerializeField] private float shakeFrequency = 0.03f;
    [Range(0.01f, 1), TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("진동 강도")]
    [SerializeField] private float shakeIntensity = 1;

    /* Damage State 변수*/
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("Stuck 상태 시간")]
    [SerializeField] private float stuckDuration;
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("Stuck 상태 무적 시간")]
    [SerializeField] private float stuckInvincibleDuration;

    /* Knockback State 변수 */
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("Knockback 상태 시간")]
    [SerializeField] private float knockbackDuration;
    [TabGroup("기획쪽 수정 변수들/group1", "피격 관련 스탯")] [LabelText("Knockback end 캔슬 시간")]
    [SerializeField] private float knockbackEndCancelTime;

    public float BlinkDuration => blinkDuration;
    public float BlinkFrequency => blinkFrequency;
    public float BlinkBrightness => blinkBrightness;
    public float ShakeDuration => shakeDuration;
    public float ShakeFrequency => shakeFrequency;
    public float ShakeIntensity => shakeIntensity;
    public float StuckInvincibleDuration => stuckInvincibleDuration;
    public float StuckDuration => stuckDuration;
    public float KnockbackDuration => knockbackDuration;
    public float KnockbackEndCancelTime => knockbackEndCancelTime;
    #endregion

    private Sequence shakeSeq;
    private Sequence blinkSeq;

    public void PlayHitEffect(float shakeTime, float blinkTime)
    {
        Shake(shakeTime);
        Blink(blinkTime);
        effector.Play(PlayerEffector.CommonPlayerEffect.HIT, false, true);
    }

    private void Shake(float duration)
    {
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_HitEffectDistance", shakeFrequency);
        propBlock.SetFloat("_HitEffectDistancePow", shakeIntensity);
        propBlock.SetFloat("_IsHit", 1);
        meshRenderer.SetPropertyBlock(propBlock);

        shakeSeq = DOTween.Sequence()
                    .SetAutoKill(true)
                    .SetUpdate(UpdateType.Fixed)
                    .OnKill(() => shakeSeq = null );
        
        float powerStart = shakeIntensity;
        shakeSeq.OnUpdate(() =>{
            meshRenderer.GetPropertyBlock(propBlock);
            float power = Mathf.Lerp(powerStart, 0.01f, Time.fixedDeltaTime);
            propBlock.SetFloat("_HitEffectDistancePow", power);
            meshRenderer.SetPropertyBlock(propBlock);
            powerStart = power;
        });

        shakeSeq.AppendInterval(duration);

        shakeSeq.OnComplete(() => 
        {
            StopShakeEffect();
        });
    }

    private void Blink(float duration)
    {
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_InvincibleWaveSpeed", blinkFrequency);
        propBlock.SetFloat("_InvincibleSwitch", 1);
        meshRenderer.SetPropertyBlock(propBlock);

        blinkSeq = DOTween.Sequence()
                    .SetAutoKill(true)
                    .SetUpdate(UpdateType.Fixed)
                    .OnKill(() => blinkSeq = null );

        blinkSeq.AppendInterval(duration);

        blinkSeq.OnComplete(() => 
        {
            StopBlinkEffect();
        });
    }

    public void StopHitEffect()
    {
        StopShakeEffect();
        StopBlinkEffect();
    }

    private void StopShakeEffect()
    {
        shakeSeq?.Kill(); 

        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_IsHit", 0);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    private void StopBlinkEffect()
    {
        blinkSeq?.Kill(); 

        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_InvincibleSwitch", 0);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    public void LookAtHitDirection(EventParameters eventParameters)
    {
        KnockBackData curKnockBackData = GetKnockBackData(eventParameters);
        // direction type에 따른 넉백 적용 vector2 계산
        Vector2 knockBackSrc = (curKnockBackData.directionType == KnockBackData.DirectionType.AktObjRelative || eventParameters.master == null)
            ? eventParameters.user.Position
            : eventParameters.master.Position;

        Vector2 currentPos = transform.position;

        float dir = Mathf.Sign(knockBackSrc.x - currentPos.x);

        if((float)Direction != dir) Flip();
    }
}
