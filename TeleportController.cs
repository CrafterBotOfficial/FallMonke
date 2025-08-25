using UnityEngine;
using System.Linq;

namespace FallMonke;

public static class TeleportController
{
    private static Transform stumpSpawnpoint;

    public static void CreateStumpAnchor()
    {
        Vector3 position = new Vector3(-67, 13, -82); // todo: make more precise, should be perfect center
        stumpSpawnpoint = new GameObject().transform;
        stumpSpawnpoint.position = position;
    }

    public static void TeleportToLobby()
    {
        TeleportLocalPlayer("/LobbySpawnpoint");
    }

    public static void TeleportToGame(int seed)
    {
        if (CustomGameManager.instance is not CustomGameManager manager)
            return;

        UnityEngine.Random.InitState(seed);
        Transform[] spawnPoints = WorldManager.GetParent()
                                              .Find("/SpawnPoints")
                                              .GetComponentsInChildren<Transform>()
                                              .ToList()
                                              .ToArray();
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
        if (anchorObject == null)
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
    private static void FisherYatesShuffle(Transform[] positions)
    {
        int n = positions.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Transform temp = positions[i];
            positions[i] = positions[j];
            positions[j] = temp;
        }
    }
}
