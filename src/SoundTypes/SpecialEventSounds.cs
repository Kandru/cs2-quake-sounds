using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Services;

namespace QuakeSounds.SoundTypes
{
    public class SpecialEventSounds(PluginConfig config, SoundService soundService, MessageService messageService,
        FilterService filterService, Dictionary<CCSPlayerController, int> playerKillsInRound) : BaseSoundType(config, soundService, messageService, filterService)
    {
        private readonly Dictionary<CCSPlayerController, int> _playerKillsInRound = playerKillsInRound;

        public bool TryToPlay(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData)
        {
            (string, Func<bool>)[] specialSounds =
            [
                ("lastmanstanding", () => TryPlayLastManStanding(victim)),
                ("selfkill", () => attacker == victim && PlaySound(attacker, victim, "selfkill", "world")),
                ("teamkill", () => victim?.IsValid == true && attacker.Team == victim.Team && PlaySound(attacker, victim, "teamkill", null)),
                ("firstblood", () => IsFirstBlood(attacker) && PlaySound(attacker, victim, "firstblood", null)),
                ("knifekill", () => eventData.Weapon.Contains("knife", StringComparison.OrdinalIgnoreCase) && PlaySound(attacker, victim, "knifekill", null)),
                ("headshot", () => eventData.Headshot && PlaySound(attacker, victim, "headshot", null)),
            ];

            foreach ((string _, Func<bool> tryPlay) in specialSounds)
            {
                if (tryPlay())
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsFirstBlood(CCSPlayerController attacker)
        {
            return _playerKillsInRound.TryGetValue(attacker, out int kills) &&
                   _playerKillsInRound.Count == 1 && kills == 1;
        }

        private bool TryPlayLastManStanding(CCSPlayerController? victim)
        {
            if (victim?.IsValid != true || victim.Team == CsTeam.Spectator || victim.Team == CsTeam.None)
            {
                Console.WriteLine("False");
                return false;
            }
            var alivePlayers = Utilities.GetPlayers().Where(p => p.Team == victim.Team && p.Pawn?.Value?.LifeState == (uint)LifeState_t.LIFE_ALIVE).ToList();
            Console.WriteLine(alivePlayers);
            return alivePlayers.Count == 1 && PlaySound(alivePlayers[0], victim, "lastmanstanding", null);
        }
    }
}
