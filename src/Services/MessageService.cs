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
            foreach (CCSPlayerController recipient in filter)
            {
                SendMessageToPlayer(recipient, player, sound);
            }
        }

        private void SendMessageToPlayer(CCSPlayerController recipient, CCSPlayerController player, Dictionary<string, string> sound)
        {
            string? message = GetLocalizedMessage(recipient, sound);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            using WithTemporaryCulture culture = new(new CultureInfo(_languageManager.GetLanguage(new SteamID(recipient.SteamID)).TwoLetterISOLanguageName));

            SendCenterMessage(recipient, player, message);
            SendChatMessage(recipient, player, message);
        }

        private string? GetLocalizedMessage(CCSPlayerController recipient, Dictionary<string, string> sound)
        {
            string playerLanguage = _languageManager.GetLanguage(new SteamID(recipient.SteamID)).TwoLetterISOLanguageName;

            return sound.TryGetValue(playerLanguage, out string? playerMessage) ? playerMessage :
                   sound.TryGetValue(CoreConfig.ServerLanguage, out string? serverMessage) ? serverMessage :
                   sound.Values.FirstOrDefault();
        }

        private void SendCenterMessage(CCSPlayerController recipient, CCSPlayerController player, string message)
        {
            if (!_config.CenterMessage)
            {
                return;
            }

            string centerMessage = GetFormattedMessage(recipient, player, message, "center.msg");

            switch (_config.CenterMessageType.ToLower(CultureInfo.CurrentCulture))
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
            if (!_config.ChatMessage)
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
