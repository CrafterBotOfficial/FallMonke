using BepInEx;
using BepInEx.Logging;
using Utilla.Attributes;

namespace FallMonke;

[BepInPlugin("com.crafterbot.monkefall", "Fall Monke", "1.1.3")]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.0"), BepInDependency("crafterbot.notificationlib", "1.1.0")]
[ModdedGamemode("FALLMONKE", "FALL MONKE", typeof(CustomGameManager))]
public class Main : BaseUnityPlugin
{
    public static Main Instance;

    private void Awake()
    {
        Instance = this;
        Utilla.Events.GameInitialized += (_, _) =>
        {
            if (NetworkSystem.Instance is not NetworkSystemPUN)
                return;

            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Main).Assembly);
            TeleportController.CreateStumpAnchor();
        };
    }

    public static void Log(object message, LogLevel level = LogLevel.Info)
    {
        Instance?.Logger.Log(level, message);
    }
}
