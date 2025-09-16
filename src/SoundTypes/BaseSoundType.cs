using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public abstract class BaseSoundType(PluginConfig config, SoundService soundService, MessageService messageService, FilterService filterService)
    {
        protected readonly PluginConfig Config = config;
        protected readonly SoundService SoundService = soundService;
        protected readonly MessageService MessageService = messageService;
        protected readonly FilterService FilterService = filterService;

        private protected bool PlaySound(CCSPlayerController attacker, CCSPlayerController? victim, string soundKey, string? playOn = null)
        {
            if (!Config.Sounds.TryGetValue(soundKey, out Dictionary<string, string>? soundConfig) ||
                !soundConfig.TryGetValue("_sound", out string? soundName))
            {
                return false;
            }

            RecipientFilter filter = FilterService.PrepareFilter(attacker, victim, soundConfig.GetValueOrDefault("_filter"));

            // Only proceed if there are players to send to
            if (filter.Count == 0)
            {
                return true; // Sound was "played" but no recipients
            }

            SoundService.PlaySound(attacker, soundName, playOn ?? Config.PlayOn, filter);
            MessageService.PrintMessage(attacker, soundConfig, filter);
            return true;
        }
    }
}
