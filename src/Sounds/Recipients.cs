using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Utils;

namespace QuakeSounds.Sounds
{
    public class Recipients(PluginConfig config)
    {
        private readonly PluginConfig _config = config;
        private readonly RecipientScope _defaultScope = SoundEntry.ParseScope(config.Global.SoundHearableBy);

        public RecipientFilter Build(CCSPlayerController? attacker, CCSPlayerController? victim, RecipientScope? scope = null)
        {
            RecipientScope filterType = scope ?? _defaultScope;
            RecipientFilter filter = [];

            foreach (CCSPlayerController player in GetEligiblePlayers().Where(p => Matches(p, attacker, victim, filterType)))
            {
                filter.Add(player);
            }

            Debug.Print($"Prepared filter ({filterType}): {string.Join(", ", filter.Select(p => p.PlayerName))}");
            return filter;
        }

        private IEnumerable<CCSPlayerController> GetEligiblePlayers()
        {
            return Utilities.GetPlayers().Where(p =>
                (!p.IsBot || !_config.Global.IgnoreBots) &&
                !_config.Data.PlayerMuted.Contains(p.SteamID));
        }

        private static bool Matches(CCSPlayerController player, CCSPlayerController? attacker, CCSPlayerController? victim, RecipientScope scope)
        {
            return scope switch
            {
                RecipientScope.AttackerTeam => player.Team == attacker?.Team,
                RecipientScope.VictimTeam => player.Team == victim?.Team,
                RecipientScope.Involved => player == attacker || player == victim,
                RecipientScope.Attacker => player == attacker,
                RecipientScope.Victim => player == victim,
                RecipientScope.Spectator => player.Team == CsTeam.Spectator,
                _ => true
            };
        }
    }
}
