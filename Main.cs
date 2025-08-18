using BepInEx;
using BepInEx.Logging;
using UnityEngine;
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
        Utilla.Events.GameInitialized += async (sender, args) =>
        {
            if (NetworkSystem.Instance is not NetworkSystemPUN)
                return;

            using AssetLoader assetLoader = new AssetLoader("FallMonke.Resources.hexagone");
            WorldManager.HexagonAsset = (GameObject)await assetLoader.LoadAsset("Hexagons");

            ExitGames.Client.Photon.Hashtable properties = new() { { CustomGameManager.MOD_KEY, true } };
            Photon.Pun.PhotonNetwork.SetPlayerCustomProperties(properties);
        };
    }

#if DEBUG
    public void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Join Room")) NetworkSystem.Instance.ConnectToRoom("CRAFTERBOT", RoomConfig.AnyPublicConfig());
        if (GUILayout.Button("Force Load World")) WorldManager.LoadWorld();
        if (GUILayout.Button("Force UnLoad World")) WorldManager.UnloadWorld();
    }
#endif

    public static void Log(object message, LogLevel level = LogLevel.Info)
    {
        instance?.Logger.Log(level, message);
    }
}
