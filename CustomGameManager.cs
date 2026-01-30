using FallMonke.GameState;
using FallMonke.NotificationSystem;
using FallMonke.Networking;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using GorillaGameModes;
using Fusion;
using System.Collections;
using UnityEngine;
using System;

namespace FallMonke;

public class CustomGameManager : GorillaGameManager
{
    public const string MOD_KEY = "fallmonke_prop";

    public INotificationHandler NotificationHandler;
    public INetworkController NetworkController;

    public GameStateEnum CurrentState;
    public IGameState CurrentStateHandler;

    public Participant[] Players;
    public int[] PlayerIDs;
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

        WorldManager.Instance.ActivateWorld();

        StartButtonPressed = false;

        NotificationHandler = new MonkeNotificationLibWrapper();

        NetworkController = new PUNNetworkController();
        NetworkController.MakeModIdentifable();
        NetworkController.SetupEventHandler();

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
        WorldManager.Instance.DeactivateWorld();
        NetworkController.Cleanup();
        TeleportController.TeleportToStump();
    }

    public void CreateParticipants()
    {
        Players = NetworkController.CreateParticipants();
        PlayerIDs = [.. Players.Select(x => x.Player.ActorNumber)];
        LocalPlayer = Players.First(x => x.Player.IsLocal);
    }

    public void UpdateBoard()
    {
        if (CurrentStateHandler is not null)
        {
            var text = CurrentStateHandler.GetBoardText();
            WorldManager.Instance.SetBoardText(text.Title, text.Body);
        }
    }

    public IEnumerator StartGameCooldown()
    {
        CooldownInAffect = true;
        SetPlayerSpeeds(canMove: false);
        yield return new WaitForSeconds(GameConfig.START_GAME_MOVEMENT_COOLDOWN_SECONDS);
        CooldownInAffect = false;
        SetPlayerSpeeds(canMove: true);
    }

    public void SetPlayerSpeeds(bool canMove)
    {
        if (canMove)
        {
            slowJumpLimit = GameConfig.SLOW_JUMP_LIMIT;
            fastJumpLimit = GameConfig.FAST_JUMP_LIMIT;
            slowJumpMultiplier = GameConfig.SLOW_JUMP_MULTIPLIER;
            fastJumpMultiplier = GameConfig.FAST_JUMP_MULTIPLIER;
        }
        else
        {
            slowJumpLimit = 0f;
            fastJumpLimit = 0f;
            slowJumpMultiplier = 0f;
            fastJumpMultiplier = 0f;
        }
    }

    public override void InfrequentUpdate()
    {
        base.InfrequentUpdate();

        if (!WorldManager.Instance.SceneLoaded)
            return;

        UpdateBoard();
        if (NetworkSystem.Instance.IsMasterClient)
        {
            if (CurrentStateHandler is null)
            {
                Main.Log("State handler is null, maybe I became master prior to start?", BepInEx.Logging.LogLevel.Warning);
                if (CurrentState == 0) HandleStateSwitch(GameStateEnum.PendingStart);
                else HandleStateSwitch(CurrentState);
            }

            var details = new GameStateDetails() with
            {
                RemainingPlayers = Players is null ? -1 : Players.Count(player => player.IsAlive),
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
        if (CurrentStateHandler is null || NetworkController is null) return false;
        if (CurrentState != GameStateEnum.PendingStart) return false;

        return NetworkController.PlayersWithModCount() >= GameConfig.REQUIRED_PLAYER_COUNT && StartButtonPressed;
    }

    public override void OnSerializeRead(object newData) { Main.Log("Got new state " + (int)newData); HandleStateSwitch((GameStateEnum)newData); }
    public override object OnSerializeWrite() { return CurrentState; }

    public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!NetworkSystem.Instance.IsMasterClient)
        {
            return;
        }
        stream.SendNext(CurrentState);
    }

    public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
    {
        if (NetworkSystem.Instance.IsMasterClient) return;

        int state = (int)stream.ReceiveNext();
        if (Enum.IsDefined(typeof(GameStateEnum), state))
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

    public readonly static Dictionary<GameStateEnum, IGameState> StateHandlers = new()
    {
        { GameStateEnum.PendingStart, new GameState.PendingStart() },
        { GameStateEnum.GameOn, new GameState.GameOn() },
        { GameStateEnum.Finished, new GameState.Finished() },
    };

    /// <summary>
    /// All calls to this must be made when the game is on, otherwise will throw an exception.
    /// </summary>
    internal static CustomGameManager GetInstance() =>
        instance as CustomGameManager ?? throw new Exception("Game manager not yet created");
}

public record struct GameStateDetails
{
    public int RemainingPlayers;
}
