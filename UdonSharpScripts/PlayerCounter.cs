
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
namespace Kurotori
{
    public class PlayerCounter : UdonSharpBehaviour
    {
        [SerializeField]
        TextMeshPro mesh;
        void Start()
        {
            mesh.text = "00";
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            int num = VRCPlayerApi.GetPlayerCount();

            mesh.text = string.Format("{0:D2}", num);
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            int num = VRCPlayerApi.GetPlayerCount();

            mesh.text = string.Format("{0:D2}", num - 1);
        }
    }
}