using SoulLink.Util;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using static RoR2.Items.BaseItemBodyBehavior;
using SoulLink.UI;
using UnityEngine.EventSystems;
using RoR2.UI;
using Rewired;
using static UnityEngine.ParticleSystem.PlaybackState;
using UnityEngine.UIElements.Experimental;
using UnityEngine.XR;
using static RoR2.MasterSpawnSlotController;
using UnityEngine.Networking;

namespace SoulLink.Items
{
    internal class SoulLinkEquip
    {
        public static EquipmentDef equipDef;
        private static String itemId = "SoulLink";
        private static SurvivorDef[] validTransformTargets;

        internal static void Init()
        {
            GenerateItem();
            AddTokens();

            var displayRules = new ItemDisplayRuleDict(null);
            ItemAPI.Add(new CustomEquipment(equipDef, displayRules));

            Hooks();
        }

        // Defining the item
        private static void GenerateItem()
        {
            equipDef = ScriptableObject.CreateInstance<EquipmentDef>();

            itemId = itemId.Replace(" ", "").ToUpper(); // Validate that itemId is in all caps, no spaces.

            equipDef.name = itemId;
            equipDef.nameToken = itemId + "_NAME";
            equipDef.pickupToken = itemId + "_PICKUP";
            equipDef.descriptionToken = itemId + "_DESCRIPTION";
            equipDef.loreToken = itemId + "_LORE";

            equipDef.pickupIconSprite = AssetUtil.LoadSprite("SoulLinkIcon.png");
            equipDef.pickupModelPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab");

            equipDef.canBeRandomlyTriggered = false;
            equipDef.canDrop = true;
            equipDef.cooldown = 60f;
        }

        // The game logic for the item's functionality goes in this method
        public static void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, body) =>
            {
                // TODO Might have to remove this, not entirely sure but leaning towards yes.
                if (body.GetComponent<SoulLinkEquipBehavior>() == null)
                {
                    body.AddItemBehavior<SoulLinkEquipBehavior>(body.inventory.GetEquipment(body.inventory.activeEquipmentSlot).equipmentDef == equipDef ? 1 : 0);
                    Log.Debug($"Added SoulLinkEquipBehavior to {body.name}");
                } else
                {
                    Log.Debug($"Bypassed adding SoulLinkEquipBehavior to {body.name}");
                }
            };

