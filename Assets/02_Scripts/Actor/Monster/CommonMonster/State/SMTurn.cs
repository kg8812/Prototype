using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMTurn : ICommonMonsterState<CommonMonster2>
    {
        private readonly int _turn = Animator.StringToHash("turn");
        private CommonMonster2 _cM;

        private float _curTime;
        private float _turnTime;

        public void OnEnter(CommonMonster2 monster)
        {
            _cM = monster;
            _turnTime = _cM.turnTime;
            _curTime = 0;
            _cM.animator.SetTrigger(_turn);
        }

        public void Update()
        {
            if (!_cM.IsPause)
            {
                _curTime += Time.deltaTime;
                if (_curTime >= _turnTime)
                {
                    _cM.TurnWithoutDelay();
                    _cM.TryChangeMonsterState(_cM.PreState);
                }
            }
        }

        public void FixedUpdate()
        {
        }

        public void OnExit()
        {
            
        }

        public void OnCancel()
        {
            
        }
    }
}