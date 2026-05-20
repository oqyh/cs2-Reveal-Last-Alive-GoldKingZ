using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.RegularExpressions;
using Reveal_Last_Alive.Config;
using CounterStrikeSharp.API.Core.Translations;
using System.Security.Cryptography;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Timers;
using System.Drawing;


namespace Reveal_Last_Alive;

public class Helper
{

    public static void RegisterCommandsAndHooks()
    {
        Server.ExecuteCommand("sv_hibernate_when_empty false");

        MainPlugin.Instance.RegisterListener<Listeners.OnServerPrecacheResources>(MainPlugin.Instance.OnServerPrecacheResources);
        MainPlugin.Instance.RegisterListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RegisterListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.RegisterEventHandler<EventRoundStart>(MainPlugin.Instance.OnEventRoundStart);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerDeath>(MainPlugin.Instance.OnEventPlayerDeath);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerDisconnect>(MainPlugin.Instance.OnEventPlayerDisconnect);

        MainPlugin.Instance.AddCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.AddCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.HookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        RegisterCssCommands(Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(), "Commands To Reload Reveal Last Alive Plugin", MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);
    }

    public static void RemoveRegisterCommandsAndHooks()
    {
        MainPlugin.Instance.RemoveListener<Listeners.OnServerPrecacheResources>(MainPlugin.Instance.OnServerPrecacheResources);
        MainPlugin.Instance.RemoveListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RemoveListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.DeregisterEventHandler<EventRoundStart>(MainPlugin.Instance.OnEventRoundStart);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerDeath>(MainPlugin.Instance.OnEventPlayerDeath);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerDisconnect>(MainPlugin.Instance.OnEventPlayerDisconnect);

        MainPlugin.Instance.RemoveCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.RemoveCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.UnhookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        RemoveCssCommands(Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(), MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);

        CustomGameData.Unload(); 
    }

