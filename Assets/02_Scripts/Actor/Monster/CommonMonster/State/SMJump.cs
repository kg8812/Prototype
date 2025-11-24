using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMJump : ICommonMonsterState<CommonMonster2>
    {
        private int _jumpEnd = Animator.StringToHash("jumpEnd");
        private CommonMonster2 _cM;

        public void OnEnter(CommonMonster2 monster)
        {
            _cM = monster;
            _cM.ExecuteEvent(EventType.OnJump, new EventParameters(_cM));
            _cM.animator.ResetTrigger(_jumpEnd);
        }

        public void Update()
        {
            // jump state logic
        }

        public void FixedUpdate()
        {
            if (_cM.MoveComponent.isJump && _cM.ActorMovement.IsStick)
            {
                _cM.MoveComponent.isJump = false;
                _cM.animator.SetTrigger(_jumpEnd);
            }
        }

        public void OnExit()
        {
            // exit jump state logic
        }

        public void OnCancel()
        {
        }
    }
}
