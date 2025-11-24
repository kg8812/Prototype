using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMPatrol : ICommonMonsterState<CommonMonster2>
    {
        private readonly int _isPatrolAnim = Animator.StringToHash("isPatrol");
        private CommonMonster2 _cM;

        private float _duration;
        private float[] _patrolDurationRange;
        private float _timer;
        private float playerDist;
        private EActorDirection goalDiraction;

        public void OnEnter(CommonMonster2 monster)
        {
            _cM = monster;
            _timer = 0;
            _duration = Random.Range(_cM.MonsterData.patrolDuration[0],
                _cM.MonsterData.patrolDuration[1]);
            
            _cM.animator.SetBool(_isPatrolAnim, true);
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
            }else
            {
                _timer += Time.deltaTime;
                if (!_cM.MonsterMove(true))
                {
                    _cM.ForcedPatrolRotation = -(int)_cM.Direction;
                    _cM.TryChangeMonsterState(MonsterState.Idle);
                    return;
                }
                if (_timer >= _duration)
                {
                    _cM.ForcedPatrolRotation = 0;
                    _cM.TryChangeMonsterState(MonsterState.Idle);
                }
            }
        }

        public void FixedUpdate()
        {
            // physics logic for move state
        }

        public void OnExit()
        {
            _cM.animator.SetBool(_isPatrolAnim, false);
        }

        public void OnCancel()
        {
        }
    }
}