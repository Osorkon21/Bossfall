// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
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
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using System;
using UnityEngine;

namespace BossfallMod.Events
{
    /// <summary>
    /// Contains Bossfall's event handlers and associated data.
    /// </summary>
    public class BossfallEventHandlers : MonoBehaviour
    {
        #region Fields

        // Spell lists for BossfallOnEnemyLootSpawned event handler. Based on arrays from vanilla's EnemyEntity script.
        static readonly byte[] FrostDaedraSpells = { 0x10, 0x14, 0x03 };
        static readonly byte[] FireDaedraSpells = { 0x0E, 0x19, 0x20 };
        static readonly byte[] BossfallSpells = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14,
            0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is entirely vanilla's, pulled from PlayerActivate. It checks if struck object is an enemy.
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

        #region Events

        // DELETE WHEN IMPLEMENTED
        // For EnemyEntity reset event, there's a method in DaggerfallEntity called SpellbookCount, call that to get spellbook.Count,
        // then delete entire spellbook using for loop and DeleteSpell method in DaggerfallEntity, do this b4 adding new Bossfall spells

        /// <summary>
        /// This method adds necessary components to the PlayerObject. If successful doing so, it never runs again.
        /// </summary>
        public static void BossfallOnRespawnerComplete()
        {
            if (GameManager.Instance.PlayerObject != null)
            {
                GameManager.Instance.PlayerObject.AddComponent<BossfallPlayerActivate>();
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

        // DELETE WHEN IMPLEMENTED
        // Check BossfallOnEnemyLootSpawned & BossfallOnContainerLootSpawned for vanilla code, make sure u properly cite where
        // it comes from & any changes u made to it, make sure u do this for every line of vanilla code u use

        /// <summary>
        /// This method uses some code from vanilla's EnemyEntity script. Contrary to what the method name says, this changes a
        /// lot more than just enemy loot and is a key part of Bossfall's increased difficulty. The event passes in an instance
        /// of EnemyEntity so I use this method to unlevel human enemies, turn Assassins into bosses only if player is above
        /// level 6, give Assassins a guaranteed poisoned weapon if player is above level 1, add drugs as potential weapon
        /// poisons, change enemy starting equipment, give Guards a chance to carry items better than Iron or Steel, slightly
        /// vary monster levels, increase skill caps and skill scaling, change spell kits, change class enemy armor scaling,
        /// and scale loot with enemy level rather than player level. I also add essential Bossfall components to enemies. If
        /// enemy has a custom ID, I only add necessary Bossfall components.
        /// </summary>
        /// <param name="sender">An instance of EnemyEntity.</param>
        /// <param name="args">MobileEnemy, DFCareer, and ItemCollection data.</param>
        public static void BossfallOnEnemyLootSpawned(object sender, EnemyLootSpawnedEventArgs args)
        {
            // Variables from the event args, split into their base components.
            DaggerfallEntityBehaviour entityBehaviour = (sender as EnemyEntity).EntityBehaviour;
            EnemyEntity entity = sender as EnemyEntity;
            MobileEnemy mobileEnemy = args.MobileEnemy;
            DFCareer career = args.EnemyCareer;
            ItemCollection items = args.Items;

            DFCareer customCareer = DaggerfallEntity.GetCustomCareerTemplate(mobileEnemy.ID);
            if (customCareer != null)
            {
                // If this is a custom enemy I don't change its level or stats - I only add necessary Bossfall components.
                Destroy(entityBehaviour.gameObject.GetComponent<EnemyAttack>());
                entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
                entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
                entityBehaviour.gameObject.AddComponent<EnemyAttack>();
                return;
            }

            if (entity.EntityType == EntityTypes.EnemyMonster)
            {
                // Enemy monster levels vary up and down a bit. This affects their accuracy, dodging, and spell power.
                entity.Level = mobileEnemy.Level + UnityEngine.Random.Range(-2, 2 + 1);

                // Non-boss monster levels can't go below 1 and are capped at 20. Only Daedra Seducers are potentially
                // affected by the level 20 cap.
                if (entity.Level < 1)
                    entity.Level = 1;

                if (entity.Level > 20)
                    entity.Level = 20;

                // This manually sets boss levels.
                if (entity.CareerIndex == (int)MobileTypes.Vampire || entity.CareerIndex == (int)MobileTypes.Lich)
                    entity.Level = UnityEngine.Random.Range(21, 25 + 1);
                else if (entity.CareerIndex == (int)MobileTypes.Dragonling_Alternate
                    || entity.CareerIndex == (int)MobileTypes.OrcWarlord)
                    entity.Level = UnityEngine.Random.Range(21, 30 + 1);
                else if (entity.CareerIndex == (int)MobileTypes.VampireAncient || entity.CareerIndex == (int)MobileTypes.DaedraLord
                    || entity.CareerIndex == (int)MobileTypes.AncientLich)
                    entity.Level = UnityEngine.Random.Range(26, 30 + 1);

                // DELETE WHEN IMPLEMENTED
                // Add method of restoring data from instance field, add field of Dictionary<string ID, BossfallSaveData_v1 data>
                // up top, delete entries after u process a given ID so the next enemy has less to iterate thru
                // if (savedEnemyID == thisEnemyID && savedBossfallEnemyLevel != 0 (or != null))
                // {
                //     entity.Level = savedBossfallEnemyLevel;
                // }
            }
            else if (entity.EntityType == EntityTypes.EnemyClass)
            {
                // This ugly thing unlevels class enemies. Their levels are weighted to usually be around 10. Level 1 and
                // 20 enemies are very rare. Class enemies are unleveled once player is at least level 7.
                if (GameManager.Instance.PlayerEntity.Level > 6)
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
                // If player is level 6 or lower, all class enemies will be within 2 levels of the player.
                else
                {
                    entity.Level = GameManager.Instance.PlayerEntity.Level + UnityEngine.Random.Range(-2, 2 + 1);

                    // The enemy's level can't go below 1.
                    if (entity.Level < 1)
                        entity.Level = 1;
                }

                // Guard levels are buffed compared to vanilla DFU. They can get a 10 level boost. HALT!
                if (entity.CareerIndex == (int)MobileTypes.Knight_CityWatch - 128)
                    entity.Level += UnityEngine.Random.Range(0, 10 + 1);

                // This manually sets Assassins to boss levels, but only if player is at least level 7.
                if (GameManager.Instance.PlayerEntity.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                    entity.Level = UnityEngine.Random.Range(21, 30 + 1);

                // DELETE WHEN IMPLEMENTED
                // Add method of restoring data from instance field, add field of Dictionary<string ID, BossfallSaveData_v1 data>
                // up top, delete entries after u process a given ID so the next enemy has less to iterate thru
                // if (savedEnemyID == thisEnemyID && savedBossfallEnemyLevel != 0 (or != null))
                // {
                //     entity.Level = savedBossfallEnemyLevel;
                // }

                // Reroll MaxHealth as it likely wasn't correct the first time around.
                entity.MaxHealth = FormulaHelper.RollEnemyClassMaxHealth(entity.Level, entity.Career.HitPointsPerLevel);

                // Once player is at least level 7, Assassin HP is set to their boss range.
                if (GameManager.Instance.PlayerEntity.Level > 6 && entity.CareerIndex == (int)MobileTypes.Assassin - 128)
                    entity.MaxHealth = UnityEngine.Random.Range(100, 300 + 1);
            }

            // Bossfall enemies scale up in skill faster than vanilla's 5/level and have a skill cap of 180 - greatly
            // increased from vanilla's 100.
            short skillsLevel = (short)((entity.Level * 7) + 30);
            if (skillsLevel > 180)
                skillsLevel = 180;

            // This applies new enemy skill values as they weren't correct the first time around.
            for (int i = 0; i <= DaggerfallSkills.Count; i++)
                entity.Skills.SetPermanentSkillValue(i, skillsLevel);

            // I send loot generation to a custom script so I can scale loot generation with enemy level.
            BossfallItem.GenerateItems(mobileEnemy.LootTableKey, items, entity.Level * 50);

            // This is responsible for high level enemy loot scaling to their level. If I didn't make bosses and high level
            // monsters use this, all enemy loot would be generated by methods which do not scale with enemy level.
            if (entityType == EntityTypes.EnemyClass || careerIndex == (int)MonsterCareers.OrcSergeant
                || careerIndex == (int)MonsterCareers.OrcShaman || careerIndex == (int)MonsterCareers.OrcWarlord
                || careerIndex == (int)MonsterCareers.FrostDaedra || careerIndex == (int)MonsterCareers.FireDaedra
                || careerIndex == (int)MonsterCareers.Daedroth || careerIndex == (int)MonsterCareers.Vampire
                || careerIndex == (int)MonsterCareers.DaedraSeducer || careerIndex == (int)MonsterCareers.VampireAncient
                || careerIndex == (int)MonsterCareers.DaedraLord || careerIndex == (int)MonsterCareers.Lich
                || careerIndex == (int)MonsterCareers.AncientLich || careerIndex == (int)MonsterCareers.Dragonling_Alternate)
            {
                // I added 1 to the maximum range. There are 3 possible variants and the third is never
                // used for enemy classes in vanilla. Adds more variety.
                SetEnemyEquipment(UnityEngine.Random.Range(0, 2 + 1));
            }

            if (entityType == EntityTypes.EnemyMonster)
            {
                // I condensed the vanilla if/else if list, all monsters use the same spells in Bossfall.
                // The only exceptions are Frost/Fire Daedra, who do use different spell lists.
                if (careerIndex == (int)MonsterCareers.Imp || careerIndex == (int)MonsterCareers.OrcShaman
                    || careerIndex == (int)MonsterCareers.Wraith || careerIndex == (int)MonsterCareers.Daedroth
                    || careerIndex == (int)MonsterCareers.Vampire || careerIndex == (int)MonsterCareers.DaedraSeducer
                    || careerIndex == (int)MonsterCareers.DaedraLord || careerIndex == (int)MonsterCareers.Lich
                    || careerIndex == (int)MonsterCareers.AncientLich)
                    SetEnemySpells(AncientLichSpells);
                else if (careerIndex == (int)MonsterCareers.FrostDaedra)
                    SetEnemySpells(FrostDaedraSpells);
                else if (careerIndex == (int)MonsterCareers.FireDaedra)
                    SetEnemySpells(FireDaedraSpells);
            }
            else if (entityType == EntityTypes.EnemyClass && (mobileEnemy.CastsMagic))
            {
                // I set enemy classes to use the same expanded spell list as enemy monsters.
                SetEnemySpells(AncientLichSpells);
            }

            DaggerfallLoot.RandomlyAddMap(mobileEnemy.MapChance, items);

            if (!string.IsNullOrEmpty(mobileEnemy.LootTableKey))
            {
                DaggerfallLoot.RandomlyAddPotion(3, items);
                DaggerfallLoot.RandomlyAddPotionRecipe(2, items);
            }

            // BEGIN COPYING FROM BOSSFALL'S ENEMYENTITY FROM GITHUB BEGINNING AT THIS POINT
        }

        // DELETE WHEN IMPLEMENTED
        // Make shop quality affect what high tier materials will spawn, use notes for guidance, make good materials
        // more likely to spawn as shop quality rises - use notes for guidance

        /// <summary>
        /// This is mostly vanilla's StockShopShelves and StockHouseContainer methods from DaggerfallLoot,
        /// comments indicate changes or additions I made.
        /// </summary>
        /// <param name="args">LootContainerTypes container identifier and ItemCollection items.</param>
        public static void BossfallOnContainerLootSpawned(object sender, ContainerLootSpawnedEventArgs args)
        {
            items.Clear();

            DFLocation.BuildingTypes buildingType = buildingData.buildingType;
            int shopQuality = buildingData.quality;
            Game.Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            byte[] itemGroups = { 0 };

            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                    itemGroups = DaggerfallLootDataTables.itemGroupsAlchemist;

                    // I added "DaggerfallLoot." to the line below to send call to vanilla's DaggerfallLoot method.
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
                if (itemGroup == ItemGroups.MensClothing && playerEntity.Gender == Game.Entity.Genders.Female)
                    itemGroup = ItemGroups.WomensClothing;
                if (itemGroup == ItemGroups.WomensClothing && playerEntity.Gender == Game.Entity.Genders.Male)
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
                                    DaggerfallUnityItem item = null;
                                    if (itemGroup == ItemGroups.Weapons)

                                        // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                        // creation method.
                                        item = BossfallItem.CreateWeapon(j + Weapons.Dagger, FormulaHelper.RandomMaterial(0));
                                    else if (itemGroup == ItemGroups.Armor)

                                        // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                        // creation method.
                                        item = BossfallItem.CreateArmor(playerEntity.Gender, playerEntity.Race, j + Armor.Cuirass, FormulaHelper.RandomArmorMaterial(0));
                                    else if (itemGroup == ItemGroups.MensClothing)
                                    {
                                        item = ItemBuilder.CreateMensClothing(j + MensClothing.Straps, playerEntity.Race);
                                        item.dyeColor = ItemBuilder.RandomClothingDye();
                                    }
                                    else if (itemGroup == ItemGroups.WomensClothing)
                                    {
                                        item = ItemBuilder.CreateWomensClothing(j + WomensClothing.Brassier, playerEntity.Race);
                                        item.dyeColor = ItemBuilder.RandomClothingDye();
                                    }
                                    else if (itemGroup == ItemGroups.MagicItems)
                                    {
                                        // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                        // creation method.
                                        item = BossfallItem.CreateRandomMagicItem(0, playerEntity.Gender, playerEntity.Race);
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
                                            item = BossfallItem.CreateHolyWater();
                                        }
                                        else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_dagger))
                                        {
                                            item = BossfallItem.CreateHolyDagger();
                                        }
                                        else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_tome))
                                        {
                                            item = BossfallItem.CreateHolyTome();
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
                                        // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                        // creation method.
                                        WeaponMaterialTypes material = FormulaHelper.RandomMaterial(0);
                                        BossfallItem.ApplyWeaponMaterial(item, material);
                                    }
                                    else if (itemGroup == ItemGroups.Armor)
                                    {
                                        // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                        // creation method.
                                        ArmorMaterialTypes material = FormulaHelper.RandomArmorMaterial(0);
                                        BossfallItem.ApplyArmorSettings(item, playerEntity.Gender, playerEntity.Race, material);
                                    }

