using Apis;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class JumpNode : CommonActionNode
    {
        [LabelText("점프 높이")] public float jumpPower;
        [LabelText("점프 시간")] public float jumpTime;
        [LabelText("점프 거리")] public float distance;
        [LabelText("앞뒤 여부")] public bool isBackDash;

        bool success;

        private Tween xTween;
        private Tween yTween;

        bool isFinished;
        
        [LabelText("End모션 스킵여부")] public bool isSkip = false;

        private IMovable mover;
        public override void OnStart()
        {
            base.OnStart();
            success = false;
            isFinished = false;
            OnAlert.RemoveAllListeners();
            OnAlert.AddListener(Alert);
            string trigger = isBackDash ? "BackDash" : "Dash";
            _actor.animator.SetTrigger(trigger);
            _actor.animator.SetBool("IsDashEnd",!isSkip);
            mover = _actor as IMovable;
        }

        void Jump()
        {
            _actor.Rb.DOKill();
            _actor.Rb.velocity = Vector2.zero;
            var tweens = mover.ActorMovement.DoJumpTween(jumpTime, jumpPower, distance, isBackDash);

            xTween = tweens.Item1;
            yTween = tweens.Item2;
            
            xTween.onUpdate += () =>
            {
                var direction = new Vector2((int)_actor.Direction * (isBackDash ? -1 : 1), 0);
                Debug.DrawRay(_actor.Position, new Vector2(direction.x * 1,direction.y), Color.red);
                if (Physics2D.Raycast(_actor.Position, direction, 1, LayerMasks.Wall))
                {
                    xTween.Kill();
                }
            };
            yTween.SetAutoKill(true);
            yTween.onKill += () =>
            {
                _actor.Rb.velocity = Vector2.zero;
                success = true;
                _actor.animator.SetTrigger("DashEnd");
                if (isSkip) isFinished = true;
            };
        }
        public override void OnSkip()
        {
            base.OnSkip();
            xTween?.Kill();
            yTween?.Kill();
            xTween = null;
            yTween = null;
            _actor.Rb.DOKill();
        }

        public override void OnStop()
        {
            base.OnStop();

            xTween?.Kill();
            yTween?.Kill();
            xTween = null;
            yTween = null;
            OnAlert.RemoveAllListeners();
        }

        public override State OnUpdate()
        {
            if (!isFinished)
            {
                return State.Running;
            }

            return success ? State.Success : State.Failure;
        }

        void Alert(string alert)
        {
            if (alert == "DashEnd")
            {
                isFinished = true;
            }
            if (alert == "DashStart")
            {
                Jump();
            }
        }
    }
}