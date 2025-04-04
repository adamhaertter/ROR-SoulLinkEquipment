using BepInEx;
using SoulLink.Items;
using R2API;
using RoR2;
using SoulLink.Util;
using UnityEngine;
using SoulLink.UI;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace SoulLink
{
    // You don't need this if you're not using R2API in your plugin,
    // it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class SoulLink : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "BlueB";
        public const string PluginName = "SoulLinkEquipment";
        public const string PluginVersion = "0.0.0"; // TODO Update version number when it's ready for a real release.

        private bool hudInitialized;

        private static ConfigEntry<KeyboardShortcut> uiOption1Key;
        private static ConfigEntry<KeyboardShortcut> uiOption2Key;
        private static ConfigEntry<KeyboardShortcut> uiOption3Key;
        private static ConfigEntry<KeyboardShortcut> uiOption4Key;
        private static ConfigEntry<KeyboardShortcut> uiOption5Key;
        private static ConfigEntry<KeyboardShortcut> uiOption6Key;
        private static ConfigEntry<KeyboardShortcut> uiOption7Key;
        private static ConfigEntry<KeyboardShortcut> uiOption8Key;
        private static ConfigEntry<KeyboardShortcut> uiOption9Key;
        private static ConfigEntry<KeyboardShortcut> uiPagingKey;
        private static ConfigEntry<bool> isLunarEquip;
        private static ConfigEntry<float> customCooldown;
        private static ConfigEntry<bool> useRandomBond;
        private static ConfigEntry<bool> allowHeretic;

        public static BepInEx.PluginInfo SavedInfo { get; private set; }

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            SavedInfo = Info;

            Log.Init(Logger);
            AssetUtil.Init();

            Log.Debug($"Asset Bundle loaded from stream. (allegedly)");

            InitializeConfigOptions();
            
            // Initialize item classes
            SoulLinkEquip.Init();
        }

        // The Update() method is run on every frame of the game.
        private void Update()
        {   
            // These are debug controls. I'm disabling them during normal gameplay, but keeping so I can test.
            //ManageCustomHUD();
            //DebugSpawnEquipment(SoulLinkEquip.equipDef, KeyCode.F1);
        }

        private void InitializeConfigOptions()
        {
            // Initialize Risk of Options Data
            ModSettingsManager.SetModDescription("This mod adds 1 configurable equipment to the game that lets the player transform between any 2 survivors. Change the menu keybinds, item tier, and tweak specific values here.");
            Sprite icon = AssetUtil.LoadSprite("SoulLinkModIcon.png");
            ModSettingsManager.SetModIcon(icon);    

            // Option Page -- Configuration
            string tabOptions = "Configuration";
            isLunarEquip = Config.Bind(new ConfigDefinition(tabOptions, "Make Lunar Equipment"), false, new ConfigDescription("Do you want the Soul Links to be a Lunar equipment? If this is enabled, the item tier will change to Lunar, so they will no longer spawn in Equipment Barrels. The functionality is the same.\n\nBy default, this is disabled, meaning they will be a standard orange Equipment. As a standard Equipment, Scavengers have a chance to spawn with this Equipment."));
            ModSettingsManager.AddOption(new CheckBoxOption(isLunarEquip, true)); // Because this happens in Awake(), we need a restart for the tier change to take effect.

            customCooldown = Config.Bind(new ConfigDefinition(tabOptions, "Equipment Cooldown"), 60f, new ConfigDescription("Set a custom cooldown (in seconds) for the Soul Links to make them more balanced (or more broken). Tweak away to your heart's content.\n\nBy default, this timer is set to 60, making the equipment useable once per minute."));
            ModSettingsManager.AddOption(new FloatFieldOption(customCooldown));
            customCooldown.SettingChanged += (obj, args) =>
            {
                if (customCooldown.Value >= 1) // Limit to 1+ because 0 lets you respawn before you even spawn in, causing infinite flight
                    SoulLinkEquip.equipDef.cooldown = customCooldown.Value; // Update cooldown in real time to avoid needing to restart.
                else
                    customCooldown.Value = 1;
            };

            useRandomBond = Config.Bind(new ConfigDefinition(tabOptions, "Bond to Random Survivor"), false, new ConfigDescription("If you don't want to navigate the UI menu or would just like some more randomness in your life, checking this box will make it so won't be able to pick your transform target. Instead, using the equipment will bond you to a random available survivor on your first use.\n\nNote: This is not the recommended way to use the mod, but I added it for a little extra fun. By default, this is false. Please note that this still links between two survivors -- your starting character and one randomly chosen on first use. This does not give a random survivor every roll."));
            ModSettingsManager.AddOption(new CheckBoxOption(useRandomBond));
            
            allowHeretic = Config.Bind(new ConfigDefinition(tabOptions, "Allow Heretic"), true, new ConfigDescription("By checking this box, you are allowing Heretic into the pool of selectable survivors. Heretic does not come with any of the four Heresy items, meaning all four of her abilities will be Nevermore (unless you have one of the Heresy items).\n\nBy default, this value is true because funny, but also because you can simply not choose her. You may want to disable this if you are using random selection."));
            ModSettingsManager.AddOption(new CheckBoxOption(allowHeretic));

            // Option Page -- Keybinds
            string tabKeybinds = "Keybinds";
            uiOption1Key = Config.Bind(new ConfigDefinition(tabKeybinds, "First Option Select"), new KeyboardShortcut(KeyCode.Alpha1), new ConfigDescription("The keybind used to select the first, or top-left, option in the Soul Links UI menu. By default this is Numrow 1.\n\nXOO\nOOO\nOOO"));
            uiOption2Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Second Option Select"), new KeyboardShortcut(KeyCode.Alpha2), new ConfigDescription("The keybind used to select the second, or top-center, option in the Soul Links UI menu. By default this is Numrow 2.\n\nOXO\nOOO\nOOO"));
            uiOption3Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Third Option Select"), new KeyboardShortcut(KeyCode.Alpha3), new ConfigDescription("The keybind used to select the third, or top-right, option in the Soul Links UI menu. By default this is Numrow 3.\n\nOOX\nOOO\nOOO"));
            uiOption4Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Fourth Option Select"), new KeyboardShortcut(KeyCode.Alpha4), new ConfigDescription("The keybind used to select the fourth, or middle-left, option in the Soul Links UI menu. By default this is Numrow 4.\n\nOOO\nXOO\nOOO"));
            uiOption5Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Fifth Option Select"), new KeyboardShortcut(KeyCode.Alpha5), new ConfigDescription("The keybind used to select the fifth, or middle-center, option in the Soul Links UI menu. By default this is Numrow 5.\n\nOOO\nOXO\nOOO"));
            uiOption6Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Sixth Option Select"), new KeyboardShortcut(KeyCode.Alpha6), new ConfigDescription("The keybind used to select the sixth, or middle-right, option in the Soul Links UI menu. By default this is Numrow 6.\n\nOOO\nOOX\nOOO"));
            uiOption7Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Seventh Option Select"), new KeyboardShortcut(KeyCode.Alpha7), new ConfigDescription("The keybind used to select the seventh, or bottom-left, option in the Soul Links UI menu. By default this is Numrow 7.\n\nOOO\nOOO\nXOO"));
            uiOption8Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Eighth Option Select"), new KeyboardShortcut(KeyCode.Alpha8), new ConfigDescription("The keybind used to select the eighth, or bottom-center, option in the Soul Links UI menu. By default this is Numrow 8.\n\nOOO\nOOO\nOXO"));
            uiOption9Key = Config.Bind(new ConfigDefinition(tabKeybinds, "Ninth Option Select"), new KeyboardShortcut(KeyCode.Alpha9), new ConfigDescription("The keybind used to select the ninth, or bottom-right, option in the Soul Links UI menu. By default this is Numrow 9.\n\nOOO\nOOO\nOOX"));
            uiPagingKey  = Config.Bind(new ConfigDefinition(tabKeybinds, "Page Toggle Key"), new KeyboardShortcut(KeyCode.Alpha0), new ConfigDescription("The keybind used to toggle the page selection in the Soul Links UI menu. By default this is Numrow 0."));

            ModSettingsManager.AddOption(new KeyBindOption(uiOption1Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption2Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption3Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption4Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption5Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption6Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption7Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption8Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiOption9Key));
            ModSettingsManager.AddOption(new KeyBindOption(uiPagingKey));
        }

        private void ManageCustomHUD()
        {
            if (Application.isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    SoulLinkPanel.Toggle();
                }

                if (!hudInitialized)
                {
                    hudInitialized = true;
                    var hud = GameObject.Find("HUDSimple(Clone)");
                    Transform mainContainer = hud.transform.Find("MainContainer");
                    Transform mainUIArea = mainContainer.Find("MainUIArea");
                    mainUIArea.gameObject.SetActive(true);

                    var springCanvas = mainUIArea.Find("SpringCanvas");
                    Transform screenLocation = springCanvas.Find("UpperRightCluster");

                    SoulLinkPanel myUI = SoulLinkPanel.CreateUI(screenLocation);
                    Instantiate(myUI, screenLocation);

                    SoulLinkPanel.Show();
                }
            }
        }

        private void DebugSpawnItem(ItemDef itemDef, KeyCode keyTrigger)
        {
            if (Input.GetKeyDown(keyTrigger))
            {
                // Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                // And then drop our defined item in front of the player.
                Log.Info($"Player pressed {keyTrigger}. Spawning our custom item {itemDef.name} at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(itemDef.itemIndex), transform.position, transform.forward * 20f);
            }
        }
        
        private void DebugSpawnEquipment(EquipmentDef equipDef, KeyCode keyTrigger)
        {
            if (Input.GetKeyDown(keyTrigger))
            {
                // Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                // And then drop our defined item in front of the player.
                Log.Info($"Player pressed {keyTrigger}. Spawning our custom equipment {equipDef.name} at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex), transform.position, transform.forward * 20f);
            }
        }

        // Risk of Options public access checks from here to EOF:
        public static KeyCode GetPagingKey() 
        { 
            return uiPagingKey.Value.MainKey; 
        }

        public static KeyCode[] GetOptionKeys()
        {
            return [
                uiOption1Key.Value.MainKey, uiOption2Key.Value.MainKey, uiOption3Key.Value.MainKey,
                uiOption4Key.Value.MainKey, uiOption5Key.Value.MainKey, uiOption6Key.Value.MainKey,
                uiOption7Key.Value.MainKey, uiOption8Key.Value.MainKey, uiOption9Key.Value.MainKey
                ];
        }

        public static bool IsConfigLunar()
        {
            return isLunarEquip.Value;
        }

        public static float GetCustomCooldown()
        {
            return customCooldown.Value;
        }

        public static bool IsRandomBond()
        {
            return useRandomBond.Value;
        }

        public static bool IsHereticAllowed()
        {
            return allowHeretic.Value;
        }
    }
}
