// this is just testing code, once the scene is ready we will load it ontop of the normal world. It will contain all of the assets and decorations and hexagons

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using FallMonke.Hexagon;

namespace FallMonke;

public static class WorldManager
{
    public static AssetLoader AssetLoader;
    private static HexagonParent hexagonParent;

    public static float EliminationHeight;

    public static void LoadWorld()
    {
        Main.Log("Loading game scene!", BepInEx.Logging.LogLevel.Message);

        // var sceneName = AssetLoader.GetSceneName();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadSceneAsync("Crafterbot", LoadSceneMode.Additive);
    }

    public static void UnloadWorld()
    {
        Main.Log("Removing world");
        SceneManager.UnloadSceneAsync("Crafterbot");
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Main.Log("Scene loaded", BepInEx.Logging.LogLevel.Message);
        if (scene.name == "Crafterbot")
        {
            hexagonParent = GameObject.FindObjectOfType<HexagonParent>();
            ((CustomGameManager)CustomGameManager.instance).NotificationHandler.Setup();
            Main.Log(hexagonParent.Hexagons.Length + " tiles");

            SetupButtons();
            try { GameObject.Find("room").GetComponent<MeshCollider>().AddComponent<GorillaSurfaceOverride>(); } catch { }

            EliminationHeight = GameObject.Find("/WaterVRview").transform.position.y;

            TeleportController.TeleportToLobby();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private static void SetupButtons()
    {
        var startGameButton = GameObject.Find("FallMonke Buttons/Start");
        var leaveButton = GameObject.Find("FallMonke Buttons/Leave");

        startGameButton.GetComponentInChildren<TMP_Text>().font = GorillaTagger.Instance.offlineVRRig.playerText1.font;
        leaveButton.GetComponentInChildren<TMP_Text>().font = GorillaTagger.Instance.offlineVRRig.playerText1.font;

        startGameButton.AddComponent<UI.Buttons.StartGameButton>();
        leaveButton.AddComponent<UI.Buttons.LeaveGameButton>();
    }

    public static FallableHexagon GetTileByIndex(int index)
    {
        return hexagonParent.Hexagons[index];
    }

    public static int GetTileIndex(FallableHexagon hex)
    {
        return hexagonParent.Hexagons.IndexOfRef(hex);
    }

    public static int GetRemainingTiles()
    {
        int result = 0;
        if (hexagonParent != null)
            foreach (var tile in hexagonParent.Hexagons)
            {
                if (!tile.IsFalling)
                {
                    result++;
                }
            }
        return result;
    }

    public static void ResetTiles()
    {
        foreach (var tile in hexagonParent.Hexagons)
            tile.Reset();
    }
}
