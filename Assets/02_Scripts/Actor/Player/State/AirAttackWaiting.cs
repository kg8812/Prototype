using Apis;
using UnityEngine;

namespace PlayerState {
    public class AirAttackWaiting : BaseAttackWaiting, IInterruptable
    {
        // TODO: Attack Combo 초기값 이슈
        public float InterruptTime { get => _player.AnimController.GetCurrentClipLength(0); set{} }
        public EPlayerState[] InteruptableStates { get => new[] { EPlayerState.AirMove, EPlayerState.Jump, EPlayerState.Dash, EPlayerState.Attack }; set{} }
        private IAttackItem _currentItem;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            
            _player.ResetGravity();

            _currentItem = AttackItemManager.CurrentItem;
            // Debug.Log(_player.weaponAtkInfo.atkCombo + " " + _player.animator.GetInteger("MaxGroundAtk"));
            // Debug.Log(_currentItem.AtkSlotIndex);

            //착지 시
            _player.StateEvent.AddEvent(EventType.OnLanding, (e) =>{
                Reset();
            });

            _player.StateEvent.AddEvent(EventType.OnEventState, (e)=>{ 
                // EventState로 공격 상태 벗어났을 때
                if(_player.CurrentState != EPlayerState.Attack && _player.CurrentState != EPlayerState.Stop){ 
                    Reset();
                }
            });

            _player.StateEvent.AddEvent(EventType.OnAttackStateEnter, CdOn );

            _player.StateEvent.AddEvent(EventType.OnIdleMotion, AirAttacked);

            _player.SetDropMaxVel(_player.AttackDropMaxVel);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _player.MoveComponent.ForceActorMovement.Drop(_player.DropResistFactor, _player.AttackDropMaxVel);
        }

        private void AirAttacked(EventParameters e) => _player.CoolDown.SetAirAttackCd(_currentItem.AtkSlotIndex, true);
        
        private void CdOn(EventParameters e)
        {
            // 공중 공격 상태에서 무기 변경 시 기존 무기는 착지 전까지 쿨타임
            if(AttackItemManager.CurrentItem.AtkSlotIndex != _currentItem.AtkSlotIndex)
            {
                _player.CoolDown.SetAirAttackCd(_currentItem.AtkSlotIndex, true);
            }
        }

        private void Reset()
        {
            _player.weaponAtkInfo.atkCombo = 0; 
            _player.OnFinalAttack = false;
            _player.CoolDown.StopCd(EPlayerCd.AirAttack);
            _player.CoolDown.ResetAirAttackCd();
            _player.StateEvent.RemoveEvent(EventType.OnIdleMotion, AirAttacked);
            _player.StateEvent.RemoveEvent(EventType.OnAttackStateEnter, CdOn);
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.ResetDropResistFactor();
        }
    }
}
