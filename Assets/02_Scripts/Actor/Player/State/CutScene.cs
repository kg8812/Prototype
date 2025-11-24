using System;
using Directing;
using UnityEngine;

namespace PlayerState
{
    public class CutScene : EventState, IAnimate
    {
        public override EPlayerState NextState { get => EPlayerState.Stop; set{} }

        public override bool EscapeCondition()
        {
            return escapeFlag;
        }

        private bool escapeFlag = false;
        private Guid gid;
        private PlayerCutsceneSkeleton cutsceneActor;
        private PlayerTimelineDummy dummy;
        private EActorDirection originalDir;
        private EActorDirection originalActorDir;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            escapeFlag = false;

            gid = _player.AddInvincibility();

            var info = _player.GetInfo(EPlayerState.CutScene) as CutsceneInfo;
            dummy = info.Dummy;

            _player.StateEvent.AddEvent(EventType.OnCutSceneEnd, (e) => { 
                // 컷씬 스켈레톤 off
                _player.CutsceneSkeleton.Disable();

                // 위치 sync
                _player.transform.position = _player.CutsceneSkeleton.transform.position;
                _player.CutsceneSkeleton.transform.localPosition = Vector3.zero;

                RestoreDir();

                // 플레이어 on
                _player.Appear();
                escapeFlag = true; 
                
            });

            _player.StopMoving();

            _player.ControllerOff();

            cutsceneActor = _player.CutsceneSkeleton;
            cutsceneActor.Enable();

            _player.Hide();

            SyncDir();

            // Sync Direction w/ CutsceneObj
            _player.StateEvent.ExecuteEvent(EventType.OnCutScene, null);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.RemoveInvincibility(gid);
            _player.ControllerOn();
        }

        private void SyncDir()
        {
            originalDir = _player.Direction;
            originalActorDir = cutsceneActor.Direction;

            if(_player.Direction != dummy.WorldDirection)
                _player.Flip();
            cutsceneActor.SetDirection(dummy.Direction);
        }

        private void RestoreDir()
        {
            _player.SetDirection(originalDir);
            cutsceneActor.SetDirection(originalActorDir);
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.CutsceneOn);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.CutsceneEnd);
        }
    }
}
