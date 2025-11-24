using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMAttack : ICommonMonsterState<CommonMonster2>
    {
        private CommonMonster2 _cM;
        private readonly int _pGInd = Animator.StringToHash("pGInd");
        private readonly int _atkAnim = Animator.StringToHash("attack");
        
        private readonly int _forceDelay = Animator.StringToHash("forceDelay");

        public void OnEnter(CommonMonster2 monster)
        {
            _cM = monster;
            _cM.ExecuteEvent(EventType.OnAttack, new EventParameters(_cM));
            _cM.TurnToPlayerWithoutDelay();
            _cM.animator.SetInteger(_pGInd, _cM.PgController.curPGId);
            _cM.animator.SetTrigger(_atkAnim);
        }

        public void Update()
        {
            // attack state logic
        }

        public void FixedUpdate()
        {
        }
        
        public void OnCancel()
        {
        }

        public void OnExit()
        {
            _cM.PgController.ForceCancel();
            _cM.animator.SetInteger(_pGInd, -1);
        }
    }
}