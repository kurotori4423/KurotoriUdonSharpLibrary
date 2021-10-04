﻿
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Kurotori
{
    public class ToggleObjectUGUI : UdonSharpBehaviour
    {
        [SerializeField] Toggle toggleObject;
		[SerializeField] GameObject[] gameObjects;
		
        void Start()
	    {
	        foreach(var go in gameObjects)
	        {
	            go.SetActive(toggleObject.isOn);
	        }
	    }
	
	    public void OnToggleChange()
	    {
	        foreach (var go in gameObjects)
	        {
	            go.SetActive(toggleObject.isOn);
	        }
	    }
    }
}