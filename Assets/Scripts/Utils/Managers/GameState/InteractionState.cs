namespace GameStateSpace
{
    /// <summary>UI + 기본 조작이 가능한 상태. 상호작용 UI가 떠 있을 때.</summary>
    public class InteractionState : GameState
    {
        public override int Priority => StatePriority.Interaction;

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