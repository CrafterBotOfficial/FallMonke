using System;
using System.Text;

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

    public GameBoardText GetBoardText()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine(string.Empty);
        if (NetworkSystem.Instance.AllNetPlayers.Length > 1)
            if (((CustomGameManager)CustomGameManager.instance).StartButtonPressed)
                stringBuilder.AppendLine("Game will start in 30 seconds");
            else
                stringBuilder.AppendLine("Ready to start");
        else
            stringBuilder.AppendLine("Not enough players");

        return new GameBoardText("Pending Game Start", stringBuilder);
    }

    public bool CanStartGame(CustomGameManager manager)
    {
        return NetworkSystem.Instance.AllNetPlayers.Length >= RequiredPlayerCount && ((CustomGameManager)CustomGameManager.instance).StartButtonPressed;
    }
}
