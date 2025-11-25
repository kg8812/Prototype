using System.Collections.Generic;
using Apis.BehaviourTreeTool;
using Command;
using DG.Tweening;
using Default;
using PlayerState;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public interface ISkillActive // 스킬 사용 인터페이스
    {
        public void Activate(ActiveSkill skill); // 사용 시작
        public void DeActivate(ActiveSkill skill); // 사용 해제
        public bool durationUse { get; } // 지속시간 사용여부
        public bool CheckUsable { get; } // 사용 가능여부

        public float CalculateDmg(float dmg);
        public float CalculateGroggy(float groggy);
        public void OnCancel(); // 캔슬 시 
        public void OnUnEquip(ActiveSkill skill);
    }

    public class InstantSkill : ISkillActive // 즉발형
    {
        private ActiveSkill _skill;

        public InstantSkill(ActiveSkill skill)
        {
            _skill = skill;
        }
        public void Activate(ActiveSkill skill)
        {
            skill.Active();
            _skill = skill;
        }

        public void DeActivate(ActiveSkill skill)
        {
        }

        public bool durationUse => true;

        public bool CheckUsable => CheckPlayer();
        
        bool CheckPlayer()
        {
            Player Player = _skill.user as Player;
            bool playerCheck = Player == null || ((_skill.usableWhenAttack || !Player.OnAttack) && !Player.IsSkill && !Player.IsClimb);
            return !_skill.usePlayerCheck || playerCheck;
        }
        public float CalculateDmg(float dmg)
        {
            return dmg;
        }

        public float CalculateGroggy(float groggy)
        {
            return groggy;
        }

        public void OnCancel()
        {
        }

        public void OnUnEquip(ActiveSkill skill)
        {
        }
    }

    public class ChargeSkill : ISkillActive // 차지형
    {
        private readonly List<(float time,UnityAction action)> chargeInfos;

        private Sequence seq;
        private bool isCancel;
        private ActiveSkill _skill;
        private float BaseChargeDmg => 0;
        private static List<ActorCommand> _endSkillCommand;
        private static List<ActorCommand> EndSkillCommand => _endSkillCommand ??= new()
        {
            ResourceUtil.Load<ActorCommand>("EndWeaponSkill")
        };
        private static List<ActorCommand> _activeSkillEndCommand;
        private static List<ActorCommand> activeSkillEndCommand => _activeSkillEndCommand ??= new()
        {
            ResourceUtil.Load<ActorCommand>("EndActiveSkill")
        };
        private List<ActorCommand> original;

        private Player player => GameManager.instance.Player;
        
        public ChargeSkill(List<(float,UnityAction)> chargeInfos,ActiveSkill skill)
        {
            this.chargeInfos = chargeInfos;
            _skill = skill;
        }
        public void Activate(ActiveSkill skill)
        {
            isCancel = false;
            skill.CurChargeTime = 0;
            skill.Icon.ChangeType(new UI_AtkItemIcon.ChargingUpdate(skill.Icon));
            skill.StartCharge();
            ActorController controller = GameManager.PlayerController as ActorController;
            if (player != null && controller != null)
            {
                if (skill.EnterSkillState)
                {
                    player.animator.SetInteger("ChargingType",skill.chargeType);
                    player.animator.ResetTrigger("ChargeEnd");
                    player.animator.SetTrigger("Charge");
                }
                
                if (skill is PlayerActiveSkill)
                {
                    original = controller.Executors[Define.GameKey.ActiveSkill].keyUpCommand.Commands;
                    controller.Executors[Define.GameKey.ActiveSkill].keyUpCommand.Commands = activeSkillEndCommand;
                }
                else if (skill != null)
                {
                    int index = skill.Item.AtkSlotIndex;
                    EndSkillCommand.ForEach(x =>
                    {
                        if (x is EndWeaponSkill es)
                        {
                            es.index = skill.Item.AtkSlotIndex;
                        }
                    });
                    
                    original = controller.Executors[Define.GameKey.Attack].keyUpCommand.Commands;
                    controller.Executors[Define.GameKey.Attack].keyUpCommand.Commands = EndSkillCommand;
                }
                
                
            }
            
            skill.eventUser?.EventManager.AddEvent(EventType.OnDash,Kill);
            skill.eventUser?.EventManager.AddEvent(EventType.OnHitReaction,Kill);
            int stage = 0;

            seq?.Kill();
            seq = DOTween.Sequence().SetAutoKill(true);

            if (skill.EndChargeAutomatic)
            {
                seq.AppendInterval(skill.ChargeTime);
                seq.AppendCallback(() =>
                {
                    if (chargeInfos.Count > 0 && stage < chargeInfos.Count)
                    {
                        chargeInfos[^1].action.Invoke();
                        stage = chargeInfos.Count;
                        skill.eventUser?.EventManager.ExecuteEvent(EventType.OnChargeEnd,
                            new EventParameters(skill.eventUser));
                    }
                });
            }
            else
            {
                seq.AppendInterval(100);
            }

            seq.onUpdate += () =>
            {
                skill.CurChargeTime += Time.deltaTime;
                skill.WhenCharging.Invoke();
                if (stage < chargeInfos.Count && skill.CurChargeTime >= chargeInfos[stage].time)
                {
                    chargeInfos[stage].action.Invoke();
                    stage++;
                }
            };

            seq.onKill += () =>
            {
                skill.eventUser?.EventManager.RemoveEvent(EventType.OnDash,Kill);
                skill.eventUser?.EventManager.RemoveEvent(EventType.OnHitReaction,Kill);
                if (skill is PlayerActiveSkill)
                {
                    controller.Executors[Define.GameKey.ActiveSkill].keyUpCommand.Commands = original;
                }
                else if (skill != null)
                {
                    int index = skill.Item.AtkSlotIndex;
                    
                    controller.Executors[Define.GameKey.Attack].keyUpCommand.Commands = original;
                }
                

                skill.CDActive.SetIconCdType(skill.Icon);
                if (!isCancel)
                {
                    skill.Active();
                }
                skill.animator?.animator.SetTrigger("ChargeEnd");
                skill.CurChargeTime = 0;
                skill.isCharging = false;
                skill.OnChargeEnd?.Invoke();
            };
            
            void Kill(EventParameters x)
            {
                isCancel = true;
                seq?.Kill();
                skill.CurCd = 0;
                skill.Cancel();
                skill.eventUser.EventManager.ExecuteEvent(EventType.OnChargeCancel,new (skill.eventUser));
            }
            
        }

        public void DeActivate(ActiveSkill skill)
        {
            seq?.Kill();
        }

        public bool durationUse => true;

        public bool CheckUsable => CheckPlayer() && !seq.IsActive();

        bool CheckPlayer()
        {
            Player Player = _skill.user as Player;
            bool playerCheck = Player == null || ((_skill.usableWhenAttack || !Player.OnAttack) && !Player.IsSkill && !Player.IsClimb);
            return !_skill.usePlayerCheck || playerCheck;
        }
        public float CalculateDmg(float dmg)
        {
            return dmg * (1 + (_skill?.chargeDmgRatio ?? 0) / 100f + BaseChargeDmg / 100f);
        }

        public float CalculateGroggy(float groggy)
        {
            return groggy * ((_skill?.chargeGroggyRatio ?? 100f) / 100f);
        }

        public void OnCancel()
        {
        }

        public void OnUnEquip(ActiveSkill skill)
        {
            DeActivate(skill);
        }
    }

    public class CastingSkill : ISkillActive // 캐스팅형
    {
        private ActiveSkill _skill;
        public CastingSkill(ActiveSkill skill)
        {
            _skill = skill;
        }

        private Sequence seq;
        public void Activate(ActiveSkill skill)
        {
            skill.StartCharge();

            skill.Icon.ChangeType(new UI_AtkItemIcon.CastingUpdate(skill.Icon));
            if (skill.user is Player player)
            {
                player.animator.ResetTrigger("ChargeEnd");
                player.animator.SetTrigger("Casting");
                player.animator.SetInteger("ChargingType",skill.CastType);
            }
            seq = DOTween.Sequence();
            skill.eventUser?.EventManager.AddEvent(EventType.OnDash,Kill);
            skill.eventUser?.EventManager.AddEvent(EventType.OnHitReaction,Kill);
            skill.eventUser?.EventManager.AddEvent(EventType.OnJump,Kill);
            skill.CurCastTime = skill.CastTime;
            
            seq.SetDelay(skill.CastTime);
            seq.AppendCallback(() =>
            {
                skill.Active();
                skill.eventUser?.EventManager.ExecuteEvent(EventType.OnCastingEnd,new EventParameters(skill.eventUser));
            });
            seq.SetAutoKill(true);
            seq.onUpdate += () =>
            {
                if (skill.CurCastTime > 0)
                {
                    skill.CurCastTime -= Time.deltaTime;
                }
            };
            seq.onKill += () =>
            {
                skill.eventUser?.EventManager.RemoveEvent(EventType.OnDash,Kill);
                skill.eventUser?.EventManager.RemoveEvent(EventType.OnHitReaction,Kill);
                skill.eventUser?.EventManager.RemoveEvent(EventType.OnJump,Kill);

                if (Mathf.Approximately(skill.Duration,0))
                {
                    skill.CDActive.SetIconCdType(skill.Icon);
                }
                skill.animator?.animator.SetTrigger("ChargeEnd");
                skill.CurChargeTime = 0;
            };
            
            void Kill(EventParameters x)
            {
                seq?.Kill();
                skill.CurCd = 0;
                skill.Cancel();
                skill.eventUser.EventManager.ExecuteEvent(EventType.OnCastingCancel,new (skill.eventUser));
            }
        }

        public void DeActivate(ActiveSkill skill)
        {
            seq?.Kill();
        }

        public bool durationUse => true;

        public bool CheckUsable => CheckPlayer() && _skill?.mover != null && _skill.mover.ActorMovement.IsStick && !seq.IsActive();
        bool CheckPlayer()
        {
            Player Player = _skill.user as Player;
            bool playerCheck = Player == null || ((_skill.usableWhenAttack || !Player.OnAttack) && !Player.IsSkill && !Player.IsClimb);
            return !_skill.usePlayerCheck || playerCheck;
        }
        public float CalculateDmg(float dmg)
        {
            return dmg;
        }

        public float CalculateGroggy(float groggy)
        {
            return groggy;
        }

        public void OnCancel()
        {
        }

        public void OnUnEquip(ActiveSkill skill)
        {
            DeActivate(skill);
        }
    }
    
    public class ToggleSkill : ISkillActive // 토글형
    {
        private readonly ActiveSkill baseSkill;
        readonly ActiveSkill secondSkill;
        public bool isToggleOn;
        private readonly bool useFrame;

        
        public ToggleSkill(ActiveSkill baseSkill,ActiveSkill secondSkill , bool useFrame)
        {
            baseSkill.OnSkillEquip += () => secondSkill.Equip(baseSkill.user);
            this.baseSkill = baseSkill;
            this.secondSkill = secondSkill;
            isToggleOn = false;
            this.useFrame = useFrame;
        }
        public void Activate(ActiveSkill skill)
        {
            if(isToggleOn)
            {
                secondSkill.Active();
            }
            else
            {
                skill.Active();
            }
            isToggleOn = !isToggleOn;
            if (useFrame)
            {
                skill.Icon.activatedFrame.gameObject.SetActive(isToggleOn);
            }
        }

        public void DeActivate(ActiveSkill skill)
        {
            if (isToggleOn)
            {
                Activate(skill);
            }
        }

        public bool durationUse => true;

        public bool CheckUsable => CheckPlayer();
        
        bool CheckPlayer()
        {
            ActiveSkill _skill = isToggleOn ? secondSkill : baseSkill;
            Player Player = _skill.user as Player;
            bool playerCheck = Player == null || ((_skill.usableWhenAttack || !Player.OnAttack) && !Player.IsSkill && !Player.IsClimb);
            return !_skill.usePlayerCheck || playerCheck;
        }
        public float CalculateDmg(float dmg)
        {
            return dmg;
        }

        public float CalculateGroggy(float groggy)
        {
            return groggy;
        }

        public void OnCancel()
        {
        }

        public void OnUnEquip(ActiveSkill skill)
        {
            isToggleOn = false;
            skill.Icon.activatedFrame.gameObject.SetActive(false);
        }
    }

    public class ContinuousSkill : ISkillActive // 지속형
    {
        private ActiveSkill _skill;
        private Player player;

        private static List<ActorCommand> _endSkillCommand;
        private static List<ActorCommand> EndSkillCommand => _endSkillCommand ??= new()
        {
            ResourceUtil.Load<ActorCommand>("EndWeaponSkill")
        };

        private static List<ActorCommand> _activeSkillEndCommand;
        private static List<ActorCommand> activeSkillEndCommand => _activeSkillEndCommand ??= new()
        {
            ResourceUtil.Load<ActorCommand>("EndActiveSkill")
        };
        
        private List<ActorCommand> original;
        ActorController controller => GameManager.PlayerController as ActorController;

        public ContinuousSkill(ActiveSkill skill)
        {
            _skill = skill;
        }
        public void Activate(ActiveSkill skill)
        {
            _skill = skill;
            _skill?.animator?.IdleOn();
            
            player = skill.user as Player;
           
            if (player != null && controller != null)
            {
                if (skill.EnterSkillState)
                {
                    player.SetState(EPlayerState.Skill);
                }

                if (skill is PlayerActiveSkill)
                {
                    original = controller.Executors[Define.GameKey.ActiveSkill].keyUpCommand.Commands;
                    controller.Executors[Define.GameKey.ActiveSkill].keyUpCommand.Commands = activeSkillEndCommand;
                }
                else if (skill != null)
                {
                    original = controller.Executors[Define.GameKey.Attack].keyUpCommand.Commands;
                    controller.Executors[Define.GameKey.Attack].keyUpCommand.Commands = EndSkillCommand;
                    EndSkillCommand.ForEach(x =>
                    {
                        if (x is EndWeaponSkill es)
                        {
                            es.index = skill.Item.AtkSlotIndex;
                        }
                    });
                }
               

            }
            _skill?.mover?.MoveOff();
            _skill?.attacker?.AttackOff();
            _skill?.Active();
            
        }

        public void DeActivate(ActiveSkill skill)
        {
           _skill?.Cancel();
        }
        
        public bool durationUse => false;
        public bool CheckUsable => CheckPlayer();
        
        bool CheckPlayer()
        {
            Player Player = _skill.user as Player;
            bool playerCheck = Player == null || ((_skill.usableWhenAttack || !Player.OnAttack) && !Player.IsSkill && !Player.IsClimb);
            return !_skill.usePlayerCheck || playerCheck;
        }
        public float CalculateDmg(float dmg)
        {
            return dmg;
        }

        public float CalculateGroggy(float groggy)
        {
            return groggy;
        }

        public void OnCancel()
        {
            if (_skill is PlayerActiveSkill && original != null)
            {
                controller.Executors[Define.GameKey.ActiveSkill].keyUpCommand.Commands = original;
            }
            else if (_skill != null)
            {

                if (original != null)
                {
                    controller.Executors[Define.GameKey.Attack].keyUpCommand.Commands = original;
                }
            }
           
        }

        public void OnUnEquip(ActiveSkill skill)
        {
            DeActivate(skill);
        }
    }
}
