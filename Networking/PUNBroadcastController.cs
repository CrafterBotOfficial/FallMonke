using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace FallMonke.Networking;

public class PUNBroadcastController : IBroadcastController
{
    public void FallPlatform(Hexagon.FallableHexagon tile)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int tileIndex = 0;

        var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EventCodesEnum.FALL_TILE,
                                 tileIndex,
                                 raiseEventOptions,
                                 SendOptions.SendReliable);
    }

    public Participant[] CreateParticipants()
    {
        var query = (
            from player in PhotonNetwork.PlayerList
            where player.ActorNumber != -1
            where PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber).CustomProperties.ContainsKey(CustomGameManager.MOD_KEY)
            select CreateParticipant(player)
        );
        return query.ToArray();
    }

    private Participant CreateParticipant(Player player)
    {
        var participant = new Participant(player);
        var rig = CustomGameManager.Instance.FindPlayerVRRig(player);
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
}
