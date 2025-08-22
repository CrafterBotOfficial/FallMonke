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
        Transform[] spawnPoints = GameObject.Find("/SpawnPoints")
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
        Vector3 position = new Vector3(-67, 11, -82); // todo: make more precise, should be perfect center
        TeleportLocalPlayer(position);
    }

    private static void TeleportLocalPlayer(string anchor)
    {
        var anchorObject = GameObject.Find(anchor);
        if (anchorObject == null)
        {
            Main.Log("Failed to find tp anchor", BepInEx.Logging.LogLevel.Fatal);
            return;
        }
        GorillaLocomotion.GTPlayer.Instance.TeleportTo(anchorObject.transform);
    }

    private static void TeleportLocalPlayer(Vector3 position)
    {
        var headRotation = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.rotation;
        Vector3 playerVRCenterOffset = Camera.main.transform.position - GorillaLocomotion.GTPlayer.Instance.transform.position;
        Vector3 floorOffset = new Vector3(playerVRCenterOffset.x, 0, playerVRCenterOffset.z);
        GorillaLocomotion.GTPlayer.Instance.TeleportTo(position - playerVRCenterOffset, headRotation, false);
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
