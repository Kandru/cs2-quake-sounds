using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Extensions;
using CounterStrikeSharp.API.Core.Translations;
using System.Globalization;
using System.Text.Json;
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
        [JsonPropertyName("players_muted")] public List<ulong> PlayersMuted { get; set; } = [];
        // player languages
        [JsonPropertyName("players_languages")] public Dictionary<ulong, string> PlayerLanguages { get; set; } = [];
    }

    public partial class QuakeSounds : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }
        private readonly PlayerLanguageManager playerLanguageManager = new();

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            // sort Config.Sounds and sub dictionaries by key
            Config.Sounds = Config.Sounds
                .OrderBy(static x => int.TryParse(x.Key, out int key) ? key : int.MaxValue)
                .ToDictionary(static x => x.Key, static x => x.Value
                    .OrderBy(static y => int.TryParse(y.Key, out int key) ? key : int.MaxValue)
                    .ToDictionary(static y => y.Key, static y => y.Value));
            // update config and write new values from plugin to config file if changed after update
            Config.Update();
            Console.WriteLine(Localizer["core.config"]);
        }

        private bool ToggleMute(CCSPlayerController player)
        {
            if (Config.PlayersMuted.Contains(player.SteamID))
            {
                _ = Config.PlayersMuted.Remove(player.SteamID);
                Config.Update();
                player.PrintToChat(Localizer["sounds.unmuted"]);
                return false;
            }
            else
            {
                Config.PlayersMuted.Add(player.SteamID);
                Config.Update();
                player.PrintToChat(Localizer["sounds.muted"]);
                return true;
            }
        }

        private void LoadPlayerLanguage(ulong? steamID)
        {
            if (!steamID.HasValue)
            {
                return;
            }

            if (Config.PlayerLanguages.TryGetValue(steamID.Value, out string? language) &&
                !string.IsNullOrWhiteSpace(language))
            {
                playerLanguageManager.SetLanguage(new SteamID(steamID.Value), new CultureInfo(language));
            }
        }

        private static bool IsValidCulture(string language)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Any(c => c.Name.Equals(language, StringComparison.OrdinalIgnoreCase));
        }

        private void SavePlayerLanguage(ulong steamId, string language)
        {
            Dictionary<string, string> languagePreferences = LoadLanguagePreferences();
            languagePreferences[steamId.ToString()] = language;

            string json = JsonSerializer.Serialize(languagePreferences, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(ModuleDirectory, "lang_preferences.json"), json);
        }

        private Dictionary<string, string> LoadLanguagePreferences()
        {
            string filePath = Path.Combine(ModuleDirectory, "lang_preferences.json");
            if (!File.Exists(filePath))
            {
                return [];
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
            }
            catch
            {
                return [];
            }
        }

        private string GetLocalizedMessage(string key)
        {
            return Localizer[key].Value;
        }
    }
}
