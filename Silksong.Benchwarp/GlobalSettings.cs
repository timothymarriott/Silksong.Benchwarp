using System.Collections.Generic;

namespace Benchwarp
{
    public class GlobalSettings
    {
        public bool ShowMenu = true;
        public bool WarpOnly = false;
        public bool UnlockAllBenches = true;
        public bool ShowScene = false;
        public int MaxSceneNames = 1;
        public bool SwapNames = false;
        public bool EnableDeploy = false;
        public bool DeployInUnsafeRooms = false;
        public bool AlwaysToggleAll = false;
        public bool ModifyVanillaBenchStyles = false;
        public string nearStyle = "Right";
        public string farStyle = "Right";
        public bool DeployCooldown = true;
        public bool Noninteractive = true;
        public bool NoMidAirDeploy = true;
        
        public bool NoPreload = true;
        public bool DoorWarp = false;
        public bool EnableHotkeys = false;
        public bool LegacyHotkeys = false;
        public bool OverrideLocalization = false;

        public Dictionary<string, string> HotkeyOverrides = new();
    }
}
