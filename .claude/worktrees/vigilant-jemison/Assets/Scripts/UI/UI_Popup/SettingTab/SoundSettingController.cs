using Save.Schema;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Apis
{
    public class SoundSettingController : UISetting_Content
    {
        enum Sliders
        {
            MasterVolume,
            BgmVolume,
            SfxVolume,
            UIVolume,
            AmbVolume,
        }

        enum Texts
        {
            MasterValue,
            BgmValue,
            SfxValue,
            AmbValue,
            UIValue,
        }
        
        

        Slider[] sliders = new Slider[(int)Define.Sound.MaxCOUNT];
        private TextMeshProUGUI[] texts = new TextMeshProUGUI[(int)Define.Sound.MaxCOUNT];

        public override void Init()
        {
            base.Init();
            Bind<Slider>(typeof(Sliders));
            Bind<TextMeshProUGUI>(typeof(Texts));

            sliders[(int)Define.Sound.BGM] = Get<Slider>((int)Sliders.BgmVolume);
            sliders[(int)Define.Sound.SFX] = Get<Slider>((int)Sliders.SfxVolume);
            sliders[(int)Define.Sound.UI] = Get<Slider>((int)Sliders.UIVolume);
            sliders[(int)Define.Sound.Ambience] = Get<Slider>((int)Sliders.AmbVolume);
            sliders[(int)Define.Sound.Master] = Get<Slider>((int)Sliders.MasterVolume);
            
            texts[(int)Define.Sound.BGM] = Get<TextMeshProUGUI>((int)Texts.BgmValue);
            texts[(int)Define.Sound.SFX] = Get<TextMeshProUGUI>((int)Texts.SfxValue);
            texts[(int)Define.Sound.UI] = Get<TextMeshProUGUI>((int)Texts.UIValue);
            texts[(int)Define.Sound.Ambience] = Get<TextMeshProUGUI>((int)Texts.AmbValue);
            texts[(int)Define.Sound.Master] = Get<TextMeshProUGUI>((int)Texts.MasterValue);


            for (int i = 0; i < (int)Define.Sound.MaxCOUNT; i++)
            {
                sliders[i].value = DataAccess.Settings.Data.Volumes[i];
                texts[i].text = GetStringByValue(sliders[i].value);
            }
            
            sliders[(int)Define.Sound.BGM].onValueChanged
                .AddListener(value => {
                    texts[(int)Define.Sound.BGM].text = GetStringByValue(value);
                    GameManager.Sound.ChangeVolume(value, Define.Sound.BGM);
                    UI_Setting.IsDirty = true;
                });
            sliders[(int)Define.Sound.SFX].onValueChanged
                .AddListener(value => {
                    texts[(int)Define.Sound.SFX].text = GetStringByValue(value);
                    GameManager.Sound.ChangeVolume(value, Define.Sound.SFX);  
                    UI_Setting.IsDirty = true;
                });
            sliders[(int)Define.Sound.Master].onValueChanged
                .AddListener(value => {
                    texts[(int)Define.Sound.Master].text = GetStringByValue(value);
                    GameManager.Sound.ChangeVolume(value, Define.Sound.Master); 
                    UI_Setting.IsDirty = true;
                });
            sliders[(int)Define.Sound.Ambience].onValueChanged
                .AddListener(value => {
                    texts[(int)Define.Sound.Ambience].text = GetStringByValue(value);
                    GameManager.Sound.ChangeVolume(value, Define.Sound.Ambience); 
                    UI_Setting.IsDirty = true;
                });
            sliders[(int)Define.Sound.UI].onValueChanged
                .AddListener(value => {
                    texts[(int)Define.Sound.UI].text = GetStringByValue(value);
                    GameManager.Sound.ChangeVolume(value, Define.Sound.UI); 
                    UI_Setting.IsDirty = true;
                });
        }

        protected override void Activated()
        {
            base.Activated();
            
        }

        public override void ResetBySaveData()
        {
            for (int i = 0; i < (int)Define.Sound.MaxCOUNT; i++)
            {
                sliders[i].value = DataAccess.Settings.Data.Volumes[i];
                texts[i].text = GetStringByValue(sliders[i].value);
            }
        }

        private string GetStringByValue(float value)
        {
            return Mathf.CeilToInt(value * 100).ToString();
        }
    }
}