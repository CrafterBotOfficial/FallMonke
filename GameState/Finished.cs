namespace FallMonke.GameState;

public class Finished : IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        return GameStateEnum.PendingStart;
    }
    public void OnSwitchTo()
    {
        CustomGameManager.Instance.NotificationHandler.ShowNotification("Getting ready for next game...");
    }
}
