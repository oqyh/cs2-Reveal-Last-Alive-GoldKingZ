using System.Runtime.InteropServices;
using Reveal_Last_Alive.Config;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Memory;

namespace Reveal_Last_Alive;

public static class CustomHooks
{
    private static MemoryFunctionWithReturn<nint, nint, nint, uint, nint, uint, uint, byte>? CSoundOpGameSystem_SetSoundEventParamFunc_2;
    private static bool _isHooked = false;

    public static void Init(CustomGameData gameData)
    {
        if (_isHooked) return;

        try
        {
            
            CSoundOpGameSystem_SetSoundEventParamFunc_2 = gameData.CreateFunction<MemoryFunctionWithReturn<nint, nint, nint, uint, nint, uint, uint, byte>>("CSoundOpGameSystem_SetSoundEventParamFunc_2");
           
            if (CSoundOpGameSystem_SetSoundEventParamFunc_2 != null)
            {
                CSoundOpGameSystem_SetSoundEventParamFunc_2.Hook(OnCSoundOpGameSystem_SetSoundEventParamFunc_2, HookMode.Pre);
            }

            _isHooked = true;
            Helper.DebugMessage("All Hooks Started");
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Hook Init Error: {ex.Message}");
        }
    }

    public static void Cleanup()
    {
        if (!_isHooked) return;

        try
        {
            if (CSoundOpGameSystem_SetSoundEventParamFunc_2 != null)
            {
                CSoundOpGameSystem_SetSoundEventParamFunc_2.Unhook(OnCSoundOpGameSystem_SetSoundEventParamFunc_2, HookMode.Pre);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Hook Cleanup Error: {ex.Message}");
        }
        finally
        {
            CSoundOpGameSystem_SetSoundEventParamFunc_2 = null;
            _isHooked      = false;
            Helper.DebugMessage("Hooks Removed");
        }
    }

    public static HookResult OnCSoundOpGameSystem_SetSoundEventParamFunc_2(DynamicHook hook)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var hash = hook.GetParam<uint>(3);
            if (hash == 0x2D8464AF)
            {
                hook.SetParam(3, 0xBD6054E9);
            }
        }
        else
        {
            var hash = hook.GetParam<uint>(2);
            if (hash == 0x2D8464AF)
            {
                hook.SetParam(2, 0xBD6054E9);
            }
        }

        return HookResult.Continue;
    } 
}