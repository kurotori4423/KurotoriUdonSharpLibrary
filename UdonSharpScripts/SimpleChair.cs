
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// シンプルな椅子用スクリプト
/// 最初から非アクティブなVRC Chairにつけておくと、アクティブ時に有効にならない問題を回避できる。
/// </summary>
public class SimpleChair : UdonSharpBehaviour
{
    public override void Interact()
    {
        Networking.LocalPlayer.UseAttachedStation();
    }
}
