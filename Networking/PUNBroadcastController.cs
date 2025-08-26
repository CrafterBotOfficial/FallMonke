using System;
using System.Linq;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace FallMonke.Networking;

public class PUNBroadcastController : IBroadcastController
{
    private PUNEventHandler eventHandler;
    private Player[] players;

    public void SetupEventHandler()
    {
        eventHandler = new PUNEventHandler();
        NetworkSystem.Instance.OnPlayerJoined += OnJoin;
        NetworkSystem.Instance.OnPlayerLeft += OnLeave;
    }

    public void FallPlatform(Hexagon.FallableHexagon tile)
    {
        int tileIndex = WorldManager.GetTileIndex(tile);

        var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, /* TargetActors = targets */ };
        PhotonNetwork.RaiseEvent((byte)EventCodesEnum.FALL_TILE,
                                 tileIndex,
                                 raiseEventOptions,
                                 SendOptions.SendReliable);
    }

    public void SendRandomSeed(int random)
    {
        if (!NetworkSystem.Instance.IsMasterClient) return;
        PhotonNetwork.RaiseEvent((byte)EventCodesEnum.SPAWN_PLAYER_ON_RANDOM_TILE,
                                 random,
                                 new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                 SendOptions.SendReliable);
    }

    /// <summary>
    /// Also shows a notification locally, along as the player is master
    /// </summary>
    public void ShowNotification(string message)
    {
        if (!NetworkSystem.Instance.IsMasterClient || message.Length > EventHandlers.ShowNotificationEventHandler.MaxMessageLength) return;
        PhotonNetwork.RaiseEvent((byte)EventCodesEnum.SHOW_NOTIFICATION,
                                 message,
                                 new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                 SendOptions.SendReliable);
    }

    public void RequestStartGame()
    {
        PhotonNetwork.RaiseEvent((byte)EventCodesEnum.REQUEST_TO_START_GAME,
                                 true,
                                 new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                 SendOptions.SendReliable);
    }

    public Participant[] CreateParticipants()
    {
        return GetPlayersQuery().Select(player => CreateParticipant(player))
                                .ToArray();
    }

    public int PlayersWithModCount()
    {
        return GetPlayers().Count();
    }

    private Player[] GetPlayers()
    {
        if (players.IsNullOrEmpty() || players.Length == 1)
        {
            players = GetPlayersQuery().ToArray();
        }
        return players;
    }

    private IEnumerable<Player> GetPlayersQuery()
    {
        var query = (
            from player in PhotonNetwork.PlayerList
            where player.ActorNumber != -1
            where player.CustomProperties.ContainsKey(CustomGameManager.MOD_KEY)
            select player
        );
        return query;
    }

    private Participant CreateParticipant(Player player)
    {
        var participant = new Participant(player);
        var rig = ((CustomGameManager)CustomGameManager.instance).FindPlayerVRRig(player);
        participant.Manager = rig.AddComponent<ParticipantManager>();
        participant.Manager.Rig = rig;
        participant.Manager.Info = participant;
        return participant;
    }

    public void MakeModIdentifable()
    {
        ExitGames.Client.Photon.Hashtable properties = new() { { CustomGameManager.MOD_KEY, true } };
        Photon.Pun.PhotonNetwork.SetPlayerCustomProperties(properties);
    }

    public void OnJoin(NetPlayer player)
    {
        try
        {
            players = GetPlayersQuery().ToArray();
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
            players = GetPlayersQuery().ToArray();
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
