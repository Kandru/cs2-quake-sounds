using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Utils;

namespace QuakeSounds.Sounds
{
    public class SoundEmitter(PluginConfig config)
    {
        private readonly PluginConfig _config = config;
        private CWorld? _worldEnt;

        public void Play(CCSPlayerController source, string sound, RecipientFilter filter, string? playOn = null)
        {
            if (filter.Count == 0)
            {
                Debug.Print($"No recipients for quake sound {sound} for player {source.PlayerName}.");
                return;
            }

            Debug.Print($"Playing quake sound {sound} for player {source.PlayerName} to {filter.Count} recipients.");

            if (sound.StartsWith("sounds/", StringComparison.OrdinalIgnoreCase))
            {
                PlayViaClientCommand(sound, filter);
            }
            else
            {
                PlayViaEmit(source, sound, playOn ?? _config.Global.PlayOnEntity, filter);
            }
        }

        /// <summary>
        /// Play a global (round/bomb) sound once — either on world, or once per recipient on themselves.
        /// </summary>
        public void PlayGlobal(string sound, RecipientFilter filter)
        {
            if (filter.Count == 0)
            {
                Debug.Print($"No recipients for global quake sound {sound}.");
                return;
            }

            if (sound.StartsWith("sounds/", StringComparison.OrdinalIgnoreCase))
            {
                PlayViaClientCommand(sound, filter);
                return;
            }

            string playOn = _config.Global.PlayOnEntity.ToLowerInvariant();
            if (playOn == "world")
            {
                PlayOnWorld(sound, filter);
                return;
            }

            if (playOn != "player")
            {
                Debug.Print($"Could not determine where to play sound (unknown config option play_on={_config.Global.PlayOnEntity}). Skipping.");
                return;
            }

            // Emit once per recipient from their own entity so everyone hears it once.
            foreach (CCSPlayerController player in filter)
            {
                RecipientFilter single = [player];
                Debug.Print($"Playing global quake sound on player {player.PlayerName}.");
                _ = player.EmitSound(sound, single);
            }
        }

        private void PlayViaClientCommand(string sound, RecipientFilter filter)
        {
            Debug.Print($"Playing quake sound via client command for {filter.Count} listening players.");
            foreach (CCSPlayerController player in filter)
            {
                player.ExecuteClientCommand($"play {sound}");
            }
        }

        private void PlayViaEmit(CCSPlayerController player, string sound, string playOn, RecipientFilter filter)
        {
            switch (playOn.ToLowerInvariant())
            {
                case "player":
                    Debug.Print($"Playing quake sound on player {player.PlayerName}.");
                    _ = player.EmitSound(sound, filter);
                    break;
                case "world":
                    PlayOnWorld(sound, filter);
                    break;
                default:
                    Debug.Print($"Could not determine where to play sound (unknown config option play_on={_config.Global.PlayOnEntity}). Skipping.");
                    break;
            }
        }

        private void PlayOnWorld(string sound, RecipientFilter filter)
        {
            if (_worldEnt?.IsValid != true)
            {
                Debug.Print("Looking for world entity...");
                _worldEnt = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();
                if (_worldEnt?.IsValid != true)
                {
                    Debug.Print("Could not find world entity.");
                    return;
                }
            }

            Debug.Print("Playing quake sound on world entity.");
            _ = _worldEnt.EmitSound(sound, filter);
        }
    }
}
