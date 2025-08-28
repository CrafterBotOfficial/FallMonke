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
        { EventCodesEnum.REQUEST_TO_START_GAME, new EventHandlers.StartGameRequestHandler() },
        { EventCodesEnum.ELIMINATE_PLAYER, new EventHandlers.EliminatePlayerEventHandler() },
    };

    public PUNEventHandler()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void Dispose()
    {
        Main.Log("Left game, cleaning up event handler");
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        try
        {
            if (photonEvent.Code != PUNBroadcastController.PUN_EVENT_CODE)
                return;

            if (CustomGameManager.instance is not CustomGameManager)
                return;

            if (photonEvent.CustomData is not CustomEventData eventData)
            {
                Main.Log("Malformed custom event", BepInEx.Logging.LogLevel.Warning);
                return;
            }

            if (eventHandlers.TryGetValue((EventCodesEnum)eventData.Code, out IEventHandler handler))
            {
                NetPlayer player = NetworkSystem.Instance.GetPlayer(photonEvent.Sender);
                handler.OnEvent(player, eventData.Data);
            }
        }
        catch (Exception ex)
        {
            Main.Log(ex, BepInEx.Logging.LogLevel.Error);
        }
    }
}
