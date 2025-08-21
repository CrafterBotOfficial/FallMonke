namespace FallMonke.GameState;

public class Finished : IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        return GameStateEnum.PendingStart;
    }
    public void OnSwitchTo()
    {
        foreach (var player in CustomGameManager.Instance.Players)
        {
            UnityEngine.Object.Destroy(player.Manager);
        }
        CustomGameManager.Instance.NotificationHandler.ShowNotification("Getting ready for next game...");
    }
}
