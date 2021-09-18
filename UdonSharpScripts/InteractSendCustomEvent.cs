
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace Kurotori
{
    public class InteractSendCustomEvent : UdonSharpBehaviour
    {
        [SerializeField]
        string customEventName;
        [SerializeField]
        UdonBehaviour behaviour;


        public override void Interact()
        {
            behaviour.SendCustomEvent(customEventName);
        }
    }
}