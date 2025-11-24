using Apis;
using Default;
using UnityEngine;

namespace UISpaces
{
    public class UI_Main: UI_Base
    {
        private bool mainUIShow;

        public bool MainUIShow
        {
            get => mainUIShow;
            set
            {
                mainUIShow = value;
                ToggleCanvas(value, true);
                    
            }
        }

        public override void Init()
        {
            base.Init();
            UIManager.SetCanvas(this, UIType.Main);
        }


        public virtual void ReShow()
        {
            ToggleCanvas(true);
        }

        protected override void Activated()
        {
            base.Activated();
            if (!mainUIShow)
            {
                ToggleCanvas(false, true);
            }
        }
    }
}