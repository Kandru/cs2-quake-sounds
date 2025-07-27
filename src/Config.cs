using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;

namespace QuakeSounds
{
    public class PluginConfig : BasePluginConfig
    {
        // Enabled or disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        // sound settings command
        [JsonPropertyName("settings_command")] public string SettingsCommand { get; set; } = "qs";
        // toggle settings menu
        [JsonPropertyName("settings_menu")] public bool SettingsMenu { get; set; } = true;
        // enable during warmup
        [JsonPropertyName("enabled_during_warmup")] public bool EnabledDuringWarmup { get; set; } = true;
        // where to play sounds on (player, world)
        [JsonPropertyName("play_on")] public string PlayOn { get; set; } = "player";
        // set who can hear the sounds (all, attacker_team, victim_team, involved, attacker, victim, spectator)
        [JsonPropertyName("filter_sounds")] public string FilterSounds { get; set; } = "all";
        // ignore bots
        [JsonPropertyName("ignore_bots")] public bool IgnoreBots { get; set; } = true;
        // ignore world damage
        [JsonPropertyName("ignore_world_damage")] public bool IgnoreWorldDamage { get; set; } = true;
        // enable center message
        [JsonPropertyName("enable_center_message")] public bool CenterMessage { get; set; } = true;
        // center message typ (default, alert or html)
        [JsonPropertyName("center_message_type")] public string CenterMessageType { get; set; } = "default";
        // enable chat message
        [JsonPropertyName("enable_chat_message")] public bool ChatMessage { get; set; } = true;
        // count self kills
        [JsonPropertyName("count_self_kills")] public bool CountSelfKills { get; set; } = false;
        // count team kills
        [JsonPropertyName("count_team_kills")] public bool CountTeamKills { get; set; } = false;
        // reset kills on death
        [JsonPropertyName("reset_kills_on_death")] public bool ResetKillsOnDeath { get; set; } = true;
        // reset kills on round start
        [JsonPropertyName("reset_kills_on_round_start")] public bool ResetKillsOnRoundStart { get; set; } = true;
        // sounds dict (language, string to match, sound path)
        [JsonPropertyName("sounds")] public Dictionary<string, Dictionary<string, string>> Sounds { get; set; } = [];
        // muted players
        [JsonPropertyName("player_muted")] public List<string> PlayersMuted { get; set; } = [];
        // player languages
        [JsonPropertyName("player_languages")] public Dictionary<string, string> PlayerLanguages { get; set; } = [];
    }

    public partial class QuakeSounds : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            // sort Config.Sounds and sub dictionaries by key
            Config.Sounds = Config.Sounds
                .OrderBy(x => int.TryParse(x.Key, out var key) ? key : int.MaxValue)
                .ToDictionary(x => x.Key, x => x.Value
                    .OrderBy(y => int.TryParse(y.Key, out var key) ? key : int.MaxValue)
                    .ToDictionary(y => y.Key, y => y.Value));
            // update config and write new values from plugin to config file if changed after update
            Config.Update();
            Console.WriteLine(Localizer["core.config"]);
        }

        private bool ToggleMute(CCSPlayerController player)
        {
            if (Config.PlayersMuted.Contains(player.NetworkIDString))
            {
                Config.PlayersMuted.Remove(player.NetworkIDString);
                Config.Update();
                player.PrintToChat(Localizer["sounds.unmuted"]);
                return false;
            }
            else
            {
                Config.PlayersMuted.Add(player.NetworkIDString);
                Config.Update();
                player.PrintToChat(Localizer["sounds.muted"]);
                return true;
            }
        }

        private void LoadPlayerLanguage(string? steamID)
        {
            if (string.IsNullOrWhiteSpace(steamID)) return;
            // check if the player has a language set
            if (Config.PlayerLanguages.TryGetValue(steamID, out var language) && !string.IsNullOrWhiteSpace(language))
            {
                // set the language for the player
                playerLanguageManager.SetLanguage(
                    new SteamID(steamID),
                    new System.Globalization.CultureInfo(language));
            }
        }

        private void SavePlayerLanguage(string? steamID, string language)
        {
            if (string.IsNullOrWhiteSpace(steamID) || string.IsNullOrWhiteSpace(language)) return;
            // Try to add or update the player's language
            if (!Config.PlayerLanguages.TryAdd(steamID, language))
            {
                Config.PlayerLanguages[steamID] = language;
            }
            // write config
            Config.Update();
            // set the language for the player
            playerLanguageManager.SetLanguage(new SteamID(steamID), new System.Globalization.CultureInfo(language));
            DebugPrint($"Saved language for player {steamID} to {language}.");
        }
    }
}
