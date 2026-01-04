using CounterStrikeSharp.API.Core;
using QuakeSounds.Services;
using QuakeSounds.SoundTypes;

namespace QuakeSounds.Managers
{
    public class SoundManager(PluginConfig config, SoundService soundService, MessageService messageService,
        FilterService filterService, Dictionary<CCSPlayerController, int> playerKillsInRound)
    {
        private readonly KillStreakSounds _killStreakSounds = new(config, soundService, messageService, filterService);
        private readonly SpecialEventSounds _specialEventSounds = new(config, soundService, messageService, filterService, playerKillsInRound);
        private readonly WeaponSounds _weaponSounds = new(config, soundService, messageService, filterService);
        private readonly GlobalSounds _globalSounds = new(config, soundService, messageService, filterService);
        private readonly Dictionary<CCSPlayerController, int> _playerKillsInRound = playerKillsInRound;

        public void PlayKillSound(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData)
        {
            int killCount = _playerKillsInRound.GetValueOrDefault(attacker);

            IOrderedEnumerable<(string Type, int Priority, Func<bool> TryPlay)> soundTypePriorities = new[]
            {
                (Type: "KillSounds", Priority: config.SoundPriorities.KillStreak, TryPlay: new Func<bool>(() =>
                    _killStreakSounds.TryToPlay(attacker, victim, eventData, killCount))),
                (Type: "SpecialEventSounds", Priority: config.SoundPriorities.SpecialEvents, TryPlay: new Func<bool>(() =>
                    _specialEventSounds.TryToPlay(attacker, victim, eventData))),
                (Type: "WeaponSounds", Priority: config.SoundPriorities.Weapons, TryPlay: new Func<bool>(() =>
                    _weaponSounds.TryToPlay(attacker, victim, eventData)))
            }.OrderBy(x => x.Priority);

            // Try to play sounds in priority order
            foreach ((string Type, int Priority, Func<bool> TryPlay) soundType in soundTypePriorities)
            {
                if (soundType.TryPlay())
                {
                    return;
                }
            }
        }

        public void PlayRoundSound(string soundKey)
        {
            _globalSounds.Play(soundKey, "round");
        }

        public void PlayBombSound(string soundKey)
        {
            _globalSounds.Play(soundKey, "bomb");
        }
    }
}
