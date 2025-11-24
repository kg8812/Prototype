
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class TransformNode : BossActionNode
    {
        public BossMonster.BossPhase phase;

        bool _isFinished;

        public float duration = 0;
        
        public override void OnStart()
        {
            base.OnStart();
            _isFinished = false;

            OnAlert.AddListener(Invoke);
            boss.OnTransformStart.Invoke();

            if (!Mathf.Approximately(duration, 0))
            {
                GameManager.instance.StartCoroutineWrapper(InvokeInTime(duration, () => _isFinished = true));
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            boss.animator.SetTrigger("ChangeState");
            OnAlert.RemoveAllListeners();
        }
       
        public override void OnSkip()
        {
            base.OnSkip();
            boss.phase = phase;
        }
        public override State OnUpdate()
        {
            if (_isFinished)
            {
                boss.phase = phase;
                boss.OnTransformEnd.Invoke();
                return State.Success;
            }
          
            return State.Running;
        }

        void Invoke(string message)
        {
            if(message == "TransformEnd")
            {
                _isFinished = true;
            }
        }
    }
}