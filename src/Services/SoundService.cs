using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace QuakeSounds.Services
{
    public class SoundService(PluginConfig config, Action<string> debugPrint)
    {
        private readonly PluginConfig _config = config;
        private readonly Action<string> _debugPrint = debugPrint;
        private CWorld? _worldEnt;

        public void PlaySound(CCSPlayerController player, string sound, string? playOn, RecipientFilter filter)
        {
            // Early return if no recipients
            if (filter.Count == 0)
            {
                _debugPrint($"No recipients for quake sound {sound} for player {player.PlayerName}.");
                return;
            }

            _debugPrint($"Playing quake sound {sound} for player {player.PlayerName} to {filter.Count} recipients.");

            if (sound.StartsWith("sounds/"))
            {
                PlaySoundViaClientCommand(sound, filter);
            }
            else
            {
                PlaySoundViaEmit(player, sound, playOn ?? _config.PlayOn, filter);
            }
        }

        private void PlaySoundViaClientCommand(string sound, RecipientFilter filter)
        {
            _debugPrint($"Playing quake sound via client command for {filter.Count} listening players.");
            foreach (CCSPlayerController player in filter)
            {
                player.ExecuteClientCommand($"play {sound}");
            }
        }

        private void PlaySoundViaEmit(CCSPlayerController player, string sound, string playOn, RecipientFilter filter)
        {
            switch (playOn.ToLower(System.Globalization.CultureInfo.CurrentCulture))
            {
                case "player":
                    _debugPrint("Playing quake sound on player.");
                    _ = player.EmitSound(sound, filter);
                    break;
                case "world":
                    PlaySoundOnWorld(sound, filter);
                    break;
                default:
                    _debugPrint($"Could not determine where to play sound (unknown config option play_on={_config.PlayOn}). Skipping.");
                    break;
            }
        }

        private void PlaySoundOnWorld(string sound, RecipientFilter filter)
        {
            // check if we already have a valid world entity cached
            if (_worldEnt?.IsValid != true)
            {
                // try to find and cache the world entity
                _debugPrint("Looking for world entity...");
                _worldEnt = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();
                if (_worldEnt?.IsValid != true)
                {
                    _debugPrint("Could not find world entity.");
                    return;
                }
            }

            _debugPrint("Playing quake sound on world entity.");
            _ = _worldEnt.EmitSound(sound, filter);
        }
    }
}
