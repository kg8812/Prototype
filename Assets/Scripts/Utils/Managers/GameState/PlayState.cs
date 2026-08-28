namespace GameStateSpace
{
    /// <summary>
    ///     일반 플레이 상태. 등록된 상태 중 우선순위가 가장 낮아 "아무것도 안 켜졌을 때"의 기본 상태가 된다.
    /// </summary>
    public class PlayState : GameState
    {
        public override int Priority => StatePriority.Play;

        public override void OnEnterState()
        {
        }

        public override void OnExitState()
        {
        }

        public override void KeyBoardControlling()
        {
            base.KeyBoardControlling();
            // Debug.Log("non battle state controlling");
            GameManager.PlayerController?.KeyControl();
            GameManager.DefaultController?.KeyControl();
        }

        public override void GamePadControlling()
        {
            base.GamePadControlling();
            GameManager.PlayerController?.GamePadControl();
            GameManager.DefaultController?.GamePadControl();
        }
    }
}