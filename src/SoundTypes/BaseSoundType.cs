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
            if (!Config.Enabled
                || !Config.Sounds.TryGetValue(soundKey, out Dictionary<string, string>? soundConfig)
                || !soundConfig.TryGetValue("_sound", out string? soundName))
            {
                return false;
            }

            RecipientFilter filter = FilterService.PrepareFilter(attacker, victim, soundConfig.GetValueOrDefault("_filter"));

            // Only proceed if there are players to send to
            if (filter.Count == 0)
            {
                return true; // Sound was "played" but no recipients
            }

            // normally only attacker gets the message, but last man standing has to be the victim (because he is the last man standing)
            SoundService.PlaySound(soundKey == "lastmanstanding" && victim is { IsValid: true } ? victim : attacker, soundName, playOn ?? Config.Global.PlayOnEntity, filter);
            MessageService.PrintMessage(soundKey == "lastmanstanding" && victim is { IsValid: true } ? victim : attacker, soundConfig, filter);

            return true;
        }
    }
}
