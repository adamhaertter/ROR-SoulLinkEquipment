# Soul Links Equipment
**v1.0.0**

Adds one new equipment to the game, which allows the player to choose another Survivor to transform back and forth between on use. This was mostly done quickly for fun, but I figured I'd share it here for anyone to mess around with.

Want to see a showcase? Check out [this video!](https://www.youtube.com/watch?v=sCxEJ0Buscs)

## Equipment

| Icon | Name | Description | Cooldown |
| --- | --- | --- | --- |
| ![SoulLink Icon](https://github.com/adamhaertter/ROR-SoulLinkEquipment/blob/master/img/SoulLinkIcon.png?raw=true) | Soul Links | On use, become a different survivor. The first use after pickup will select the survivor to bond to, then further uses will trigger the transformation. | 60s<br>(configurable) |

*The tier of this item is configurable. It can be made either a standard Equipment or Lunar Equipment from the Settings menu in-game.*

## How to Use
The Soul Links are part of the normal equipment item pool, meaning you can find them in any of the normal ways (equipment barrel, shrine of chance, etc.). Once you've picked them up, your **first use** will bring up this UI Menu allowing you to select which Survivor you'd like to transform into. This applies to each pickup of the item and at the start of each stage.

![Demonstration of the custom menu](https://github.com/adamhaertter/ROR-SoulLinkEquipment/blob/master/img/Demo_Menu.gif?raw=true)

The menu offers all of the playable survivors you have available to you. You can select options 1 through 9 by pressing the number keys on the main row of your keyboard. Press 0 to toggle to the next page of survivors. (*You can remap these buttons in the settings menu via Risk Of Options.*) Once you've selected a Survivor, the cooldown will be cut down drastically for this first use.

![Demonstration of the item in action](https://github.com/adamhaertter/ROR-SoulLinkEquipment/blob/master/img/Demo_Transform.gif?raw=true)

Now that you've chosen your Survivor, you are bonded to that Survivor. On the next use of the equipment, you will transform into the character you chose, retaining your current damage, items, and curse (if you're playing Eclipse), but removing most buffs and debuffs. Using the equipment again will change you back into your original character. Have fun making some crazy strategies or finding ways to break the game with this!

Some extra information about the interactions
- Soul Links **cannot** be called randomly by Bottled Chaos.
- Soul Links *are* able to be called and reduced by Gesture of the Drowned.
- Soul Links *do* work with Fuel Cell's cooldown reduction, but transforming **will not** give you extra charges.
- Soul Links *will not remove* MUL-T's offhand equipment, but you can't use them unless you swap back into MUL-T.
- Scavengers and Equipment Drones *can* get and use the Soul Links. If you die to a wild Void Fiend, this is why.
- Modded Survivors *are* compatible. The menu pulls from all survivors you have available, so if you have a modded character unlocked, transform away!

## Configuration
This mod has Risk of Options support! Through either the in-game menu or the config file, you can customize the following features to your liking:
- **Item Tier** - Should this be a regular or Lunar equipment?
- **Equipment Cooldown** - You can make this as broken or balanced as you'd like.
- **Randomization** - Want to skip the UI phase? Let the game pick a random character for you!
    - **Heresy** - Should the game be allowed to give you Heretic (even if it's really funny)?
- **Keybinds** - Reconfigure all the keybinds for the custom menu to your specific liking.

## Issues

Have an issue with the mod? Find a bug? Did I accidentally break something in the game that I missed during testing? Let me know over on GitHub by [opening an issue](https://github.com/adamhaertter/ROR-SoulLinkEquipment/issues) so I can address it! Please include your log if possible :)

If you don't have GitHub, feel free to also leave a comment on the YouTube video linked above. I'll try to monitor both in the future!

### Known Bugs
- There are some potential alignment issues that may arise with the custom UI menu if you have not yet unlocked at least 4 characters. The menu is still fully functional, but some text might become misaligned in these cases.
- Limited use moves, like Captain's beacons, are still refreshed upon transforming back into your original charcter if they've all been used up. This should still remove your old beacons from the map, though.

## Credits    
- [ItemDisplayPlacementHelper by KingEnderBrine](https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/)
    - Seriously, I would not have done item displays without this mod's help.