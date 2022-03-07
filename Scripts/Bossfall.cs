// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall
// Original Author: Osorkon
// Contributors:    
// 
// Notes: 
//

using BossfallMod.EnemyAI;
using BossfallMod.Events;
using BossfallMod.Formulas;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using FullSerializer;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BossfallMod
{
    /// <summary>
    /// Acts as Bossfall's command center.
    /// </summary>
    public class Bossfall : MonoBehaviour, IHasModSaveData
    {
        #region Fields

        // A mod (Bossfall in this case).
        static Mod mod;

        #endregion

        #region Properties

        /// <summary>
        /// Required interface implementation to save Bossfall data.
        public Type SaveDataType
        {
            get {
                // DWI

                // Remove below Debug.Log message once testing complete

                Debug.Log("SaveDataType 'get' accessor called.");
                return typeof(BossfallSaveData_v1); }
        }

        // Bossfall settings, set during mod startup.
        public static int PowerfulEnemiesAre { get; private set; }
        public static int EnemyMoveSpeed { get; private set; }
        public static int SkillAdvancementDifficulty { get; private set; }
        public static bool BossProximityWarning { get; private set; }
        public static bool DisplayEnemyLevel { get; private set; }
        public static bool AlternateLootPiles { get; private set; }

        #endregion

        #region Mod Init

        // This registers Bossfall to vanilla's ModManager.
        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            // This initializes the mod, attaches a Bossfall component to a new GameObject, and points SaveDataInterface calls
            // to the current instance of Bossfall.
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<Bossfall>();
            mod.SaveDataInterface = go.GetComponent<Bossfall>();

            // This gives enemies Bossfall stats.
            int index = 0;
            while (index < BossfallOverrides.bossfallEnemyStats.Length)
            {
                EnemyBasics.Enemies[index] = BossfallOverrides.bossfallEnemyStats[index];
                index++;
            }

            // This replaces vanilla FormulaHelper methods with Bossfall methods.
            BossfallOverrides.RegisterOverrides(mod);

            // This disables parts of EnemyMotor, EnemySenses, and EnemyAttack. It also enables Bossfall AI.
            GameManager.Instance.DisableAI = true;
        }

        void Awake()
        {
            // Gets mod settings.
            var settings = mod.GetSettings();

            // Sets properties based on setting values.
            PowerfulEnemiesAre = settings.GetValue<int>("Difficulty", "PowerfulEnemiesAre");
            EnemyMoveSpeed = settings.GetValue<int>("Difficulty", "EnemyMoveSpeed");
            SkillAdvancementDifficulty = settings.GetValue<int>("Difficulty", "SkillAdvancementDifficulty");
            BossProximityWarning = settings.GetValue<bool>("Difficulty", "BossProximityWarning");
            DisplayEnemyLevel = settings.GetValue<bool>("Miscellaneous", "DisplayEnemyLevel");
            AlternateLootPiles = settings.GetValue<bool>("Miscellaneous", "AlternateLootPiles");

            // I assume this tells DFU Bossfall has loaded everything it needs to.
            mod.IsReady = true;
        }

        #endregion

        #region Unity

        void Start()
        {
            // DELETE WHEN IMPLEMENTED
            
            // Add ItemGroups.Armor to static Dictionary storeBuysItemType in DaggerfallTradeWindow
            // Erase old List entry first, should end up looking like:
            // { DFLocation.BuildingTypes.GeneralStore, new List<ItemGroups>()
            // { ItemGroups.Armor, ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Transportation, ItemGroups.Jewellery, ItemGroups.Weapons, ItemGroups.UselessItems2 } }
            // w/Reflection - this will make General Stores buy armor, but don't make 'em sell Armor. Follow notes for guidance

            // This expands the loot pile sprite list if the Alternate Loot Piles setting is on.
            if (AlternateLootPiles)
            {
                DaggerfallLootDataTables.randomTreasureIconIndices = BossfallOverrides.alternateRandomTreasureIconIndices;
            }

            // Bossfall subscribes to many vanilla events.
            PlayerEnterExit.OnRespawnerComplete += BossfallEventHandlers.BossfallOnRespawnerComplete;
            PlayerEnterExit.OnTransitionDungeonInterior += BossfallEventHandlers.BossfallOnTransitionDungeonInterior;
            EnemyEntity.OnLootSpawned += BossfallEventHandlers.BossfallOnEnemyLootSpawned;
            PlayerActivate.OnLootSpawned += BossfallEventHandlers.BossfallOnContainerLootSpawned;
            LootTables.OnLootSpawned += BossfallEventHandlers.BossfallOnTabledLootSpawned;
            SaveLoadManager.OnLoad += BossfallEventHandlers.BossfallOnLoad;
            DaggerfallUI.Instance.UserInterfaceManager.OnWindowChange += BossfallEventHandlers.BossfallOnWindowChange;
        }

        #endregion

        #region IHasModSaveData

        /// <summary>
        /// This method is called only if a Bossfall save file can't be found in vanilla's save folder.
        /// </summary>
        /// <returns>A new instance of BossfallSaveData_v1.</returns>
        public object NewSaveData()
        {
            // DWI

            // Remove Debug.Log lines when testing complete

            Debug.Log("NewSaveData method reached.");

            BossfallSaveData_v1 bossfallSaveData = new BossfallSaveData_v1();
            return bossfallSaveData;
        }

        /// <summary>
        /// Finds and builds Bossfall save data.
        /// </summary>
        /// <returns>BossfallSaveData_v1.</returns>
        public object GetSaveData()
        {
            // Create instance for save data.
            BossfallSaveData_v1 bossfallSaveData = new BossfallSaveData_v1();

            // Fresh dictionary instance for enemy data.
            bossfallSaveData.bossfallEnemySaveData = new Dictionary<ulong, BossfallEnemySaveData>();

            // I invoke the private SerializableEnemies "get" property accessor in SerializableStateManager using Reflection.
            // It's easier than calling GetObjectsOfType<DaggerfallEntityBehaviour> and then searching the permanent scene
            // cache for enemies. I can't call GetEnemyData in SerializableStateManager as that method doesn't return enemy
            // levels or DaggerfallEntityBehaviour I can then search for enemy levels, so it wouldn't help me.
            SerializableStateManager serializableStateManager = SaveLoadManager.StateManager;
            Type type = serializableStateManager.GetType();
            PropertyInfo property = type.GetProperty("SerializableEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo[] accessors = property.GetAccessors(true);
            Dictionary<ulong, ISerializableGameObject> enemyDictionary =
                accessors[0].Invoke(serializableStateManager, null) as Dictionary<ulong, ISerializableGameObject>;

            // There may be no enemies to save, so I check before iterating.
            if (enemyDictionary != null)
            {
                // Processes each entry in enemyDictionary and adds entries to bossfallEnemySaveData.
                foreach (var kvp in enemyDictionary)
                {
                    // Variables used in this foreach.
                    ulong key;
                    SerializableEnemy serializableEnemy;
                    DaggerfallEntityBehaviour entityBehaviour;
                    EnemyEntity entity;
                    BossfallEnemySenses enemySenses;
                    BossfallEnemySaveData enemyData = new BossfallEnemySaveData();

                    // I don't know if the Value will ever be null. This check may be unnecessary.
                    if (kvp.Value == null)
                    {
                        // DWI

                        // Delete Debug.Log in below line once testing complete

                        Debug.Log("Value in entry was null. Skipping entry.");
                        continue;
                    }

                    // I don't know if the Key will ever be 0. This check may be unnecessary.
                    if (kvp.Key == 0)
                    {
                        // DWI

                        // Delete Debug.Log in below line once testing complete

                        Debug.Log("Key in entry was 0. Skipping entry.");
                        continue;
                    }

                    serializableEnemy = kvp.Value as SerializableEnemy;

                    // I don't know if this will ever be null. I check anyway.
                    if (serializableEnemy == null)
                    {
                        // DWI

                        // Delete Debug.Log in below line once testing complete

                        Debug.Log("SerializableEnemy not present in Value. Skipping entry.");
                        continue;
                    }

                    // I don't know if this will ever be null. I check anyway.
                    if (serializableEnemy.GetComponent<DaggerfallEntityBehaviour>() == null)
                    {
                        // DWI

                        // Delete Debug.Log in below line once testing complete

                        Debug.Log("DaggerfallEntityBehaviour component not found. Skipping entry.");
                        continue;
                    }

                    // Gives me access to data I want to save.
                    entityBehaviour = serializableEnemy.GetComponent<DaggerfallEntityBehaviour>();
                    entity = entityBehaviour.Entity as EnemyEntity;
                    enemySenses = serializableEnemy.GetComponent<BossfallEnemySenses>();

                    // This is the data I want to save. Keys are unique enemy Load IDs.
                    key = kvp.Key;
                    enemyData.enemyLevel = entity.Level;
                    enemyData.canDetectInvisible = enemySenses.CanDetectInvisible;

                    // Adds collected data as a new dictionary entry.
                    bossfallSaveData.bossfallEnemySaveData.Add(key, enemyData);
                }
            }

            // Return assembled collection of Bossfall save data. Will be empty if no enemy data was found.
            return bossfallSaveData;
        }

        /// <summary>
        /// Restores saved Bossfall data to enemies.
        /// </summary>
        /// <param name="saveData">BossfallSaveData_v1.</param>
        public void RestoreSaveData(object saveData)
        {
            // DWI

            // Remove below Debug.Log lines after testing complete

            Debug.Log("Bossfall.RestoreSaveData method reached.");

            // Check if incoming object is Bossfall save data.
            BossfallSaveData_v1 bossfallSaveData = saveData as BossfallSaveData_v1;

            // I don't think this will ever be null, but I check anyway.
            if (bossfallSaveData == null)
            {
                // DWI

                // Remove below Debug.Log lines after testing complete

                Debug.Log("Bossfall.RestoreSaveData received an object that wasn't BossfallSaveData_v1 or it was null. Clearing EntityDictionary and returning.");

                // This dictionary was populated with references to all instantiated enemies earlier in the load process,
                // and I clear it to avoid potential duplicate entry errors.
                BossfallEventHandlers.EntityDictionary.Clear();
                return;
            }
            // If player has never used Bossfall before, this will always be null.
            else if (bossfallSaveData.bossfallEnemySaveData == null)
            {
                // DWI

                // Remove below Debug.Log lines after testing complete

                Debug.Log("Bossfall.RestoreSaveData.bossfallEnemySaveData is null. Clearing EntityDictionary and returning.");

                // This dictionary was populated with references to all instantiated enemies earlier in the load process,
                // and I clear it to avoid potential duplicate entry errors.
                BossfallEventHandlers.EntityDictionary.Clear();
                return;
            }
            // If this is reached and is true, something has gone wrong. This dictionary should never be null.
            else if (BossfallEventHandlers.EntityDictionary == null)
            {
                // DWI

                // Remove below Debug.Log lines after testing complete
               
                Debug.Log("BossfallEventHandlers entityDictionary was somehow null. Reinstantiating it and returning from Bossfall.RestoreSaveData method.");
                BossfallEventHandlers.EntityDictionary = new Dictionary<ulong, DaggerfallEntityBehaviour>();
                return;
            }

            // Processes save data by matching it to a dictionary contained in another script.
            foreach (var kvp in bossfallSaveData.bossfallEnemySaveData)
            {
                // I use this variable to remove entries from the nested dictionary as I go.
                ulong keyToRemove = 0;

                // This dictionary was built earlier in the load process and contains references to every instantiated enemy.
                foreach (var kvp1 in BossfallEventHandlers.EntityDictionary)
                {
                    // If match not found, keep looking.
                    if (kvp.Key != kvp1.Key)
                        continue;

                    // DWI

                    // Remove Debug.Log lines below after testing complete

                    Debug.Log("Key match found. Running level and stats resets.");

                    // Save the Key so I can remove it later. This progressively reduces dictionary size and iteration time.
                    keyToRemove = kvp1.Key;

                    // These variables give me access to values I want to replace with Bossfall save data.
                    DaggerfallEntityBehaviour entityBehaviour = kvp1.Value;
                    EnemyEntity enemy = entityBehaviour.Entity as EnemyEntity;
                    BossfallEnemySenses enemySenses = entityBehaviour.GetComponent<BossfallEnemySenses>();

                    // Restores Bossfall save data to enemies.
                    enemy.Level = kvp.Value.enemyLevel;
                    enemySenses.CanDetectInvisible = kvp.Value.canDetectInvisible;

                    // Every class enemy needs their armor level restored.
                    if (enemy.EntityType == EntityTypes.EnemyClass)
                    {
                        // This for loop is from BossfallEventHandlers.
                        for (int i = 0; i < enemy.ArmorValues.Length; i++)
                        {
                            // Bossfall class enemy armor doesn't change based on enemy equipment. It only scales by enemy level.
                            enemy.ArmorValues[i] = (sbyte)(60 - (enemy.Level * 2));

                            // If their level is higher than 20, they're either an Assassin boss or a Guard, so I check.
                            if (enemy.Level > 20 && enemy.CareerIndex == (int)MobileTypes.Assassin - 128)
                            {
                                enemy.ArmorValues[i] = 0;
                            }
                        }
                    }

                    // I recalculate enemy skill levels using the standard Bossfall formula.
                    short skillsLevel = (short)((enemy.Level * 7) + 30);

                    // Standard Bossfall skillsLevel cap.
                    if (skillsLevel > 180)
                        skillsLevel = 180;

                    // I reset enemy skill levels here using standard Bossfall methods.
                    for (int i = 0; i <= DaggerfallSkills.Count; i++)
                        enemy.Skills.SetPermanentSkillValue(i, skillsLevel);

                    // DWI

                    // Remove Debug.Log lines below once testing complete

                    Debug.Log("Enemy level-dependent values restored.");
                }

                // If a match was found in the nested foreach, keyToRemove will not be 0.
                if (keyToRemove != 0)
                {
                    // DWI

                    // Remove Debug.Log lines below after testing complete

                    Debug.Log("Removing processed entry from BossfallEventHandlers.EntityDictionary.");

                    // Remove entry from dictionary after processing so I have progressively less to iterate through.
                    BossfallEventHandlers.EntityDictionary.Remove(keyToRemove);
                }
            }

            // Clear dictionary as final step. It likely doesn't contain anything as I removed entries while processing,
            // but it never hurts to be careful.
            BossfallEventHandlers.EntityDictionary.Clear();
        }

        #endregion
    }

    #region Save Data

    /// <summary>
    /// Data saved by Bossfall. Enemy load IDs are the keys.
    /// </summary>
    [fsObject("v1")]
    public class BossfallSaveData_v1
    {
        public Dictionary<ulong, BossfallEnemySaveData> bossfallEnemySaveData;
    }

    /// <summary>
    /// Saved in a Dictionary keyed to enemy UID.
    /// </summary>
    public struct BossfallEnemySaveData
    {
        public int enemyLevel;
        public bool canDetectInvisible;
    }

    #endregion
}
