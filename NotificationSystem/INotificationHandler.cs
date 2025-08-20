namespace FallMonke.NotificationSystem;

public interface INotificationHandler
{
    public void Setup();
    public void ShowNotification(string text);
}
