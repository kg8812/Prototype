using Apis.UI;
using Save.Schema;

namespace Apis
{
    public class GraphicSettingController : UISetting_Content
    {
        public override void Init()
        {
            base.Init();
            Bind<UIAsset_Carousel>(typeof(Carousels));


            Get<UIAsset_Carousel>((int)Carousels.DisplayMode).ValueChanged.AddListener(OnDisPlayModeChanged);
            Get<UIAsset_Carousel>((int)Carousels.GraphicsQuality).ValueChanged.AddListener(OnGraphicsQualityChanged);
            Get<UIAsset_Carousel>((int)Carousels.AntiAliasing).ValueChanged.AddListener(OnAntiAliasingChanged);
            Get<UIAsset_Carousel>((int)Carousels.Frame).ValueChanged.AddListener(OnFrameChanged);
        }

        public override void ResetBySaveData(SettingData data)
        {
            // TODO: 그래픽 관련 세팅 목록 완료되면 하기
        }


        private void OnDisPlayModeChanged(int id)
        {
        }

        private void OnGraphicsQualityChanged(int id)
        {
        }

        private void OnAntiAliasingChanged(int id)
        {
        }

        private void OnFrameChanged(int id)
        {
        }

        private enum Carousels
        {
            DisplayMode,
            GraphicsQuality,
            AntiAliasing,
            Frame
        }
    }
}