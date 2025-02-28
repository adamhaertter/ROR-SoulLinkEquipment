using BepInEx;
using PrestigeItems.Items;
using SoulLink.Items;
using R2API;
using RoR2;
using SoulLink.Util;
using UnityEngine;
using SoulLink.UI;

namespace SoulLink
{
    // You don't need this if you're not using R2API in your plugin,
    // it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
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
        public const string PluginVersion = "0.0.0";

        private bool hudInitialized;

        public static PluginInfo SavedInfo { get; private set; }

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            SavedInfo = Info;

            Log.Init(Logger);
            AssetUtil.Init();

            Log.Debug($"Asset Bundle loaded from stream. (allegedly)");

            //// Initialize item classes
            //DevCube.Init();
            SoulLinkEquip.Init();
        }

        // The Update() method is run on every frame of the game.
        private void Update()
        {
            ManageCustomHUD();

            // These are debug controls. I'm disabling them during normal gameplay, but keeping so I can test.
            DebugSpawnEquipment(SoulLinkEquip.equipDef, KeyCode.F1);
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
    }
}
