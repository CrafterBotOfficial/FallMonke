using System.Linq;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using FallMonke.Hexagon;

namespace FallMonke;

public static class WorldManager
{
    private static bool sceneLoaded;

    private static GameObject sceneParent;
    private static HexagonParent hexagonParent;

    public static float EliminationHeight;

    public static TMP_FontAsset GorillaTextFont;
    private static TextMeshPro boardText;

    public static async void ActivateWorld()
    {
        if (!sceneLoaded)
        {
            LoadWorld();
            while (!sceneLoaded)
            {
                await Task.Yield();
            }
        }
        SetWorldActive(true);
        TeleportController.TeleportToLobby();
    }

    public static void DeactivateWorld()
    {
        SetWorldActive(false);
    }

    private static void SetWorldActive(bool active)
    {
        if (sceneParent != null)
        {
            sceneParent.SetActive(active);
            return;
        }
        Main.Log("World is null", BepInEx.Logging.LogLevel.Error);
    }

    public static void LoadWorld()
    {
        Main.Log("Loading game scene!", BepInEx.Logging.LogLevel.Message);

        Stream assetReaderStream = typeof(Main).Assembly.GetManifestResourceStream("FallMonke.Resources.hexagone");
        // var bundle = AssetBundle.LoadFromStream(assetReaderStream);
        var bundleCreateRequest = AssetBundle.LoadFromStreamAsync(assetReaderStream);

        bundleCreateRequest.completed += _ =>
        {
            Main.Log(bundleCreateRequest.assetBundle.GetAllScenePaths());
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync("Crafterbot", LoadSceneMode.Additive);
            assetReaderStream.Dispose();
        };
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Crafterbot")
            return;
        Main.Log("Scene loaded", BepInEx.Logging.LogLevel.Message);

        sceneParent = scene.GetRootGameObjects()[0]; //GameObject.Find("/SceneParent");

        hexagonParent = GameObject.FindObjectOfType<HexagonParent>();
        Main.Log(hexagonParent.Hexagons.Length + " tiles");

        GorillaTextFont = GorillaTagger.Instance.offlineVRRig.playerText1.font;

        SetupButtons();

        sceneParent.transform.Find("/room").AddComponent<GorillaSurfaceOverride>().transform
            .GetChild(0)
            .AddComponent<GorillaSurfaceOverride>().overrideIndex = 120; // for the glass sounds

        EliminationHeight = sceneParent.transform.Find("/WaterVRview").transform.position.y;

        // setup board
        boardText = sceneParent.transform.Find("/TextComponents/TextHeader").GetComponent<TextMeshPro>();
        boardText.font = GorillaTextFont;
        boardText.text = string.Empty;

        sceneParent.SetActive(false);
        sceneLoaded = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private static void SetupButtons()
    {
        var startGameButton = GameObject.Find("FallMonke Buttons/Start");
        var leaveButton = GameObject.Find("FallMonke Buttons/Leave");
        var streamerModeButton = GameObject.Find("FallMonke Buttons/Streamer");

        startGameButton.GetComponentInChildren<TMP_Text>().font = GorillaTextFont;
        leaveButton.GetComponentInChildren<TMP_Text>().font = GorillaTextFont;
        streamerModeButton.GetComponentInChildren<TMP_Text>().font = GorillaTextFont;

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

    public static async void ResetTiles()
    {
        var tiles = hexagonParent.Hexagons.Where(x => x.IsFalling).ToArray();
        TeleportController.FisherYatesShuffle(tiles);  // the random seed should still be synced between players
        int delay = 5000 / tiles.Length;

        foreach (var tile in tiles)
        {
            tile.Reset();
            await Task.Delay(delay);
        }
        Main.Log("Finished resetting tiles");
    }

    public static Transform GetParent()
    {
        if (sceneParent == null)
        {
            Main.Log("Scene not loaded yet trying to play game.", BepInEx.Logging.LogLevel.Fatal);
            return null;
        }
        return sceneParent.transform;
    }

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
}
