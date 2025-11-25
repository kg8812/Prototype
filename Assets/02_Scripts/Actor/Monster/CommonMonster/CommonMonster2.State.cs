using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.CommonMonster2
{
    public enum MonsterState
    {
        Idle,Death
    }

    public partial class CommonMonster2
    {
        private StateMachine<CommonMonster2> _mState;
        private Dictionary<MonsterState, IState<CommonMonster2>> _mStates;

        public MonsterState PreState { get; set; }

        [ReadOnly] public MonsterState curState;

        public MonsterState CurState
        {
            get => curState;
            set => curState = value;
        }

        private bool
            // _isActivated,
            // _isRecognized,
            // _isDead,     -> Actor::IsDead
            // _isGroggyed, -> Monster::IsGroggyed
            // _isJumped,   -> Actor::isJump
            _isee;

        // For Util
        public string MStateString => CurState.ToString();

        private void AwakeState()
        {
            _mStates = new Dictionary<MonsterState, IState<CommonMonster2>>();

            CurState = MonsterState.Idle;
            _mStates.Add(MonsterState.Idle, new SMIdle());
            _mStates.Add(MonsterState.Death, new SMDeath());
            
            _mState = new StateMachine<CommonMonster2>(this, _mStates[MonsterState.Idle]);
        }
        
        protected override void Update()
        {
            base.Update();
            _playerTrans = GameManager.instance.PlayerTrans;
            if (ReferenceEquals(_playerTrans, null)) return;
            _mState?.Update();
            
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            _mState?.FixedUpdate();
        }

        public bool CheckChangeMonsterState(MonsterState toState)
        {
            if (!_isActivated) return false;
            // if (!allowRestart && CurState == toState) return false;

            // 일반몬스터는 죽음 상태 -> 아무 상태가 불가능 (초기화 제외)
            if (CurState == MonsterState.Death) return false;

            if (toState != MonsterState.Death &&
                SubBuffCount(SubBuffType.Debuff_Stun) > 0)
            {
                return false;
            }

            return true;
        }


        public bool TryChangeMonsterState(MonsterState toState)
        {
            if (!CheckChangeMonsterState(toState)) return false;
            return ChangeMonsterState(toState);
        }

        public bool ChangeMonsterState(MonsterState toState)
        {
            if (CurState == toState) return false;

            if (_mStates.TryGetValue(toState, out IState<CommonMonster2> newState))
            {
                PreState = CurState;
                CurState = toState;
                _mState.SetState(newState);
                // ExecuteEvent(BuffEventType.OnStateChanged, new ActorInfo(this){monsterState = CurState});
                return true;
            }

            return false;
        }
        
        
        
        // overring
        public override void IdleOn()
        {
            base.IdleOn();
            TryChangeMonsterState(MonsterState.Idle);
        }
    }
}