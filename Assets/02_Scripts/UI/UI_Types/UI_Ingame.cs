using Apis;
using Default;
using Managers;
using UnityEngine;

namespace UI
{
    public class UI_Ingame: UI_Base
    {
        protected Transform targetTrans = null;
        protected Vector2 targetPos = Vector2.zero;


        private bool optimizationActivate;

        private Camera mainCam;
        private Camera uiCam;
        private RectTransform _canvasTrans;
        private Vector2 camPixel;

        protected Vector2 calcPos;


        public override void TryActivated(bool force = false)
        {
            CalCulateTargetPos();
            PositioningFollower();
            optimizationActivate = true;
            base.TryActivated(force);
        }

        public override void Init()
        {
            base.Init();
            UIManager.SetCanvas(this, UIType.Ingame);

            mainCam = CameraManager.instance.MainCam;
            uiCam = CameraManager.instance.UICam;
            _canvasTrans = GetComponent<RectTransform>();
            camPixel = new Vector2(uiCam.pixelWidth, uiCam.pixelHeight) * 0.5f;
        }


        protected virtual void Update()
        {
            if (_activated && optimizationActivate)
            {
                CalCulateTargetPos();
                PositioningFollower();
            }
        }

        protected virtual void CalCulateTargetPos()
        {
            if (!ReferenceEquals(null, targetTrans))
            {
                if (ReferenceEquals(null, targetTrans.gameObject))
                {
                    Debug.Log("Destroyed object");
                    return;
                }

                targetPos = (Vector2)targetTrans.position;
            }

            calcPos = (Vector2)mainCam.WorldToScreenPoint(targetPos) - camPixel;
            
        }

        protected virtual void PositioningFollower()
        {
            
        }



        public void ChangeOptimizationActivate(bool value)
        {
            if (_activated)
            {
                optimizationActivate = value;
            }
        }
    }
}