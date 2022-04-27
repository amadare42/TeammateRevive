using System;
using System.Collections;
using System.Linq;
using BepInEx;
using MonoMod.RuntimeDetour;
using R2API;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using TeammateRevive;
using TeammateRevive.Configuration;
using TeammateRevive.Logging;
using TeammateRevive.ProgressBar;
using TeammateRevive.Resources;
using UnityEngine;

namespace TeammateRevival
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(MainTeammateRevival.PluginGUID, MainTeammateRevival.PluginName, MainTeammateRevival.PluginVersion)]
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(NetworkingAPI), nameof(ItemAPI), nameof(LanguageAPI))]
    public class TeammateRevivalPlayground : BaseUnityPlugin
    {
        private ProgressBarController controller;
        private DetourModManager detourModManager;
        private PluginConfig pluginConfig;

        private void Awake()
        {
            detourModManager = new DetourModManager();
            
            this.pluginConfig = PluginConfig.Load(this.Config);
            Log.Init(this.pluginConfig, this.Logger);
            CustomResources.LoadCustomResources();
            this.controller = new ProgressBarController();

            if (Stage.instance)
            {
                this.controller.AttachProgressBar(RoR2.UI.HUD.instancesList.First());
                this.controller.Show();
            }
            else
            {
                On.RoR2.UI.HUD.OnEnable +=  (orig, self) =>
                {
                    try
                    {
                        orig(self);
                        StartCoroutine(ShowAfterDelay());
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                };
            }
            
            // Plugin startup logic
            Logger.LogInfo($"Plugin TeammateRevivalPlayground is loaded!");
        }

        private IEnumerator ShowAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            this.controller.AttachProgressBar(RoR2.UI.HUD.instancesList.First());
            this.controller.Show();
        }

        private void OnDestroy()
        {
            this.controller.Destroy();
            this.detourModManager.Unload(typeof(TeammateRevivalPlayground).Assembly);
        }
    }
}