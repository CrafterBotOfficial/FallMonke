using MonkeNotificationLib;

namespace FallMonke.NotificationSystem;

public class MonkeNotificationLibWrapper : INotificationHandler
{
    private INotifier notifier;

    public MonkeNotificationLibWrapper()
    {
        notifier = new Notifier("FallMonke");
    }

    public void ShowNotification(string text)
    {
        if (!UI.Buttons.StreamerModeButton.MuteNotifications)
        {
            notifier.Message(text);
        }
    }
}
