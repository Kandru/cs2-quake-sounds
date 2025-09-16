using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Extensions;
using CounterStrikeSharp.API.Modules.Menu;
using Microsoft.Extensions.Localization;

namespace QuakeSounds
{
    public partial class QuakeSounds
    {
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 0, usage: "")]
        public void CommandQuakeSoundSettings(CCSPlayerController? player, CommandInfo command)
        {
            if (player?.IsValid != true)
            {
                return;
            }

            if (Config.Commands.SettingsMenu)
            {
                ShowSettingsMenu(player);
            }
            else
            {
                _ = ToggleMute(player);
            }
        }

        [ConsoleCommand("quakesounds", "QuakeSounds admin commands")]
        [CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY, minArgs: 1, usage: "<command>")]
        public void CommandQuakeSoundAdmin(CCSPlayerController player, CommandInfo command)
        {
            string subCommand = command.GetArg(1).ToLower(System.Globalization.CultureInfo.CurrentCulture);
            string response = ProcessAdminCommand(subCommand);
            command.ReplyToCommand(response);
        }

        public void OnLanguageCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player?.IsValid != true)
            {
                return;
            }

            string text = command.GetCommandString;
            ProcessLanguageCommand(player, text);
        }

        private void ShowSettingsMenu(CCSPlayerController player)
        {
            MenuManager.CloseActiveMenu(player);
            ChatMenu menu = new(Localizer["menu.title"]);

            LocalizedString menuText = Config.Data.PlayerMuted.Contains(player.SteamID)
                ? Localizer["menu.unmute"]
                : Localizer["menu.mute"];

            _ = menu.AddMenuOption(menuText, (_, _) => ToggleMute(player));
            MenuManager.OpenChatMenu(player, menu);
        }

        private string ProcessAdminCommand(string subCommand)
        {
            return subCommand switch
            {
                "reload" => ReloadConfig(),
                "disable" => DisablePlugin(),
                "enable" => EnablePlugin(),
                _ => Localizer["admin.unknown_command"].Value.Replace("{command}", subCommand)
            };
        }

        private string ReloadConfig()
        {
            Config.Reload();
            return Localizer["admin.reload"];
        }

        private string DisablePlugin()
        {
            Config.Enabled = false;
            Config.Update();
            return Localizer["admin.disable"];
        }

        private string EnablePlugin()
        {
            Config.Enabled = true;
            Config.Update();
            return Localizer["admin.enable"];
        }
    }
}
