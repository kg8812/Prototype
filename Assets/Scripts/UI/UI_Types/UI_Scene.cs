using System;
using Apis;
using Managers;
using UnityEngine;

namespace Default
{
    

    public class UI_Scene : UI_Base, IController
    {
        public bool dontShowMainCam = false;

        [Tooltip("활성화될 때 게임을 일시정지한다. 일시정지 메뉴처럼 정지가 꼭 필요한 UI에서만 켠다.")]
        public bool pauseOnActivate;

        private Guid _pauseGuid;

        public override void Init()
        {
            base.Init();
            UIManager.SetCanvas(this, UIType.Scene);
        }
        

        protected override void Activated()
        {
            base.Activated();
            GameManager.UI.RegisterUIController(this);
            if (pauseOnActivate)
            {
                _pauseGuid = GameManager.instance.RegisterPause();
            }

            if (dontShowMainCam)
            {
                CameraManager.instance.ToggleMainCamCullingMask(false);
            }
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            // 등록한 적이 있을 때만 해제한다. 플래그로 판정하면 활성화 이후 값이 바뀌었을 때 guid가 샌다.
            if (_pauseGuid != Guid.Empty)
            {
                GameManager.instance.RemovePause(_pauseGuid);
                _pauseGuid = Guid.Empty;
            }
        }

        public override void TryDeactivated(bool force = false)
        {
            if (dontShowMainCam)
            {
                CameraManager.instance.ToggleMainCamCullingMask(true);
            }
            base.TryDeactivated(force);
        }
        
        public virtual void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Cancel)))
            {
                CloseOwn();
            }
        }

        public virtual void GamePadControl()
        {
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Cancel)))
            {
                CloseOwn();
            }
        }

        private void OnDestroy()
        {
            if(_activated && !GameManager.IsQuitting)
                GameManager.UI.RemoveController(this);
        }

    }
}