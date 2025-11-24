using System;
using System.Collections.Generic;
using Apis;
using Default;
using Save.Schema;
using TMPro;
using UISpaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UI_MainHud : UI_Main
    {
        public Dictionary<PlayerType, Sprite> portraits = new();

        //Enum.GetValues로 사용중, Enum목록 제거하지 말것
        enum SubItems
        {
            PotionCounts,
            BuffCollector,
            AtkItemIcon1,
            AtkItemIcon2,
            AtkItemIcon3,
            AtkItemIcon4,
            SkillIcon,
            HpBar,
            ViichanGauge,
            JururuStack,
            LilpaBullet,
            GoseguGauge,
            IneMana,
            JingGauge,
            ExpBar
        }

        enum Texts
        {
            SoulText,
            LobbySoulText,
            SkillCdText,
            WpSkillCdText
        }

        enum Images
        {
            Portrait
        }

        static UI_MainHud instance;

        private UnityEvent<Player> _setEvent = new();
        private UnityEvent<Player> _afterSet = new();
        /// <summary>
        ///  UI 오브젝트들 참조 세팅용 이벤트 (플레이어 연결 등)
        /// </summary>
        public UnityEvent<Player> setEvent => _setEvent ??= new(); 
        /// <summary>
        /// 참조 세팅 된 후에, 텍스트 값 조절 등 할 때 사용
        /// </summary>
        public UnityEvent<Player> afterSet => _afterSet ??= new();
        
        public static UI_MainHud Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UI_MainHud>();
                }
                return instance;
            }
        }

        TextMeshProUGUI soulText;
        TextMeshProUGUI lobbySoulText;
        UI_BuffCollector buffs;

        Image portrait;

        public UI_AtkItemIcon mainSkillIcon;
        public List<UI_AtkItemIcon> atkItemIcons;

        private SubItems[] subs;
        public override void Init()
        {
            base.Init();
            instance = this;
            Bind<UI_Base>(typeof(SubItems));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Image>(typeof(Images));
            soulText = Get<TextMeshProUGUI>((int)Texts.SoulText);
            lobbySoulText = Get<TextMeshProUGUI>((int)Texts.LobbySoulText);
            subs = (SubItems[])Enum.GetValues(typeof(SubItems));
            foreach (SubItems sub in subs)
            {
                UI_Base item = Get<UI_Base>((int)sub);
                subItems.Add(item);
                item.Init();
            }

            portrait = GetImage((int)Images.Portrait);
            buffs = Get<UI_Base>((int)SubItems.BuffCollector).GetComponent<UI_BuffCollector>();
            GameManager.instance.afterPlayerStart.AddListener(ResisterPlayer);
            GameManager.instance.OnSoulChange.AddListener(ChangeSoul);
            GameManager.instance.OnLobbySoulChange.AddListener(ChangeSoul);
            ChangeSoul();
            buffs.SetDescPivot(new Vector2(0, 0));

            GameManager.instance.onPlayerChange.AddListener(x =>
            {
                setEvent.Invoke(x);
                afterSet.Invoke(x);
                ResisterPlayer(GameManager.instance.Player);
            });
        }

        public override void TryActivated(bool force = false)
        {
            base.TryActivated(force);
            // foreach (SubItems sub in subs)
            // {
            //     UI_Base item = Get<UI_Base>((int)sub);
            //     item.TryActivated(true);
            // }
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            // foreach (SubItems sub in subs)
            // {
            //     UI_Base item = Get<UI_Base>((int)sub);
            //     item.TryDeactivated(true);
            // }
        }

        void ChangeSoul(int x = 0)
        {
            soulText.text = GameManager.instance.Soul.ToString();
            lobbySoulText.text = GameManager.instance.LobbySoul.ToString();
        }
        
        private void ResisterPlayer(Player player)
        {
            Get<UI_Base>((int)SubItems.HpBar).GetComponent<UI_HpBar>().ResetActors();
            Get<UI_Base>((int)SubItems.HpBar).GetComponent<UI_HpBar>().Init(player);
            
            // TODO: 일단 activated에서 init쪽으로 옮겼는데 나중에 문제 생기면 고치기
            buffs.Init(player);
        }

        public void ChangePortrait()
        {
            portrait.sprite = portraits[GameManager.instance.Player.playerType];
        }
    }
}