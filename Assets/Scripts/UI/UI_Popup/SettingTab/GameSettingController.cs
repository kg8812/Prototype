using Apis.UI;
using Save.Schema;

namespace Apis
{
    public class GameSettingController : UISetting_Content
    {
        public override void Init()
        {
            base.Init();
            Bind<UIAsset_Carousel>(typeof(Carousels));
            Bind<UIAsset_Toggle>(typeof(Toggles));

            Get<UIAsset_Carousel>((int)Carousels.Language).ValueChanged.AddListener(OnLanguageChanged);
            Get<UIAsset_Toggle>((int)Toggles.CameraShaking).OnValueChanged.AddListener(OnCamShakeChanged);
        }

        public override void ResetBySaveData(SettingData data)
        {
            Get<UIAsset_Carousel>((int)Carousels.Language).MoveTo((int)data.languageType);
            // TODO: 추후 카메라 쉐이킹 setting 목록에 추가되면 설정.
            // if (data.CamShake)
            // {
            //     Get<UIAsset_Toggle>((int)Toggles.CamShakeToggle).SelectOn();
            // }
            // else
            // {
            //     Get<UIAsset_Toggle>((int)Toggles.CamShakeToggle).SelectOff(true);
            // }
        }


        private void OnLanguageChanged(int id)
        {
            // TODO: 얼액 이후 언어 변경 키면 설정.
            UI_Setting.IsDirty = true;
        }

        private void OnCamShakeChanged(bool isTrue)
        {
            // TODO
            UI_Setting.IsDirty = true;
        }

        private enum Carousels
        {
            Language
        }

        private enum Toggles
        {
            CameraShaking
        }
    }
}