using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace FallMonke.Networking;

public class PUNBroadcastController : IBroadcastController
{
    private PUNEventHandler eventHandler;

    public void SetupEventHandler()
    {
        eventHandler = new PUNEventHandler();
    }

    public void FallPlatform(Hexagon.FallableHexagon tile)
    {
        int tileIndex = WorldManager.GetTileIndex(tile);

        var targets = ((CustomGameManager)CustomGameManager.instance).ParticipantActorNumbers;
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

    public Participant[] CreateParticipants()
    {
        var query = (
            from player in PhotonNetwork.PlayerList
            where player.ActorNumber != -1
            where player.CustomProperties.ContainsKey(CustomGameManager.MOD_KEY)
            select CreateParticipant(player)
        );
        return query.ToArray();
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

    public void Cleanup()
    {
        eventHandler.Dispose();
    }
}
