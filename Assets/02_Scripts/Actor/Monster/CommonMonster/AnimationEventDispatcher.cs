using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    [System.Serializable]
    public class UnityAnimationEvent : UnityEvent<string>
    {
    }

    [RequireComponent(typeof(Animator))]
    public class AnimationEventDispatcher : MonoBehaviour
    {
        [HideInInspector] public UnityAnimationEvent OnAnimationStart;
        [HideInInspector] public UnityAnimationEvent OnAnimationComplete;

        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                AnimationClip clip = animator.runtimeAnimatorController.animationClips[i];

                AnimationEvent animationStartEvent = new AnimationEvent();
                animationStartEvent.time = 0;
                animationStartEvent.functionName = "AnimationStartHandler";
                animationStartEvent.stringParameter = clip.name;

                AnimationEvent animationEndEvent = new AnimationEvent();
                animationEndEvent.time = clip.length;
                animationEndEvent.functionName = "AnimationCompleteHandler";
                animationEndEvent.stringParameter = clip.name;

                clip.AddEvent(animationStartEvent);
                clip.AddEvent(animationEndEvent);
            }
        }

        public void AnimationStartHandler(string name)
        {
            // Debug.Log($"{name} animation start.");
            OnAnimationStart?.Invoke(name);
        }

        public void AnimationCompleteHandler(string name)
        {
            // Debug.Log($"{name} animation complete.");
            OnAnimationComplete?.Invoke(name);
        }
    }
}