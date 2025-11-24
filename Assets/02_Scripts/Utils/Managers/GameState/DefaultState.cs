using System;
using UnityEngine;

namespace GameStateSpace
{
    public class DefaultState : GameState
    {
        private Guid _pauseGuid;
        public override void OnEnterState()
        {
            _pauseGuid = GameManager.instance.RegisterPause();
        }

        public override void OnExitState()
        {
            GameManager.instance.RemovePause(_pauseGuid);
        }
        public override void KeyBoardControlling()
        {
            base.KeyBoardControlling();
            GameManager.UiController?.KeyControl();
        }

        public override void GamePadControlling()
        {
            base.GamePadControlling();
            GameManager.UiController?.GamePadControl();
        }
    }
}