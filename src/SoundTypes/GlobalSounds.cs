using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public class GlobalSounds(PluginConfig config, SoundService soundService, MessageService messageService, FilterService filterService) : BaseSoundType(config, soundService, messageService, filterService)
    {
        private readonly Random rnd = new();

        public void Play(string soundKey, string prefix)
        {
            string fullKey = $"{prefix}_{soundKey}";
            if (!Config.Sounds.TryGetValue(fullKey, out Dictionary<string, string>? soundConfig) ||
                !soundConfig.TryGetValue("_sound", out string? soundName))
            {
                return;
            }

            // check for multiple sounds separated by comma and choose a random one
            if (soundName.Contains(','))
            {
                string[] soundNames = soundName.Split(',');
                soundName = soundNames[rnd.Next(soundNames.Length)].Trim();
            }

            RecipientFilter filter = FilterService.PrepareFilter(null, null, soundConfig.GetValueOrDefault("_filter"));

            foreach (CCSPlayerController? player in Utilities.GetPlayers().Where(static p => !p.IsBot))
            {
                SoundService.PlaySound(player, soundName, Config.Global.PlayOnEntity, filter);
            }
        }
    }
}
