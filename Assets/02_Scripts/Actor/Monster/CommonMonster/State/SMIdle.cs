using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMIdle: ICommonMonsterState<CommonMonster2>
    {
        private readonly int _pgIndAnim = Animator.StringToHash("pGInd");
        private readonly int _isPatrolAnim = Animator.StringToHash("isPatrol");
        private readonly int _idleAnim = Animator.StringToHash("idle");
        
        private CommonMonster2 _cM;
        private bool _isPatrol;
        private float _duration;
        
        private float _timer;
        private EActorDirection _goalDir;
        public void OnEnter(CommonMonster2 m)
        {
            _cM = m;
            // idle 들어오기전 체크
            if (_cM.IsRecognized)
            {
                if (_cM.ableMove && !_cM.CheckPlayerRl()
                    && _cM.TryChangeMonsterState(MonsterState.Turn))
                {
                    return;
                }

                if (_cM.TryChangeMonsterState(MonsterState.Move))
                {
                    return;
                }
                
            }
            
            _isPatrol = _cM.MonsterData.isPatrol;
            if (_isPatrol)
            {
                _duration = Random.Range(_cM.MonsterData.patrolDuration[0], _cM.MonsterData.patrolDuration[1]);
            }
            _timer = 0;
            
            _cM.animator.SetInteger(_pgIndAnim, -1);
            _cM.animator.SetBool(_isPatrolAnim, false);
            _cM.animator.SetTrigger(_idleAnim);
        }

        public void Update()
        {
            _cM.CheckRecognition();
            if (_cM.IsRecognized)
            {
                _cM.TryChangeMonsterState(MonsterState.Move);
            }else if (!_cM.IsActivated)
            {
                _cM.TryChangeMonsterState(MonsterState.None);
            }else if (_isPatrol && !_cM.IsPause)
            {
                _timer += Time.deltaTime;
                if (_timer >= _duration)
                {
                    _goalDir = (EActorDirection)(_cM.ForcedPatrolRotation != 0
                        ? _cM.ForcedPatrolRotation
                        : Random.Range(0, 2) * 2 - 1);
                    if (_goalDir != _cM.Direction)
                    {
                        _cM.TryChangeMonsterState(MonsterState.Turn);
                        _cM.PreState = MonsterState.Patrol;
                    }
                    else
                    {
                        _cM.TryChangeMonsterState(MonsterState.Patrol);
                    }
                    
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