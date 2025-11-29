using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using System.Globalization;

namespace QuakeSounds.Services
{
    public class MessageService(PluginConfig config, PlayerLanguageManager languageManager, Func<string, string> getLocalizedString)
    {
        private readonly PluginConfig _config = config;
        private readonly PlayerLanguageManager _languageManager = languageManager;
        private readonly Func<string, string> _getLocalizedString = getLocalizedString;

        public void PrintMessage(CCSPlayerController player, Dictionary<string, string> sound, RecipientFilter filter)
        {
            // Early return if no recipients or if both message types are disabled
            if (filter.Count == 0 || (!_config.Messages.CenterMessage && !_config.Messages.ChatMessage))
            {
                return;
            }

            foreach (CCSPlayerController recipient in filter.Where(p => !p.IsBot && !p.IsHLTV))
            {
                SendMessageToPlayer(recipient, player, sound);
            }
        }

        private void SendMessageToPlayer(CCSPlayerController recipient, CCSPlayerController player, Dictionary<string, string> sound)
        {
            CultureInfo playerCulture = _languageManager.GetLanguage(new SteamID(recipient.SteamID)) ?? CultureInfo.InvariantCulture;
            string? languageCode = ResolvePlayerLanguageCode(recipient.SteamID, playerCulture);

            string? message = GetLocalizedMessage(sound, languageCode);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            using WithTemporaryCulture culture = new(playerCulture);

            SendCenterMessage(recipient, player, message);
            SendChatMessage(recipient, player, message);
        }

        private string? GetLocalizedMessage(Dictionary<string, string> sound, string? playerLanguage)
        {
            if (TryGetMessage(sound, playerLanguage, out string? playerMessage))
            {
                return playerMessage;
            }

            string serverLanguage = NormalizeLanguageCode(CoreConfig.ServerLanguage);
            if (TryGetMessage(sound, serverLanguage, out string? serverMessage))
            {
                return serverMessage;
            }

            return sound.Values.FirstOrDefault();
        }

        private string? ResolvePlayerLanguageCode(ulong steamId, CultureInfo fallbackCulture)
        {
            if (_config.Data.PlayerLanguages.TryGetValue(steamId, out string? storedLanguage) && !string.IsNullOrWhiteSpace(storedLanguage))
            {
                return NormalizeLanguageCode(storedLanguage);
            }

            string cultureName = NormalizeLanguageCode(fallbackCulture.Name);
            if (!string.IsNullOrEmpty(cultureName))
            {
                return cultureName;
            }

            return NormalizeLanguageCode(fallbackCulture.TwoLetterISOLanguageName);
        }

        private static bool TryGetMessage(Dictionary<string, string> sound, string? languageKey, out string? message)
        {
            message = null;

            if (string.IsNullOrWhiteSpace(languageKey))
            {
                return false;
            }

            if (sound.TryGetValue(languageKey, out string? directMatch))
            {
                message = directMatch;
                return true;
            }

            string? caseInsensitiveMatch = sound.FirstOrDefault(
                pair => pair.Key.Equals(languageKey, StringComparison.OrdinalIgnoreCase)).Value;

            if (!string.IsNullOrEmpty(caseInsensitiveMatch))
            {
                message = caseInsensitiveMatch;
                return true;
            }

            return false;
        }

        private static string NormalizeLanguageCode(string? languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                return string.Empty;
            }

            int separatorIndex = languageCode.IndexOf('-', StringComparison.Ordinal);
            string normalized = separatorIndex > 0 ? languageCode[..separatorIndex] : languageCode;
            normalized = normalized.Trim();

            if (normalized.Length == 0 || normalized.Equals("iv", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return normalized.ToLowerInvariant();
        }

        private void SendCenterMessage(CCSPlayerController recipient, CCSPlayerController player, string message)
        {
            if (!_config.Messages.CenterMessage)
            {
                return;
            }

            string centerMessage = GetFormattedMessage(recipient, player, message, "center.msg");

            switch (_config.Messages.CenterMessageType.ToLower(CultureInfo.CurrentCulture))
            {
                case "default":
                    recipient.PrintToCenter(centerMessage);
                    break;
                case "alert":
                    recipient.PrintToCenterAlert(centerMessage);
                    break;
                case "html":
                    recipient.PrintToCenterHtml(centerMessage);
                    break;
                default:
                    break;
            }
        }

        private void SendChatMessage(CCSPlayerController recipient, CCSPlayerController player, string message)
        {
            if (!_config.Messages.ChatMessage)
            {
                return;
            }

            string chatMessage = GetFormattedMessage(recipient, player, message, "chat.msg");
            recipient.PrintToChat(chatMessage);
        }

        private string GetFormattedMessage(CCSPlayerController recipient, CCSPlayerController player, string message, string localizerKey)
        {
            string templateKey = recipient == player ? $"{localizerKey}.player" : $"{localizerKey}.other";
            string template = _getLocalizedString(templateKey);

            return recipient == player
                ? template.Replace("{message}", message)
                : template.Replace("{player}", player.PlayerName).Replace("{message}", message);
        }
    }
}
