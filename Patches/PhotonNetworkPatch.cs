using HarmonyLib;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace FallMonke.Patches;

[HarmonyPatch(typeof(Room), nameof(Room.SetCustomProperties))]
public static class PhotonNetworkPatch
{
    [HarmonyPostfix]
    private static void Debug_Log(Hashtable propertiesToSet)
    {
        Main.Log("===== ROOM CUSTOM PROP CHANGE", BepInEx.Logging.LogLevel.Message);
        Main.Log(propertiesToSet.ToString());
        Main.Log(System.Environment.StackTrace);
    }
}
