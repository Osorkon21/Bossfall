// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net), Hazelnut, Numidium
// Contributors:    vanilla DFU code Allofich, Hazelnut, Lypyl (lypyl@dfworkshop.net), Numidium, TheLacus
// 
// Notes: This script uses code from several vanilla scripts. Comments indicate authorship, please verify
//        authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using BossfallMod.EnemyAI;
using BossfallMod.Items;
using BossfallMod.Utility;
using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Save;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BossfallMod.Events
{
    /// <summary>
    /// Bossfall event handlers and associated data.
    /// </summary>
    public class BossfallEventHandlers : MonoBehaviour
    {
        #region Fields

        // Spell lists for BossfallOnEnemyLootSpawned event handler. Based on arrays from vanilla's EnemyEntity script.
        readonly byte[] BossfallFrostDaedraSpells = new byte[] { 0x10, 0x14, 0x03 };
        readonly byte[] BossfallFireDaedraSpells = new byte[] { 0x0E, 0x19, 0x20 };
        readonly byte[] BossfallGenericSpells = new byte[] { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17,
            0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };

        // Used to restore my custom shields.
        ItemData_v1 itemData = new ItemData_v1();

        // Used if player is creating a custom character.
        const int bossfallDefaultHpPerLevel = 20;
        CreateCharCustomClass customClassWindow;
        CreateCharSpecialAdvantageWindow advantageWindow;
        DFCareer createdClass;
        TextLabel hpLabel;
        Button hitPointsUpButton;
        Button hitPointsDownButton;
        Panel daggerPanel;
        FieldInfo difficultyPoints;
        FieldInfo advantageAdjust;
        FieldInfo disadvantageAdjust;
        DaggerfallListPickerWindow primaryPicker;
        DaggerfallListPickerWindow secondaryPicker;
        TextLabel[] advantageLabels;
        bool customClassSetupComplete;
        bool advDisSetupComplete;

        // True if BossfallPlayerActivate component has been attached to the PlayerObject.
        bool bossfallPlayerActivateAdded;

        // This Dictionary is based on vanilla DFU's difficultyDict from the CreateCharSpecialAdvantageWindow script and
        // overwrites its vanilla counterpart during custom character creation. A super-powered custom character is highly
        // recommended to succeed in Bossfall. I rebalanced costs with that in mind. Most advantages are cheaper and most
        // disadvantages drop the difficulty dagger much more.
        readonly Dictionary <string, int> bossfallDifficultyDict = new Dictionary<string, int>
        {
            { HardStrings.acuteHearing, 1 },

            // Reduced from vanilla's 4.
            { HardStrings.adrenalineRush, 1 },
            { HardStrings.athleticism, 2 },

            // Reduced from vanilla's 6.
            { HardStrings.bonusToHit + HardStrings.animals, 3 },

            { HardStrings.bonusToHit + HardStrings.daedra, 3 },

            // Reduced from vanilla's 6.
            { HardStrings.bonusToHit + HardStrings.humanoid, 3 },
            { HardStrings.bonusToHit + HardStrings.undead, 3 },

            { HardStrings.expertiseIn, 2 },

            // Increased from vanilla's 10. I think too many immunities make the game too easy.
            { HardStrings.immunity, 28 },

            { HardStrings.increasedMagery + HardStrings.intInSpellPoints, 2 },

            // Slightly increased from vanilla's 4.
            { HardStrings.increasedMagery + HardStrings.intInSpellPoints15, 5 },

            { HardStrings.increasedMagery + HardStrings.intInSpellPoints175, 6 },

            // Slightly reduced from vanilla's 8.
            { HardStrings.increasedMagery + HardStrings.intInSpellPoints2, 7},

            { HardStrings.increasedMagery + HardStrings.intInSpellPoints3, 10 },
            { HardStrings.rapidHealing + HardStrings.general, 4 },
            { HardStrings.rapidHealing + HardStrings.inDarkness, 3 },
            { HardStrings.rapidHealing + HardStrings.inLight, 2 },

            // Reduced from vanilla's 14. Doesn't do much at higher HP.
            { HardStrings.regenerateHealth + HardStrings.general, 6 },

            // Reduced from vanilla's 10. Doesn't do much at higher HP.
            { HardStrings.regenerateHealth + HardStrings.inDarkness, 5 },

            // Reduced from vanilla's 6. Doesn't do much at higher HP.
            { HardStrings.regenerateHealth + HardStrings.inLight, 3 },

            // Reduced from vanilla's 2.
            { HardStrings.regenerateHealth + HardStrings.whileImmersed, 1 },

            // Increased from vanilla's 5. I think too many resistances make the game too easy.
            { HardStrings.resistance, 14 },

            // Until I get around to reworking Spell Absorption, these 3 advantages are non-useable.
            { HardStrings.spellAbsorption + HardStrings.general, 400 },
            { HardStrings.spellAbsorption + HardStrings.inDarkness, 400 },
            { HardStrings.spellAbsorption + HardStrings.inLight, 400 },

            { HardStrings.criticalWeakness, -14 },

            // Increased from vanilla's -6.
            { HardStrings.damage + HardStrings.fromHolyPlaces, -7 },

            // Increased from vanilla's -10. It's a crippling disadvantage.
            { HardStrings.damage + HardStrings.fromSunlight, -28 },

            // Increased from vanilla's -7. A difficult disadvantage with certain popular mods.
            { HardStrings.darknessPoweredMagery + HardStrings.lowerMagicAbilityDaylight, -10 },

            // Increased from vanilla's -10. It's a crippling disadvantage with certain popular mods.
            { HardStrings.darknessPoweredMagery + HardStrings.unableToUseMagicInDaylight, -21 },

            // Increased from vanilla's -2. I assume player is using RPR:I which has good chain armor.
            { HardStrings.forbiddenArmorType + HardStrings.chain, -14 },

            // Increased from vanilla's -1. I assume player is using RPR:I which has good leather armor.
            { HardStrings.forbiddenArmorType + HardStrings.leather, -7 },

            // Increased from vanilla's -5. It's a crippling disadvantage.
            { HardStrings.forbiddenArmorType + HardStrings.plate, -28 },

            // Increased from vanilla's -5. Forbidden materials in Bossfall can be very difficult.
            { HardStrings.forbiddenMaterial + HardStrings.adamantium, -8 },

            // Increased from vanilla's -2. It's a crippling disadvantage.
            { HardStrings.forbiddenMaterial + HardStrings.daedric, -28 },

            // Reduced from vanilla's -7. Dwarven isn't as common in Bossfall compared to vanilla.
            { HardStrings.forbiddenMaterial + HardStrings.dwarven, -4 },

            // Increased from vanilla's -5. The Ebony Dagger is a must-have.
            { HardStrings.forbiddenMaterial + HardStrings.ebony, -10 },

            // Reduced from vanilla's -9. Elven isn't as common in Bossfall compared to vanilla.
            { HardStrings.forbiddenMaterial + HardStrings.elven, -2 },

            // Increased from vanilla's -1. Forbidden Iron would make the early game extremely difficult.
            { HardStrings.forbiddenMaterial + HardStrings.iron, -14 },

            { HardStrings.forbiddenMaterial + HardStrings.mithril, -6 },

            // Increased from vanilla's -3. Player needs high-tier materials to effectively fight bosses.
            { HardStrings.forbiddenMaterial + HardStrings.orcish, -12 },

            // Increased from vanilla's -6. A crippling disadvantage - some enemies are immune to everything but Silver.
            { HardStrings.forbiddenMaterial + HardStrings.silver, -28 },

            // Increased from vanilla's -10. Forbidden Steel would be nearly impossible in Bossfall.
            { HardStrings.forbiddenMaterial + HardStrings.steel, -42 },

            // Increased from vanilla's -1. Shields are difficult to live without.
            { HardStrings.forbiddenShieldTypes, -3 },

            // Increased from vanilla's -2. Very difficult in Bossfall as some enemies have custom weapon immunities.
            { HardStrings.forbiddenWeaponry, -7 },

            // Increased from vanilla's -14. A crippling disadvantage.
            { HardStrings.inabilityToRegen, -28 },

            // Increased from vanilla's -10. A very difficult disadvantage.
            { HardStrings.lightPoweredMagery + HardStrings.lowerMagicAbilityDarkness, -14 },

            // Increased from vanilla's -14. A crippling disadvantage.
            { HardStrings.lightPoweredMagery + HardStrings.unableToUseMagicInDarkness, -28 },

            // Increased from vanilla's -5. Quite difficult due to Bossfall's increased enemy spell variety.
            { HardStrings.lowTolerance, -7 },

            // Increased from vanilla's -4. Quite difficult with Bossfall's increased enemy damage and armor.
            { HardStrings.phobia, -7}
        };

        #endregion

        #region Properties

        /// <summary>
        /// Returns the only instance of BossfallEventHandlers.
        /// </summary>
        public static BossfallEventHandlers Instance { get { return Bossfall.Instance.GetComponent<BossfallEventHandlers>(); } }

        /// <summary>
        /// If game is being restored, this dictionary is populated with enemy references so I can easily restore Bossfall save data.
        /// </summary>
        public Dictionary<ulong, DaggerfallEntityBehaviour> EntityDictionary { get; }
            = new Dictionary<ulong, DaggerfallEntityBehaviour>();

        #endregion

        #region Coroutines

        /// <summary>
        /// Runs during custom character creation and overwrites some vanilla calculations with my own.
        /// </summary>
        /// <returns>Null.</returns>
        IEnumerator SetupCustomClassWindow()
        {
            // Pause execution of this method for one frame. Fields I want to change haven't been assigned to and
            // vanilla's event handlers haven't been registered yet.
            yield return null;

            // Using Reflection I access necessary vanilla fields in the CreateCharCustomClass script.
            Type type = customClassWindow.GetType();
            FieldInfo fieldInfo = type.GetField("createdClass", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo1 = type.GetField("hpLabel", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo2 = type.GetField("hitPointsUpButton", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo3 = type.GetField("hitPointsDownButton", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo4 = type.GetField("daggerPanel", BindingFlags.NonPublic | BindingFlags.Instance);
            difficultyPoints = type.GetField("difficultyPoints", BindingFlags.NonPublic | BindingFlags.Instance);
            advantageAdjust = type.GetField("advantageAdjust", BindingFlags.NonPublic | BindingFlags.Instance);
            disadvantageAdjust = type.GetField("disadvantageAdjust", BindingFlags.NonPublic | BindingFlags.Instance);
            createdClass = (DFCareer)fieldInfo.GetValue(customClassWindow);
            hpLabel = (TextLabel)fieldInfo1.GetValue(customClassWindow);
            hitPointsUpButton = (Button)fieldInfo2.GetValue(customClassWindow);
            hitPointsDownButton = (Button)fieldInfo3.GetValue(customClassWindow);
            daggerPanel = (Panel)fieldInfo4.GetValue(customClassWindow);

            // Initializes default custom class HP per level to 20 without increasing default difficulty.
            createdClass.HitPointsPerLevel = bossfallDefaultHpPerLevel;
            hpLabel.Text = bossfallDefaultHpPerLevel.ToString();

            // These event handlers replace vanilla's difficultyPoints calculations with my own.
            hitPointsUpButton.OnMouseClick += BossfallCustomClass_HitPointsUpButton_OnMouseClick;
            hitPointsDownButton.OnMouseUp += BossfallCustomClass_HitPointsDownButton_OnMouseClick;

            // Deregisters event handlers and resets setup fields once window is closed.
            customClassWindow.OnClose += BossfallCustomClassWindow_OnClose;

            // Only run this coroutine once per window instance.
            customClassSetupComplete = true;
        }

        /// <summary>
        /// Runs during custom character creation and overwrites some vanilla data and calculations with my own.
        /// </summary>
        /// <returns>Null.</returns>
        IEnumerator SetupAdvDisWindow()
        {
            // Pause execution of this method for one frame. The field I want to change hasn't been assigned to and
            // vanilla's event handlers haven't been registered yet.
            yield return null;

            // Using Reflection I overwrite the "difficultyDict" field in the CreateCharSpecialAdvantageWindow script
            // with a custom Dictionary and access other necessary fields.
            Type type = advantageWindow.GetType();
            FieldInfo fieldInfo = type.GetField("difficultyDict", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo1 = type.GetField("primaryPicker", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo2 = type.GetField("secondaryPicker", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo3 = type.GetField("advantageLabels", BindingFlags.NonPublic | BindingFlags.Instance);
            primaryPicker = (DaggerfallListPickerWindow)fieldInfo1.GetValue(advantageWindow);
            secondaryPicker = (DaggerfallListPickerWindow)fieldInfo2.GetValue(advantageWindow);
            advantageLabels = (TextLabel[])fieldInfo3.GetValue(advantageWindow);
            fieldInfo.SetValue(advantageWindow, bossfallDifficultyDict);

            // These event handlers replace vanilla's difficulty dagger calculations with my own.
            primaryPicker.OnItemPicked += BossfallPrimaryPicker_OnItemPicked;
            secondaryPicker.OnItemPicked += BossfallSecondaryPicker_OnItemPicked;

            // This assigns an event handler to each advantageLabels array element that replaces vanilla's difficulty
            // dagger calculations with my own. The array always contains 14 TextLabel elements.
            for (int i = 0; i < 14; i++)
                advantageLabels[i].OnMouseClick += BossfallAdvantageLabel_OnMouseClick;

            // Deregisters event handlers and resets setup fields once window is closed.
            advantageWindow.OnClose += BossfallSpecialAdvantageWindow_OnClose;

            // Only run this coroutine once per window instance.
            advDisSetupComplete = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is vanilla's, pulled from PlayerActivate. It checks if struck object is an enemy.
        /// </summary>
        /// <param name="hitInfo">RaycastHit information about struck object.</param>
        /// <param name="mobileEnemy">The method checks if struck object contains this.</param>
        /// <returns>True if struck object contains DaggerfallEntityBehaviour.</returns>
        bool MobileEnemyCheck(RaycastHit hitInfo, out DaggerfallEntityBehaviour mobileEnemy)
        {
            mobileEnemy = hitInfo.transform.GetComponent<DaggerfallEntityBehaviour>();

            return mobileEnemy != null;
        }

        /// <summary>
        /// This method is based on vanilla DFU's UpdateDifficulty method from the CreateCharCustomClass script. It replaces
        /// vanilla's custom class difficulty calculations with my own. Comments indicate changes or additions I made.
        /// </summary>
        void BossfallUpdateDifficulty()
        {
            const int defaultDaggerX = 220;
            const int defaultDaggerY = 115;
            const int minDaggerY = 46;
            const int maxDaggerY = 186;

            // I use "bossfallDefaultHpPerLevel" in the line below rather than vanilla's "defaultHpPerLevel".
            if (createdClass.HitPointsPerLevel >= bossfallDefaultHpPerLevel)
            {
                // Using Reflection I set the value of vanilla DFU's "difficultyPoints" field in the CreateCharCustomClass
                // script. I use a default HP/lvl value of 20 rather than vanilla's 8.
                difficultyPoints.SetValue(customClassWindow, createdClass.HitPointsPerLevel - bossfallDefaultHpPerLevel);
            }
            else
            {
                // Using Reflection I set the value of vanilla DFU's "difficultyPoints" field in the CreateCharCustomClass
                // script. I use a default HP/lvl value of 20 rather than vanilla's 8, and each HP below default reduces
                // "difficultyPoints" by 4 rather than vanilla's 2.
                difficultyPoints.SetValue(customClassWindow, -(4 * (bossfallDefaultHpPerLevel - createdClass.HitPointsPerLevel)));
            }

            // This unwieldy mess replaces vanilla's "difficultyPoints += advantageAdjust + disadvantageAdjust;" line and does
            // the exact same thing using Reflection.
            difficultyPoints.SetValue(customClassWindow, (int)difficultyPoints.GetValue(customClassWindow)
                + (int)advantageAdjust.GetValue(customClassWindow) + (int)disadvantageAdjust.GetValue(customClassWindow));

            // I replaced vanilla's "difficultyPoints" with "(int)difficultyPoints.GetValue(customClassWindow)" in the line
            // below. I also removed the unnecessary "(float)" cast.
            createdClass.AdvancementMultiplier = 0.3f + (2.7f * ((int)difficultyPoints.GetValue(customClassWindow) + 12) / 52f);

            // I removed " = 0" from the line below.
            int daggerY;

            // I replaced vanilla's "difficultyPoints" with "(int)difficultyPoints.GetValue(customClassWindow)" in the line below.
            if ((int)difficultyPoints.GetValue(customClassWindow) >= 0)
            {
                // I replaced vanilla's "difficultyPoints" with "(int)difficultyPoints.GetValue(customClassWindow)" in the
                // statement below.
                daggerY = Math.Max(minDaggerY, (int)(defaultDaggerY
                    - (37 * ((int)difficultyPoints.GetValue(customClassWindow) / 40f))));
            }
            else
            {
                // I replaced vanilla's "difficultyPoints" with "(int)difficultyPoints.GetValue(customClassWindow)" in the
                // statement below.
                daggerY = Math.Min(maxDaggerY, (int)(defaultDaggerY
                    + (41 * (-(int)difficultyPoints.GetValue(customClassWindow) / 12f))));
            }

            daggerPanel.Position = new Vector2(defaultDaggerX, daggerY);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// This runs if a new game is starting and sets up player's character.
        /// </summary>
        /// <param name="sender">An instance of StartGameBehaviour.</param>
        /// <param name="args">Null.</param>
        public void BossfallOnStartGame(object sender, EventArgs args)
        {
            // Variable used in this method.
            StartGameBehaviour startGameBehaviour = sender as StartGameBehaviour;

            // If starting character is a custom class, do nothing.
            if (!startGameBehaviour.CharacterDocument.isCustom)
            {
                // Barbarians gain 25 HP per level and I don't want to reduce this.
                if (startGameBehaviour.CharacterDocument.career.Name != "Barbarian")
                {
                    // This sets all non-Barbarian canned class HP gained per level to 20. Anything lower than that risks
                    // making Bossfall impossible, and that wouldn't be very fun.
                    startGameBehaviour.CharacterDocument.career.HitPointsPerLevel = bossfallDefaultHpPerLevel;

                    // This sets all non-Barbarian canned class starting HP to 45. Anything lower than that risks making
                    // the early game impossible, and that wouldn't be very fun.
                    GameManager.Instance.PlayerEntity.MaxHealth = 45;
                    GameManager.Instance.PlayerEntity.SetHealth(45);

                    // Not sure these two steps are necessary, but I do them anyway.
                    startGameBehaviour.CharacterDocument.maxHealth = 45;
                    startGameBehaviour.CharacterDocument.currentHealth = 45;
                }
            }

            if (GameManager.Instance.PlayerObject != null)
            {
                if (!bossfallPlayerActivateAdded)
                {
                    // Adds necessary Bossfall components.
                    GameManager.Instance.PlayerObject.AddComponent<BossfallPlayerActivate>();

                    bossfallPlayerActivateAdded = true;
                }
            }
        }

        /// <summary>
        /// This method performs setup on the PlayerObject if player is loading a game. Only runs once.
        /// </summary>
        public void BossfallOnRespawnerComplete()
        {
            if (GameManager.Instance.PlayerObject != null)
            {
                if (!bossfallPlayerActivateAdded)
                {
                    // Adds necessary Bossfall components.
                    GameManager.Instance.PlayerObject.AddComponent<BossfallPlayerActivate>();

                    bossfallPlayerActivateAdded = true;
                }

                // Don't run this method again.
                PlayerEnterExit.OnRespawnerComplete -= BossfallOnRespawnerComplete;
            }
        }

        /// <summary>
        /// This method runs when player is entering a dungeon. It replaces random vanilla enemies with Bossfall enemies.
        /// </summary>
        /// <param name="args">DaggerfallDungeon instance of the dungeon being entered and an empty StaticDoor instance.</param>
        public void BossfallOnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            // No enemies will be present at this point if game is being loaded, so don't run this method.
            if (SaveLoadManager.Instance.LoadInProgress)
                return;

            // The dungeon being entered.
            DaggerfallDungeon dungeon = args.DaggerfallDungeon;

            // I got this number from RDBLayout.
            const int randomMonsterMarker = 15;

            // Only enemies will register Raycast hits.
            int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemies");

            // Find Random Enemies node created in RDBLayout so I can assign new enemies to it.
            GameObject randomEnemiesNode = GameObject.Find("Random Enemies");

            // These for loops are based on DaggerfallDungeon's EnumerateDebuggerMarkers method.
            // I scan the entire dungeon for random spawn points, block by block.
            for (int i = 0; i < dungeon.Summary.LocationData.Dungeon.Blocks.Length; i++)
            {
                // Gets DFBlock data for current block.
                DFBlock blockData = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(
                    dungeon.Summary.LocationData.Dungeon.Blocks[i].BlockName);

                // Reseeds random number generator with each new block. Not sure if this necessary.
                UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

                // Each RdbBlock has an ObjectRootList array.
                for (int j = 0; j < blockData.RdbBlock.ObjectRootList.Length; j++)
                {
                    // Some ObjectRootList arrays have null elements.
                    if (blockData.RdbBlock.ObjectRootList[j].RdbObjects == null)
                        continue;

                    // RdbObjects are the objects I want to scan for - they contain random enemy spawn positions.
                    for (int k = 0; k < blockData.RdbBlock.ObjectRootList[j].RdbObjects.Length; k++)
                    {
                        // I only want to find random monster spawn points and nothing else.
                        if (blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].Resources.FlatResource.TextureRecord
                            == randomMonsterMarker)
                        {
                            // Reads random spawn position - converted from classic to Unity units - from RdbObjects array.
                            Vector3 position = new Vector3(blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].XPos,
                                -blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].YPos,
                                blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].ZPos) * MeshReader.GlobalScale;

                            // Adjustment to spawn point based on where dungeon block is relative to the entire dungeon.
                            Vector3 dungeonBlockPosition =
                                new Vector3(dungeon.Summary.LocationData.Dungeon.Blocks[i].X * RDBLayout.RDBSide, 0,
                                dungeon.Summary.LocationData.Dungeon.Blocks[i].Z * RDBLayout.RDBSide);

                            // The random monster spawn point I want to check.
                            Vector3 adjustedPosition = dungeonBlockPosition + position;

                            // I tried using a Linecast but I didn't have 100% enemy detection rates, so I changed tactics.
                            // I shoot a sphere straight up, this sphere originates well underneath the random spawn point.
                            // This method appears to be 100% successful at detecting enemies.
                            if (Physics.SphereCast(adjustedPosition + new Vector3(0, -2f, 0),
                                0.05f, Vector3.up, out RaycastHit hit, 3.5f, enemyLayerMask))
                            {
                                // If the SphereCast registers a hit, I verify that the struck object is an enemy.
                                if (MobileEnemyCheck(hit, out DaggerfallEntityBehaviour entity))
                                {
                                    // Once enemy existence is verified, I destroy the enemy.
                                    Destroy(entity.gameObject);

                                    // Checks if spawn position is underwater. I pulled this water level check from EnemyMotor.
                                    if ((dungeon.Summary.LocationData.Dungeon.Blocks[i].WaterLevel * -1 * MeshReader.GlobalScale)
                                        >= adjustedPosition.y + (100 * MeshReader.GlobalScale))
                                    {
                                        // Spawns an underwater enemy using Bossfall's expanded encounter tables.
                                        GameObject[] waterEnemy = GameObjectHelper.CreateFoeGameObjects(
                                            adjustedPosition, BossfallEncounterTables.Instance.ChooseRandomEnemy(true));

                                        // Use already created "Random Enemies" node as parent transform.
                                        waterEnemy[0].transform.parent = randomEnemiesNode.transform;

                                        // Activate enemy.
                                        waterEnemy[0].SetActive(true);
                                    }
                                    else
                                    {
                                        // Spawns a non-water enemy using Bossfall's expanded encounter tables.
                                        GameObject[] nonWaterEnemy = GameObjectHelper.CreateFoeGameObjects(
                                            adjustedPosition, BossfallEncounterTables.Instance.ChooseRandomEnemy(false));

                                        // Use already created "Random Enemies" node as parent transform.
                                        nonWaterEnemy[0].transform.parent = randomEnemiesNode.transform;

                                        // Activate enemy.
                                        nonWaterEnemy[0].SetActive(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method uses some code from vanilla's EnemyEntity script. It sets up enemies using Bossfall rules.
        /// </summary>
        /// <param name="sender">An instance of EnemyEntity.</param>
        /// <param name="args">MobileEnemy, DFCareer, and ItemCollection data.</param>
        public void BossfallOnEnemyLootSpawned(object sender, EnemyLootSpawnedEventArgs args)
        {
            // Variables I use in this method. Some are from the event args, split into their base components.
            DaggerfallEntityBehaviour entityBehaviour = (sender as EnemyEntity).EntityBehaviour;
            EnemyEntity entity = sender as EnemyEntity;
            MobileEnemy mobileEnemy = args.MobileEnemy;
            ItemCollection items = args.Items;
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            // I pulled these next two lines from the SetEnemyCareer method in EnemyEntity and modified them for Bossfall.
            DFCareer customCareer = DaggerfallEntity.GetCustomCareerTemplate(mobileEnemy.ID);
            if (customCareer != null)
            {
                // If this is a custom enemy I don't change its level or stats - I only add necessary Bossfall components.
                entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();
                return;
            }

            // This if/else if is from the SetEnemyCareer method in EnemyEntity, modified for Bossfall.
            if (entity.EntityType == EntityTypes.EnemyMonster)
            {
                // Enemy monster levels vary up and down a bit. This affects their accuracy, dodging, and spell power.
                entity.Level = mobileEnemy.Level + UnityEngine.Random.Range(-2, 2 + 1);

                // Non-boss monster levels can't go below 1 and are capped at 20. Only Daedra Seducers are potentially
                // affected by the level 20 cap.
                if (entity.Level < 1)
                {
                    entity.Level = 1;
                }

                if (entity.Level > 20)
                {
                    entity.Level = 20;
                }

                // This manually sets boss levels.
                if (entity.CareerIndex == (int)MobileTypes.Vampire || entity.CareerIndex == (int)MobileTypes.Lich)
                {
                    entity.Level = UnityEngine.Random.Range(21, 25 + 1);
                }
                else if (entity.CareerIndex == (int)MobileTypes.Dragonling_Alternate
                    || entity.CareerIndex == (int)MobileTypes.OrcWarlord)
                {
                    entity.Level = UnityEngine.Random.Range(21, 30 + 1);
                }
                else if (entity.CareerIndex == (int)MobileTypes.VampireAncient || entity.CareerIndex == (int)MobileTypes.DaedraLord
                    || entity.CareerIndex == (int)MobileTypes.AncientLich)
                {
                    entity.Level = UnityEngine.Random.Range(26, 30 + 1);
                }

                // This for loop is from the SetEnemyCareer method in EnemyEntity, modified for Bossfall. I added it here as
                // theoretically a monster's armor values could be lower than I want if vanilla spawns them with very good
                // equipment. To avoid that possibility, I reset their armor to my intended values.
                for (int i = 0; i < entity.ArmorValues.Length; i++)
                {
                    entity.ArmorValues[i] = (sbyte)(mobileEnemy.ArmorValue * 5);
                }
            }
            else if (entity.EntityType == EntityTypes.EnemyClass)
            {
                // This ugly thing unlevels class enemies. Their levels are weighted to usually be around 10. Level 1 and
                // 20 enemies are very rare. Class enemies are unleveled once player is at least level 7.
                if (player.Level > 6)
                {
                    int roll = Dice100.Roll();

                    if (roll > 0)
                    {
                        if (roll > 1)
                        {
                            if (roll > 2)
                            {
                                if (roll > 3)
                                {
                                    if (roll > 6)
                                    {
                                        if (roll > 11)
                                        {
                                            if (roll > 20)
                                            {
                                                if (roll > 33)
                                                {
                                                    if (roll > 50)
                                                    {
                                                        if (roll > 67)
                                                        {
                                                            if (roll > 80)
                                                            {
                                                                if (roll > 89)
                                                                {
                                                                    if (roll > 94)
                                                                    {
                                                                        if (roll > 97)
                                                                        {
                                                                            if (roll > 98)
                                                                            {
                                                                                if (roll > 99)
                                                                                {
                                                                                    entity.Level = UnityEngine.Random.Range(18, 20 + 1);
                                                                                }
                                                                                else
                                                                                {
                                                                                    entity.Level = 17;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                entity.Level = 16;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            entity.Level = 15;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        entity.Level = 14;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    entity.Level = 13;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                entity.Level = 12;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            entity.Level = 11;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        entity.Level = 10;
                                                    }
                                                }
                                                else
                                                {
                                                    entity.Level = 9;
                                                }
                                            }
                                            else
                                            {
                                                entity.Level = 8;
                                            }
                                        }
                                        else
                                        {
                                            entity.Level = 7;
                                        }
                                    }
                                    else
                                    {
                                        entity.Level = 6;
                                    }
                                }
                                else
                                {
                                    entity.Level = 5;
                                }
                            }
                            else
                            {
                                entity.Level = 4;
                            }
                        }
                        else
                        {
                            entity.Level = UnityEngine.Random.Range(1, 3 + 1);
                        }
                    }
                }
                else
                {
                    // I pulled this from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
                    // If player is level 6 or lower, all class enemies will be within 2 levels of the player.
                    entity.Level = player.Level + UnityEngine.Random.Range(-2, 2 + 1);

                    // The enemy's level can't go below 1.
                    if (entity.Level < 1)
                    {
                        entity.Level = 1;
                    }
                }

                // I pulled this from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
                // Guard levels are buffed compared to vanilla DFU. They can get a 10 level boost. HALT!
                if (entity.CareerIndex == (int)MobileTypes.Knight_CityWatch - 128)
                    entity.Level += UnityEngine.Random.Range(0, 10 + 1);

                // This manually sets Assassins to boss levels, but only if player is at least level 7.
                if (player.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                    entity.Level = UnityEngine.Random.Range(21, 30 + 1);

                // I copied this line from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
                // I re-roll enemy's MaxHealth as it likely wasn't correct the first time around.
                entity.MaxHealth = FormulaHelper.RollEnemyClassMaxHealth(entity.Level, entity.Career.HitPointsPerLevel);

                // Once player is at least level 7, Assassin HP is set to their boss range.
                if (player.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                    entity.MaxHealth = UnityEngine.Random.Range(100, 300 + 1);

                // This for loop is based on one from the SetEnemyEquipment method in EnemyEntity. This sets
                // class enemy armor depending on their level and ignores current equipped armor.
                for (int i = 0; i < entity.ArmorValues.Length; i++)
                {
                    entity.ArmorValues[i] = (sbyte)(60 - (entity.Level * 2));

                    // Once player is at least level 7, Assassins have boss armor.
                    if (player.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                    {
                        entity.ArmorValues[i] = 0;
                    }
                }
            }

            // I pulled this skillsLevel section from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
            // Enemies scale up faster than vanilla's 5/level and have a skill cap of 180 - greatly increased from vanilla's 100.
            short skillsLevel = (short)((entity.Level * 7) + 30);

            if (skillsLevel > 180)
            {
                skillsLevel = 180;
            }

            // I pulled this from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall. This re-applies
            // enemy skill values as they weren't correct the first time around.
            for (int i = 0; i <= DaggerfallSkills.Count; i++)
            {
                entity.Skills.SetPermanentSkillValue(i, skillsLevel);
            }

            // I send loot generation to a custom method so I can scale loot table items with enemy level.
            BossfallItemBuilder.Instance.GenerateItems(mobileEnemy.LootTableKey, items, entity.Level * 50);

            // I based this section below on a part of the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
            // If enemy is in this list, they start with equipment and it scales with their level.
            if (entity.EntityType == EntityTypes.EnemyClass || entity.CareerIndex == (int)MonsterCareers.OrcSergeant
                || entity.CareerIndex == (int)MonsterCareers.OrcShaman || entity.CareerIndex == (int)MonsterCareers.OrcWarlord
                || entity.CareerIndex == (int)MonsterCareers.FrostDaedra || entity.CareerIndex == (int)MonsterCareers.FireDaedra
                || entity.CareerIndex == (int)MonsterCareers.Daedroth || entity.CareerIndex == (int)MonsterCareers.Vampire
                || entity.CareerIndex == (int)MonsterCareers.DaedraSeducer || entity.CareerIndex == (int)MonsterCareers.VampireAncient
                || entity.CareerIndex == (int)MonsterCareers.DaedraLord || entity.CareerIndex == (int)MonsterCareers.Lich
                || entity.CareerIndex == (int)MonsterCareers.AncientLich || entity.CareerIndex == (int)MonsterCareers.Dragonling_Alternate)
            {
                // I send the call to a custom item generation method. I use all three possible variants - the third is
                // never used for enemy classes in vanilla. Adds more variety.
                BossfallItemBuilder.Instance.AssignEnemyStartingEquipment(player, entity, UnityEngine.Random.Range(0, 2 + 1));
            }

            // This if/else if is based on a section of the SetEnemyCareer method in EnemyEntity, modified for Bossfall.
            // I assign all monsters and human spellcasters the same spell kit, with the exception of Frost/Fire Daedra,
            // whose spell kits are only slightly modified from vanilla's. I reroute SetEnemySpells method calls to a custom
            // method elsewhere in this script. I also remove Ghost and VampireAncient spellbooks.
            if (entity.EntityType == EntityTypes.EnemyMonster)
            {
                if (entity.CareerIndex == (int)MonsterCareers.Imp || entity.CareerIndex == (int)MonsterCareers.OrcShaman
                    || entity.CareerIndex == (int)MonsterCareers.Wraith || entity.CareerIndex == (int)MonsterCareers.Daedroth
                    || entity.CareerIndex == (int)MonsterCareers.Vampire || entity.CareerIndex == (int)MonsterCareers.DaedraSeducer
                    || entity.CareerIndex == (int)MonsterCareers.DaedraLord || entity.CareerIndex == (int)MonsterCareers.Lich
                    || entity.CareerIndex == (int)MonsterCareers.AncientLich)
                    SetEnemySpells(BossfallGenericSpells, entity);
                else if (entity.CareerIndex == (int)MonsterCareers.FrostDaedra)
                    SetEnemySpells(BossfallFrostDaedraSpells, entity);
                else if (entity.CareerIndex == (int)MonsterCareers.FireDaedra)
                    SetEnemySpells(BossfallFireDaedraSpells, entity);

                // This deletes the spellbook contents of Ghosts and Vampire Ancients so they no longer cast any spells. The
                // DeleteSpell method changes index numbers after every spell deletion, so I iterate backwards to catch everything.
                else if (entity.CareerIndex == (int)MonsterCareers.Ghost || entity.CareerIndex == (int)MonsterCareers.VampireAncient)
                {
                    for (int i = entity.SpellbookCount() - 1; i >= 0; i--)
                    {
                        entity.DeleteSpell(i);
                    }
                }
            }
            else if (entity.EntityType == EntityTypes.EnemyClass && (mobileEnemy.CastsMagic))
            {
                SetEnemySpells(BossfallGenericSpells, entity);
            }

            // I pulled this map generation line out of the SetEnemyCareer method in EnemyEntity and made no changes to it.
            // I include it because I clear and re-generate this enemy's ItemCollection so I have to run this again.
            DaggerfallLoot.RandomlyAddMap(mobileEnemy.MapChance, items);

            // I pulled these potion lines out of the SetEnemyCareer method in EnemyEntity. I raised potion generation
            // chance to 4 rather than 3. I had to include these lines as I clear and re-spawn this enemy's ItemCollection.
            if (!string.IsNullOrEmpty(mobileEnemy.LootTableKey))
            {
                DaggerfallLoot.RandomlyAddPotion(4, items);
                DaggerfallLoot.RandomlyAddPotionRecipe(2, items);
            }

            // As a finishing touch, I add essential Bossfall AI components to the enemy.
            entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
            entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
            entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();

            // If game is being loaded, add this enemy to a reference dictionary so I can easily modify this enemy elsewhere.
            // Keys are unique enemy load IDs, which have already been restored at this point in the load process.
            if (SaveLoadManager.Instance.LoadInProgress)
            {
                DaggerfallEnemy dfEnemy = entityBehaviour.GetComponent<DaggerfallEnemy>();
                ulong key = dfEnemy.LoadID;
                EntityDictionary.Add(key, entityBehaviour);
            }
        }

        /// <summary>
        /// This method is based on a method of the same name from vanilla's EnemyEntity, modified for Bossfall. I clear
        /// the enemy's existing spellbook and replace it with my custom list. I also change enemy's Magicka capacity.
        /// </summary>
        /// <param name="spellList">The spell list to assign to the enemy.</param>
        /// <param name="enemyEntity">The enemy to assign the spell list to.</param>
        void SetEnemySpells(byte[] spellList, EnemyEntity enemyEntity)
        {
            // This deletes enemy's vanilla spellbook. The DeleteSpell method changes index numbers after every spell
            // deletion, so I iterate backwards to catch everything.
            for (int i = enemyEntity.SpellbookCount() - 1; i >= 0; i--)
            {
                enemyEntity.DeleteSpell(i);
            }

            // I set MaxMagicka based on the level of the enemy. Each spell - regardless of type - costs enemy 40 Magicka.
            if (enemyEntity.Level > 0 && enemyEntity.Level < 8)
            {
                // Enough for 2 spells.
                enemyEntity.MaxMagicka = 80;
            }
            else if (enemyEntity.Level >= 8 && enemyEntity.Level < 13)
            {
                // Enough for 3 spells.
                enemyEntity.MaxMagicka = 120;
            }
            else if (enemyEntity.Level >= 13 && enemyEntity.Level < 16)
            {
                // Enough for 4 spells.
                enemyEntity.MaxMagicka = 160;
            }
            else if (enemyEntity.Level >= 16 && enemyEntity.Level < 18)
            {
                // Enough for 5 spells.
                enemyEntity.MaxMagicka = 200;
            }
            else if (enemyEntity.Level >= 18 && enemyEntity.Level < 20)
            {
                // Enough for 6 spells.
                enemyEntity.MaxMagicka = 240;
            }
            else if (enemyEntity.Level == 20)
            {
                // Enough for 8 spells.
                enemyEntity.MaxMagicka = 300;
            }
            else if (enemyEntity.Level >= 21 && enemyEntity.Level < 26)
            {
                // Enough for 30 spells.
                enemyEntity.MaxMagicka = 1200;
            }
            else if (enemyEntity.Level >= 26)
            {
                // Infinite spells.
                enemyEntity.MaxMagicka = 1000000;
            }

            // This line is from the SetEnemySpells method in EnemyEntity, modified for Bossfall.
            enemyEntity.CurrentMagicka = enemyEntity.MaxMagicka;

            // This foreach is from the SetEnemySpells method in EnemyEntity, slightly modified for Bossfall.
            foreach (byte spellID in spellList)
            {
                // I inlined the "SpellRecord.SpellRecordData" declaration.
                GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(spellID, out SpellRecord.SpellRecordData spellData);
                if (spellData.index == -1)
                {
                    Debug.LogError("Failed to locate enemy spell in standard spells list.");
                    continue;
                }

                // I inlined the "EffectBundleSettings" declaration.
                if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spellData, BundleTypes.Spell, out EffectBundleSettings bundle))
                {
                    Debug.LogError("Failed to create effect bundle for enemy spell: " + spellData.spellName);
                    continue;
                }

                // I added "enemyEntity." to the line below.
                enemyEntity.AddSpell(bundle);
            }
        }

        /// <summary>
        /// This is mostly vanilla's StockShopShelves and StockHouseContainer methods from DaggerfallLoot, modified for
        /// Bossfall. Comments indicate changes or additions I made.
        /// </summary>
        /// <param name="sender">An instance of PlayerActivate.</param>
        /// <param name="args">LootContainerTypes container identifier and ItemCollection items.</param>
        public void BossfallOnContainerLootSpawned(object sender, ContainerLootSpawnedEventArgs args)
        {
            // Variables I use in this method. Some are from the event args, split into their base components.
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            PlayerActivate playerActivate = sender as PlayerActivate;
            PlayerEnterExit playerEnterExit = playerActivate.GetComponent<PlayerEnterExit>();
            LootContainerTypes containerType = args.ContainerType;
            ItemCollection items = args.Loot;
            PlayerGPS.DiscoveredBuilding buildingData = playerEnterExit.BuildingDiscoveryData;

            // I added this. Certain old saves will have a zero here.
            if (buildingData.buildingKey == 0)
                return;

            // This decides how loot should be spawned based on whether it's a shop shelf or house container.
            if (containerType == LootContainerTypes.ShopShelves)
            {
                items.Clear();

                DFLocation.BuildingTypes buildingType = buildingData.buildingType;
                int shopQuality = buildingData.quality;
                ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
                byte[] itemGroups = { 0 };

                switch (buildingType)
                {
                    case DFLocation.BuildingTypes.Alchemist:
                        itemGroups = DaggerfallLootDataTables.itemGroupsAlchemist;

                        // I added "DaggerfallLoot." to the line below to send the call to vanilla's DaggerfallLoot method.
                        DaggerfallLoot.RandomlyAddPotionRecipe(25, items);
                        break;
                    case DFLocation.BuildingTypes.Armorer:
                        itemGroups = DaggerfallLootDataTables.itemGroupsArmorer;
                        break;
                    case DFLocation.BuildingTypes.Bookseller:
                        itemGroups = DaggerfallLootDataTables.itemGroupsBookseller;
                        break;
                    case DFLocation.BuildingTypes.ClothingStore:
                        itemGroups = DaggerfallLootDataTables.itemGroupsClothingStore;
                        break;
                    case DFLocation.BuildingTypes.GemStore:
                        itemGroups = DaggerfallLootDataTables.itemGroupsGemStore;
                        break;
                    case DFLocation.BuildingTypes.GeneralStore:
                        itemGroups = DaggerfallLootDataTables.itemGroupsGeneralStore;
                        items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Horse));
                        items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Small_cart));
                        break;
                    case DFLocation.BuildingTypes.PawnShop:
                        itemGroups = DaggerfallLootDataTables.itemGroupsPawnShop;
                        break;
                    case DFLocation.BuildingTypes.WeaponSmith:
                        itemGroups = DaggerfallLootDataTables.itemGroupsWeaponSmith;
                        break;
                }

                for (int i = 0; i < itemGroups.Length; i += 2)
                {
                    ItemGroups itemGroup = (ItemGroups)itemGroups[i];
                    int chanceMod = itemGroups[i + 1];

                    // I changed "playerEntity" to "player" and deleted "Game.Entity." from the line below.
                    if (itemGroup == ItemGroups.MensClothing && player.Gender == Genders.Female)
                        itemGroup = ItemGroups.WomensClothing;

                    // I changed "playerEntity" to "player" and deleted "Game.Entity." from the line below.
                    if (itemGroup == ItemGroups.WomensClothing && player.Gender == Genders.Male)
                        itemGroup = ItemGroups.MensClothing;

                    if (itemGroup != ItemGroups.Furniture && itemGroup != ItemGroups.UselessItems1)
                    {
                        // I thought General Stores and Pawn Shops stocked too many books. This check greatly
                        // reduces generated books in those types of stores. The amount of books generated depends on
                        // shop quality - as quality rises, they may stock more books and have greater chances to do so.
                        // General Stores and Pawn Shops below average store quality will never stock any books.
                        if (itemGroup == ItemGroups.Books && buildingType != DFLocation.BuildingTypes.Bookseller
                            && buildingType != DFLocation.BuildingTypes.Library)
                        {
                            if (Dice100.SuccessRoll(5) && shopQuality > 7)
                            {
                                items.AddItem(ItemBuilder.CreateRandomBook());
                            }
                            if (Dice100.SuccessRoll(15) && shopQuality > 13)
                            {
                                items.AddItem(ItemBuilder.CreateRandomBook());
                            }
                            if (Dice100.SuccessRoll(25) && shopQuality > 17)
                            {
                                items.AddItem(ItemBuilder.CreateRandomBook());
                            }
                        }
                        // I thought Booksellers didn't stock enough books. This new check greatly increases
                        // the amount of books on Bookseller shelves.
                        else if (itemGroup == ItemGroups.Books && buildingType == DFLocation.BuildingTypes.Bookseller)
                        {
                            int bookMod = shopQuality + UnityEngine.Random.Range(-5, 5 + 1);
                            for (int j = 0; j <= bookMod; j++)
                            {
                                items.AddItem(ItemBuilder.CreateRandomBook());
                            }
                        }
                        else if (itemGroup == ItemGroups.Books)
                        {
                            int qualityMod = (shopQuality + 3) / 5;
                            if (qualityMod >= 4)
                                --qualityMod;
                            qualityMod++;
                            for (int j = 0; j <= qualityMod; ++j)
                            {
                                items.AddItem(ItemBuilder.CreateRandomBook());
                            }
                        }
                        else
                        {
                            System.Array enumArray = itemHelper.GetEnumArray(itemGroup);
                            for (int j = 0; j < enumArray.Length; ++j)
                            {
                                ItemTemplate itemTemplate = itemHelper.GetItemTemplate(itemGroup, j);
                                if (itemTemplate.rarity <= shopQuality)
                                {
                                    int stockChance = chanceMod * 5 * (21 - itemTemplate.rarity) / 100;
                                    if (Dice100.SuccessRoll(stockChance))
                                    {
                                        // I removed "= null" from the following line.
                                        DaggerfallUnityItem item;
                                        if (itemGroup == ItemGroups.Weapons)

                                            // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                            // creation method.
                                            item = BossfallItemBuilder.Instance.CreateWeapon(j + Weapons.Dagger, FormulaHelper.RandomMaterial(0));
                                        else if (itemGroup == ItemGroups.Armor)

                                            // I replaced "playerEntity.Level" with 0 in the FormulaHelper method call to unlevel
                                            // loot. I also call a different item creation method and replaced "playerEntity" with
                                            // "player" in the first and second parameters.
                                            item = BossfallItemBuilder.Instance.CreateArmor(player.Gender, player.Race, j + Armor.Cuirass, FormulaHelper.RandomArmorMaterial(0));
                                        else if (itemGroup == ItemGroups.MensClothing)
                                        {
                                            // I replaced "playerEntity" with "player" in the line below.
                                            item = ItemBuilder.CreateMensClothing(j + MensClothing.Straps, player.Race);
                                            item.dyeColor = ItemBuilder.RandomClothingDye();
                                        }
                                        else if (itemGroup == ItemGroups.WomensClothing)
                                        {
                                            // I replaced "playerEntity" with "player" in the line below.
                                            item = ItemBuilder.CreateWomensClothing(j + WomensClothing.Brassier, player.Race);
                                            item.dyeColor = ItemBuilder.RandomClothingDye();
                                        }
                                        else if (itemGroup == ItemGroups.MagicItems)
                                        {
                                            // I replaced "playerEntity.Level" with 0 in the first parameter to unlevel loot. I
                                            // also call a different item creation method and replace "playerEntity" with "player"
                                            // in the second and third parameters.
                                            item = BossfallItemBuilder.Instance.CreateRandomMagicItem(0, player.Gender, player.Race);
                                        }
                                        else
                                        {
                                            item = new DaggerfallUnityItem(itemGroup, j);
                                            if (DaggerfallUnity.Settings.PlayerTorchFromItems && item.IsOfTemplate(ItemGroups.UselessItems2, (int)UselessItems2.Oil))

                                                // I added "UnityEngine." before "Random.Range".
                                                item.stackCount = UnityEngine.Random.Range(5, 20 + 1);

                                            // These checks create Holy items with custom enchantments. Only Pawn
                                            // Shops will ever stock them. If the Pawn Shop message is "better appointed than most"
                                            // Holy Water may be on the shelf. If the Pawn Shop message is "incense and soft music"
                                            // Holy Water, Holy Daggers, and Holy Tomes may be on the shelf.
                                            if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_water))
                                            {
                                                item = BossfallItemBuilder.Instance.CreateHolyWater();
                                            }
                                            else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_dagger))
                                            {
                                                item = BossfallItemBuilder.Instance.CreateHolyDagger();
                                            }
                                            else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_tome))
                                            {
                                                item = BossfallItemBuilder.Instance.CreateHolyTome();
                                            }
                                        }
                                        items.AddItem(item);
                                    }
                                }
                            }
                            int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(itemGroup);
                            for (int j = 0; j < customItemTemplates.Length; j++)
                            {
                                ItemTemplate itemTemplate = itemHelper.GetItemTemplate(itemGroup, customItemTemplates[j]);
                                if (itemTemplate.rarity <= shopQuality)
                                {
                                    int stockChance = chanceMod * 5 * (21 - itemTemplate.rarity) / 100;
                                    if (Dice100.SuccessRoll(stockChance))
                                    {
                                        DaggerfallUnityItem item = ItemBuilder.CreateItem(itemGroup, customItemTemplates[j]);

                                        if (itemGroup == ItemGroups.Weapons)
                                        {
                                            // I replaced "playerEntity.Level" with 0 to unlevel loot.
                                            WeaponMaterialTypes material = FormulaHelper.RandomMaterial(0);

                                            // I reroute the method call to a custom method.
                                            BossfallItemBuilder.Instance.ApplyWeaponMaterial(item, material);
                                        }
                                        else if (itemGroup == ItemGroups.Armor)
                                        {
                                            // I replaced "playerEntity.Level" with 0 to unlevel loot.
                                            ArmorMaterialTypes material = FormulaHelper.RandomArmorMaterial(0);

                                            // I reroute the method call to a custom method. I also replaced "playerEntity"
                                            // with "player" in the second and third parameter.
                                            BossfallItemBuilder.Instance.ApplyArmorSettings(item, player.Gender, player.Race, material);
                                        }

                                        items.AddItem(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // This begins a section of code copied from PlayerActivate, modified for Bossfall. I'm sure I am missing an
                // obvious solution, but I couldn't figure out how to get the loot container's DaggerfallLoot component and I
                // need its TextureRecord, so I fire a new ray and check the RaycastHit for DaggerfallLoot.

                // I removed "= new Ray()" from the line below.
                Ray ray;
                Camera mainCamera = GameManager.Instance.MainCamera;
                int playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
                if (GameManager.Instance.PlayerMouseLook.cursorActive)
                {
                    if (DaggerfallUnity.Settings.RetroRenderingMode > 0)
                    {
                        float largeHUDHeight = 0;
                        if (DaggerfallUI.Instance.DaggerfallHUD != null && DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled && DaggerfallUnity.Settings.LargeHUDDocked)
                            largeHUDHeight = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ScreenHeight;
                        float xm = Input.mousePosition.x / Screen.width;
                        float ym = (Input.mousePosition.y - largeHUDHeight) / (Screen.height - largeHUDHeight);
                        Vector2 retroMousePos = new Vector2(mainCamera.targetTexture.width * xm, mainCamera.targetTexture.height * ym);
                        ray = mainCamera.ScreenPointToRay(retroMousePos);
                    }
                    else
                    {
                        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    }
                }
                else
                {
                    ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                }

                // I don't think this Raycast will ever return false, but I check anyway.
                if (!Physics.Raycast(ray, out RaycastHit hit, PlayerActivate.TreasureActivationDistance, playerLayerMask))
                {
                    Debug.Log("Raycast missed loot container.");
                    return;
                }

                // I don't think DaggerfallLoot will ever be null, but I check anyway.
                if (hit.transform.GetComponent<DaggerfallLoot>() == null)
                {
                    Debug.Log("No DaggerfallLoot in RaycastHit.");
                    return;
                }

                DaggerfallLoot loot = hit.transform.GetComponent<DaggerfallLoot>();
                // The above line ends the section of code copied from PlayerActivate.

                items.Clear();

                DFLocation.BuildingTypes buildingType = buildingData.buildingType;

                // I added "loot." to the line below.
                uint modelIndex = (uint)loot.TextureRecord;

                // I removed "= null" from the following two lines.
                byte[] privatePropertyList;
                DaggerfallUnityItem item;

                if (buildingType < DFLocation.BuildingTypes.House5)
                {
                    if (modelIndex >= 2)
                    {
                        if (modelIndex >= 4)
                        {
                            if (modelIndex >= 11)
                            {
                                if (modelIndex >= 15)
                                {
                                    privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels15AndUp[(int)buildingType];
                                }
                                else
                                {
                                    privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels11to14[(int)buildingType];
                                }
                            }
                            else
                            {
                                privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels4to10[(int)buildingType];
                            }
                        }
                        else
                        {
                            privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels2to3[(int)buildingType];
                        }
                    }
                    else
                    {
                        privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels0to1[(int)buildingType];
                    }
                    if (privatePropertyList == null)
                        return;

                    // I added "UnityEngine." before "Random.Range" to the line below.
                    int randomChoice = UnityEngine.Random.Range(0, privatePropertyList.Length);
                    ItemGroups itemGroup = (ItemGroups)privatePropertyList[randomChoice];
                    int continueChance = 100;
                    bool keepGoing = true;
                    while (keepGoing)
                    {
                        if (itemGroup != ItemGroups.MensClothing && itemGroup != ItemGroups.WomensClothing)
                        {
                            if (itemGroup == ItemGroups.MagicItems)
                            {
                                // I replaced "playerEntity.Level" with 0 in the first parameter to unlevel loot. I
                                // also call a different item creation method and replace "playerEntity" with "player"
                                // in the second and third parameters.
                                item = BossfallItemBuilder.Instance.CreateRandomMagicItem(0, player.Gender, player.Race);
                            }
                            else if (itemGroup == ItemGroups.Books)
                            {
                                item = ItemBuilder.CreateRandomBook();
                            }
                            else
                            {
                                if (itemGroup == ItemGroups.Weapons)

                                    // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                    // creation method.
                                    item = BossfallItemBuilder.Instance.CreateRandomWeapon(0);
                                else if (itemGroup == ItemGroups.Armor)

                                    // I replaced "playerEntity.Level" with 0 in the first parameter to unlevel loot. I
                                    // also call a different item creation method and replace "playerEntity" with "player"
                                    // in the second and third parameters.
                                    item = BossfallItemBuilder.Instance.CreateRandomArmor(0, player.Gender, player.Race);
                                else
                                {
                                    System.Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(itemGroup);

                                    // I added "UnityEngine." before "Random.Range".
                                    item = new DaggerfallUnityItem(itemGroup, UnityEngine.Random.Range(0, enumArray.Length));

                                    // These checks create Holy items with custom enchantments. I don't know which 
                                    // building types can generate religious items, so I can't tell you where to look. I know
                                    // they can generate in some taverns, but I never checked anything beyond that.
                                    if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_water))
                                    {
                                        item = BossfallItemBuilder.Instance.CreateHolyWater();
                                    }
                                    else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_dagger))
                                    {
                                        item = BossfallItemBuilder.Instance.CreateHolyDagger();
                                    }
                                    else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_tome))
                                    {
                                        item = BossfallItemBuilder.Instance.CreateHolyTome();
                                    }
                                }
                            }
                        }
                        else
                        {
                            // I replaced "playerEntity" with "player" in the first and second parameters.
                            item = ItemBuilder.CreateRandomClothing(player.Gender, player.Race);
                        }
                        continueChance >>= 1;
                        if (DFRandom.rand() % 100 > continueChance)
                            keepGoing = false;
                        items.AddItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Generates items for all dungeon and building interior loot piles.
        /// </summary>
        /// <param name="sender">Null.</param>
        /// <param name="args">The loot pile's location index, loot table key, and ItemCollection.</param>
        public void BossfallOnTabledLootSpawned(object sender, TabledLootSpawnedEventArgs args)
        {
            // Variables used in this method. Some are from the event args, split into their base components.
            ItemCollection items = args.Items;
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;

            // I need to know if player is inside a dungeon as I plan on varying loot contents and quality by dungeon type.
            // "playerEnterExit.IsPlayerInsideDungeon" isn't set until loot is already generated, so that was a dead end.
            // Fortunately there was a different property to check.
            if (playerEnterExit.IsCreatingDungeonObjects)
            {
                DFRegion.DungeonTypes playerDungeon = playerEnterExit.Dungeon.Summary.DungeonType;

                if (!BossfallItemBuilder.Instance.GenerateLoot(items, (int)playerDungeon, 0, true))
                    Debug.Log($"Dungeon loot pile generation failed, {playerDungeon} is invalid or out of range.");
            }

            // Only other option is a non-dungeon interior loot pile.
            else
            {
                DFRegion.LocationTypes playerLocation = GameManager.Instance.PlayerGPS.CurrentLocationType;

                if (!BossfallItemBuilder.Instance.GenerateLoot(items, (int)playerLocation, 0, false))
                    Debug.Log($"Non-dungeon interior loot pile generation failed, {playerLocation} is invalid or out of range.");
            }
        }

        /// <summary>
        /// Replaces vanilla shields with custom Bossfall versions on game loads.
        /// </summary>
        /// <param name="saveData">Vanilla save data, unused in this method.</param>
        public void BossfallOnLoad(SaveData_v1 saveData)
        {
            // Variable used in this method.
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            // If player's inventory is empty, do nothing.
            if (player.Items != null)
            {
                // This search only returns armor.
                List<DaggerfallUnityItem> inventoryArmor = player.Items.SearchItems(ItemGroups.Armor);

                // If no armor found, do nothing.
                if (inventoryArmor != null)
                {
                    // I go backwards through the list as I remove objects from the list while iterating.
                    for (int i = inventoryArmor.Count - 1; i >= 0; i--)
                    {
                        DaggerfallUnityItem item = inventoryArmor[i];

                        // If not a shield, do nothing.
                        if (item.IsShield)
                        {
                            // Get save data from shield, store in a local field.
                            itemData = item.GetSaveData();

                            DaggerfallUnityItem newItem;

                            // Create a custom shield using saved field data.
                            if (item.TemplateIndex == (int)Armor.Buckler)
                                newItem = new Buckler(itemData);
                            else if (item.TemplateIndex == (int)Armor.Round_Shield)
                                newItem = new RoundShield(itemData);
                            else if (item.TemplateIndex == (int)Armor.Kite_Shield)
                                newItem = new KiteShield(itemData);
                            else
                                newItem = new TowerShield(itemData);

                            // Variables used to handle equipped shields.
                            ItemEquipTable playerEquipTable = GameManager.Instance.PlayerEntity.ItemEquipTable;
                            bool wasEquipped = false;
                            int condition = item.currentCondition;

                            // Equipped shield check.
                            if (playerEquipTable.IsEquipped(item))
                            {
                                // I unequip the old shield before destroying it.
                                player.UpdateEquippedArmorValues(playerEquipTable.UnequipItem(EquipSlots.LeftHand), false);

                                // Activate special handling for equipped shields.
                                wasEquipped = true;
                            }

                            // Remove old shield from inventory.
                            player.Items.RemoveItem(item);

                            // Add the custom shield to player's inventory.
                            player.Items.AddItem(newItem, ItemCollection.AddPosition.Front);

                            // Special handling for replacing equipped shields.
                            if (wasEquipped)
                            {
                                // Equip the custom shield in the old shield's place.
                                playerEquipTable.EquipItem(newItem, true, false);
                                player.UpdateEquippedArmorValues(newItem, true);

                                // If old shield had a Cast When Held enchantment, when I equipped the new custom one it took
                                // durability damage. I don't want to unnecessarily damage player's shield, so as a final step
                                // I restore the new shield to the old shield's condition.
                                newItem.currentCondition = condition;
                            }
                        }
                    }
                }
            }

            // Wagon items section.
            if (player.WagonItems != null)
            {
                // This search only returns armor.
                List<DaggerfallUnityItem> wagonArmor = player.WagonItems.SearchItems(ItemGroups.Armor);

                // No point checking for shields if no armor present.
                if (wagonArmor != null)
                {
                    // I go backwards through the list as I remove objects from the list while iterating.
                    for (int i = wagonArmor.Count - 1; i >= 0; i--)
                    {
                        DaggerfallUnityItem item = wagonArmor[i];

                        // Shield check. If not a shield, do nothing.
                        if (item.IsShield)
                        {
                            // Get save data from shield, store in a local field.
                            itemData = item.GetSaveData();

                            DaggerfallUnityItem newItem;

                            // Create a custom shield using saved field data.
                            if (item.TemplateIndex == (int)Armor.Buckler)
                                newItem = new Buckler(itemData);
                            else if (item.TemplateIndex == (int)Armor.Round_Shield)
                                newItem = new RoundShield(itemData);
                            else if (item.TemplateIndex == (int)Armor.Kite_Shield)
                                newItem = new KiteShield(itemData);
                            else
                                newItem = new TowerShield(itemData);

                            // Remove item from player's wagon.
                            player.WagonItems.RemoveItem(item);

                            // Add the custom shield to player's wagon.
                            player.WagonItems.AddItem(newItem, ItemCollection.AddPosition.Front);
                        }
                    }
                }
            }

            // Items that are currently being repaired section.
            if (player.OtherItems != null)
            {
                // This search only returns armor.
                List<DaggerfallUnityItem> otherArmor = player.OtherItems.SearchItems(ItemGroups.Armor);

                // No point checking for shields if no armor present.
                if (otherArmor != null)
                {
                    // I go backwards through the list as I remove objects from the list while iterating.
                    for (int i = otherArmor.Count - 1; i >= 0; i--)
                    {
                        DaggerfallUnityItem item = otherArmor[i];

                        // Shield check. If not a shield, do nothing.
                        if (item.IsShield)
                        {
                            // Get save data from shield, store in a local field.
                            itemData = item.GetSaveData();

                            DaggerfallUnityItem newItem;

                            // Create a custom shield using saved field data, which restores repair data.
                            if (item.TemplateIndex == (int)Armor.Buckler)
                                newItem = new Buckler(itemData);
                            else if (item.TemplateIndex == (int)Armor.Round_Shield)
                                newItem = new RoundShield(itemData);
                            else if (item.TemplateIndex == (int)Armor.Kite_Shield)
                                newItem = new KiteShield(itemData);
                            else
                                newItem = new TowerShield(itemData);

                            // Remove old shield from repair list.
                            player.OtherItems.RemoveItem(item);

                            // Add custom shield to repair list.
                            player.OtherItems.AddItem(newItem, ItemCollection.AddPosition.Front);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This replaces some vanilla items with custom Bossfall versions. Also implements Bossfall character creation.
        /// </summary>
        /// <param name="sender">An instance of UserInterfaceManager.</param>
        /// <param name="args">Null.</param>
        public void BossfallOnWindowChange(object sender, EventArgs args)
        {
            // Variables used in this method. Some are from the parameters, split into their base components.
            UserInterfaceManager uiManager = sender as UserInterfaceManager;
            DaggerfallTradeWindow tradeWindow = uiManager.TopWindow as DaggerfallTradeWindow;
            DaggerfallInventoryWindow inventoryWindow = uiManager.TopWindow as DaggerfallInventoryWindow;
            CreateCharCustomClass classWindow = uiManager.TopWindow as CreateCharCustomClass;
            CreateCharSpecialAdvantageWindow advDisWindow = uiManager.TopWindow as CreateCharSpecialAdvantageWindow;
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            // Trade window section. Replaces vanilla shields with Bossfall versions and replaces vanilla's magic item
            // vendor items with custom Bossfall magic items.
            if (tradeWindow != null)
            {
                // If no Merchant Items are found, do nothing.
                if (tradeWindow.MerchantItems != null)
                {
                    // I use this to determine how many magic items and soul gems I need to recreate.
                    PlayerGPS.DiscoveredBuilding buildingDiscoveryData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;

                    // This searches for armor.
                    List<DaggerfallUnityItem> merchantArmor = tradeWindow.MerchantItems.SearchItems(ItemGroups.Armor);

                    // Only magic item merchants have spellbooks for sale.
                    bool sellsMagic = tradeWindow.MerchantItems.Contains(ItemGroups.MiscItems, (int)MiscItems.Spellbook);

                    // Only soul gem merchants sell soul gems.
                    bool sellsSoulGems = tradeWindow.MerchantItems.Contains(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);

                    // Magic item and soul gem merchant section.
                    if (sellsMagic && sellsSoulGems)
                    {
                        // I replace everything with custom Bossfall items.
                        tradeWindow.MerchantItems.Clear();

                        // This begins a section of code from vanilla's GetMerchantMagicItems method from the
                        // DaggerfallGuildServicePopupWindow script, modified for Bossfall.
                        int numOfItems = (buildingDiscoveryData.quality / 2) + 1;
                        int seed = (int)(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                            / DaggerfallDateTime.MinutesPerDay);
                        UnityEngine.Random.InitState(seed);

                        for (int i = 0; i <= numOfItems; i++)
                        {
                            // Since I pass 0 as the first parameter, these magic items will be unleveled.
                            DaggerfallUnityItem magicItem = BossfallItemBuilder.Instance.CreateRandomMagicItem(0, player.Gender, player.Race);
                            magicItem.IdentifyItem();
                            tradeWindow.MerchantItems.AddItem(magicItem);
                        }

                        tradeWindow.MerchantItems.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));

                        for (int i = 0; i <= numOfItems; i++)
                        {
                            DaggerfallUnityItem magicItem;
                            if (Dice100.FailedRoll(25))
                            {
                                magicItem = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);

                                // Bossfall Soul Gems are ten times more expensive than vanilla's.
                                magicItem.value = 50000;
                                magicItem.TrappedSoulType = MobileTypes.None;
                            }
                            else
                            {
                                magicItem = BossfallItemBuilder.Instance.CreateRandomlyFilledSoulTrap();
                            }

                            tradeWindow.MerchantItems.AddItem(magicItem);
                        }
                        // This ends the section of code from vanilla's DaggerfallGuildServicePopupWindow script.
                    }

                    // Magic item merchant section.
                    else if (sellsMagic)
                    {
                        // I replace everything with custom Bossfall items.
                        tradeWindow.MerchantItems.Clear();

                        // This begins a section of code from vanilla's GetMerchantMagicItems method from the
                        // DaggerfallGuildServicePopupWindow script, modified for Bossfall.
                        int numOfItems = (buildingDiscoveryData.quality / 2) + 1;
                        int seed = (int)(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                            / DaggerfallDateTime.MinutesPerDay);
                        UnityEngine.Random.InitState(seed);

                        for (int i = 0; i <= numOfItems; i++)
                        {
                            // Since I pass 0 as the first parameter, these magic items will be unleveled.
                            DaggerfallUnityItem magicItem = BossfallItemBuilder.Instance.CreateRandomMagicItem(0, player.Gender, player.Race);
                            magicItem.IdentifyItem();
                            tradeWindow.MerchantItems.AddItem(magicItem);
                        }

                        tradeWindow.MerchantItems.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));
                        // This ends the section of code from vanilla's DaggerfallGuildServicePopupWindow script.
                    }

                    // Soul gem merchant section.
                    else if (sellsSoulGems)
                    {
                        // I replace everything with custom Bossfall items.
                        tradeWindow.MerchantItems.Clear();

                        // This begins a section of code from vanilla's GetMerchantMagicItems method from the
                        // DaggerfallGuildServicePopupWindow script, modified for Bossfall.
                        int numOfItems = (buildingDiscoveryData.quality / 2) + 1;
                        int seed = (int)(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                            / DaggerfallDateTime.MinutesPerDay);
                        UnityEngine.Random.InitState(seed);

                        for (int i = 0; i <= numOfItems; i++)
                        {
                            DaggerfallUnityItem magicItem;
                            if (Dice100.FailedRoll(25))
                            {
                                magicItem = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);

                                // Bossfall Soul Gems are ten times more expensive than vanilla's.
                                magicItem.value = 50000;
                                magicItem.TrappedSoulType = MobileTypes.None;
                            }
                            else
                            {
                                magicItem = BossfallItemBuilder.Instance.CreateRandomlyFilledSoulTrap();
                            }

                            tradeWindow.MerchantItems.AddItem(magicItem);
                        }
                        // This ends the section of code from vanilla's DaggerfallGuildServicePopupWindow script.
                    }

                    // Conventional merchant section.
                    else if (merchantArmor != null)
                    {
                        // I go backwards through the armor list as I remove elements while iterating.
                        for (int i = merchantArmor.Count - 1; i >= 0; i--)
                        {
                            DaggerfallUnityItem item = merchantArmor[i];

                            // If armor is not a shield, do nothing.
                            if (item.IsShield)
                            {
                                // Get save data from shield, store in a local field.
                                itemData = item.GetSaveData();

                                DaggerfallUnityItem newItem;

                                // Create a custom shield using saved field data.
                                if (item.TemplateIndex == (int)Armor.Buckler)
                                    newItem = new Buckler(itemData);
                                else if (item.TemplateIndex == (int)Armor.Round_Shield)
                                    newItem = new RoundShield(itemData);
                                else if (item.TemplateIndex == (int)Armor.Kite_Shield)
                                    newItem = new KiteShield(itemData);
                                else
                                    newItem = new TowerShield(itemData);

                                // Remove old shield from merchant items.
                                tradeWindow.MerchantItems.RemoveItem(item);

                                // Add new custom shield to merchant items.
                                tradeWindow.MerchantItems.AddItem(newItem, ItemCollection.AddPosition.Front);
                            }
                        }
                    }

                    // As a final step, refresh the trade window to display new items.
                    tradeWindow.Refresh();
                }
            }
            // Converts shields in loot piles and corpses to my custom shields whenever player is looting anything.
            else if (inventoryWindow != null)
            {
                // If player is not looting anything, do nothing.
                if (inventoryWindow.LootTarget != null)
                {
                    // If there are no items in the target loot container, do nothing.
                    if (inventoryWindow.LootTarget.Items != null)
                    {
                        // Check for armor among the loot.
                        List<DaggerfallUnityItem> lootArmor = inventoryWindow.LootTarget.Items.SearchItems(ItemGroups.Armor);

                        // If no armor is present, do nothing.
                        if (lootArmor != null)
                        {
                            // I go backwards through armor list as I remove elements while iterating.
                            for (int i = lootArmor.Count - 1; i >= 0; i--)
                            {
                                DaggerfallUnityItem item = lootArmor[i];

                                // If armor is not a shield, do nothing.
                                if (item.IsShield)
                                {
                                    // Get save data from shield, store in a local field.
                                    itemData = item.GetSaveData();

                                    DaggerfallUnityItem newItem;

                                    // Create a custom shield using saved field data.
                                    if (item.TemplateIndex == (int)Armor.Buckler)
                                        newItem = new Buckler(itemData);
                                    else if (item.TemplateIndex == (int)Armor.Round_Shield)
                                        newItem = new RoundShield(itemData);
                                    else if (item.TemplateIndex == (int)Armor.Kite_Shield)
                                        newItem = new KiteShield(itemData);
                                    else
                                        newItem = new TowerShield(itemData);

                                    // Remove old shield from loot.
                                    inventoryWindow.LootTarget.Items.RemoveItem(item);

                                    // Add new custom shield to loot.
                                    inventoryWindow.LootTarget.Items.AddItem(newItem, ItemCollection.AddPosition.Front);
                                }
                            }

                            // Refresh inventory window to display any new custom shields.
                            inventoryWindow.Refresh();
                        }
                    }
                }
            }
            // Character creation custom class window section. Replaces vanilla difficulty values and calculations
            // with my own.
            else if (classWindow != null)
            {
                // Window setup runs once per window instance.
                if (!customClassSetupComplete)
                {
                    // I use this window reference elsewhere in this script.
                    customClassWindow = classWindow;

                    // This coroutine will wait until the next frame to begin execution.
                    StartCoroutine(SetupCustomClassWindow());
                }
            }
            // Character creation Special Advantage/Disadvantage section. Replaces vanilla difficulty values and
            // calculations with my own.
            else if (advDisWindow != null)
            {
                // Window setup runs once per window instance.
                if (!advDisSetupComplete)
                {
                    // I use this window reference elsewhere in this script.
                    advantageWindow = advDisWindow;

                    // This coroutine will wait until the next frame to begin execution.
                    StartCoroutine(SetupAdvDisWindow());
                }
            }
        }

        /// <summary>
        /// This runs when player adds HP gained per level within the custom character creation window. It replaces
        /// vanilla's difficulty calculations with my own.
        /// </summary>
        /// <param name="sender">Unused in this method.</param>
        /// <param name="pos">Unused in this method.</param>
        void BossfallCustomClass_HitPointsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            BossfallUpdateDifficulty();
        }

        /// <summary>
        /// This runs when player subtracts HP gained per level within the custom character creation window. It replaces
        /// vanilla's difficulty calculations with my own.
        /// </summary>
        /// <param name="sender">Unused in this method.</param>
        /// <param name="pos">Unused in this method.</param>
        void BossfallCustomClass_HitPointsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            BossfallUpdateDifficulty();
        }

        /// <summary>
        /// This runs once player closes the custom class window during custom character creation. It resets previously
        /// used setup fields to defaults and deregisters from all custom class window events.
        /// </summary>
        void BossfallCustomClassWindow_OnClose()
        {
            // Deregisters custom class window event handlers.
            hitPointsUpButton.OnMouseClick -= BossfallCustomClass_HitPointsUpButton_OnMouseClick;
            hitPointsDownButton.OnMouseUp -= BossfallCustomClass_HitPointsDownButton_OnMouseClick;
            customClassWindow.OnClose -= BossfallCustomClassWindow_OnClose;

            // Reset custom class window fields to defaults.
            customClassWindow = null;
            difficultyPoints = null;
            advantageAdjust = null;
            disadvantageAdjust = null;
            createdClass = null;
            hpLabel = null;
            hitPointsUpButton = null;
            hitPointsDownButton = null;
            daggerPanel = null;

            // Finally, reset setup complete bool.
            customClassSetupComplete = false;
        }

        /// <summary>
        /// This runs when player selects certain advantages/disadvantages within the CreateCharSpecialAdvantage window.
        /// It replaces vanilla's difficulty calculations with my own.
        /// </summary>
        /// <param name="index">Unused in this method.</param>
        /// <param name="advantageName">Unused in this method.</param>
        void BossfallPrimaryPicker_OnItemPicked(int index, string advantageName)
        {
            BossfallUpdateDifficulty();
        }

        /// <summary>
        /// This runs when player selects certain advantages/disadvantages within the CreateCharSpecialAdvantage window.
        /// It replaces vanilla's difficulty calculations with my own.
        /// </summary>
        /// <param name="index">Unused in this method.</param>
        /// <param name="itemString">Unused in this method.</param>
        void BossfallSecondaryPicker_OnItemPicked(int index, string itemString)
        {
            BossfallUpdateDifficulty();
        }

        /// <summary>
        /// This runs when player removes special advantages/disadvantages from a list in the CreateCharSpecialAdvantage
        /// window. It replaces vanilla's difficulty calculations with my own.
        /// </summary>
        /// <param name="sender">Unused in this method.</param>
        /// <param name="pos">Unused in this method.</param>
        void BossfallAdvantageLabel_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            BossfallUpdateDifficulty();
        }

        /// <summary>
        /// This runs once player closes the Special Advantage/Disadvantage window during custom character creation.
        /// It resets previously used setup fields to defaults and deregisters from all Special Advantage/Disadvantage
        /// window events.
        /// </summary>
        void BossfallSpecialAdvantageWindow_OnClose()
        {
            // I use this variable to check what window is being displayed.
            UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;

            // Only reset fields and deregister event handlers if the Special Advantage/Disadvantage window is actually
            // being closed. That window's OnClose event can fire while window is still in the uiManager display stack.
            if (uiManager.TopWindow is CreateCharCustomClass)
            {
                // Deregisters some event handlers used in the special advantage/disadvantage window. This array
                // always has 14 elements.
                for (int i = 0; i < 14; i++)
                    advantageLabels[i].OnMouseClick -= BossfallAdvantageLabel_OnMouseClick;

                // Deregisters the rest of the event handlers used in the special advantage/disadvantage window.
                primaryPicker.OnItemPicked -= BossfallPrimaryPicker_OnItemPicked;
                secondaryPicker.OnItemPicked -= BossfallSecondaryPicker_OnItemPicked;
                advantageWindow.OnClose -= BossfallSpecialAdvantageWindow_OnClose;

                // Resets special advantage/disadvantage window fields to defaults.
                advantageLabels = null;
                primaryPicker = null;
                secondaryPicker = null;
                advantageWindow = null;

                // Finally, reset setup complete bool.
                advDisSetupComplete = false;
            }
        }

        #endregion
    }
}
