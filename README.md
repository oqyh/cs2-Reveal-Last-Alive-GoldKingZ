## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] Reveal-Last-Alive-GoldKingZ (1.0.1)

Reveal Last Player Alive By Glow/Chicken

![glow](https://github.com/user-attachments/assets/badb4691-d0f4-433e-808c-8ebb7176ed25)


---

## 📦 Dependencies
[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2d2d2d?logo=sourceengine)](https://www.sourcemm.net)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

[![MultiAddonManager](https://img.shields.io/badge/MultiAddonManager-181717?logo=github&logoColor=white)](https://github.com/Source2ZE/MultiAddonManager) [Optional: If You Want Custom Sounds]

[![Auto-Downloader-GoldKingZ](https://img.shields.io/badge/Auto--Downloader--GoldKingZ-FFD700?logo=github&logoColor=black)](https://github.com/oqyh/cs2-Auto-Downloader-GoldKingZ) [Optional: If You Want Custom Sounds]

[![JSON](https://img.shields.io/badge/JSON-000000?logo=json)](https://www.newtonsoft.com/json) [Included in zip]



---

## 📥 Installation

### Plugin Installation
1. Download the latest `Reveal-Last-Alive-GoldKingZ.x.x.x.zip` release
2. Extract contents to your `csgo` directory
3. Configure settings in `Reveal-Last-Alive-GoldKingZ/config/config.json`
4. Restart your server

---

## 🛠️ `config.json`


<details open>
<summary><b>Main Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `Reload_Plugin_CommandsInGame` | Console/Chat commands to reload plugin (Console commands run via console **and** chat using `!`) | `Console_Commands: cmd1,cmd2 \| Chat_Commands: cmd1,cmd2`<br>Both empty = Disabled | - |
| `Reload_Plugin_Flags` | Permissions for reload command | `SteamIDs: ... \| Flags: ... \| Groups: ...`<br>All empty = Allow everyone | `Reload_Plugin_CommandsInGame` not empty |
| `Reload_Plugin_Hide` | Hide chat after executing reload command | `0`-No<br>`1`-Only after successful toggle<br>`2`-Always hide | `Reload_Plugin_CommandsInGame` not empty |
| `RevealLastPlayerOnTeam` | Team to reveal last player | `1`-CT<br>`2`-T<br>`3`-CT + T | - |
| `Play_Sound` | Sound played on reveal | Sound path/event<br>`""`-Disabled | - |
| `Sound_Volume` | Volume for non-"sounds/" audio | `0%` to `100%` | When using without sounds/ |

</details>


<details>
<summary><b>Chicken Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `Chicken_Enable` | Enable chicken indicator | `true`-Yes<br>`false`-No | - |
| `Chicken_GlowType` | Chicken glow visibility | `true`-Near crosshair<br>`false`-Always | `Chicken_Enable=true` |
| `Chicken_GlowRange` | Max chicken glow distance | Number (e.g. `5000`) | `Chicken_Enable=true` |
| `Chicken_Size` | Chicken model size | Number (e.g. `10`) | `Chicken_Enable=true` |
| `Chicken_GlowColor` | Chicken glow color (RGBA) | `R , G, B , A` (e.g. `20 , 255, 0 , 255`)<br>Picker: [rgbacolorpicker.com](https://rgbacolorpicker.com/) | `Chicken_Enable=true` |

</details>


<details>
<summary><b>Player Glow Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `Player_Enable` | Enable player glow | `true`-Yes<br>`false`-No | - |
| `Player_GlowType` | Player glow visibility | `true`-Near crosshair<br>`false`-Always | `Player_Enable=true` |
| `Player_GlowRange` | Max player glow distance | Number (e.g. `5000`) | `Player_Enable=true` |
| `Player_GlowColor` | Player glow color (RGBA) | `R , G, B , A` (e.g. `20 , 255, 0 , 255`)<br>Picker: [rgbacolorpicker.com](https://rgbacolorpicker.com/) | `Player_Enable=true` |

</details>


<details>
<summary><b>Utilities Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `AutoUpdateSignatures` | Auto-Update Signatures | `true`-Yes<br>`false`-No | - |
| `EnableDebug` | Debug Mode | `true`-Enable<br>`false`-Disable | - |

</details>

---


## 📜 Changelog

<details>
<summary><b>📋 View Version History</b> (Click to expand 🔽)</summary>

### [1.0.1]
- Fix Bug Method not found: 'CounterStrikeSharp.API.Modules.Utils.QAngle'
- Added Reload_Plugin_CommandsInGame
- Added Reload_Plugin_Flags
- Added Reload_Plugin_Hide
- Added RevealLastPlayerOnTeam Mode 3
- Change Chicken_GlowColor From Hex To RGBA
- Change Player_GlowColor From Hex To RGBA

### [1.0.0]
- Initial plugin release

</details>

---
