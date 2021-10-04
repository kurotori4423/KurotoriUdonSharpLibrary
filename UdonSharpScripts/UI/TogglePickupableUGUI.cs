
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
namespace Kurotori
{
    public class TogglePickupableUGUI : UdonSharpBehaviour
    {
        public Toggle toggle;

        public VRC_Pickup[] pickupObjects;

        void Start()
        {
            for (int i = 0; i < pickupObjects.Length; ++i)
            {
                pickupObjects[i].pickupable = toggle.isOn;
            }
        }

        public void OnToggleChange()
        {
            for (int i = 0; i < pickupObjects.Length; ++i)
            {
                pickupObjects[i].pickupable = toggle.isOn;
            }
        }
    }
}