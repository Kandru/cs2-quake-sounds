using CounterStrikeSharp.API.Modules.Utils;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        private readonly List<string> _precacheModels = [];

        private void OnServerPrecacheResources(ResourceManifest manifest)
        {
            // add soundevent file to precache
            if (!string.IsNullOrEmpty(Config.Precache.SoundEventFile))
            {
                manifest.AddResource(Config.Precache.SoundEventFile);
            }
            // add list to precache
            foreach (string model in _precacheModels)
            {
                manifest.AddResource(model);
            }
        }
    }
}
