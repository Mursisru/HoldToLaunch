**Developer:** Mursisru

# Hold To Launch

[![Nuclear Option](https://img.shields.io/badge/Game-Nuclear%20Option-blue)](https://store.steampowered.com/app/2168680/Nuclear_Option/)
[![BepInEx 5](https://img.shields.io/badge/Loader-BepInEx%205-orange)](https://docs.bepinex.dev/)
[![Version](https://img.shields.io/badge/Version-1.0.0-green)](https://github.com/Mursisru/HoldToLaunch/releases/tag/v1.0.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow)](https://github.com/Mursisru/HoldToLaunch/blob/main/LICENSE)

MP-safe **QoL** mod for **[Nuclear Option](https://store.steampowered.com/app/2168680/Nuclear_Option/)**: requires holding **Fire** before missile/bomb release. Guns can be limited to realistic burst length. **Client-side gate only** — no custom network RPCs, no server authority changes.

**Plugin GUID:** `com.at747.holdtolaunch`  
**Version:** `1.0.0`

---

## Table of contents

- [Features](#features)
- [Requirements](#requirements)
- [Install](#install)
- [Configuration](#configuration)
- [Validation checklist](#validation-checklist-spmp)
- [Build](#build)
- [License](#license)

---

## Features

- **Hold-to-release** for missiles and bombs — default hold **0.9 s** before vanilla `WeaponManager.Fire` proceeds.
- **`OUT OF ARC`** geometry — extended hold up to **1.3 s** (configurable) when solution is marginal.
- **Denied states never release** — `SafetyIsOn`, not `Ready`, `SalvoInProgress`, `remoteSim`, etc.
- **Gun burst limiter** — optional auto-cut after **1.0 s** continuous trigger (configurable).
- **Multiplayer safe** — mod sends no custom `Cmd*` / `Rpc*`; launch still occurs only through vanilla `Pilot.Fire → WeaponManager.Fire`.

---

## Requirements

- **[Nuclear Option](https://store.steampowered.com/app/2168680/Nuclear_Option/)** (Steam)
- **[BepInEx 5](https://docs.bepinex.dev/)** x64
- **[Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager)** (recommended)

---

## Install

> [!IMPORTANT]
> **BepInEx 5 (x64) required** — install [BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html) before this mod.

1. Download **`HoldToLaunch_Engine.dll`** from [Releases](https://github.com/Mursisru/HoldToLaunch/releases) or build Release.
2. Copy to:

   ```text
   Nuclear Option\BepInEx\plugins\HoldToLaunch_Engine.dll
   ```

3. Config: `BepInEx\config\com.at747.holdtolaunch.cfg` (created on first run).

---

## Configuration

| Key | Default | Description |
|-----|---------|-------------|
| `DefaultHoldSeconds` | `0.9` | Hold time for valid launch solution |
| `OutOfArcHoldSeconds` | `1.3` | Hold time when `OUT OF ARC` |
| `GunBurstSeconds` | `1.0` | Max continuous gun fire per trigger pull (`0` = off) |

Edit via Configuration Manager or the `.cfg` file while the game is closed.

---

## Validation checklist (SP/MP)

1. Select missile or bomb station; keep gun excluded if testing missiles only.
2. Hold **Fire**:
   - valid solution → release only after `DefaultHoldSeconds` (default **0.9 s**);
   - `OUT OF ARC` → release only after `OutOfArcHoldSeconds` (default **1.3 s**).
3. Denied launch states (`SafetyIsOn`, not `Ready`, `SalvoInProgress`, `remoteSim`) must **never** release.
4. Guns: hold trigger continuously → fire stops after `GunBurstSeconds`; release and press again → next burst starts.
5. Multiplayer: confirm no custom network messages; launch still via vanilla weapon pipeline only.

---

## Build

```powershell
msbuild HoldToLaunch_Engine\HoldToLaunch_Engine.csproj /p:Configuration=Release
```

Output: `HoldToLaunch_Engine\bin\Release\HoldToLaunch_Engine.dll`

---

## License

MIT — see [LICENSE](LICENSE).

---

## Keywords

nuclear-option, bepinex, harmony, mod, hold-to-launch, qol, multiplayer-safe, csharp, unity
