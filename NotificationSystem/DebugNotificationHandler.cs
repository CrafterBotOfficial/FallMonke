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
        try
        {
            GameObject.FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None).Where(x => x.gameObject.name == "TextHeader").FirstOrDefault().font = GorillaTagger.Instance.offlineVRRig.playerText1.font;

            textObject = GameObject.FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None).Where(x => x.gameObject.name == "DebugText").FirstOrDefault();
            textObject.font = GorillaTagger.Instance.offlineVRRig.playerText1.font;
            textObject.text = string.Empty;
        }
        catch (System.Exception ex)
        {
            Main.Log(ex, BepInEx.Logging.LogLevel.Error);
        }
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
