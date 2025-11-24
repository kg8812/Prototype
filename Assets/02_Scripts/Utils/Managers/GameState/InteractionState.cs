namespace GameStateSpace
{
    public class InteractionState: GameState
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
            GameManager.UiController?.KeyControl();
            GameManager.DefaultController?.KeyControl();
        }

        public override void GamePadControlling()
        {
            base.GamePadControlling();
            
            GameManager.UiController?.GamePadControl();
            GameManager.DefaultController?.GamePadControl();
        }

        // public void ToNonBattleState()
        // {
        //     if (GameManager.instance.CurGameStateType != GameStateType.InteractionState) return;
        //     GameManager.instance.Resume();
        //     GameManager.instance.ChangeGameState(GameStateType.NonBattleState);
        // }
    }
}