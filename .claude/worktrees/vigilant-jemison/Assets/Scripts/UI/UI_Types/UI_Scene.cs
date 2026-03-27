using System;
using Apis;
using Managers;
using UnityEngine;

namespace Default
{
    

    public class UI_Scene : UI_Base, IController
    {
        public bool dontShowMainCam = false;
        public bool isOverlay;

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
            if (!isOverlay)
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
            if (!isOverlay)
            {
                GameManager.instance.RemovePause(_pauseGuid);
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