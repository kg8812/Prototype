using Apis.UI.Focus;

namespace Apis
{
    public class UITab_Character: UI_FocusContent
    {
        
        public override void KeyControl()
        {
            // 캐릭터 탭은 예외적으로 아무런 상호작용을 하지 않음.
        }

        public override void OnOpen()
        {
            // 캐릭터 탭은 예외적으로 아무런 focusParent가 없음.
        }
    }
}