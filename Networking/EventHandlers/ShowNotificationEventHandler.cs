namespace FallMonke.Networking.EventHandlers;

public class ShowNotificationEventHandler : IEventHandler
{
    public const int MaxMessageLength = 150; // TODO: figure out whats to big before it covers the screen 

    public void OnEvent(NetPlayer sender, object data)
    {
        if (sender.IsMasterClient && data is string message && message.Length < MaxMessageLength)
        {
            ((CustomGameManager)CustomGameManager.instance).NotificationHandler.ShowNotification(message);
        }
    }
}
