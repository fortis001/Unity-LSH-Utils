using UnityEngine;
using UnityEngine.EventSystems;

namespace LSH.Utils.Drag
{
    public abstract class DraggableBase : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private bool _isDraggable = true;

        public bool IsDragging { get; private set; }

        public void SetDraggable(bool isDraggable)
        {
            _isDraggable = isDraggable;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isDraggable)
            {
                return;
            }

            IsDragging = true;
            BeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            Drag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            IsDragging = false;
            EndDrag(eventData);
        }

        protected abstract void BeginDrag(PointerEventData eventData);
        protected abstract void Drag(PointerEventData eventData);
        protected abstract void EndDrag(PointerEventData eventData);
    }
}