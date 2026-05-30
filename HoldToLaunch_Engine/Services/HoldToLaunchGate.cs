using System.Collections.Generic;
using UnityEngine;

namespace HoldToLaunch_Engine.Services
{
    internal enum LaunchPermissionState
    {
        Denied,
        Allowed,
        OutOfArc
    }

    internal static class HoldToLaunchGate
    {
        private sealed class PilotHoldState
        {
            internal float HoldStart;
            internal float LastSeen;
            internal bool FiredThisPress;
            internal float GunBurstStart;
            internal bool GunBlockedUntilRelease;
            internal int LastStation;
            internal bool BlockUntilRelease;
            internal bool HasStation;
            internal bool FireHeld;
        }

        private static readonly Dictionary<int, PilotHoldState> States = new Dictionary<int, PilotHoldState>();

        internal static void ResetAll() => States.Clear();

        internal static void UpdateFireInput(Pilot pilot, bool fireHeld)
        {
            if (pilot == null)
                return;

            var now = Time.unscaledTime;
            var state = GetOrCreateState(pilot.GetInstanceID(), now);
            var wasHeld = state.FireHeld;
            state.FireHeld = fireHeld;
            state.LastSeen = now;

            if (!fireHeld)
            {
                state.FiredThisPress = false;
                state.GunBlockedUntilRelease = false;
                state.BlockUntilRelease = false;
                return;
            }

            if (!wasHeld)
            {
                state.HoldStart = now;
                state.FiredThisPress = false;
                state.GunBurstStart = now;
                state.GunBlockedUntilRelease = false;
            }
        }

        internal static void OnWeaponStationChanged(Pilot pilot, bool fireHeld)
        {
            if (pilot?.aircraft?.weaponManager?.currentWeaponStation == null)
                return;

            var station = pilot.aircraft.weaponManager.currentWeaponStation;
            var now = Time.unscaledTime;
            var state = GetOrCreateState(pilot.GetInstanceID(), now);

            if (state.HasStation && state.LastStation == station.Number)
                return;

            state.HasStation = true;
            state.LastStation = station.Number;
            state.FiredThisPress = false;
            state.GunBurstStart = now;
            state.GunBlockedUntilRelease = false;

            if (fireHeld)
            {
                state.BlockUntilRelease = true;
                return;
            }

            state.BlockUntilRelease = false;
        }

        internal static bool ShouldAllowFire(Pilot pilot)
        {
            HoldToLaunchConfigCache.Refresh();
            if (!HoldToLaunchConfigCache.Enabled)
                return true;
            if (pilot == null || pilot.aircraft == null || pilot.aircraft.weaponManager == null)
                return true;
            if (!(pilot.currentState is PilotPlayerState))
                return true;

            var station = pilot.aircraft.weaponManager.currentWeaponStation;
            if (station == null || station.WeaponInfo == null)
                return false;

            var now = Time.unscaledTime;
            var state = GetOrCreateState(pilot.GetInstanceID(), now);

            if (!state.HasStation)
            {
                state.HasStation = true;
                state.LastStation = station.Number;
            }
            else if (state.LastStation != station.Number)
                OnWeaponStationChanged(pilot, state.FireHeld);

            if (state.BlockUntilRelease)
                return false;

            if (station.WeaponInfo.gun)
                return HandleGunBurst(state, now);

            var permission = EvaluatePermission(pilot.aircraft.weaponManager, pilot.aircraft, station);
            if (permission == LaunchPermissionState.Denied)
                return false;

            var requiredHold = permission == LaunchPermissionState.OutOfArc
                ? HoldToLaunchConfigCache.OutOfArcHoldSeconds
                : HoldToLaunchConfigCache.DefaultHoldSeconds;

            requiredHold = ClampHold(requiredHold);

            if (state.FiredThisPress)
                return false;

            if (now - state.HoldStart < requiredHold)
                return false;

            state.FiredThisPress = true;
            if (HoldToLaunchConfigCache.DebugLog)
            {
                HoldToLaunchPlugin.Log?.LogInfo(
                    $"Launch released after hold={requiredHold:0.00}s state={permission}");
            }

            return true;
        }

