using System;

namespace FallMonke.GameState;

public class PendingStart : IGameState
{
    private const int RequiredPlayerCount = 2;
    private static readonly TimeSpan GameOnCountdown = TimeSpan.FromMinutes(.5f);

    private bool countdown;
    private DateTime startGameTime;

    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        var manager = (CustomGameManager)CustomGameManager.instance;

        if (!countdown && CanStartGame(manager))
        {
            Main.Log("Start game countdown!");
            manager.NotificationHandler.ShowNotification("Game will start in " + GameOnCountdown);
            countdown = true;
            startGameTime = DateTime.Now + GameOnCountdown;
        }

        if (countdown)
        {
            if (!CanStartGame(manager))
            {
                Main.Log("Game requirements no longer met, aborting timer");
                manager.NotificationHandler.ShowNotification("Not enough players to start!");
                countdown = false;
                return GameStateEnum.PendingStart;
            }

            if (startGameTime <= DateTime.Now)
            {
                Main.Log("Game on!");
                manager.NotificationHandler.ShowNotification("Game on!");
                return GameStateEnum.GameOn;
            }
        }

        return GameStateEnum.PendingStart;
    }

    public void OnSwitchTo()
    {
        countdown = false;
    }

    public bool CanStartGame(CustomGameManager manager)
    {
        // todo: add any other requirements, like players actually having the mod
        // todo: change to check custom props using IBroadcastController to ensure it doesnt start if quest players are present
        return NetworkSystem.Instance.AllNetPlayers.Length >= RequiredPlayerCount;
    }
}
