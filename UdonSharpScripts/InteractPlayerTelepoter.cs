
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace Kurotori
{
    public class InteractPlayerTelepoter : UdonSharpBehaviour
    {
        public Transform teleportTo;

        public override void Interact()
        {
            var player = Networking.LocalPlayer;

            if (player != null)
            {
                player.TeleportTo(teleportTo.position, teleportTo.rotation);
            }
        }
    }
}