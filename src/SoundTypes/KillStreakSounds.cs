using CounterStrikeSharp.API.Core;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public class KillStreakSounds(PluginConfig config, SoundService soundService, MessageService messageService,
        FilterService filterService) : BaseSoundType(config, soundService, messageService, filterService)
    {
        public bool TryToPlay(CCSPlayerController attacker, CCSPlayerController? victim, int killCount)
        {
            return PlaySound(attacker, victim, killCount.ToString());
        }
    }
}
