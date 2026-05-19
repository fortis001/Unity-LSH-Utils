using System.Collections.Generic;
using UnityEngine;

namespace LSH.Utils.UI
{
    public class PopupTextPool
    {
        private readonly PopupText _popupPrefab;
        private readonly Transform _poolRoot;
        private readonly Queue<PopupText> _pool = new();

        public PopupTextPool(PopupText prefab, Transform root)
        {
            _popupPrefab = prefab;
            _poolRoot = root;
        }

        public void Init(int prewarmCount = 3)
        {
            Prewarm(prewarmCount);
        }

        public PopupText Get()
        {
            PopupText popupText = _pool.Count > 0
                ? _pool.Dequeue()
                : CreateNew();

            popupText.transform.SetParent(_poolRoot);
            return popupText;
        }

        public void Release(PopupText popupText)
        {
            if (popupText == null)
                return;

            popupText.gameObject.SetActive(false);
            popupText.transform.SetParent(_poolRoot);

            _pool.Enqueue(popupText);
        }

        private void Prewarm(int prewarmCount)
        {
            for (int i = 0; i < prewarmCount; i++)
            {
                PopupText popupText = CreateNew();
                popupText.gameObject.SetActive(false);
                _pool.Enqueue(popupText);
            }
        }

        private PopupText CreateNew()
        {
            return Object.Instantiate(_popupPrefab, _poolRoot);
        }
    }
}