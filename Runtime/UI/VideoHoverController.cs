using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;


namespace LSH.Utils
{
    public class VideoHoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private VideoPlayer _videoPlayer;

        private void Start()
        {
            if (_videoPlayer != null)
                _videoPlayer.Pause();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_videoPlayer != null && !_videoPlayer.isPlaying)
                _videoPlayer.Play();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_videoPlayer != null && _videoPlayer.isPlaying)
                _videoPlayer.Pause();
        }
    }
}