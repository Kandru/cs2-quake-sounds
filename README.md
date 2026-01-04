# CounterstrikeSharp - Quake Sounds

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-quake-sounds?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-quake-sounds/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-map-modifier](https://img.shields.io/github/issues/Kandru/cs2-quake-sounds)](https://github.com/Kandru/cs2-quake-sounds/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

Add classic Quake-style audio announcements to your CS2 server! This plugin brings back the nostalgic gaming experience with sound effects for kill streaks, special events, and weapon-specific eliminations. Features include customizable kill streak thresholds, multi-language support, player preferences, and flexible sound filtering. Easy to install with an provided workshop addon with a lot of quake sounds.

## Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-quake-sounds/releases/).
2. Move the "QuakeSounds" folder to the `/addons/counterstrikesharp/plugins/` directory.
3. Download and install [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager)
4. Add at least one Workshop Addon with Quake Sounds to the configuration of the MultiAddonManager. I provide the following for use: https://steamcommunity.com/sharedfiles/filedetails/?id=3461824328
5. Restart the server.

Updating is even easier: simply overwrite all plugin files and they will be reloaded automatically. To automate updates please use our [CS2 Update Manager](https://github.com/Kandru/cs2-update-manager/).


## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/QuakeSounds/QuakeSounds.json`.

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

### enabled

Whether this plug-in is enabled or not.

### debug

Debug mode. Only necessary during development or testing.

### global

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

### quakesounds (Server Console Only)

Ability to run sub-commands:

```
quakesounds reload
quakesounds disable
quakesounds enable
```

#### reload

Reloads the configuration.

#### disable

Disables the sounds instantly and remembers this state.

#### enable

Enables the sounds instantly and remembers this state.

## Compile Yourself

Clone the project:

```bash
git clone https://github.com/Kandru/cs2-quake-sounds.git
```

Go to the project directory

```bash
  cd cs2-quake-sounds
```

Install dependencies

```bash
  dotnet restore
```

Build debug files (to use on a development game server)

```bash
  dotnet build
```

Build release files (to use on a production game server)

```bash
  dotnet publish
```

## FAQ

### Which sound should be played when? Can you provide examples?

Please refer to the example configuration found above or search the internet. However, here's a list you can refer to:

```
3 Frags → TRIPLE KILL
5 Frags → MULTI KILL
6 Frags → RAMPAGE
7 Frags → KILLING SPREE
8 Frags → DOMINATING
9 Frags → IMPRESSIVE
10 Frags → UNSTOPPABLE
11 Frags → OUTSTANDING
12 Frags → MEGA KILL
13 Frags → ULTRA KILL
14 Frags → EAGLE EYE
15 Frags → OWNAGE
16 Frags → COMBO KING
17 Frags → MANIAC
18 Frags → LUDICROUS KILL
19 Frags → BULLSEYE
20 Frags → EXCELLENT
21 Frags → PANCAKE
22 Frags → HEAD HUNTER
23 Frags → UNREAL
24 Frags → ASSASSIN
25 Frags → WICKED SICK
26 Frags → MASSACRE
27 Frags → KILLING MACHINE
28 Frags → MONSTER KILL
29 Frags → HOLY SHIT
30 Frags → G O D L I K E
```

### Where can I find sounds?

Sounds are spread over the internet. We provide a Workshop Addon which you can use: https://steamcommunity.com/sharedfiles/filedetails/?id=3461824328 which contains the following sounds:

#### Male Sounds

```
QuakeSoundsD.Assassin
QuakeSoundsD.Bullseye
QuakeSoundsD.Comboking
QuakeSoundsD.Combowhore
QuakeSoundsD.Dominating
QuakeSoundsD.Dominating2
QuakeSoundsD.Dominating3
QuakeSoundsD.Doublekill
QuakeSoundsD.Doublekill2
QuakeSoundsD.Doublekill3
QuakeSoundsD.Eagleeye
QuakeSoundsD.Eagleeye2
QuakeSoundsD.Excellent
QuakeSoundsD.Firstblood
QuakeSoundsD.Firstblood2
QuakeSoundsD.Firstblood3
QuakeSoundsD.Firstblood4
QuakeSoundsD.Flawless
QuakeSoundsD.Godlike
QuakeSoundsD.Godlike2
QuakeSoundsD.Haha
QuakeSoundsD.Hattrick
QuakeSoundsD.Hattrick2
QuakeSoundsD.Headhunter
QuakeSoundsD.Headshot
QuakeSoundsD.Headshot2
QuakeSoundsD.Headshot3
QuakeSoundsD.Headshot4
QuakeSoundsD.Holyshit
QuakeSoundsD.Humiliating
QuakeSoundsD.Humiliatingdefeat
QuakeSoundsD.Humiliation
QuakeSoundsD.Impressive
QuakeSoundsD.Impressive2
QuakeSoundsD.Killingmachine
QuakeSoundsD.Killingspree
QuakeSoundsD.Killingspree2
QuakeSoundsD.Ludicrouskill
QuakeSoundsD.Ludicrouskill2
QuakeSoundsD.Maniac
QuakeSoundsD.Massacre
QuakeSoundsD.Megakill
QuakeSoundsD.Megakill2
QuakeSoundsD.Monsterkill
QuakeSoundsD.Monsterkill2
QuakeSoundsD.Multikill
QuakeSoundsD.Multikill2
QuakeSoundsD.Outstanding
QuakeSoundsD.Ownage
QuakeSoundsD.Pancake
QuakeSoundsD.Payback
QuakeSoundsD.Perfect
QuakeSoundsD.Play
QuakeSoundsD.Prepare
QuakeSoundsD.Prepare2
QuakeSoundsD.Rampage
QuakeSoundsD.Retribution
QuakeSoundsD.Teamkiller
QuakeSoundsD.Triplekill
QuakeSoundsD.Ultrakill
QuakeSoundsD.Ultrakill2
QuakeSoundsD.Unstoppable
QuakeSoundsD.Unstoppable2
QuakeSoundsD.Vengeance
QuakeSoundsD.Wickedsick
```

#### Female Sounds

```
QuakeSoundsF.Bottomfeeder
QuakeSoundsF.Dominating
QuakeSoundsF.Firstblood
QuakeSoundsF.Godlike
QuakeSoundsF.Headshot
QuakeSoundsF.Hexakill
QuakeSoundsF.Holyshit
QuakeSoundsF.Humiliation
QuakeSoundsF.Killingspree
QuakeSoundsF.Monsterkill
QuakeSoundsF.Multikill
QuakeSoundsF.Pentakill
QuakeSoundsF.Prepare
QuakeSoundsF.Quadrakill
QuakeSoundsF.Rampage
QuakeSoundsF.Shutdown
QuakeSoundsF.Ultrakill
QuakeSoundsF.Unstoppable
QuakeSoundsF.Wickedsick
QuakeSoundsF.Lastmanstanding
```

#### Bomb Sounds
```
BombSounds.Sec1
BombSounds.Sec2
BombSounds.Sec3
BombSounds.Sec4
BombSounds.Sec5
BombSounds.Sec6
BombSounds.Sec7
BombSounds.Sec8
BombSounds.Sec9
BombSounds.Sec10
BombSounds.Sec20
BombSounds.Sec30
```

### I cannot hear any sounds and I used your provided example configuration and MultiAddonManager

Make sure the **MultiAddonManager** does not have any errors in the server console. Make also sure your Client is actually asked during connecting to your server to download the Quake Sounds. If the client is not asked you do **NOT** have the files on your system and will **NOT** hear anything.

When having everything installed correctly you **MUST** see the following inside your server console upon connection:

```
[MultiAddonManager] Addon 3461824328 downloaded successfully
[MultiAddonManager] Client kalle (XXXX) connected:
[MultiAddonManager] first connection, sending addon 3461824328
[MultiAddonManager] Client kalle (XXXX) connected:
[MultiAddonManager] reconnected within the interval and has all addons, allowing
```

Also make sure your Metamod version is up to date. Otherwise MultiAddonManager will not work:

```
[META] Failed to load plugin addons/multiaddonmanager/bin/multiaddonmanager: Plugin requires newer Metamod version (17 > 16)
```

## License

Released under [GPLv3](/LICENSE) by [@Kandru](https://github.com/Kandru).

## Authors

- [@derkalle4](https://www.github.com/derkalle4)
- [@jmgraeffe](https://www.github.com/jmgraeffe)
