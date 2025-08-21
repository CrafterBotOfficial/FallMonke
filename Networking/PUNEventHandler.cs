using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using FallMonke.Networking.EventHandlers;

namespace FallMonke.Networking;

public class PUNEventHandler : IOnEventCallback, IDisposable
{
    private static Dictionary<EventCodesEnum, IEventHandler> eventHandlers = new Dictionary<EventCodesEnum, IEventHandler>()
    {
        { EventCodesEnum.FALL_TILE, new EventHandlers.FallHexagonEventHandler() },
        { EventCodesEnum.SPAWN_PLAYER_ON_RANDOM_TILE, new EventHandlers.SpawnPlayerEventHandler() },
        { EventCodesEnum.SHOW_NOTIFICATION, new EventHandlers.ShowNotificationEventHandler() },
    };

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
            if (CustomGameManager.instance is not CustomGameManager) return;
            if (eventHandlers.TryGetValue((EventCodesEnum)photonEvent.Code, out IEventHandler handler))
            {
                NetPlayer player = NetworkSystem.Instance.GetPlayer(photonEvent.Sender);
                handler.OnEvent(player, photonEvent.CustomData);
            }
        }
        catch (Exception ex)
        {
            Main.Log(ex, BepInEx.Logging.LogLevel.Error);
        }
    }
}
