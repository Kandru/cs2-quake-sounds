using CounterStrikeSharp.API.Core;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public class KillStreakSounds(PluginConfig config, SoundService soundService, MessageService messageService,
        FilterService filterService) : BaseSoundType(config, soundService, messageService, filterService)
    {
        public bool TryToPlay(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData, int killCount)
        {
            // ignore world damage
            return (!eventData.Weapon.Equals("world", StringComparison.OrdinalIgnoreCase) || !Config.Global.IgnoreWorldDamage) && PlaySound(attacker, victim, killCount.ToString());
        }
    }
}
