using DG.Tweening;
using Sirenix.OdinInspector;
using Spine;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class GoseguJump : BossActionNode
    {
        [LabelText("점프 높이")] public float jumpPower;
        [LabelText("점프 시간")] public float jumpTime;
        [LabelText("점프할 벽")] public WallDir wall;
        public Ease ease;

        public enum WallDir
        {
            Left = 0,Mid,Right
        }
        Tween _tween;

        GoseguBoss gosegu;

        bool isFinished;

        private GameObject[] walls;

        private GameObject _obj;

        private Bone target;
        public override void OnStart()
        {
            base.OnStart();
            _tween = null;

            isFinished = false;
            OnAlert.AddListener(Alert);
            _actor.animator.ResetTrigger("JumpEnd");
            _actor.animator.SetTrigger("Jump");
            gosegu = boss.GetComponent<GoseguBoss>();
            walls = gosegu.walls;
            _obj = walls[(int)wall];
            target = gosegu.skeleton.skeleton.FindBone("target");
        }

        void Jump()
        {
            Vector2 endPos = _obj.transform.position + Vector3.up * jumpPower;
            gosegu.Rb.gravityScale = 0;
            _actor.Rb.DOKill();

            // target.SetLocalPosition(new Vector2(endPos.x * (int)gosegu.direction,endPos.y));
            // target.UpdateWorldTransform();
            // target.UpdateAppliedTransform();
            _tween = _actor.Rb.DOMove(endPos,jumpTime);
            _tween.SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
            _tween.onKill += () =>
            {
                _actor.animator.SetTrigger("JumpEnd");
            };
        }
        public override void OnSkip()
        {
            base.OnSkip();
            _tween?.Kill();
            _tween = null;
        }
       
        public override void OnStop()
        {
            base.OnStop();

            _tween?.Kill();
            _tween = null;
        }

        public override State OnUpdate()
        {
            if (_obj == null)
            {
                return State.Failure;
            }

            if (!isFinished)
            {
                return State.Running;
            }

            return State.Success;
        }

        void Alert(string alert)
        {
            if (alert == "JumpStart")
            {
                Jump();
            }
            if (alert == "JumpEnd")
            {
                isFinished = true;
            }
        }
    }
}