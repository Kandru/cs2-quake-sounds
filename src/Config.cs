using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;
using QuakeSounds.Sounds;
using QuakeSounds.Utils;

namespace QuakeSounds
{
    public class CommandSettings
    {
        [JsonPropertyName("settings")] public string SettingsCommand { get; set; } = "qs";
        [JsonPropertyName("settings_menu")] public bool SettingsMenu { get; set; } = false;
    }

    public class GlobalSettings
    {
        [JsonPropertyName("enabled_during_warmup")] public bool EnabledDuringWarmup { get; set; } = true;
        [JsonPropertyName("play_on_entity")] public string PlayOnEntity { get; set; } = "player";
        [JsonPropertyName("sound_hearable_by")] public string SoundHearableBy { get; set; } = "all";
        [JsonPropertyName("ignore_bots")] public bool IgnoreBots { get; set; } = false;
        [JsonPropertyName("ignore_world_damage")] public bool IgnoreWorldDamage { get; set; } = true;
        [JsonPropertyName("count_self_kills")] public bool CountSelfKills { get; set; } = false;
        [JsonPropertyName("count_team_kills")] public bool CountTeamKills { get; set; } = false;
        [JsonPropertyName("reset_kills_on_death")] public bool ResetKillsOnDeath { get; set; } = true;
        [JsonPropertyName("reset_kills_on_round_start")] public bool ResetKillsOnRoundStart { get; set; } = true;
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

    public class PrecacheConfig
    {
        [JsonPropertyName("soundevent_file")] public string SoundEventFile { get; set; } = "soundevents/soundevents_quakesounds.vsndevts";
        [JsonPropertyName("additional_files")] public List<string> AdditionalFiles { get; set; } = [];
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
        [JsonPropertyName("precache")] public PrecacheConfig Precache { get; set; } = new();
        [JsonPropertyName("commands")] public CommandSettings Commands { get; set; } = new();
        [JsonPropertyName("messages")] public MessageSettings Messages { get; set; } = new();
        [JsonPropertyName("sound_priorities")] public SoundPriorities SoundPriorities { get; set; } = new();
        [JsonPropertyName("sounds")] public Dictionary<string, Dictionary<string, string>> Sounds { get; set; } = [];
        [JsonPropertyName("data")] public PlayerData Data { get; set; } = new();
    }

    public partial class QuakeSounds : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }

        private readonly Dictionary<string, SoundEntry> _entries = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<uint, SoundEntry> _soundHashes = [];
        private KillSoundKind[] _killPriority = [];

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            Config.Sounds = Config.Sounds
                .OrderBy(static x => int.TryParse(x.Key, out int key) ? key : int.MaxValue)
                .ToDictionary(static x => x.Key, static x => x.Value
                    .OrderBy(static y => int.TryParse(y.Key, out int key) ? key : int.MaxValue)
                    .ToDictionary(static y => y.Key, static y => y.Value));
            Config.Update();
            ApplyConfig();
            Console.WriteLine(Localizer["core.config"]);
        }

        /// <summary>
        /// Rebuild parsed sound cache and derived services. Called after initial parse and after reload.
        /// </summary>
        internal void ApplyConfig()
        {
            Debug.Configure(Config.Debug, msg => Localizer["core.debugprint"].Value.Replace("{message}", msg));

            _entries.Clear();
            _soundHashes.Clear();

            foreach ((string key, Dictionary<string, string> raw) in Config.Sounds)
            {
                SoundEntry? entry = SoundEntry.TryParse(key, raw);
                if (entry == null)
                {
                    continue;
                }

                _entries[key] = entry;
                foreach (string soundName in entry.SoundNames)
                {
                    uint hash = SoundEventUtils.GenerateSoundHash(soundName);
                    _ = _soundHashes.TryAdd(hash, entry);
                }
            }

            _killPriority =
            [
                .. new (KillSoundKind Kind, int Priority)[]
                {
                    (KillSoundKind.SpecialEvents, Config.SoundPriorities.SpecialEvents),
                    (KillSoundKind.Weapons, Config.SoundPriorities.Weapons),
                    (KillSoundKind.KillStreak, Config.SoundPriorities.KillStreak)
                }.OrderBy(static x => x.Priority).Select(static x => x.Kind)
            ];

            RebuildServices();
        }

        private void RebuildServices()
        {
            var emitter = new SoundEmitter(Config);
            var recipients = new Recipients(Config);
            var announcer = new Announcer(Config, _playerLanguageManager, Localizer);
            _soundPlayer = new SoundPlayer(Config, _entries, _killPriority, emitter, announcer, recipients, _playerKillCounter);
        }
    }
}
