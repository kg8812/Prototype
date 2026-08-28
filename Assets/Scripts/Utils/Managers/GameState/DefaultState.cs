using System;

namespace GameStateSpace
{
    /// <summary>UI만 조작 가능한 상태. 페이드 중, 시스템 알림 중, 플레이어가 없는 씬에서 쓴다.</summary>
    public class DefaultState : GameState
    {
        public override int Priority => StatePriority.Default;


        public override void OnEnterState()
        {
        }

        public override void OnExitState()
        {
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