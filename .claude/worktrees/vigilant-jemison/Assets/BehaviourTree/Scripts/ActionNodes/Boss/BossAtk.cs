using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class BossAtk : BossActionNode
    {
        public int atkNum;

        [Tooltip("마지막 모션 캔슬 여부")] public bool isSkip;
        public bool WaitForEnd = true;

        [LabelText("공격 파생타입")] [InfoBox("0 = 기본, 1 = _1 등등")]
        public int atkType;

        private bool isFinished;

        public override void OnStart()
        {
            base.OnStart();

            boss.animator.SetBool("IsAttackEnd", !isSkip);
            isFinished = false;
            boss.animator.SetInteger("Attack", atkNum);
            boss.animator.SetInteger("AttackType", atkType);

            OnAlert.AddListener(Invoke);

            var index = atkType > 0 ? atkNum * 100 + atkType : atkNum;
            boss.StartAtkPattern(index);
        }

        public override void OnStop()
        {
            base.OnStop();
            OnAlert.RemoveAllListeners();
            boss.EndAttack(atkNum);
        }

        public override State OnUpdate()
        {
            if (isFinished || !WaitForEnd) return State.Success;

            return State.Running;
        }

        public override void OnSkip()
        {
            base.OnSkip();
            OnAlert.RemoveAllListeners();
            boss.CancelAttack();
        }

        private void Invoke(string message)
        {
            if (message == "AttackEnd") isFinished = true;
        }
    }
}