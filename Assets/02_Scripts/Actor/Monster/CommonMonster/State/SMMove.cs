using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMMove : ICommonMonsterState<CommonMonster2>
    {
        private readonly int _move = Animator.StringToHash("move");
        private CommonMonster2 _cM;
        private float _pDist;

        public void OnEnter(CommonMonster2 m)
        {
            _cM = m;
            if (_cM.ableMove && !_cM.CheckPlayerRl() && _cM.TryChangeMonsterState(MonsterState.Turn))
            {
                _cM.PreState = MonsterState.Move;
                return;
            }

            // Debug.LogError("move");
            _cM.PgController.EndPlayingPG();
            Update();
            if (_cM.CurState != MonsterState.Move) return;
            _cM.animator.SetTrigger(_move);
            
        }

        public void Update()
        {
            _pDist = _cM.CheckRecognition();
            if (!_cM.IsActivated)
            {
                _cM.TryChangeMonsterState(MonsterState.None);
            }
            else if (!_cM.IsRecognized)
            {
                _cM.TryChangeMonsterState(MonsterState.Idle);
            }
            else
            {
                if (_cM.ableMove && !_cM.CheckPlayerRl() &&
                    _cM.TryChangeMonsterState(MonsterState.Turn))
                {
                    _cM.PreState = MonsterState.Move;
                }
                else if (_cM.ableAttack)
                {
                    int availablePg = _cM.PgController.SelectRandomPg(_pDist);
                    if (availablePg != -1)
                    {
                        if (_cM.CheckChangeMonsterState(MonsterState.Attack) &&
                            _cM.PgController.PlayPatternGroup(availablePg))
                        {
                            _cM.ChangeMonsterState(MonsterState.Attack);
                        }
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            _cM.MonsterMove(false);
        }

        public void OnExit()
        {
            if (_cM.Rb.velocity.x != 0)
            {
                if (_cM.ActorMovement.dirVec.x == 0) return;
                _cM.Rb.velocity -= _cM.ActorMovement.dirVec *
                                   (_cM.Rb.velocity.x / _cM.ActorMovement.dirVec.x);
            }
        }

        public void OnCancel()
        {
        }
    }
}