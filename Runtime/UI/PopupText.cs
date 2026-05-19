using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace LSH.Utils.UI
{
    public class PopupText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private Color _defaultColor = Color.white;

        private Coroutine _playCoroutine;
        private Action<PopupText> _onFinished;

        public void Activate(string text, Action<PopupText> onFinished)
        {
            Activate(text, _defaultColor, onFinished);
        }

        public void Activate(int value, Action<PopupText> onFinished)
        {
            Activate($"+{value}", _defaultColor, onFinished);
        }

        public void Activate(string text, Color color, Action<PopupText> onFinished)
        {
            if (_text != null)
            {
                _text.text = text;
                _text.color = color;
            }

            _onFinished = onFinished;
            gameObject.SetActive(true);

            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
            }

            _playCoroutine = StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            if (_animation != null && _showClip != null)
            {
                _animation.clip = _showClip;
                _animation.Play();

                yield return new WaitForSeconds(_showClip.length);
            }

            _playCoroutine = null;
            ResetColor();
            _onFinished?.Invoke(this);
        }

        private void ResetColor()
        {
            if (_text != null)
            {
                _text.color = _defaultColor;
            }
        }
    }
}