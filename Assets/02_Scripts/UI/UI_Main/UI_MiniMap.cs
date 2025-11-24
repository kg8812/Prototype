
using Apis;
using UI;
using UnityEngine;

namespace UISpaces
{
    public class UI_MiniMap: UI_Main
    {
        [SerializeField] private RectTransform parent;

        public override void Init()
        {
            base.Init();
            UI_TabMenu.OnUiToggle += TabMenuToggle;
            TabMenuToggle(false);
        }

        private void TabMenuToggle(bool isOn)
        {
            if (isOn) return;
            RectTransform mapTrans = Map.instance.SetParent(parent);
            mapTrans.localPosition = Vector3.zero;
            mapTrans.anchoredPosition = Vector2.zero;
            mapTrans.localScale = Vector3.one;
            Map.instance.UpdateToPlayer = true;
        }
    }
}