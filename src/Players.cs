using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Globalization;
using QuakeSounds.Utils;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        private readonly PlayerLanguageManager _playerLanguageManager = new();
        private readonly Dictionary<int, int> _playerKillCounter = [];

        private bool ToggleMute(CCSPlayerController player)
        {
            if (Config.Data.PlayerMuted.Contains(player.SteamID))
            {
                _ = Config.Data.PlayerMuted.Remove(player.SteamID);
                Config.Update();
                player.PrintToChat(Localizer["sounds.unmuted"]);
                return false;
            }

            Config.Data.PlayerMuted.Add(player.SteamID);
            Config.Update();
            player.PrintToChat(Localizer["sounds.muted"]);
            return true;
        }

        private void LoadPlayerLanguage(ulong? steamID)
        {
            if (!steamID.HasValue)
            {
                return;
            }

            if (Config.Data.PlayerLanguages.TryGetValue(steamID.Value, out string? language) && !string.IsNullOrWhiteSpace(language))
            {
                _playerLanguageManager.SetLanguage(new SteamID(steamID.Value), GetCultureOrInvariant(language));
            }
        }

        private void SavePlayerLanguage(ulong? steamID, string language)
        {
            if (!steamID.HasValue || string.IsNullOrWhiteSpace(language))
            {
                return;
            }

            if (!Config.Data.PlayerLanguages.TryAdd(steamID.Value, language))
            {
                Config.Data.PlayerLanguages[steamID.Value] = language;
            }

            Config.Update();
            _playerLanguageManager.SetLanguage(new SteamID(steamID.Value), GetCultureOrInvariant(language));
            Debug.Print($"Saved language for player {steamID} to {language}.");
        }

        private static CultureInfo GetCultureOrInvariant(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return CultureInfo.InvariantCulture;
            }

            try
            {
                return new CultureInfo(language);
            }
            catch (CultureNotFoundException)
            {
                Console.WriteLine($"[QuakeSounds] Culture '{language}' not available, using default (invariant).");
                return CultureInfo.InvariantCulture;
            }
        }

        private void ResetKill(CCSPlayerController player)
        {
            if (Config.Global.ResetKillsOnDeath)
            {
                _playerKillCounter[player.Slot] = 0;
            }
        }

        private void ClearKills() => _playerKillCounter.Clear();

        private void RemoveKill(CCSPlayerController player) => _playerKillCounter.Remove(player.Slot);

        private bool ShouldCountKill(CCSPlayerController attacker, CCSPlayerController? victim)
        {
            if (attacker == victim)
            {
                return Config.Global.CountSelfKills;
            }

            if (victim?.IsValid == true && attacker.Team == victim.Team)
            {
                return Config.Global.CountTeamKills;
            }

            return true;
        }

        private void UpdateKillCount(CCSPlayerController attacker)
        {
            _ = _playerKillCounter.TryGetValue(attacker.Slot, out int kills);
            _playerKillCounter[attacker.Slot] = kills + 1;
            Debug.Print($"Player {attacker.PlayerName} has {_playerKillCounter[attacker.Slot]} kills.");
        }
    }
}
