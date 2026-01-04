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

Global settings for this plug-in.

#### enabled_during_warmup

Whether or not quake sounds are enabled during warmup.

#### play_on_entity

Determines where the sound will play: either at the player's position or at a fixed world position. Note that using a world position can result in poorly placed sounds on custom maps, making them hard to hear. Playing the sound at the player's position may reveal their location. If you use a full sound path instead of a sound name, the sound will play at maximum volume without directional effects or volume control at the player regardless of this setting, similar to opening a media player and playing the sound.

Possible options:

- player
- world

#### sound_hearable_by

Who is able to listen to sounds. Defaults to "all":

- all -> everyone will hear the sound
- attacker_team -> everyone in the attacker team will hear the sound
- victim_team -> everyone in the victim team will hear the sound
- involved -> both attacker and victim will hear the sound
- attacker -> only the attacker will hear the sound
- victim -> only the victim will hear the sound
- spectator -> only spectators will hear the sound

#### ignore_bots

Whether or not bots will make quake sounds. If disabled only players will make quake sounds.

#### ignore_world_damage

Whether or not to ignore world damage (e.g. by switchting to spectator). Makes sense to leave enabled.

### precache

#### soundevent_file

Precaches the sound event definition file in your (or as per default in my provided) quake sounds workshop add-on. You can use the default `soundevents_addon.vsndevts` but this gets overwritten by the CS2 server IF a workshop map is having the same name. Therefore you SHOULD rename the file (or simply duplicate it like I did) to make sure your sounds are loaded fine!

### count_self_kills

Whether or not to count self kills.

### count_team_kills

Whether or not to count team kills.

### reset_kills_on_death

Whether or not to reset kill streak on player death.

### reset_kills_on_round_start

Whether or not to reste kill streak on round start.

### commands

#### settings

The chat command players can type in to open the quakesounds menu (if enabled) or simply toggle the quake sounds for them.

#### settings_menu

Whether or not to show a settings menu instead of simply changing the mute status. Currently set to disabled because there is only one setting option. May change in future.

### messages

#### enable_center_message

Whether or not to show a center message.

#### center_message_type

Type of the center message. Can be one of the following:

- default
- alert
- html

#### enable_chat_message

Whether or not to enable chat messages.

### sound_priorities

Only one sound can be played for a player on a specific action. You can set a global priority for each sound type. Lower number means higher priority. Per default special events (like self-kills, teamkills, ..) are the highest priority. Followed by "weapons" in case you want to have a specific sound for a weapon kill and then finally the "normal" kill-streak sounds.

### sounds

List of all sounds. The Key is either the amount of kills, a weapon name or one of the defined special keys:

- bomb_planted
- bomb_<SECONDS_UNTIL_EXPLODING>
- bomb_defused
- bomb_exploded
- firstblood
- headshot
- knifekill
- selfkill
- teamkill
- round_start
- round_end
- round_freeze_end

All weapons can make a sound on kill, e.g. HE-Grenade (weapon_hegrenade). Just use *weapon_<name>* as a key.

All sounds **MUST** contain a list of at least two entries. One is the *_sound* file name or path. If it is a file name you will need a Workshop Addon where these file names are defined. If you use a path, e.g. *sounds/cs2/quakesounds/default/haha.vsnd* you don't need a Workshop Addon. However the player must have the given file in his game files somewhere.

Additionally you can set the volume of a given sound with the *_volume* key. Default is "1" (100%). You can adjust it to a higher volume, for example *1.5* for 150%. Or lower, e.g. *0.5* for 50%.

#### Example with Workshop Addon Soundevent name

All settings done in the Soundevent file for this specific sound name apply (e.g. Volume, sound distance, pitch, ...). Other players will hear it because the sound is positional. Players can change the volume via ingame settings.

```json
"headshot": {
  "de": "Kopfschuss",
  "en": "Headshot",
  "_sound": "QuakeSoundsD.Headshot",
  "_filter": "all" <-- optional to set players that can hear the sound. Can either be set globally (check config above or get overriden per sound)
}
```

#### Example with Workshop Addon sound path

No further settings possible. Will play at 100% sound volume of the sound file used. Sound is non-positional. Players cannot change the volume via ingame settings.

```json
"headshot": {
  "de": "Kopfschuss",
  "en": "Headshot",
  "_sound": "sounds/cs2/quakesounds/default/headshot.vsnd",
  "_filter": "all" <-- optional to set players that can hear the sound. Can either be set globally (check config above or get overriden per sound)
}
```

### data

I don't forcing server owners to use MySQL for simple stuff... therefore player choices simply get saved in the config file.

#### player_muted

List of all muted Steam IDs. Players can use *!qs* to mute or unmute themself.

#### player_languages

List of all players who used *!lang* to set their language. Currently it seems that plug-ins will not get the correct translation for a player. Therefore this plug-in intercepts the *!lang* command and saves the language for the player and loads it each time the player connects. This will ensure the proper translation for a given sound.

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
