using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class DisplayMaster : UdonSharpBehaviour
{
    [SerializeField]
    TextMeshPro playerDisplay;

    int masterID = 0;

    void Start()
    {
        
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        var id = VRCPlayerApi.GetPlayerId(player);

        UpdateMaster(id);

        UpdateDisplay();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if(player.playerId == masterID)
        {
            masterID = GetNextMasterID(player.playerId);
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        var player = VRCPlayerApi.GetPlayerById(masterID);

        if (player != null)
        {
            playerDisplay.text = string.Format("{0} : {1}", player.playerId, player.displayName);
        }
    }

    private void UpdateMaster(int id)
    {
        if(masterID == 0)
        {
            // 初回処理
            masterID = id;
        }
        else
        {
            if(id < masterID)
            {
                masterID = id;
            }
        }
    }

    private int GetNextMasterID(int oldMasterID)
    {
        int newMaster = 0;

        VRCPlayerApi[] players = new VRCPlayerApi[80];

        VRCPlayerApi.GetPlayers(players);

        foreach(var player in players)
        {
            if (player == null) continue;

            if(player.playerId != oldMasterID)
            {
                if(newMaster == 0)
                {
                    newMaster = player.playerId;
                }
                else
                {
                    if(player.playerId < newMaster)
                    {
                        newMaster = player.playerId;
                    }
                }
            }
        }

        return newMaster;
    }
}
