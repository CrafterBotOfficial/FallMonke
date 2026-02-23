using System;
using System.Linq;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace FallMonke.Networking;

public class PUNNetworkController : INetworkController 
{
    private PUNEventHandler eventHandler;
    private Player[] players;

    public void SetupEventHandler()
    {
        eventHandler ??= new PUNEventHandler();
        NetworkSystem.Instance.OnPlayerJoined += OnJoin;
        NetworkSystem.Instance.OnPlayerLeft += OnLeave;
    }

    public void FallPlatform(Hexagon.FallableHexagon tile)
    {
        int tileIndex = WorldManager.Instance.GetTileIndex(tile);
        SendEvent(EventCodesEnum.FALL_TILE, tileIndex, ReceiverGroup.Others);
    }

    public void SendTeleportToGame(int random)
    {
        if (NetworkSystem.Instance.IsMasterClient)
        {
            SendEvent(EventCodesEnum.SPAWN_PLAYER_ON_RANDOM_TILE, random);
        }
    }

    /// <summary>
    /// Also shows a notification locally, along as the player is master
    /// </summary>
    public void ShowNotification(string message)
    {
        if (NetworkSystem.Instance.IsMasterClient && message.Length < EventHandlers.ShowNotificationEventHandler.MaxMessageLength)
        {
            SendEvent(EventCodesEnum.SHOW_NOTIFICATION, message);
        }
    }

    public void RequestStartGame()
    {
        SendEvent(EventCodesEnum.REQUEST_TO_START_GAME);
    }

    public void SendReportElimination()
    {
        SendEvent(EventCodesEnum.ELIMINATE_PLAYER);
    }

    public Participant[] CreateParticipants()
    {
        return [..GetPlayersQuery().Select(CreateParticipant)];
    }

    private void SendEvent(EventCodesEnum code)
    {
        SendEvent(code, true);
    }

    private void SendEvent(EventCodesEnum code, object content, ReceiverGroup receivers = ReceiverGroup.All)
    {
        int[] targets = ((CustomGameManager)GorillaGameManager.instance).PlayerIDs;
        PhotonNetwork.RaiseEvent((byte)code,
                                 content,
                                 new RaiseEventOptions { TargetActors = targets, Receivers = receivers },
                                 SendOptions.SendReliable);
    }

    public int PlayersWithModCount()
    {
        return GetPlayers().Count();
    }

    private Player[] GetPlayers()
    {
        if (players.IsNullOrEmpty() || players.Length == 1)
        {
            players = [..GetPlayersQuery()];
        }
        return players;
    }

    private IEnumerable<Player> GetPlayersQuery()
    {
        var query =
            from player in PhotonNetwork.PlayerList
            where player.ActorNumber != -1
            where player.CustomProperties.ContainsKey(CustomGameManager.MOD_KEY)
            select player;
        return query;
    }

    private Participant CreateParticipant(Player player)
    {
        var participant = new Participant(player);
        var rig = ((CustomGameManager)GorillaGameManager.instance).FindPlayerVRRig(player);
        participant.Manager = player.IsLocal ? rig.AddComponent<LocalPlayerManager>() : rig.AddComponent<ParticipantManager>();
        participant.Manager.Rig = rig;
        participant.Manager.Info = participant;
        return participant;
    }

    public void MakeModIdentifable()
    {
        string modVersion = Main.Instance.Info.Metadata.Version.ToString();
        Hashtable properties = new() { { CustomGameManager.MOD_KEY, modVersion } };
        if (properties != null)
        {
            PhotonNetwork.SetPlayerCustomProperties(properties);
            return;
        }
        Main.Log("Failed to set player properties, will not be allowed to participate in games", BepInEx.Logging.LogLevel.Error);
    }

    public void OnJoin(NetPlayer player)
    {
        try
        {
            players = [..GetPlayersQuery()];
        }
        catch (Exception ex)
        {
            Main.Log(ex, BepInEx.Logging.LogLevel.Error);
        }
    }

    public void OnLeave(NetPlayer leavePlayer)
    {
        try
        {
            players = [..GetPlayersQuery()];
        }
        catch (Exception ex)
        {
            Main.Log(ex, BepInEx.Logging.LogLevel.Error);
        }
    }

    public void Cleanup()
    {
        NetworkSystem.Instance.OnPlayerJoined -= OnJoin;
        NetworkSystem.Instance.OnPlayerLeft -= OnLeave;
        eventHandler.Dispose();
    }
}
