using CounterStrikeSharp.API.Core;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public class KillSounds(PluginConfig config, SoundService soundService, MessageService messageService,
        FilterService filterService, Dictionary<CCSPlayerController, int> playerKillsInRound) : BaseSoundType(config, soundService, messageService, filterService)
    {
        private readonly Dictionary<CCSPlayerController, int> _playerKillsInRound = playerKillsInRound;

        public bool TryPlayKillCountSound(CCSPlayerController attacker, CCSPlayerController? victim, int killCount)
        {
            return TryPlaySoundConfig(attacker, victim, killCount.ToString());
        }

        public bool TryPlaySpecialKillSound(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData)
        {
            (string, bool, string?)[] specialSounds =
            [
                ("selfkill", attacker == victim, "world"),
                ("teamkill", victim?.IsValid == true && attacker.Team == victim.Team, null),
                ("firstblood", IsFirstBlood(attacker), null),
                ("knifekill", eventData.Weapon.Contains("knife", StringComparison.OrdinalIgnoreCase), null),
                ("headshot", eventData.Headshot, null)
            ];

            foreach ((string? soundKey, bool condition, string? playOn) in specialSounds)
            {
                if (condition && TryPlaySoundConfig(attacker, victim, soundKey, playOn))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsFirstBlood(CCSPlayerController attacker)
        {
            return _playerKillsInRound.TryGetValue(attacker, out int kills) &&
                   _playerKillsInRound.Count == 1 && kills == 1;
        }
    }
}
