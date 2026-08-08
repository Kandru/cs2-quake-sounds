using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.UserMessages;
using QuakeSounds.Sounds;
using QuakeSounds.Utils;
using System.Globalization;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        public override string ModuleName => "CS2 QuakeSounds";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        private SoundPlayer? _soundPlayer;
        private int _bombExplosionTimer;

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            RegisterEventHandler<EventPlayerChat>(OnPlayerChatCommand);
            RegisterEventHandler<EventBombPlanted>(OnBombPlanted);
            RegisterEventHandler<EventBombDefused>(OnBombDefused);
            RegisterEventHandler<EventBombExploded>(OnBombExploded);
            HookUserMessage(208, HookUserMessage208);
            AddCommand(Config.Commands.SettingsCommand, "QuakeSounds user settings", CommandQuakeSoundSettings);

            if (hotReload)
            {
                foreach (CCSPlayerController entry in Utilities.GetPlayers().Where(
                    static p => p.IsValid && !p.IsBot))
                {
                    LoadPlayerLanguage(entry.SteamID);
                }
            }
        }

        public override void Unload(bool hotReload)
        {
            RemoveListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
            RemoveListener<Listeners.OnMapStart>(OnMapStart);
            DeregisterEventHandler<EventRoundStart>(OnRoundStart);
            DeregisterEventHandler<EventRoundEnd>(OnRoundEnd);
            DeregisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            DeregisterEventHandler<EventPlayerChat>(OnPlayerChatCommand);
            DeregisterEventHandler<EventBombPlanted>(OnBombPlanted);
            DeregisterEventHandler<EventBombDefused>(OnBombDefused);
            DeregisterEventHandler<EventBombExploded>(OnBombExploded);
            UnhookUserMessage(208, HookUserMessage208);
            RemoveCommand(Config.Commands.SettingsCommand, CommandQuakeSoundSettings);
            _soundPlayer = null;
        }

        public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            if (!Config.Global.EnabledDuringWarmup && GameRules.IsWarmup)
            {
                Debug.Print("Ignoring during warmup.");
                return HookResult.Continue;
            }

            CCSPlayerController? victim = @event.Userid;
            if (victim == null || !victim.IsValid || (victim.IsBot && Config.Global.IgnoreBots))
            {
                return HookResult.Continue;
            }

            if (_playerKillCounter.ContainsKey(victim.Slot))
            {
                ResetKill(victim);
            }

            CCSPlayerController? attacker = @event.Attacker;
            if (attacker == null || !attacker.IsValid || (attacker.IsBot && Config.Global.IgnoreBots))
            {
                return HookResult.Continue;
            }

            if (ShouldCountKill(attacker, victim))
            {
                UpdateKillCount(attacker);
            }

            _soundPlayer?.PlayKill(attacker, victim, @event);
            return HookResult.Continue;
        }

        private HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null || !player.IsValid || player.IsBot)
            {
                return HookResult.Continue;
            }

            LoadPlayerLanguage(player.SteamID);
            return HookResult.Continue;
        }

        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null || !player.IsValid)
            {
                return HookResult.Continue;
            }

            RemoveKill(player);
            return HookResult.Continue;
        }

        private HookResult OnPlayerChatCommand(EventPlayerChat @event, GameEventInfo info)
        {
            CCSPlayerController? player = Utilities.GetPlayerFromUserid(@event.Userid);
            if (player == null || !player.IsValid || player.IsBot)
            {
                return HookResult.Continue;
            }

            if (@event.Text.StartsWith("!lang", StringComparison.OrdinalIgnoreCase))
            {
                string? language = @event.Text.Split(' ').Skip(1).FirstOrDefault()?.Trim();
                if (language == null
                    || !CultureInfo.GetCultures(CultureTypes.AllCultures)
                        .Any(c => c.Name.Equals(language, StringComparison.OrdinalIgnoreCase)))
                {
                    return HookResult.Continue;
                }

                SavePlayerLanguage(player.SteamID, language);
            }

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
            _ = AddTimer(1.0f, BombSoundHandler);
            _soundPlayer?.PlayBomb("planted");
            return HookResult.Continue;
        }

        private HookResult OnBombDefused(EventBombDefused @event, GameEventInfo info)
        {
            _bombExplosionTimer = 0;
            _soundPlayer?.PlayBomb("defused");
            return HookResult.Continue;
        }

        private HookResult OnBombExploded(EventBombExploded @event, GameEventInfo info)
        {
            _bombExplosionTimer = 0;
            _soundPlayer?.PlayBomb("exploded");
            return HookResult.Continue;
        }

        private void BombSoundHandler()
        {
            int secondsLeft = _bombExplosionTimer - (int)Server.CurrentTime;
            if (_bombExplosionTimer == 0 || secondsLeft <= 0)
            {
                return;
            }

            _soundPlayer?.PlayBomb(secondsLeft.ToString());
            _ = AddTimer(1.0f, BombSoundHandler);
        }

        private void OnMapStart(string mapName)
        {
            GameRules.Invalidate();
            ClearKills();
        }

        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            if (Config.Global.ResetKillsOnRoundStart)
            {
                ClearKills();
            }

            _bombExplosionTimer = 0;
            _soundPlayer?.PlayRound("start");
            return HookResult.Continue;
        }

        public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            _bombExplosionTimer = 0;
            _soundPlayer?.PlayRound("end");
            return HookResult.Continue;
        }

        public HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            _soundPlayer?.PlayRound("freeze_end");
            return HookResult.Continue;
        }

        public HookResult HookUserMessage208(UserMessage um)
        {
            uint soundevent = um.ReadUInt("soundevent_hash");
            if (!_soundHashes.TryGetValue(soundevent, out SoundEntry? entry) || entry.Volume is not float volume)
            {
                return HookResult.Continue;
            }

            uint soundeventGuid = um.ReadUInt("soundevent_guid");
            Debug.Print($"Received sound event: {entry.Key}, hash: {soundevent}.");
            new SoundParams
            {
                Guid = soundeventGuid,
                Recipients = um.Recipients
            }
            .Volume(volume)
            .Send();
            Debug.Print($"Sending sound event: {entry.Key} with volume {volume}");
            return HookResult.Continue;
        }
    }
}
