// this is just testing code, once the scene is ready we will load it ontop of the normal world. It will contain all of the assets and decorations and hexagons

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using FallMonke.Hexagon;

namespace FallMonke;

public static class WorldManager
{
    private static bool sceneLoaded;

    public static AssetLoader AssetLoader;
    private static HexagonParent hexagonParent;
    public static float EliminationHeight;

    public static TMP_FontAsset GorillaTextFont;
    private static TextMeshPro boardText;

    public static void SetBoardText(string header, System.Text.StringBuilder stringBuilder)
    {
        if (boardText == null)
        {
            Main.Log(stringBuilder, BepInEx.Logging.LogLevel.Warning);
            return;
        }
        var headerBuilder = new System.Text.StringBuilder();
        headerBuilder.AppendLine(header);
        headerBuilder.AppendLine("- - - - - - - - - - - - - - - - - - - - - - - - -");
        boardText.text = headerBuilder.Append(stringBuilder).ToString();
    }

    public static async void LoadWorld()
    {
        while (sceneLoaded) // sometimes the scene wont unload in a timely enough fashion and then it will laod another ontop of it.
        {
            await Task.Yield();
        }
        Main.Log("Loading game scene!", BepInEx.Logging.LogLevel.Message);

        if (!NetworkSystem.Instance.InRoom)
        {
            Main.Log("Player left prior to scene loading.");
            WorldManager.UnloadWorld();
            return;
        }

        // var sceneName = AssetLoader.GetSceneName();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadSceneAsync("Crafterbot", LoadSceneMode.Additive);
        sceneLoaded = true;
    }

    public static void UnloadWorld()
    {
        Main.Log("Removing world");
        var operation = SceneManager.UnloadSceneAsync("Crafterbot");
        operation.completed += _ =>
        {
            Main.Log("Scene finished unloading");
            sceneLoaded = false;
        };
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Main.Log("Scene loaded", BepInEx.Logging.LogLevel.Message);
        if (scene.name != "Crafterbot")
            return;

        hexagonParent = GameObject.FindObjectOfType<HexagonParent>();
        ((CustomGameManager)CustomGameManager.instance).NotificationHandler.Setup();
        Main.Log(hexagonParent.Hexagons.Length + " tiles");

        SetupButtons();

        GameObject.Find("room").AddComponent<GorillaSurfaceOverride>().transform.GetChild(0).AddComponent<GorillaSurfaceOverride>().overrideIndex = 120;

        EliminationHeight = GameObject.Find("/WaterVRview").transform.position.y;

        GorillaTextFont = GorillaTagger.Instance.offlineVRRig.playerText1.font;

        // setup board
        boardText = GameObject.Find("/TextComponents/TextHeader").GetComponent<TextMeshPro>();
        boardText.font = GorillaTextFont;
        boardText.text = string.Empty;

        TeleportController.TeleportToLobby();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private static void SetupButtons()
    {
        var startGameButton = GameObject.Find("FallMonke Buttons/Start");
        var leaveButton = GameObject.Find("FallMonke Buttons/Leave");
        var streamerModeButton = GameObject.Find("FallMonke Buttons/Streamer");

        startGameButton.GetComponentInChildren<TMP_Text>().font = GorillaTagger.Instance.offlineVRRig.playerText1.font;
        leaveButton.GetComponentInChildren<TMP_Text>().font = GorillaTagger.Instance.offlineVRRig.playerText1.font;
        streamerModeButton.GetComponentInChildren<TMP_Text>().font = GorillaTagger.Instance.offlineVRRig.playerText1.font;

        startGameButton.AddComponent<UI.Buttons.StartGameButton>();
        leaveButton.AddComponent<UI.Buttons.LeaveGameButton>();
        streamerModeButton.AddComponent<UI.Buttons.StreamerModeButton>();
    }

    public static FallableHexagon GetTileByIndex(int index)
    {
        return hexagonParent.Hexagons[index];
    }

    public static int GetTileIndex(FallableHexagon hex)
    {
        return hexagonParent.Hexagons.IndexOfRef(hex);
    }

    public static void ResetTiles()
    {
        foreach (var tile in hexagonParent.Hexagons)
            tile.Reset();
    }
}
