using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ArchipelagoNotIncluded.Patches
{
    public static class APPatches
    {
         public static bool isModItem(string InternalName)
        {
            //Debug.Log($"Checking ModItem InternalName: {InternalName}");
            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == InternalName);
            ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.internal_name == InternalName);
            if (defItem == null && (modItem != null && !modItem.randomized))
            {
                //Debug.Log($"Found ModItem: {InternalName}");
                return true;
            }
            //Debug.Log("Not ModItem");
            return false;
        }

        public static bool CheckItemList(string InternalName)
        {
            if (ArchipelagoNotIncluded.StarterTech.Contains(InternalName))
                return true;

            if (ArchipelagoNotIncluded.Options.CreateModList)
                return false;

            //Debug.Log($"InternalName: {InternalName} {ArchipelagoNotIncluded.hadBionicDupe}");
            if (APSaveData.Instance.HadBionicDupe && InternalName == "CraftingTable")
                return true;

            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.internal_name == InternalName);
            ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.internal_name == InternalName);
            if (defItem != null)
            {
                bool ItemFound = APSaveData.Instance.LocalItemList.Contains(defItem.name);
                //Debug.Log($"CheckItemList: {ItemFound}");
                return ItemFound;
            }
            else if (modItem != null)
            {
                bool ItemFound = APSaveData.Instance.LocalItemList.Contains(modItem.name);
                //Debug.Log($"CheckItemList: {ItemFound}");
                return ItemFound;
            }
            else
                return false;
            /*    return false;
            if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived?.Count() == 0)
                return false;
            foreach (ItemInfo item in ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived)
                if (item.ItemDisplayName == defItem.name)
                    return true;
            return false;*/
        }

        public static bool CheckItemList(TechItem TechItem)
        {
            //Debug.Log($"Name: {TechItem.Name}, Id: {TechItem.Id} {ArchipelagoNotIncluded.hadBionicDupe}");
            if (ArchipelagoNotIncluded.Options.CreateModList)
                return false;

            char[] delimiters = { '<', '>' };
            string name = ArchipelagoNotIncluded.CleanName(TechItem.Name);

            if (APSaveData.Instance.HadBionicDupe && TechItem.Id == "CraftingTable")
                return true;

            /*if (ArchipelagoNotIncluded.session.Items.AllItemsReceived.SingleOrDefault(i => i.ItemDisplayName == name) != null)
            {
                Debug.Log($"Found item in received list: {name}");
                return true;
            }
            else
            {
                Debug.Log($"Not in received list: {name}");
                return false;
            }*/
            //return true;
            return APSaveData.Instance.LocalItemList.Contains(name);
            /*if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived == null)
                return false;
            if (ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived.Count() == 0)
                return false;
            foreach (ItemInfo item in ArchipelagoNotIncluded.netmon?.session?.Items?.AllItemsReceived)
                if (item.ItemDisplayName == name)
                    return true;
            return false;*/
            //return false;
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.OnSpawn))]
    public static class SaveLoader_Load_Patch
    {
        public static void Postfix()
        {
            if (ArchipelagoNotIncluded.Options.CreateModList)
                return;
            if (APSaveData.Instance.APSeedInfo != null)
                ArchipelagoNotIncluded.info = APSaveData.Instance.APSeedInfo;

            List<PlanScreen.PlanInfo> storage = [];
            /*foreach (PlanScreen.PlanInfo info in TUNING.BUILDINGS.PLANORDER)
            {
                Debug.Log($"{info.category}: ");
                foreach (KeyValuePair<string, string> kvp in info.buildingAndSubcategoryData)
                {
                    Debug.Log($"{kvp.Key}: {kvp.Value}");
                }
            }*/
            //foreach (PlanScreen.PlanInfo key in TUNING.BUILDINGS.PLANORDER)
            //storage.Add(new PlanScreen.PlanInfo(key.category, key.hideIfNotResearched, key.data, ));
            if (!APSaveData.Instance.AllowResourceChecks)
                return;

            foreach (DefaultItem item in ArchipelagoNotIncluded.AllDefaultItems)
            {
                //Debug.Log(info.spaced_out);
                //Debug.Log(item.internal_tech + " " + item.internal_tech_base);
                string InternalTech = ArchipelagoNotIncluded.info.spaced_out ? item.internal_tech : item.internal_tech_base;
                switch (item.version)
                {
                    case "BaseOnly":
                        if (ArchipelagoNotIncluded.info.spaced_out)
                            break;
                        goto default;
                    case "SpacedOut":
                        if (!ArchipelagoNotIncluded.info.spaced_out)
                            break;
                        goto default;
                    case "Frosty":
                        if (!ArchipelagoNotIncluded.info.frosty)
                            break;
                        goto default;
                    case "Bionic":
                        if (!ArchipelagoNotIncluded.info.bionic)
                            break;
                        goto default;
                    default:
                        if (ArchipelagoNotIncluded.allTechList.ContainsKey(item.internal_name))
                            continue;
                        ArchipelagoNotIncluded.allTechList.Add(item.internal_name, item.name);
                        break;
                }
                if (ArchipelagoNotIncluded.Sciences?.Count > 0 && ArchipelagoNotIncluded.Sciences.TryGetValue(InternalTech, out List<string> techList) == true)
                {
                    techList ??= [];
                    techList.Add(item.internal_name);
                }
                else
                {
                    if (InternalTech == "None")
                        continue;
                    ArchipelagoNotIncluded.Sciences[InternalTech] =
                        [
                            item.internal_name
                        ];
                }
            }

            if (ArchipelagoNotIncluded.AllModItems != null)
            {
                foreach (ModItem item in ArchipelagoNotIncluded.AllModItems)
                {
                    if (ArchipelagoNotIncluded.info?.apModItems.Contains(item.internal_name) == true)
                    {
                        item.randomized = true;
                        if (ArchipelagoNotIncluded.allTechList.ContainsKey(item.internal_name))
                            continue;
                        ArchipelagoNotIncluded.allTechList.Add(item.internal_name, item.name);
                    }
                }
            }

            Techs instance = Db.Get().Techs;

            Dictionary<string, int> idCounts = [];
            foreach (KeyValuePair<string, List<string>> pair in ArchipelagoNotIncluded.info.technologies)
            {
                if (pair.Key.ToLower() == "none" || String.IsNullOrEmpty(pair.Key))
                {
                    Debug.LogWarningFormat("Skipping technology with key '{0}'", pair.Key);
                    continue;
                }
                Debug.Log($"Generating research for {pair.Key}, ({pair.Value.Join(s => s, ",")})");
                Tech tech = instance.TryGet(pair.Key);
                Dictionary<string, float> researchCost = null;
                if (ArchipelagoNotIncluded.cheatmode)
                    researchCost = new Dictionary<string, float>
                        {
                            {"basic", 1f },
                            {"advanced", 0f },
                            {"nuclear", 0f },
                            {"orbital", 0f }
                        };
                if (tech == null)
                    tech = new Tech(pair.Key, [], instance, researchCost);
                else
                {
                    if (researchCost != null)
                        tech.costsByResearchTypeID = researchCost;
                    tech.unlockedItemIDs = [];
                    tech.unlockedItems = [];
                }
                foreach (string techitemidplayer in pair.Value)
                {
                    string[] splits = techitemidplayer.Split([">>"], StringSplitOptions.RemoveEmptyEntries);
                    string techitemid = splits[0];
                    int playerid = int.Parse(splits[1]);
                    if (idCounts.ContainsKey(techitemid))
                        idCounts[techitemid]++;
                    else
                        idCounts[techitemid] = 0;
                    //Debug.Log($"Player: {ArchipelagoNotIncluded.info.AP_PlayerID} ItemID: {techitemid} PlayerID: {playerid}");
                    if (ArchipelagoNotIncluded.info.AP_PlayerID == playerid && !techitemid.StartsWith("Care Package"))
                    {
                        //Debug.Log("Item was given default sprite");
                        TechItem item = Db.Get().TechItems.TryGet(techitemid);
                        if (item != null)
                        {
                            item.parentTechId = pair.Key;
                            tech.unlockedItems.Add(item);
                            //InjectionMethods.MoveItemToNewTech(techitemid, item.parentTechId, pair.Key);
                        }
                        else
                        {
                            Debug.Log($"TechItem for {techitemid} does not exist");
                            //tech.unlockedItems.Add(Db.Get().TechItems.AddTechItem(techitemid, techitemid, techitemid, Db.Get().TechItems.GetPrefabSpriteFnBuilder(techitemid.ToTag())));
                            //InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, techitemid, techitemid, techitemid + "_kanim");
                        }
                        tech.AddUnlockedItemIDs([techitemid]);
                        //InjectionMethods.AddBuildingToTechnology(pair.Key, techitemid);
                        //ArchipelagoNotIncluded.allTechList.Remove(techitemid);
                    }
                    else
                    {
                        //Debug.Log("Item was given custom sprite");
                        techitemid += idCounts[techitemid];
                        TechItem item = Db.Get().TechItems.TryGet(techitemid);
                        if (item != null)
                        {
                            item.parentTechId = pair.Key;
                            tech.unlockedItems.Add(item);
                        }
                        else
                        {
                            if (ArchipelagoNotIncluded.cheatmode)
                                InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, techitemid, "A mysterious item from another world", "apItemSprite_kanim");
                            else
                                InjectionMethods.AddItemToTechnologyKanim(techitemid, pair.Key, "Unknown Artifact", "A mysterious item from another world", "apItemSprite_kanim");
                        }
                    }
                }

                //ArchipelagoNotIncluded.TechList.Add(tech.Id, new List<string>(tech.unlockedItemIDs));
                //new Tech(pair.Key, pair.Value.ToList(), __instance);
            }
            /*foreach (string item in ArchipelagoNotIncluded.allTechList.Keys)
            {
                ModUtil.AddBuildingToPlanScreen(TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[item], item);
            }

            foreach (KeyValuePair<Tag, HashedString> kvp in PlanScreen.Instance.tagCategoryMap)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value}");
            }*/
            //PlanScreen.Instance.tagCategoryMap.Clear();
            //foreach (KeyValuePair<Tag, HashedString> keyValuePair in storage)
            //    PlanScreen.Instance.tagCategoryMap.Add(keyValuePair.Key, keyValuePair.Value);
            int apItems = APSaveData.Instance.LocalItemList.Count;
            Debug.Log($"apItems: {apItems}, lastItem: {ArchipelagoNotIncluded.lastItem}");
            /*if (apItems == 0)
                return;

            if (apItems == ArchipelagoNotIncluded.lastItem)
                return;*/

            //ArchipelagoNotIncluded.netmon.ProcessItemQueue();
        }
    }

    [HarmonyPatch(typeof(PlanScreen))]
    [HarmonyPatch(nameof(PlanScreen.OnSpawn))]
    public static class PlanScreen_OnSpawn_Patch
    {
        public static void Postfix()
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return;
            /*List<string> strings = new List<string>()
            {
                "CryoFuel Propulsion",
                "High Velocity Destruction",
                "Improved Hydrocarbon Propulsion",
                "Radbolt Propulsion",
            };
            foreach (string name in strings)
            {
                List<DefaultItem> defItems = ArchipelagoNotIncluded.info.spaced_out ? ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech == name) : ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech_base == name);
                int modItems = 0;
                if (ArchipelagoNotIncluded.info.apModItems.Count > 0)
                    modItems = ArchipelagoNotIncluded.AllModItems.FindAll(i => i.tech == name).Count;
                Debug.Log($"Count: {defItems.Count} {modItems}");
                int count = defItems.Count + modItems;
                string[] locationNames = new string[count];
                for (int i = 0; i < count; i++)
                {
                    string fullLocationName = $"{name} - {i + 1}";
                    Debug.Log($"Location: {fullLocationName} - {i + 1}");
                    locationNames[i] = fullLocationName;
                }
                ArchipelagoNotIncluded.AddLocationChecks(locationNames);
            }
            ArchipelagoNotIncluded.AddLocationChecks("Discover Resource: Sucrose");*/
            ArchipelagoNotIncluded.netmon.ProcessLocationQueue();
            ArchipelagoNotIncluded.netmon.ProcessItemQueue();
        }
    }

    [HarmonyPatch(typeof(SaveGame))]
    [HarmonyPatch(nameof(SaveGame.OnPrefabInit))]
    public class SaveGame_Patch
    {
        public static void Postfix(SaveGame __instance)
        {
            __instance.gameObject.AddOrGet<APSaveData>();
        }
    }

    [HarmonyPatch(typeof(DiscoveredResources))]
    [HarmonyPatch(nameof(DiscoveredResources.Discover), new[] { typeof(Tag), typeof(Tag) })]
    public static class Discover_Patch
    {
        public static void Prefix(DiscoveredResources __instance, out int __state)
        {
            __state = __instance.newDiscoveries.Count;
        }

        public static void Postfix(DiscoveredResources __instance, Tag tag, int __state)
        {
            if (APSaveData.Instance.AllowResourceChecks && __instance.newDiscoveries.Count > __state && ArchipelagoNotIncluded.info != null)      // New Discovery was added
            {
                string ResourceName = tag.ProperNameStripLink();
                Debug.Log($"New Discovery: Name: {tag.Name}, StripLink: {ResourceName}");
                //foreach (string resource in ArchipelagoNotIncluded.info.resourceChecks)
                //Debug.Log(resource);
                string location = $"Discover Resource: {ResourceName}";
                if (ArchipelagoNotIncluded.info.resourceChecks.Contains(location))
                    ArchipelagoNotIncluded.AddLocationChecks(location);
            }
        }
    }

    [HarmonyPatch(typeof(ColonyAchievementTracker))]
    [HarmonyPatch(nameof(ColonyAchievementTracker.TriggerNewAchievementCompleted))]
    public static class TriggerNewAchievementCompleted_Patch
    {
        public static void Postfix(string achievement)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return;

            string goal = ArchipelagoNotIncluded.info.goal;
            APSaveData inst = APSaveData.Instance;
            Debug.Log($"New Achievement: {achievement} Goal: {goal}");
            switch (achievement)
            {
                case "CompleteResearchTree":
                    if (goal == "research_all")
                        inst.GoalComplete = true;
                    goto default;
                case "space_race":
                case "ReachedSpace":
                    if (goal == "launch_rocket")
                        inst.GoalComplete = true;
                    goto default;
                case "Thriving":
                    if (goal == "home_sweet_home")
                        inst.GoalComplete = true;
                    goto default;
                case "ReachedDistantPlanet":
                    if (goal == "great_escape")
                        inst.GoalComplete = true;
                    goto default;
                case "CollectedArtifacts":
                    if (goal == "cosmic_archaeology")
                        inst.GoalComplete = true;
                    goto default;
                default:
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(MonumentPart))]
    [HarmonyPatch(nameof(MonumentPart.IsMonumentCompleted))]
    public static class Monument_Patch
    {
        public static void Postfix(bool __result)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return;

            if (ArchipelagoNotIncluded.info.goal == "monument" && __result)
                APSaveData.Instance.GoalComplete = true;

        }
    }

    [HarmonyPatch(typeof(Research))]
    [HarmonyPatch(nameof(Research.CheckBuyResearch))]
    public static class Research_CheckBuyResearch_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);
            MethodInfo getInstance = AccessTools.PropertyGetter(typeof(Game), nameof(Game.Instance));
            FieldInfo activeResearch = AccessTools.Field(typeof(Research), nameof(Research.activeResearch));
            MethodInfo sendCheck = AccessTools.Method(typeof(Research_CheckBuyResearch_Patch), nameof(Research_CheckBuyResearch_Patch.SendArchipelagoCheck));
            MethodInfo kmonoTrigger = AccessTools.Method(typeof(KMonoBehaviour), nameof(KMonoBehaviour.Trigger), [typeof(int), typeof(object)]);

            matcher.MatchStartForward(new CodeMatch(OpCodes.Call, getInstance))
                .RemoveInstructions(6)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, activeResearch),
                    new CodeInstruction(OpCodes.Call, sendCheck)
                );

            return matcher.InstructionEnumeration();
        }

        static void SendArchipelagoCheck(TechInstance instance)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
            {
                Game.Instance.Trigger(-107300940, instance.tech);
                return;
            }

            char[] delimiters = { '<', '>' };
            string name = instance.tech.Name.Split(delimiters)[2];
            Debug.Log($"Name: {name}");
            if (name == "Cryofuel Propulsion")
                name = "CryoFuel Propulsion";
            if (name == "Projectiles")
                name = "Jetpacks";
            List<DefaultItem> defItems = ArchipelagoNotIncluded.info.spaced_out ? ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech == name) : ArchipelagoNotIncluded.AllDefaultItems.FindAll(i => i.tech_base == name);
            int modItems = 0;
            if (ArchipelagoNotIncluded.info.apModItems.Count > 0)
                modItems = ArchipelagoNotIncluded.AllModItems.FindAll(i => i.tech == name).Count;
            Debug.Log($"Count: {defItems.Count} {modItems}");
            int count = defItems.Count + modItems;
            string[] locationNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                string fullLocationName = $"{name} - {i + 1}";
                Debug.Log($"Location: {fullLocationName} - {i + 1}");
                locationNames[i] = fullLocationName;
            }
            ArchipelagoNotIncluded.AddLocationChecks(locationNames);
        }
    }

    [HarmonyPatch]
    public static class Event_Subscribe_Patch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            Dictionary<Type, string> MethodDict = new Dictionary<Type, string>()
                {
                    {typeof(PlanBuildingToggle), nameof(PlanBuildingToggle.Config)},
                    {typeof(PlanScreen), nameof(PlanScreen.OnPrefabInit)},
                    //{typeof(BuildMenuBuildingsScreen), nameof(BuildMenuBuildingsScreen.OnSpawn)},
                    //{typeof(BuildMenu), nameof(BuildMenu.OnCmpEnable)},
                    {typeof(ConsumerManager), nameof(ConsumerManager.OnSpawn)},
                    {typeof(MaterialSelectionPanel), nameof(MaterialSelectionPanel.OnPrefabInit)},
                    {typeof(SelectModuleSideScreen), nameof(SelectModuleSideScreen.OnCmpEnable)},
                };
            foreach (KeyValuePair<Type, string> pair in MethodDict)
                yield return AccessTools.Method(pair.Key, pair.Value);
        }
        public static void Postfix(object __instance, MethodBase __originalMethod)
        {
            if (ArchipelagoNotIncluded.info == null)
                return;

            int eventid = 11390976;
            switch (__instance)
            {
                case PlanBuildingToggle toggle:
                    toggle.gameSubscriptions.Add(Game.Instance.Subscribe(eventid, toggle.CheckResearch));
                    break;
                case PlanScreen screen:
                    if (!BuildMenu.UseHotkeyBuildMenu())
                        Game.Instance.Subscribe(eventid, screen.OnResearchComplete);
                    break;
                case BuildMenuBuildingsScreen screen:
                    Game.Instance.Subscribe(eventid, screen.OnResearchComplete);
                    break;
                case BuildMenu menu:
                    if (__originalMethod.Name == nameof(BuildMenu.OnCmpEnable))
                        Game.Instance.Subscribe(eventid, menu.OnResearchComplete);
                    else
                        Game.Instance.Unsubscribe(eventid, menu.OnResearchComplete);
                    break;
                case ConsumerManager manager:
                    Game.Instance.Subscribe(eventid, manager.RefreshDiscovered);
                    break;
                case MaterialSelectionPanel panel:
                    panel.gameSubscriptionHandles.Add(Game.Instance.Subscribe(eventid, delegate { panel.RefreshSelectors(); }));
                    break;
                case SelectModuleSideScreen screen:
                    screen.gameSubscriptionHandles.Add(Game.Instance.Subscribe(eventid, screen.UpdateBuildableStates));
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(POITechItemUnlocks.Instance))]
    [HarmonyPatch(nameof(POITechItemUnlocks.Instance.UnlockTechItems))]
    public static class POITechItemUnlocks_unlockTechItems_Patch
    {
        public static bool Prefix(POITechItemUnlocks.Instance __instance)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return true;

            int count = 0;

            List<string> techIDs = __instance.def.POITechUnlockIDs;
            switch (techIDs)
            {
                case List<string> x when x.Contains("Campfire"):
                    count = 2;
                    break;
                case List<string> x when x.Contains("MissileFabricator"):
                    count = 3;
                    break;
                default:
                    return true;
            }
            string[] locationNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                string fullLocationName = $"Research Portal - {i + 1}";
                Debug.Log($"Location: {fullLocationName} - {i + 1}");
                locationNames[i] = fullLocationName;
            }
            ArchipelagoNotIncluded.AddLocationChecks(locationNames);
            APSaveData.Instance.ResearchPortalUnlocked = true;

            MusicManager.instance.PlaySong("Stinger_ResearchComplete", false);
            __instance.UpdateUnlocked();

            return false;
        }
    }

    [HarmonyPatch(typeof(POITechItemUnlocks.Instance))]
    [HarmonyPatch(nameof(POITechItemUnlocks.Instance.UpdateUnlocked))]
    public static class POITechItemUnlocks_UpdateUnlocked_Patch
    {
        public static bool Prefix(POITechItemUnlocks.Instance __instance)
        {
            if (!APSaveData.Instance.AllowResourceChecks)
                return true;

            bool value = false;
            if (APSaveData.Instance.ResearchPortalUnlocked)
                value = true;
            __instance.sm.isUnlocked.Set(value, __instance.smi, false);

            return false;
        }
    }
}
