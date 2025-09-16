using System.Reflection;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        private void DebugPrint(string message)
        {
            if (Config.Debug)
            {
                Console.WriteLine(Localizer["core.debugprint"].Value.Replace("{message}", message));
            }
        }

        private static object? GetGameRule(string rule)
        {
            IEnumerable<CCSGameRulesProxy> ents = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            foreach (CCSGameRulesProxy ent in ents)
            {
                CCSGameRules? gameRules = ent.GameRules;
                if (gameRules == null)
                {
                    continue;
                }

                PropertyInfo? property = gameRules.GetType().GetProperty(rule);
                if (property != null && property.CanRead)
                {
                    return property.GetValue(gameRules);
                }
            }
            return null;
        }
    }
}