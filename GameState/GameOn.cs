using System.Linq;

namespace FallMonke.GameState;

public class GameOn : IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        if (details.RemainingPlayers == 1)
        {
            var manager = CustomGameManager.Instance;
            manager.NotificationHandler.ShowNotification("Game over!");
            manager.NotificationHandler.ShowNotification(GetWinner().Player.SanitizedNickName + " wins!");
            return GameStateEnum.Finished;
        }

        if (details.RemainingPlayers < 1)
        {
            Main.Log("Not enough players to continue the game.");
            CustomGameManager.Instance.NotificationHandler.ShowNotification("Game over!");
            return GameStateEnum.Finished;
        }

        return GameStateEnum.GameOn;
    }

    public void OnSwitchTo()
    {
        Main.Log("GameOn: Im getting called, things are starting up");
        CustomGameManager.Instance.CreateParticipants();
        Main.Log($"{CustomGameManager.Instance.Players.Length}/{NetworkSystem.Instance.AllNetPlayers.Length} players Participanting");
        FallMonke.CustomGameManager.Instance.NotificationHandler.ShowNotification("Game on!");
        TeleportController.TeleportToGame();
    }

    private Participant GetWinner()
    {
        return CustomGameManager.Instance.Players.First(x => x.IsAlive);
    }
}
