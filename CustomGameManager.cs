/*
If a player joins late we should just ignore them, partipants are set when the game starts
Should remove all references to PUN as it seems it will be removed in future updates, should rely on the Networking and NetworkSystems instead
*/

using FallMonke.GameState;
using FallMonke.NotificationSystem;
using FallMonke.Networking;
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

    public INotificationHandler NotificationHandler;
    public IBroadcastController BroadcastController;

    public GameStateEnum CurrentState;
    public IGameState CurrentStateHandler;

    public Participant[] Players;
    public int[] ParticipantActorNumbers;
    public Participant LocalPlayer;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        fastJumpLimit = 10;
        fastJumpMultiplier = 1;
    }

    // this is called when the gamemode serializer is made 
    public override void StartPlaying()
    {
        base.StartPlaying();
        Main.Log("CustomGameManager starting!", BepInEx.Logging.LogLevel.Message);
        WorldManager.LoadWorld();

#if DEBUG
        NotificationHandler = new NotificationSystem.DebugNotificationHandler();
#else
        // todo
#endif

        BroadcastController = new Networking.PUNBroadcastController();  // todo: ideally make this only happen if PUN is nolonger an option
        BroadcastController.MakeModIdentifable();

        if (NetworkSystem.Instance.IsMasterClient)
        {
            Main.Log("Looks like I'm the first one here, setting mode to search for players");
            CurrentState = GameStateEnum.PendingStart;
            CurrentStateHandler = StateHandlers[CurrentState];
        }
    }

    public override void StopPlaying()
    {
        base.StopPlaying();
        WorldManager.UnloadWorld();
        BroadcastController.Cleanup();
    }

    public void CreateParticipants()
    {
        Players = BroadcastController.CreateParticipants();
        ParticipantActorNumbers = CustomGameManager.Instance.Players.Select(player => player.Player.ActorNumber).ToArray();
        LocalPlayer = Players.First(x => x.Player.IsLocal);
    }

    // todo: must be called right after player is eliminated to ensure Players.Count != 0 never ever
    public override void InfrequentUpdate()
    {
        base.InfrequentUpdate();
        if (NetworkSystem.Instance.IsMasterClient)
        {
            var details = new GameStateDetails
            {
                RemainingPlayers = Players is null ? -1 : Players.Count(player => player.IsAlive),
                RemainingTiles = WorldManager.GetRemainingTiles()
            };
            HandleStateSwitch(CurrentStateHandler.CheckGameState(details));
        }
    }

    public void HandleStateSwitch(GameStateEnum newState)
    {
        if (newState != CurrentState)
        {
            if (!StateHandlers.TryGetValue(newState, out IGameState handler))
            {
                Main.Log("Invalid state enum, maybe a cheater sending bad events as master?");
                return;
            }
            CurrentState = newState;
            Main.Log("State switched to " + newState.ToString());
            CurrentStateHandler = handler;
            CurrentStateHandler.OnSwitchTo();
        }
    }

    public override void OnSerializeRead(object newData) { Main.Log("Got new state " + (int)newData); HandleStateSwitch((GameStateEnum)newData); }
    public override object OnSerializeWrite() { return CurrentState; }

    public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!NetworkSystem.Instance.IsMasterClient)
        {
            Main.Log("Why do I have authority? Maybe I am a cheater?", BepInEx.Logging.LogLevel.Warning);
            return;
        }
        stream.SendNext(CurrentState);
    }

    public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
    {
        if (NetworkSystem.Instance.IsMasterClient) return;

        int state = (int)stream.ReceiveNext();
        if (System.Enum.IsDefined(typeof(GameStateEnum), state))
        {
            Main.Log("Got new state " + state);
            HandleStateSwitch((GameStateEnum)state);
        }
    }

    public override void AddFusionDataBehaviour(NetworkObject netObject) {/* netObject.AddBehaviour<TagGameModeData>(); */}

    public override void OnMasterClientSwitched(NetPlayer newMaster)
    {
        base.OnMasterClientSwitched(newMaster);
        if (newMaster.IsLocal)
        {
            Main.Log("Looks like Im incharge now");
            // todo: add any logic to handle the game, but probably not nessacry since everything just checks if the local player is the master anyway 
        }
    }

    public override string GameModeName()
    {
        return "FALLMONKE";
    }

    public new string GameTypeName()
    {
        Main.Log("GameTypeName called", BepInEx.Logging.LogLevel.Message);
        return "FALLMONKE";
    }

    public override GameModeType GameType()
    {
        return (GameModeType)1760;
    }

    public readonly static Dictionary<GameStateEnum, IGameState> StateHandlers = new Dictionary<GameStateEnum, IGameState>()
    {
        { GameStateEnum.PendingStart, new GameState.PendingStart() },
        { GameStateEnum.GameOn, new GameState.GameOn() },
        { GameStateEnum.Finished, new GameState.Finished() },
    };
}

public struct GameStateDetails
{
    public int RemainingPlayers;
    public int RemainingTiles;
}
