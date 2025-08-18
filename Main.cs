using BepInEx;
using BepInEx.Logging;
using Utilla.Attributes;

namespace FallMonke;

[BepInPlugin("com.crafterbot.monkefall", "Fall Monke", "1.0.0")]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.0")]
[ModdedGamemode("fallmonke", "Fall Monke", typeof(CustomGameManager))]
public class Main : BaseUnityPlugin
{
    private static Main instance;

    private void Awake()
    {
        instance = this;
        Utilla.Events.GameInitialized += (sender, args) =>
        {
            ExitGames.Client.Photon.Hashtable properties = new() { { CustomGameManager.MOD_KEY, true } };
            Photon.Pun.PhotonNetwork.SetPlayerCustomProperties(properties);
        };
    }

    public static void Log(object message, LogLevel level = LogLevel.Info)
    {
        instance?.Logger.Log(level, message);
    }
}
