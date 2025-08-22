using MonkeNotificationLib;

namespace FallMonke.NotificationSystem;

public class MonkeNotificationLibWrapper : INotificationHandler
{
    public void Setup()
    {
    }
    public void ShowNotification(string text)
    {
        NotificationController.AppendMessage("FallMonke", text);
    }
}
