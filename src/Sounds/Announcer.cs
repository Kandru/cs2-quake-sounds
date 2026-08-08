using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace QuakeSounds.Sounds
{
    public class Announcer(PluginConfig config, PlayerLanguageManager languageManager, IStringLocalizer localizer)
    {
        private readonly PluginConfig _config = config;
        private readonly PlayerLanguageManager _languageManager = languageManager;
        private readonly IStringLocalizer _localizer = localizer;

        public void Print(CCSPlayerController source, SoundEntry entry, RecipientFilter filter)
        {
            if (filter.Count == 0 || (!_config.Messages.CenterMessage && !_config.Messages.ChatMessage) || entry.Messages.Count == 0)
            {
                return;
            }

            foreach (CCSPlayerController recipient in filter.Where(static p => !p.IsBot && !p.IsHLTV))
            {
                SendToPlayer(recipient, source, entry);
            }
        }

        private void SendToPlayer(CCSPlayerController recipient, CCSPlayerController source, SoundEntry entry)
        {
            string? message = ResolveMessage(entry.Messages, recipient.SteamID);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            CultureInfo culture = _languageManager.GetLanguage(new SteamID(recipient.SteamID)) ?? CultureInfo.InvariantCulture;
            using WithTemporaryCulture _ = new(culture);

            SendCenter(recipient, source, message);
            SendChat(recipient, source, message);
        }

        private string? ResolveMessage(Dictionary<string, string> messages, ulong steamId)
        {
            if (_config.Data.PlayerLanguages.TryGetValue(steamId, out string? stored)
                && TryGet(messages, NormalizeLanguage(stored), out string? message))
            {
                return message;
            }

            if (TryGet(messages, NormalizeLanguage(CoreConfig.ServerLanguage), out message))
            {
                return message;
            }

            return messages.Values.FirstOrDefault();
        }

        private static bool TryGet(Dictionary<string, string> messages, string? languageKey, out string? message)
        {
            message = null;
            if (string.IsNullOrWhiteSpace(languageKey))
            {
                return false;
            }

            return messages.TryGetValue(languageKey, out message);
        }

        private static string NormalizeLanguage(string? languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                return string.Empty;
            }

            int separatorIndex = languageCode.IndexOf('-', StringComparison.Ordinal);
            string normalized = separatorIndex > 0 ? languageCode[..separatorIndex] : languageCode;
            normalized = normalized.Trim().ToLowerInvariant();
            return normalized.Equals("iv", StringComparison.OrdinalIgnoreCase) ? string.Empty : normalized;
        }

        private void SendCenter(CCSPlayerController recipient, CCSPlayerController source, string message)
        {
            if (!_config.Messages.CenterMessage)
            {
                return;
            }

            string text = Format(recipient, source, message, "center.msg");
            switch (_config.Messages.CenterMessageType.ToLowerInvariant())
            {
                case "default":
                    recipient.PrintToCenter(text);
                    break;
                case "alert":
                    recipient.PrintToCenterAlert(text);
                    break;
                case "html":
                    recipient.PrintToCenterHtml(text);
                    break;
            }
        }

        private void SendChat(CCSPlayerController recipient, CCSPlayerController source, string message)
        {
            if (!_config.Messages.ChatMessage)
            {
                return;
            }

            recipient.PrintToChat(Format(recipient, source, message, "chat.msg"));
        }

        private string Format(CCSPlayerController recipient, CCSPlayerController source, string message, string key)
        {
            string templateKey = recipient == source ? $"{key}.player" : $"{key}.other";
            string template = _localizer[templateKey];
            return recipient == source
                ? template.Replace("{message}", message)
                : template.Replace("{player}", source.PlayerName).Replace("{message}", message);
        }
    }
}
