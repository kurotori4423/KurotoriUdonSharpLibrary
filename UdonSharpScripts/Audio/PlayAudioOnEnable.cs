
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// オブジェクトがアクティブになった時に一回音声を再生する
/// （2021/03/03現在 エディタ上では最初から非アクティブのオブジェクトでは動きません。）
/// </summary>
public class PlayAudioOnEnable : UdonSharpBehaviour
{
    [SerializeField]
    AudioSource audioSorce;

    private void OnEnable()
    {
        audioSorce.Play();
    }
}
