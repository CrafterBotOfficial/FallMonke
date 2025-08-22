using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using Utilla.Attributes;

namespace FallMonke;

[BepInPlugin("com.crafterbot.monkefall", "Fall Monke", "1.0.0")]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.0"), BepInDependency("crafterbot.notificationlib")]
[BepInIncompatibility("tonimacaroni.computerinterface")] // No way to specify a version? If so we must manually check if bad version of CI
[ModdedGamemode("FALLMONKE", "FALL MONKE", typeof(CustomGameManager))]
public class Main : BaseUnityPlugin
{
    private static Main instance;

    private void Awake()
    {
        instance = this;
        Utilla.Events.GameInitialized += (sender, args) =>
        {
            if (NetworkSystem.Instance is not NetworkSystemPUN) // todo: add fusion support, assuming the game ever actually switches
                return;
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Main).Assembly);

            TeleportController.CreateStumpAnchor();

            AssetLoader assetLoader = new AssetLoader("FallMonke.Resources.hexagone");
            WorldManager.AssetLoader = assetLoader;
        };
    }

#if DEBUG
    public void OnGUI()
    {
        GUILayout.BeginVertical();
        // if (GUILayout.Button("Join Room")) NetworkSystem.Instance.ConnectToRoom("CRAFTERBOT", RoomConfig.AnyPublicConfig());
        if (GUILayout.Button("Force Load World")) WorldManager.LoadWorld();
        if (GUILayout.Button("Force UnLoad World")) WorldManager.UnloadWorld();
        // if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom) GUILayout.Label(Photon.Pun.PhotonNetwork.CurrentRoom.CustomProperties.ToString());
        // if (GUILayout.Button("Force Start Game Local")) { CustomGameManager.Instance. }
    }
#endif

    public static void Log(object message, LogLevel level = LogLevel.Info)
    {
        instance?.Logger.Log(level, message);
    }
}
