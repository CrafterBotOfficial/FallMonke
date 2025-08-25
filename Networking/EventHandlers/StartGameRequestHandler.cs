namespace FallMonke.Networking.EventHandlers;

public class StartGameRequestHandler : IEventHandler
{
    public void OnEvent(NetPlayer sender, object data)
    {
        if (CustomGameManager.instance is not CustomGameManager manager)
            return;

        if (manager.CurrentState != GameState.GameStateEnum.PendingStart)
        {
            if (sender.IsLocal)
            {
                manager.NotificationHandler.ShowNotification("Please wait for this round to finish.");
            }
            return;
        }

        if (manager.StartButtonPressed)
        {
            if (sender.IsLocal)
            {
                Main.Log("Calm tf down, its already pressed");
                manager.NotificationHandler.ShowNotification("Game start request already issued, take a chill pill.");
            }
            return;
        }
        manager.StartButtonPressed = true;
        manager.NotificationHandler.ShowNotification($"{sender.SanitizedNickName} has pressed the start game button");
    }
}
