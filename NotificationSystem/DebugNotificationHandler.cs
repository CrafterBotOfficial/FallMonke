#if DEBUG

using System.Text;

namespace FallMonke.NotificationSystem;

public class DebugNotificationHandler : INotificationHandler
{
    public void ShowNotification(string text)
    {
        var textObj = UnityEngine.GameObject.Find("/world/debugtxtorsomething").GetComponent<UnityEngine.TextMesh>();
        var builder = new StringBuilder(textObj.text);
        builder.AppendLine(text);
        textObj.text = builder.ToString();
    }
}

#endif
