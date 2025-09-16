using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Extensions;
using CounterStrikeSharp.API.Core.Translations;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuakeSounds
{
    public class CommandSettings
    {
        [JsonPropertyName("settings")] public string SettingsCommand { get; set; } = "qs";
        [JsonPropertyName("settings_menu")] public bool SettingsMenu { get; set; } = true;
    }

    public class GlobalSettings
    {
        [JsonPropertyName("enabled during warmup")] public bool EnabledDuringWarmup { get; set; } = true;
        [JsonPropertyName("play_on_entity")] public string PlayOnEntity { get; set; } = "player";
        [JsonPropertyName("sound_hearable_by")] public string SoundHearableBy { get; set; } = "all";
        [JsonPropertyName("ignore_bots")] public bool IgnoreBots { get; set; } = true;
        [JsonPropertyName("ignore_world_damage")] public bool IgnoreWorldDamage { get; set; } = true;
    }

    public class MessageSettings
    {
        [JsonPropertyName("enable_center_message")] public bool CenterMessage { get; set; } = true;
        [JsonPropertyName("center_message_type")] public string CenterMessageType { get; set; } = "default";
        [JsonPropertyName("enable_chat_message")] public bool ChatMessage { get; set; } = true;
    }

    public class PlayerData
    {
        [JsonPropertyName("player_muted")] public List<ulong> PlayerMuted { get; set; } = [];
        [JsonPropertyName("player_languages")] public Dictionary<ulong, string> PlayerLanguages { get; set; } = [];
    }

    public class SoundPriorities
    {
        [JsonPropertyName("special_events")] public int SpecialEvents { get; set; } = 1;
        [JsonPropertyName("weapons")] public int Weapons { get; set; } = 2;
        [JsonPropertyName("kill_streak")] public int KillStreak { get; set; } = 3;
    }

    public class PluginConfig : BasePluginConfig
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        [JsonPropertyName("global")] public GlobalSettings Global { get; set; } = new();
        [JsonPropertyName("count_self_kills")] public bool CountSelfKills { get; set; } = false;
        [JsonPropertyName("count_team_kills")] public bool CountTeamKills { get; set; } = false;
        [JsonPropertyName("reset_kills_on_death")] public bool ResetKillsOnDeath { get; set; } = true;
        [JsonPropertyName("reset_kills_on_round_start")] public bool ResetKillsOnRoundStart { get; set; } = true;
        [JsonPropertyName("commands")] public CommandSettings Commands { get; set; } = new();
        [JsonPropertyName("messages")] public MessageSettings Messages { get; set; } = new();
        [JsonPropertyName("sound_priorities")] public SoundPriorities SoundPriorities { get; set; } = new();
        [JsonPropertyName("sounds")] public Dictionary<string, Dictionary<string, string>> Sounds { get; set; } = [];
        [JsonPropertyName("data")] public PlayerData Data { get; set; } = new();
    }

    public partial class QuakeSounds : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }
        private readonly PlayerLanguageManager playerLanguageManager = new();

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            // sort Config.Sound.Sounds and sub dictionaries by key
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
            if (Config.Data.PlayerMuted.Contains(player.SteamID))
            {
                _ = Config.Data.PlayerMuted.Remove(player.SteamID);
                Config.Update();
                player.PrintToChat(Localizer["sounds.unmuted"]);
                return false;
            }
            else
            {
                Config.Data.PlayerMuted.Add(player.SteamID);
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

            if (Config.Data.PlayerLanguages.TryGetValue(steamID.Value, out string? language) &&
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
