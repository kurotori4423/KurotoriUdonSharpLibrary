
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace Kurotori
{
    public class TotalPlayerCounter : UdonSharpBehaviour
    {
        [SerializeField]
        TextMeshPro mesh;

        int count = 0;

        void Start()
        {
            mesh.text = "0";
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (count < player.playerId)
            {
                count = player.playerId;
            }
            mesh.text = count.ToString();
        }
    }
}