using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace FallMonke.Networking;

public class PUNEventHandler : IOnEventCallback, IDisposable
{
    public PUNEventHandler()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    // ~PUNEventHandler()
    public void Dispose()
    {
        Main.Log("Left game, cleaning up event handler");
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        try
        {
            if (photonEvent.Code != (byte)EventCodesEnum.FALL_TILE)
                return;

            Main.Log("got event");
            
            if (CustomGameManager.Instance is not CustomGameManager manager)
                return;

            if (manager.CurrentState != GameState.GameStateEnum.GameOn)
                return;

            if (photonEvent.CustomData is not int tileIndex)
            {
                Main.Log("Bad event, not expected data type", BepInEx.Logging.LogLevel.Warning);
                return;
            }

            NetPlayer player = NetworkSystem.Instance.GetPlayer(photonEvent.Sender);
            if (player == null || player.IsLocal)
            {
                Main.Log("Bad event", BepInEx.Logging.LogLevel.Warning);
                return;
            }

            var tile = WorldManager.GetTileByIndex(tileIndex);

            VRRig rig = manager.FindPlayerVRRig(player);
            const float maxDistance = 10;
            if (Vector3.Distance(rig.transform.position, tile.transform.position) > maxDistance)
            {
                Main.Log("Tile to fare away, likely a cheater.", BepInEx.Logging.LogLevel.Warning);
                return;
            }

            Main.Log("Falling tile, told to from other player", BepInEx.Logging.LogLevel.Debug);
            tile.Fall();
        }
        catch (Exception ex)
        {
            Main.Log(ex, BepInEx.Logging.LogLevel.Error);
        }
    }
}
