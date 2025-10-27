using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public class BombSounds(PluginConfig config, SoundService soundService, MessageService messageService, FilterService filterService) : BaseSoundType(config, soundService, messageService, filterService)
    {
        public bool TryToPlay(string soundKey)
        {
            if (!Config.Sounds.TryGetValue($"bomb_{soundKey}", out Dictionary<string, string>? soundConfig) ||
                !soundConfig.TryGetValue("_sound", out string? soundName))
            {
                return false;
            }

            RecipientFilter filter = FilterService.PrepareFilter(null, null, soundConfig.GetValueOrDefault("_filter"));

            foreach (CCSPlayerController? player in Utilities.GetPlayers().Where(static p => !p.IsBot))
            {
                SoundService.PlaySound(player, soundName, Config.Global.PlayOnEntity, filter);
            }

            return true;
        }

        public void PlayBombSound(string soundKey)
        {
            _ = TryToPlay(soundKey);
        }
    }
}
