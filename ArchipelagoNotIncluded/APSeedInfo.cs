using System;
using System.Collections.Generic;

namespace ArchipelagoNotIncluded
{
    public class APSeedInfo
    {
        public Version APWorld_Version { get; set; }
        public string AP_seed { get; set; }
        public string AP_slotName { get; set; }
        public int AP_PlayerID { get; set; }
        public string URL { get; set; }
        public int port { get; set; }
        public bool spaced_out { get; set; }
        public bool frosty { get; set; }
        public bool bionic { get; set; }
        public bool teleporter { get; set; }

        public Dictionary<string, List<string>> technologies { get; set; }
        public List<string> apModItems { get; set; }
        public string goal { get; set; }
        public string planet { get; set; }
        public List<string> resourceChecks { get; set; }

        public APSeedInfo(string version = "0.8.5.0", string win_condition = "research_all")
        {
            version ??= "0.8.5.0";
            APWorld_Version = new Version(version);
            if (win_condition == null)
                goal = "research_all";
            technologies = [];
            apModItems = [];
            resourceChecks = [];
        }

        public string GetGoal()
        {
            switch (goal)
            {
                case "launch_rocket":
                    return STRINGS.UI.GOALS.LAUNCH_ROCKET;
                case "monument":
                    return STRINGS.UI.GOALS.MONUMENT;
                case "research_all":
                    return STRINGS.UI.GOALS.RESEARCH_ALL;
                case "home_sweet_home":
                    return STRINGS.UI.GOALS.HOME_SWEET_HOME;
                case "great_escape":
                    return STRINGS.UI.GOALS.GREAT_ESCAPE;
                case "cosmic_archaeology":
                    return STRINGS.UI.GOALS.COSMIC_ARCHAEOLOGY;
                default:
                    return STRINGS.UI.GOALS.UNKNOWN;
            }
        }
    }

}