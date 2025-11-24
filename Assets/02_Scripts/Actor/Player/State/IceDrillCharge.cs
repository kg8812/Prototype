using System.Collections;
using System.Collections.Generic;
using Apis;
using Command;
using Default;
using NewNewInvenSpace;
using PlayerState;
using UnityEngine;

namespace PlayerState
{
    public class IceDrillCharge : BaseRunState, IAnimate
    {
        private UseIceDrill _iceDrillCommand;
        private UseIceDrill IceDrillCommand => _iceDrillCommand ??= ResourceUtil.Load<UseIceDrill>("IceDrillCommand");
        
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            _player.IsIceDrill = true;
            var drill = t.EffectSpawner.Spawn(Define.PlayerEffect.IceDrillLoop, t.Position, false, true);
            SpineUtils.AddBoneFollower(t.Mecanim,"center",drill.gameObject);
            drill.gameObject.SetRadius(0.5f,_player.Direction);
            var List = new List<ActorCommand>()
            {
                IceDrillCommand
            };
            
            for (int i = 0; i < AttackItemManager.AtkInven.AtkItemInven.Count; i++)
            {
                if (AttackItemManager.AtkInven.CanUseAttackItem(i))
                {
                    switch (i)
                    {
                        case 0:
                            t.Controller.Executors[Define.GameKey.Attack1].keyDownCommand.Commands = List;
                            break;
                        case 1:
                            t.Controller.Executors[Define.GameKey.Attack2].keyDownCommand.Commands = List;
                            break;
                        case 2:
                            t.Controller.Executors[Define.GameKey.Attack3].keyDownCommand.Commands = List;
                            break;
                        case 3:
                            t.Controller.Executors[Define.GameKey.Attack4].keyDownCommand.Commands = List;
                            break;
                    }
                }
            }
            _player.StateEvent.AddEvent(EventType.OnTurn, OnTurn);
        }

        private void OnTurn(EventParameters e) => RemoveAbleState(EPlayerState.IceDrillCharge);

        public void OnEnterAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsIceDrill, true);
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.IsIceDrill = false;
            _player?.Controller.ReturnToBase();
            _player?.EffectSpawner.Remove(Define.PlayerEffect.IceDrillLoop);
            _player.StateEvent.RemoveEvent(EventType.OnTurn, OnTurn);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsIceDrill, false);
        }
    }

    public class IceDrillExecute : EventState, IInterruptable
    {
        public override EPlayerState NextState { get => EPlayerState.Stop; set {} }

        public float InterruptTime { get => _player.IceDirllAfterDelay; set {} }

        public EPlayerState[] InteruptableStates => new EPlayerState[] {EPlayerState.Move, EPlayerState.Jump, EPlayerState.Attack, EPlayerState.AirMove, EPlayerState.Dash };

        private bool escapeFlag = false;
        public override bool EscapeCondition()
        {
            return escapeFlag;
        }

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            escapeFlag = false;
            _player.StopMoving();
            _player.StateEvent.AddEvent(EventType.OnDrillComplete, (e) => { escapeFlag = true; });
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.StateEvent.RemoveAllEvents(EventType.OnDrillComplete);
        }
    }
}