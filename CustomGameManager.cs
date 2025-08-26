/*
Should remove all references to PUN as it seems it will be removed in future updates, should rely on the FallMonke.Networking and NetworkSystems instead
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
    public const string MOD_KEY = "fallmonke_prop";
    public const int REQUIRED_PLAYER_COUNT = 2;

    public INotificationHandler NotificationHandler;
    public IBroadcastController BroadcastController;

    public GameStateEnum CurrentState;
    public IGameState CurrentStateHandler;

    public Participant[] Players;
    public Participant LocalPlayer;

    public bool StartButtonPressed;
    public bool CooldownInAffect;

    public override void Awake()
    {
        base.Awake();
        SetPlayerSpeeds(canMove: true);
    }

    // this is called when the gamemode serializer is made 
    public override void StartPlaying()
    {
        base.StartPlaying();
        Main.Log("CustomGameManager starting!", BepInEx.Logging.LogLevel.Message);
        WorldManager.ActivateWorld();

        StartButtonPressed = false;

        NotificationHandler = new NotificationSystem.MonkeNotificationLibWrapper();

        BroadcastController = new Networking.PUNBroadcastController();  // todo: ideally make this only happen if PUN is nolonger an option
        BroadcastController.MakeModIdentifable();
        BroadcastController.SetupEventHandler();

        if (NetworkSystem.Instance.IsMasterClient)
        {
            Main.Log("Looks like I'm the first one here, setting mode to search for players");
            CurrentState = GameStateEnum.PendingStart;
            CurrentStateHandler = StateHandlers[CurrentState];
        }

        TeleportController.TeleportToLobby();
    }

    public override void StopPlaying()
    {
        base.StopPlaying();
        WorldManager.DeactivateWorld();
        BroadcastController.Cleanup();
        TeleportController.TeleportToStump();
    }

    public void CreateParticipants()
    {
        Players = BroadcastController.CreateParticipants();
        LocalPlayer = Players.First(x => x.Player.IsLocal);
    }

    // should be moved to game state handlers
    public void UpdateBoard()
    {
        if (CurrentStateHandler != null)
        {
            var text = CurrentStateHandler.GetBoardText();
            WorldManager.SetBoardText(text.Title, text.Body);
        }
    }

    public void SetPlayerSpeeds(bool canMove)
    {
        if (canMove)
        {
            slowJumpLimit = 6.5f;
            fastJumpLimit = 8.5f;
            slowJumpMultiplier = 1.1f;
            fastJumpMultiplier = 1.3f;
        }
        else
        {
            slowJumpLimit = 0f;
            fastJumpLimit = 0f;
            slowJumpMultiplier = 0f;
            fastJumpMultiplier = 0f;
        }
    }

    // todo: must be called right after player is eliminated to ensure Players.Count != 0 never ever
    public override void InfrequentUpdate()
    {
        base.InfrequentUpdate();
        UpdateBoard();
        if (NetworkSystem.Instance.IsMasterClient)
        {
            if (CurrentStateHandler == null)
            {
                Main.Log("State handler is null, maybe I became master prior to start?", BepInEx.Logging.LogLevel.Warning);
                if (CurrentState == 0) HandleStateSwitch(GameStateEnum.PendingStart);
                else HandleStateSwitch(CurrentState);
            }

            var details = new GameStateDetails
            {
                RemainingPlayers = Players == null ? -1 : Players.Count(player => player.IsAlive),
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

    public bool CanStartGame()
    {
        if (CurrentStateHandler == null || BroadcastController == null) return false;
        if (CurrentState != GameStateEnum.PendingStart) return false;

        return BroadcastController.PlayersWithModCount() >= REQUIRED_PLAYER_COUNT && StartButtonPressed;
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
        else Main.Log("Bad game state " + state, BepInEx.Logging.LogLevel.Warning);
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
}
