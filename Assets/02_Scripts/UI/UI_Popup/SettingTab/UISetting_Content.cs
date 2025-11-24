using Apis.UI.Focus;
using Save.Schema;

namespace Apis
{
    public class UISetting_Content: UI_FocusContent
    {
        public FocusParent focusParent;

        public override void Init()
        {
            base.Init();
            focusParent ??= GetComponent<FocusParent>();
            focusParent?.InitCheck();
        }

        public virtual void ResetBySaveData(SettingData data)
        {
            
        }

        public override void KeyControl()
        {
            focusParent?.KeyControl();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            focusParent?.GamePadControl();
        }

        public override void OnOpen()
        {
            focusParent?.FocusReset();
        }
    }
}