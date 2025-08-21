using UnityEngine;
using System.Linq;

namespace FallMonke;

public static class TeleportController
{
    public static void TeleportToLobby()
    {
        TeleportLocalPlayer("/LobbySpawnpoint");
    }

    public static void TeleportToGame(int seed)
    {
        if (CustomGameManager.instance is not CustomGameManager manager)
            return;

        UnityEngine.Random.InitState(seed);
        Vector3[] spawnPoints = GameObject.Find("/SpawnPoints")
                                          .GetComponentsInChildren<Transform>()
                                          .ToList()
                                          .Select(x => x.position)
                                          .ToArray();
        FisherYatesShuffle(spawnPoints);

        var players = NetworkSystem.Instance.AllNetPlayers.OrderBy(x => x.ActorNumber).ToArray();
        int myIndex = System.Array.IndexOf(players, NetworkSystem.Instance.LocalPlayer);
        Vector3 mySpawnpoint = spawnPoints[myIndex];

        TeleportLocalPlayer(mySpawnpoint);
    }

    private static void TeleportLocalPlayer(string anchor)
    {
        var anchorObject = GameObject.Find(anchor);
        if (anchorObject == null)
        {
            Main.Log("Failed to find tp anchor", BepInEx.Logging.LogLevel.Fatal);
            return;
        }
        TeleportLocalPlayer(anchorObject.transform.position);
    }

    private static void TeleportLocalPlayer(Vector3 position)
    {
        GorillaLocomotion.GTPlayer.Instance.TeleportTo(position, Quaternion.identity, false);
    }

    // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    private static void FisherYatesShuffle(Vector3[] positions)
    {
        int n = positions.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Vector3 temp = positions[i];
            positions[i] = positions[j];
            positions[j] = temp;
        }
    }
}
