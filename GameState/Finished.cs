namespace FallMonke.GameState;

public class Finished : IGameState
{
    public GameStateEnum CheckGameState(int remainingPlayers, int remainingTiles)
    {
        return GameStateEnum.PendingStart;
    }
    public void OnSwitchTo()
    {
        CustomGameManager.Instance.NotificationHandler.ShowNotification("Getting ready for next game...");
    }
}
