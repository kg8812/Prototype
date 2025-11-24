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
        
        // public void ExitDefaultStateToNon()
        // {
        //     if(GameManager.instance.CurGameStateType != GameStateType.DefaultState) return;
        //     GameManager.instance.ChangeGameState(GameStateType.NonBattleState);
        // }


        // private void EscapeSceneChanging(string sceneName)
        // {
        //     if (sceneName == "Lobby" || sceneName == "Tutorial" || sceneName.StartsWith("Map") || sceneName == "Ending")
        //     {
        //         // n초 기다리지 말고 바로 이동으로 수정
        //         GameManager.instance.ChangeGameState(GameStateType.NonBattleState);
        //         // GameManager.instance.StartCoroutineWrapper(EscapeState());
        //     }
        // }

        /*
        private IEnumerator EscapeState()
        {
            // 씬 로딩 후, n초 기다리고 탈출
            yield return new WaitForSecondsRealtime(1f);
            GameManager.instance.ChangeGameState(GameStateType.NonBattleState);
        }
        */
    }
}