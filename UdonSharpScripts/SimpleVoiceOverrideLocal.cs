
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
namespace Kurotori
{
    public class SimpleVoiceOverrideLocal : UdonSharpBehaviour
    {
        [SerializeField]
        private Slider farSlider;

        [SerializeField]
        private Text farDistanceText;

        void Start()
        {
            if (farDistanceText != null)
            {
                farDistanceText.text = string.Format("{0:0.00}", farSlider.value);
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            player.SetVoiceDistanceFar(farSlider.value);
        }

        public void ChangeDistance()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[80];
            VRCPlayerApi.GetPlayers(players);
            foreach (var player in players)
            {
                if (player == null) continue;

                player.SetVoiceDistanceFar(farSlider.value);
            }

            if (farDistanceText != null)
            {
                farDistanceText.text = string.Format("{0:0.00}", farSlider.value);
            }
        }
    }
}
