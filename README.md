# Hold To Launch

MP-safe QoL mod for Nuclear Option.

- Requires holding `Fire` before missile/bomb release.
- Default hold: `0.9s`.
- `OUT OF ARC`: hold up to configured max (`1.3s` by default).
- Guns can be auto-limited to realistic bursts (default `1.0s`).
- No server-side mechanic changes (client-side gate only).

## Validation Checklist (SP/MP)

1. Select missile or bomb station, keep gun excluded.
2. Hold `Fire`:
   - valid solution -> release only after `DefaultHoldSeconds` (default `0.9s`);
   - `OUT OF ARC` geometry -> release only after `OutOfArcHoldSeconds` (default `1.3s`).
3. Denied launch states (`SafetyIsOn`, not `Ready`, `SalvoInProgress`, `remoteSim`) must never release.
4. Guns: hold trigger continuously -> fire stops after `GunBurstSeconds`; release and press again -> next burst starts.
5. Multiplayer safety check:
   - mod sends no custom `Cmd*`/`Rpc*`;
   - actual launch still occurs only through vanilla `Pilot.Fire -> WeaponManager.Fire`.
