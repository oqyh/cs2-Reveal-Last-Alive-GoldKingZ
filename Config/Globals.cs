using CounterStrikeSharp.API.Core;

namespace Reveal_Last_Alive;

public class Globals
{
    public bool Downloading_FromGithub = false;
    public bool Announced_CT = false;
    public bool Announced_T  = false;
    public CounterStrikeSharp.API.Modules.Timers.Timer Timer_CT = null!;
    public CounterStrikeSharp.API.Modules.Timers.Timer Timer_T  = null!;
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public DateTime EventPlayerChat { get; set; }
        public PlayerDataClass(CCSPlayerController Playerr, DateTime EventPlayerChatt)
        {
            Player = Playerr;
            EventPlayerChat = EventPlayerChatt;
        }
    }
    public Dictionary<int, PlayerDataClass> Player_Data = new Dictionary<int, PlayerDataClass>();

    public class PlayerDataClass_CT
    {
        public CCSPlayerController Player { get; set; }
        public string ModelName  { get; set; }
        public CDynamicProp ModelRelay { get; set; }
        public CDynamicProp ModelGlow { get; set; }
        public CDynamicProp ChickenRelay { get; set; }
        public CDynamicProp ChickenGlow { get; set; }

        public PlayerDataClass_CT(CCSPlayerController Playerr, string ModelNamee, CDynamicProp ModelRelayy, CDynamicProp ModelGloww, CDynamicProp ChickenRelayy, CDynamicProp ChickenGloww)
        {
            Player = Playerr;
            ModelName = ModelNamee;
            ModelRelay = ModelRelayy;
            ModelGlow = ModelGloww;
            ChickenRelay = ChickenRelayy;
            ChickenGlow = ChickenGloww;
        }
    }
    public Dictionary<int, PlayerDataClass_CT> Player_Data_CT = new Dictionary<int, PlayerDataClass_CT>();

    public class PlayerDataClass_T
    {
        public CCSPlayerController Player { get; set; }
        public string ModelName  { get; set; }
        public CDynamicProp ModelRelay { get; set; }
        public CDynamicProp ModelGlow { get; set; }
        public CDynamicProp ChickenRelay { get; set; }
        public CDynamicProp ChickenGlow { get; set; }

        public PlayerDataClass_T(CCSPlayerController Playerr, string ModelNamee, CDynamicProp ModelRelayy, CDynamicProp ModelGloww, CDynamicProp ChickenRelayy, CDynamicProp ChickenGloww)
        {
            Player = Playerr;
            ModelName = ModelNamee;
            ModelRelay = ModelRelayy;
            ModelGlow = ModelGloww;
            ChickenRelay = ChickenRelayy;
            ChickenGlow = ChickenGloww;
        }
    }
    public Dictionary<int, PlayerDataClass_T> Player_Data_T = new Dictionary<int, PlayerDataClass_T>();

    public void Clear()
    {
        foreach (var data_ct in Player_Data_CT.Values)
        {
            if (data_ct.ChickenRelay != null && data_ct.ChickenRelay.IsValid)
            {
                data_ct.ChickenRelay.Remove();
                data_ct.ChickenRelay = null!;
            }
            if (data_ct.ChickenGlow != null && data_ct.ChickenGlow.IsValid)
            {
                data_ct.ChickenGlow.Remove();
                data_ct.ChickenGlow = null!;
            }

            if (data_ct.ModelRelay != null && data_ct.ModelRelay.IsValid)
            {
                data_ct.ModelRelay.Remove();
                data_ct.ModelRelay = null!;
            }
            if (data_ct.ModelGlow != null && data_ct.ModelGlow.IsValid)
            {
                data_ct.ModelGlow.Remove();
                data_ct.ModelGlow = null!;
            }
        }

        foreach (var data_t in Player_Data_T.Values)
        {
            if (data_t.ChickenRelay != null && data_t.ChickenRelay.IsValid)
            {
                data_t.ChickenRelay.Remove();
                data_t.ChickenRelay = null!;
            }
            if (data_t.ChickenGlow != null && data_t.ChickenGlow.IsValid)
            {
                data_t.ChickenGlow.Remove();
                data_t.ChickenGlow = null!;
            }

            if (data_t.ModelRelay != null && data_t.ModelRelay.IsValid)
            {
                data_t.ModelRelay.Remove();
                data_t.ModelRelay = null!;
            }
            if (data_t.ModelGlow != null && data_t.ModelGlow.IsValid)
            {
                data_t.ModelGlow.Remove();
                data_t.ModelGlow = null!;
            }
        }

        Player_Data?.Clear();
        Player_Data_CT?.Clear();
        Player_Data_T?.Clear();

        Timer_CT?.Kill();
        Timer_CT = null!;

        Timer_T?.Kill();
        Timer_T = null!;

        Announced_CT = false;
        Announced_T  = false;
    }
}