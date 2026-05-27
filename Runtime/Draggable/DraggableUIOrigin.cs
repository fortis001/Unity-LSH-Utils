using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSH.Utils.Drag
{
    public class DraggableUIOrigin : DraggableBase
    {
        public event Action<DraggableUIOrigin, Vector2> OnDropped;

        private RectTransform _rectTransform;
        private Vector2 _originPosition;
        private Vector2 _dragOffset;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void BeginDrag(PointerEventData eventData)
        {
            _originPosition = _rectTransform.anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            _dragOffset = _rectTransform.anchoredPosition - localPoint;
        }

        protected override void Drag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
            {
                _rectTransform.anchoredPosition = localPoint + _dragOffset;
            }
        }

        protected override void EndDrag(PointerEventData eventData)
        {
            OnDropped?.Invoke(this, _rectTransform.anchoredPosition);
        }

        public void ReturnToOrigin()
        {
            _rectTransform.anchoredPosition = _originPosition;
        }

        public void SnapTo(Vector2 anchoredPosition)
        {
            _rectTransform.anchoredPosition = anchoredPosition;
        }
    }
}