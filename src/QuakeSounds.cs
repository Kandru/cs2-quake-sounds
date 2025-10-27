using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
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
        private int _bombExplosionTimer = 0;

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerChat>(OnPlayerChatCommand);
            RegisterEventHandler<EventBombPlanted>(OnBombPlanted);
            RegisterEventHandler<EventBombDefused>(OnBombDefused);
            RegisterEventHandler<EventBombExploded>(OnBombExploded);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            AddCommand(Config.Commands.SettingsCommand, "QuakeSounds user settings", CommandQuakeSoundSettings);
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
            RemoveListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
            DeregisterEventHandler<EventRoundStart>(OnRoundStart);
            DeregisterEventHandler<EventRoundEnd>(OnRoundEnd);
            DeregisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerChat>(OnPlayerChatCommand);
            DeregisterEventHandler<EventBombPlanted>(OnBombPlanted);
            DeregisterEventHandler<EventBombDefused>(OnBombDefused);
            DeregisterEventHandler<EventBombExploded>(OnBombExploded);
            RemoveListener<Listeners.OnMapStart>(OnMapStart);
            RemoveCommand(Config.Commands.SettingsCommand, CommandQuakeSoundSettings);
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
            if (!Config.Global.EnabledDuringWarmup && (bool)GetGameRule("WarmupPeriod")!)
            {
                DebugPrint("Ignoring during warmup.");
                return HookResult.Continue;
            }

            // check victim
            CCSPlayerController? victim = @event.Userid;
            if (victim == null
                || victim.IsValid != true
                || (victim.IsBot && Config.Global.IgnoreBots))
            {
                return HookResult.Continue;
            }

            // reset kill count if victim exists in dictionary
            if (_playerKillsInRound.ContainsKey(victim))
            {
                _playerKillsInRound[victim] = 0;
            }

            if (@event.Weapon.Equals("world", StringComparison.OrdinalIgnoreCase) && Config.Global.IgnoreWorldDamage)
            {
                DebugPrint("Ignoring world damage.");
                return HookResult.Continue;
            }

            // check attacker
            CCSPlayerController? attacker = @event.Attacker;
            if (attacker == null
                || attacker.IsValid != true
                || (attacker.IsBot && Config.Global.IgnoreBots))
            {
                return HookResult.Continue;
            }

            // process possible kill sound
            ProcessKill(attacker, victim, @event);
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

        private HookResult OnBombPlanted(EventBombPlanted @event, GameEventInfo info)
        {
            ConVar? mpC4Timer = ConVar.Find("mp_c4timer");
            if (mpC4Timer == null)
            {
                return HookResult.Continue;
            }
            _bombExplosionTimer = (int)Server.CurrentTime + mpC4Timer.GetPrimitiveValue<int>() + 1;
            AddTimer(1.0f, BombSoundHandler);
            _soundManager?.PlayBombSound("planted");
            return HookResult.Continue;
        }

        private HookResult OnBombDefused(EventBombDefused @event, GameEventInfo info)
        {
            _bombExplosionTimer = 0;
            _soundManager?.PlayBombSound("defused");
            return HookResult.Continue;
        }

        private HookResult OnBombExploded(EventBombExploded @event, GameEventInfo info)
        {
            _bombExplosionTimer = 0;
            _soundManager?.PlayBombSound("exploded");
            return HookResult.Continue;
        }

        private void BombSoundHandler()
        {
            int secondsLeft = _bombExplosionTimer - (int)Server.CurrentTime;
            if (_bombExplosionTimer == 0
                || secondsLeft <= 0)
            {
                return;
            }
            _soundManager?.PlayBombSound(secondsLeft.ToString());
            AddTimer(1.0f, BombSoundHandler);
        }

        private void OnMapStart(string mapName)
        {
            // reset player kills
            _playerKillsInRound.Clear();
        }

        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            _playerKillsInRound.Clear();
            _soundManager?.PlayRoundSound("start");
            return HookResult.Continue;
        }

        public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            _bombExplosionTimer = 0;
            _soundManager?.PlayRoundSound("end");
            return HookResult.Continue;
        }

        public HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            _soundManager?.PlayRoundSound("freeze_end");
            return HookResult.Continue;
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
    }
}