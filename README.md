# Archipelago Not Included

## What is this?

As of right now, the bare minimum has been completed to enable Multiworld functionality. The goal is to add it to the large list of games for [Archipelago Multiworld Randomizer](https://archipelago.gg)<br/>
It is currently up-to-date with all DLC except for Prehistoric Planet Pack (nearly done making it compatible) and any combination of DLC can be enabled/disabled.<br/>
There are some known visual bugs that haven't been fixed yet, but nothing should affect gameplay. (See [Known Bugs](#known-bugs) below)

## Usage

The randomization logic is handled by the Archipelago randomizer, so you have to have that installed to create the seed information and/or viewing items sent/recieved while playing. The mod does not change any of the standard planetoid or dupe generation and can work with any setting (see below note about logical requirements if you find an unwinnable scenario).

This mod should be compatible with any other mod ~~that does not modify the tech tree in any way. Any mods that add technologies will not be included in the randomization and will not appear in your tech tree.~~

## Logic

In addition to requiring their research points and research buildings, each tier of research also comes with another logical requirement:
+ Advanced Research logically requires the ability to pipe fluids and gasses
+ Materials Research logically requires the ability to Refine metal, and produce Radbolts
+ Space Research logically requires the ability to build a functioning rocket and produce Data Banks

The logic might still not be completable, due to starting planetoid conditions. If this happens, please share your mod json and world seed in the Oxygen Not Included thread in the Archipelago Discord, under Future-Game-Design.

Ranching Dreckos for plastic or Hatches for Refined Metal is considered in-logic, and if necessary, buildings will be available before they are logically required.

## Installing the Mod (only manual installation for now)

### ~~Steam Workshop~~

1. ~~Open `Steam` and navigate to the `Oxygen Not Included` `Workshop` page.~~
2. ~~Use the search bar to search for `ArchipelagoNotIncluded`.~~
3. ~~Subscribe to the mod.~~
4. ~~Open `Oxygen Not Included`.~~
5. ~~Navigate to the `Mods` menu.~~
6. ~~Click the empty checkbox next to `ArchipelagoNotIncluded`.~~
7. ~~Close the `Mods` menu and accept the changes. The game will need to restart.~~
8. ~~The mod is now installed. If you want buildings added by Mods to be included in the randomization, proceed to [Mod List Setup (Optional)](#mod-list-setup-(optional)). If not, proceed to [Seed Generation](#seed-generation)~~

### Manual Installation

1. Download the `ArchipelagoNotIncluded.zip`
2. Navigate to your ONI Mods Folder. On Windows, this is either<br/>`C:/Users/[YOUR NAME]/Documents/Klei/OxygenNotIncluded/mods` OR<br/>`C:/Users/[YOUR NAME]/OneDrive/Documents/Klei/OxygenNotIncluded/mods`
3. If there is a folder called `Local`, open it. If not, make a new folder called `Local` and then open.
4. Place the `ArchipelagoNotIncluded.zip` file in the `Local` folder and extract it. You should now have a folder called `ArchipelagoNotIncluded`.
5. Open the new `ArchipelagoNotIncluded` folder. You should see `.dll` files and most importantly, `mod.yaml` and `mod_info.yaml`. If you do, everything has been done correctly.
6. Open Oxygen Not Included.
7. Navigate to the `Mods` menu.
8. Click the empty checkbox next to `ArchipelagoNotIncluded`.
9. Close the `Mods` menu and accept the changes. The game will need to restart.

## Mod List Setup (Optional)

If you want to randomize buildings added by mods, perform the following steps. If you don't want to randomize them, skip to [Seed Generation](#seed-generation)

1. After installing the mod via one of the methods detailed above, it should have restarted with the mod enabled. If not, open `Oxygen Not Included`.
2. Navigate to the `Mods` menu.
3. Enable any mods you want to play with.
4. Click the `Settings` button next to `ArchipelagoNotIncluded`
5. Change the `Player Name` and remember this for later. This is the name other Multiworld players will see and will be needed in `Seed Generation`
6. Click the `Create Mod List` checkbox and click `Ok`.
7. Navigate to the ONI Mods Folder. On Windows, this is either<br/>`C:/Users/[YOUR NAME]/Documents/Klei/OxygenNotIncluded/mods/config/ArchipelagoNotIncluded` OR<br/>`C:/Users/[YOUR NAME]/OneDrive/Documents/Klei/OxygenNotIncluded/mods/config/ArchipelagoNotIncluded`
8. You should see a file called `[Player Name]_ModItems.json`.
9. Copy this file somewhere easily accessed, it will be needed in `Seed Generation`

## Seed Generation

Install Archipelago if you haven't already. It can be found [here](https://github.com/ArchipelagoMW/Archipelago/releases).
Required files can be found on the [Releases](https://github.com/ShadowKitty42/ONI-Archipelago/releases/latest) page.

1. Install oni.apworld into the `custom_worlds` folder in your Archipelago installation.
2. If any players have created a Mod List, navigate to `data/ONI` in your Archipelago installation, and copy the `[Player Name]_ModItems.json` file(s) to that folder.
3. Download the attached YAML and modify any options you deem necessary and place it in your Players directory in your Archipelago Installation. You can choose any combination of DLC listed in the YAML.
4. Run "Generate" either through the executable or the AP launcher.
5. The result will be in your `Output` directory. This can be uploaded to [Archipelago Host Game](https://archipelago.gg/uploads) to use Archipelago servers as the host. Or it can be ran locally by extracting the ZIP and running the `.archipelago` file.

## Connecting to Archipelago

1. After installing the mod via one of the methods detailed above, it should have restarted with the mod enabled. If not, open `Oxygen Not Included`.
2. Open the `Mods` menu.
3. (Optional) Enable any mods you want to use. None of them will be randomized and will function as normal. Any newly enabled mods will require a restart to apply.
4. Click the `Settings` button next to `ArchipelagoNotIncluded`
5. Change the settings listed to match your setup. NOTE: Player name MUST match what you put in the YAML file in step #3 of Seed Generation. (in URL, change it to `localhost` if running locally)
6. You're ready to play! Start a new game and enjoy!

## Things to do

### Planned Features

- [x] ~~Add in-game options menu for changing Multiworld settings (URL and Port)~~
- [x] ~~Add additional locations to the game other than Research Tree~~
- [ ] Add Death Link Options. Current ideas for this are Random (Kill a random dupe when someone dies), Oldest (Kill oldest dupe), and Character Linked (Each dupe is linked to someone participating in Death Link)
- [x] ~~Maybe add support for mods that add new technologies~~
- [ ] Add location checks and logic for achievements
- [ ] Possibly add resource randomization via 1:1 mapping (for example, every dirt spot will drop algae).

### Known Bugs

- [x] ~~When research is completed, the original tech category gets a <!> icon even if nothing was added to it.~~
- [ ] When research is completed, a research complete notification is generated and has incorrect information about what was unlocked.
- [x] ~~If your game includes items from other games, they will not appear in the Research Tree. Despite this, every Research topic still unlocks the same number of items as normal.~~
- [ ] Building Categories appear at the bottom even if there are no buildings listed for it.
- [ ] Trying to "Copy" the Ration Box you start the game with causes a crash.
