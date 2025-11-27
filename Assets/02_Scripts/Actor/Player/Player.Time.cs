using System.Collections;
using Apis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public partial class Player
{
    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("공격 모션 최소 재생 시간")] [SerializeField]
    private float _attackEscapeTime;

    [LabelText("컨트롤러 버퍼 시간")] [SerializeField]
    private float _bufferTime = 0.5f;

    public PlayerCd CoolDown;

    private Coroutine jumpCoroutine;
    public float BufferTime => _bufferTime;

    public float GroundAtkWeaponEscapeTime(int index)
    {
        if (AttackItemManager.CurrentItem is Weapon weapon)
            return _attackEscapeTime * weapon.atkSpeed * (weapon.GroundCancelTimes[index] / 100) / (1 + AtkSpeed / 100);

        return 0;
    }

    public float AirAtkWeaponEscapeTime(int index)
    {
        if (AttackItemManager.CurrentItem is Weapon weapon)
            return _attackEscapeTime * weapon.atkSpeed *
                (weapon.AirCancelTimes[index] / 100) / (1 + AtkSpeed / 100);

        return 0;
    }

    public void StopJumpCoroutine()
    {
        if (jumpCoroutine == null) return;
        GameManager.instance.StopCoroutineWrapper(jumpCoroutine);
    }

    public Coroutine StartTimer(float time, UnityAction onEnd)
    {
        IEnumerator timer()
        {
            yield return new WaitForSeconds(time);
            onEnd.Invoke();
        }

        return GameManager.instance.StartCoroutineWrapper(timer());
    }

    public void StopTimer(Coroutine timer)
    {
        GameManager.instance.StopCoroutineWrapper(timer);
    }
}