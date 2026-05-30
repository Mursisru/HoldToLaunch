using HarmonyLib;
using HoldToLaunch_Engine.Services;
using Rewired;

namespace HoldToLaunch_Engine.Patches
{
    [HarmonyPatch(typeof(PilotPlayerState), "PlayerControls")]
    internal static class PilotPlayerStatePatch
    {
        private static readonly AccessTools.FieldRef<PilotPlayerState, Player> PlayerField =
            AccessTools.FieldRefAccess<PilotPlayerState, Player>("player");

        private static readonly AccessTools.FieldRef<PilotPlayerState, Pilot> PilotField =
            AccessTools.FieldRefAccess<PilotPlayerState, Pilot>("pilot");

        private static void Prefix(PilotPlayerState __instance)
        {
            var pilot = PilotField(__instance);
            var player = PlayerField(__instance);
            if (pilot == null || player == null)
                return;

            HoldToLaunchGate.UpdateFireInput(pilot, player.GetButton("Fire"));
        }
    }
}
