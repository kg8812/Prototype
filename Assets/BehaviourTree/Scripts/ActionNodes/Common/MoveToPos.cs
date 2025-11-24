using DG.Tweening;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class MoveToPos : CommonActionNode
    {
        public string objectName;
        public float time;
        
        bool success;

        Tweener tweener;

        public Ease ease;
        public override void OnStart()
        {
            base.OnStart();
            Transform pos = GameObject.Find(objectName).transform;
            success = false;
            _actor.Rb.DOKill();
            if (pos != null)
            {
                Vector3 p = pos.position;
                tweener = _actor.transform.DOMove(new Vector3(p.x, p.y, 0), time);
                blackBoard.tweener = tweener;
                tweener.SetEase(ease);
                tweener.OnComplete(() => success = true).
                    OnKill(() =>
                    {
                        if (tweener.IsActive())
                        {
                            state = State.Failure;
                            success = false;
                        }
                    }).SetUpdate(UpdateType.Fixed);
            }
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
        }

        public override void OnSkip()
        {
            base.OnSkip();
            _actor.Rb.DOKill();
            blackBoard.tweener = null;
            tweener = null;
        }
        public override State OnUpdate()
        {
            if (tweener != null && tweener.IsActive())
            {
                return State.Running;
            }

            return success ? State.Success : State.Failure;
        }
    }
}