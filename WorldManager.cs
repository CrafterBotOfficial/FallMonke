using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using FallMonke.Hexagon;
using System.Collections;

namespace FallMonke;

public sealed class WorldManager
{
    private static readonly object padLock = new();

    private static readonly Lazy<WorldManager> instance = new Lazy<WorldManager>(() => new WorldManager());
    public static WorldManager Instance => instance.Value; 

    public bool SceneLoaded => sceneLoaded;

    private Task loadWorldTask;
#pragma warning disable CS0414
    private volatile bool sceneLoaded;
#pragma warning restore CS0414

    private GameObject sceneParent;
    private HexagonParent hexagonParent;

    public float EliminationHeight;

    public TMP_FontAsset GorillaTextFont;
    private TextMeshPro boardText;

    public async Task ActivateWorld()
    {
        lock (padLock)
        {
            loadWorldTask ??= LoadWorld();
        }
        await loadWorldTask;

        Main.Log("Teleporting player to world");
        SetWorldActive(true);
        TeleportController.TeleportToLobby();
    }

    public void DeactivateWorld()
    {
        SetWorldActive(false);
    }

    private void SetWorldActive(bool active)
    {
        if (sceneParent is not null)
        {
            sceneParent.SetActive(active);
            return;
        }
        Main.Log("World isn't defined", BepInEx.Logging.LogLevel.Error);
    }

    public Task LoadWorld()
    {
        Main.Log("Loading game scene!", BepInEx.Logging.LogLevel.Message);

        var taskCompletionSource = new TaskCompletionSource<object>();

        Stream assetReaderStream = typeof(Main).Assembly.GetManifestResourceStream("FallMonke.Resources.hexagone");
        var bundleCreateRequest = AssetBundle.LoadFromStreamAsync(assetReaderStream);
        bundleCreateRequest.completed += _ =>
        {
            // Main.Log(bundleCreateRequest.assetBundle.GetAllScenePaths());
            assetReaderStream.Dispose();

            var loadSceneOperation = SceneManager.LoadSceneAsync("Crafterbot", LoadSceneMode.Additive);
            loadSceneOperation.completed += _ =>
            {
                try
                {
                    OnSceneLoaded();
                }
                catch (Exception ex)
                {
                    Main.Log("Failed to setup scene: " + ex, BepInEx.Logging.LogLevel.Fatal);
                }
                finally
                {
                    taskCompletionSource.SetResult(null);
                }
            };
        };

        return taskCompletionSource.Task;
    }

    private void OnSceneLoaded()
    {
        Main.Log("Scene loaded", BepInEx.Logging.LogLevel.Message);

        ZoneManagement.AddSceneToForceStayLoaded("Crafterbot");
        sceneParent = SceneManager.GetSceneByName("Crafterbot").GetRootGameObjects()[0]; // GameObject.Find("/SceneParent");

        hexagonParent = GameObject.FindFirstObjectByType<HexagonParent>();
        Main.Log(hexagonParent.Hexagons.Length + " tiles");

        GorillaTextFont = GorillaTagger.Instance.offlineVRRig.playerText1.font;

        SetupButtons();

        sceneParent.transform.Find("room").AddComponent<GorillaSurfaceOverride>().transform
            .GetChild(0)
            .AddComponent<GorillaSurfaceOverride>().overrideIndex = 120; // for the glass sounds

        EliminationHeight = sceneParent.transform.Find("WaterVRview").transform.position.y;

        // setup board
        boardText = sceneParent.transform.Find("TextComponents/TextHeader").GetComponent<TextMeshPro>();
        boardText.font = GorillaTextFont;
        boardText.text = string.Empty;

        sceneParent.SetActive(false);
        sceneLoaded = true;
    }

    private void SetupButtons()
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

    public FallableHexagon GetTileByIndex(int index)
    {
        return hexagonParent.Hexagons[index];
    }

    public int GetTileIndex(FallableHexagon hex)
    {
        return hexagonParent.Hexagons.IndexOfRef(hex);
    }

    public IEnumerator ResetTilesCorountine()
    {
        if (!sceneLoaded)
        {
            // Main.Log("Scene not loaded yet.", BepInEx.Logging.LogLevel.Warning);
            yield break;
        }
        var tiles = hexagonParent.Hexagons.Where(x => x.IsFalling).ToArray();
        TeleportController.FisherYatesShuffle(tiles);  // the random seed should still be synced between players, not that it really matters in this context
        int delay = 5000 / tiles.Length;

        foreach (var tile in tiles)
        {
            tile.Reset();
            yield return new WaitForSeconds(delay / 1000f);
        }
        Main.Log("Finished resetting tiles");
    }

    public Transform GetParent()
    {
        if (sceneParent is null)
        {
            Main.Log("Scene not loaded, yet trying to play game.", BepInEx.Logging.LogLevel.Fatal);
            return null;
        }
        return sceneParent.transform;
    }

    public void SetBoardText(string header, System.Text.StringBuilder stringBuilder)
    {
        if (boardText is null)
        {
            Main.Log(stringBuilder, BepInEx.Logging.LogLevel.Warning);
            return;
        }
        var headerBuilder = new System.Text.StringBuilder();
        headerBuilder.AppendLine(header);
        headerBuilder.AppendLine("- - - - - - - - - - - - - - - - - - - -");
        boardText.text = headerBuilder.Append(stringBuilder).ToString();
    }
}