    public static void RegisterCssCommands(string[]? commands, string description, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd)) continue;
            MainPlugin.Instance.AddCommand(cmd, description, callback);
        }
    }
    public static void RemoveCssCommands(string[]? commands, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd)) continue;
            MainPlugin.Instance.RemoveCommand(cmd, callback);
        }
    }
    
    public static void CheckPlayerInGlobals(CCSPlayerController player)
    {
        if (!player.IsValid()) return;

        var g_Main = MainPlugin.Instance.g_Main;
        if (!g_Main.Player_Data.ContainsKey(player.Slot))
        {
            var initialData = new Globals.PlayerDataClass(
                player,
                DateTime.MinValue
            );
            g_Main.Player_Data.TryAdd(player.Slot, initialData);
        }else
        {
            g_Main.Player_Data[player.Slot].Player = player;
        }
    }

    public static void MuteCommands(CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um, int Config, bool Fully = false)
    {
        if (um == null) return;
        if (!Fully && Config == 2 || Fully && Config > 0)
        {
            um.Recipients.Clear();
        }
    }

    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, CounterStrikeSharp.API.Modules.Commands.CommandInfo commandInfo, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i]?.ToString() ?? "");
        }

        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
                    {
                        player.PrintToConsole(" " + trimmedPart);
                    }
                    else
                    {
                        player.PrintToChat(" " + trimmedPart);
                    }
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
            {
                player.PrintToConsole(message);
            }
            else
            {
                player.PrintToChat(message);
            }
        }
    }

    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        if (string.IsNullOrEmpty(groups) || player == null || !player.IsValid)
            return false;

        return groups.Split('|')
            .Select(segment => segment.Trim())
            .Any(trimmedSegment => Permission_CheckPermissionSegment(player, trimmedSegment));
    }

    private static bool Permission_CheckPermissionSegment(CCSPlayerController player, string segment)
    {
        if (string.IsNullOrEmpty(segment)) return false;

        int colonIndex = segment.IndexOf(':');
        if (colonIndex == -1 || colonIndex == 0) return false;

        string prefix = segment.Substring(0, colonIndex).Trim().ToLower();
        string values = segment.Substring(colonIndex + 1).Trim();

        return prefix switch
        {
            "steamid" or "steamids" or "steam" or "steams" => Permission_CheckSteamIds(player, values),
            "flag" or "flags" => Permission_CheckFlags(player, values),
            "group" or "groups" => Permission_CheckGroups(player, values),
            _ => false
        };
    }

    private static bool Permission_CheckSteamIds(CCSPlayerController player, string steamIds)
    {
        if (string.IsNullOrEmpty(steamIds)) return false;

        steamIds = steamIds.Replace("[", "").Replace("]", "");

        var (steam2, steam3, steam32, steam64) = player.SteamID.GetPlayerSteamID();
        var steam3NoBrackets = steam3.Trim('[', ']');

        return steamIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim())
            .Any(trimmedId =>
                string.Equals(trimmedId, steam2, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam3NoBrackets, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam32, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam64, StringComparison.OrdinalIgnoreCase)
            );
    }

    private static bool Permission_CheckFlags(CCSPlayerController player, string flags)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(flags))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null)
            return false;

        var requiredFlags = flags
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToList();

        if (playerData._flags != null &&
            requiredFlags.Any(reqFlag =>
                playerData._flags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase)))
            return true;

        var allFlags = playerData.GetAllFlags();
        return allFlags != null &&
            requiredFlags.Any(reqFlag =>
                allFlags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase));
    }

    private static bool Permission_CheckGroups(CCSPlayerController player, string groups)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(groups))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null || playerData.Groups == null || !playerData.Groups.Any())
            return false;

        return groups
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Any(reqGroup => playerData.Groups.Contains(reqGroup, StringComparer.OrdinalIgnoreCase));
    }
    
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeNone = true, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p =>
                p != null &&
                p.IsValid &&
                p.Connected == PlayerConnectedState.Connected &&
                (IncludeBots || !p.IsBot) &&
                (IncludeHLTV || !p.IsHLTV) &&
                ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
                (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) ||
                (IncludeNone && p.TeamNum == (byte)CsTeam.None) ||
                (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p =>
            p != null &&
            p.IsValid &&
            p.Connected == PlayerConnectedState.Connected &&
            (IncludeBots || !p.IsBot) &&
            (IncludeHLTV || !p.IsHLTV) &&
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) ||
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }

    public static void ClearVariables()
    {
        var g_Main = MainPlugin.Instance.g_Main;

        g_Main.Clear();
    }


    public static void DebugMessage(string message, bool important = false)
    {
        if (!Configs.Instance.EnableDebug && !important) return;
        var color = important ? Con.Red : Con.Magenta;
        string output = $"[Reveal Last Alive]: {message}";
        Con.WriteLine(color + output);
    }

    public static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }

    

    public static void DownloadMissingFiles()
    {
        if(MainPlugin.Instance.g_Main.Downloading_FromGithub)return;

        MainPlugin.Instance.g_Main.Downloading_FromGithub = true;

        _ = Task.Run(async () =>
        {
            try
            {
                await DownloadMissingFilesAsync();
            }
            finally
            {
                MainPlugin.Instance.g_Main.Downloading_FromGithub = false;
            }
        });
    }
    public static async Task DownloadMissingFilesAsync()
    {
        try
        {
            await Start_DownloadMissingFiles();
            await Server.NextFrameAsync(CustomGameData.Load);
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadMissingFilesAsync failed: {ex.Message}");
        }
    }
    public static async Task Start_DownloadMissingFiles()
    {
        try
        {
            string localPath_gamedata = Path.Combine(MainPlugin.Instance.ModuleDirectory, "gamedata/gamedata.json");
            string githubUrl_gamedata = "https://raw.githubusercontent.com/oqyh/cs2-Private-Plugins/main/Resources/gamedata.json";
            await DownloadFromGitHub(localPath_gamedata, githubUrl_gamedata, Configs.Instance.AutoUpdateSignatures);
            
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadMissingFiles Error: {ex.Message}");
        }
    }

    private static readonly HttpClient _httpClient_Github = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(50)
    };
    private static readonly TimeSpan _timeout_Github = TimeSpan.FromSeconds(50);
    public static async Task DownloadFromGitHub(string filePath, string githubUrl, bool AutoUpdate = false)
    {
        try
        {
            string fullPath = Path.Combine(MainPlugin.Instance.ModuleDirectory, filePath);

            string? dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _httpClient_Github.DefaultRequestHeaders.Remove("User-Agent");
            _httpClient_Github.DefaultRequestHeaders.Add("User-Agent", "CS2-Reveal");

            string actualDownloadUrl = githubUrl;

            if (githubUrl.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                using var ctsTxt = new CancellationTokenSource(_timeout_Github);
                var txtResponse = await _httpClient_Github.GetAsync(githubUrl, ctsTxt.Token);
                txtResponse.EnsureSuccessStatusCode();
                actualDownloadUrl = (await txtResponse.Content.ReadAsStringAsync()).Trim();
            }

            using var ctsBytes = new CancellationTokenSource(_timeout_Github);
            var bytesResponse = await _httpClient_Github.GetAsync(actualDownloadUrl, ctsBytes.Token);
            bytesResponse.EnsureSuccessStatusCode();
            byte[] remoteBytes = await bytesResponse.Content.ReadAsByteArrayAsync();

            bool needDownload = !File.Exists(fullPath);

            if (!needDownload && AutoUpdate)
            {
                using var sha256 = SHA256.Create();
                string Hash(byte[] b) => BitConverter.ToString(sha256.ComputeHash(b)).Replace("-", "").ToLowerInvariant();

                byte[] localBytes = await File.ReadAllBytesAsync(fullPath);
                needDownload = Hash(localBytes) != Hash(remoteBytes);
            }

            if (needDownload)
            {
                await File.WriteAllBytesAsync(fullPath, remoteBytes);
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadFromGitHub Error: {ex.Message}");
        }
    }



    
    public static void CheckForReveal()
    {
        if (IsWarmup()) return;

        var g_Main = MainPlugin.Instance.g_Main;
        int mode = Configs.Instance.RevealLastPlayerOnTeam;

        if ((mode == 1 || mode == 3) && g_Main.Timer_CT == null)
        {
            var aliveCTs = GetPlayersController(true, false, false, false)
                .Where(p => p.IsValid(true)
                        && p.TeamNum == (byte)CsTeam.CounterTerrorist
                        && p.IsAlive())
                .ToList();

            if (aliveCTs.Count == 1)
            {
                var lastCT = aliveCTs[0];
                g_Main.Timer_CT = MainPlugin.Instance.AddTimer(
                    1.0f,
                    () => Start_Reveal(lastCT, isCT: true),
                    TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
            }
        }

        if ((mode == 2 || mode == 3) && g_Main.Timer_T == null)
        {
            var aliveTs = GetPlayersController(true, false, false, false)
                .Where(p => p.IsValid(true)
                        && p.TeamNum == (byte)CsTeam.Terrorist
                        && p.IsAlive())
                .ToList();

            if (aliveTs.Count == 1)
            {
                var lastT = aliveTs[0];
                g_Main.Timer_T = MainPlugin.Instance.AddTimer(
                    1.0f,
                    () => Start_Reveal(lastT, isCT: false),
                    TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
            }
        }
    }

    public static void Start_Reveal(CCSPlayerController player, bool isCT)
    {
        var g_Main = MainPlugin.Instance.g_Main;

        int aliveCount = GetPlayersController(true, false, false, false)
            .Count(p => p.IsValid(true)
                    && p.TeamNum == (byte)(isCT ? CsTeam.CounterTerrorist : CsTeam.Terrorist)
                    && p.IsAlive());

        if (aliveCount != 1)
        {
            if (isCT)
            {
                g_Main.Timer_CT?.Kill();
                g_Main.Timer_CT = null!;
                g_Main.Announced_CT = false;

                foreach (var key in g_Main.Player_Data_CT.Keys.ToList())
                {
                    var d = g_Main.Player_Data_CT[key];
                    if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                    if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                    if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                    if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                    g_Main.Player_Data_CT.Remove(key);
                }
            }
            else
            {
                g_Main.Timer_T?.Kill();
                g_Main.Timer_T = null!;
                g_Main.Announced_T = false;

                foreach (var key in g_Main.Player_Data_T.Keys.ToList())
                {
                    var d = g_Main.Player_Data_T[key];
                    if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                    if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                    if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                    if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                    g_Main.Player_Data_T.Remove(key);
                }
            }
            return;
        }

        if (!player.IsValid(true) || !player.IsAlive()) return;

        if (Configs.Instance.Chicken_Enable)
        {
            SetGlowChicken(player);
        }

        if (Configs.Instance.Player_Enable)
        {
            SetGlowPlayer(player);
        }

        if (!string.IsNullOrEmpty(Configs.Instance.Play_Sound))
        {
            float effectiveVolume = Configs.Instance.Sound_Volume.ToEffectiveVolume();
            if (effectiveVolume > 0)
            {
                foreach (var players in GetPlayersController(false, false, false, false))
                {
                    if (!players.IsValid()
                    || players.PlayerPawn == null || !players.PlayerPawn.IsValid
                    || players.PlayerPawn.Value == null || !players.PlayerPawn.Value.IsValid) continue;

                    if (Configs.Instance.Play_Sound.StartsWith("sounds/"))
                    {
                        players.ExecuteClientCommand($"play {Configs.Instance.Play_Sound}");
                    }
                    else
                    {
                        RecipientFilter playersfilter = [players];
                        players.PlayerPawn.Value.EmitSound(Configs.Instance.Play_Sound, playersfilter, effectiveVolume);
                    }
                }
            }
            
        }

        if (isCT && !g_Main.Announced_CT)
        {
            AdvancedServerPrintToChatAll(MainPlugin.Instance.Localizer["PrintToChatToAll.LastPlayer.Alive"], player.PlayerName);
            g_Main.Announced_CT = true;
        }
        else if (!isCT && !g_Main.Announced_T)
        {
            AdvancedServerPrintToChatAll(MainPlugin.Instance.Localizer["PrintToChatToAll.LastPlayer.Alive"], player.PlayerName);
            g_Main.Announced_T = true;
        }
    }

    public static void StopRevealIfNeeded()
    {
        var g_Main = MainPlugin.Instance.g_Main;
        int mode = Configs.Instance.RevealLastPlayerOnTeam;

        if (g_Main.Timer_CT != null && (mode == 1 || mode == 3))
        {
            int aliveCTCount = GetPlayersController(true, false, false, false)
                .Count(p => p.IsValid(true)
                        && p.TeamNum == (byte)CsTeam.CounterTerrorist
                        && p.IsAlive());

            if (aliveCTCount != 1)
            {
                g_Main.Timer_CT?.Kill();
                g_Main.Timer_CT = null!;
                g_Main.Announced_CT = false;

                foreach (var key in g_Main.Player_Data_CT.Keys.ToList())
                {
                    var d = g_Main.Player_Data_CT[key];
                    if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                    if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                    if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                    if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                    g_Main.Player_Data_CT.Remove(key);
                }
            }
        }

        if (g_Main.Timer_T != null && (mode == 2 || mode == 3))
        {
            int aliveTCount = GetPlayersController(true, false, false, false)
                .Count(p => p.IsValid(true)
                        && p.TeamNum == (byte)CsTeam.Terrorist
                        && p.IsAlive());

            if (aliveTCount != 1)
            {
                g_Main.Timer_T?.Kill();
                g_Main.Timer_T = null!;
                g_Main.Announced_T = false;

                foreach (var key in g_Main.Player_Data_T.Keys.ToList())
                {
                    var d = g_Main.Player_Data_T[key];
                    if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                    if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                    if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                    if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                    g_Main.Player_Data_T.Remove(key);
                }
            }
        }
    }

    public static System.Numerics.Vector3 CalculateBehindPosition(CCSPlayerPawn pawn)
    {
        var basePos = pawn.AbsOrigin;
        float x = basePos!.X;
        float y = basePos.Y;
        float z = basePos.Z + 30.0f;

        float eyeAngleY = pawn.EyeAngles.Y;

        float angleInRadians = (eyeAngleY + 180) * (MathF.PI / 180);
        const float spawnOffset = 60.0f;

        float spawnX = x + spawnOffset * MathF.Cos(angleInRadians);
        float spawnY = y + spawnOffset * MathF.Sin(angleInRadians);

        return new System.Numerics.Vector3(
            spawnX,
            spawnY,
            z + 30.0f
        );
    }

    public static void SetGlowChicken(CCSPlayerController player)
    {
        if (!player.IsValid(true) || player.PlayerPawn.Value == null) return;

        var g_Main = MainPlugin.Instance.g_Main;
        int mode = Configs.Instance.RevealLastPlayerOnTeam;

        bool isCT = player.TeamNum == (byte)CsTeam.CounterTerrorist && (mode == 1 || mode == 3);
        bool isT  = player.TeamNum == (byte)CsTeam.Terrorist        && (mode == 2 || mode == 3);
        if (!isCT && !isT) return;

        int slot = player.Slot;
        var spawnPosition = CalculateBehindPosition(player.PlayerPawn.Value);

        if (isCT)
        {
            foreach (var key in g_Main.Player_Data_CT.Keys.Where(k => k != slot).ToList())
            {
                var d = g_Main.Player_Data_CT[key];
                if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                g_Main.Player_Data_CT.Remove(key);
            }

            if (g_Main.Player_Data_CT.TryGetValue(slot, out var dct) && dct.ChickenGlow != null && dct.ChickenGlow.IsValid)
            {
                dct.ChickenRelay.Teleport(spawnPosition);
                dct.ChickenGlow.Teleport(spawnPosition);
                return;
            }
        }

        if (isT)
        {
            foreach (var key in g_Main.Player_Data_T.Keys.Where(k => k != slot).ToList())
            {
                var d = g_Main.Player_Data_T[key];
                if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                g_Main.Player_Data_T.Remove(key);
            }

            if (g_Main.Player_Data_T.TryGetValue(slot, out var dt) && dt.ChickenGlow != null && dt.ChickenGlow.IsValid)
            {
                dt.ChickenRelay.Teleport(spawnPosition);
                dt.ChickenGlow.Teleport(spawnPosition);
                return;
            }
        }

        var chicken = Utilities.CreateEntityByName<CChicken>("chicken");
        if (chicken == null) return;
        chicken.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(1u << 2);
        //chicken.SetModel("models/chicken/chicken.vmdl");
        chicken.DispatchSpawn();
        chicken.Spawnflags = 256U;
        chicken.RenderMode = RenderMode_t.kRenderNone;
        chicken.Teleport(spawnPosition);
        chicken.AcceptInput("DisableCollision");

        var chickenGlow = Utilities.CreateEntityByName<CChicken>("chicken");
        if (chickenGlow == null) return;
        chickenGlow.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(1u << 2);
        //chickenGlow.SetModel("models/chicken/chicken.vmdl");
        chickenGlow.DispatchSpawn();
        chickenGlow.Glow.GlowColorOverride = Configs.Instance.Chicken_GlowColor.ToColor();
        chickenGlow.Glow.GlowRange         = Configs.Instance.Chicken_GlowRange;
        chickenGlow.Glow.GlowTeam          = -1;
        chickenGlow.Glow.GlowType          = Configs.Instance.Chicken_GlowType ? 2 : 3;
        chickenGlow.Glow.GlowRangeMin      = 100;
        chickenGlow.Teleport(spawnPosition);
        chickenGlow.AcceptInput("DisableCollision");
        chickenGlow.AcceptInput("SetScale", null, null, Configs.Instance.Chicken_Size.ToString());
        chickenGlow.AcceptInput("SetParent", caller: chickenGlow, activator: chicken, value: "!activator");
        chicken.AcceptInput("SetParent", caller: chicken, activator: player.PlayerPawn.Value, value: "!activator");

        if (isCT)
        {
            if (g_Main.Player_Data_CT.TryGetValue(slot, out var d))
            {
                d.ChickenRelay = chicken;
                d.ChickenGlow  = chickenGlow;
            }
            else
            {
                g_Main.Player_Data_CT[slot] = new Globals.PlayerDataClass_CT(player, "", null!, null!, chicken, chickenGlow);
            }
        }
        else
        {
            if (g_Main.Player_Data_T.TryGetValue(slot, out var d))
            {
                d.ChickenRelay = chicken;
                d.ChickenGlow  = chickenGlow;
            }
            else
            {
                g_Main.Player_Data_T[slot] = new Globals.PlayerDataClass_T(player, "", null!, null!, chicken, chickenGlow);
            }
        }
    }


    public static void SetGlowPlayer(CCSPlayerController player)
    {
        if (!player.IsValid(true) || player.PlayerPawn.Value == null) return;

        var g_Main = MainPlugin.Instance.g_Main;
        int mode = Configs.Instance.RevealLastPlayerOnTeam;

        bool isCT = player.TeamNum == (byte)CsTeam.CounterTerrorist && (mode == 1 || mode == 3);
        bool isT  = player.TeamNum == (byte)CsTeam.Terrorist        && (mode == 2 || mode == 3);
        if (!isCT && !isT) return;

        int slot = player.Slot;
        string modelName = player.PlayerPawn.Value.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;

        if (isCT)
        {
            foreach (var key in g_Main.Player_Data_CT.Keys.Where(k => k != slot).ToList())
            {
                var d = g_Main.Player_Data_CT[key];
                if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                g_Main.Player_Data_CT.Remove(key);
            }

            if (g_Main.Player_Data_CT.TryGetValue(slot, out var dct) && dct.ModelGlow != null && dct.ModelGlow.IsValid)
            {
                if (dct.ModelName != modelName)
                {
                    if (dct.ModelRelay != null && dct.ModelRelay.IsValid) dct.ModelRelay.Remove();
                    if (dct.ModelGlow  != null && dct.ModelGlow .IsValid) dct.ModelGlow.Remove();
                    dct.ModelRelay = null!;
                    dct.ModelGlow  = null!;
                }
                else return;
            }
        }

        if (isT)
        {
            foreach (var key in g_Main.Player_Data_T.Keys.Where(k => k != slot).ToList())
            {
                var d = g_Main.Player_Data_T[key];
                if (d.ChickenRelay != null && d.ChickenRelay.IsValid) d.ChickenRelay.Remove();
                if (d.ChickenGlow  != null && d.ChickenGlow .IsValid) d.ChickenGlow.Remove();
                if (d.ModelRelay   != null && d.ModelRelay  .IsValid) d.ModelRelay.Remove();
                if (d.ModelGlow    != null && d.ModelGlow   .IsValid) d.ModelGlow.Remove();
                g_Main.Player_Data_T.Remove(key);
            }

            if (g_Main.Player_Data_T.TryGetValue(slot, out var dt) && dt.ModelGlow != null && dt.ModelGlow.IsValid)
            {
                if (dt.ModelName != modelName)
                {
                    if (dt.ModelRelay != null && dt.ModelRelay.IsValid) dt.ModelRelay.Remove();
                    if (dt.ModelGlow  != null && dt.ModelGlow .IsValid) dt.ModelGlow.Remove();
                    dt.ModelRelay = null!;
                    dt.ModelGlow  = null!;
                }
                else return;
            }
        }

        var modelRelay = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
        if (modelRelay == null) return;
        modelRelay.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(1u << 2);
        modelRelay.SetModel(modelName);
        modelRelay.DispatchSpawn();
        modelRelay.Spawnflags = 256u;
        modelRelay.RenderMode = RenderMode_t.kRenderNone;

        var modelGlow = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
        if (modelGlow == null) return;
        modelGlow.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(1u << 2);
        modelGlow.Render = Color.FromArgb(1, 0, 0, 0);
        modelGlow.SetModel(modelName);
        modelGlow.DispatchSpawn();
        modelGlow.Glow.GlowColorOverride = Configs.Instance.Player_GlowColor.ToColor();
        modelGlow.Glow.GlowRange         = Configs.Instance.Player_GlowRange;
        modelGlow.Glow.GlowTeam          = -1;
        modelGlow.Glow.GlowType          = Configs.Instance.Player_GlowType ? 2 : 3;
        modelGlow.Glow.GlowRangeMin      = 100;

        modelRelay.AcceptInput("FollowEntity", player.PlayerPawn.Value, modelRelay, "!activator");
        modelGlow.AcceptInput("FollowEntity", modelRelay, modelGlow, "!activator");

        if (isCT)
        {
            if (g_Main.Player_Data_CT.TryGetValue(slot, out var d))
            {
                d.ModelName  = modelName;
                d.ModelRelay = modelRelay;
                d.ModelGlow  = modelGlow;
            }
            else
            {
                g_Main.Player_Data_CT[slot] = new Globals.PlayerDataClass_CT(player, modelName, modelRelay, modelGlow, null!, null!);
            }
        }
        else
        {
            if (g_Main.Player_Data_T.TryGetValue(slot, out var d))
            {
                d.ModelName  = modelName;
                d.ModelRelay = modelRelay;
                d.ModelGlow  = modelGlow;
            }
            else
            {
                g_Main.Player_Data_T[slot] = new Globals.PlayerDataClass_T(player, modelName, modelRelay, modelGlow, null!, null!);
            }
        }
    }
}