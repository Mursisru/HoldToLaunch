using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace HoldToLaunch_Engine
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class HoldToLaunchPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.at747.holdtolaunch";
        public const string PluginName = "Hold To Launch";
        public const string PluginVersion = AppVersion.ReleaseBase;

        internal static ManualLogSource Log { get; private set; }

        internal static ConfigEntry<bool> Enabled { get; private set; }
        internal static ConfigEntry<float> DefaultHoldSeconds { get; private set; }
        internal static ConfigEntry<float> OutOfArcHoldSeconds { get; private set; }
        internal static ConfigEntry<float> MinHoldSeconds { get; private set; }
        internal static ConfigEntry<float> MaxHoldSeconds { get; private set; }
        internal static ConfigEntry<bool> GunBurstLimiterEnabled { get; private set; }
        internal static ConfigEntry<float> GunBurstSeconds { get; private set; }
        internal static ConfigEntry<bool> DebugLog { get; private set; }

        private void Awake()
        {
            Log = Logger;

            Enabled = Config.Bind("General", "Enabled", true,
                "Enable hold-to-launch gate for all non-gun weapons.");
            DefaultHoldSeconds = Config.Bind("Timing", "DefaultHoldSeconds", 0.9f,
                new ConfigDescription("Default hold time when launch solution is valid.",
                    new AcceptableValueRange<float>(0.6f, 1.3f)));
            OutOfArcHoldSeconds = Config.Bind("Timing", "OutOfArcHoldSeconds", 1.3f,
                new ConfigDescription("Hold time used while target is OUT OF ARC.",
                    new AcceptableValueRange<float>(0.6f, 1.3f)));
            MinHoldSeconds = Config.Bind("Timing", "MinHoldClamp", 0.6f,
                new ConfigDescription("Lower clamp for hold timings.",
                    new AcceptableValueRange<float>(0.6f, 1.3f)));
            MaxHoldSeconds = Config.Bind("Timing", "MaxHoldClamp", 1.3f,
                new ConfigDescription("Upper clamp for hold timings.",
                    new AcceptableValueRange<float>(0.6f, 1.3f)));
            GunBurstLimiterEnabled = Config.Bind("Gun", "GunBurstLimiterEnabled", true,
                "If true, continuous gun fire is auto-limited to burst length.");
            GunBurstSeconds = Config.Bind("Gun", "GunBurstSeconds", 1.0f,
                new ConfigDescription("Max continuous gun burst duration in seconds. Release trigger to start next burst.",
                    new AcceptableValueRange<float>(0.4f, 2.0f)));
            DebugLog = Config.Bind("Debug", "DebugLog", false, "Enable verbose logging.");

            new Harmony(PluginGuid).PatchAll();
            Logger.LogInfo($"{PluginName} {AppVersion.Display} loaded.");
        }
    }
}
