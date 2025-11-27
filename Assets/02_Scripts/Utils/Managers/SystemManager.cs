using System;
using Apis;
using GameStateSpace;
using UI;
using UnityEngine.Events;

namespace Managers
{
    public static class SystemManager
    {
        private static UnityAction<bool> workByCheck;
        private static UnityAction workByAlert;

        private static IController preUIController;

        private static UI_SystemCheck _uiSystemCheck;
        private static UI_SystemAlert _uiSystemAlert;

        private static Guid _systemCheckGuid;
        private static Guid _systemAlertGuid;


        public static void SystemCheck(string msg, UnityAction<bool> todo)
        {
            _uiSystemCheck = GameManager.UI.CreateUI("UI_SystemCheck", UIType.Popup) as UI_SystemCheck;
            if (_uiSystemCheck)
            {
                workByCheck = todo;
                // preUIController = GameManager.UiController;

                _uiSystemCheck?.SetText(msg);
                _systemCheckGuid = GameManager.instance.TryOnGameState(GameStateType.DefaultState);
                // GameManager.UiController = _uiSystemCheck;
            }
        }

        public static void SystemAlert(string msg, UnityAction todo)
        {
            _uiSystemAlert = GameManager.UI.CreateUI("UI_SystemAlert", UIType.Popup) as UI_SystemAlert;
            if (_uiSystemAlert)
            {
                workByAlert = todo;
                // preUIController = GameManager.UiController;
                _uiSystemAlert.SetText(msg);
                _systemAlertGuid = GameManager.instance.TryOnGameState(GameStateType.DefaultState);
                // GameManager.UiController = _uiSystemCheck;
            }
        }


        public static void SystemCheckComplete(bool isYes)
        {
            GameManager.instance.TryOffGameState(GameStateType.DefaultState, _systemCheckGuid);
            // GameManager.UiController = preUIController;
            GameManager.UI.CloseUI(_uiSystemCheck);
            workByCheck?.Invoke(isYes);
        }

        public static void SystemAlertComplete()
        {
            GameManager.instance.TryOffGameState(GameStateType.DefaultState, _systemAlertGuid);
            // GameManager.UiController = preUIController;
            GameManager.UI.CloseUI(_uiSystemAlert);
            workByAlert?.Invoke();
        }
    }
}