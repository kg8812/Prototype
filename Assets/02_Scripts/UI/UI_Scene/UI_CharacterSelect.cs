using System.Collections.Generic;
using Apis;
using Apis.Managers;
using UI;
using UnityEngine;
using Default;
using Managers;
using Save.Schema;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Apis.UI;
using Apis.UI.Focus;
using UnityEngine.Video;

namespace Apis
{
    public class UI_CharacterSelect : UI_Scene
    {
        enum Buttons
        {
            Ine,
            Jing,
            Lil,
            Jururu,
            Gosegu,
            Viichan,
            Random
        }

        private enum SelectType
        {
            Ine,
            Jingburger,
            Lilpa,
            Jururu,
            Gosegu,
            Viichan,
            Random
        }

        private enum Texts
        {
            DescText,
            ActiveTitle,
            ActiveDesc,
            PassiveTitle,
            PassiveDesc
        }

        private enum Imgs
        {
            VideoPlayerImg
        }

        public UnityEvent<Player> selectPlayer;
        public FocusParent focusParent;
        [SerializeField] private VideoPlayer vp;

        private bool isShowCutscene;
        private bool isCreated;
        private bool _canClose = true;

        private GameObject popup;
        private TextMeshProUGUI popupText;

        private Dictionary<SelectType, UIAsset_Button> buttons;

        [SerializeField] private Button backButton;


        public override void Init()
        {
            base.Init();
            selectPlayer = new();

            buttons = new();
            Bind<UIAsset_Button>(typeof(Buttons));
            Bind<Image>(typeof(Imgs));
            Bind<TextMeshProUGUI>(typeof(Texts));

            for (int i = 0; i < 7; i++)
            {
                SelectType sType = (SelectType)i;
                buttons.Add(sType, Get<UIAsset_Button>(i));
                buttons[sType].OnClick.AddListener(() => SetPopup(sType));
                buttons[sType].FocusOn = () => ShowInfo(sType);
            }
        }


        private PlayerType SelectTypeToPlayerType(SelectType selectType)
        {
            if (selectType == SelectType.Random)
            {
                List<int> list = new();
                for (int i = 0; i < 6; i++)
                {
                    if (Check((SelectType)i))
                        list.Add(i);
                }

                int rand = Random.Range(0, list.Count);
                return (PlayerType)list[rand];
            }
            else
            {
                return (PlayerType)(int)selectType;
            }
        }

        public override void TryActivated(bool force = false)
        {
            isShowCutscene = false;
            isCreated = false;
            ToggleCanClose(true);

            foreach (var x in buttons.Keys)
            {
                if (!Check(x))
                    buttons[x].HoverOn();
                else
                    buttons[x].HoverOff();
            }

            if (GameManager.instance.Player == null)
            {
                buttons[SelectType.Random].HoverOn();
            }

            base.TryActivated(force);
        }

        public static string PlayerPassiveSkillName(int sType) =>
            StrUtil.GetEquipmentName(4101 + 100 * sType);
        
        public static string PlayerActiveSkillName(int sType) =>
            StrUtil.GetEquipmentName(4102 + 100 * sType);
        
        public static string PlayerPassiveSkillDesc(int sType) =>
            StrUtil.GetEquipmentDesc(4101 + 100 * sType);
        
        public static string PlayerActiveSkillDesc(int sType) =>
            StrUtil.GetEquipmentDesc(4102 + 100 * sType);

        private void ShowInfo(SelectType sType)
        {
            bool isQ = sType == SelectType.Random;
            string qT = "?";
            GetText((int)Texts.DescText).text = isQ ? qT : LanguageManager.Str(1010231 + (int)sType);
            GetText((int)Texts.ActiveTitle).text =
                isQ ? LanguageManager.Str(1010212) : PlayerActiveSkillName((int)sType);
            GetText((int)Texts.ActiveDesc).text = isQ ? qT : PlayerActiveSkillDesc((int)sType);
            GetText((int)Texts.PassiveTitle).text =
                isQ ? LanguageManager.Str(1010213) : PlayerPassiveSkillName((int)sType);
            GetText((int)Texts.PassiveDesc).text = isQ ? qT : PlayerPassiveSkillDesc((int)sType);


            if (isQ)
            {
                GetImage((int)Imgs.VideoPlayerImg).color = Color.black;
                vp.Stop();
            }
            else
            {
                vp.Stop();
                GetImage((int)Imgs.VideoPlayerImg).color = Color.white;
                vp.clip = ResourceUtil.Load<VideoClip>("Videos/Pv1");
                vp.Play();
            }
        }

        bool Check(SelectType type)
        {
            if (GameManager.instance.Player != null)
            {
                return (int)GameManager.instance.Player.playerType != (int)type;
            }

            return true;
        }

        private void SetPopup(SelectType type)
        {
            if (!Check(type)) return;

            int id = type switch
            {
                SelectType.Ine => 1010221,
                SelectType.Jingburger => 1010222,
                SelectType.Lilpa => 1010223,
                SelectType.Jururu => 1010224,
                SelectType.Gosegu => 1010225,
                SelectType.Viichan => 1010226,
                SelectType.Random => 1010227,
                _ => 1010227
            };

            SystemManager.SystemCheck(LanguageManager.Str(id), (isOn) =>
            {
                if (isOn)
                    ChangeCharacter(type);
            });
        }

        private void ChangeCharacter(SelectType character)
        {
            if (isCreated) return;
            isCreated = true;

            if (isShowCutscene)
            {
                FadeManager.instance.Fading(() =>
                {
                    Player newPlayer = Player.CreatePlayerByType(SelectTypeToPlayerType(character));
                    GameManager.UI.CloseUI(this);
                    GameManager.Sound.Stop(Define.Sound.BGM);
                    GameManager.Directing.ShowVideoCutScene(103, () => { selectPlayer.Invoke(newPlayer); });
                });
            }
            else
            {
                FadeManager.instance.Fading(() =>
                {
                    selectPlayer.Invoke(Player.CreatePlayerByType(SelectTypeToPlayerType(character)));

                    GameManager.UI.CloseUI(this);
                });
            }
        }

        public void ToggleShowCutScene(bool isShow)
        {
            isShowCutscene = isShow;
        }

        public void ToggleCanClose(bool isOn)
        {
            _canClose = isOn;
            backButton.gameObject.SetActive(isOn);
        }

        public override void KeyControl()
        {
            if (_canClose)
                base.KeyControl();
            focusParent?.KeyControl();
        }

        public override void GamePadControl()
        {
            if (_canClose)
                base.GamePadControl();
            focusParent?.GamePadControl();
        }
    }
}