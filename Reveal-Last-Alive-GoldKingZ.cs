using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Utils;
using Reveal_Last_Alive.Config;
using System.Drawing;
using System.Text;

namespace Reveal_Last_Alive;

public class MainPlugin : BasePlugin
{
    public override string ModuleName => "Reveal Last Player Alive By Glow/Chicken";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    public static MainPlugin Instance { get; set; } = new();
    public Globals g_Main = new();
    public readonly Game_UserMessages Game_UserMessages = new();
    public FakeConVar<bool> g_EnablePlugin = new("gkz_reveal", "Reveal Last Alive Plugin [true = Enable / false = Disable]", true);

    public override void Load(bool hotReload)
    {
        Instance = this;
        RegisterFakeConVars(typeof(ConVar));
        Configs.Load(ModuleDirectory);

        Helper.RemoveRegisterCommandsAndHooks();
        Helper.ClearVariables();

        Helper.DownloadMissingFiles();
        Helper.RegisterCommandsAndHooks();

        if (hotReload)
        {            
            Helper.RemoveRegisterCommandsAndHooks();
            Helper.ClearVariables();

            Helper.DownloadMissingFiles();
            Helper.RegisterCommandsAndHooks();

            Helper.StopRevealIfNeeded();
            Helper.CheckForReveal();
        }
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        manifest.AddResource("models/chicken/chicken.vmdl");
    }

    public void OnMapStart(string Map)
    {
        Helper.DownloadMissingFiles();
    }

    public HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (@event == null || !g_EnablePlugin.Value)return HookResult.Continue;

        Helper.ClearVariables();
        
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event == null || !g_EnablePlugin.Value) return HookResult.Continue;

        Helper.StopRevealIfNeeded();
        Helper.CheckForReveal();
        
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;
        if(!player.IsValid(true)) return HookResult.Continue;
        
        player.RemoveGlowAndChicken();

        return HookResult.Continue;
    }

    public HookResult OnPlayerSay(CCSPlayerController? player, CommandInfo info)
    {
        return HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public HookResult OnPlayerSay_Team(CCSPlayerController? player, CommandInfo info)
    {
        return HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public HookResult OnUserMessage_OnSayText2(CounterStrikeSharp.API.Modules.UserMessages.UserMessage um)
    {
        var player = Utilities.GetPlayerFromIndex(um.ReadInt("entityindex"));
        return HandlePlayerMessage(player, Encoding.UTF8.GetString(um.ReadBytes("param2")), um);
    }
    
    private HookResult HandlePlayerMessage(CCSPlayerController? player, string? rawMessage, CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um = null)
    {
        if (!g_EnablePlugin.Value || !player.IsValid()) return HookResult.Continue;
        if (string.IsNullOrWhiteSpace(rawMessage)) return HookResult.Continue;

        string message = rawMessage.Trim();
        Game_UserMessages.HookPlayerChat_UserMessages(player, message, um);

        return HookResult.Continue;
    }
    
    public void OnMapEnd()
    {
        try
        {
            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"OnMapEnd Error: {ex.Message}", true);
        }
    }

    public override void Unload(bool hotReload)
    {
        try
        {
            Helper.RemoveRegisterCommandsAndHooks();
            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Unload Error: {ex.Message}", true);
        }

        if (hotReload)
        {
            try
            {
                Helper.RemoveRegisterCommandsAndHooks();
                Helper.ClearVariables();
            }
            catch (Exception ex)
            {
                Helper.DebugMessage($"Unload hotReload Error: {ex.Message}", true);
            }
        }
    }



    /* [ConsoleCommand("css_test", "test")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (!player.IsValid()) return;
    } */
    
}