using System;
using System.Collections.Generic;
using Apis;
using Default;
using TMPro;
using UISpaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class UI_MainHud : UI_Main
    {

        //Enum.GetValues로 사용중, Enum목록 제거하지 말것
        enum SubItems
        {
            BuffCollector,
            SkillIcon,
            HpBar,
        }

        enum Texts
        {
            SkillCdText,
            WpSkillCdText
        }

        enum Images
        {
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

        UI_BuffCollector buffs;

        public UI_AtkItemIcon mainSkillIcon;

        private SubItems[] subs;
        public override void Init()
        {
            base.Init();
            instance = this;
            Bind<UI_Base>(typeof(SubItems));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Image>(typeof(Images));
            subs = (SubItems[])Enum.GetValues(typeof(SubItems));
            foreach (SubItems sub in subs)
            {
                UI_Base item = Get<UI_Base>((int)sub);
                subItems.Add(item);
                item.Init();
            }

            buffs = Get<UI_Base>((int)SubItems.BuffCollector).GetComponent<UI_BuffCollector>();
            GameManager.instance.afterPlayerStart.AddListener(ResisterPlayer);
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
        }

        protected override void Deactivated()
        {
            base.Deactivated();
        }
        
        private void ResisterPlayer(Player player)
        {
            Get<UI_Base>((int)SubItems.HpBar).GetComponent<UI_HpBar>().ResetActors();
            Get<UI_Base>((int)SubItems.HpBar).GetComponent<UI_HpBar>().Init(player);
            
            // TODO: 일단 activated에서 init쪽으로 옮겼는데 나중에 문제 생기면 고치기
            buffs.Init(player);
        }
    }
}