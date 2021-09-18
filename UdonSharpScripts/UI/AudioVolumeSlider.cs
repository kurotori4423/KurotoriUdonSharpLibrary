
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Kurotori
{
    public class AudioVolumeSlider : UdonSharpBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        Slider slider;

        [SerializeField]
        Text volumeDisplay;

        public void Start()
        {
            audioSource.volume = slider.value;
        }

        public void ChangeVolume()
        {
            var volume = slider.value;
            audioSource.volume = volume;

            if (volumeDisplay != null)
            {
                volumeDisplay.text = string.Format("{0:0.0}", volume);
            }
        }

    }
}