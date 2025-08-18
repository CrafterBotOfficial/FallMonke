/*
If a player joins late we should just ignore them, partipants are set when the game starts
*/

using FallMonke.GameState;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using GorillaGameModes;
using Fusion;

namespace FallMonke;

public class CustomGameManager : GorillaGameManager
{
    public static CustomGameManager Instance;

    public const string MOD_KEY = "fallmonke_prop";

    public GameStateEnum CurrentState;
    public IGameState CurrentStateHandler;
    public List<Participant> Players;

    private Dictionary<GameStateEnum, IGameState> GameHandlerDict;

    // this is called when the gamemode serializer is made 
    public override void StartPlaying()
    {
        base.StartPlaying();
        Instance = this;
        WorldManager.LoadWorld();
    }

    public override void StopPlaying()
    {
        base.StopPlaying();
        WorldManager.UnloadWorld();
    }

    private void StartGame()
    {
        Players = (
            from player in NetworkSystem.Instance.AllNetPlayers
            where player.ActorNumber != -1
            where PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber).CustomProperties.ContainsKey(MOD_KEY)
            select new Participant(player, FindPlayerVRRig(player))
        )
        .ToList();
    }

    public override void InfrequentUpdate()
    {
        base.InfrequentUpdate();
        if (NetworkSystem.Instance.IsMasterClient)
        {
            int alivePlayers = Players.Count(player => player.IsAlive);
            int remainingTiles = 10;
            CurrentState = CurrentStateHandler.CheckGameState(alivePlayers, remainingTiles);
        }
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
        // netObject.AddBehaviour<TagGameModeData>();
    }

    public override GameModeType GameType()
    {
        return GameModeType.Custom;
    }
}
