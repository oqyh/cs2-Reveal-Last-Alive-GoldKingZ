using CounterStrikeSharp.API.Core;
using Reveal_Last_Alive.Config;
using CounterStrikeSharp.API.Modules.Commands;
using Newtonsoft.Json.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.UserMessages;

namespace Reveal_Last_Alive;

public class Game_UserMessages
{
    public HookResult HookPlayerChat_UserMessages(CCSPlayerController? player, string message, UserMessage? um = null)
    {
        if (!player.IsValid()) return HookResult.Continue;
        
        Helper.CheckPlayerInGlobals(player);

        if (Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
        {
            Handle_ReloadPlugin(player, null!, um!);
        }

        return HookResult.Continue;
    }

    #region Commands Hook

    public void CommandsAction_ReloadPlugin(CCSPlayerController? player, CommandInfo info)
    {
        if (!MainPlugin.Instance.g_EnablePlugin.Value || !player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);
        
        Handle_ReloadPlugin(player, info, null!);
    }
    
    #endregion Commands Hook




    #region Handles
    public static void Handle_ReloadPlugin(CCSPlayerController player, CommandInfo commandInfo = null!, UserMessage um = null!)
    {
        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;

        bool onetime = (DateTime.Now - playerData.EventPlayerChat).TotalSeconds > 0.4;
        if (onetime) playerData.EventPlayerChat = DateTime.Now;


        var cfg = Configs.Instance.Reload_Plugin;

        if (cfg.Reload_Plugin_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, cfg.Reload_Plugin_Flags))
        {
            if (onetime)
            {
                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Not.Allowed"]);
            }
        }
        else
        {
            if (onetime)
            {
                Server.NextFrame(() =>
                {
                    Helper.RemoveRegisterCommandsAndHooks();
                    Helper.ClearVariables();

                    Configs.Load(MainPlugin.Instance.ModuleDirectory);
                    Helper.DownloadMissingFiles();
                    Helper.RegisterCommandsAndHooks();

                    Helper.StopRevealIfNeeded();
                    Helper.CheckForReveal();
                });

                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Successfully"]);
            }

            Helper.MuteCommands(um, cfg.Reload_Plugin_Hide);
        }

        Helper.MuteCommands(um, cfg.Reload_Plugin_Hide, true);
    }

    #endregion Handles
}