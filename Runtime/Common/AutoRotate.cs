using UnityEngine;


namespace LSH.Utils
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed = 180f;
        [SerializeField] private bool _useUnscaledTime = false;

        public void SetSpeed(float speed) => _rotateSpeed = speed;

        private void Update()
        {
            float delta = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            transform.Rotate(0f, 0f, _rotateSpeed * delta);
        }
    }
}
