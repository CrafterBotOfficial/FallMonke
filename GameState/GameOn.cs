
namespace FallMonke.GameState;

public class GameOn : IGameState
{
    public GameStateEnum CheckGameState(int remainingPlayers, int remainingTiles)
    {
        if (remainingPlayers == 1)
        {
            var manager = CustomGameManager.Instance;
            manager.NotificationHandler.ShowNotification("Game over!");
            manager.NotificationHandler.ShowNotification(manager.Players[0].Player.SanitizedNickName + " wins!");
            return GameStateEnum.Finished;
        }
        return GameStateEnum.GameOn;
    }

    public void OnSwitchTo()
    {
        Main.Log("GameOn: Im getting called, things are starting up");
        CustomGameManager.Instance.CreateParticipants();
        Main.Log($"{CustomGameManager.Instance.Players.Length}/{NetworkSystem.Instance.AllNetPlayers.Length} players Participanting");
        // FallMonke.CustomGameManager.Instance.NotificationHandler.ShowNotification("Game on!");
        // FallMonke.CustomGameManager.Instance.StartGame();
    }
}
