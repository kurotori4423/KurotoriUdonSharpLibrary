
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using System;

public class SimpleTMPClock : UdonSharpBehaviour
{
    [SerializeField]
    TextMeshPro dateDisplay;
    [SerializeField]
    TextMeshPro clockDisplay;

    void Start()
    {
        
    }

    private void Update()
    {
        dateDisplay.text = DateTime.Now.ToLongDateString();
        clockDisplay.text = DateTime.Now.ToLongTimeString();
    }
}