        private static LaunchPermissionState EvaluatePermission(
            WeaponManager weaponManager,
            Aircraft aircraft,
            WeaponStation station)
        {
            if (weaponManager == null || station == null || station.WeaponInfo == null)
                return LaunchPermissionState.Denied;
            if (aircraft.remoteSim)
                return LaunchPermissionState.Denied;
            if (station.SafetyIsOn(aircraft))
                return LaunchPermissionState.Denied;
            if (!station.Ready() || station.SalvoInProgress)
                return LaunchPermissionState.Denied;

            if (station.WeaponInfo.bomb)
                return EvaluateBombPermission(weaponManager, aircraft, station);

            var requirements = station.WeaponInfo.targetRequirements;
            if (requirements.minAlignment <= 0f)
                return LaunchPermissionState.Allowed;

            var targets = weaponManager.GetTargetList();
            if (targets == null || targets.Count == 0 || targets[0] == null)
                return LaunchPermissionState.Denied;

            var toTarget = (Vector3)(targets[0].GlobalPosition() - aircraft.GlobalPosition());
            if (toTarget.sqrMagnitude < 0.0001f)
                return LaunchPermissionState.Allowed;

            var cameraManager = SceneSingleton<CameraStateManager>.i;
            var forward = (cameraManager != null && cameraManager.mainCamera != null)
                ? cameraManager.mainCamera.transform.forward
                : Vector3.forward;
            var targetAngle = Vector3.Angle(forward, toTarget.normalized);
            return targetAngle > requirements.minAlignment
                ? LaunchPermissionState.OutOfArc
                : LaunchPermissionState.Allowed;
        }

        private static LaunchPermissionState EvaluateBombPermission(
            WeaponManager weaponManager,
            Aircraft aircraft,
            WeaponStation station)
        {
            var targets = weaponManager.GetTargetList();
            if (targets == null || targets.Count == 0)
                return LaunchPermissionState.Denied;

            var sum = Vector3.zero;
            var count = 0;
            for (var i = 0; i < targets.Count; i++)
            {
                var unit = targets[i];
                if (unit == null || unit.disabled)
                    continue;
                if (!aircraft.NetworkHQ.TryGetKnownPosition(unit, out var knownPos))
                    continue;
                sum += knownPos.ToLocalPosition();
                count++;
            }

            if (count == 0)
                return LaunchPermissionState.Denied;

            var avg = sum / count;
            var avgTarget = avg.ToGlobalPosition() + Vector3.up * station.WeaponInfo.airburstHeight;
            var toTarget = avgTarget - aircraft.GlobalPosition();
            if (toTarget.sqrMagnitude < 0.001f)
                return LaunchPermissionState.Denied;

            var pitchDotUp = Vector3.Dot(aircraft.transform.forward, Vector3.up);
            if (pitchDotUp > -0.01f)
            {
                var altitudeToTarget = -toTarget.y;
                var horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
                var horizontalDist = horizontal.magnitude;
                var vel = aircraft.rb.velocity;
                var speed = vel.magnitude;
                if (speed < 1f || altitudeToTarget <= 0f)
                    return LaunchPermissionState.Denied;

                var fallTime = Kinematics.FallTime(altitudeToTarget, vel.y);
                var alongTrack = Vector3.Dot(vel.normalized, horizontal.normalized);
                if (Mathf.Abs(alongTrack) < 0.001f)
                    return LaunchPermissionState.Denied;

                var rel = horizontalDist / (alongTrack * speed) - fallTime;
                return rel >= -5f && rel <= 5f
                    ? LaunchPermissionState.Allowed
                    : LaunchPermissionState.Denied;
            }

            return LaunchPermissionState.Allowed;
        }

        private static float ClampHold(float value)
        {
            var min = HoldToLaunchConfigCache.MinHoldSeconds;
            var max = HoldToLaunchConfigCache.MaxHoldSeconds;
            if (max < min)
                max = min;
            return Mathf.Clamp(value, min, max);
        }

        private static bool HandleGunBurst(PilotHoldState state, float now)
        {
            if (!HoldToLaunchConfigCache.GunBurstLimiterEnabled)
                return true;

            if (state.GunBlockedUntilRelease)
                return false;

            var burstSeconds = Mathf.Clamp(HoldToLaunchConfigCache.GunBurstSeconds, 0.4f, 2.0f);
            if (now - state.GunBurstStart < burstSeconds)
                return true;

            state.GunBlockedUntilRelease = true;
            if (HoldToLaunchConfigCache.DebugLog)
            {
                HoldToLaunchPlugin.Log?.LogInfo(
                    $"Gun burst stopped at {burstSeconds:0.00}s; waiting trigger release.");
            }

            return false;
        }

        private static PilotHoldState GetOrCreateState(int pilotId, float now)
        {
            if (!States.TryGetValue(pilotId, out var state))
            {
                state = new PilotHoldState
                {
                    LastStation = -1,
                    HoldStart = now,
                    LastSeen = now
                };
                States[pilotId] = state;
            }

            return state;
        }
    }
}
