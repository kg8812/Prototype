using Apis;
using Apis;
using Apis.Managers;
using DG.Tweening;
using Default;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class UI_GameOver : UI_Scene
    {
        private bool isRestarting;
        enum Contents
        {
            RestartActivation
        }
        enum Buttons
        {
            RestartButton
        }
        
        private GameObject restartActivation;

        Button restartButton;
        
        public override void KeyControl()
        {
        }

        public override void TryActivated(bool force = false)
        {
            restartActivation.SetActive(false);

            base.TryActivated(force);
            
        }

        protected override void Activated()
        {
            base.Activated();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Select)))
            {
                if (restartActivation.activeSelf)
                {
                    restartButton.onClick.Invoke();
                }
            }
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<GameObject>(typeof(Contents));
            restartButton = Get<Button>((int)Buttons.RestartButton);
            restartActivation = Get<GameObject>((int)Contents.RestartActivation);
            
            AddUIEvent(restartButton.gameObject, x =>
            {
                restartActivation.SetActive(true);
            }, Define.UIEvent.PointEnter);
            AddUIEvent(restartButton.gameObject, x =>
            {
                restartActivation.SetActive(false);
            },Define.UIEvent.PointExit);
            restartButton.onClick.AddListener(() =>
            {
                FadeManager.instance.fadeIn.AddListener(Close);
                GameManager.SectorMag.MoveToLastShelter(true);
                GameManager.instance.BattleStateClass.ResetMonsterHpBar();
            });
        }

        new void Close()
        {
            GameManager.UI.CloseUI(this);
            GameManager.instance.Player.ResetPlayerStatus();
            FadeManager.instance.fadeIn.RemoveListener(Close);
        }
    }
}