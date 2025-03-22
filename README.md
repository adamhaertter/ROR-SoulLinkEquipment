# Soul Link Equipment Mod
<p>
<img src="https://github.com/yurijserrano/Github-Profile-Readme-Logos/blob/master/programming%20languages/c%23.svg?raw=true" height="50">
<img src="https://github.com/yurijserrano/Github-Profile-Readme-Logos/blob/master/tools/unity.png?raw=true" height=50>
</p>

## Description

This is a mod for the game Risk of Rain 2. It simply adds one Equipment-tier item to the game which allows you to swap your playable character when used. It features custom UI functionality, as well as configurable options support to tweak specifics and replace the keybinds for the custom window. The item is fully modelled and sprited.

## To-Do List
- Hide the debug controls (P, F1)
- Create the Thunderstore upload
- Address inline TODOs
- Code cleanup (last)

## Issues and Bugs

If you found this because you've encountered an issue with the mod, please submit a report as a GitHub Issue above so that I can look into it. Thanks!

### Known Bugs
- Charactes with skills limited to a certain number of charges per stage, like Captain's supply beacons, will have those charges reset when they are transformed. 
- The UI text alignment can become misaligned if the player has less than 4 playable Survivors unlocked, which is quite unlikely unless starting an entirely new profile. The UI is still fully functional in these cases, but could benefit from some layout optimization on my end.

## Items

#### Implemented
| Icon | Name | Description |
| --- | --- | --- |
| ![SoulLink Icon](https://github.com/adamhaertter/ROR-SoulLinkEquipment/blob/master/img/SoulLinkIcon_Fused.png?raw=true) | Soul Links | Transform into another Survivor at will. |

*The tier of this item is configurable. It can be made either a standard Equipment or Lunar Equipment from the Settings menu in-game.*

## Installation
You can download the latest uploaded release from the ThunderStore on the release [page](https://thunderstore.io/package/BlueBubbee/Prestige_Items_Beta/) or by using the mod manager. **This is the recommended installation.** I'd recommend using r2ModMan as your launcher, but the Thunderstore app probably works too.

If you prefer to manually install, ensure you have BepInEx, R2API, and Risk_Of_Options already installed. You can download the `SoulLink.dll`, `SoulLink.pdb`, and `soullinkassets` files from the GitHub Releases page and place them in a folder under your `[Mod Profile]/BepInEx/plugins/` folder. These files need to be at the same folder level as each other to be able to load the sprites and models. 

## Credits
I built this with the knowledge I had gained from my work on [my first mod](https://github.com/adamhaertter/PrestigeItemsMod/), but cross referencing all of these resources was still incredibly helpful.
- [Modding Wiki Tutorials & Boilerplate Demo](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Getting-Started/First-Mod/)
- [Henry Tutorial](https://github.com/ArcPh1r3/HenryTutorial) 
- [KomradeSpectre's Aetherium Item Tutorial](https://www.youtube.com/watch?v=8TsF8elv_m0)
- [ItemDisplayPlacementHelper by KingEnderBrine](https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/)
    - I would not have been up to doing the item displays if it weren't for this mod.