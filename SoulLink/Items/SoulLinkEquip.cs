using SoulLink.Util;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using static RoR2.Items.BaseItemBodyBehavior;

namespace SoulLink.Items
{
    internal class SoulLinkEquip
    {
        public static EquipmentDef equipDef;
        private static String itemId = "SoulLink";
        private bool isFirstUse = true; //TODO This is gonna have to become a body behavior.
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

            // TODO Load your assets
            equipDef.pickupIconSprite = AssetUtil.LoadSprite("");
            equipDef.pickupModelPrefab = AssetUtil.LoadModel("");

            equipDef.canBeRandomlyTriggered = true;
            equipDef.canDrop = true;
            equipDef.cooldown = 60f;

            // TODO Initialize valid list of transformable survivors
            validTransformTargets = SurvivorCatalog.survivorDefs;
        }

        // The game logic for the item's functionality goes in this method
        public static void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, body) =>
            {
                body.AddItemBehavior<SoulLinkEquipBehavior>(body.inventory.GetEquipment(body.inventory.activeEquipmentSlot).equipmentDef == equipDef ? 1 : 0);
                Log.Debug($"Added SoulLinkEquipBehavior to {body.name}");
            };

            // TODO Write item functionality. Start with the trigger, then write the rest. See other item implementations for examples.
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
            LanguageAPI.Add(itemId + "", "Soul Link");
            LanguageAPI.Add(itemId + "_NAME", "Soul Link");
            LanguageAPI.Add(itemId + "_PICKUP", "<style=cLunarObjective>Transform</style> into another Survivor <style=cHumanObjective>at will.</style>");
            LanguageAPI.Add(itemId + "_DESCRIPTION", "<style=cHumanObjective>On use,</style> become a different survivor. <style=cHumanObjective>The first use</style> after pickup will select the survivor to bond to, then further uses will trigger <style=cLunarObjective>the transformation.</style>");

            string lore = "Lore Text"; //TODO Write your lore text here to be shown in the logbook.
            LanguageAPI.Add(itemId + "_LORE", lore);
        }

        public class SoulLinkEquipBehavior : CharacterBody.ItemBehavior
        {
            [ItemDefAssociation(useOnServer = true, useOnClient = false)]
            public static EquipmentDef GetEquipDef() => equipDef;
            public bool activated = false;
            private bool firstTimeUse = true;
            private SurvivorDef chosenSurvivorTarget;
            private SurvivorDef[] TransformTargetOptions {  get; set; }

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
                        // TODO Prompt the user to pick a target
                        Log.Debug("SoulLinkEquip: First time use!");
                        firstTimeUse = false;
                    }
                    else
                    {
                        // Transform the user into the target
                        Log.Debug("SoulLinkEquip: NOT first time use!");
                        if (chosenSurvivorTarget)
                        {
                            this.body.master.bodyPrefab = chosenSurvivorTarget.bodyPrefab;
                            this.body.master.Respawn(this.body.master.GetBody().transform.position + new Vector3(0f, 10f, 0f), body.master.GetBody().transform.rotation);
                        } else
                        {
                            firstTimeUse = true;
                        }
                    }
                    activated = false;

                }
            }

        }


    }
}
