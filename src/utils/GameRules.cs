using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Collections.Concurrent;
using System.Reflection;

namespace QuakeSounds.Utils
{
    public static class GameRules
    {
        public static CCSGameRulesProxy? _gameRulesProxy;
        public static CCSGameRules? _gameRules;
        private static IEnumerable<CCSTeam>? _teamManager;
        private static readonly ConcurrentDictionary<string, PropertyInfo?> _rulePropertyCache = new(StringComparer.Ordinal);

        private static CCSGameRules? GetGameRule()
        {
            if (_gameRules == null
                || _gameRulesProxy == null
                || !_gameRulesProxy.IsValid)
            {
                _gameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
                    .FirstOrDefault(static e => e != null && e.IsValid);
                _gameRules = _gameRulesProxy?.GameRules;
            }
            return _gameRules;
        }

        private static bool TryGetGameRules(out CCSGameRules rules, out CCSGameRulesProxy proxy)
        {
            _ = GetGameRule();
            if (_gameRules != null && _gameRulesProxy != null)
            {
                rules = _gameRules;
                proxy = _gameRulesProxy;
                return true;
            }
            rules = null!;
            proxy = null!;
            return false;
        }

        public static object? Get(string rule)
        {
            if (!TryGetGameRules(out CCSGameRules? rules, out _))
            {
                return null;
            }

            Type type = rules.GetType();
            string key = string.Concat(type.FullName, ":", rule);
            PropertyInfo? property = _rulePropertyCache.GetOrAdd(key, _ => type.GetProperty(rule));
            return property?.CanRead == true ? property.GetValue(rules) : null;
        }

        public static void ResetCaches()
        {
            _gameRules = null;
            _gameRulesProxy = null;
            _teamManager = null;
            _rulePropertyCache.Clear();
        }
    }
}