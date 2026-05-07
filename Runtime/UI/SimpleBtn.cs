using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace LSH.Utils
{
    public class SimpleBtn : MonoBehaviour, IInteractable,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Serializable]
        public struct ButtonColorState
        {
            public Color imageColor;
            public Color outlineColor;

            public void SetNormalState(TextMeshProUGUI text)
            {
                imageColor = Color.white;
                outlineColor = text.fontMaterial.GetColor("_OutlineColor");
            }
        }

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        [SerializeField] private ButtonColorState _highlightedState;
        [SerializeField] private ButtonColorState _pressedState;
        private ButtonColorState _normalState;

        public event Action OnHover;
        public event Action OnClick;

        private void Awake()
        {
            if (_text != null && _text.fontMaterial.HasProperty("_OutlineColor"))
            {
                _text.fontMaterial = new Material(_text.fontMaterial);
                _normalState.SetNormalState(_text);
            }
        }

        private void Start()
        {
            ApplyTransition(_normalState);
        }

        private void ApplyTransition(ButtonColorState state)
        {
            if (_image != null)
                _image.color = state.imageColor;

            if (_text != null && _text.fontMaterial.HasProperty("_OutlineColor"))
                _text.fontMaterial.SetColor("_OutlineColor", state.outlineColor);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ApplyTransition(_highlightedState);
            OnHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ApplyTransition(_normalState);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ApplyTransition(_pressedState);
            OnClick?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.fullyExited)
                ApplyTransition(_normalState);
            else
                ApplyTransition(_highlightedState);
        }
    }
}