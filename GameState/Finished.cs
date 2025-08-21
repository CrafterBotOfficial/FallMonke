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
        switchTime = DateTime.Now + TimeSpan.FromSeconds(5); // a janky way to ensure all clients switch to finished so they cleanup. Should fix layer
        foreach (var player in CustomGameManager.Instance.Players)
        {
            UnityEngine.Object.Destroy(player.Manager);
        }
        CustomGameManager.Instance.NotificationHandler.ShowNotification("Getting ready for next game...");
    }
}
