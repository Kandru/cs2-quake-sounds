using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace QuakeSounds.Utils
{
    public static class GameRules
    {
        private static CCSGameRules? _gameRules;

        public static void Invalidate() => _gameRules = null;

        private static CCSGameRules? Resolve()
        {
            if (_gameRules == null)
            {
                _gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
                    .FirstOrDefault(static e => e != null && e.IsValid)?.GameRules;
            }
            return _gameRules;
        }

        public static bool IsWarmup => Resolve()?.WarmupPeriod == true;
    }
}
