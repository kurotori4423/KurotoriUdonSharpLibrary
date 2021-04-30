
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 同期なしシンプルミュージックプレイヤー
/// 自動でループ再生する。
/// </summary>
public class SimpleMusicPlayer : UdonSharpBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip[] clips;

    [SerializeField]
    bool isPlay = true;

    int playIndex = 0;

    void Start()
    {
        if (isPlay)
        {
            audioSource.Stop();
            audioSource.clip = clips[playIndex];
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (isPlay)
        {
            if (!audioSource.isPlaying)
            {
                playIndex = (playIndex + 1) % clips.Length;
                audioSource.clip = clips[playIndex];
                audioSource.Play();
            }
        }
        else
        {
            if(audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void Play()
    {
        if(!audioSource.isPlaying)
        {
            isPlay = true;
            audioSource.Stop();
            audioSource.Play();
        }
    }

    public void Stop()
    {
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
            isPlay = false;
        }
    }

}
