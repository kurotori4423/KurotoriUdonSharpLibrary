
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 半同期型ミュージックプレイヤー
/// 
/// 使い方
/// Play, PlayNext, StopをSendCustomEventで呼び出す。
/// 
/// 注意点
/// マスターの切り替わり時に曲が更新された場合壊れる可能性あり
/// </summary>
namespace Kurotori
{
    public class MusicPlayer : UdonSharpBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        AudioClip[] clips;

        [UdonSynced(UdonSyncMode.None)]
        int playIndex;

        [UdonSynced(UdonSyncMode.None)]
        int state; // 0: stop 1:ready 2:play

        // for client

        private bool isReady = false;

        private bool compliteInitSync = false;

        // for Owner
        private int playerVote = 0;

        private void Start()
        {
        }

        // 同期方法
        // 曲変更時の準備状態で同期変数を数フレームで変更するとその変化をクライアントが受信できない。
        // そこで、値の更新を受け取ったかどうか、クライアントから返答してもらい(ReadyVote)
        // オーナーは全員から返事が返ったか確認(CheckVote)してから値を変更している。

        private void Update()
        {
            if (Networking.LocalPlayer.IsOwner(gameObject))
            {
                UpdateOwner();
            }
            else
            {
                UpdateClient();
            }
        }

        private void UpdateClient()
        {
            if (audioSource.isPlaying)
            {
                switch (state)
                {
                    case 0: // Stop
                        audioSource.Stop();
                        Debug.LogError("[L]Music Stop:" + playIndex);
                        isReady = false;
                        break;
                    case 1: // Ready
                        if (!isReady)
                        {
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ReadyVote");
                            Debug.LogError("Vote.");
                            audioSource.clip = clips[playIndex];
                            audioSource.Play();
                            Debug.LogError("[L]Music Play:" + playIndex);
                            isReady = true;
                        }
                        break;
                    case 2: // Play
                        isReady = false;
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case 0: // Stop
                        audioSource.Stop();
                        Debug.LogError("[L]Music Stop:" + playIndex);
                        isReady = false;
                        break;
                    case 1: // Ready
                        if (!isReady)
                        {
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ReadyVote");
                            Debug.LogError("Vote.");
                            audioSource.clip = clips[playIndex];
                            audioSource.Play();
                            Debug.LogError("[L]Music Play:" + playIndex);
                            isReady = true;
                        }
                        break;
                    case 2: // Play
                            // 曲が終了しているが、再生状態
                        isReady = false;
                        Debug.LogError("Client Stop Play" + playIndex);
                        if (!compliteInitSync)
                        {
                            audioSource.clip = clips[playIndex];
                            audioSource.Play();
                            compliteInitSync = true;
                        }
                        break;
                }
            }
        }

        private void UpdateOwner()
        {
            if (audioSource.isPlaying)
            {
                switch (state)
                {
                    case 0: // Stop
                        audioSource.Stop();
                        Debug.LogError("[L]Music Stop:" + playIndex);
                        playerVote = 0;
                        break;
                    case 1: // Ready
                        if (CheckVote())
                        {
                            Debug.LogError("Client Vote Complite.");
                            audioSource.clip = clips[playIndex];
                            audioSource.Play();

                            state = 2;
                        }
                        break;
                    case 2: // Play
                        playerVote = 0;
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case 0: // Stop
                        playerVote = 0;
                        break;
                    case 1: // Ready
                        if (CheckVote())
                        {
                            Debug.LogError("Client Vote Complite.");
                            audioSource.clip = clips[playIndex];
                            audioSource.Play();

                            state = 2;
                        }
                        break;
                    case 2: // Play
                            // 曲が終了した状態
                        Debug.LogError("[L]Music End:" + playIndex);
                        PlayNextOwner();
                        playerVote = 0;
                        break;

                }
            }
        }

        public void ReadyVote()
        {
            playerVote += 1;
        }

        private bool CheckVote()
        {
            var clientCount = VRCPlayerApi.GetPlayerCount() - 1;

            if (playerVote == clientCount)
                return true;

            return false;
        }

        public void Play()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "PlayOwner");
        }

        public void PlayOwner()
        {
            Debug.LogError("PlayOwner");
            switch (state)
            {
                case 0: // Stop
                    state = 1;
                    Debug.LogError("ChangeState:Ready");
                    break;
                case 1: // Next
                    break;
                case 2: // Play
                    break;
            }
        }

        public void PlayNext()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "PlayNextOwner");
        }

        public void PlayNextOwner()
        {
            Debug.LogError("PlayNextOwner");
            playIndex = (playIndex + 1) % clips.Length;
            state = 1;

            Debug.LogError("ChangeState:Next");
            Debug.LogError("Next is" + playIndex);
        }

        public void Stop()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "StopOwner");
        }

        public void StopOwner()
        {
            Debug.LogError("StopOwner");
            switch (state)
            {
                case 0: // Stop
                    break;
                case 1: // Next
                    state = 0;
                    Debug.LogError("ChangeState:Stop");
                    break;
                case 2: // Play
                    state = 0;
                    Debug.LogError("ChangeState:Stop");
                    break;
            }
        }
    }
}