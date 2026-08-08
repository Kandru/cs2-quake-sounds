using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using QuakeSounds.Utils;

namespace QuakeSounds.Sounds
{
    public class SoundPlayer(
        PluginConfig config,
        Dictionary<string, SoundEntry> entries,
        KillSoundKind[] killPriority,
        SoundEmitter emitter,
        Announcer announcer,
        Recipients recipients,
        Dictionary<int, int> killCounter)
    {
        private readonly PluginConfig _config = config;
        private readonly Dictionary<string, SoundEntry> _entries = entries;
        private readonly KillSoundKind[] _killPriority = killPriority;
        private readonly SoundEmitter _emitter = emitter;
        private readonly Announcer _announcer = announcer;
        private readonly Recipients _recipients = recipients;
        private readonly Dictionary<int, int> _killCounter = killCounter;

        public void PlayKill(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData)
        {
            int killCount = _killCounter.GetValueOrDefault(attacker.Slot);

            foreach (KillSoundKind kind in _killPriority)
            {
                bool played = kind switch
                {
                    KillSoundKind.SpecialEvents => TryPlaySpecial(attacker, victim, eventData),
                    KillSoundKind.Weapons => TryPlayWeapon(attacker, victim, eventData),
                    KillSoundKind.KillStreak => TryPlayKillStreak(attacker, victim, eventData, killCount),
                    _ => false
                };

                if (played)
                {
                    return;
                }
            }
        }

        public void PlayRound(string soundKey) => PlayGlobal($"round_{soundKey}");

        public void PlayBomb(string soundKey) => PlayGlobal($"bomb_{soundKey}");

        private void PlayGlobal(string fullKey)
        {
            if (!_config.Enabled || !_entries.TryGetValue(fullKey, out SoundEntry? entry))
            {
                return;
            }

            RecipientFilter filter = _recipients.Build(null, null, entry.Scope);
            _emitter.PlayGlobal(entry.PickSound(), filter);
        }

        private bool TryPlayKillStreak(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData, int killCount)
        {
            if (eventData.Weapon.Equals("world", StringComparison.OrdinalIgnoreCase) && _config.Global.IgnoreWorldDamage)
            {
                return false;
            }

            return Play(attacker, victim, killCount.ToString());
        }

        private bool TryPlayWeapon(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData)
        {
            string weapon = eventData.Weapon;
            string weaponKey = weapon.StartsWith("weapon_", StringComparison.OrdinalIgnoreCase)
                ? weapon.ToLowerInvariant()
                : "weapon_" + weapon.ToLowerInvariant();
            return Play(attacker, victim, weaponKey);
        }

        private bool TryPlaySpecial(CCSPlayerController attacker, CCSPlayerController? victim, EventPlayerDeath eventData)
        {
            // Preserve fall-through: a matching condition whose sound is missing continues to the next candidate.
            if (TryPlayLastManStanding(attacker, victim))
            {
                return true;
            }

            if (attacker == victim && Play(attacker, victim, "selfkill", playOn: "world"))
            {
                return true;
            }

            if (victim?.IsValid == true && attacker.Team == victim.Team && Play(attacker, victim, "teamkill"))
            {
                return true;
            }

            if (IsFirstBlood(attacker) && Play(attacker, victim, "firstblood"))
            {
                return true;
            }

            if (eventData.Weapon.Contains("knife", StringComparison.OrdinalIgnoreCase) && Play(attacker, victim, "knifekill"))
            {
                return true;
            }

            if (eventData.Headshot && Play(attacker, victim, "headshot"))
            {
                return true;
            }

            return false;
        }

        private bool IsFirstBlood(CCSPlayerController attacker)
        {
            return _killCounter.TryGetValue(attacker.Slot, out int kills)
                && _killCounter.Count == 1
                && kills == 1;
        }

        private bool TryPlayLastManStanding(CCSPlayerController attacker, CCSPlayerController? victim)
        {
            if (attacker?.IsValid != true
                || victim?.IsValid != true
                || victim.Team is CsTeam.Spectator or CsTeam.None)
            {
                return false;
            }

            List<CCSPlayerController> alive = [.. Utilities.GetPlayers()
                .Where(p => p.Team == victim.Team && p.Pawn?.Value?.LifeState == (uint)LifeState_t.LIFE_ALIVE)];

            return alive.Count == 1 && Play(attacker, alive[0], "lastmanstanding", source: alive[0]);
        }

        private bool Play(
            CCSPlayerController attacker,
            CCSPlayerController? victim,
            string soundKey,
            CCSPlayerController? source = null,
            string? playOn = null)
        {
            if (!_config.Enabled || !_entries.TryGetValue(soundKey, out SoundEntry? entry))
            {
                return false;
            }

            RecipientFilter filter = _recipients.Build(attacker, victim, entry.Scope);
            if (filter.Count == 0)
            {
                return true; // Sound matched but nobody listens — still counts as played for priority.
            }

            CCSPlayerController emitSource = source ?? attacker;
            string soundName = entry.PickSound();
            _emitter.Play(emitSource, soundName, filter, playOn ?? _config.Global.PlayOnEntity);
            _announcer.Print(emitSource, entry, filter);
            return true;
        }
    }
}
