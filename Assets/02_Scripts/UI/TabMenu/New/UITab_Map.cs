using System;
using System.Collections.Generic;
using Apis.DataType;
using Apis.UI;
using Apis.UI.Focus;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Apis
{
    public class UITab_Map: UI_FocusContent
    {
        [SerializeField] private float minZoom = 30f;
        [SerializeField] private float maxZoom = 600f;
        private readonly Vector2 screenQuater = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        private readonly Vector2 renderImgSize = new Vector2(1200, 700);
        
        
        [SerializeField] private Transform parentCanvas;
        [SerializeField] private Color color = Color.white;

        [SerializeField] private Sprite sprite;

        [SerializeField] private RectTransform playerTrans;
        [SerializeField] private Vector2 zeroPos;
        [SerializeField] private float ratio;
        [SerializeField] private float preSize = 0.01f;
        [SerializeField] private float wheelSpeed = 30f;

        enum Images
        {
            MapImg,
            DragSection
        }

        enum RectTransforms
        {
            MapImg,
            MapBackground,
            MapParent
        }

        enum Sliders
        {
            ZoomSlider
        }

        enum Btns
        {
            ResetBtn
        }

        private MapDataType _mapDataType;


        // private Image mapImg;
        private RectTransform mapImgRect;
        private RectTransform mapParent;
        private Slider zoomSlider;

        private float preZoom = 100f;
        private float zoom = 100f;

        private Vector2 mapParentScreenPos;

        private Vector2 beginPos;
        private Vector2 curPos;

        

        private Vector2 beginMousePos;
        private Vector2 curMousePos;
        private Vector2 deltaMousePos;
        private Vector2 curMousePos2;

        // raw img의 render img에서의 start position
        private Vector2 spCam;

        // screen position to mapCam position(rawImg) resize value
        private float reSizeRatio;

        // raw img의 screen에서의 start position
        private Vector2 spScr;

        private bool _isDragWithWheel;

        public override void Init()
        {
            base.Init();
            _mapDataType = new();
            preZoom = zoom;


            Bind<RectTransform>(typeof(RectTransforms));
            Bind<Image>(typeof(Images));
            Bind<Slider>(typeof(Sliders));
            Bind<UIAsset_Button>(typeof(Btns));
            mapParent = Get<RectTransform>((int)RectTransforms.MapBackground);
            mapParentScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.instance.UICam, mapParent.position);
            // Debug.Log(mapParentScreenPos);
            // mapImg = GetImage((int)Images.MapImg);
            zoomSlider = Get<Slider>((int)Sliders.ZoomSlider);

            zoomSlider.onValueChanged.AddListener(value =>
            {
                if (_isDragWithWheel)
                {
                    _isDragWithWheel = false;
                }
                else
                {
                    zoom = value * (maxZoom - minZoom) + minZoom;
                    ZoomChanged(false);
                }
            });
            
            Get<UIAsset_Button>((int)Btns.ResetBtn).OnClick.AddListener(Reset);

            AddUIEvent(GetImage((int)Images.DragSection).gameObject,
                d =>
                {
                    beginMousePos = Input.mousePosition;
                    beginPos = mapImgRect.anchoredPosition;
                }, Define.UIEvent.BeginDrag);
            AddUIEvent(GetImage((int)Images.DragSection).gameObject, OnDrag, Define.UIEvent.Drag);
            AddUIEvent(GetImage((int)Images.DragSection).gameObject, _ => WheelCheck(), Define.UIEvent.PointStay);

            // SetConstants();
            preZoom = zoom;
            Reset();
        }

        // private void SetConstants()
        // {
        //     if (renderImgSize.x / renderImgSize.y > mapImgRect.sizeDelta.x / mapImgRect.sizeDelta.y)
        //     {
        //         // raw img의 가로가 더 긴 상태.
        //         reSizeRatio = renderImgSize.y / mapImgRect.sizeDelta.y;
        //         spCam = new Vector2((renderImgSize.x - mapImgRect.sizeDelta.x) * 0.5f, 0);
        //     }
        //     else
        //     {
        //         // raw img의 세로가 더 긴 상태.
        //         reSizeRatio = renderImgSize.x / mapImgRect.sizeDelta.x;
        //         spCam = new Vector2(0, (renderImgSize.y - mapImgRect.sizeDelta.y) * 0.5f);
        //     }
        //
        //     spScr = mapImgRect.anchoredPosition + screenQuater - mapImgRect.sizeDelta * 0.5f;
        // }

        public override void KeyControl()
        {
            base.KeyControl();
            // TODO: R키 추가 필요
            if (InputManager.GetKeyDown(KeyCode.R))
            {
                Reset();
            }
        }

        protected override void Activated()
        {
            base.Activated();
            // Debug.Log("map activated");
            mapImgRect = Map.instance.SetParent(Get<RectTransform>((int)RectTransforms.MapParent));
            mapImgRect.localPosition = Vector3.zero;
            mapImgRect.anchoredPosition = Vector3.zero;
            mapImgRect.localScale = Vector3.one;
            Map.instance.UpdateToPlayer = false;
        }


        public override void OnOpen()
        {
            base.OnOpen();
            Reset();
        }

        public void Reset()
        {
            if (mapImgRect == null) return;
            zoom = 100f;
            preZoom = zoom;
            mapImgRect.localScale = Vector3.one * (preSize * zoom);
            Map.instance.MoveToPlayer();
            // mapImgRect.anchoredPosition = zeroPos - (Vector2)GameManager.instance.Player.Position * ratio;
            zoomSlider.value = CalCurateSliderValue();
        }

        private float CalCurateSliderValue()
        {
            return (zoom - minZoom) / (maxZoom - minZoom);
        }


        private void OnDrag(PointerEventData action)
        {
            curMousePos = action.position;
            
            deltaMousePos = curMousePos - beginMousePos;
            // mapImgRect.anchoredPosition += deltaMousePos;
            Map.instance.Move(deltaMousePos / (preSize * zoom));
            beginMousePos = curMousePos;
        }

        
        private const string MouseScrollAxis = "Mouse ScrollWheel";
        private readonly Vector3 offset = new Vector3(0, 0, -10);
        private float _wheel;

        private void WheelCheck()
        {
            _wheel = Input.GetAxis(MouseScrollAxis);
            if (Mathf.Abs(_wheel) > 0.01f)
            {
                preZoom = zoom;
                zoom = Mathf.Clamp(zoom + _wheel * wheelSpeed, minZoom, maxZoom);
                ZoomChanged();
                _isDragWithWheel = true;
                zoomSlider.value = CalCurateSliderValue();
            }
        }

        /// <summary>
        /// (movingObj.localPosition * preScale - mouseLocalPosition) * postScale / preScale + mouseLocalPosition
        /// 
        /// </summary>

        private void ZoomChanged(bool isMouse = true)
        {
            // 1️⃣ 마우스 위치를 기준으로 로컬 좌표 변환
            Vector2 mouseLocalPosition;
            if (isMouse)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    mapParent, Input.mousePosition, CameraManager.instance.UICam, out mouseLocalPosition
                );
                // mouseLocalPosition = (Vector2)Input.mousePosition - mapParentScreenPos;
            }
            else
            {
                mouseLocalPosition = Vector2.zero;
            }
            // Debug.Log(mapParentScreenPos);
            // Debug.Log(mouseLocalPosition);
            // Debug.Log(Input.mousePosition);

            float newScale = preSize * zoom;

            // Vector2 targetPosition =
            //     Map.instance.moveRect.anchoredPosition * newScale + (zoom / preZoom - 1) * mouseLocalPosition;

            Vector2 targetPosition =
                Map.instance.moveRect.anchoredPosition + ((1 - zoom / preZoom)/newScale) * mouseLocalPosition;

            // 3️⃣ 이미지 크기 및 위치 적용
            mapImgRect.localScale = Vector3.one * newScale;
            Map.instance.MoveTo(targetPosition);
            
            // //
            // //
            // //
            // // Debug.DrawRay(curMousePos2, (Vector2)mapCamtrans.position - curMousePos2, Color.red, 0.1f);
            // // Debug.Log($"pos: {mapImgRect.anchoredPosition.x}, {mapImgRect.anchoredPosition.y}");
            // Debug.Log($"zoom {mapImgRect.anchoredPosition}, {curMousePos2}");
            // mapImgRect.anchoredPosition = (mapImgRect.anchoredPosition - curMousePos2) * (zoom / preZoom) + curMousePos2;
            // mapImgRect.sizeDelta = Vector3.one * zoom;
            // _isDragWithWheel = true;
            // zoomSlider.value = CalCurateSliderValue();
            // preZoom = zoom;
        }
        
    }
}