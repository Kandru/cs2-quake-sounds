using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public abstract class BaseSoundType
    {
        protected readonly PluginConfig Config;
        protected readonly SoundService SoundService;
        protected readonly MessageService MessageService;
        protected readonly FilterService FilterService;

        protected BaseSoundType(PluginConfig config, SoundService soundService, MessageService messageService, FilterService filterService)
        {
            Config = config;
            SoundService = soundService;
            MessageService = messageService;
            FilterService = filterService;
        }

        protected bool TryPlaySoundConfig(CCSPlayerController attacker, CCSPlayerController? victim, string soundKey, string? playOn = null)
        {
            if (!Config.Sounds.TryGetValue(soundKey, out Dictionary<string, string>? soundConfig) ||
                !soundConfig.TryGetValue("_sound", out string? soundName))
            {
                return false;
            }

            RecipientFilter filter = FilterService.PrepareFilter(attacker, victim, soundConfig.GetValueOrDefault("_filter"));
            SoundService.PlaySound(attacker, soundName, playOn ?? Config.PlayOn, filter);
            MessageService.PrintMessage(attacker, soundConfig, filter);
            return true;
        }
    }
}
