using UnityEngine;

namespace HoldToLaunch_Engine.Services
{
    internal static class HoldToLaunchConfigCache
    {
        private static int _frame = -1;

        internal static bool Enabled;
        internal static float DefaultHoldSeconds;
        internal static float OutOfArcHoldSeconds;
        internal static float MinHoldSeconds;
        internal static float MaxHoldSeconds;
        internal static bool GunBurstLimiterEnabled;
        internal static float GunBurstSeconds;
        internal static bool DebugLog;

        internal static void Refresh()
        {
            int frame = Time.frameCount;
            if (frame == _frame)
                return;
            _frame = frame;

            Enabled = HoldToLaunchPlugin.Enabled != null && HoldToLaunchPlugin.Enabled.Value;
            DefaultHoldSeconds = HoldToLaunchPlugin.DefaultHoldSeconds != null
                ? HoldToLaunchPlugin.DefaultHoldSeconds.Value
                : 0.35f;
            OutOfArcHoldSeconds = HoldToLaunchPlugin.OutOfArcHoldSeconds != null
                ? HoldToLaunchPlugin.OutOfArcHoldSeconds.Value
                : 0.55f;
            MinHoldSeconds = HoldToLaunchPlugin.MinHoldSeconds != null
                ? HoldToLaunchPlugin.MinHoldSeconds.Value
                : 0.05f;
            MaxHoldSeconds = HoldToLaunchPlugin.MaxHoldSeconds != null
                ? HoldToLaunchPlugin.MaxHoldSeconds.Value
                : 2f;
            GunBurstLimiterEnabled = HoldToLaunchPlugin.GunBurstLimiterEnabled != null
                && HoldToLaunchPlugin.GunBurstLimiterEnabled.Value;
            GunBurstSeconds = HoldToLaunchPlugin.GunBurstSeconds != null
                ? HoldToLaunchPlugin.GunBurstSeconds.Value
                : 0.8f;
            DebugLog = HoldToLaunchPlugin.DebugLog != null && HoldToLaunchPlugin.DebugLog.Value;
        }
    }
}