                                    items.AddItem(item);
                                }
                            }
                        }
                    }
                }
            }

            // DELETE WHEN IMPLEMENTED
            // Split this into "else" or "else if", dependent on whether loot container is a shop shelf or house container
            // "else" if only other option is house container, "else if" if there are other options than shop shelf or house container

            items.Clear();

            DFLocation.BuildingTypes buildingType = buildingData.buildingType;
            uint modelIndex = (uint)TextureRecord;
            byte[] privatePropertyList = null;
            DaggerfallUnityItem item = null;
            Game.Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

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

                // I added "UnityEngine." before "Random.Range".
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
                            // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                            // creation method.
                            item = BossfallItem.CreateRandomMagicItem(0, playerEntity.Gender, playerEntity.Race);
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
                                item = BossfallItem.CreateRandomWeapon(0);
                            else if (itemGroup == ItemGroups.Armor)

                                // I replaced "playerEntity.Level" with 0 to unlevel loot. I also call a different item
                                // creation method.
                                item = BossfallItem.CreateRandomArmor(0, playerEntity.Gender, playerEntity.Race);
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
                                    item = BossfallItem.CreateHolyWater();
                                }
                                else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_dagger))
                                {
                                    item = BossfallItem.CreateHolyDagger();
                                }
                                else if (item.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_tome))
                                {
                                    item = BossfallItem.CreateHolyTome();
                                }
                            }
                        }
                    }
                    else
                    {
                        item = ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race);
                    }
                    continueChance >>= 1;
                    if (DFRandom.rand() % 100 > continueChance)
                        keepGoing = false;
                    items.AddItem(item);
                }
            }
        }

        public static void BossfallOnStartLoad(SaveData_v1 saveData)
        {
            // DELETE WHEN IMPLEMENTED
            // U get save data, load mod save into ur cache at this time so u have it for
            // EnemyEntity event level restore. As u go thru IDs & restore levels remove IDs from cache that you've already
            // handled so u progressively have less and less in the cache to iterate thru, should end up w/empty cache @ end
        }

        public static void BossfallOnLoad(SaveData_v1 saveData)
        {
            // DELETE WHEN IMPLEMENTED
            // Use this event to write CanDetectInvisible value to restored enemies
        }

        public static void BossfallOnTabledLootSpawned(object sender, TabledLootSpawnedEventArgs args)
        {
            // DELETE WHEN IMPLEMENTED
            // This event handler will alter all loot piles in dungeons, houses, etc. This event will fire only when loot
            // piles are spawned, all enemy loot spawning is handled in BossfallOnEnemyLootSpawned
        }

        #endregion
    }
}
