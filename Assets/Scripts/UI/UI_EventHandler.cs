using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Default
{
    public class UI_EventHandler : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IDropHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<PointerEventData> OnClickHandler = null;
        public Action<PointerEventData> OnBeginDragHandler = null;
        public Action<PointerEventData> OnDragHandler = null;
        public Action<PointerEventData> OnEndDragHandler = null;
        public Action<PointerEventData> OnDropHandler = null;
        public Action<PointerEventData> OnPointerDownHandler = null;
        public Action<PointerEventData> OnPointerUpHandler = null;
        public Action<PointerEventData> OnPointerEnterHandler = null;
        public Action<PointerEventData> OnPointerExitHandler = null;
        public Action<PointerEventData> OnPointerStayHandler = null;

        PointerEventData ev = null;
        bool isPointerOver = false;
        private void Awake()
        {
            isPointerOver = false;
        }
        private void OnEnable()
        {
            isPointerOver = false;

        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickHandler?.Invoke(eventData);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragHandler?.Invoke(eventData);
        }
        
        
        public void OnDrag(PointerEventData eventData)
        {
            OnDragHandler?.Invoke(eventData);
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragHandler?.Invoke(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnDropHandler?.Invoke(eventData);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownHandler?.Invoke(eventData);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpHandler?.Invoke(eventData);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterHandler?.Invoke(eventData);
            isPointerOver = true;
            ev = eventData;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitHandler?.Invoke(eventData);
            isPointerOver = false;
        }

        private void Update()
        {
            if (isPointerOver)
            {
                OnPointerStayHandler?.Invoke(ev);
            }
        }
    }
}