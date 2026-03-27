using UnityEngine;

public interface IAnimator : IMonoBehaviour
{
    // 애니메이터 사용자
    public Animator animator { get; }
    public void IdleOn();
    public void StopAnimation();
    public void ResumeAnimation();
}