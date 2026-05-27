using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSH.Utils.Drag
{
    public class DraggableUIGhost : DraggableBase
    {
        public event Action<DraggableUIGhost, Vector2> OnDropped;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _originAlphaWhileDragging = 0.4f;
        [SerializeField] private float _ghostAlpha = 0.8f;

        private RectTransform _rectTransform;
        private CanvasGroup _originCanvasGroup;
        private Image _sourceImage;

        private GameObject _ghostObject;
        private RectTransform _ghostRectTransform;

        private float _originAlpha = 1f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originCanvasGroup = GetComponent<CanvasGroup>();
            _sourceImage = GetComponent<Image>();

            if (_canvas == null)
            {
                _canvas = GetComponentInParent<Canvas>();
            }

            if (_originCanvasGroup != null)
            {
                _originAlpha = _originCanvasGroup.alpha;
            }
        }

        protected override void BeginDrag(PointerEventData eventData)
        {
            CreateGhost();

            if (_originCanvasGroup != null)
            {
                _originCanvasGroup.alpha = _originAlphaWhileDragging;
                _originCanvasGroup.blocksRaycasts = false;
            }

            MoveGhost(eventData);
        }

        protected override void Drag(PointerEventData eventData)
        {
            MoveGhost(eventData);
        }

        protected override void EndDrag(PointerEventData eventData)
        {
            Vector2 dropPosition = _ghostRectTransform != null
                ? _ghostRectTransform.anchoredPosition
                : _rectTransform.anchoredPosition;

            RestoreOrigin();
            DestroyGhost();

            OnDropped?.Invoke(this, dropPosition);
        }

        private void CreateGhost()
        {
            if (_canvas == null || _sourceImage == null)
            {
                return;
            }

            _ghostObject = new GameObject($"{name}_Ghost", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            _ghostObject.transform.SetParent(_canvas.transform, false);

            Image ghostImage = _ghostObject.GetComponent<Image>();
            ghostImage.sprite = _sourceImage.sprite;
            ghostImage.color = _sourceImage.color;
            ghostImage.raycastTarget = false;
            ghostImage.preserveAspect = _sourceImage.preserveAspect;

            CanvasGroup ghostCanvasGroup = _ghostObject.GetComponent<CanvasGroup>();
            ghostCanvasGroup.alpha = _ghostAlpha;
            ghostCanvasGroup.blocksRaycasts = false;

            _ghostRectTransform = _ghostObject.GetComponent<RectTransform>();
            _ghostRectTransform.sizeDelta = _rectTransform.sizeDelta;
            _ghostRectTransform.pivot = _rectTransform.pivot;
        }

        private void MoveGhost(PointerEventData eventData)
        {
            if (_ghostRectTransform == null || _canvas == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            _ghostRectTransform.anchoredPosition = localPoint;
        }

        private void RestoreOrigin()
        {
            if (_originCanvasGroup == null)
            {
                return;
            }

            _originCanvasGroup.alpha = _originAlpha;
            _originCanvasGroup.blocksRaycasts = true;
        }

        private void DestroyGhost()
        {
            if (_ghostObject == null)
            {
                return;
            }

            Destroy(_ghostObject);
            _ghostObject = null;
            _ghostRectTransform = null;
        }
    }
}