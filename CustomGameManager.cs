using FallMonke.GameState;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using GorillaGameModes;
using Fusion;

namespace FallMonke;

public class CustomGameManager : GorillaGameManager
{    
    public GameStateEnum CurrentState;
    public IGameState CurrentStateHandler;
    public List<Participant> Players;

    public override void StartPlaying()
    {
        base.StartPlaying();
        Players = (
            from player in NetworkSystem.Instance.AllNetPlayers
            where IsPlayerValid(player)
            select new Participant(player, FindPlayerVRRig(player))
        )
        .ToList();
    }

    public override void StopPlaying()
    {
        base.StopPlaying();
    }

    private void UpdateGameState() {
        if (!NetworkSystem.Instance.IsMasterClient) 
            return;

        int alivePlayers = Players.Count(player => player.IsAlive);
        CurrentStateHandler.
    }

    private bool IsPlayerValid(NetPlayer player) {
        return true;
    }

    public override void OnSerializeRead(object newData)
    {
        Main.Log("Got new state " + (int)newData);
        CurrentState = (GameStateEnum)newData; 
    }

    public override object OnSerializeWrite()
    {
        return CurrentState;
    }

    public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    public override void AddFusionDataBehaviour(NetworkObject netObject)
    {
        netObject.AddBehaviour<TagGameModeData>();
    }

    public override GameModeType GameType() {
        return GameModeType.Custom;
    }
}
