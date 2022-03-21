// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code InconsolableCellist, Allofich, Hazelnut, Numidium
//
// 
// Notes: This script uses code from several vanilla scripts. Comments indicate authorship, please verify
//        authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using DaggerfallConnect.FallExe;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BossfallMod.Items
{
    /// <summary>
    /// Custom loot spawning methods.
    /// </summary>
    public class BossfallItemBuilder : MonoBehaviour
    {
        #region Fields

        // The following 11 fields are vanilla fields from ItemBuilder. I changed all the arrays to be instance fields.
        const int chooseAtRandom = -1;

        readonly short[] weightMultipliersByMaterial = new short[]
        {
            4, 5, 4, 4, 3, 4, 4, 2, 4, 5
        };

        readonly short[] valueMultipliersByMaterial = new short[]
        {
            1, 2, 4, 8, 16, 32, 64, 128, 256, 512
        };

        readonly int[] extraSpellPtsEnchantPts = new int[]
        {
            0x1F4, 0x1F4, 0x1F4, 0x1F4, 0xC8, 0xC8, 0xC8, 0x2BC, 0x320, 0x384, 0x3E8
        };

        readonly int[] potentVsEnchantPts = new int []
        {
            0x320, 0x384, 0x3E8, 0x4B0
        };

        readonly int[] regensHealthEnchantPts = new int[]
        {
            0x0FA0, 0x0BB8, 0x0BB8
        };

        readonly int[] vampiricEffectEnchantPts = new int[]
        {
            0x7D0, 0x3E8
        };

        readonly int[] increasedWeightAllowanceEnchantPts = new int[]
        {
            0x190, 0x258
        };

        readonly int[] improvesTalentsEnchantPts = new int[]
        {
            0x1F4, 0x258, 0x258
        };

        readonly int[] goodRepWithEnchantPts = new int[]
        {
            0x3E8, 0x3E8, 0x3E8, 0x3E8, 0x3E8, 0x1388
        };

        readonly ushort[] enchantmentPointCostsForNonParamTypes = new ushort[]
        {
            0, 0x0F448, 0x0F63C, 0x0FF9C, 0x0FD44, 0, 0, 0, 0x384, 0x5DC, 0x384, 0x64, 0x2BC
        };

        // Vanilla's array from ItemBuilder, changed to be a non-readonly instance field. I assign vanilla's value to this
        // field in Awake.
        int[][] enchantmentPtsForItemPowerArrays;

        // This array represents materials, Iron through Daedric. Durability decreases as material tier increases.
        readonly short[] bossfallConditionMultipliersByMaterial = new short[]
        {
            2, 3, 2, 2, 2, 1, 1, 1, 1, 1
        };

        // This array is mostly vanilla code from LootTables, but I changed it to be a readonly instance method. I also changed
        // some numbers for Bossfall's custom loot generation.
        readonly LootChanceMatrix[] bossfallLootTables = new LootChanceMatrix[]
        {
            new LootChanceMatrix() {key = "-", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "A", MinGold = 1, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = 2, CL = 4, BK = 0, M2 = 2, RL = 0 },
            new LootChanceMatrix() {key = "B", MinGold = 0, MaxGold = 0, P1 = 10, P2 = 10, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "C", MinGold = 1, MaxGold = 20, P1 = 10, P2 = 10, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 25, MI = 3, CL = 0, BK = 2, M2 = 2, RL = 2 },
            new LootChanceMatrix() {key = "D", MinGold = 1, MaxGold = 4, P1 = 6, P2 = 6, C1 = 6, C2 = 6, C3 = 6, M1 = 6, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 4 },
            new LootChanceMatrix() {key = "E", MinGold = 1, MaxGold = 80, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 10, WP = 10, MI = 3, CL = 4, BK = 2, M2 = 1, RL = 15 },
            new LootChanceMatrix() {key = "F", MinGold = 1, MaxGold = 30, P1 = 2, P2 = 2, C1 = 5, C2 = 5, C3 = 5, M1 = 2, AM = 50, WP = 50, MI = 1, CL = 0, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "G", MinGold = 1, MaxGold = 15, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 50, WP = 50, MI = 1, CL = 5, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "H", MinGold = 1, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 100, MI = 1, CL = 2, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "I", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 2, CL = 0, BK = 0, M2 = 0, RL = 5 },
            new LootChanceMatrix() {key = "J", MinGold = 1, MaxGold = 150, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = 3, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "K", MinGold = 1, MaxGold = 10, P1 = 3, P2 = 3, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 5, WP = 5, MI = 3, CL = 0, BK = 5, M2 = 2, RL = 100 },
            new LootChanceMatrix() {key = "L", MinGold = 1, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 50, WP = 50, MI = 1, CL = 75, BK = 0, M2 = 5, RL = 3 },
            new LootChanceMatrix() {key = "M", MinGold = 1, MaxGold = 15, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 2, AM = 10, WP = 10, MI = 1, CL = 15, BK = 2, M2 = 3, RL = 1 },
            new LootChanceMatrix() {key = "N", MinGold = 1, MaxGold = 80, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 5, MI = 1, CL = 20, BK = 5, M2 = 2, RL = 5 },
            new LootChanceMatrix() {key = "O", MinGold = 1, MaxGold = 20, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 1, AM = 10, WP = 15, MI = 2, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "P", MinGold = 1, MaxGold = 20, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 10, MI = 2, CL = 0, BK = 10, M2 = 5, RL = 0 },
            new LootChanceMatrix() {key = "Q", MinGold = 1, MaxGold = 80, P1 = 2, P2 = 2, C1 = 8, C2 = 8, C3 = 8, M1 = 2, AM = 10, WP = 25, MI = 3, CL = 35, BK = 5, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "R", MinGold = 1, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 5, AM = 5, WP = 15, MI = 2, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "S", MinGold = 1, MaxGold = 125, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 15, AM = 10, WP = 10, MI = 3, CL = 0, BK = 5, M2 = 5, RL = 0 },
            new LootChanceMatrix() {key = "T", MinGold = 1, MaxGold = 80, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 100, WP = 100, MI = 1, CL = 0, BK = 0, M2 = 0, RL = 0},
            new LootChanceMatrix() {key = "U", MinGold = 1, MaxGold = 30, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 10, AM = 10, WP = 10, MI = 2, CL = 0, BK = 2, M2 = 2, RL = 10 },
        };

        #endregion

        #region Properties

        public static BossfallItemBuilder Instance { get { return Bossfall.Instance.GetComponent<BossfallItemBuilder>(); } }

        #endregion

        #region Unity

        void Awake()
        {
            // I assign vanilla's value from the ItemBuilder script to this field, which is also from vanilla's ItemBuilder
            // script.
            enchantmentPtsForItemPowerArrays = new int[][]
            {
                null, null, null, extraSpellPtsEnchantPts, potentVsEnchantPts, regensHealthEnchantPts, vampiricEffectEnchantPts,
                increasedWeightAllowanceEnchantPts, null, null, null, null, null, improvesTalentsEnchantPts,
                goodRepWithEnchantPts
            };
        }

        #endregion

        #region Loot Generation

        /// <summary>
        /// Vanilla's method from DaggerfallLoot, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="LootTableKey">The loot table key.</param>
        /// <param name="collection">The item collection being altered.</param>
        /// <param name="enemyLevelModifier">If spawning loot for an enemy, enemy level * 50. Otherwise, 0.</param>
        public void GenerateItems(string LootTableKey, ItemCollection collection, int enemyLevelModifier = 0)
        {
            // I reroute the call to a method in this script.
            LootChanceMatrix matrix = GetMatrix(LootTableKey);

            // I reroute method calls to a method in this script so I can easily customize loot generation. I pass on
            // enemyLevelModifier so loot scales with enemy level rather than the player's.
            DaggerfallUnityItem[] newitems = GenerateRandomLoot(matrix, GameManager.Instance.PlayerEntity, enemyLevelModifier);

            collection.Import(newitems);
        }

        /// <summary>
        /// Vanilla's method from LootTables, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="items">I changed DaggerfallLoot to ItemCollection to better fit event args.</param>
        /// <param name="locationIndex">Current location.</param>
        /// <param name="locationModifier">I added this parameter. I plan on varying loot contents/quality by location.</param>
        /// <param name="isDungeon">I added this parameter. I plan on varying loot contents/quality by location.</param>
        /// <returns>True if locationIndex yields a valid loot table key, false otherwise.</returns>
        public bool GenerateLoot(ItemCollection items, int locationIndex, int locationModifier = 0, bool isDungeon = true)
        {
            string[] lootTableKeys = {               // Travel map outdoor location mapped to this lootTableKeys index
            "K", // DungeonTypes Crypt                  LocationTypes TownCity          Walled town
            "N", // DungeonTypes Orc Stronghold         LocationTypes TownHamlet        Medium town 
            "N", // DungeonTypes Human Stronghold       LocationTypes TownVillage       Small town
            "N", // DungeonTypes Prison                 LocationTypes HomeFarms         Grange, etc.
            "K", // DungeonTypes Desecrated Temple      LocationTypes DungeonLabyrinth  Large dungeon
            "M", // DungeonTypes Mine                   LocationTypes ReligionTemple    Regional temple
            "M", // DungeonTypes Natural Cave           LocationTypes Tavern            Pub, etc.
            "Q", // DungeonTypes Coven                  LocationTypes DungeonKeep       Medium dungeon
            "K", // DungeonTypes Vampire Haunt          LocationTypes HomeWealthy       Manor, etc.
            "U", // DungeonTypes Laboratory             LocationTypes ReligionCult      Shrine
            "D", // DungeonTypes Harpy Nest             LocationTypes DungeonRuin       Small dungeon
            "N", // DungeonTypes Ruined Castle          LocationTypes HomePoor          Hovel, etc.
            "L", // DungeonTypes Spider Nest            LocationTypes Graveyard         Cemetery
            "F", // DungeonTypes Giant Stronghold       LocationTypes Coven             Witch coven
            "S", // DungeonTypes Dragon's Den           LocationTypes HomeYourShips     Player's ship
            "N", // DungeonTypes Barbarian Stronghold
            "M", // DungeonTypes Volcanic Caves
            "L", // DungeonTypes Scorpion Nest
            "N", // DungeonTypes Cemetery
            };

            if (locationIndex < lootTableKeys.Length)
            {
                // I reroute the method call to a method in this script. I added the (currently unused)
                // locationModifier parameter at the end as I plan on varying loot pile contents and quality depending
                // on player's current location. I also replaced "loot.Items" in the second parameter with "items".
                GenerateItems(lootTableKeys[locationIndex], items, locationModifier);
                char key = lootTableKeys[locationIndex][0];
                int alphabetIndex = key - 64;

                if (alphabetIndex >= 10 && alphabetIndex <= 15)
                {
                    int[] mapChances = { 2, 1, 1, 2, 2, 15 };
                    int mapChance = mapChances[alphabetIndex - 10];

                    // In the following three lines I replaced "loot.Items" with "items".
                    DaggerfallLoot.RandomlyAddMap(mapChance, items);
                    DaggerfallLoot.RandomlyAddPotion(4, items);
                    DaggerfallLoot.RandomlyAddPotionRecipe(2, items);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Vanilla's method from LootTables, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="key">Key of matrix to get.</param>
        /// <returns>LootChanceMatrix.</returns>
        LootChanceMatrix GetMatrix(string key)
        {
            // I reroute this call to an array contained in this script.
            for (int i = 0; i < bossfallLootTables.Length; i++)
            {
                // I reroute this call to an array contained in this script.
                if (bossfallLootTables[i].key == key)

                    // I reroute this call to an array contained in this script.
                    return bossfallLootTables[i];
            }

            // I reroute this call to an array contained in this script.
            return bossfallLootTables[0];
        }

        /// <summary>
        /// Vanilla's method from LootTables, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="matrix">Loot chance matrix.</param>
        /// <param name="playerEntity">Player entity.</param>
        /// <param name="enemyLevelModifier">If spawning loot for an enemy, enemy level * 50. Otherwise, 0.</param>
        /// <returns>DaggerfallUnityItem array.</returns>
        DaggerfallUnityItem[] GenerateRandomLoot(LootChanceMatrix matrix, PlayerEntity playerEntity, int enemyLevelModifier = 0)
        {
            float chance;
            List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

            // I added "UnityEngine." to the below line.
            UnityEngine.Random.InitState(items.GetHashCode());

            // DELETE WHEN IMPLEMENTED
            // Make gold generated scale w/enemy level rather than player's, don't just use enemyLevelModifier tho as then gold
            // would be way too ez to get... think of another scaling system

            int playerMod;

            // I added this declaration.
            int roll = Dice100.Roll();

            // I added this condition tree. It randomly determines how much gold will be generated and ignores player level.
            if (roll > 80)
            {
                if (roll > 85)
                {
                    if (roll > 88)
                    {
                        if (roll > 91)
                        {
                            if (roll > 94)
                            {
                                if (roll > 96)
                                {
                                    if (roll > 97)
                                    {
                                        if (roll > 98)
                                        {
                                            if (roll > 99)
                                            {
                                                playerMod = 20;
                                            }
                                            else
                                            {
                                                playerMod = UnityEngine.Random.Range(16, 19 + 1);
                                            }
                                        }
                                        else
                                        {
                                            playerMod = UnityEngine.Random.Range(13, 15 + 1);
                                        }
                                    }
                                    else
                                    {
                                        playerMod = UnityEngine.Random.Range(10, 12 + 1);
                                    }
                                }
                                else
                                {
                                    playerMod = UnityEngine.Random.Range(6, 9 + 1);
                                }
                            }
                            else
                            {
                                playerMod = 5;
                            }
                        }
                        else
                        {
                            playerMod = 4;
                        }
                    }
                    else
                    {
                        playerMod = 3;
                    }
                }
                else
                {
                    playerMod = 2;
                }
            }
            else
            {
                playerMod = 1;
            }

            int goldCount = UnityEngine.Random.Range(matrix.MinGold, matrix.MaxGold + 1) * playerMod;

            if (goldCount > 0)
            {
                items.Add(ItemBuilder.CreateGoldPieces(goldCount));
            }

            chance = matrix.WP;
            while (Dice100.SuccessRoll((int)chance))
            {
                // I replaced playerEntity.Level with enemyLevelModifier and rerouted the method call.
                items.Add(CreateRandomWeapon(enemyLevelModifier));

                chance *= 0.5f;
            }

            chance = matrix.AM;
            while (Dice100.SuccessRoll((int)chance))
            {
                // I replaced playerEntity.Level with enemyLevelModifier and rerouted the method call.
                items.Add(CreateRandomArmor(enemyLevelModifier, playerEntity.Gender, playerEntity.Race));

                chance *= 0.5f;
            }

            // I replaced playerEntity.Level with playerMod in the ingredient generation lines below. This causes
            // ingredients to be generated in much lower quantities than vanilla, this generation does not scale with
            // player level. If player gets lucky, numerous ingredients will be generated. I reroute all ingredient
            // generation method calls to the RandomIngredient method contained in this script.
            RandomIngredient(matrix.C1 * playerMod, ItemGroups.CreatureIngredients1, items);
            RandomIngredient(matrix.C2 * playerMod, ItemGroups.CreatureIngredients2, items);
            RandomIngredient(matrix.C3, ItemGroups.CreatureIngredients3, items);
            RandomIngredient(matrix.P1 * playerMod, ItemGroups.PlantIngredients1, items);
            RandomIngredient(matrix.P2 * playerMod, ItemGroups.PlantIngredients2, items);
            RandomIngredient(matrix.M1, ItemGroups.MiscellaneousIngredients1, items);
            RandomIngredient(matrix.M2, ItemGroups.MiscellaneousIngredients2, items);

            chance = matrix.MI;
            while (Dice100.SuccessRoll((int)chance))
            {
                // I replaced playerEntity.Level with enemyLevelModifier and rerouted the method call.
                items.Add(CreateRandomMagicItem(enemyLevelModifier, playerEntity.Gender, playerEntity.Race));

                chance *= 0.5f;
            }

            chance = matrix.CL;
            while (Dice100.SuccessRoll((int)chance))
            {
                items.Add(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
                chance *= 0.5f;
            }

            chance = matrix.BK;
            while (Dice100.SuccessRoll((int)chance))
            {
                items.Add(ItemBuilder.CreateRandomBook());
                chance *= 0.5f;
            }

            chance = matrix.RL;
            while (Dice100.SuccessRoll((int)chance))
            {
                // I rerouted the method call to a method in this script.
                items.Add(CreateRandomReligiousItem());

                chance *= 0.5f;
            }

            return items.ToArray();
        }

        /// <summary>
        /// Vanilla's method from LootTables, changed to be an instance method.
        /// </summary>
        /// <param name="chance">Generation chance.</param>
        /// <param name="ingredientGroup">Which ingredient group to generate items from.</param>
        /// <param name="targetItems">Item list to add generated ingredient to.</param>
        void RandomIngredient(float chance, ItemGroups ingredientGroup, List<DaggerfallUnityItem> targetItems)
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                targetItems.Add(ItemBuilder.CreateRandomIngredient(ingredientGroup));
                chance *= 0.5f;
            }
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <returns>A new religious item.</returns>
        DaggerfallUnityItem CreateRandomReligiousItem()
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.ReligiousItems);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.ReligiousItems, groupIndex);

            // If specific Holy items are created, hand off to the new functions I added to
            // put custom enchantments on the item.
            if (newItem.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_water))
                return CreateHolyWater();
            else if (newItem.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_dagger))
                return CreateHolyDagger();
            else if (newItem.IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_tome))
                return CreateHolyTome();

            return newItem;
        }

        /// <summary>
        /// This new function creates Holy Water that casts Holy Word on use.
        /// </summary>
        /// <returns>Enchanted Holy Water.</returns>
        public DaggerfallUnityItem CreateHolyWater()
        {
            DaggerfallUnityItem holyWater = new DaggerfallUnityItem(ItemGroups.ReligiousItems, 4);
            holyWater.legacyMagic = new DaggerfallEnchantment[] { new DaggerfallEnchantment() { type = EnchantmentTypes.CastWhenUsed, param = 58 } };
            holyWater.IdentifyItem();

            return holyWater;
        }

        /// <summary>
        /// This new function creates a Holy Dagger that casts Holy Word on use.
        /// </summary>
        /// <returns>Enchanted Holy Dagger.</returns>
        public DaggerfallUnityItem CreateHolyDagger()
        {
            DaggerfallUnityItem holyDagger = new DaggerfallUnityItem(ItemGroups.ReligiousItems, 11);
            holyDagger.legacyMagic = new DaggerfallEnchantment[] { new DaggerfallEnchantment() { type = EnchantmentTypes.CastWhenUsed, param = 58 } };
            holyDagger.IdentifyItem();

            return holyDagger;
        }

        /// <summary>
        /// This new function creates a Holy Tome that casts Banish Daedra on use.
        /// </summary>
        /// <returns>Enchanted Holy Tome.</returns>
        public DaggerfallUnityItem CreateHolyTome()
        {
            DaggerfallUnityItem holyTome = new DaggerfallUnityItem(ItemGroups.ReligiousItems, 12);
            holyTome.legacyMagic = new DaggerfallEnchantment[] { new DaggerfallEnchantment() { type = EnchantmentTypes.CastWhenUsed, param = 57 } };
            holyTome.IdentifyItem();

            return holyTome;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <returns>A randomly filled soul trap.</returns>
        public DaggerfallUnityItem CreateRandomlyFilledSoulTrap()
        {
            MobileTypes soul = MobileTypes.None;
            while (soul == MobileTypes.None)
            {
                MobileTypes randomSoul = (MobileTypes)UnityEngine.Random.Range((int)MobileTypes.Rat, (int)MobileTypes.Lamia + 1);
                if (randomSoul == MobileTypes.Horse_Invalid ||
                    randomSoul == MobileTypes.Dragonling)
                    continue;
                else
                    soul = randomSoul;
            }

            // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
            DaggerfallUnityItem newItem = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);

            newItem.TrappedSoulType = soul;
            MobileEnemy mobileEnemy = GameObjectHelper.EnemyDict[(int)soul];

            // Soul Gems are now worth enemy SoulPts and nothing more.
            newItem.value = mobileEnemy.SoulPts;

            return newItem;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="weapon">Which weapon to generate.</param>
        /// <param name="material">What material the weapon should have.</param>
        /// <returns>A new weapon.</returns>
        public DaggerfallUnityItem CreateWeapon(Weapons weapon, WeaponMaterialTypes material)
        {
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.Weapons, (int)weapon);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Weapons, groupIndex);

            if (weapon == Weapons.Arrow)
            {   
                // I changed the maximum value from 20 to 30. Arrows now spawned with this function
                // generate in stacks of up to 30.
                newItem.stackCount = UnityEngine.Random.Range(1, 30 + 1);

                newItem.currentCondition = 0;
            }
            else
            {
                // I reroute the method call to a method contained in this script.
                ApplyWeaponMaterial(newItem, material);
            }
            return newItem;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="enemyLevelModifier">If spawning loot for an enemy, enemy level * 50. Otherwise, 0.</param>
        /// <returns>A random weapon.</returns>
        public DaggerfallUnityItem CreateRandomWeapon(int enemyLevelModifier)
        {
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            Array enumArray = itemHelper.GetEnumArray(ItemGroups.Weapons);
            int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(ItemGroups.Weapons);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length + customItemTemplates.Length);
            DaggerfallUnityItem newItem;

            if (groupIndex < enumArray.Length)
                newItem = new DaggerfallUnityItem(ItemGroups.Weapons, groupIndex);
            else
                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                newItem = ItemBuilder.CreateItem(ItemGroups.Weapons, customItemTemplates[groupIndex - enumArray.Length]);

            // I pass on enemyLevelModifier so loot scales with enemy level rather than player's.
            WeaponMaterialTypes material = FormulaHelper.RandomMaterial(enemyLevelModifier);

            // I reroute the method call to a method contained in this script.
            ApplyWeaponMaterial(newItem, material);

            if (groupIndex == 18)
            {
                // I changed the maximum value from 20 to 30. Now arrows generated in shops and on
                // enemies spawn in stacks of up to 30.
                newItem.stackCount = UnityEngine.Random.Range(1, 30 + 1);

                newItem.currentCondition = 0;
                newItem.nativeMaterialValue = 0;
            }

            return newItem;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="weapon">The weapon to apply material to.</param>
        /// <param name="material">The material to apply to the weapon.</param>
        public void ApplyWeaponMaterial(DaggerfallUnityItem weapon, WeaponMaterialTypes material)
        {
            weapon.nativeMaterialValue = (int)material;

            // I reroute the method call to a method contained in this script.
            weapon = SetItemPropertiesByMaterial(weapon, material);

            weapon.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetWeaponDyeColor(material);

            if (GameManager.Instance.PlayerEntity.Gender == Genders.Female)
                weapon.PlayerTextureArchive -= 1;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="gender">Gender armor is created for.</param>
        /// <param name="race">Race armor is created for.</param>
        /// <param name="armor">Type of armor item to create.</param>
        /// <param name="material">Material of armor.</param>
        /// <param name="variant">Visual variant of armor. If -1, a random variant is chosen.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public DaggerfallUnityItem CreateArmor(Genders gender, Races race, Armor armor, ArmorMaterialTypes material, int variant = -1)
        {
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.Armor, (int)armor);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Armor, groupIndex);

            // No point running item template checks if item isn't a shield.
            if (newItem.IsShield)
            {
                // Replaces vanilla shields with my custom shield items so I can vary armor values with shield material.
                if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Buckler))
                {
                    newItem = new Buckler();
                }
                else if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Round_Shield))
                {
                    newItem = new RoundShield();
                }
                else if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Kite_Shield))
                {
                    newItem = new KiteShield();
                }
                else if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Tower_Shield))
                {
                    newItem = new TowerShield();
                }
            }

            // I reroute the method call to a method contained in this script.
            ApplyArmorSettings(newItem, gender, race, material, variant);

            return newItem;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="enemyLevelModifier">If spawning loot for an enemy, enemy level * 50. Otherwise, 0.</param>
        /// <param name="gender">Gender armor is created for.</param>
        /// <param name="race">Race armor is created for.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public DaggerfallUnityItem CreateRandomArmor(int enemyLevelModifier, Genders gender, Races race)
        {
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            Array enumArray = itemHelper.GetEnumArray(ItemGroups.Armor);
            int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(ItemGroups.Armor);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length + customItemTemplates.Length);
            DaggerfallUnityItem newItem;

            if (groupIndex < enumArray.Length)
                newItem = new DaggerfallUnityItem(ItemGroups.Armor, groupIndex);
            else
                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                newItem = ItemBuilder.CreateItem(ItemGroups.Armor, customItemTemplates[groupIndex - enumArray.Length]);

            // No point running item template checks if item isn't a shield.
            if (newItem.IsShield)
            {
                // Replaces vanilla shields with my custom shield items so I can vary armor values with shield material.
                if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Buckler))
                {
                    newItem = new Buckler();
                }
                else if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Round_Shield))
                {
                    newItem = new RoundShield();
                }
                else if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Kite_Shield))
                {
                    newItem = new KiteShield();
                }
                else if (newItem.IsOfTemplate(ItemGroups.Armor, (int)Armor.Tower_Shield))
                {
                    newItem = new TowerShield();
                }
            }

            // I pass on enemyLevelModifier so loot scales with enemy level rather than player's. I also reroute the method
            // call to a method in this script.
            ApplyArmorSettings(newItem, gender, race, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

            return newItem;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="armor">The armor to apply settings to.</param>
        /// <param name="gender">Player's gender.</param>
        /// <param name="race">Player's race.</param>
        /// <param name="material">Armor material type of the armor.</param>
        /// <param name="variant">If 0 or more, use that specific variant. If less than 0, randomly assign a variant.</param>
        public void ApplyArmorSettings(DaggerfallUnityItem armor, Genders gender, Races race, ArmorMaterialTypes material, int variant = 0)
        {
            if (gender == Genders.Female)

                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                armor.PlayerTextureArchive = ItemBuilder.firstFemaleArchive;
            else
                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                armor.PlayerTextureArchive = ItemBuilder.firstMaleArchive;

            // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
            ItemBuilder.SetRace(armor, race);

            // I reroute this method call to a method contained in this script.
            ApplyArmorMaterial(armor, material);

            if (variant >= 0)

                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                ItemBuilder.SetVariant(armor, variant);
            else
                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                ItemBuilder.RandomizeArmorVariant(armor);
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="armor">The armor item to apply material to.</param>
        /// <param name="material">What material to apply to the armor.</param>
        void ApplyArmorMaterial(DaggerfallUnityItem armor, ArmorMaterialTypes material)
        {
            armor.nativeMaterialValue = (int)material;

            if (armor.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
            {
                armor.weightInKg /= 2;
            }
            else if (armor.nativeMaterialValue == (int)ArmorMaterialTypes.Chain)
            {
                armor.value *= 2;
            }
            else if (armor.nativeMaterialValue >= (int)ArmorMaterialTypes.Iron)
            {
                int plateMaterial = armor.nativeMaterialValue - 0x0200;

                // I reroute this method call to a method contained in this script.
                armor = SetItemPropertiesByMaterial(armor, (WeaponMaterialTypes)plateMaterial);
            }

            armor.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetArmorDyeColor(material);
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="enemyLevelModifier">If spawning loot for an enemy, enemy level * 50. Otherwise, 0.</param>
        /// <param name="gender">Player's gender.</param>
        /// <param name="race">Player's race.</param>
        /// <returns>An enchanted item.</returns>
        public DaggerfallUnityItem CreateRandomMagicItem(int enemyLevelModifier, Genders gender, Races race)
        {
            // I pass on enemyLevelModifier so loot scales with enemy level rather than player's. I also reroute the method
            // call to a method in this script. I send the "chooseAtRandom" call to a field in this script, which I copied
            // from vanilla's ItemBuilder script.
            return CreateRegularMagicItem(chooseAtRandom, enemyLevelModifier, gender, race);
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="chosenItem">An integer index of the item to create, or -1 for a random one.</param>
        /// <param name="enemyLevelModifier">If spawning loot for an enemy, enemy level * 50. Otherwise, 0.</param>
        /// <param name="gender">The gender to create an item for.</param>
        /// <param name="race">The race to create an item for.</param>
        /// <returns>DaggerfallUnityItem</returns>
        /// <exception cref="Exception">When a base item cannot be created.</exception>
        DaggerfallUnityItem CreateRegularMagicItem(int chosenItem, int enemyLevelModifier, Genders gender, Races race)
        {
            byte[] itemGroups0 = { 2, 3, 6, 10, 12, 14, 25 };
            byte[] itemGroups1 = { 2, 3, 6, 12, 25 };

            DaggerfallUnityItem newItem = null;

            // I reroute the call to a custom array of regular magic item templates.
            MagicItemTemplate[] magicItems = BossfallMagicItemTemplates.Instance.CustomMagicItemTemplates;

            // I replaced "regularMagicItems" with "magicItems" in the line below.
            if (chosenItem > magicItems.Length)
                throw new Exception(string.Format("Magic item subclass {0} does not exist", chosenItem));

            // I send the "chooseAtRandom" call to a field in this script, which I copied from vanilla's ItemBuilder script.
            if (chosenItem == chooseAtRandom)
            {
                // I replaced "regularMagicItems" with "magicItems" in the line below.
                chosenItem = UnityEngine.Random.Range(0, magicItems.Length);
            }
            // I replaced "regularMagicItems" with "magicItems" in the line below.
            MagicItemTemplate magicItem = magicItems[chosenItem];

            ItemGroups group = 0;
            if (magicItem.group == 0)
                group = (ItemGroups)itemGroups0[UnityEngine.Random.Range(0, 7)];
            else if (magicItem.group == 1)
                group = (ItemGroups)itemGroups1[UnityEngine.Random.Range(0, 5)];
            else if (magicItem.group == 2)
                group = ItemGroups.Weapons;

            if (group == ItemGroups.Weapons)
            {
                // I pass on enemyLevelModifier so loot scales with enemy level rather than player's. I also reroute the method
                // call to a method in this script.
                newItem = CreateRandomWeapon(enemyLevelModifier);

                while (newItem.GroupIndex == 18)

                    // I pass on enemyLevelModifier so loot scales with enemy level rather than player's. I also reroute the method
                    // call to a method in this script.
                    newItem = CreateRandomWeapon(enemyLevelModifier);
            }
            else if (group == ItemGroups.Armor)

                // I pass on enemyLevelModifier so loot scales with enemy level rather than player's. I also reroute the method
                // call to a method in this script.
                newItem = CreateRandomArmor(enemyLevelModifier, gender, race);

            else if (group == ItemGroups.MensClothing || group == ItemGroups.WomensClothing)

                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                newItem = ItemBuilder.CreateRandomClothing(gender, race);

            else if (group == ItemGroups.ReligiousItems)
            {
                // I reroute the method call to a method contained in this script.
                newItem = CreateRandomReligiousItem();

                // No Holy Water/Daggers/Tomes as regular enchanted items - I want these Holy items to have custom
                // enchantments I add elsewhere in this script. I call a method contained in this script.
                while (newItem.GroupIndex == 4 || newItem.GroupIndex == 11 || newItem.GroupIndex == 12)
                    newItem = CreateRandomReligiousItem();
            }
            else if (group == ItemGroups.Gems)

                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                newItem = ItemBuilder.CreateRandomGem();
            else
                // I added "ItemBuilder." to send it to vanilla's ItemBuilder method.
                newItem = ItemBuilder.CreateRandomJewellery();

            if (newItem == null)
                throw new Exception("CreateRegularMagicItem() failed to create an item.");

            newItem.shortName = magicItem.name;

            newItem.legacyMagic = new DaggerfallEnchantment[magicItem.enchantments.Length];
            for (int i = 0; i < magicItem.enchantments.Length; ++i)
                newItem.legacyMagic[i] = magicItem.enchantments[i];

            newItem.maxCondition = magicItem.uses;
            newItem.currentCondition = magicItem.uses;

            int value = 0;
            for (int i = 0; i < magicItem.enchantments.Length; ++i)
            {
                if (magicItem.enchantments[i].type != EnchantmentTypes.None
                    && magicItem.enchantments[i].type < EnchantmentTypes.ItemDeteriorates)
                {
                    switch (magicItem.enchantments[i].type)
                    {
                        case EnchantmentTypes.CastWhenUsed:
                        case EnchantmentTypes.CastWhenHeld:
                        case EnchantmentTypes.CastWhenStrikes:

                            // I removed "Formulas." from the line below.
                            value += FormulaHelper.GetSpellEnchantPtCost(magicItem.enchantments[i].param);
                            break;
                        case EnchantmentTypes.RepairsObjects:
                        case EnchantmentTypes.AbsorbsSpells:
                        case EnchantmentTypes.EnhancesSkill:
                        case EnchantmentTypes.FeatherWeight:
                        case EnchantmentTypes.StrengthensArmor:

                            // I reroute the array call to an array contained in this script.
                            value += enchantmentPointCostsForNonParamTypes[(int)magicItem.enchantments[i].type];
                            break;
                        case EnchantmentTypes.SoulBound:
                            MobileEnemy mobileEnemy = GameObjectHelper.EnemyDict[magicItem.enchantments[i].param];
                            value += mobileEnemy.SoulPts;
                            break;
                        default:

                            // I reroute the array call to an array contained in this script.
                            value += enchantmentPtsForItemPowerArrays[(int)magicItem.enchantments[i].type][magicItem.enchantments[i].param];
                            break;
                    }
                }
            }

            newItem.value = value;

            return newItem;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="item">Item to have its properties modified.</param>
        /// <param name="material">Material to use to apply properties.</param>
        /// <returns>DaggerfallUnityItem</returns>
        DaggerfallUnityItem SetItemPropertiesByMaterial(DaggerfallUnityItem item, WeaponMaterialTypes material)
        {
            // I reroute the array call to an array contained in this script.
            item.value *= 3 * valueMultipliersByMaterial[(int)material];

            // I reroute the method call to a method contained in this script.
            item.weightInKg = CalculateWeightForMaterial(item, material);

            // I removed "/ 4" from the end of the line below, and called my new bossfallConditionMultipliersByMaterial array
            // rather than vanilla ItemBuilder's conditionMultipliersByMaterial array.
            item.maxCondition = item.maxCondition * bossfallConditionMultipliersByMaterial[(int)material];

            item.currentCondition = item.maxCondition;

            return item;
        }

        /// <summary>
        /// Vanilla's method from ItemBuilder, changed to be an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="item">The item to calculate weight of.</param>
        /// <param name="material">The material of the item.</param>
        /// <returns>Item weight, rounded to the nearest quarter kg.</returns>
        float CalculateWeightForMaterial(DaggerfallUnityItem item, WeaponMaterialTypes material)
        {
            int quarterKgs = (int)(item.weightInKg * 4);

            // I reroute the array call to an array contained in this script.
            float matQuarterKgs = (float)(quarterKgs * weightMultipliersByMaterial[(int)material]) / 4;

            return Mathf.Round(matQuarterKgs) / 4;
        }

        // DELETE AFTER IMPLEMENTATION
        // Make custom classes start w/Steel Longsword rather than Iron

        // DELETE AFTER IMPLEMENTATION
        // Make AssignEnemyStartingEquipment check for existence of modded wpns/armor, if present give 'em chance to have either
        // a vanilla weapon or armor piece or a modded weapon or armor piece (only give 'em one or the other, not both). Use notes
        // for guidance

        /// <summary>
        /// Vanilla's method from ItemHelper. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="player">The player entity.</param>
        /// <param name="enemyEntity">The enemy who is having their equipment assigned.</param>
        /// <param name="variant">The item variant.</param>
        public void AssignEnemyStartingEquipment(PlayerEntity player, EnemyEntity enemyEntity, int variant)
        {
            // I replaced itemLevel with enemyLevelModifier, which is enemy level * 50. This causes loot to scale with
            // enemy level rather than the player's.
            int enemyLevelModifier = enemyEntity.Level * 50;

            Genders playerGender = player.Gender;
            Races race = player.Race;
            int chance = 0;

            // I commented out the two lines below. Since Guards are ridiculously powerful in Bossfall, I want them to
            // be rewarding if the player manages to best them.

            // if (enemyEntity.EntityType == EntityTypes.EnemyClass && enemyEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
            //    itemLevel = 1;

            if (variant == 0)
            {
                int item = UnityEngine.Random.Range((int)Weapons.Broadsword, (int)(Weapons.Longsword) + 1);

                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on to
                // FormulaHelper.RandomMaterial method rather than the player's level.
                DaggerfallUnityItem weapon = CreateWeapon((Weapons)item, FormulaHelper.RandomMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(weapon, true, false);
                enemyEntity.Items.AddItem(weapon);

                // I didn't like how often enemies were generated with full suits of armor, so I reduced item generation chances.
                chance = 30;

                item = UnityEngine.Random.Range((int)Armor.Buckler, (int)(Armor.Round_Shield) + 1);
                if (Dice100.SuccessRoll(chance))
                {
                    // I removed "Items." from the third parameter of the CreateArmor method call. It now reads "Armor"
                    // instead of "Items.Armor". I also reroute the method call to a method contained in this script and
                    // pass enemyLevelModifier on to FormulaHelper.RandomArmorMaterial method rather than the player's level.
                    DaggerfallUnityItem armor = CreateArmor(playerGender, race, (Armor)item, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

                    enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                    enemyEntity.Items.AddItem(armor);
                }
                else if (Dice100.SuccessRoll(chance))
                {
                    item = UnityEngine.Random.Range((int)Weapons.Dagger, (int)(Weapons.Shortsword) + 1);

                    // I reroute the method call to a method contained in this script and pass enemyLevelModifier on to
                    // FormulaHelper.RandomMaterial method rather than the player's level.
                    weapon = CreateWeapon((Weapons)item, FormulaHelper.RandomMaterial(enemyLevelModifier));

                    enemyEntity.ItemEquipTable.EquipItem(weapon, true, false);
                    enemyEntity.Items.AddItem(weapon);
                }
            }
            else
            {
                int item = UnityEngine.Random.Range((int)Weapons.Claymore, (int)(Weapons.Battle_Axe) + 1);

                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on to
                // FormulaHelper.RandomMaterial method rather than the player's level.
                DaggerfallUnityItem weapon = CreateWeapon((Weapons)item, FormulaHelper.RandomMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(weapon, true, false);
                enemyEntity.Items.AddItem(weapon);

                if (variant == 1)

                    // I didn't like how often enemies were generated with full suits of armor, so I reduced item generation chances.
                    chance = 45;

                else if (variant == 2)

                    // I didn't like how often enemies were generated with full suits of armor, so I reduced item generation chances.
                    chance = 60;
            }
            
            if (Dice100.SuccessRoll(chance))
            {
                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on
                // to FormulaHelper.RandomArmorMaterial method rather than the player's level.
                DaggerfallUnityItem armor = CreateArmor(playerGender, race, Armor.Helm, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            
            if (Dice100.SuccessRoll(chance))
            {
                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on
                // to FormulaHelper.RandomArmorMaterial method rather than the player's level.
                DaggerfallUnityItem armor = CreateArmor(playerGender, race, Armor.Right_Pauldron, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            
            if (Dice100.SuccessRoll(chance))
            {
                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on
                // to FormulaHelper.RandomArmorMaterial method rather than the player's level.
                DaggerfallUnityItem armor = CreateArmor(playerGender, race, Armor.Left_Pauldron, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            
            if (Dice100.SuccessRoll(chance))
            {
                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on
                // to FormulaHelper.RandomArmorMaterial method rather than the player's level.
                DaggerfallUnityItem armor = CreateArmor(playerGender, race, Armor.Cuirass, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            
            if (Dice100.SuccessRoll(chance))
            {
                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on
                // to FormulaHelper.RandomArmorMaterial method rather than the player's level.
                DaggerfallUnityItem armor = CreateArmor(playerGender, race, Armor.Greaves, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            
            if (Dice100.SuccessRoll(chance))
            {
                // I reroute the method call to a method contained in this script and pass enemyLevelModifier on
                // to FormulaHelper.RandomArmorMaterial method rather than the player's level.
                DaggerfallUnityItem armor = CreateArmor(playerGender, race, Armor.Boots, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));

                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            // I added this if section. Any enemy that uses equipment now has a chance to drop gauntlets.
            if (Dice100.SuccessRoll(chance))
            {
                DaggerfallUnityItem armor = CreateArmor(playerGender, race, Armor.Gauntlets, FormulaHelper.RandomArmorMaterial(enemyLevelModifier));
                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }

            if (player.Level > 1)
            {
                DaggerfallUnityItem weapon = enemyEntity.ItemEquipTable.GetItem(EquipSlots.RightHand);

                // I removed Centaurs and Orcs from this poison weapon section. They don't use this item generation method.
                if (weapon != null && (enemyEntity.EntityType == EntityTypes.EnemyClass
                    || enemyEntity.MobileEnemy.ID == (int)MobileTypes.OrcSergeant))
                {
                    int chanceToPoison = 5;
                    if (enemyEntity.MobileEnemy.ID == (int)MobileTypes.Assassin)

                        // I increased chanceToPoison to 100. Assassins will always have a poisoned weapon if player is
                        // above level 1.
                        chanceToPoison = 100;

                    if (Dice100.SuccessRoll(chanceToPoison))
                    {
                        // I changed "135" to "139". Drugs can now be weapon poisons. Adds more variety. I also removed
                        // "Items." from the line below. It now reads "Poisons" instead of "Items.Poisons".
                        weapon.poisonType = (Poisons)UnityEngine.Random.Range(128, 139 + 1);
                    }
                }
            }
        }

        #endregion
    }
}
