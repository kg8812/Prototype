using System;
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
        private enum SubItems
        {
        }

        private enum Texts
        {
            SkillCdText,
            WpSkillCdText
        }

        private enum Images
        {
        }
        
        private static UI_MainHud instance;

        private UnityEvent<Player> _setEvent = new();
        private UnityEvent<Player> _afterSet = new();

        private SubItems[] subs;
        
        /// <summary>
        ///     UI 오브젝트들 참조 세팅용 이벤트 (플레이어 연결 등)
        /// </summary>
        public UnityEvent<Player> setEvent => _setEvent ??= new UnityEvent<Player>();

        /// <summary>
        ///     참조 세팅 된 후에, 텍스트 값 조절 등 할 때 사용
        /// </summary>
        public UnityEvent<Player> afterSet => _afterSet ??= new UnityEvent<Player>();

        public static UI_MainHud Instance
        {
            get
            {
                if (instance == null) instance = FindFirstObjectByType<UI_MainHud>();
                return instance;
            }
        }

        public override void Init()
        {
            base.Init();
            instance = this;
            Bind<UI_Base>(typeof(SubItems));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Image>(typeof(Images));
            subs = (SubItems[])Enum.GetValues(typeof(SubItems));
            foreach (var sub in subs)
            {
                var item = Get<UI_Base>((int)sub);
                subItems.Add(item);
                item.Init();
            }

            GameManager.instance.afterPlayerStart.AddListener(ResisterPlayer);

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
        }

        
    }
}