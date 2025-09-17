using System;

namespace FallMonke.GameState;

public class Finished : IGameState
{
    private DateTime switchTime;

    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        if (switchTime <= DateTime.Now)
        {
            Main.Log("Safety delay over, changing state to game ready.");
            return GameStateEnum.PendingStart;
        }
        return GameStateEnum.Finished;
    }

    public void OnSwitchTo()
    {
        var manager = (CustomGameManager)CustomGameManager.instance;
        manager.StartButtonPressed = false;

        manager.NotificationHandler.ShowNotification("Getting ready for next game...");
        switchTime = DateTime.Now + TimeSpan.FromSeconds(GameConfig.FINISHED_DELAY_SECONDS); // a janky way to ensure all players switch to finished so they cleanup.

        if (!manager.Players.IsNullOrEmpty())
            foreach (var player in manager.Players)
            {
                UnityEngine.Object.Destroy(player.Manager);
            }

        TeleportController.TeleportToLobby();

        // play gtag game over sound
        VRRig.LocalRig.PlayTagSoundLocal(2, .25f, stopCurrentAudio: true);
    }

    public GameBoardText GetBoardText()
    {
        return new GameBoardText("Loading", new("Game over grace period."));
    }
}
