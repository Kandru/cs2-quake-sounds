using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace QuakeSounds.Services
{
    public class FilterService(PluginConfig config, Action<string> debugPrint)
    {
        private readonly PluginConfig _config = config;
        private readonly Action<string> _debugPrint = debugPrint;

        public RecipientFilter PrepareFilter(CCSPlayerController? attacker, CCSPlayerController? victim, string? soundFilter = null)
        {
            RecipientFilter filter = [];
            string filterType = soundFilter ?? _config.FilterSounds;

            foreach (CCSPlayerController? player in GetEligiblePlayers().Where(p => MatchesFilter(p, attacker, victim, filterType)))
            {
                filter.Add(player);
            }

            _debugPrint($"Prepared filter ({soundFilter ?? "all"}): {string.Join(", ", filter.Select(p => p.PlayerName))}");
            return filter;
        }

        private IEnumerable<CCSPlayerController> GetEligiblePlayers()
        {
            return Utilities.GetPlayers().Where(p =>
                (!p.IsBot || !_config.IgnoreBots) &&
                !_config.PlayersMuted.Contains(p.SteamID));
        }

        private static bool MatchesFilter(CCSPlayerController player, CCSPlayerController? attacker, CCSPlayerController? victim, string filterType)
        {
            return filterType.ToLower(System.Globalization.CultureInfo.CurrentCulture) switch
            {
                "all" => true,
                "attacker_team" => player.Team == attacker?.Team,
                "victim_team" => player.Team == victim?.Team,
                "involved" => player == attacker || player == victim,
                "attacker" => player == attacker,
                "victim" => player != attacker && player != victim,
                "spectator" => player.Team == CsTeam.Spectator,
                _ => true
            };
        }
    }
}
