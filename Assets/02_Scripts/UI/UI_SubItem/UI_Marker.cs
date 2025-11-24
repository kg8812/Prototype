using Apis;
using Default;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI.UI_SubItem
{
    public class UI_Marker: UI_Base
    {

        private RectTransform _myRect;
        private Image _img;

        public override void Init()
        {
            base.Init();
            _myRect = GetComponent<RectTransform>();
            _img = GetComponent<Image>();
        }

        public void SetMarkerType(MarkerType type)
        {
            if (Map.instance.MarkerSprites.TryGetValue(type, out var value))
            {
                _img.sprite = value;
            }
        }

        public void UpdatePos(Vector2 worldPos)
        {
            // Debug.Log($"{gameObject.name} marker update pos {worldPos}");
            _myRect.anchoredPosition = Map.CalculateWorldPosToMapPos(worldPos);
        }
    }
}