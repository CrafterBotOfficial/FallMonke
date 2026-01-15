using UnityEngine;
using System.Linq;

namespace FallMonke;

public static class TeleportController
{
    private static Transform stumpSpawnpoint;

    public static void CreateStumpAnchor()
    {
        var position = new Vector3(-66.6f, 12f, -80.6f);
        stumpSpawnpoint = new GameObject().transform;
        stumpSpawnpoint.position = position;
    }

    public static void TeleportToLobby()
    {
        TeleportLocalPlayer("LobbySpawnpoint");
    }

    public static void TeleportToGame(int seed)
    {
        if (GorillaGameManager.instance is not CustomGameManager manager)
            return;

        Random.InitState(seed);
        Transform[] spawnPoints = [..WorldManager.Instance.GetParent()
                                                       .Find("SpawnPoints")
                                                       .GetComponentsInChildren<Transform>()];
        FisherYatesShuffle(spawnPoints);

        var players = NetworkSystem.Instance.AllNetPlayers.OrderBy(x => x.ActorNumber).ToArray();
        int myIndex = System.Array.IndexOf(players, NetworkSystem.Instance.LocalPlayer);
        Transform mySpawnpoint = spawnPoints[myIndex];

        GorillaLocomotion.GTPlayer.Instance.TeleportTo(mySpawnpoint);
    }

    public static void TeleportToStump()
    {
        TeleportLocalPlayer(stumpSpawnpoint);
    }

    private static void TeleportLocalPlayer(string anchor)
    {
        var anchorObject = GameObject.Find(anchor);
        if (anchorObject is null)
        {
            Main.Log("Failed to find tp anchor", BepInEx.Logging.LogLevel.Fatal);
            return;
        }
        TeleportLocalPlayer(anchorObject.transform);
    }

    private static void TeleportLocalPlayer(Transform anchor)
    {
        GorillaLocomotion.GTPlayer.Instance.TeleportTo(anchor.transform);
    }

    // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    public static void FisherYatesShuffle(object[] items)
    {
        int n = items.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}
