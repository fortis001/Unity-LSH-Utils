using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LSH.Utils.Drag
{
    [RequireComponent(typeof(Collider2D))]
    public class Draggable2DObjectOrigin : DraggableBase
    {
        public event Action<Draggable2DObjectOrigin, Vector2> OnDropped;

        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _targetRoot;

        private Vector3 _originPosition;
        private Vector3 _dragOffset;

        private Transform TargetRoot => _targetRoot != null ? _targetRoot : transform;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        protected override void BeginDrag(PointerEventData eventData)
        {
            _originPosition = TargetRoot.position;

            Vector3 pointerWorldPosition = GetWorldPosition(eventData.position, TargetRoot.position.z);
            _dragOffset = TargetRoot.position - pointerWorldPosition;
        }

        protected override void Drag(PointerEventData eventData)
        {
            TargetRoot.position = GetWorldPosition(eventData.position, TargetRoot.position.z) + _dragOffset;
        }

        protected override void EndDrag(PointerEventData eventData)
        {
            OnDropped?.Invoke(this, TargetRoot.position);
        }

        private Vector3 GetWorldPosition(Vector2 screenPosition, float zPosition)
        {
            if (_camera == null)
            {
                return TargetRoot.position;
            }

            Vector3 position = screenPosition;
            position.z = Mathf.Abs(_camera.transform.position.z - zPosition);

            Vector3 worldPosition = _camera.ScreenToWorldPoint(position);
            worldPosition.z = zPosition;

            return worldPosition;
        }

        public void SnapTo(Vector2 worldPosition)
        {
            TargetRoot.position = new Vector3(
                worldPosition.x,
                worldPosition.y,
                TargetRoot.position.z
            );
        }

        public void ReturnToOrigin()
        {
            TargetRoot.position = _originPosition;
        }
    }
}