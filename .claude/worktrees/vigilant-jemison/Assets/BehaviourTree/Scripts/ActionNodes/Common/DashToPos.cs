using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class DashToPos : CommonActionNode
    {
        public string objectName;
        public float speed;
        public float distanceCondition;
        [LabelText("End모션 스킵여부")] public bool isSkip;

        [Tooltip("플레이어가 반대쪽으로 가면 멈추는지 여부")] public bool stopIfOp = true;

        private float distance;
        private bool isFinished;

        private IMovable mover;
        private GameObject pos;

        private Tweener tweener;

        public override void OnStart()
        {
            base.OnStart();
            mover = _actor as IMovable;

            _actor.animator.ResetTrigger("DashEnd");
            OnAlert.RemoveAllListeners();
            pos = GameObject.Find(objectName);

            mover?.ActorMovement?.Stop();

            _actor.animator.SetBool("IsDashEnd", !isSkip);
            _actor.animator.SetTrigger("Dash");
            OnAlert.AddListener(Alert);
            isFinished = false;
        }

        private void Dash()
        {
            _actor.Rb.DOKill();
            if (pos != null)
            {
                var posX = pos.TryGetComponent(out Actor act) ? act.Position.x : pos.transform.position.x;

                mover?.ActorMovement?.SetGravityToZero();

                var trans = _actor.transform;

                var d = posX - trans.position.x;
                _actor.SetDirection(d < 0 ? EActorDirection.Left : EActorDirection.Right);

                tweener = _actor.Rb.DOMoveX(_actor.Position.x + (int)_actor.Direction, 1 / speed);
                blackBoard.tweener = tweener;
                tweener.SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).OnKill(() =>
                {
                    mover?.ActorMovement?.Stop();
                    _actor.animator.SetTrigger("DashEnd");
                    mover?.ActorMovement?.ResetGravity();

                    if (isSkip) isFinished = true;
                }).SetUpdate(UpdateType.Fixed);

                tweener.onUpdate += () =>
                {
                    var direction = new Vector2((int)_actor.Direction, 0);
                    if (Physics2D.Raycast(_actor.Position, direction, 0.75f, LayerMasks.Wall)) tweener.Kill();
                };
            }
        }

        public override void OnStop()
        {
            base.OnStop();

            if (blackBoard.tweener == tweener && tweener.IsActive())
            {
                tweener.Kill();
                blackBoard.tweener = null;
                tweener = null;
                _actor.animator.ResetTrigger("DashEnd");
            }

            OnAlert.RemoveAllListeners();
        }

        public override State OnUpdate()
        {
            var x = pos.TryGetComponent(out Actor act) ? act.Position.x : pos.transform.position.x;

            distance = Mathf.Abs(x - _actor.Position.x);

            if (distance < distanceCondition || distance < 0.05f)
            {
                tweener?.Kill();
                tweener = null;
            }

            if ((_actor.Position.x < x && _actor.Direction == EActorDirection.Left)
                || (_actor.Position.x > x && _actor.Direction == EActorDirection.Right))
            {
                tweener?.Kill();
                tweener = null;
            }

            if (isFinished) return State.Success;

            return State.Running;
        }

        public override void OnSkip()
        {
            base.OnSkip();
            _actor.Rb.DOKill();
            blackBoard.tweener = null;
        }

        private void Alert(string alert)
        {
            if (!isStarted) return;

            if (alert == "DashStart") Dash();
            if (alert == "DashEnd") isFinished = true;
        }
    }
}