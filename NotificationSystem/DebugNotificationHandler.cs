#if DEBUG

using System.Text;
using System.Linq;
using UnityEngine;
using TMPro;

namespace FallMonke.NotificationSystem;

public class DebugNotificationHandler : INotificationHandler
{
    private TextMeshPro textObject;

    public void Setup()
    {
        textObject = GameObject.FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None).Where(x => x.gameObject.name == "DebugText").FirstOrDefault();
        textObject.text = string.Empty;
    }

    public void ShowNotification(string text)
    {
        if (textObject == null)
        {
            Main.Log("No debug text to notify to", BepInEx.Logging.LogLevel.Warning);
            Main.Log(text);
            return;
        }

        var builder = new StringBuilder(textObject.text);
        builder.AppendLine(text);
        textObject.text = builder.ToString();
    }
}

#endif
