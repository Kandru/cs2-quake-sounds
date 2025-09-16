using CounterStrikeSharp.API.Core;
using QuakeSounds.Services;
using System.Globalization;

namespace QuakeSounds.SoundTypes
{
    public class WeaponSounds(PluginConfig config, SoundService soundService, MessageService messageService, FilterService filterService) : BaseSoundType(config, soundService, messageService, filterService)
    {
        public bool TryPlayWeaponSound(CCSPlayerController attacker, CCSPlayerController? victim, string weapon)
        {
            string weaponKey = GetWeaponKey(weapon);
            return PlaySound(attacker, victim, weaponKey);
        }

        private static string GetWeaponKey(string weapon)
        {
            return weapon.StartsWith("weapon_", StringComparison.OrdinalIgnoreCase)
                ? weapon.ToLower(CultureInfo.CurrentCulture)
                : "weapon_" + weapon.ToLower(CultureInfo.CurrentCulture);
        }
    }
}
