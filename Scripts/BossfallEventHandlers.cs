// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net), Hazelnut
// Contributors:    vanilla DFU code Allofich, Numidium, TheLacus, Lypyl (lypyl@dfworkshop.net), Hazelnut
// 
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
using System.Collections.Generic;
using UnityEngine;

namespace BossfallMod.Events
{
    /// <summary>
    /// Contains Bossfall's event handlers and associated data.
    /// </summary>
    public class BossfallEventHandlers : MonoBehaviour
    {
        // DWI

        // Clear this script using standard citing methods, recheck entire script as u changed a lot

        // DWI

        // check contributors when done clearing make sure u have listed all, remove all that don't apply

        #region Fields

        // Spell lists for BossfallOnEnemyLootSpawned event handler. Based on arrays from vanilla's EnemyEntity script.
        static readonly byte[] BossfallFrostDaedraSpells = { 0x10, 0x14, 0x03 };
        static readonly byte[] BossfallFireDaedraSpells = { 0x0E, 0x19, 0x20 };
        static readonly byte[] BossfallGenericSpells = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10,
            0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };

        // Used to restore my custom shields.
        static ItemData_v1 itemData;

        // Stores references to enemies if game is being loaded so I can easily modify these enemies elsewhere.
        static Dictionary<ulong, DaggerfallEntityBehaviour> entityDictionary = new Dictionary<ulong, DaggerfallEntityBehaviour>();

        #endregion

        #region Properties

