﻿using SoulLink.Util;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static RoR2.Items.BaseItemBodyBehavior;
using SoulLink.UI;
using UnityEngine.EventSystems;
using RoR2.UI;
using UnityEngine.Networking;

namespace SoulLink.Items
{
    internal class SoulLinkEquip
    {
        public static EquipmentDef equipDef;
        private static string itemId = "SoulLink";
        private static SurvivorDef[] validTransformTargets;
        private readonly static string chatMessage = "<style=cHumanObjective>{user}</style><style=cEvent> has bound their soul to </style><style=cWorldEvent>{target}</style>";
        private readonly static string[] buffsToTransfer = ["bdPermanentCurse", "bdPermanentDebuff", "bdSoulCost", "bdExtraLifeBuff", "bdFreeUnlock", "bdBanditSkull", "bdChakraBuff", "bdRevitalizeBuff"];

        internal static void Init()
        {
            GenerateItem();
            AddTokens();

            var displayRules = CreateDisplayRules();
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

            equipDef.pickupModelPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab");

            equipDef.canBeRandomlyTriggered = false;
            equipDef.canDrop = true;
            equipDef.cooldown = SoulLink.GetCustomCooldown();
            
            if (SoulLink.IsConfigLunar())
            {
                equipDef.pickupIconSprite = AssetUtil.LoadSprite("SoulLinkIcon_Lunar.png");
                equipDef.isLunar = true;
                equipDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            }
            else
            {
                equipDef.pickupIconSprite = AssetUtil.LoadSprite("SoulLinkIcon.png");
                equipDef.isLunar = false;
                equipDef.colorIndex = ColorCatalog.ColorIndex.Equipment;
            }
        }

        private static ItemDisplayRuleDict CreateDisplayRules()
        {
            var displays = new ItemDisplayRuleDict();

            displays.Add("EquipmentDroneBody", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "GunBarrelBase",
                    localPos = new Vector3(-.2F, 0F, 2.1F),
                    localAngles = new Vector3(90F, 0F, 0F),
                    localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });

