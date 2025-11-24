using Apis;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class GoseguClimb : BossActionNode
    {

        public enum Direction
        {
            Left = 0,Mid=1,Right=2,Closest=3,
        }
        [LabelText("점프할 위치")] public Direction Dir;
        private EActorDirection dir;
        [LabelText("포물선 높이")]public float jumpHeight;
        [LabelText("이동 높이")] public float climbHeight;
        [LabelText("이동 시간")] public float moveTime;
        [LabelText("이동 Ease")] public Ease ease;
        GoseguBoss gosegu;

        Vector2 endPos;

        bool isFinished;
        public override void OnStart()
        {
            base.OnStart();
            
            gosegu = _actor.GetComponent<GoseguBoss>();
            OnAlert.RemoveListener(Invoke);
            OnAlert.AddListener(Invoke);

            switch (Dir)
            {
                case Direction.Left:
                case Direction.Right:
                case Direction.Mid:
                    endPos = gosegu.walls[(int)Dir].transform.position + Vector3.up * climbHeight;
                    break;
                case Direction.Closest:
                    float min = float.MaxValue;
                    gosegu.walls.ForEach(x =>
                    {
                        float distance = Vector2.Distance(gosegu.Position, x.transform.position);
                        if (min > distance)
                        {
                            min = distance;
                            endPos = x.transform.position + Vector3.up * climbHeight;
                        }
                    });
                    break;
            }

            dir = gosegu.transform.position.x < endPos.x ? EActorDirection.Left : EActorDirection.Right; 
            boss.SetDirection(dir);
            isFinished = false; 
            
            boss.animator.SetTrigger("Climb");
        }

        DG.Tweening.Sequence seq;
        void Climb()
        {
            gosegu.Rb.DOKill();
            gosegu.Rb.gravityScale = 0;
            gosegu.animator.ResetTrigger("Land");
            seq = gosegu.Rb.DOJump(endPos, jumpHeight,1,moveTime).SetEase(ease).SetAutoKill(true);
            seq.SetUpdate(UpdateType.Fixed);
            bool isChanged = false;
            seq.onUpdate += () =>
            {
                if (isChanged) return;

                Debug.DrawRay(gosegu.Position, Vector2.left * (int)gosegu.Direction * 0.5f,Color.red);
                if (Physics2D.Raycast(gosegu.Position, Vector2.left * (int)gosegu.Direction,0.5f,LayerMasks.BackGround))
                {
                    isChanged = true;
                    gosegu.animator.SetTrigger("Land");
                }
            };
            
            seq.onKill += () =>
            {
                isFinished = true;
            };
        }
    
        public override State OnUpdate()
        {
            if(gosegu == null)
            {
                return State.Failure;
            }
            if (!isFinished) return State.Running;

            return State.Success;
        }
        
        void Invoke(string str)
        {
            if(str == "Climb")
            {
                Climb();
            }
        }
    }
}