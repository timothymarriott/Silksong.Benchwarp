
using System.Linq;
using UnityEngine;
using static Benchwarp.Benchwarp;

namespace Benchwarp
{
    internal static class Hooks
    {
        internal static void Hook()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += BenchMaker.TryToDeploy;
            On.PlayerData.SetBool += BenchWatcher;
            On.PlayerData.GetString += RespawnAtDeployedBench;
            On.PlayerData.SetString += RemoveRespawnFromDeployedBench;
            // Imagine if GetPlayerIntHook actually worked
            On.GameManager.OnNextLevelReady += FixRespawnType;
            On.PlayMakerFSM.OnEnable += OnEnableBenchFsm;
            Events.OnBenchwarp += ChangeScene.OnBenchwarpCleanup;
        }

        internal static void Unhook()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= BenchMaker.TryToDeploy;
            On.PlayerData.SetBool -= BenchWatcher;
            On.PlayerData.GetString -= RespawnAtDeployedBench;
            On.PlayerData.SetString -= RemoveRespawnFromDeployedBench;
            On.GameManager.OnNextLevelReady -= FixRespawnType;
            
            On.PlayMakerFSM.OnEnable -= OnEnableBenchFsm;
            Events.OnBenchwarp -= ChangeScene.OnBenchwarpCleanup;
        }


        public static void BenchWatcher(On.PlayerData.orig_SetBool orig, PlayerData self, string target, bool val)
        {
            orig(self, target, val);
            if (target == nameof(PlayerData.atBench) && val)
            {
                foreach (Bench bench in Bench.Benches)
                {
                    if (bench.IsLocked() || !bench.AtBench()) continue;

                    bench.SetVisited(true);
                    break;
                }
            }
        }

        private static void RemoveRespawnFromDeployedBench(On.PlayerData.orig_SetString orig, PlayerData self, string stringName, string value)
        {
            orig(self, stringName, value);
            switch (stringName)
            {
                case nameof(PlayerData.respawnMarkerName):
                    if (value != BenchMaker.DEPLOYED_BENCH_RESPAWN_MARKER_NAME)
                    {
                        LS.atDeployedBench = false;
                    }
                    break;
                case nameof(PlayerData.respawnScene):
                    if (value != LS.benchScene)
                    {
                        LS.atDeployedBench = false;
                    }
                    break;
            }
        }

        private static string RespawnAtDeployedBench(On.PlayerData.orig_GetString orig, PlayerData self,
            string stringName)
        {
            string value = orig(self, stringName);
            return !LS.atDeployedBench
                ? value
                : stringName switch
                {
                    nameof(PlayerData.respawnMarkerName) => BenchMaker.DEPLOYED_BENCH_RESPAWN_MARKER_NAME,
                    nameof(PlayerData.respawnScene) => LS.benchScene,
                    _ => value,
                };
        }

        private static void FixRespawnType(On.GameManager.orig_OnNextLevelReady orig, GameManager self)
        {
            if (GameManager.instance.RespawningHero)
            {
                Transform spawnPoint = HeroController.instance.LocateSpawnPoint();
                if (spawnPoint != null && spawnPoint.gameObject != null
                    && spawnPoint.gameObject.GetComponents<PlayMakerFSM>().Any(fsm => fsm.FsmName == "Bench Control"))
                {
                    PlayerData.instance.respawnType = 1;
                }
                else
                {
                    PlayerData.instance.respawnType = 0;
                }
            }

            orig(self);
        }

        private static void OnEnableBenchFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);
            if (fsm.FsmName == "Bench Control")
            {
                FixAreaTitleBug(fsm);
                if (GS.ModifyVanillaBenchStyles) StyleOverride(fsm);
            }
        }

        private static void FixAreaTitleBug(PlayMakerFSM fsm)
        {
            if (fsm.Fsm.GlobalTransitions?.Where(t => t.EventName != "BIG TITLE START")?.ToArray() is HutongGames.PlayMaker.FsmTransition[] ts)
            {
                fsm.Fsm.GlobalTransitions = ts;
            }
        }

        private static void StyleOverride(PlayMakerFSM fsm)
        {
            if (fsm.gameObject == BenchMaker.DeployedBench) return;
            switch (fsm.gameObject.scene.name)
            {
                // benches that are too much trouble to implement

                case "Ruins1_02": // mostly works, but the bench sprite is part of Quirrel
                case "Deepnest_East_13": // camp
                case "Fungus1_24": // qg cornifer
                case "Mines_18": // cg2

                // Tolls work, but only after a scene change
                case "Fungus3_50":
                case "Ruins1_31":
                case "Abyss_18":
                    return;
            }


            GameObject benchGO = fsm.gameObject;
            Bench bench = Bench.Benches.FirstOrDefault(b => b.sceneName == benchGO.scene.name);
            if (bench == null) return;

            if (!BenchStyle.IsValidStyle(bench.style) || !BenchStyle.IsValidStyle(GS.nearStyle) || !BenchStyle.IsValidStyle(GS.farStyle)) return;

            BenchStyle origStyle = BenchStyle.GetStyle(bench.style);
            BenchStyle nearStyle = BenchStyle.GetStyle(GS.nearStyle);
            BenchStyle farStyle = BenchStyle.GetStyle(GS.farStyle);

            Vector3 position = benchGO.transform.position - bench.specificOffset;
            nearStyle.ApplyFsmAndPositionChanges(benchGO, position);
            nearStyle.ApplyLitSprite(benchGO);
            farStyle.ApplyDefaultSprite(benchGO);
            Object.Destroy(benchGO.GetComponent<RestBenchTilt>());
        }
    }
}