            displays.Add("mdlCommandoDualies", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Pelvis",
                    localPos = new Vector3(0.15965F, -0.10945F, -0.13692F),
                    localAngles = new Vector3(314.421F, 130.9908F, 201.2805F),
                    localScale = new Vector3(0.06134F, 0.03805F, 0.06134F)

                }
            });
            displays.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "BowHinge1L",
                    localPos = new Vector3(0.16519F, 0.04391F, 0.04425F),
                    localAngles = new Vector3(344.6824F, 320.9641F, 240.9161F),
                    localScale = new Vector3(0.06956F, 0.06956F, 0.06956F)

                }
            });
            displays.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Stomach",
                    localPos = new Vector3(0.21794F, 0.03553F, 0.09086F),
                    localAngles = new Vector3(11.73269F, 130.9432F, 323.907F),
                    localScale = new Vector3(0.06571F, 0.06571F, 0.06571F)
                }
            });
            displays.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Neck",
                    localPos = new Vector3(0.21707F, 1.58903F, 0.35364F),
                    localAngles = new Vector3(346.6717F, 315.2442F, 295.8514F),
                    localScale = new Vector3(0.59539F, 0.59539F, 0.59539F)
                }
            });
            displays.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Chest",
                    localPos = new Vector3(0.02309F, -0.11628F, -0.29646F),
                    localAngles = new Vector3(343.5479F, 128.2443F, 32.49729F),
                    localScale = new Vector3(0.08699F, 0.08699F, 0.08699F)
                }
            });
            displays.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "UpperArmR",
                    localPos = new Vector3(-0.12986F, 0.1991F, -0.04129F),
                    localAngles = new Vector3(347.4306F, 345.6252F, 85.64943F),
                    localScale = new Vector3(0.08238F, 0.08238F, 0.08238F)
                }
            });
            displays.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "HandL",
                    localPos = new Vector3(-0.09109F, 0.0813F, -0.03169F),
                    localAngles = new Vector3(21.70813F, 323.4272F, 49.0989F),
                    localScale = new Vector3(0.08252F, 0.08394F, 0.07476F)
                }
            });
            displays.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "WeaponPlatform",
                    localPos = new Vector3(0.26516F, -0.35155F, 0.48567F),
                    localAngles = new Vector3(348.9675F, 351.5085F, 73.60771F),
                    localScale = new Vector3(0.24464F, 0.13364F, 0.13364F)
                }
            });
            displays.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Chest",
                    localPos = new Vector3(0.35216F, 0.49106F, 0.23169F),
                    localAngles = new Vector3(350.0082F, 312.9914F, 285.678F),
                    localScale = new Vector3(0.08396F, 0.06583F, 0.08396F)
                }
            });
            displays.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Head",
                    localPos = new Vector3(-2.05468F, 1.85249F, -3.67751F),
                    localAngles = new Vector3(15.07537F, 324.9648F, 73.08989F),
                    localScale = new Vector3(-1.77162F, 2.20423F, 2.53584F)
                }
            });
            displays.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "HandL",
                    localPos = new Vector3(-0.05515F, -0.11752F, -0.01267F),
                    localAngles = new Vector3(290.4974F, 312.2321F, 331.4503F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            // Removed here: Heretic. She doesn't get item displays in the base game.
            displays.Add("mdlRailGunner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "GunRoot",
                    localPos = new Vector3(-0.01791F, 0.00001F, -0.36122F),
                    localAngles = new Vector3(0.00006F, 78.96882F, 271.9931F),
                    localScale = new Vector3(0.05038F, 0.05038F, 0.05038F)
                }
            });
            displays.Add("mdlVoidSurvivor", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Hand",
                    localPos = new Vector3(-0.01978F, -0.03643F, -0.01855F),
                    localAngles = new Vector3(55.10281F, 68.31735F, 76.12302F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            displays.Add("mdlSeeker", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Pack",
                    localPos = new Vector3(0.08363F, 0.17586F, -0.34696F),
                    localAngles = new Vector3(347.5371F, 236.0009F, 99.67917F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            displays.Add("mdlFalseSon", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.00107F, 0.06443F, -0.04812F),
                    localAngles = new Vector3(285.4764F, 342.4372F, 287.4732F),
                    localScale = new Vector3(0.20777F, 0.11744F, 0.14546F)
                }
            });
            // I may or may not have phoned this one in HARD. Enjoy his "gold necklace" lmao
            displays.Add("mdlChef", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "Head",
                    localPos = new Vector3(0.03178F, 0.16548F, 0.25901F),
                    localAngles = new Vector3(327.3342F, 168.3224F, 3.33605F),
                    localScale = new Vector3(0.11638F, 0.17782F, 0.22501F)

                }
            });
            displays.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = AssetUtil.LoadModel("SoulLinkModel.prefab"),
                    childName = "MuzzleEnergyCannon",
                    localPos = new Vector3(7.05081F, 3.682F, -5.10893F),
                    localAngles = new Vector3(352.1646F, 347.9308F, 280.7483F),
                    localScale = new Vector3(2.4235F, 1.67428F, 2.85059F)
                }
            });

            return displays;
        }

        // The game logic for the item's functionality goes in this method
        public static void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, body) =>
            {
                orig(body);
                if (body.GetComponent<SoulLinkEquipBehavior>() == null)
                {
                    body.AddItemBehavior<SoulLinkEquipBehavior>(body.inventory.GetEquipment(body.inventory.activeEquipmentSlot).equipmentDef == equipDef ? 1 : 0);
                    //Log.Debug($"Added SoulLinkEquipBehavior to {body.name}");
                } else
                { 
                    // Check if the equipment is held in any possible slots (thanks MUL-T)
                    bool foundMyEquipment = false;
                    foreach (var equipState in body.inventory.equipmentStateSlots)
                    {
                        if(equipState.equipmentDef == equipDef)
                        {
                            // If you have this equipment in one of your slots.
                           foundMyEquipment = true;
                            break;
                        }
                    }
                    if (!foundMyEquipment)
                    {
                        // If a body no longer has this equipment but has had it before, we want to reset the params.
                        //Log.Debug($"{body.name} no longer has Soul Links, resetting behavior to first time use state.");
                        var behavior = body.GetComponent<SoulLinkEquipBehavior>();
                        behavior.firstTimeUse = true;
                        behavior.chosenBodyTarget = null; // Nullify this so that the menu can reassign a new character.
                    }
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
                            //Log.Debug("TransformTargetOptions initialized on myBehavior.");
                        }
                        myBehavior.activated = true; 
                        //Log.Debug("SoulLinkEquip: Behavior activated set to true.");
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
            validTransformTargets = SurvivorCatalog.orderedSurvivorDefs.Where(survivorDef =>
                                                                             SurvivorCatalog.SurvivorIsUnlockedOnThisClient(survivorDef.survivorIndex) &&
                                                                             survivorDef.CheckRequiredExpansionEnabled() && // Avoid character piracy lol
                                                                             (SoulLink.IsHereticAllowed() || survivorDef.bodyPrefab.name != "HereticBody") &&
                                                                             survivorDef.CheckUserHasRequiredEntitlement(((MPEventSystem)EventSystem.current).localUser)).ToArray();

            Log.Debug($"validTransformTargets set with {validTransformTargets.Length} as its length");
        }

        private static void BroadcastBondedChatMessage(CharacterBody userBody, CharacterBody targetBody)
        {
            String customChatMsg = chatMessage.Replace("{target}", targetBody.GetDisplayName());
            // Apply coloring to the user's name based on team affiliation - should make it easier to see if this is a friendly or dangerous message in a pinch.
            switch (userBody.master.teamIndex)
            {
                case TeamIndex.Monster:
                    customChatMsg = customChatMsg.Replace("cHumanObjective", "cDeath");
                    break;
                case TeamIndex.Void:
                    customChatMsg = customChatMsg.Replace("cHumanObjective", "cIsVoid");
                    break;
                case TeamIndex.Lunar:
                    customChatMsg = customChatMsg.Replace("cHumanObjective", "cLunarObjective");
                    break;
                case TeamIndex.Neutral:
                    customChatMsg = customChatMsg.Replace("cHumanObjective", "cArtifact");
                    break;
                case TeamIndex.Player:
                    if (!userBody.isPlayerControlled) // If it's on the player's team but not the player, it's probably an equipment drone. Bad assumption if you're using Engi Turret Equip mod but whatever they can be orange too.
                        customChatMsg = customChatMsg.Replace("<style=cHumanObjective>{user}</style>", $"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Equipment)}>" + "{user}</color>");
                    break;
                // No need for a default case since the style cHumanObjective is written into the original string. Do not remove that or you'll get white text!
            }
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = customChatMsg.Replace("{user}", userBody.isPlayerControlled ? userBody.master.playerCharacterMasterController.networkUser.userName : userBody.GetDisplayName()) });
        }

        public class SoulLinkEquipBehavior : CharacterBody.ItemBehavior
        {
            [ItemDefAssociation(useOnServer = true, useOnClient = false)]
            public static EquipmentDef GetEquipDef() => equipDef;
            public bool activated = false;
            public bool firstTimeUse = true;
            public CharacterBody chosenBodyTarget; 
            public SurvivorDef[] TransformTargetOptions {  get; set; }

            private SoulLinkPanel menu;
            private bool wasJustRespawned = false;
            private float healthBeforeTransform;
            //private float curseBeforeTransform; // Disabling in favor of transferring curse via the debuffs
            private List<(BuffIndex, int)> starterBuffs;

            private void onEnable()
            {

            }

            private void Start()
            {
               
            }

            // FixedUpdate runs on game tick update
            private void FixedUpdate()
            {
                if (activated)
                {
                    if (chosenBodyTarget == null && (SoulLink.IsRandomBond() || !body.isPlayerControlled))
                    {
                        // Manual override so if an AI body (Scavenger, Equip Drone, etc.) picks this up, they won't have to menu.
                        firstTimeUse = false;
                        chosenBodyTarget = GetBodyFromSurvivorDef(TransformTargetOptions[UnityEngine.Random.RandomRangeInt(0, TransformTargetOptions.Length)]);
                        //Log.Debug($"AI user {body.name} has been assigned {chosenBodyTarget.name} as their Link");
                        BroadcastBondedChatMessage(body, chosenBodyTarget);
                    }

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

                            menu = SoulLinkPanel.CreateUI(mainUIArea);

                            menu.optionCatalogue = GetTargetImages(TransformTargetOptions);
                            //Log.Debug($"optionCatalogue set in FixedUpdate. TransformTargetOptions.Length {TransformTargetOptions.Length}, optionCatalogue.Length {menu.optionCatalogue.Length}");
                            menu.Render();
                            Log.Debug("Menu Initialized in Behavior");
                        }
                        firstTimeUse = false;
                    }
                    else
                    {
                        // Transform the user into the target
                        Log.Debug("SoulLinkEquip: NOT first time use!");
                        if (chosenBodyTarget)

                        {
                            CharacterBody originalBody = BodyCatalog.bodyPrefabBodyComponents.Where(searchBody => searchBody.bodyIndex == body.bodyIndex).FirstOrDefault();
                            this.body.master.bodyPrefab = BodyCatalog.GetBodyPrefab(chosenBodyTarget.bodyIndex);
                            var newBody = this.body.master.Respawn(this.body.master.GetBody().transform.position + new Vector3(0f, 5f, 0f), body.master.GetBody().transform.rotation);
                            var newBehavior = newBody.AddItemBehavior<SoulLinkEquipBehavior>(1);
                            newBehavior.firstTimeUse = false;
                            newBehavior.chosenBodyTarget = originalBody;
                            newBehavior.wasJustRespawned = true;
                            newBehavior.healthBeforeTransform = body.healthComponent.health;
                            //newBehavior.curseBeforeTransform = body.cursePenalty;

                            var buffsToReapply = new List<(BuffIndex, int)>();
                            foreach(var buffIndex in body.activeBuffsList)
                            {
                                var buff = BuffCatalog.GetBuffDef(buffIndex);
                                //Log.Debug($"Body {body.name} has buff {buff.name} in activeBuffsList");
                                if (buffsToTransfer.Contains(buff.name))
                                {
                                    buffsToReapply.Add((buff.buffIndex, body.GetBuffCount(buff)));
                                }

                            }
                            newBehavior.starterBuffs = buffsToReapply;
                            
                            Log.Debug($"Transforming! originalBody: {originalBody.name}, newBody: {newBody.name}");
                        } else
                        {
                            Log.Debug($"No chosenBodyTarget detected on current body {body.name}");
                            firstTimeUse = true;
                        }
                    }
                    activated = false;

                }
                // Assign the selected character once it is chosen in the menu
                if(menu && menu.selectedOptionIndex >= 0 && chosenBodyTarget == null)
                {
                    var selectedIndex = (menu.currentPage * 9) + menu.selectedOptionIndex;
                    chosenBodyTarget = GetBodyFromSurvivorDef(TransformTargetOptions[selectedIndex]); 
                    Log.Debug($"chosenBodyTarget picked! selectedIndex {selectedIndex}, menu.selectedOptionIndex {menu.selectedOptionIndex}, chosenBodyTarget.name {chosenBodyTarget.name}");
                    Destroy(menu.gameObject); // Only destroy the window once we have our answer. 
                    body.inventory.DeductActiveEquipmentCooldown(equipDef.cooldown * 3 / 4);
                    BroadcastBondedChatMessage(body, chosenBodyTarget);
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
                    //body.cursePenalty = curseBeforeTransform; 

                    /* NOTE TO SELF:
                     * In transferring curse via the debuff system instead of on the health component, 
                     * you now take extra curse on each transform because of the initial hit for equalizing damage.
                     * 
                     * I kind of like this stylistically actually, but it could become too punishing in practice.
                     * If I want to change this later, I'll just have to find this massive block of text so I know
                     * to remove all curse debuffs before I start adding them right here.
                     */
                    foreach(var buff in starterBuffs)
                    {
                        for(int i = 0; i < buff.Item2; i++)
                        {
                            // Add as many noteworthy buffs as the user had before transforming.
                            body.AddBuff(BuffCatalog.GetBuffDef(buff.Item1));
                        }
                    }
                    wasJustRespawned = false;
                }
            }

            // Retrieves the CharacterBody when given a SurvivorDef, part of remapping from chosenSurvivorTarget : SurvivorDef to chosenBodyTarget : CharacterBody
            private static CharacterBody GetBodyFromSurvivorDef(SurvivorDef survivorDef)
            {
                return BodyCatalog.bodyPrefabBodyComponents.Where(bodyDef => 
                    bodyDef.bodyIndex == SurvivorCatalog.GetBodyIndexFromSurvivorIndex(survivorDef.survivorIndex)
                    ).ToArray().FirstOrDefault();
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
