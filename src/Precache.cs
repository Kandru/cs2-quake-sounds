using CounterStrikeSharp.API.Modules.Utils;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        private void OnServerPrecacheResources(ResourceManifest manifest)
        {
            if (!string.IsNullOrEmpty(Config.Precache.SoundEventFile))
            {
                manifest.AddResource(Config.Precache.SoundEventFile);
            }

            foreach (string file in Config.Precache.AdditionalFiles)
            {
                manifest.AddResource(file);
            }
        }
    }
}
