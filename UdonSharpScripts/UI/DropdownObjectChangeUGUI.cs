
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
namespace Kurotori
{
    public class DropdownObjectChangeUGUI : UdonSharpBehaviour
    {
        [SerializeField] Dropdown dropdown;
        [SerializeField] GameObject[] gameObjects;

        void Start()
        {
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                gameObjects[i].SetActive(dropdown.value == i);
            }
        }

        public void OnDropdownChange()
        {
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                gameObjects[i].SetActive(dropdown.value == i);
            }
        }
    }
}