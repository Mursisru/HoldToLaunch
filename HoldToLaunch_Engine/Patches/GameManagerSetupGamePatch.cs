using HarmonyLib;
using HoldToLaunch_Engine.Services;

namespace HoldToLaunch_Engine.Patches
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.SetupGame))]
    internal static class GameManagerSetupGamePatch
    {
        private static void Postfix() => HoldToLaunchGate.ResetAll();
    }
}
