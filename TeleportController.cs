using UnityEngine;

namespace FallMonke;

public static class TeleportController
{
    public static void TeleportToLobby()
    {
        TeleportLocalPlayer("/LobbySpawnpoint");
    }

    public static void TeleportToGame()
    {
        if (CustomGameManager.Instance is not CustomGameManager manager)
            return;

        int index = manager.Players.IndexOfRef(manager.LocalPlayer);
        Transform spawnPoint = GameObject.Find("/Spawnpoints/").transform.GetChild(index);

        TeleportLocalPlayer(spawnPoint.position);
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
}
