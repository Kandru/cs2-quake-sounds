using System.Globalization;

namespace QuakeSounds.Sounds
{
    public enum RecipientScope
    {
        All,
        AttackerTeam,
        VictimTeam,
        Involved,
        Attacker,
        Victim,
        Spectator
    }

    public enum KillSoundKind
    {
        SpecialEvents,
        Weapons,
        KillStreak
    }

    public sealed class SoundEntry
    {
        public required string Key { get; init; }
        public required string[] SoundNames { get; init; }
        public required Dictionary<string, string> Messages { get; init; }
        public float? Volume { get; init; }
        public RecipientScope? Scope { get; init; }

        public string PickSound() => SoundNames.Length == 1
            ? SoundNames[0]
            : SoundNames[Random.Shared.Next(SoundNames.Length)];

        public static SoundEntry? TryParse(string key, Dictionary<string, string> raw)
        {
            if (!raw.TryGetValue("_sound", out string? soundName) || string.IsNullOrWhiteSpace(soundName))
            {
                return null;
            }

            string[] soundNames = [.. soundName.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
            if (soundNames.Length == 0)
            {
                return null;
            }

            Dictionary<string, string> messages = raw
                .Where(static kvp => !kvp.Key.StartsWith('_'))
                .ToDictionary(static kvp => kvp.Key, static kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

            float? volume = null;
            if (raw.TryGetValue("_volume", out string? volumeRaw)
                && float.TryParse(volumeRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedVolume))
            {
                volume = parsedVolume;
            }

            RecipientScope? scope = null;
            if (raw.TryGetValue("_filter", out string? filterRaw))
            {
                scope = ParseScope(filterRaw);
            }

            return new SoundEntry
            {
                Key = key,
                SoundNames = soundNames,
                Messages = messages,
                Volume = volume,
                Scope = scope
            };
        }

        public static RecipientScope ParseScope(string? value)
        {
            return (value ?? "all").Trim().ToLowerInvariant() switch
            {
                "attacker_team" => RecipientScope.AttackerTeam,
                "victim_team" => RecipientScope.VictimTeam,
                "involved" => RecipientScope.Involved,
                "attacker" => RecipientScope.Attacker,
                "victim" => RecipientScope.Victim,
                "spectator" => RecipientScope.Spectator,
                _ => RecipientScope.All
            };
        }
    }
}
