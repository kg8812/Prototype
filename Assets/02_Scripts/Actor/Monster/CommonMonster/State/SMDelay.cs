using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMDelay: ICommonMonsterState<CommonMonster2>
    {
        private readonly int _forceDelay = Animator.StringToHash("forceDelay");
        private readonly int _exitDelay = Animator.StringToHash("exitDelay");
        private readonly int _forceTurn = Animator.StringToHash("forceTurn");
        
        private CommonMonster2 _cM;
        private float _delayTime;
        private float _curTime;
        
        public void OnEnter(CommonMonster2 monster)
        {
            _cM = monster;
            // _cM.animator.SetTrigger(_forceDelay);
            _delayTime = Random.Range(_cM.MonsterData.delayDuration[0],
                _cM.MonsterData.delayDuration[1]);
            _curTime = 0;
        }

        public void Update()
        {
            if (!_cM.IsPause)
            {
                _curTime += Time.deltaTime;
                if (_curTime >= _delayTime)
                {
                    // if (_cM.ableMove && !_cM.CheckPlayerRl() && _cM.TryChangeMonsterState(MonsterState.Turn))
                    // {
                    //     Debug.Log("delay to turn");
                    //     _cM.PreState = MonsterState.Move;
                    //     _cM.animator.SetTrigger(_forceTurn);
                    // }
                    // else 
                    // move로 전환하지만 move에서 turn 체크 미리 함.
                    if(!_cM.TryChangeMonsterState(MonsterState.Move))
                    {
                        Debug.LogError($"monster: {_cM.monsterId} - delay 패턴에서 빠져나가지 않음.");
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            // _commonMonster.StopCheck();
        }

        public void OnExit()
        {
            
        }

        public void OnCancel()
        {
        }
    }
}