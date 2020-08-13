using UnityEngine;
using UnityEngine.EventSystems;

namespace VRCorp.SoundsSystem.Scripts
{
    public class ButtonSoundsComponent : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public AudioClip onEnterSound;
        public AudioClip onClickSound;
        private AudioSource _audioSource;

        private void Awake()
        {
            if (onEnterSound == null || onClickSound == null)
            {
                Debug.LogWarning("Audioclips not found");
            }

            _audioSource = GameObject.Find("MainAudioSource").GetComponent<AudioSource>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioSource.clip = onEnterSound;
            _audioSource.Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _audioSource.clip = onClickSound;
            _audioSource.Play();
        }
    }
}
