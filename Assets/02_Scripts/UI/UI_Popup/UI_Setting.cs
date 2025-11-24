using Apis.Managers;
using Default;
using Apis.UI;
using Managers;
using Save.Schema;
using UnityEngine.UI;

namespace Apis
{ 
    public class UI_Setting : UI_Popup
    {
        public UI_HeaderMenu_nton settingHeaderMenuNton;
        public static bool IsDirty;

        private UISetting_Content[] _contents;
        public override void Init()
        {
            base.Init();
            settingHeaderMenuNton.Init();
            _contents = transform.GetComponentsInChildren<UISetting_Content>();
        }

        public override void TryActivated(bool force = false)
        {
            settingHeaderMenuNton.Reset();
            // TODO: 저장된 데이터 기반으로 setting 값들 초기화
            foreach (var content in _contents)
            {
                content.ResetBySaveData(DataAccess.Settings.Data);
            }
            IsDirty = false;
            base.TryActivated(force);
        }

        public override void KeyControl()
        {
            base.KeyControl();
            settingHeaderMenuNton.KeyControl();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            settingHeaderMenuNton.GamePadControl();
        }

        public override void CloseOwn()
        {
            if (IsDirty)
            {
                SystemManager.SystemCheck(LanguageManager.Str(10118801), isOn =>
                {
                    if (isOn)
                    {
                        DataAccess.Settings.Save();
                    }
                    base.CloseOwn();
                });
            }
            else
            {
                base.CloseOwn();
            }
        }
    }
}