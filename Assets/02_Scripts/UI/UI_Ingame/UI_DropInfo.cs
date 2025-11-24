
using UnityEngine;

namespace UI
{
    public class UI_DropInfo : UI_Ingame
    {
        enum RectTransforms
        {
            background
        }
    
        private Vector2 offset = new Vector2(0, 0);

        private RectTransform rect;
        private Vector2 worldPos = Vector2.zero;

        public override void Init()
        {
            base.Init();
        
            Bind<RectTransform>(typeof(RectTransforms));
            rect = Get<RectTransform>((int)RectTransforms.background);
        }

        public void SetInfo(Vector2 targetPos)
        {
            this.targetPos = targetPos;
        }
        public void SetInfo(Transform targetTrans)
        {
            this.targetTrans = targetTrans;
        }


        protected override void PositioningFollower()
        {
            rect.anchoredPosition = calcPos + offset;
        }
    }

}
