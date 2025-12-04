using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Epic.OnlineServices.Platform;
using KMod;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoNotIncluded
{
    [ModInfo("https://github.com/peterhaneve/ONIMods", collapse: true)]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    //[RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    public sealed class ANIOptions : IOptions
    {
        [Option("Create Mod List", "Creates list of mod items for Archipelago to randomize." +
            "\nAutomatically turns itself off after creating the list.")]
        [JsonProperty]
        public bool CreateModList { get; set; }

        [Option("URL", "Archipelago server location. Default: Archipelago.gg\nIf you are connecting locally: localhost.")]
        [JsonProperty]
		public string URL { get; set; }

        [Option("Port", "Port number to connect to.")]
        [JsonProperty]
        public int Port { get; set; }

        [Option("Player Name", "Also called Slot Name.")]
        [JsonProperty]
        public string SlotName { get; set; }

        [Option("Password", "Password for Multiworld. Leave blank if there isn't one.")]
        [JsonProperty]
        public string Password { get; set; }

        [Option("Automatic Reconnect", "Automatically reconnects to Archipelago if connection is lost.")]
        [JsonProperty]
        public bool AutoReconnect { get; set; }

        [Option("Reconnect Interval", "Time (in seconds) between reconnection attempts.")]
        [JsonProperty]
        public int ReconnectInterval { get; set; }

        public static string modPath = Path.Combine(Path.Combine(Manager.GetDirectory(), "config"), "ArchipelagoNotIncluded");

        public ANIOptions()
        {
            CreateModList = false;
            URL = "Archipelago.gg";
            Port = 38281;
            SlotName = "PlayerName";
            Password = "";
            AutoReconnect = true;
            ReconnectInterval = 30;
        }

        public void OnOptionsChanged()
        {
            string text = "";
            if (CreateModList)
            {
                
                /*List<ModItem> modItems = new List<ModItem>();
                foreach (Tech tech in Db.Get().Techs.resources)
                {
                    foreach (TechItem techitem in tech.unlockedItems)
                    {
                        DefaultItem defItem = AllDefaultItems.Find(i => i.internal_name == techitem.Id);
                        if (defItem == null && !PreUnlockedTech.Contains(techitem.Id))
                            modItems.Add(new ModItem(techitem));
                    }
                }*/
                string modItemsPath = Path.Combine(Path.Combine(Manager.GetDirectory(), "config"), "ArchipelagoNotIncluded");
                if (!System.IO.Directory.Exists(modItemsPath))
                    System.IO.Directory.CreateDirectory(modItemsPath);
                Debug.Log($"Items: {ArchipelagoNotIncluded.AllModItems.Count}, Directory: {modItemsPath}");
                //File.WriteAllText(modDirectory.ToString() + "\\ModItems.json", JsonConvert.SerializeObject(modItems, Formatting.Indented));
                if (ArchipelagoNotIncluded.AllModItems.Count > 0)
                {
                    text = $"Mod List created at {modItemsPath} \n\n";
                    using FileStream fs = File.Open(Path.Combine(modItemsPath, $"{SlotName}_ModItems.json"), FileMode.Create);
                    using StreamWriter sw = new(fs);
                    using JsonTextWriter jw = new(sw);
                    jw.Formatting = Formatting.Indented;
                    jw.IndentChar = ' ';
                    jw.Indentation = 4;

                    JsonSerializer serializer = new();
                    serializer.Serialize(jw, ArchipelagoNotIncluded.AllModItems);
                }
                CreateModList = false;
                POptions.WriteSettings(this);
            }

            ArchipelagoNotIncluded.netmon.session.Socket.DisconnectAsync();
            APNetworkMonitor netmon = new(URL, Port, SlotName, Password);
            netmon.StartSession();
            var dialogue = ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas));
            text += "Connection to Archipelago failed.\nPlease check your connection settings and try again.";
            string title = "Archipelago";
            System.Action confirm = null;
            LoginResult result = netmon.session.TryConnectAndLogin("Oxygen Not Included", SlotName, ItemsHandlingFlags.AllItems, APNetworkMonitor.APVersion, password: Password);
            if (result.Successful)
            {
                if (ArchipelagoNotIncluded.netmon.session != null)
                {
                    ArchipelagoNotIncluded.lastItem = 0;
                }

                ArchipelagoNotIncluded.netmon = netmon;
                LoginSuccessful success = (LoginSuccessful)result;
                ArchipelagoNotIncluded.info = JsonConvert.DeserializeObject<APSeedInfo>(JsonConvert.SerializeObject(success.SlotData), [new VersionConverter()]);
                Debug.Log($"SlotData Received - AP World Version: {ArchipelagoNotIncluded.info.APWorld_Version}");

                text = "Connection to Archipelago was successful!";
                if (ArchipelagoNotIncluded.info.spaced_out && !DlcManager.IsExpansion1Active())
                {
                    if (DlcManager.IsContentOwned("EXPANSION1_ID"))
                    {
                        text += "\nThe game will now restart to enable\nSpaced Out DLC.";
                        title = "Spaced Out DLC";
                        confirm = new System.Action(() => DlcManager.ToggleDLC("EXPANSION1_ID"));
                    }
                    else
                    {
                        text += "\nSpaced Out DLC has been enabled on Archipelago but is not in your Steam Library. You will need to purchase it or change your Archipelago settings.";
                        title = "DLC Warning";
                    }

                }
                else if (!ArchipelagoNotIncluded.info.spaced_out && DlcManager.IsExpansion1Active())
                {
                    text += "\nThe game will now restart to enable\nSpaced Out DLC.";
                    title = "Spaced Out DLC";
                    confirm = new System.Action(() => DlcManager.ToggleDLC("EXPANSION1_ID"));
                }
            }
            dialogue.PopupConfirmDialog(text, confirm, null, title_text: title);
        }

        public IEnumerable<IOptionsEntry> CreateOptions()
        {
            return [];
        }
    }
}
