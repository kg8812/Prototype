using System.Collections;
using Apis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public partial class Player
{
    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] 
    [LabelText("컨트롤러 버퍼 시간")] [SerializeField]
    private float _bufferTime = 0.5f;

    public PlayerCd CoolDown;

    private Coroutine jumpCoroutine;
    public float BufferTime => _bufferTime;
    
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