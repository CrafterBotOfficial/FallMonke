using BepInEx;
using BepInEx.Logging;
using Utilla.Attributes;

namespace FallMonke;

[BepInPlugin("com.crafterbot.monkefall", "Fall Monke", "1.1.0")]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.0"), BepInDependency("crafterbot.notificationlib")]
[ModdedGamemode("FALLMONKE", "FALL MONKE", typeof(CustomGameManager))]
public class Main : BaseUnityPlugin
{
    private static Main instance;

    private void Awake()
    {
        instance = this;
        Utilla.Events.GameInitialized += (sender, args) =>
        {
            if (NetworkSystem.Instance is not NetworkSystemPUN)
                return;

            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Main).Assembly);
            TeleportController.CreateStumpAnchor();
        };
    }

    public static void Log(object message, LogLevel level = LogLevel.Info)
    {
        instance?.Logger.Log(level, message);
    }
}
