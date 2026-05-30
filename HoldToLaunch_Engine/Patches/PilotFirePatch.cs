using HarmonyLib;
using HoldToLaunch_Engine.Services;

namespace HoldToLaunch_Engine.Patches
{
    [HarmonyPatch(typeof(Pilot), nameof(Pilot.Fire))]
    internal static class PilotFirePatch
    {
        private static bool Prefix(Pilot __instance) => HoldToLaunchGate.ShouldAllowFire(__instance);
    }
}
