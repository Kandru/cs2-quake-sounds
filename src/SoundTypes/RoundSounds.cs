using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public class RoundSounds(PluginConfig config, SoundService soundService, MessageService messageService, FilterService filterService) : BaseSoundType(config, soundService, messageService, filterService)
    {
        public void PlayRoundSound(string soundKey)
        {
            if (!Config.Sounds.TryGetValue(soundKey, out Dictionary<string, string>? soundConfig) ||
                !soundConfig.TryGetValue("_sound", out string? soundName))
            {
                return;
            }

            RecipientFilter filter = FilterService.PrepareFilter(null, null, soundConfig.GetValueOrDefault("_filter"));

            foreach (CCSPlayerController? player in Utilities.GetPlayers().Where(static p => !p.IsBot))
            {
                SoundService.PlaySound(player, soundName, Config.PlayOn, filter);
            }
        }
    }
}