        /// <summary>
        /// If game is being restored, this dictionary is populated with enemy references so I can easily restore Bossfall save data.
        /// </summary>
        public static Dictionary<ulong, DaggerfallEntityBehaviour> EntityDictionary
        {
            get { return entityDictionary; }
            set { entityDictionary = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is vanilla's, pulled from PlayerActivate. I changed the method to be static but changed nothing else.
        /// It checks if struck object is an enemy.
        /// </summary>
        /// <param name="hitInfo">RaycastHit info.</param>
        /// <param name="mobileEnemy">The object being checked.</param>
        /// <returns>True if struck object is an enemy.</returns>
        static bool MobileEnemyCheck(RaycastHit hitInfo, out DaggerfallEntityBehaviour mobileEnemy)
        {
            mobileEnemy = hitInfo.transform.GetComponent<DaggerfallEntityBehaviour>();

            return mobileEnemy != null;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// This method performs setup on the PlayerObject. If successful doing so, it never runs again.
        /// </summary>
        public static void BossfallOnRespawnerComplete()
        {
            if (GameManager.Instance.PlayerObject != null)
            {
                // Adds necessary Bossfall components.
                GameManager.Instance.PlayerObject.AddComponent<BossfallPlayerActivate>();

                // Once setup is complete, don't run this method again.
                PlayerEnterExit.OnRespawnerComplete -= BossfallOnRespawnerComplete;
            }
        }

        /// <summary>
        /// If game is not being restored this method finds every random monster spawn point in the dungeon being entered,
        /// creates a Spherecast originating underneath the floor at that point, and fires that Sphere straight up. If the
        /// Spherecast hits an enemy collider I destroy that enemy and respawn a different enemy at the same position using
        /// Bossfall's unleveled encounter tables. I do this to avoid destroying main quest fixed dungeon enemies.
        /// </summary>
        /// <param name="args">DaggerfallDungeon instance of the dungeon being entered and an empty StaticDoor instance.</param>
        public static void BossfallOnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
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
                                            adjustedPosition, BossfallEncounterTables.ChooseRandomEnemy(true));

                                        // Use already created "Random Enemies" node as parent transform.
                                        waterEnemy[0].transform.parent = randomEnemiesNode.transform;

                                        // Activate enemy.
                                        waterEnemy[0].SetActive(true);
                                    }
                                    else
                                    {
                                        // Spawns a non-water enemy using Bossfall's expanded encounter tables.
                                        GameObject[] nonWaterEnemy = GameObjectHelper.CreateFoeGameObjects(
                                            adjustedPosition, BossfallEncounterTables.ChooseRandomEnemy(false));

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
        /// This method uses some code from vanilla's EnemyEntity script. Contrary to what the method name says, this changes a
        /// lot more than just enemy loot and is a key part of Bossfall's increased difficulty. The event passes in an instance
        /// of EnemyEntity so I use this method to unlevel human enemies, turn Assassins into bosses only if player is above
        /// level 6, give Assassins a guaranteed poisoned weapon if player is above level 1, add drugs as potential weapon
        /// poisons, change enemy starting equipment, give Guards a chance to carry items better than Iron or Steel, slightly
        /// vary monster levels, increase skill caps and skill scaling, change spell kits, change class enemy armor scaling,
        /// and scale loot with enemy level rather than player level. I also perform necessary Bossfall AI setup on enemies. If
        /// enemy has a custom ID, I only perform necessary Bossfall AI setup and do nothing else. I also use this method to
        /// populate a dictionary of enemy references if game is being loaded so I can restore Bossfall save data elsewhere.
        /// </summary>
        /// <param name="sender">An instance of EnemyEntity.</param>
        /// <param name="args">MobileEnemy, DFCareer, and ItemCollection data.</param>
        public static void BossfallOnEnemyLootSpawned(object sender, EnemyLootSpawnedEventArgs args)
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
                // If this is a custom enemy I don't change its level or stats - I only add necessary Bossfall components. Unity
                // executes components in the order they are attached to a given gameObject, so by destroying EnemyAttack and
                // then re-adding it BossfallEnemyAttack's Update method is executed before EnemyAttack's Update method, which
                // is necessary for Bossfall to function correctly.
                Destroy(entityBehaviour.gameObject.GetComponent<EnemyAttack>());
                entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
                entityBehaviour.gameObject.AddComponent<EnemyAttack>();
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
            BossfallItemBuilder.GenerateItems(mobileEnemy.LootTableKey, items, entity.Level * 50);

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
                BossfallItemBuilder.AssignEnemyStartingEquipment(player, entity, UnityEngine.Random.Range(0, 2 + 1));
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

                // This deletes the spellbook contents of Ghosts and Vampire Ancients so they no longer cast any spells.
                else if (entity.CareerIndex == (int)MonsterCareers.Ghost || entity.CareerIndex == (int)MonsterCareers.VampireAncient)
                {
                    for (int i = 0; i < entity.SpellbookCount(); i++)
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

            // I pulled these potion lines out of the SetEnemyCareer method in EnemyEntity and made no changes to it.
            // I include it because I clear and re-generate this enemy's ItemCollection so I have to run this again.
            if (!string.IsNullOrEmpty(mobileEnemy.LootTableKey))
            {
                DaggerfallLoot.RandomlyAddPotion(3, items);
                DaggerfallLoot.RandomlyAddPotionRecipe(2, items);
            }

            // As a finishing touch, I add essential Bossfall AI components to the enemy. Unity executes components in the order
            // they are attached to a given gameObject, so by destroying EnemyAttack and then re-adding it BossfallEnemyAttack's
            // Update method is executed before EnemyAttack's Update method, which is necessary for Bossfall to function correctly.
            Destroy(entityBehaviour.gameObject.GetComponent<EnemyAttack>());
            entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();
            entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
            entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
            entityBehaviour.gameObject.AddComponent<EnemyAttack>();

            // If game is being loaded, add this enemy to a reference dictionary so I can easily modify this enemy elsewhere.
            // Keys are unique enemy load IDs, which have already been restored at this point in the load process.
            if (SaveLoadManager.Instance.LoadInProgress)
            {
                DaggerfallEnemy dfEnemy = entityBehaviour.GetComponent<DaggerfallEnemy>();
                ulong key = dfEnemy.LoadID;
                entityDictionary.Add(key, entityBehaviour);
            }
        }

        /// <summary>
        /// This method is based on a method of the same name from vanilla's EnemyEntity, modified for Bossfall. I clear
        /// the enemy's existing spellbook and replace it with my custom list. Bossfall enemy Magicka scales differently
        /// than vanilla's, and bosses above level 25 have effectively infinite Magicka.
        /// </summary>
        /// <param name="spellList">The spell list to assign to the enemy.</param>
        /// <param name="enemyEntity">The enemy to assign the spell list to.</param>
        static void SetEnemySpells(byte[] spellList, EnemyEntity enemyEntity)
        {
            // This deletes enemy's vanilla spellbook.
            for (int i = 0; i < enemyEntity.SpellbookCount(); i++)
            {
                enemyEntity.DeleteSpell(i);
            }

            // I set MaxMagicka based on the level of the enemy. Each spell - regardless of type - costs enemy 40 Magicka.
            if (enemyEntity.Level > 0 && enemyEntity.Level < 8)
            {
                // Enough for 2 spells.
                enemyEntity.MaxMagicka = 79;
            }
            else if (enemyEntity.Level >= 8 && enemyEntity.Level < 13)
            {
                // Enough for 3 spells.
                enemyEntity.MaxMagicka = 119;
            }
            else if (enemyEntity.Level >= 13 && enemyEntity.Level < 16)
            {
                // Enough for 4 spells.
                enemyEntity.MaxMagicka = 159;
            }
            else if (enemyEntity.Level >= 16 && enemyEntity.Level < 18)
            {
                // Enough for 5 spells.
                enemyEntity.MaxMagicka = 199;
            }
            else if (enemyEntity.Level >= 18 && enemyEntity.Level < 20)
            {
                // Enough for 6 spells.
                enemyEntity.MaxMagicka = 239;
            }
            else if (enemyEntity.Level == 20)
            {
                // Enough for 8 spells.
                enemyEntity.MaxMagicka = 300;
            }
            else if (enemyEntity.Level >= 21 && enemyEntity.Level < 26)
            {
                // Enough for 30 spells.
                enemyEntity.MaxMagicka = 1199;
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
        public static void BossfallOnContainerLootSpawned(object sender, ContainerLootSpawnedEventArgs args)
        {
            // Variables I use in this method. Some are from the event args, split into their base components.
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            PlayerActivate playerActivate = sender as PlayerActivate;
            PlayerEnterExit playerEnterExit = playerActivate.GetComponent<PlayerEnterExit>();
            LootContainerTypes containerType = args.ContainerType;
            ItemCollection items = args.Loot;
            PlayerGPS.DiscoveredBuilding buildingData = playerEnterExit.BuildingDiscoveryData;

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
                            int bookMod = (shopQuality + (UnityEngine.Random.Range(-5, 5 + 1)));
                            for (int j = 0; j <= bookMod; ++j)
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
                                            item = BossfallItemBuilder.CreateWeapon(j + Weapons.Dagger, FormulaHelper.RandomMaterial(0));
                                        else if (itemGroup == ItemGroups.Armor)

                                            // I replaced "playerEntity.Level" with 0 in the FormulaHelper method call to unlevel
                                            // loot. I also call a different item creation method and replaced "playerEntity" with
                                            // "player" in the first and second parameters.
                                            item = BossfallItemBuilder.CreateArmor(player.Gender, player.Race, j + Armor.Cuirass, FormulaHelper.RandomArmorMaterial(0));
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
                                            item = BossfallItemBuilder.CreateRandomMagicItem(0, player.Gender, player.Race);
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
                                                item = BossfallItemBuilder.CreateHolyWater();
                                            }
                                            else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_dagger))
                                            {
                                                item = BossfallItemBuilder.CreateHolyDagger();
                                            }
                                            else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_tome))
                                            {
                                                item = BossfallItemBuilder.CreateHolyTome();
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
                                            BossfallItemBuilder.ApplyWeaponMaterial(item, material);
                                        }
                                        else if (itemGroup == ItemGroups.Armor)
                                        {
                                            // I replaced "playerEntity.Level" with 0 to unlevel loot.
                                            ArmorMaterialTypes material = FormulaHelper.RandomArmorMaterial(0);

                                            // I reroute the method call to a custom method. I also replaced "playerEntity"
                                            // with "player" in the second and third parameter.
                                            BossfallItemBuilder.ApplyArmorSettings(item, player.Gender, player.Race, material);
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
                                item = BossfallItemBuilder.CreateRandomMagicItem(0, player.Gender, player.Race);
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
                                    item = BossfallItemBuilder.CreateRandomWeapon(0);
                                else if (itemGroup == ItemGroups.Armor)

                                    // I replaced "playerEntity.Level" with 0 in the first parameter to unlevel loot. I
                                    // also call a different item creation method and replace "playerEntity" with "player"
                                    // in the second and third parameters.
                                    item = BossfallItemBuilder.CreateRandomArmor(0, player.Gender, player.Race);
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
                                        item = BossfallItemBuilder.CreateHolyWater();
                                    }
                                    else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_dagger))
                                    {
                                        item = BossfallItemBuilder.CreateHolyDagger();
                                    }
                                    else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_tome))
                                    {
                                        item = BossfallItemBuilder.CreateHolyTome();
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
        public static void BossfallOnTabledLootSpawned(object sender, TabledLootSpawnedEventArgs args)
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

                if (!BossfallItemBuilder.GenerateLoot(items, (int)playerDungeon, 0, true))
                    Debug.Log($"Dungeon loot pile generation failed, {playerDungeon} is invalid or out of range.");
            }

            // Only other option is a non-dungeon interior loot pile.
            else
            {
                DFRegion.LocationTypes playerLocation = GameManager.Instance.PlayerGPS.CurrentLocationType;

                if (!BossfallItemBuilder.GenerateLoot(items, (int)playerLocation, 0, false))
                    Debug.Log($"Non-dungeon interior loot pile generation failed, {playerLocation} is invalid or out of range.");
            }
        }

        /// <summary>
        /// I need to search three ItemCollections for vanilla shields to replace them with my custom ones - the player's
        /// inventory, their wagon, and their item repair list. A messy solution, but the other way would require me to
        /// register custom shield items and manually prohibit the game from ever creating them like other modded items -
        /// also, if the player ever disabled Bossfall, all of their shields would disappear as they would all be Bossfall
        /// custom shields. As I want shields to act exactly like vanilla's except have different armor values based on
        /// shield material, I found this alternate solution unacceptable. Thus, I wrote this method.
        /// </summary>
        /// <param name="saveData">Vanilla save data.</param>
        public static void BossfallOnLoad(SaveData_v1 saveData)
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
        /// This method checks if the inventory or trade windows are being opened. If they are, it searches for vanilla shields
        /// among loot or shop items and replaces them with my custom shields. In combination with BossfallOnLoad, it replaces
        /// every vanilla shield with custom shields. It also replaces all vanilla magic items and soul gems with Bossfall's
        /// magic items and soul gems.
        /// </summary>
        /// <param name="sender">An instance of UserInterfaceManager.</param>
        /// <param name="args">Null.</param>
        public static void BossfallOnWindowChange(object sender, EventArgs args)
        {
            // Variables used in this method. Some are from the parameters, split into their base components.
            UserInterfaceManager uiManager = sender as UserInterfaceManager;
            DaggerfallTradeWindow tradeWindow = uiManager.TopWindow as DaggerfallTradeWindow;
            DaggerfallInventoryWindow inventoryWindow = DaggerfallUI.Instance.InventoryWindow;
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            // Trade window section.
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
                            DaggerfallUnityItem magicItem = BossfallItemBuilder.CreateRandomMagicItem(0, player.Gender, player.Race);
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
                                magicItem = BossfallItemBuilder.CreateRandomlyFilledSoulTrap();
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
                            DaggerfallUnityItem magicItem = BossfallItemBuilder.CreateRandomMagicItem(0, player.Gender, player.Race);
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
                                magicItem = BossfallItemBuilder.CreateRandomlyFilledSoulTrap();
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
        }

        #endregion
    }
}
