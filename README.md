> [!CAUTION]
> Please follow the installation guide thoroughly and make sure to edit your configuration before using this plug-in. It will NOT have sounds until you add some in the configuration file by yourself. Examples are below!

# CounterstrikeSharp - Quake Sounds

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-quake-sounds?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-quake-sounds/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-map-modifier](https://img.shields.io/github/issues/Kandru/cs2-quake-sounds)](https://github.com/Kandru/cs2-quake-sounds/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

Bring the classic Quake announcer experience to CS2! This plugin adds customizable sound effects for kill streaks, headshots, and other special events. It supports multiple languages and flexible sound configuration.

## Installation

1.  **Download**: Get the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-quake-sounds/releases/).
2.  **Install Plugin**: Extract the `QuakeSounds` folder into your server's `/addons/counterstrikesharp/plugins/` directory.
3.  **Install Dependencies**: Download and install [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager). This is required to ensure players download the sound files.
4.  **Add Sounds**: Configure MultiAddonManager to include a Quake Sounds workshop addon. We recommend using ours: [Steam Workshop ID 3461824328](https://steamcommunity.com/sharedfiles/filedetails/?id=3461824328), alternatively generate your own Workshop Addon by using our [quake-sounds-resources](https://github.com/Kandru/cs2-quake-sounds-resources).
5.  **Restart**: Restart your server to load the plugin and addon manager.
6.  **Configure**: Open `/addons/counterstrikesharp/configs/plugins/QuakeSounds/QuakeSounds.json` and add your desired sounds. **Note:** The plugin comes with no sounds enabled by default!

*Tip: For automatic updates, use our [CS2 Update Manager](https://github.com/Kandru/cs2-update-manager/).*

## Configuration

The configuration file is located at `/addons/counterstrikesharp/configs/plugins/QuakeSounds/QuakeSounds.json`.

### Example Configuration

```json
{
  "enabled": true,
  "debug": false,
  "global": {
    "enabled_during_warmup": true,
    "play_on_entity": "player",
    "sound_hearable_by": "all",
    "ignore_bots": true,
    "ignore_world_damage": true
  },
  "precache": {
    "soundevent_file": "soundevents/soundevents_quakesounds.vsndevts"
  },
  "count_self_kills": false,
  "count_team_kills": false,
  "reset_kills_on_death": true,
  "reset_kills_on_round_start": true,
  "commands": {
    "settings": "qs",
    "settings_menu": false
  },
  "messages": {
    "enable_center_message": true,
    "center_message_type": "default",
    "enable_chat_message": true
  },
  "sound_priorities": {
    "special_events": 1,
    "weapons": 2,
    "kill_streak": 3
  },
  "sounds": {
    "3": {
      "de": "Dreifach-Kill",
      "en": "Triple Kill",
      "_sound": "QuakeSoundsD.Triplekill"
    },
    "5": {
      "de": "Multi-Kill",
      "en": "Multi Kill",
      "_sound": "QuakeSoundsD.Multikill"
    },
    "6": {
      "de": "Randale",
      "en": "Rampage",
      "_sound": "QuakeSoundsD.Rampage"
    },
    "7": {
      "de": "Abschuss-Serie",
      "en": "Killing Spree",
      "_sound": "QuakeSoundsD.Killingspree"
    },
    "8": {
      "de": "Dominierend",
      "en": "Dominating",
      "_sound": "QuakeSoundsD.Dominating"
    },
    "9": {
      "de": "Beeindruckend",
      "en": "Impressive",
      "_sound": "QuakeSoundsD.Impressive"
    },
    "10": {
      "de": "Unstoppbar",
      "en": "Unstoppable",
      "_sound": "QuakeSoundsD.Unstoppable"
    },
    "lastmanstanding": {
      "de": "Letzter Mann",
      "en": "Last Man Standing",
      "_sound": "QuakeSoundsF.Lastmanstanding"
    },
    "firstblood": {
      "de": "Erster Abschuss",
      "en": "First Blood",
      "_sound": "QuakeSoundsD.Firstblood"
    },
    "knifekill": {
      "de": "Messer-Kill",
      "en": "Knife Kill",
      "_sound": "QuakeSoundsD.Haha"
    },
    "teamkill": {
      "de": "Team-Kill",
      "en": "Team Kill",
      "_sound": "QuakeSoundsD.Teamkiller"
    },
    "selfkill": {
      "de": "Selbstt\u00F6tung",
      "en": "Self Kill",
      "_sound": "QuakeSoundsD.Perfect"
    },
    "headshot": {
      "de": "Kopfschuss",
      "en": "Headshot",
      "_sound": "QuakeSoundsD.Headshot"
    },
    "weapon_hegrenade": {
      "de": "Granaten-Kill",
      "en": "Grenade Kill",
      "_sound": "QuakeSoundsD.Perfect"
    },
    "round_start": {
      "_sound": "QuakeSoundsD.Prepare"
    },
    "round_freeze_end": {
      "_sound": "QuakeSoundsD.Play"
    },
    "bomb_30": {
    "_sound": "BombSounds.Sec30"
    },
    "bomb_20": {
    "_sound": "BombSounds.Sec20"
    },
    "bomb_10": {
    "_sound": "BombSounds.Sec10"
    },
    "bomb_9": {
    "_sound": "BombSounds.Sec9"
    },
    "bomb_8": {
    "_sound": "BombSounds.Sec8"
    },
    "bomb_7": {
    "_sound": "BombSounds.Sec7"
    },
    "bomb_6": {
    "_sound": "BombSounds.Sec6"
    },
    "bomb_5": {
    "_sound": "BombSounds.Sec5"
    },
    "bomb_4": {
    "_sound": "BombSounds.Sec4"
    },
    "bomb_3": {
    "_sound": "BombSounds.Sec3"
    },
    "bomb_2": {
    "_sound": "BombSounds.Sec2"
    },
    "bomb_1": {
    "_sound": "BombSounds.Sec1"
    }
  },
  "data": {
    "player_muted": [
      123456789
    ],
    "player_languages": {
      "123456789": "de"
    }
  },
  "ConfigVersion": 1
}
```

### Configuration Guide

#### Global Settings
*   **`global`**: Controls general behavior.
    *   `play_on_entity`: "player" (sound follows player) or "world" (fixed position).
    *   `sound_hearable_by`: Who hears the sound. Options: `all`, `attacker_team`, `victim_team`, `involved`, `attacker`, `victim`, `spectator`.
*   **`precache`**: Points to the sound event file in your workshop addon. **Important:** If you use a custom addon, ensure this matches the file inside the addon.

#### Defining Sounds (`sounds`)
This is the core of the configuration. You can map sounds to:
*   **Kill Streaks**: Use the number of kills as the key (e.g., `"3"`, `"5"`).
*   **Special Events**: Use one of the following event names:
    *   `round_start`
    *   `round_end`
    *   `round_freeze_end`
    *   `bomb_planted`
    *   `bomb_defused`
    *   `bomb_exploded`
    *   `bomb_<SECONDS>` (e.g., `bomb_10`, `bomb_5`)
    *   `lastmanstanding`
    *   `firstblood`
    *   `headshot`
    *   `knifekill`
    *   `selfkill`
    *   `teamkill`
*   **Weapons**: Use `"weapon_<name>"` (e.g., `"weapon_hegrenade"`) to play sounds for specific weapon kills.

**Sound Entry Format:**
Each sound entry requires:
1.  **Translations**: Text to display (e.g., `"en": "Headshot"`).
2.  **`_sound`**: The sound to play.
    *   **Option A (Recommended):** Use a SoundEvent name (e.g., `"QuakeSoundsD.Headshot"`). These are defined in the workshop addon, are positional, and respect user volume settings.
    *   **Option B:** Use a direct file path (e.g., `"sounds/mysounds/headshot.vsnd"`). These play at full volume and are not positional.
3.  **`_filter`** (Optional): Override who hears this specific sound (e.g., `"attacker"`).

#### Messages & Commands
*   **`messages`**:
    *   `enable_center_message`: Toggle center screen announcements.
    *   `center_message_type`: The visual style of the center message. Options: `default`, `alert`, `html`.
    *   `enable_chat_message`: Toggle chat announcements.
*   **`commands`**: Customize the command to open the menu (default: `qs`).

#### Sound Priorities (`sound_priorities`)
When multiple sound events occur simultaneously (e.g., a headshot that is also a killstreak), this setting determines which one takes precedence. **Lower numbers mean higher priority.**
*   `special_events`: Events like headshot, knife kill, round end.
*   `weapons`: Specific weapon kill sounds.
*   `kill_streak`: Kill streak sounds (Multi Kill, etc.).

## Commands

### Player Commands
*   `!qs` (or configured command): Open the settings menu to toggle sounds or mute specific types.
*   `!lang <language>`: Set your preferred language for announcements.

### Admin Commands (Console)
*   `quakesounds reload`: Reload the configuration.
*   `quakesounds disable`: Globally disable sounds.
*   `quakesounds enable`: Globally enable sounds.

## Available Sounds (Workshop Addon)

If you use the recommended Workshop Addon (ID: `3461824328`), you can use these sound keys in your config:

<details>
<summary>Click to view Male Sounds</summary>

`QuakeSoundsD.Assassin`, `QuakeSoundsD.Bullseye`, `QuakeSoundsD.Comboking`, `QuakeSoundsD.Combowhore`, `QuakeSoundsD.Dominating`, `QuakeSoundsD.Doublekill`, `QuakeSoundsD.Eagleeye`, `QuakeSoundsD.Excellent`, `QuakeSoundsD.Firstblood`, `QuakeSoundsD.Flawless`, `QuakeSoundsD.Godlike`, `QuakeSoundsD.Haha`, `QuakeSoundsD.Hattrick`, `QuakeSoundsD.Headhunter`, `QuakeSoundsD.Headshot`, `QuakeSoundsD.Holyshit`, `QuakeSoundsD.Humiliating`, `QuakeSoundsD.Humiliation`, `QuakeSoundsD.Impressive`, `QuakeSoundsD.Killingmachine`, `QuakeSoundsD.Killingspree`, `QuakeSoundsD.Ludicrouskill`, `QuakeSoundsD.Maniac`, `QuakeSoundsD.Massacre`, `QuakeSoundsD.Megakill`, `QuakeSoundsD.Monsterkill`, `QuakeSoundsD.Multikill`, `QuakeSoundsD.Outstanding`, `QuakeSoundsD.Ownage`, `QuakeSoundsD.Pancake`, `QuakeSoundsD.Payback`, `QuakeSoundsD.Perfect`, `QuakeSoundsD.Play`, `QuakeSoundsD.Prepare`, `QuakeSoundsD.Rampage`, `QuakeSoundsD.Retribution`, `QuakeSoundsD.Teamkiller`, `QuakeSoundsD.Triplekill`, `QuakeSoundsD.Ultrakill`, `QuakeSoundsD.Unstoppable`, `QuakeSoundsD.Vengeance`, `QuakeSoundsD.Wickedsick`
</details>

<details>
<summary>Click to view Female Sounds</summary>

`QuakeSoundsF.Bottomfeeder`, `QuakeSoundsF.Dominating`, `QuakeSoundsF.Firstblood`, `QuakeSoundsF.Godlike`, `QuakeSoundsF.Headshot`, `QuakeSoundsF.Hexakill`, `QuakeSoundsF.Holyshit`, `QuakeSoundsF.Humiliation`, `QuakeSoundsF.Killingspree`, `QuakeSoundsF.Monsterkill`, `QuakeSoundsF.Multikill`, `QuakeSoundsF.Pentakill`, `QuakeSoundsF.Prepare`, `QuakeSoundsF.Quadrakill`, `QuakeSoundsF.Rampage`, `QuakeSoundsF.Shutdown`, `QuakeSoundsF.Ultrakill`, `QuakeSoundsF.Unstoppable`, `QuakeSoundsF.Wickedsick`, `QuakeSoundsF.Lastmanstanding`
</details>

<details>
<summary>Click to view Bomb Sounds</summary>

`BombSounds.Sec1` through `BombSounds.Sec10`, `BombSounds.Sec20`, `BombSounds.Sec30`
</details>

## Troubleshooting

**I don't hear any sounds!**
1.  **Check MultiAddonManager**: Ensure it is installed and running without errors in the server console.
2.  **Check Downloads**: When you connect to the server, do you see a download for the workshop addon? If not, the client doesn't have the files.
3.  **Check Console**: Upon connection, you should see logs from MultiAddonManager confirming the addon download.
4.  **Metamod Version**: Ensure your Metamod is up to date, or MultiAddonManager may fail.

## Development

To build the plugin from source:

1.  Clone the repo: `git clone https://github.com/Kandru/cs2-quake-sounds.git`
2.  Restore dependencies: `dotnet restore`
3.  Build: `dotnet build` (Debug) or `dotnet publish` (Release)

## License & Authors

Released under [GPLv3](/LICENSE) by [@Kandru](https://github.com/Kandru).

**Authors:**
- [@derkalle4](https://www.github.com/derkalle4)