            // Write item functionality. Start with the trigger, then write the rest. See other item implementations for examples.
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, slot, currentEquipDef) =>
            {
                Log.Debug("SoulLinkEquip: Action performed.");
                if (currentEquipDef == equipDef)
                {

                    if (!slot.characterBody)
                    {
                        return false;
                    }

                    // Initialize item behavioral instance
                    var myBehavior = slot.characterBody.GetComponent<SoulLinkEquipBehavior>();

                    if(myBehavior)
                    {
                        if (myBehavior.TransformTargetOptions == null)
                        {
                            SearchForSurvivorDefs();
                            myBehavior.TransformTargetOptions = validTransformTargets;
                            Log.Debug("TransformTargetOptions initialized on myBehavior.");
                        }
                        myBehavior.activated = true; 
                        Log.Debug("SoulLinkEquip: Behavior activated set to true.");
                    }

                    return true;

                }
                else
                {
                    return orig(slot, currentEquipDef);
                }
            };

        }

        // String definitions / key lookup
        private static void AddTokens()
        {
            LanguageAPI.Add(itemId + "", "Soul Links");
            LanguageAPI.Add(itemId + "_NAME", "Soul Links");
            LanguageAPI.Add(itemId + "_PICKUP", "Transform into another Survivor at will.");
            LanguageAPI.Add(itemId + "_DESCRIPTION", "<style=cHumanObjective>On use,</style> become a different survivor. <style=cHumanObjective>The first use</style> after pickup will select the survivor to bond to, then further uses will trigger <style=cArtifact>the transformation.</style>");

            string lore = "Order: Soul Links" +
                            "\nTracking Number: 888 * ****" +
                            "\nEstimated Delivery: 10 / 04 / 2099" +
                            "\nShipping Method: Priority" +
                            "\nShipping Address: Pacific Proposal Park, Sole United America, Earth" +
                            "\nShipping Details:" +
                            "\n" +
                            "\nA rather symbollic gesture, but perhaps it will be a welcome one nonetheless. Excavation sites found these two rings forged entirely linked within one another. They are inseparable, but God knows we've tried to break it up. They seem to emit a strange, almost imperceptible, energy reading when pulled tightly together so the inner rings clash. Maybe your establishment can find a use for them." +
                            "\n" +
                            "\nIf you don't happen to hear from me, I'll be taking some time away from the lab. Lately, I just haven't been feeling quite like myself." +
                            "\n" +
                            "\nRegards," +
                            "\nDr.Robinson...?";
            LanguageAPI.Add(itemId + "_LORE", lore);
        }
        private static void SearchForSurvivorDefs()
        {
            //validTransformTargets = SurvivorCatalog.survivorDefs; // TODO change back, checking if my search conditions are bad.

            validTransformTargets = SurvivorCatalog.orderedSurvivorDefs.Where(survivorDef =>
                                                                             SurvivorCatalog.SurvivorIsUnlockedOnThisClient(survivorDef.survivorIndex) &&
                                                                             survivorDef.CheckRequiredExpansionEnabled() && // Avoid character piracy lol
                                                                             survivorDef.CheckUserHasRequiredEntitlement(((MPEventSystem)EventSystem.current).localUser)).ToArray();

            Log.Debug($"validTransformTargets set with {validTransformTargets.Length} as its length");
        }

        public class SoulLinkEquipBehavior : CharacterBody.ItemBehavior
        {
            [ItemDefAssociation(useOnServer = true, useOnClient = false)]
            public static EquipmentDef GetEquipDef() => equipDef;
            public bool activated = false;
            public bool firstTimeUse = true;
            public SurvivorDef chosenSurvivorTarget;
            public SurvivorDef[] TransformTargetOptions {  get; set; }

            private SoulLinkPanel menu;
            private bool wasJustRespawned = false;
            private float healthBeforeTransform;
            private float curseBeforeTransform;

            private void onEnable()
            {
                //Log.Debug($"onEnable entered for PrestigeFungus");

            }

            private void Start()
            {
               
            }

            // FixedUpdate runs on game tick update
            private void FixedUpdate()
            {
                if (activated)
                {
                    if (firstTimeUse)
                    {
                        // Prompt the user to pick a target
                        Log.Debug("SoulLinkEquip: First time use!");
                        if(menu == null)
                        {
                            var hud = GameObject.Find("HUDSimple(Clone)");
                            Transform mainContainer = hud.transform.Find("MainContainer");
                            Transform mainUIArea = mainContainer.Find("MainUIArea");
                            mainUIArea.gameObject.SetActive(true);

                            //var springCanvas = mainUIArea.Find("SpringCanvas");
                            //Transform screenLocation = springCanvas.Find("UpperRightCluster");

                            menu = SoulLinkPanel.CreateUI(mainUIArea);
                            //Instantiate(menu, mainUIArea);

                            menu.optionCatalogue = GetTargetImages(TransformTargetOptions);
                            Log.Debug($"optionCatalogue set in FixedUpdate. TransformTargetOptions.Length {TransformTargetOptions.Length}, optionCatalogue.Length {menu.optionCatalogue.Length}");
                            menu.Render();
                            //SoulLinkPanel.Show();
                            Log.Debug("Menu Initialized in Behavior");
                        }
                        //SoulLinkPanel.Toggle();
                        firstTimeUse = false;
                    }
                    else
                    {
                        // Transform the user into the target
                        Log.Debug("SoulLinkEquip: NOT first time use!");
                        if (chosenSurvivorTarget)
                        {
                            SurvivorDef originalBody = SurvivorCatalog.GetSurvivorDef(SurvivorCatalog.GetSurvivorIndexFromBodyIndex(this.body.bodyIndex));
                            this.body.master.bodyPrefab = chosenSurvivorTarget.bodyPrefab;
                            var newBody = this.body.master.Respawn(this.body.master.GetBody().transform.position + new Vector3(0f, 5f, 0f), body.master.GetBody().transform.rotation);

                            var newBehavior = newBody.AddItemBehavior<SoulLinkEquipBehavior>(1);
                            newBehavior.firstTimeUse = false;
                            newBehavior.chosenSurvivorTarget = originalBody;
                            newBehavior.wasJustRespawned = true;
                            newBehavior.healthBeforeTransform = body.healthComponent.health;
                            newBehavior.curseBeforeTransform = body.cursePenalty;

                            Log.Debug("Values set before respawning...");
                            Log.Debug("Transforming!");
                        } else
                        {
                            firstTimeUse = true;
                        }
                    }
                    activated = false;

                }
                if(menu && menu.selectedOptionIndex >= 0 && chosenSurvivorTarget == null)
                {
                    var selectedIndex = (menu.currentPage * 9) + menu.selectedOptionIndex;
                    chosenSurvivorTarget = TransformTargetOptions[selectedIndex];
                    Log.Debug($"chosenSurvivorTarget picked! selectedIndex {selectedIndex}, menu.selectedOptionIndex {menu.selectedOptionIndex}, chosenSurvivorTarget.cachedName {chosenSurvivorTarget.cachedName}");
                    Destroy(menu.gameObject); // Only destroy the window once we have our answer.
                    body.inventory.DeductActiveEquipmentCooldown(equipDef.cooldown * 3 / 4);
                }
                // Deal damage and apply curse on transformation
                // I have to do this here rather than in the transformation block because I have to let the game recalculate the health portions for the new body first.
                if (wasJustRespawned && NetworkServer.active)
                {
                    var damageInfo = new DamageInfo
                    {
                        position = transform.position,
                        damage = body.healthComponent.combinedHealth - healthBeforeTransform,
                        procCoefficient = 0f,
                        damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock,
                        damageColorIndex = DamageColorIndex.Luminous,
                        attacker = null
                    };

                    body.healthComponent.TakeDamage(damageInfo);
                    body.cursePenalty = curseBeforeTransform;
                    wasJustRespawned = false;
                }
            }

        }

        private static Sprite[] GetTargetImages(SurvivorDef[] survivorDefs) // I know this is a horrible reference but I am exhausted!!
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (SurvivorDef def in survivorDefs)
            {
                Texture tex = SurvivorCatalog.GetSurvivorPortrait(def.survivorIndex);
                sprites.Add(SoulLinkPanel.ConvertTextureToSprite(tex as Texture2D));
            }
            Log.Debug($"GetTargetImages: Returning {sprites.Count} sprites from {survivorDefs.Length} survivorDefs");
            return sprites.ToArray();
        }
    }
}
