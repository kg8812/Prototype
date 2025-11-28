namespace GameStateSpace
{
    public class NonBattleState : GameState
    {
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