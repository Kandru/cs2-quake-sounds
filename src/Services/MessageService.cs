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
            string? message = GetMessageForPlayer(sound, recipient.SteamID);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            CultureInfo playerCulture = _languageManager.GetLanguage(new SteamID(recipient.SteamID)) ?? CultureInfo.InvariantCulture;
            using WithTemporaryCulture culture = new(playerCulture);

            SendCenterMessage(recipient, player, message);
            SendChatMessage(recipient, player, message);
        }

        private string? GetMessageForPlayer(Dictionary<string, string> sound, ulong playerSteamId)
        {
            // Try player's stored language
            if (_config.Data.PlayerLanguages.TryGetValue(playerSteamId, out string? storedLanguage) && 
                TryGetLocalizedMessage(sound, NormalizeLanguageCode(storedLanguage), out string? message))
            {
                return message;
            }

            // Try server language
            if (TryGetLocalizedMessage(sound, NormalizeLanguageCode(CoreConfig.ServerLanguage), out message))
            {
                return message;
            }

            // Return first available message
            return sound.Values.FirstOrDefault();
        }

        private static bool TryGetLocalizedMessage(Dictionary<string, string> sound, string? languageKey, out string? message)
        {
            message = null;
            if (string.IsNullOrWhiteSpace(languageKey))
            {
                return false;
            }

            foreach (var kvp in sound)
            {
                if (kvp.Key.Equals(languageKey, StringComparison.OrdinalIgnoreCase))
                {
                    message = kvp.Value;
                    return true;
                }
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
            normalized = normalized.Trim().ToLowerInvariant();

            return normalized.Equals("iv", StringComparison.OrdinalIgnoreCase) ? string.Empty : normalized;
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
