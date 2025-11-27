using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class DashNode : CommonActionNode
    {
        public float time;
        public float distance;
        public bool isBackDash;

        [LabelText("End모션 스킵여부")] public bool isSkip;

        private bool isFinished;
        private bool success;

        private Tweener tweener;

        public override void OnStart()
        {
            base.OnStart();
            success = false;
            isFinished = false;
            OnAlert.AddListener(Alert);

            _actor.animator.SetBool("IsDashEnd", !isSkip);
            _actor.animator.ResetTrigger("DashEnd");
            var trigger = isBackDash ? "BackDash" : "Dash";
            _actor.animator.SetTrigger(trigger);
        }

        private void Dash()
        {
            if (_actor is not IMovable mover) return;

            mover.Rb.DOKill();
            mover.Rb.linearVelocity = Vector3.zero;

            tweener = mover.ActorMovement.DashTemp(time, distance, isBackDash).SetAutoKill(true).SetEase(Ease.Linear);
            _actor.ExecuteEvent(EventType.OnDash, new EventParameters(_actor));

            mover.Rb.gravityScale = 0;
            blackBoard.tweener = tweener;
            tweener.onKill += () =>
            {
                success = true;
                _actor.animator.SetTrigger("DashEnd");
                mover.ActorMovement.Stop();
                if (isSkip) isFinished = true;
                mover.ActorMovement.ResetGravity();
            };
            tweener.onUpdate += () =>
            {
                var direction = new Vector2((int)_actor.Direction * (isBackDash ? -1 : 1), 0);
                if (Physics2D.Raycast(_actor.Position, direction, 0.75f, LayerMasks.Wall)) tweener.Kill();
            };
        }

        public override void OnStop()
        {
            base.OnStop();

            if (blackBoard.tweener == tweener)
            {
                tweener.Kill();
                blackBoard.tweener = null;
                tweener = null;
            }

            OnAlert.RemoveAllListeners();
        }

        public override State OnUpdate()
        {
            if (!isFinished) return State.Running;

            return success ? State.Success : State.Failure;
        }

        private void Alert(string alert)
        {
            if (!isStarted) return;

            if (alert == "DashEnd") isFinished = true;

            if (alert == "DashStart") Dash();
        }
    }
}