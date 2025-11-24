using System;
using System.Collections.Generic;
using Apis;
using Apis.SkillTrees;
using Apis;
using Default;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
        readonly Player player;

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
            IAttackItem item = AttackItemManager.GetItem(index);
            return item != null && item.TryAttack();
        }

        public void Attack(int combo)
        {
            if (AttackItemManager.CurrentItem is Weapon weapon)
            {
                weapon.Attack(combo);
            }
            else
            {
                Debug.LogError("무기가 할당되지 않음");
            }
           
        }
    }

    public class ViichanBeastAttack : IPlayerAttack
    {
        private Player player;
        private ViichanPassiveSkill passive;

        public ViichanBeastAttack(Player player, ViichanPassiveSkill passive)
        {
            this.player = player;
            this.passive = passive;
        }

        public float GroundAttackEscapeTime(int index)
        {
            return passive.atkTime;
        }

        public float AirAttackEscapeTime(int index)
        {
            return passive.atkTime;
        }

        public void Attack()
        {
            EventParameters parameters = new(player);
            player.attackColliders.ForEach(x => { x.Init(player, new FixedAmount(passive.Atk)); });
            player.ExecuteEvent(EventType.OnAttack, parameters);
            player.animator.SetTrigger("Attack");
        }

        public bool CheckAttackable(int index)
        {
            return true;
        }
        public void Attack(int combo)
        {
            passive?.OnBeastAtk?.Invoke(combo);
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
        Player _player;
        private float _DashTime;
        private int _MotionType = 0;

        private ParticleDestroyer effectTrail;
        private ParticleDestroyer effectGlow;

        public BasicDash(Player player)
        {
            _player = player;
            _DashTime = _player.DashTime;
        }

        private Guid _guid;
        public Tween Dash()
        {
            _player.DashLandingOff();
            // _player.Hide();
            _player.HitCollider.enabled = false;

            // _player.Effector.
            effectTrail = _player.Effector.Play(PlayerEffector.CommonPlayerEffect.ONDASH,false,false);
            effectGlow = _player.Effector.Play(PlayerEffector.CommonPlayerEffect.STARTDASH,false,false);

            effectTrail.transform.localScale = new Vector3((int)_player.Direction, 1, 1);
            effectGlow.transform.localScale = new Vector3((int)_player.Direction, 1, 1);

            return _player.ActorMovement.DashTemp(_player.DashTime, _player.DashSpeed * _player.DashTime, false, _player.DodgeSpeedGraph);
        }

        public void DashEnd()
        {
            // _player.SetState(EPlayerState.DashLanding);
        }

        public void OnEnd()
        {
            _player.Effector.Play(PlayerEffector.CommonPlayerEffect.ENDDASH,false,false);
            // _player.Appear();
            // _player.effector.Stop(effectDash);
            _player.effector.Stop(effectGlow);
            _player.HitCollider.enabled = true;
        }

        public float DashTime() => _DashTime;
        public int MotionType() => _MotionType;
    }

    public class PokdoDashJump : IPlayerDash
    {
        [Serializable]
        public struct PokdoDashInfo
        {
            [LabelText("도약 거리")] public float distance;
            [LabelText("도약 높이")] public float height;
            [LabelText("도약 시간")] public float jumpTime;
            [LabelText("데미지 계수")] public float dmg;
            [LabelText("그로기 수치")] public int groggy;
        }

        Tween tween;

        private List<Tween> allTweens;
        private Player player;
        private PokdoDashInfo dashInfo;
        private JururuActiveSkill skill;
        
        public PokdoDashJump(Player player, JururuActiveSkill skill, PokdoDashInfo dashInfo)
        {
            allTweens = new();
            this.player = player;
            this.dashInfo = dashInfo;
            this.skill = skill;
        }

        protected void GroundSkill()
        {
            AttackObject obj = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.Effect,
                "LandEffect", player.transform.position);
            obj.Init(player, new FixedAmount(skill.Atk * dashInfo.dmg / 100f), 1);
            obj.Init(dashInfo.groggy);
        }

        public Tween Dash()
        {
            player.HitCollider.enabled = false;

            skill.curPokdoStand.SetState("Dash");
            player.Rb.DOKill();
            (Tween x, Tween y) tweens =
                player.ActorMovement.DoJumpTween(dashInfo.jumpTime, dashInfo.height, dashInfo.distance, false);
            tweens.x.KillWhenBoxCast(player, 0.4f, Vector2.right * (int)player.Direction, new Vector2(0.1f, 0.5f),
                LayerMasks.MapAndPlatform | LayerMasks.Wall | LayerMasks.Enemy).SetEase(Ease.Linear);

            tween = tweens.y;

            allTweens.Add(tweens.x);
            allTweens.Add(tweens.y);

            return tween;
        }

        public void DashEnd()
        {
            allTweens.ForEach(x => x?.Kill());
            allTweens.Clear();
        }

        public void OnEnd()
        {
            GroundSkill();
            skill.curPokdoStand?.SetState("Idle");
            player.HitCollider.enabled = true;
        }

        public float DashTime()
        {
            return dashInfo.jumpTime;
        }

        public int MotionType()
        {
            return 0;
        }
    }

    public class BeastDash : IPlayerDash
    {
        Player _player;
        private float _DashTime;
        
        public BeastDash(Player player)
        {
            _player = player;
            _DashTime = _player.DashTime;
        }

        private ParticleDestroyer effectStart;
        private ParticleDestroyer effectDash;
        
        private Guid _guid;
        public Tween Dash()
        {
            _player.DashLandingOff();
            _player.Hide();
            _player.HitCollider.enabled = false;

            effectStart = _player.Effector.Play(PlayerEffector.CommonPlayerEffect.ONDASH,false,false);
            effectDash = _player.Effector.Play(PlayerEffector.CommonPlayerEffect.STARTDASH,false,false);

            effectStart.transform.localScale = new Vector3((int)_player.Direction, 1, 1);
            effectDash.transform.localScale = new Vector3((int)_player.Direction, 1, 1);
            return _player.ActorMovement.DashTemp(_player.DashTime, _player.DashSpeed, false);
        }

        public void DashEnd()
        {
            // _player.SetState(EPlayerState.DashLanding);
        }

        public void OnEnd()
        {
            _player.Effector.Play(PlayerEffector.CommonPlayerEffect.ENDDASH,false,false);
            _player.Appear();
            _player.Effector.Stop(effectStart);
            _player.Effector.Stop(effectDash);
            _player.HitCollider.enabled = true;
        }


        public float DashTime() => _DashTime;
        public int MotionType() => 1;
    }

    public class ViichanShieldDash : IPlayerDash
    {
        private ViichanActiveSkill skill;
        private ViichanTree2B tree;
        private Player player;

        private Tween dash;

        private AttackObject col;
        
        public ViichanShieldDash(Player player, ViichanActiveSkill skill, ViichanTree2B tree)
        {
            this.skill = skill;
            this.tree = tree;
            this.player = player;
        }

        public Tween Dash()
        {
            dash = player.ActorMovement.DashInSpeed(tree.speed, tree.distance, false).SetEase(tree.ease).SetAutoKill(true);
            col = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                Define.CommonObjects.PlayerAtkCollider, player.Position);
            col.Init(player,new FixedAmount(skill.Atk * tree.atkInfo.dmg / 100f));
            col.Init(tree.atkInfo);
            col.transform.SetParent(player.transform);
            col.AddAtkEventOnce(param =>
            {
                skill.Cancel();
                dash?.Kill();
            });
            col.transform.localScale = tree.size;
            dash.onKill += () =>
            {
                col?.Destroy();
                col = null;
            };
            player.HitCollider.enabled = false;
            player.DashLandingOff();

            return dash;
        }

        public void DashEnd()
        {
        }

        public void OnEnd()
        {
            player.HitCollider.enabled = true;
        }

        public float DashTime()
        {
            return tree.distance / tree.speed;
        }

        public int MotionType()
        {
            return 0;
        }
    }

    #endregion
}