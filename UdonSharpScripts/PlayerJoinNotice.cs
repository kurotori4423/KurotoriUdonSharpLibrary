
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// プレイヤー入室サウンド
/// </summary>
public class PlayerJoinNotice : UdonSharpBehaviour
{
    [SerializeField]
    AudioClip joinSound;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    float COOL_TIME = 1.0f;

    bool isCoolTime = false;
    float nowCoolTime;

    void Start()
    {
        nowCoolTime = COOL_TIME;
    }

    private void Update()
    {
        if (isCoolTime)
        {
            nowCoolTime -= Time.deltaTime;

            if (nowCoolTime < 0.0f)
            {
                nowCoolTime = COOL_TIME;
                isCoolTime = false;
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (!isCoolTime)
        {
            audioSource.PlayOneShot(joinSound);
            isCoolTime = true;
        }
    }
}
