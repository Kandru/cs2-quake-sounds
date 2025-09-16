using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using QuakeSounds.Services;
using QuakeSounds.Managers;
using System.Globalization;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        public override string ModuleName => "CS2 QuakeSounds";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        private readonly Dictionary<CCSPlayerController, int> _playerKillsInRound = [];

        private SoundService? _soundService;
        private MessageService? _messageService;
        private FilterService? _filterService;
        private SoundManager? _soundManager;

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerChat>(OnPlayerChatCommand);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            AddCommand(Config.SettingsCommand, "QuakeSounds user settings", CommandQuakeSoundSettings);
            InitializeServices();
            if (hotReload)
            {
                foreach (CCSPlayerController entry in Utilities.GetPlayers().Where(
                    static p => p.IsValid
                        && !p.IsBot))
                {
                    LoadPlayerLanguage(entry.SteamID);
                }
            }
        }

        public override void Unload(bool hotReload)
        {
            DeregisterEventHandler<EventRoundStart>(OnRoundStart);
            DeregisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerChat>(OnPlayerChatCommand);
            RemoveListener<Listeners.OnMapStart>(OnMapStart);
            RemoveCommand(Config.SettingsCommand, CommandQuakeSoundSettings);
            DestroyServices();
        }

        private void InitializeServices()
        {
            _soundService = new SoundService(Config, DebugPrint);
            _messageService = new MessageService(Config, new(), GetLocalizedMessage);
            _filterService = new FilterService(Config, DebugPrint);
            _soundManager = new SoundManager(Config, _soundService, _messageService, _filterService, _playerKillsInRound);
        }

        private void DestroyServices()
        {
            _soundService = null;
            _messageService = null;
            _filterService = null;
            _soundManager = null;
        }

        public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            if (!ShouldProcessDeath(@event))
            {
                return HookResult.Continue;
            }

            CCSPlayerController? attacker = @event.Attacker;
            CCSPlayerController? victim = @event.Userid;

            if (!IsValidAttacker(attacker))
            {
                return HookResult.Continue;
            }

            ProcessKill(attacker!, victim, @event);
            return HookResult.Continue;
        }

        private HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot)
            {
                return HookResult.Continue;
            }

            LoadPlayerLanguage(player.SteamID);
            return HookResult.Continue;
        }

        private HookResult OnPlayerChatCommand(EventPlayerChat @event, GameEventInfo info)
        {
            CCSPlayerController? player = Utilities.GetPlayerFromUserid(@event.Userid);
            if (player == null
                || !player.IsValid
                || player.IsBot)
            {
                return HookResult.Continue;
            }

            if (@event.Text.StartsWith("!lang", StringComparison.OrdinalIgnoreCase))
            {
                // get language from command instead of player because it defaults to english all the time Oo
                string? language = @event.Text.Split(' ').Skip(1).FirstOrDefault()?.Trim();
                if (language == null
                    || !CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.Name.Equals(language, StringComparison.OrdinalIgnoreCase)))
                {
                    return HookResult.Continue;
                }
                // set language for player
                SavePlayerLanguage(player.SteamID, language);
                return HookResult.Continue;
            }
            // redraw GUI
            return HookResult.Continue;
        }

        private void OnMapStart(string mapName)
        {
            // reset player kills
            _playerKillsInRound.Clear();
        }

        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            _playerKillsInRound.Clear();
            _soundManager?.PlayRoundSound("round_start");
            return HookResult.Continue;
        }

        public HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            _soundManager?.PlayRoundSound("freeze_end");
            return HookResult.Continue;
        }

        private bool ShouldProcessDeath(EventPlayerDeath @event)
        {
            if (!Config.EnabledDuringWarmup && (bool)GetGameRule("WarmupPeriod")!)
            {
                DebugPrint("Ignoring during warmup.");
                return false;
            }

            if (@event.Weapon.Equals("world", StringComparison.OrdinalIgnoreCase) && Config.IgnoreWorldDamage)
            {
                DebugPrint("Ignoring world damage.");
                return false;
            }

            return true;
        }

        private bool IsValidAttacker(CCSPlayerController? attacker)
        {
            return attacker?.IsValid == true && (!attacker.IsBot || !Config.IgnoreBots);
        }

        private void ProcessKill(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath @event)
        {
            if (ShouldCountKill(attacker, victim))
            {
                UpdateKillCount(attacker);
            }

            _soundManager?.PlayKillSound(attacker, victim, @event);
        }

        private bool ShouldCountKill(CCSPlayerController attacker, CCSPlayerController? victim)
        {
            return attacker == victim
                ? Config.CountSelfKills
                : victim?.IsValid == true && attacker.Team == victim.Team ? Config.CountTeamKills : attacker != victim;
        }

        private void UpdateKillCount(CCSPlayerController attacker)
        {
            _ = _playerKillsInRound.TryGetValue(attacker, out int kills);
            _playerKillsInRound[attacker] = kills + 1;
            DebugPrint($"Player {attacker.PlayerName} has {_playerKillsInRound[attacker]} kills.");
        }

        private void ProcessLanguageCommand(CCSPlayerController player, string text)
        {
            string? language = text.Split(' ').Skip(1).FirstOrDefault()?.Trim();
            if (string.IsNullOrEmpty(language) || !IsValidCulture(language))
            {
                return;
            }

            SavePlayerLanguage(player.SteamID, language);
        }
    }
}