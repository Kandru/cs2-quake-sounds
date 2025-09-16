using CounterStrikeSharp.API.Core;
using QuakeSounds.Services;
using QuakeSounds.SoundTypes;

namespace QuakeSounds.Managers
{
    public class SoundManager(PluginConfig config, SoundService soundService, MessageService messageService,
        FilterService filterService, Dictionary<CCSPlayerController, int> playerKillsInRound)
    {
        private readonly KillSounds _killSounds = new(config, soundService, messageService, filterService, playerKillsInRound);
        private readonly WeaponSounds _weaponSounds = new(config, soundService, messageService, filterService);
        private readonly RoundSounds _roundSounds = new(config, soundService, messageService, filterService);
        private readonly Dictionary<CCSPlayerController, int> _playerKillsInRound = playerKillsInRound;

        public void PlayKillSound(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData)
        {
            int killCount = _playerKillsInRound.GetValueOrDefault(attacker);

            if (_killSounds.TryPlayKillCountSound(attacker, victim, killCount))
            {
                return;
            }

            if (_killSounds.TryPlaySpecialKillSound(attacker, victim, eventData))
            {
                return;
            }

            _ = _weaponSounds.TryPlayWeaponSound(attacker, victim, eventData.Weapon);
        }

        public void PlayRoundSound(string soundKey)
        {
            _roundSounds.PlayRoundSound(soundKey);
        }
    }
}
