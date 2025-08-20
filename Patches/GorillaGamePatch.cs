using HarmonyLib;

namespace FallMonke.Patches;

[HarmonyPatch(typeof(GorillaGameManager), "GameTypeName")]
public static class GorillaGamePatch
{
    [HarmonyPostfix]
    private static void GameTypeName(GorillaGameManager __instance, ref string __result)
    {
        if (__instance is CustomGameManager)
        {
            __result = "FALLMONKE";
        }
    }
}
