using CounterStrikeSharp.API.Core;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Drawing;
using CounterStrikeSharp.API;


namespace Reveal_Last_Alive;

public static class Extension
{
    public static bool IsValid([NotNullWhen(true)] this CCSPlayerController? player, bool IncludeBots = false, bool IncludeHLTV = false)
    {
        if (player == null || !player.IsValid)
            return false;

        if (!IncludeBots && player.IsBot)
            return false;

        if (!IncludeHLTV && player.IsHLTV)
            return false;

        return true;
    }
    public static bool IsAlive(this CCSPlayerController p)
    {
        var pawn = p.PlayerPawn?.Value;
        return pawn is { IsValid: true, LifeState: (byte)LifeState_t.LIFE_ALIVE };
    }

    public static void RemoveGlowAndChicken(this CCSPlayerController? player)
    {
        if (player == null || !player.IsValid) return;

        var g_Main = MainPlugin.Instance.g_Main;
        int slot = player.Slot;

        if (g_Main.Player_Data_CT.TryGetValue(slot, out var dct))
        {
            if (dct.ChickenRelay != null && dct.ChickenRelay.IsValid) dct.ChickenRelay.Remove();
            if (dct.ChickenGlow  != null && dct.ChickenGlow .IsValid) dct.ChickenGlow.Remove();
            if (dct.ModelRelay   != null && dct.ModelRelay  .IsValid) dct.ModelRelay.Remove();
            if (dct.ModelGlow    != null && dct.ModelGlow   .IsValid) dct.ModelGlow.Remove();
            g_Main.Player_Data_CT.Remove(slot);
        }

        if (g_Main.Player_Data_T.TryGetValue(slot, out var dt))
        {
            if (dt.ChickenRelay != null && dt.ChickenRelay.IsValid) dt.ChickenRelay.Remove();
            if (dt.ChickenGlow  != null && dt.ChickenGlow .IsValid) dt.ChickenGlow.Remove();
            if (dt.ModelRelay   != null && dt.ModelRelay  .IsValid) dt.ModelRelay.Remove();
            if (dt.ModelGlow    != null && dt.ModelGlow   .IsValid) dt.ModelGlow.Remove();
            g_Main.Player_Data_T.Remove(slot);
        }
    }

    public static float ToEffectiveVolume(this string FirstVol, string SecVol = null!)
    {
        float Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 1f;

            input = input.Replace("%", "").Trim();

            if (!float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return 1f;
            }

            return result / 100f;
        }

        float effectiveVolume = Parse(FirstVol);

        if (!string.IsNullOrWhiteSpace(SecVol))
        {
            effectiveVolume *= Parse(SecVol);
        }

        return effectiveVolume > 0f ? Math.Clamp(effectiveVolume, 0.01f, 1.0f) : 0f;
    }

    public static Color ToColor(this string colorString)
    {
        if (string.IsNullOrWhiteSpace(colorString))
        {
            Helper.DebugMessage("Color string cannot be empty or whitespace.");
            return Color.Transparent;
        }

        var components = colorString
            .Replace(',', ' ')
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (components.Length != 3 && components.Length != 4)
        {
            Helper.DebugMessage("Invalid color format. Expected 3 or 4 components.");
            return Color.Transparent;
        }

        if (!int.TryParse(components[0], out int r) || r < 0 || r > 255)
        {
            Helper.DebugMessage("Invalid Red component value. Must be 0-255.");
            return Color.Transparent;
        }
        if (!int.TryParse(components[1], out int g) || g < 0 || g > 255)
        {
            Helper.DebugMessage("Invalid Green component value. Must be 0-255.");
            return Color.Transparent;
        }
        if (!int.TryParse(components[2], out int b) || b < 0 || b > 255)
        {
            Helper.DebugMessage("Invalid Blue component value. Must be 0-255.");
            return Color.Transparent;
        }

        byte a = 255;
        if (components.Length == 4)
        {
            string alphaComponent = components[3];
            if (alphaComponent.Contains('.'))
            {
                if (!double.TryParse(alphaComponent, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double alphaDouble))
                {
                    Helper.DebugMessage("Invalid alpha format. Must be 0-255 or 0.0-1.0.");
                    return Color.Transparent;
                }
                if (alphaDouble < 0 || alphaDouble > 1)
                {
                    Helper.DebugMessage("Alpha value must be between 0.0 and 1.0 when using decimal format.");
                    return Color.Transparent;
                }
                a = (byte)(alphaDouble * 255);
            }
            else
            {
                if (!int.TryParse(alphaComponent, out int alphaInt) || alphaInt < 0 || alphaInt > 255)
                {
                    Helper.DebugMessage("Invalid alpha value. Must be 0-255 or 0.0-1.0.");
                    return Color.Transparent;
                }
                a = (byte)alphaInt;
            }
        }
        return Color.FromArgb(a, r, g, b);
    }

    private const ulong Steam64Offset = 76561197960265728UL;
    public static (string steam2, string steam3, string steam32, string steam64) GetPlayerSteamID(this ulong steamId64)
    {
        uint id32 = (uint)(steamId64 - Steam64Offset);
        var steam32 = id32.ToString();
        uint y = id32 & 1;
        uint z = id32 >> 1;
        var steam2 = $"STEAM_0:{y}:{z}";
        var steam3 = $"[U:1:{steam32}]";
        var steam64 = steamId64.ToString();
        return (steam2, steam3, steam32, steam64);
    }

    public static string[]? ConvertCommands(this string input, bool EventPlayerChat = false)
    {
        var parts = input.Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split(':', 2))
            .ToDictionary(
                p => p[0].Trim(),
                p => p.Length > 1
                    ? p[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrEmpty(c))
                    : Enumerable.Empty<string>()
            );

        if (!parts.Values.Any(v => v.Any())) return null;

        if (!EventPlayerChat)
        {
            return parts.FirstOrDefault().Value?.Select(c =>
            {
                if (c.StartsWith("!"))
                {
                    var cmd = c.TrimStart('!');
                    return cmd.StartsWith("css_") ? cmd : "css_" + cmd;
                }
                return c;
            }).Distinct().ToArray();
        }

        var first = parts.FirstOrDefault().Value?
            .Select(c =>
            {
                var cmd = c.TrimStart('!');
                if (cmd.StartsWith("css_"))
                    cmd = cmd.Substring(4);
                return "!" + cmd;
            }) ?? Enumerable.Empty<string>();

        var rest = parts.Skip(1).SelectMany(p => p.Value);
        var result = first.Concat(rest).Distinct().ToArray();

        return result.Length == 0 ? null : result;
    }

    public static bool HasValidPermissionData(this string? groups)
    {
        if (string.IsNullOrWhiteSpace(groups)) return false;

        var segments = groups.Split('|', StringSplitOptions.RemoveEmptyEntries);
        foreach (var seg in segments)
        {
            var trimmed = seg.Trim();
            if (string.IsNullOrEmpty(trimmed))
                continue;

            int colonIndex = trimmed.IndexOf(':');
            if (colonIndex == -1 || colonIndex == 0)
                continue;

            string values = trimmed.Substring(colonIndex + 1).Trim();
            if (!string.IsNullOrEmpty(values))
                return true;
        }

        return false;
    }
}