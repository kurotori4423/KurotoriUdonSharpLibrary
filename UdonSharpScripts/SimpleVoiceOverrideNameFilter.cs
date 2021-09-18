
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Kurotori
{
    public class SimpleVoiceOverrideNameFilter : UdonSharpBehaviour
    {
        [UdonSynced(UdonSyncMode.None)]
        private float voiceDistanceFar = 25.0f;

        [SerializeField]
        private Slider farSlider;

        [SerializeField]
        private string ownerName;

        [SerializeField]
        private Text farDistanceText;

        [SerializeField]
        private int delayFrame = 60;

        private int delayCounter = 0;

        private bool firstSync = true;

        void Start()
        {
            if (farDistanceText != null)
            {
                farDistanceText.text = string.Format("{0:0.00}", farSlider.value);
            }
        }

        private void Update()
        {
            if (firstSync && delayCounter < delayFrame)
            {
                delayCounter++;
            }
            else
            {
                ChangeDistanceSync();
                firstSync = false;
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            player.SetVoiceDistanceFar(voiceDistanceFar);

            if (player.Equals(Networking.LocalPlayer))
            {
                if (!Networking.LocalPlayer.IsOwner(gameObject))
                {
                    farSlider.interactable = false;

                    if (Networking.LocalPlayer.displayName.Equals(ownerName))
                    {
                        Networking.SetOwner(Networking.LocalPlayer, gameObject);
                        farSlider.interactable = true;
                    }
                }
                else
                {
                    if (!Networking.LocalPlayer.displayName.Equals(ownerName))
                    {
                        farSlider.interactable = false;
                    }
                }
            }
        }

        public void ChangeDistance()
        {
            voiceDistanceFar = farSlider.value;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeDistanceSync");

            ChangeDistanceSync();
        }

        public void ChangeDistanceSync()
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
            {
                farSlider.value = voiceDistanceFar;
            }

            VRCPlayerApi[] players = new VRCPlayerApi[80];
            VRCPlayerApi.GetPlayers(players);
            foreach (var player in players)
            {
                if (player == null) continue;

                player.SetVoiceDistanceFar(voiceDistanceFar);
            }

            if (farDistanceText != null)
            {
                farDistanceText.text = string.Format("{0:0.00}", voiceDistanceFar);
            }
        }
    }
}