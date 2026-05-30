# Changelog

## 1.0.0 — 2026-05-30

### Performance
- HoldToLaunchConfigCache: BepInEx settings cached once per frame in ShouldAllowFire (no gate logic change).

## 1.0.0 Build DEV1H8

- Fixed double-hold after weapon switch: `BlockUntilRelease` now uses real `Fire` button state, not `LastSeen` timing gap.
- Weapon switch handled in `NextWeapon`/`PreviousWeapon` patches; hold timer resets on new trigger press only.

## 1.0.0 Build DEV1H1

- Initial scaffold.
- MP-safe hold-to-launch gate for all non-gun weapons.
- Configurable hold timings via BepInEx Configuration Manager.

## 1.0.0 Build DEV1H2

- Added realistic gun burst limiter: continuous cannon fire auto-stops after configured burst time.
- To fire next burst, trigger must be released and pressed again.
- Added `GunBurstLimiterEnabled` and `GunBurstSeconds` config options.

## 1.0.0 Build DEV1H3

- Fixed launch gate: no target now blocks non-gun launch release.
- Fixed station switch behavior: switching weapon while holding fire now requires full trigger release and re-hold before next launch.

## 1.0.0 Build DEV1H4

- Bomb-specific launch conditions added:
  - level/up-pitch (REL-like) mode: release allowed only when `REL` is within `[-5, +5]`;
  - dive (CCIP-like) mode: release allowed whenever at least one valid target exists.

## 1.0.0 Build DEV1H5

- Fixed inconsistent launch behavior requiring multiple holds:
  - removed hard state reset on temporary `Denied` conditions;
  - corrected first-seen station initialization to avoid false extra release cycles;
  - station switch still correctly requires full release and re-hold.

## 1.0.0 Build DEV1H6

- Fixed station-switch gate flakiness:
  - release/re-hold is now required only when station changes during continuous trigger hold;
  - normal station change after proper trigger release no longer forces an extra redundant cycle.
- Reduced release detection gap to improve first-attempt consistency.

## 1.0.0 Build DEV1H7

- Fixed intermittent double-hold issue by tracking actual `Fire` button state each frame via `PilotPlayerState` patch.
- Release reset no longer depends only on call timing gaps, which were FPS-sensitive.
- Increased fallback release gap tolerance for low-FPS moments.
