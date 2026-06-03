using CounterStrikeSharp.API.Modules.Utils;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        private void OnServerPrecacheResources(ResourceManifest manifest)
        {
            // add soundevent file to precache
            if (!string.IsNullOrEmpty(Config.Precache.SoundEventFile))
            {
                manifest.AddResource(Config.Precache.SoundEventFile);
            }
            // add additional files to precache (if any)
            if (Config.Precache.AdditionalFiles.Count > 0)
            {
                foreach(string file in Config.Precache.AdditionalFiles)
                {
                    manifest.AddResource(file);
                }
            }
        }
    }
}
