using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LSH.Utils.Drag
{
    [RequireComponent(typeof(Collider2D))]
    public class Draggable2DObjectGhost : DraggableBase
    {
        public event Action<Draggable2DObjectGhost, Vector2> OnDropped;

        [SerializeField] private Camera _camera;

        [SerializeField] private GameObject _ghostPrefab;
        [SerializeField] private Transform _targetRoot;
        [SerializeField] private Transform _visualRoot;

        [SerializeField] private float _originAlphaWhileDragging = 0.4f;
        [SerializeField] private float _ghostAlpha = 0.8f;

        private Vector3 _originPosition;
        private GameObject _ghostObject;

        private SpriteRenderer[] _originRenderers;
        private Color[] _originColors;

        private Transform TargetRoot => _targetRoot != null ? _targetRoot : transform;
        private Transform VisualRoot => _visualRoot != null ? _visualRoot : transform;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            _originRenderers = VisualRoot.GetComponentsInChildren<SpriteRenderer>();
            _originColors = new Color[_originRenderers.Length];

            for (int i = 0; i < _originRenderers.Length; i++)
            {
                _originColors[i] = _originRenderers[i].color;
            }
        }

        protected override void BeginDrag(PointerEventData eventData)
        {
            _originPosition = TargetRoot.position;

            CreateGhost();
            SetOriginAlpha(_originAlphaWhileDragging);
            MoveGhost(eventData);
        }

        protected override void Drag(PointerEventData eventData)
        {
            MoveGhost(eventData);
        }

        protected override void EndDrag(PointerEventData eventData)
        {
            Vector2 dropPosition = _ghostObject != null
                ? _ghostObject.transform.position
                : TargetRoot.position;

            RestoreOriginAlpha();
            DestroyGhost();

            OnDropped?.Invoke(this, dropPosition);
        }

        private void CreateGhost()
        {
            if (_ghostPrefab != null)
            {
                _ghostObject = Instantiate(
                    _ghostPrefab,
                    TargetRoot.position,
                    TargetRoot.rotation
                );

                return;
            }

            CreateGhostFromSpriteRenderers();
        }

        private void CreateGhostFromSpriteRenderers()
        {
            if (_originRenderers == null || _originRenderers.Length == 0)
            {
                return;
            }

            _ghostObject = new GameObject($"{name}_Ghost");
            _ghostObject.transform.SetPositionAndRotation(
                TargetRoot.position,
                TargetRoot.rotation
            );

            _ghostObject.transform.localScale = Vector3.one;

            for (int i = 0; i < _originRenderers.Length; i++)
            {
                SpriteRenderer originRenderer = _originRenderers[i];

                if (originRenderer == null || originRenderer.sprite == null)
                {
                    continue;
                }

                GameObject ghostRendererObject = new GameObject($"{originRenderer.gameObject.name}_Ghost");

                ghostRendererObject.transform.SetPositionAndRotation(
                    originRenderer.transform.position,
                    originRenderer.transform.rotation
                );

                ghostRendererObject.transform.localScale = originRenderer.transform.lossyScale;

                ghostRendererObject.transform.SetParent(_ghostObject.transform, true);

                SpriteRenderer ghostRenderer = ghostRendererObject.AddComponent<SpriteRenderer>();

                ghostRenderer.sprite = originRenderer.sprite;
                ghostRenderer.flipX = originRenderer.flipX;
                ghostRenderer.flipY = originRenderer.flipY;
                ghostRenderer.drawMode = originRenderer.drawMode;
                ghostRenderer.size = originRenderer.size;
                ghostRenderer.tileMode = originRenderer.tileMode;
                ghostRenderer.sortingLayerID = originRenderer.sortingLayerID;
                ghostRenderer.sortingOrder = originRenderer.sortingOrder;

                Color color = originRenderer.color;
                color.a *= _ghostAlpha;
                ghostRenderer.color = color;

                ghostRenderer.sharedMaterial = originRenderer.sharedMaterial;
            }
        }

        private void MoveGhost(PointerEventData eventData)
        {
            if (_ghostObject == null)
            {
                return;
            }

            _ghostObject.transform.position = GetWorldPosition(
                eventData.position,
                TargetRoot.position.z
            );
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

        private void SetOriginAlpha(float alpha)
        {
            if (_originRenderers == null)
            {
                return;
            }

            for (int i = 0; i < _originRenderers.Length; i++)
            {
                if (_originRenderers[i] == null)
                {
                    continue;
                }

                Color color = _originRenderers[i].color;
                color.a = alpha;
                _originRenderers[i].color = color;
            }
        }

        private void RestoreOriginAlpha()
        {
            if (_originRenderers == null || _originColors == null)
            {
                return;
            }

            for (int i = 0; i < _originRenderers.Length; i++)
            {
                if (_originRenderers[i] == null)
                {
                    continue;
                }

                _originRenderers[i].color = _originColors[i];
            }
        }

        private void DestroyGhost()
        {
            if (_ghostObject == null)
            {
                return;
            }

            Destroy(_ghostObject);
            _ghostObject = null;
        }

        public void SnapOriginTo(Vector2 worldPosition)
        {
            TargetRoot.position = new Vector3(
                worldPosition.x,
                worldPosition.y,
                TargetRoot.position.z
            );
        }

        public void ReturnOriginToStart()
        {
            TargetRoot.position = _originPosition;
        }
    }
}