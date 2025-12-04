using HarmonyLib;
using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoNotIncluded.Patches
{
    [HarmonyPatch(typeof(DestinationSelectPanel))]
    [HarmonyPatch(nameof(DestinationSelectPanel.UpdateDisplayedClusters))]
    public static class UpdateDisplayedClusters_Patch
    {
        public static bool Prefix(DestinationSelectPanel __instance)
        {
            if (ArchipelagoNotIncluded.Options.CreateModList)
                return true;

            string cluster = string.Empty;
            if (ArchipelagoNotIncluded.info != null)
            {
                string planet = ArchipelagoNotIncluded.info.planet;
                if (DlcManager.IsExpansion1Active())
                {
                    if (ArchipelagoNotIncluded.ClassicPlanets.ContainsKey(planet))
                        cluster = ArchipelagoNotIncluded.ClassicPlanets[planet];
                    else if (ArchipelagoNotIncluded.SpacedOutPlanets.ContainsKey(planet))
                        cluster = ArchipelagoNotIncluded.SpacedOutPlanets[planet];
                    else if (ArchipelagoNotIncluded.ClassicLabPlanets.ContainsKey(planet))
                        cluster = ArchipelagoNotIncluded.ClassicLabPlanets[planet];
                }
                else
                {
                    if (ArchipelagoNotIncluded.BasePlanets.ContainsKey(planet))
                        cluster = ArchipelagoNotIncluded.BasePlanets[planet];
                    else if (ArchipelagoNotIncluded.BaseLabPlanets.ContainsKey(planet))
                        cluster = ArchipelagoNotIncluded.BaseLabPlanets[planet];
                }
                if (!cluster.IsNullOrWhiteSpace())
                {
                    __instance.clusterKeys.Clear();
                    __instance.clusterStartWorlds.Clear();
                    __instance.asteroidData.Clear();
                    __instance.clusterKeys.Add(cluster);
                    string layout = SettingsCache.clusterLayouts.clusterCache[cluster].GetStartWorld();
                    ColonyDestinationAsteroidBeltData value = new(layout, 0, cluster);
                    __instance.asteroidData[cluster] = value;
                    __instance.clusterStartWorlds.Add(cluster, layout);

                    // Prevent UI crashes caused by having 1 planet listed
                    __instance.dragTarget.onBeginDrag -= new System.Action(__instance.BeginDrag);
                    __instance.dragTarget.onDrag -= new System.Action(__instance.Drag);
                    __instance.dragTarget.onEndDrag -= new System.Action(__instance.EndDrag);
                    __instance.leftArrowButton.gameObject.SetActive(false);
                    __instance.rightArrowButton.gameObject.SetActive(false);
                }
            }
            //foreach (string clusterKey in SettingsCache.clusterLayouts.clusterCache.Keys)
            //    Debug.Log($"Cluster name: {clusterKey}, {Strings.Get(SettingsCache.clusterLayouts.clusterCache[clusterKey].name)}");
            /*foreach (string clusterName in SettingsCache.GetClusterNames())
                Debug.Log($"Cluster name: {clusterName}");
            foreach (string world in SettingsCache.GetWorldNames())
                Debug.Log($"World name: {world}");*/

            //Resets information used in DebugHelpers
            ArchipelagoNotIncluded.ItemList.Clear();
            ArchipelagoNotIncluded.ItemListDetailed.Clear();
            ArchipelagoNotIncluded.DebugWasUsed = false;
            ArchipelagoNotIncluded.lastIndexSaved = 0;
            ArchipelagoNotIncluded.runCount = 0;
            ArchipelagoNotIncluded.planetText = string.Empty;
            return cluster.IsNullOrWhiteSpace();
        }
    }

    [HarmonyPatch(typeof(ClusterCategorySelectionScreen))]
    [HarmonyPatch(nameof(ClusterCategorySelectionScreen.OnSpawn))]
    public static class Cluster_OnSpawn_Patch
    {
        public static void Postfix(ClusterCategorySelectionScreen __instance)
        {
            ArchipelagoNotIncluded.AllowResourceChecks = true;
            __instance.closeButton.onClick += new System.Action(DisallowResourceChecks);
            if (ArchipelagoNotIncluded.info == null)
                return;
            if (DlcManager.IsExpansion1Active())
            {
                if (ArchipelagoNotIncluded.ClassicPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                {
                    __instance.eventStyle.button.gameObject.SetActive(false);
                    __instance.spacedOutStyle.button.gameObject.SetActive(false);
                }
                else if (ArchipelagoNotIncluded.SpacedOutPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                {
                    __instance.classicStyle.button.gameObject.SetActive(false);
                    __instance.eventStyle.button.gameObject.SetActive(false);
                }
                else if (ArchipelagoNotIncluded.ClassicLabPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                {
                    __instance.classicStyle.button.gameObject.SetActive(false);
                    __instance.spacedOutStyle.button.gameObject.SetActive(false);
                }
            }
            else
            {
                if (ArchipelagoNotIncluded.BasePlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                {
                    __instance.eventStyle.button.gameObject.SetActive(false);
                }
                else if (ArchipelagoNotIncluded.BaseLabPlanets.ContainsKey(ArchipelagoNotIncluded.info.planet))
                {
                    __instance.vanillaStyle.button.gameObject.SetActive(false);
                }
            }
        }
        public static void DisallowResourceChecks()
        {
            ArchipelagoNotIncluded.AllowResourceChecks = false;
        }
    }

    [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
    [HarmonyPatch(nameof(ColonyDestinationSelectScreen.LaunchClicked))]
    public static class LaunchClicked_Patch
    {
        public static void Postfix()
        {
            APSeedInfo info = ArchipelagoNotIncluded.info;
            if (ArchipelagoNotIncluded.Options.CreateModList || info == null)
                return;
            if (info.teleporter)
                CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.Teleporters, "Enabled");

            CustomGameSettings cgs = CustomGameSettings.Instance;
            foreach (KeyValuePair<string, SettingConfig> keyValuePair in cgs.MixingSettings)    // DLC2_ID  DLC3_ID
            {
                DlcMixingSettingConfig dlcSetting = keyValuePair.Value as DlcMixingSettingConfig;
                if (dlcSetting != null)
                {
                    switch (dlcSetting.id)
                    {
                        case DlcManager.DLC2_ID:
                            if (!info.frosty)
                                cgs.SetMixingSetting(dlcSetting, "Disabled");
                            else
                                cgs.SetMixingSetting(dlcSetting, "Enabled");
                            break;
                        case DlcManager.DLC3_ID:
                            if (!info.bionic)
                                cgs.SetMixingSetting(dlcSetting, "Disabled");
                            else
                                cgs.SetMixingSetting(dlcSetting, "Enabled");
                            break;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch(nameof(MainMenu.OnSpawn))]
    public static class MainMenuOnSpawn_Patch
    {
        public static void Postfix()
        {
            ArchipelagoNotIncluded.netmon.ReadyForItems = false;
            ArchipelagoNotIncluded.lastItem = 0;
        }
    }

    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch(nameof(MainMenu.NewGame))]
    public static class NewGame_Patch
    {
        public static bool Prefix(MainMenu __instance)
        {
            var dialogue = ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas));
            string text = "No connection to Archipelago. New game will proceed without it.\n(If you're not trying to connect, just ignore this.)";
            string title = "Archipelago";
            System.Action confirm = null;
            System.Action cancel = new System.Action(() => __instance.FindOrAdd<NewGameFlow>().ClearCurrentScreen());
            if (ArchipelagoNotIncluded.netmon.session != null)
            {
                text = "Successfully connected to Archipelago.";
                cancel = null;
            }

            DlcManager.DlcInfo dlc2 = DlcManager.DLC_PACKS["DLC2_ID"];
            DlcManager.DlcInfo dlc3 = DlcManager.DLC_PACKS["DLC3_ID"];
            if (ArchipelagoNotIncluded.info?.frosty == true && !DlcManager.IsContentSubscribed(dlc2.id))
            {
                text = "\nFrosty DLC is enabled in Archipelago but not detected.\nPlease confirm DLC installation before trying to start a new game.";
                title = Strings.Get(dlc2.dlcTitle);
                confirm = new System.Action(cancel);

            }
            if (ArchipelagoNotIncluded.info?.bionic == true && !DlcManager.IsContentSubscribed(dlc3.id))
            {
                text = "\nBionic DLC is enabled in Archipelago but not detected.\nPlease confirm DLC installation before trying to start a new game.";
                title = Strings.Get(dlc3.dlcTitle);
                confirm = new System.Action(cancel);

            }
            dialogue.PopupConfirmDialog(text, confirm, cancel, title_text: title);
            return true;
        }
    }
}
