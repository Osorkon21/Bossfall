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

        const int bossfallDefaultHpPerLevel = 20;

        ItemData_v1 oldShield;

        CreateCharCustomClass customClassWindow;
        CreateCharSpecialAdvantageWindow specialAdvantageWindow;
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

        bool customClassWindowSetupComplete;
        bool specialAdvantageWindowSetupComplete;
        bool bossfallPlayerActivateAdded;

        // This array is based on vanilla's FrostDaedraSpells field from the EnemyEntity script, but I added Frostbite.
        readonly byte[] BossfallFrostDaedraSpells = new byte[]
        {
            0x10, 0x14, 0x03
        };

        // This array is based on vanilla's FireDaedraSpells field from the EnemyEntity script, but I added God's Fire.
        readonly byte[] BossfallFireDaedraSpells = new byte[]
        {
            0x0E, 0x19, 0x20
        };

        readonly byte[] BossfallGenericSpells = new byte[]
        {
            0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24,
            0x27, 0x35, 0x36, 0x37, 0x40
        };

        // This Dictionary is based on vanilla DFU's difficultyDict from the CreateCharSpecialAdvantageWindow script.
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

            // Increased from vanilla's 10.
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

            // Reduced from vanilla's 14.
            { HardStrings.regenerateHealth + HardStrings.general, 6 },

            // Reduced from vanilla's 10.
            { HardStrings.regenerateHealth + HardStrings.inDarkness, 5 },

            // Reduced from vanilla's 6.
            { HardStrings.regenerateHealth + HardStrings.inLight, 3 },

            // Reduced from vanilla's 2.
            { HardStrings.regenerateHealth + HardStrings.whileImmersed, 1 },

            // Increased from vanilla's 5.
            { HardStrings.resistance, 14 },

            // I increased these values to 400. Spell Absorption is too OP.
            { HardStrings.spellAbsorption + HardStrings.general, 400 },
            { HardStrings.spellAbsorption + HardStrings.inDarkness, 400 },
            { HardStrings.spellAbsorption + HardStrings.inLight, 400 },

            { HardStrings.criticalWeakness, -14 },

            // Increased from vanilla's -6.
            { HardStrings.damage + HardStrings.fromHolyPlaces, -7 },

            // Increased from vanilla's -10.
            { HardStrings.damage + HardStrings.fromSunlight, -28 },

            // Increased from vanilla's -7.
            { HardStrings.darknessPoweredMagery + HardStrings.lowerMagicAbilityDaylight, -10 },

            // Increased from vanilla's -10.
            { HardStrings.darknessPoweredMagery + HardStrings.unableToUseMagicInDaylight, -21 },

            // Increased from vanilla's -2.
            { HardStrings.forbiddenArmorType + HardStrings.chain, -14 },

            // Increased from vanilla's -1.
            { HardStrings.forbiddenArmorType + HardStrings.leather, -7 },

            // Increased from vanilla's -5.
            { HardStrings.forbiddenArmorType + HardStrings.plate, -28 },

            // Increased from vanilla's -5.
            { HardStrings.forbiddenMaterial + HardStrings.adamantium, -8 },

            // Increased from vanilla's -2.
            { HardStrings.forbiddenMaterial + HardStrings.daedric, -28 },

            // Reduced from vanilla's -7.
            { HardStrings.forbiddenMaterial + HardStrings.dwarven, -4 },

            // Increased from vanilla's -5.
            { HardStrings.forbiddenMaterial + HardStrings.ebony, -10 },

            // Reduced from vanilla's -9.
            { HardStrings.forbiddenMaterial + HardStrings.elven, -2 },

            // Increased from vanilla's -1.
            { HardStrings.forbiddenMaterial + HardStrings.iron, -14 },

            { HardStrings.forbiddenMaterial + HardStrings.mithril, -6 },

            // Increased from vanilla's -3.
            { HardStrings.forbiddenMaterial + HardStrings.orcish, -12 },

            // Increased from vanilla's -6.
            { HardStrings.forbiddenMaterial + HardStrings.silver, -28 },

            // Increased from vanilla's -10.
            { HardStrings.forbiddenMaterial + HardStrings.steel, -42 },

            // Increased from vanilla's -1.
            { HardStrings.forbiddenShieldTypes, -3 },

            // Increased from vanilla's -2.
            { HardStrings.forbiddenWeaponry, -7 },

            // Increased from vanilla's -14.
            { HardStrings.inabilityToRegen, -28 },

            // Increased from vanilla's -10.
            { HardStrings.lightPoweredMagery + HardStrings.lowerMagicAbilityDarkness, -14 },

            // Increased from vanilla's -14.
            { HardStrings.lightPoweredMagery + HardStrings.unableToUseMagicInDarkness, -28 },

            // Increased from vanilla's -5.
            { HardStrings.lowTolerance, -7 },

            // Increased from vanilla's -4.
            { HardStrings.phobia, -7}
        };

        #endregion

        #region Properties

        public static BossfallEventHandlers Instance { get { return Bossfall.Instance.GetComponent<BossfallEventHandlers>(); } }

        public Dictionary<ulong, DaggerfallEntityBehaviour> AllRestoredEnemies { get; }
            = new Dictionary<ulong, DaggerfallEntityBehaviour>();

        #endregion

        #region Coroutines

        /// <summary>
        /// Implements Bossfall character creation.
        /// </summary>
        IEnumerator SetupCustomClassWindow()
        {
            yield return null;

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

            createdClass.HitPointsPerLevel = bossfallDefaultHpPerLevel;
            hpLabel.Text = bossfallDefaultHpPerLevel.ToString();

            hitPointsUpButton.OnMouseClick += BossfallCustomClassWindow_HitPointsUpButton_OnMouseClick;
            hitPointsDownButton.OnMouseUp += BossfallCustomClassWindow_HitPointsDownButton_OnMouseClick;
            customClassWindow.OnClose += BossfallCustomClassWindow_OnClose;

            customClassWindowSetupComplete = true;
        }

        /// <summary>
        /// Implements Bossfall character creation.
        /// </summary>
        IEnumerator SetupSpecialAdvantageWindow()
        {
            yield return null;

            Type type = specialAdvantageWindow.GetType();

            FieldInfo fieldInfo = type.GetField("difficultyDict", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo1 = type.GetField("primaryPicker", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo2 = type.GetField("secondaryPicker", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo3 = type.GetField("advantageLabels", BindingFlags.NonPublic | BindingFlags.Instance);

            primaryPicker = (DaggerfallListPickerWindow)fieldInfo1.GetValue(specialAdvantageWindow);
            secondaryPicker = (DaggerfallListPickerWindow)fieldInfo2.GetValue(specialAdvantageWindow);
            advantageLabels = (TextLabel[])fieldInfo3.GetValue(specialAdvantageWindow);

            fieldInfo.SetValue(specialAdvantageWindow, bossfallDifficultyDict);

            primaryPicker.OnItemPicked += BossfallSpecialAdvantageWindow_PrimaryPicker_OnItemPicked;
            secondaryPicker.OnItemPicked += BossfallSpecialAdvantageWindow_SecondaryPicker_OnItemPicked;
            specialAdvantageWindow.OnClose += BossfallSpecialAdvantageWindow_OnClose;

            // The advantageLabels array always contains 14 elements.
            for (int i = 0; i < 14; i++)
                advantageLabels[i].OnMouseClick += BossfallSpecialAdvantageWindow_AdvantageLabel_OnMouseClick;

            specialAdvantageWindowSetupComplete = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is vanilla's, pulled from PlayerActivate.
        /// </summary>
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
                // I added this line.
                difficultyPoints.SetValue(customClassWindow, createdClass.HitPointsPerLevel - bossfallDefaultHpPerLevel);
            }
            else
            {
                // I added this line.
                difficultyPoints.SetValue(customClassWindow, -(4 * (bossfallDefaultHpPerLevel - createdClass.HitPointsPerLevel)));
            }

            // I added this statement.
            difficultyPoints.SetValue(customClassWindow, (int)difficultyPoints.GetValue(customClassWindow)
                + (int)advantageAdjust.GetValue(customClassWindow) + (int)disadvantageAdjust.GetValue(customClassWindow));

            // I replaced vanilla's "difficultyPoints" with "(int)difficultyPoints.GetValue(customClassWindow)" in the line
            // below. I also removed the unnecessary "(float)" cast.
            createdClass.AdvancementMultiplier = 0.3f + (2.7f * ((int)difficultyPoints.GetValue(customClassWindow) + 12) / 52f);

            // I removed " = 0" from vanilla's line below.
            int daggerY;

            // I replaced vanilla's "difficultyPoints" with "(int)difficultyPoints.GetValue(customClassWindow)" in the line
            // below.
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
        /// Modifies canned class starting HP and HP/level. Adds components to PlayerObject.
        /// </summary>
        public void BossfallOnStartGame(object sender, EventArgs args)
        {
            StartGameBehaviour startGameBehaviour = sender as StartGameBehaviour;

            if (!startGameBehaviour.CharacterDocument.isCustom)
            {
                if (startGameBehaviour.CharacterDocument.career.Name != "Barbarian")
                {
                    startGameBehaviour.CharacterDocument.career.HitPointsPerLevel = bossfallDefaultHpPerLevel;

                    GameManager.Instance.PlayerEntity.MaxHealth = 45;
                    GameManager.Instance.PlayerEntity.SetHealth(45);
                    startGameBehaviour.CharacterDocument.maxHealth = 45;
                    startGameBehaviour.CharacterDocument.currentHealth = 45;
                }
            }

            if (GameManager.Instance.PlayerObject != null)
            {
                if (!bossfallPlayerActivateAdded)
                {
                    GameManager.Instance.PlayerObject.AddComponent<BossfallPlayerActivate>();
                    bossfallPlayerActivateAdded = true;
                }
            }
        }

        /// <summary>
        /// Adds components to PlayerObject.
        /// </summary>
        public void BossfallOnRespawnerComplete()
        {
            if (GameManager.Instance.PlayerObject != null)
            {
                if (!bossfallPlayerActivateAdded)
                {
                    GameManager.Instance.PlayerObject.AddComponent<BossfallPlayerActivate>();
                    bossfallPlayerActivateAdded = true;
                }

                PlayerEnterExit.OnRespawnerComplete -= BossfallOnRespawnerComplete;
            }
        }

        /// <summary>
        /// Finds, destroys, and replaces every random enemy.
        /// </summary>
        public void BossfallOnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            if (SaveLoadManager.Instance.LoadInProgress)
                return;

            // I copied this value from RDBLayout.
            const int randomMonsterMarker = 15;

            DaggerfallDungeon dungeon = args.DaggerfallDungeon;
            int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemies");
            GameObject randomEnemiesNode = GameObject.Find("Random Enemies");

            // These for loops are based on DaggerfallDungeon's EnumerateDebuggerMarkers method.
            for (int i = 0; i < dungeon.Summary.LocationData.Dungeon.Blocks.Length; i++)
            {
                DFBlock blockData = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(
                    dungeon.Summary.LocationData.Dungeon.Blocks[i].BlockName);

                UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

                for (int j = 0; j < blockData.RdbBlock.ObjectRootList.Length; j++)
                {
                    if (blockData.RdbBlock.ObjectRootList[j].RdbObjects == null)
                        continue;

                    for (int k = 0; k < blockData.RdbBlock.ObjectRootList[j].RdbObjects.Length; k++)
                    {
                        if (blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].Resources.FlatResource.TextureRecord
                            == randomMonsterMarker)
                        {
                            Vector3 unadjustedMarkerPosition = new Vector3(blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].XPos,
                                -blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].YPos,
                                blockData.RdbBlock.ObjectRootList[j].RdbObjects[k].ZPos) * MeshReader.GlobalScale;

                            Vector3 dungeonBlockPositionAdjustment =
                                new Vector3(dungeon.Summary.LocationData.Dungeon.Blocks[i].X * RDBLayout.RDBSide, 0,
                                dungeon.Summary.LocationData.Dungeon.Blocks[i].Z * RDBLayout.RDBSide);

                            Vector3 markerPosition = dungeonBlockPositionAdjustment + unadjustedMarkerPosition;

                            if (Physics.SphereCast(markerPosition + new Vector3(0, -2f, 0),
                                0.05f, Vector3.up, out RaycastHit hit, 3.5f, enemyLayerMask))
                            {
                                if (MobileEnemyCheck(hit, out DaggerfallEntityBehaviour entity))
                                {
                                    Destroy(entity.gameObject);

                                    // I copied this water level check from EnemyMotor.
                                    if ((dungeon.Summary.LocationData.Dungeon.Blocks[i].WaterLevel * -1 * MeshReader.GlobalScale)
                                        >= markerPosition.y + (100 * MeshReader.GlobalScale))
                                    {
                                        GameObject[] waterEnemy = GameObjectHelper.CreateFoeGameObjects(
                                            markerPosition, BossfallEncounterTables.Instance.ChooseRandomEnemy(true));
                                        waterEnemy[0].transform.parent = randomEnemiesNode.transform;
                                        waterEnemy[0].SetActive(true);
                                    }
                                    else
                                    {
                                        GameObject[] nonWaterEnemy = GameObjectHelper.CreateFoeGameObjects(
                                            markerPosition, BossfallEncounterTables.Instance.ChooseRandomEnemy(false));
                                        nonWaterEnemy[0].transform.parent = randomEnemiesNode.transform;
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
        /// Sets up enemies using Bossfall rules. This method uses some code from vanilla's EnemyEntity script.
        /// </summary>
        public void BossfallOnEnemyLootSpawned(object sender, EnemyLootSpawnedEventArgs args)
        {
            DaggerfallEntityBehaviour entityBehaviour = (sender as EnemyEntity).EntityBehaviour;
            EnemyEntity entity = sender as EnemyEntity;
            MobileEnemy mobileEnemy = args.MobileEnemy;
            ItemCollection items = args.Items;
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            // I pulled these next two lines from the SetEnemyCareer method in EnemyEntity and modified them for Bossfall.
            DFCareer customCareer = DaggerfallEntity.GetCustomCareerTemplate(mobileEnemy.ID);
            if (customCareer != null)
            {
                entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();

                return;
            }

            // The layout of this if/else if is based on the SetEnemyCareer method in EnemyEntity, but I wrote most of its
            // contents. Comments indicate copied vanilla code.
            if (entity.EntityType == EntityTypes.EnemyMonster)
            {
                // I pulled this line from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
                entity.Level = mobileEnemy.Level + UnityEngine.Random.Range(-2, 2 + 1);

                if (entity.Level < 1)
                    entity.Level = 1;

                if (entity.Level > 20)
                    entity.Level = 20;

                if (entity.CareerIndex == (int)MobileTypes.Vampire || entity.CareerIndex == (int)MobileTypes.Lich)
                {
                    entity.Level = UnityEngine.Random.Range(21, 25 + 1);
                }
                else if (entity.CareerIndex == (int)MobileTypes.Dragonling_Alternate
                    || entity.CareerIndex == (int)MobileTypes.OrcWarlord)
                {
                    entity.Level = UnityEngine.Random.Range(21, 30 + 1);
                }
                else if (entity.CareerIndex == (int)MobileTypes.VampireAncient
                    || entity.CareerIndex == (int)MobileTypes.DaedraLord || entity.CareerIndex == (int)MobileTypes.AncientLich)
                {
                    entity.Level = UnityEngine.Random.Range(26, 30 + 1);
                }

                // This for loop is from the SetEnemyCareer method in EnemyEntity, modified for Bossfall.
                for (int i = 0; i < entity.ArmorValues.Length; i++)
                    entity.ArmorValues[i] = (sbyte)(mobileEnemy.ArmorValue * 5);
            }
            else if (entity.EntityType == EntityTypes.EnemyClass)
            {
                // This ugly thing unlevels class enemies. Their levels are weighted to usually be around 10.
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
                    // I pulled this line from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
                    entity.Level = player.Level + UnityEngine.Random.Range(-2, 2 + 1);

                    if (entity.Level < 1)
                        entity.Level = 1;
                }

                // I pulled this from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
                if (entity.CareerIndex == (int)MobileTypes.Knight_CityWatch - 128)
                    entity.Level += UnityEngine.Random.Range(0, 10 + 1);

                if (player.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                    entity.Level = UnityEngine.Random.Range(21, 30 + 1);

                // I copied this line from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
                entity.MaxHealth = FormulaHelper.RollEnemyClassMaxHealth(entity.Level, entity.Career.HitPointsPerLevel);

                if (player.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                    entity.MaxHealth = UnityEngine.Random.Range(100, 300 + 1);

                // This for loop is from the SetEnemyEquipment method in EnemyEntity, but I wrote its contents.
                for (int i = 0; i < entity.ArmorValues.Length; i++)
                {
                    entity.ArmorValues[i] = (sbyte)(60 - (entity.Level * 2));

                    if (player.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                        entity.ArmorValues[i] = 0;
                }
            }

            // I pulled this from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
            short skillsLevel = (short)((entity.Level * 7) + 30);

            // I pulled this from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
            if (skillsLevel > 180)
                skillsLevel = 180;

            // I pulled this from the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
            for (int i = 0; i <= DaggerfallSkills.Count; i++)
                entity.Skills.SetPermanentSkillValue(i, skillsLevel);

            BossfallItemBuilder.Instance.GenerateItems(mobileEnemy.LootTableKey, items, entity.Level * 50);

            // I based this section below on a part of the SetEnemyCareer method in EnemyEntity and modified it for Bossfall.
            if (entity.EntityType == EntityTypes.EnemyClass || entity.CareerIndex == (int)MonsterCareers.OrcSergeant
                || entity.CareerIndex == (int)MonsterCareers.OrcShaman || entity.CareerIndex == (int)MonsterCareers.OrcWarlord
                || entity.CareerIndex == (int)MonsterCareers.FrostDaedra || entity.CareerIndex == (int)MonsterCareers.FireDaedra
                || entity.CareerIndex == (int)MonsterCareers.Daedroth || entity.CareerIndex == (int)MonsterCareers.Vampire
                || entity.CareerIndex == (int)MonsterCareers.DaedraSeducer || entity.CareerIndex == (int)MonsterCareers.VampireAncient
                || entity.CareerIndex == (int)MonsterCareers.DaedraLord || entity.CareerIndex == (int)MonsterCareers.Lich
                || entity.CareerIndex == (int)MonsterCareers.AncientLich || entity.CareerIndex == (int)MonsterCareers.Dragonling_Alternate)
            {
                BossfallItemBuilder.Instance.AssignEnemyStartingEquipment(player, entity, UnityEngine.Random.Range(0, 2 + 1));
            }

            // This if/else if is based on a section of the SetEnemyCareer method in EnemyEntity, modified for Bossfall.
            // I reroute SetEnemySpells method calls to a custom method.
            if (entity.EntityType == EntityTypes.EnemyMonster)
            {
                if (entity.CareerIndex == (int)MonsterCareers.Imp || entity.CareerIndex == (int)MonsterCareers.OrcShaman
                    || entity.CareerIndex == (int)MonsterCareers.Wraith || entity.CareerIndex == (int)MonsterCareers.Daedroth
                    || entity.CareerIndex == (int)MonsterCareers.Vampire || entity.CareerIndex == (int)MonsterCareers.DaedraSeducer
                    || entity.CareerIndex == (int)MonsterCareers.DaedraLord || entity.CareerIndex == (int)MonsterCareers.Lich
                    || entity.CareerIndex == (int)MonsterCareers.AncientLich)
                {
                    SetEnemySpells(BossfallGenericSpells, entity);
                }
                else if (entity.CareerIndex == (int)MonsterCareers.FrostDaedra)
                {
                    SetEnemySpells(BossfallFrostDaedraSpells, entity);
                }
                else if (entity.CareerIndex == (int)MonsterCareers.FireDaedra)
                {
                    SetEnemySpells(BossfallFireDaedraSpells, entity);
                }
                else if (entity.CareerIndex == (int)MonsterCareers.Ghost || entity.CareerIndex == (int)MonsterCareers.VampireAncient)
                {
                    for (int i = entity.SpellbookCount() - 1; i >= 0; i--)
                        entity.DeleteSpell(i);
                }
            }
            else if (entity.EntityType == EntityTypes.EnemyClass && mobileEnemy.CastsMagic)
                SetEnemySpells(BossfallGenericSpells, entity);

            // I pulled this map generation line out of the SetEnemyCareer method in EnemyEntity and made no changes to it.
            DaggerfallLoot.RandomlyAddMap(mobileEnemy.MapChance, items);

            // I pulled these potion lines out of the SetEnemyCareer method in EnemyEntity. I raised potion generation
            // chance to 4 rather than vanilla's 3.
            if (!string.IsNullOrEmpty(mobileEnemy.LootTableKey))
            {
                DaggerfallLoot.RandomlyAddPotion(4, items);
                DaggerfallLoot.RandomlyAddPotionRecipe(2, items);
            }

            entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
            entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
            entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();

            if (SaveLoadManager.Instance.LoadInProgress)
            {
                DaggerfallEnemy dfEnemy = entityBehaviour.GetComponent<DaggerfallEnemy>();
                ulong key = dfEnemy.LoadID;

                AllRestoredEnemies.Add(key, entityBehaviour);
            }
        }

        /// <summary>
        /// This method is based on a method of the same name from vanilla's EnemyEntity, modified for Bossfall. Comments
        /// indicate changes or additions I made.
        /// </summary>
        void SetEnemySpells(byte[] spellList, EnemyEntity enemyEntity)
        {
            // This deletes enemy's vanilla spellbook.
            for (int i = enemyEntity.SpellbookCount() - 1; i >= 0; i--)
                enemyEntity.DeleteSpell(i);

            // I set MaxMagicka based on enemy's level. Each spell - regardless of type - costs enemy 40 Magicka.
            if (enemyEntity.Level > 0 && enemyEntity.Level < 8)
            {
                enemyEntity.MaxMagicka = 80;
            }
            else if (enemyEntity.Level >= 8 && enemyEntity.Level < 13)
            {
                enemyEntity.MaxMagicka = 120;
            }
            else if (enemyEntity.Level >= 13 && enemyEntity.Level < 16)
            {
                enemyEntity.MaxMagicka = 160;
            }
            else if (enemyEntity.Level >= 16 && enemyEntity.Level < 18)
            {
                enemyEntity.MaxMagicka = 200;
            }
            else if (enemyEntity.Level >= 18 && enemyEntity.Level < 20)
            {
                enemyEntity.MaxMagicka = 240;
            }
            else if (enemyEntity.Level == 20)
            {
                enemyEntity.MaxMagicka = 320;
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

            // This foreach is from the SetEnemySpells method in EnemyEntity, modified for Bossfall.
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
        public void BossfallOnContainerLootSpawned(object sender, ContainerLootSpawnedEventArgs args)
        {
            // I added these variables.
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            PlayerActivate playerActivate = sender as PlayerActivate;
            PlayerEnterExit playerEnterExit = playerActivate.GetComponent<PlayerEnterExit>();
            LootContainerTypes containerType = args.ContainerType;
            ItemCollection items = args.Loot;
            PlayerGPS.DiscoveredBuilding buildingData = playerEnterExit.BuildingDiscoveryData;

            // I added this.
            if (buildingData.buildingKey == 0)
                return;

            // I added this if/else.
            if (containerType == LootContainerTypes.ShopShelves)
            {
                // The below line begins vanilla's StockShopShelves method copied from DaggerfallLoot.
                items.Clear();

                DFLocation.BuildingTypes buildingType = buildingData.buildingType;
                int shopQuality = buildingData.quality;
                ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
                byte[] itemGroups = { 0 };

                switch (buildingType)
                {
                    case DFLocation.BuildingTypes.Alchemist:
                        itemGroups = DaggerfallLootDataTables.itemGroupsAlchemist;

                        // I added "DaggerfallLoot." to the line below.
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
                        // I thought General Stores and Pawn Shops stocked too many books. This new check greatly
                        // reduces book generation for these types of stores.
                        if (itemGroup == ItemGroups.Books && buildingType != DFLocation.BuildingTypes.Bookseller
                            && buildingType != DFLocation.BuildingTypes.Library)
                        {
                            if (shopQuality > 7 && Dice100.SuccessRoll(5))
                                items.AddItem(ItemBuilder.CreateRandomBook());

                            if (shopQuality > 13 && Dice100.SuccessRoll(15))
                                items.AddItem(ItemBuilder.CreateRandomBook());

                            if (shopQuality > 17 && Dice100.SuccessRoll(25))
                                items.AddItem(ItemBuilder.CreateRandomBook());
                        }
                        // I thought Booksellers didn't stock enough books. This new check greatly increases
                        // the amount of books on Bookseller shelves.
                        else if (itemGroup == ItemGroups.Books && buildingType == DFLocation.BuildingTypes.Bookseller)
                        {
                            int booksInStock = (shopQuality / 2) + UnityEngine.Random.Range(-7, 7 + 1);

                            if (booksInStock < 1)
                                booksInStock = 1;

                            for (int j = 0; j < booksInStock; j++)
                                items.AddItem(ItemBuilder.CreateRandomBook());
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

                                            // These new checks create Holy items with custom enchantments. Only Pawn
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
                // The above line ends vanilla's StockShopShelves method copied from DaggerfallLoot.
            }
            else
            {
                // This begins a code section copied from vanilla's Update method from the PlayerActivate script, modified
                // for Bossfall. I'm sure I am missing an obvious solution, but I couldn't figure out how to get the house
                // container's DaggerfallLoot component and I need its TextureRecord, so I fire a new ray and check the
                // RaycastHit for DaggerfallLoot.

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
                // The above line ends the code section copied from vanilla's Update method from the PlayerActivate script.

                // I added this check.
                if (!Physics.Raycast(ray, out RaycastHit hit, PlayerActivate.TreasureActivationDistance, playerLayerMask))
                {
                    Debug.Log("Raycast missed house container.");

                    return;
                }

                // I added this check.
                if (hit.transform.GetComponent<DaggerfallLoot>() == null)
                {
                    Debug.Log("The struck object had no DaggerfallLoot component.");

                    return;
                }

                // I added this line.
                DaggerfallLoot houseContainer = hit.transform.GetComponent<DaggerfallLoot>();

                // The below line begins vanilla's StockHouseContainer method copied from DaggerfallLoot.
                items.Clear();

                DFLocation.BuildingTypes buildingType = buildingData.buildingType;

                // I added "houseContainer." to the line below.
                uint modelIndex = (uint)houseContainer.TextureRecord;

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

                                    // These new checks create Holy items with custom enchantments. I don't know which 
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
                // The above line ends vanilla's StockHouseContainer method copied from DaggerfallLoot.
            }
        }

        /// <summary>
        /// Generates items for all dungeon and building interior loot piles.
        /// </summary>
        public void BossfallOnTabledLootSpawned(object sender, TabledLootSpawnedEventArgs args)
        {
            ItemCollection items = args.Items;
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;

            if (playerEnterExit.IsCreatingDungeonObjects)
            {
                DFRegion.DungeonTypes playerDungeon = playerEnterExit.Dungeon.Summary.DungeonType;

                if (!BossfallItemBuilder.Instance.GenerateLoot(items, (int)playerDungeon, 0, true))
                    Debug.Log($"Dungeon loot pile generation failed, {playerDungeon} is invalid or out of range.");
            }
            else
            {
                DFRegion.LocationTypes playerLocation = GameManager.Instance.PlayerGPS.CurrentLocationType;

                if (!BossfallItemBuilder.Instance.GenerateLoot(items, (int)playerLocation, 0, false))
                    Debug.Log($"Non-dungeon interior loot pile generation failed, {playerLocation} is invalid or out of range.");
            }
        }

        /// <summary>
        /// Replaces vanilla shields with custom Bossfall versions.
        /// </summary>
        public void BossfallOnLoad(SaveData_v1 saveData)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            if (player.Items.Count > 0)
            {
                List<DaggerfallUnityItem> inventoryArmor = player.Items.SearchItems(ItemGroups.Armor);

                if (inventoryArmor.Count > 0)
                {
                    for (int i = inventoryArmor.Count - 1; i >= 0; i--)
                    {
                        DaggerfallUnityItem armor = inventoryArmor[i];

                        if (armor.IsShield)
                        {
                            oldShield = armor.GetSaveData();

                            DaggerfallUnityItem newShield;

                            if (armor.TemplateIndex == (int)Armor.Buckler)
                                newShield = new Buckler(oldShield);
                            else if (armor.TemplateIndex == (int)Armor.Round_Shield)
                                newShield = new RoundShield(oldShield);
                            else if (armor.TemplateIndex == (int)Armor.Kite_Shield)
                                newShield = new KiteShield(oldShield);
                            else
                                newShield = new TowerShield(oldShield);

                            ItemEquipTable playerEquipTable = GameManager.Instance.PlayerEntity.ItemEquipTable;
                            bool wasEquipped = false;
                            int oldCondition = armor.currentCondition;

                            if (playerEquipTable.IsEquipped(armor))
                            {
                                player.UpdateEquippedArmorValues(playerEquipTable.UnequipItem(EquipSlots.LeftHand), false);
                                wasEquipped = true;
                            }

                            player.Items.RemoveItem(armor);
                            player.Items.AddItem(newShield, ItemCollection.AddPosition.Front);

                            if (wasEquipped)
                            {
                                playerEquipTable.EquipItem(newShield, true, false);
                                player.UpdateEquippedArmorValues(newShield, true);
                                newShield.currentCondition = oldCondition;
                            }
                        }
                    }
                }
            }

            if (player.WagonItems.Count > 0)
            {
                List<DaggerfallUnityItem> wagonArmor = player.WagonItems.SearchItems(ItemGroups.Armor);

                if (wagonArmor.Count > 0)
                {
                    for (int i = wagonArmor.Count - 1; i >= 0; i--)
                    {
                        DaggerfallUnityItem armor = wagonArmor[i];

                        if (armor.IsShield)
                        {
                            oldShield = armor.GetSaveData();

                            DaggerfallUnityItem newShield;

                            if (armor.TemplateIndex == (int)Armor.Buckler)
                                newShield = new Buckler(oldShield);
                            else if (armor.TemplateIndex == (int)Armor.Round_Shield)
                                newShield = new RoundShield(oldShield);
                            else if (armor.TemplateIndex == (int)Armor.Kite_Shield)
                                newShield = new KiteShield(oldShield);
                            else
                                newShield = new TowerShield(oldShield);

                            player.WagonItems.RemoveItem(armor);
                            player.WagonItems.AddItem(newShield, ItemCollection.AddPosition.Front);
                        }
                    }
                }
            }

            if (player.OtherItems.Count > 0)
            {
                List<DaggerfallUnityItem> armorBeingRepaired = player.OtherItems.SearchItems(ItemGroups.Armor);

                if (armorBeingRepaired.Count > 0)
                {
                    for (int i = armorBeingRepaired.Count - 1; i >= 0; i--)
                    {
                        DaggerfallUnityItem armor = armorBeingRepaired[i];

                        if (armor.IsShield)
                        {
                            oldShield = armor.GetSaveData();

                            DaggerfallUnityItem newShield;

                            if (armor.TemplateIndex == (int)Armor.Buckler)
                                newShield = new Buckler(oldShield);
                            else if (armor.TemplateIndex == (int)Armor.Round_Shield)
                                newShield = new RoundShield(oldShield);
                            else if (armor.TemplateIndex == (int)Armor.Kite_Shield)
                                newShield = new KiteShield(oldShield);
                            else
                                newShield = new TowerShield(oldShield);

                            player.OtherItems.RemoveItem(armor);
                            player.OtherItems.AddItem(newShield, ItemCollection.AddPosition.Front);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This replaces some vanilla items with custom Bossfall versions. Also implements Bossfall character creation.
        /// </summary>
        public void BossfallOnWindowChange(object sender, EventArgs args)
        {
            UserInterfaceManager uiManager = sender as UserInterfaceManager;
            DaggerfallTradeWindow tradeWindow = uiManager.TopWindow as DaggerfallTradeWindow;
            DaggerfallInventoryWindow inventoryWindow = uiManager.TopWindow as DaggerfallInventoryWindow;
            CreateCharCustomClass custClassWindow = uiManager.TopWindow as CreateCharCustomClass;
            CreateCharSpecialAdvantageWindow specAdvantageWindow = uiManager.TopWindow as CreateCharSpecialAdvantageWindow;
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            if (tradeWindow != null)
            {
                if (tradeWindow.MerchantItems.Count > 0)
                {
                    List<DaggerfallUnityItem> merchantArmor = tradeWindow.MerchantItems.SearchItems(ItemGroups.Armor);

                    bool sellsMagicItems = tradeWindow.MerchantItems.Contains(ItemGroups.MiscItems, (int)MiscItems.Spellbook);
                    bool sellsSoulGems = tradeWindow.MerchantItems.Contains(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);

                    if (sellsMagicItems && sellsSoulGems)
                    {
                        tradeWindow.MerchantItems.Clear();

                        PlayerGPS.DiscoveredBuilding buildingDiscoveryData
                            = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;

                        // This begins a section of code from vanilla's GetMerchantMagicItems method from the
                        // DaggerfallGuildServicePopupWindow script, modified for Bossfall.
                        int numOfItems = (buildingDiscoveryData.quality / 2) + 1;
                        int seed = (int)(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                            / DaggerfallDateTime.MinutesPerDay);
                        UnityEngine.Random.InitState(seed);

                        for (int i = 0; i <= numOfItems; i++)
                        {
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
                        // The above line ends the section of code from vanilla's DaggerfallGuildServicePopupWindow script.

                        tradeWindow.Refresh();
                    }
                    else if (sellsMagicItems)
                    {
                        tradeWindow.MerchantItems.Clear();

                        PlayerGPS.DiscoveredBuilding buildingDiscoveryData
                            = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;

                        // This begins a section of code from vanilla's GetMerchantMagicItems method from the
                        // DaggerfallGuildServicePopupWindow script, modified for Bossfall.
                        int numOfItems = (buildingDiscoveryData.quality / 2) + 1;
                        int seed = (int)(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                            / DaggerfallDateTime.MinutesPerDay);
                        UnityEngine.Random.InitState(seed);

                        for (int i = 0; i <= numOfItems; i++)
                        {
                            DaggerfallUnityItem magicItem = BossfallItemBuilder.Instance.CreateRandomMagicItem(0, player.Gender, player.Race);
                            magicItem.IdentifyItem();
                            tradeWindow.MerchantItems.AddItem(magicItem);
                        }

                        tradeWindow.MerchantItems.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));
                        // The above line ends the section of code from vanilla's DaggerfallGuildServicePopupWindow script.

                        tradeWindow.Refresh();
                    }
                    else if (sellsSoulGems)
                    {
                        tradeWindow.MerchantItems.Clear();

                        PlayerGPS.DiscoveredBuilding buildingDiscoveryData
                            = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;

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
                        // The above line ends the section of code from vanilla's DaggerfallGuildServicePopupWindow script.

                        tradeWindow.Refresh();
                    }
                    else if (merchantArmor.Count > 0)
                    {
                        bool refreshWindow = false;

                        for (int i = merchantArmor.Count - 1; i >= 0; i--)
                        {
                            DaggerfallUnityItem armor = merchantArmor[i];

                            if (armor.IsShield)
                            {
                                oldShield = armor.GetSaveData();

                                DaggerfallUnityItem newShield;

                                if (armor.TemplateIndex == (int)Armor.Buckler)
                                    newShield = new Buckler(oldShield);
                                else if (armor.TemplateIndex == (int)Armor.Round_Shield)
                                    newShield = new RoundShield(oldShield);
                                else if (armor.TemplateIndex == (int)Armor.Kite_Shield)
                                    newShield = new KiteShield(oldShield);
                                else
                                    newShield = new TowerShield(oldShield);

                                tradeWindow.MerchantItems.RemoveItem(armor);
                                tradeWindow.MerchantItems.AddItem(newShield, ItemCollection.AddPosition.Front);

                                if (!refreshWindow)
                                    refreshWindow = true;
                            }
                        }

                        if (refreshWindow)
                            tradeWindow.Refresh();
                    }
                }
            }
            else if (inventoryWindow != null)
            {
                if (inventoryWindow.LootTarget != null)
                {
                    if (inventoryWindow.LootTarget.Items.Count > 0)
                    {
                        List<DaggerfallUnityItem> lootArmor = inventoryWindow.LootTarget.Items.SearchItems(ItemGroups.Armor);

                        if (lootArmor.Count > 0)
                        {
                            bool refreshWindow = false;

                            for (int i = lootArmor.Count - 1; i >= 0; i--)
                            {
                                DaggerfallUnityItem armor = lootArmor[i];

                                if (armor.IsShield)
                                {
                                    oldShield = armor.GetSaveData();

                                    DaggerfallUnityItem newShield;

                                    if (armor.TemplateIndex == (int)Armor.Buckler)
                                        newShield = new Buckler(oldShield);
                                    else if (armor.TemplateIndex == (int)Armor.Round_Shield)
                                        newShield = new RoundShield(oldShield);
                                    else if (armor.TemplateIndex == (int)Armor.Kite_Shield)
                                        newShield = new KiteShield(oldShield);
                                    else
                                        newShield = new TowerShield(oldShield);

                                    inventoryWindow.LootTarget.Items.RemoveItem(armor);
                                    inventoryWindow.LootTarget.Items.AddItem(newShield, ItemCollection.AddPosition.Front);

                                    if (!refreshWindow)
                                        refreshWindow = true;
                                }
                            }

                            if (refreshWindow)
                                inventoryWindow.Refresh();
                        }
                    }
                }
            }
            else if (custClassWindow != null)
            {
                if (!customClassWindowSetupComplete)
                {
                    customClassWindow = custClassWindow;

                    StartCoroutine(SetupCustomClassWindow());
                }
            }
            else if (specAdvantageWindow != null)
            {
                if (!specialAdvantageWindowSetupComplete)
                {
                    specialAdvantageWindow = specAdvantageWindow;

                    StartCoroutine(SetupSpecialAdvantageWindow());
                }
            }
        }

        void BossfallCustomClassWindow_HitPointsUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            BossfallUpdateDifficulty();
        }

        void BossfallCustomClassWindow_HitPointsDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            BossfallUpdateDifficulty();
        }

        void BossfallCustomClassWindow_OnClose()
        {
            hitPointsUpButton.OnMouseClick -= BossfallCustomClassWindow_HitPointsUpButton_OnMouseClick;
            hitPointsDownButton.OnMouseUp -= BossfallCustomClassWindow_HitPointsDownButton_OnMouseClick;
            customClassWindow.OnClose -= BossfallCustomClassWindow_OnClose;

            customClassWindow = null;
            difficultyPoints = null;
            advantageAdjust = null;
            disadvantageAdjust = null;
            createdClass = null;
            hpLabel = null;
            hitPointsUpButton = null;
            hitPointsDownButton = null;
            daggerPanel = null;

            customClassWindowSetupComplete = false;
        }

        void BossfallSpecialAdvantageWindow_PrimaryPicker_OnItemPicked(int index, string advantageName)
        {
            BossfallUpdateDifficulty();
        }

        void BossfallSpecialAdvantageWindow_SecondaryPicker_OnItemPicked(int index, string itemString)
        {
            BossfallUpdateDifficulty();
        }

        void BossfallSpecialAdvantageWindow_AdvantageLabel_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            BossfallUpdateDifficulty();
        }

        void BossfallSpecialAdvantageWindow_OnClose()
        {
            UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;

            if (uiManager.TopWindow is CreateCharCustomClass)
            {
                // This array always has 14 elements.
                for (int i = 0; i < 14; i++)
                    advantageLabels[i].OnMouseClick -= BossfallSpecialAdvantageWindow_AdvantageLabel_OnMouseClick;

                primaryPicker.OnItemPicked -= BossfallSpecialAdvantageWindow_PrimaryPicker_OnItemPicked;
                secondaryPicker.OnItemPicked -= BossfallSpecialAdvantageWindow_SecondaryPicker_OnItemPicked;
                specialAdvantageWindow.OnClose -= BossfallSpecialAdvantageWindow_OnClose;

                advantageLabels = null;
                primaryPicker = null;
                secondaryPicker = null;
                specialAdvantageWindow = null;

                specialAdvantageWindowSetupComplete = false;
            }
        }

        #endregion
    }
}
