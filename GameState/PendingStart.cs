using System;
using System.Text;

namespace FallMonke.GameState;

public class PendingStart : IGameState
{
    private static readonly TimeSpan GameOnCountdown = TimeSpan.FromSeconds(10);

    private bool countdown;
    private DateTime startGameTime;

    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        var manager = (CustomGameManager)CustomGameManager.instance;

        if (!countdown && manager.CanStartGame())
        {
            Main.Log("Start game countdown!");
            manager.NotificationHandler.ShowNotification("Game will start in " + GameOnCountdown);
            countdown = true;
            startGameTime = DateTime.Now + GameOnCountdown;
        }

        if (countdown)
        {
            if (!manager.CanStartGame())
            {
                Main.Log("Game requirements no longer met, aborting timer");
                manager.NotificationHandler.ShowNotification("Not enough players to start!");
                countdown = false;
                return GameStateEnum.PendingStart;
            }

            if (startGameTime <= DateTime.Now)
            {
                Main.Log("Game on!");
                return GameStateEnum.GameOn;
            }
        }

        return GameStateEnum.PendingStart;
    }

    public void OnSwitchTo()
    {
        countdown = false;
        WorldManager.Instance.ResetTiles();
    }

    public GameBoardText GetBoardText()
    {
        var manager = (CustomGameManager)CustomGameManager.instance;
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine(string.Empty);

        bool enoughPlayers = manager.NetworkController.PlayersWithModCount() >= CustomGameManager.REQUIRED_PLAYER_COUNT;
        if (enoughPlayers)
            if (manager.StartButtonPressed)
                stringBuilder.AppendLine("Game will start in 10 seconds");
            else
                stringBuilder.AppendLine("Ready to start");
        else
            stringBuilder.AppendLine("Not enough players");

        return new GameBoardText("Pending Game Start", stringBuilder);
    }
}
