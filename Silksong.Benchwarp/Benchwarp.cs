using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Benchwarp
{
    [BepInAutoPlugin(id: "com.lem00ns.silksong_benchwarp")]
    public partial class Benchwarp : BaseUnityPlugin
    {
        public static ManualLogSource log;
        internal static Benchwarp instance;

        public static GlobalSettings GS { get; private set; } = new();
        public static SaveSettings LS { get; private set; } = new();

        private void Awake()
        {
            instance = this;
            log = BepInEx.Logging.Logger.CreateLogSource("Benchwarp");
            On.SteamOnlineSubsystem.ctor += (orig, self, platform) =>
            {
                return;
            };
        }

        private void Start()
        {
            Hooks.Hook();

            GUIController.Setup();
            GUIController.Instance.BuildMenus();

            if (LS.benchDeployed && GameManager.instance.sceneName == LS.benchScene)
            {
                BenchMaker.MakeDeployedBench(); // Since the mod could be reenabled in any scene
            }

            if (GS.LegacyHotkeys)
            {
                Hotkeys.ApplyLegacyHotkeys();
            }

            Hotkeys.RefreshHotkeys();
        }


        public void Unload()
        {
            Hooks.Unhook();
            GUIController.Unload();
            BenchMaker.DestroyBench(DontDeleteData: true);
            Hotkeys.RemoveLegacyHotkeys();
        }

        
        new public void SaveGlobalSettings()
        {
            try
            {
                File.WriteAllText(Application.persistentDataPath + "/benchwarp_config.json", JsonConvert.SerializeObject(GS, Formatting.Indented, new JsonSerializerSettings(){}));
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        public void OnLoadGlobal(GlobalSettings s)
        {
            GS = s ?? GS ?? new();
        }

        public GlobalSettings OnSaveGlobal()
        {
            return GS;
        }

        public void OnLoadLocal(SaveSettings s)
        {
            LS = s ?? new();
        }

        public SaveSettings OnSaveLocal()
        {
            return LS;
        }


    }
}
