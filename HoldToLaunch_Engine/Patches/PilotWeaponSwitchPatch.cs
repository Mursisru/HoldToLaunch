using HarmonyLib;
using HoldToLaunch_Engine.Services;
using Rewired;

namespace HoldToLaunch_Engine.Patches
{
    [HarmonyPatch(typeof(Pilot), nameof(Pilot.NextWeapon))]
    internal static class PilotNextWeaponPatch
    {
        private static void Postfix(Pilot __instance)
        {
            if (!(__instance?.currentState is PilotPlayerState))
                return;

            var player = ReInput.players.GetPlayer(0);
            var fireHeld = player != null && player.GetButton("Fire");
            HoldToLaunchGate.OnWeaponStationChanged(__instance, fireHeld);
        }
    }

    [HarmonyPatch(typeof(Pilot), nameof(Pilot.PreviousWeapon))]
    internal static class PilotPreviousWeaponPatch
    {
        private static void Postfix(Pilot __instance)
        {
            if (!(__instance?.currentState is PilotPlayerState))
                return;

            var player = ReInput.players.GetPlayer(0);
            var fireHeld = player != null && player.GetButton("Fire");
            HoldToLaunchGate.OnWeaponStationChanged(__instance, fireHeld);
        }
    }
}
