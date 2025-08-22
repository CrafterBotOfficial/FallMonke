using MonkeNotificationLib;

namespace FallMonke.NotificationSystem;

public class MonkeNotificationLibWrapper : INotificationHandler
{
    public void Setup()
    {
    }

    public void ShowNotification(string text)
    {
        if (!UI.Buttons.StreamerModeButton.MuteNotifications)
        {
            NotificationController.AppendMessage("FallMonke", text);
        }
    }
}
