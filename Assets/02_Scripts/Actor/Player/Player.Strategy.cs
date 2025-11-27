using System;
using Apis;
using DG.Tweening;
using UnityEngine;

public partial class Player
{
    #region 공격 전략

    public interface IPlayerAttack
    {
        public float GroundAttackEscapeTime(int index);
        public float AirAttackEscapeTime(int index);
        void Attack();
        bool CheckAttackable(int index);

        public void Attack(int combo);
    }

    public class PlayerWeaponAttack : IPlayerAttack
    {
        private readonly Player player;

        public PlayerWeaponAttack(Player player)
        {
            this.player = player;
        }

        float IPlayerAttack.GroundAttackEscapeTime(int index)
        {
            return player.GroundAtkWeaponEscapeTime(index);
        }

        float IPlayerAttack.AirAttackEscapeTime(int index)
        {
            return player.AirAtkWeaponEscapeTime(index);
        }

        public void Attack()
        {
            if (AttackItemManager.CurrentItem is Weapon weapon)
            {
                player.animator.SetInteger("AttackType", weapon.AtkMotionType);
                EventParameters parameters = new(player);
                player.ExecuteEvent(EventType.OnAttack, parameters);
                player.animator.SetTrigger("Attack");
            }
        }

        public bool CheckAttackable(int index)
        {
            var item = AttackItemManager.GetItem(index);
            return item != null && item.TryAttack();
        }

        public void Attack(int combo)
        {
            if (AttackItemManager.CurrentItem is Weapon weapon)
                weapon.Attack(combo);
            else
                Debug.LogError("무기가 할당되지 않음");
        }
    }

    #endregion

    #region 대쉬 방식 인터페이스

    public interface IPlayerDash
    {
        public Tween Dash(); // 대쉬 실행 함수, 대쉬 전 필요한 작업 있으면 여기서 처리
        public void DashEnd(); // 대쉬 종료시킬 함수, *특별한 상황 아니면 player.IdleOn()으로 설정*
        public void OnEnd(); // 대쉬 종료 후 실행 함수
        public float DashTime(); // 대쉬 지속 시간
        public int MotionType(); // 애니메이터 모션 변수, 0이 기본 모션
    }

    public class BasicDash : IPlayerDash
    {
        private readonly float _DashTime;

        private Guid _guid;
        private readonly int _MotionType = 0;
        private readonly Player _player;

        public BasicDash(Player player)
        {
            _player = player;
            _DashTime = _player.DashTime;
        }

        public Tween Dash()
        {
            _player.DashLandingOff();
            // _player.Hide();
            _player.HitCollider.enabled = false;

            return _player.ActorMovement.DashTemp(_player.DashTime, _player.DashSpeed * _player.DashTime, false,
                _player.DodgeSpeedGraph);
        }

        public void DashEnd()
        {
            // _player.SetState(EPlayerState.DashLanding);
        }

        public void OnEnd()
        {
            // _player.Appear();
            // _player.effector.Stop(effectDash);
            _player.HitCollider.enabled = true;
        }

        public float DashTime()
        {
            return _DashTime;
        }

        public int MotionType()
        {
            return _MotionType;
        }
    }

    #endregion
}