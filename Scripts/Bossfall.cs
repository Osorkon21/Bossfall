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
using BossfallMod.Items;
using BossfallMod.Utility;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility;
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
    /// Bossfall's command center.
    /// </summary>
    public class Bossfall : MonoBehaviour, IHasModSaveData
    {
        #region Fields

        static Mod mod;

        MethodInfo serializableEnemiesGetMethod;
        Dictionary<DFLocation.BuildingTypes, List<ItemGroups>> shopsBuyTheseItemGroups;

        #endregion

        #region Properties

        public Type SaveDataType { get { return typeof(BossfallSaveData_v1); } }

        public int PowerfulEnemiesAre { get; private set; }
        public int EnemyMoveSpeed { get; private set; }
        public int SkillAdvancementDifficulty { get; private set; }
        public bool BossProximityWarning { get; private set; }
        public bool DisplayEnemyLevel { get; private set; }
        public bool AlternateLootPiles { get; private set; }

        public static Bossfall Instance { get; private set; }

        #endregion

        #region Mod Init

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<Bossfall>();

            Instance = go.GetComponent<Bossfall>();

            mod.SaveDataInterface = Instance;

            var settings = mod.GetSettings();
            Instance.PowerfulEnemiesAre = settings.GetValue<int>("Difficulty", "PowerfulEnemiesAre");
            Instance.EnemyMoveSpeed = settings.GetValue<int>("Difficulty", "EnemyMoveSpeed");
            Instance.SkillAdvancementDifficulty = settings.GetValue<int>("Difficulty", "SkillAdvancementDifficulty");
            Instance.BossProximityWarning = settings.GetValue<bool>("Difficulty", "BossProximityWarning");
            Instance.DisplayEnemyLevel = settings.GetValue<bool>("Miscellaneous", "DisplayEnemyLevel");
            Instance.AlternateLootPiles = settings.GetValue<bool>("Miscellaneous", "AlternateLootPiles");

            go.AddComponent<BossfallEventHandlers>();
            go.AddComponent<BossfallItemBuilder>();
            go.AddComponent<BossfallOverrides>();
            go.AddComponent<BossfallEncounterTables>();
            go.AddComponent<BossfallMagicItemTemplates>();

            int index = 0;

            while (index < BossfallOverrides.Instance.BossfallEnemyStats.Length)
            {
                EnemyBasics.Enemies[index] = BossfallOverrides.Instance.BossfallEnemyStats[index];

                index++;
            }

            BossfallOverrides.Instance.RegisterOverrides(mod);

            // This enables Bossfall AI.
            GameManager.Instance.DisableAI = true;

            if (Instance.AlternateLootPiles)
                DaggerfallLootDataTables.randomTreasureIconIndices
                    = BossfallOverrides.Instance.BossfallAlternateRandomTreasureIconIndices;

            StartGameBehaviour.OnStartGame += BossfallEventHandlers.Instance.BossfallOnStartGame;
            PlayerEnterExit.OnRespawnerComplete += BossfallEventHandlers.Instance.BossfallOnRespawnerComplete;
            PlayerEnterExit.OnTransitionDungeonInterior += BossfallEventHandlers.Instance.BossfallOnTransitionDungeonInterior;
            EnemyEntity.OnLootSpawned += BossfallEventHandlers.Instance.BossfallOnEnemyLootSpawned;
            PlayerActivate.OnLootSpawned += BossfallEventHandlers.Instance.BossfallOnContainerLootSpawned;
            LootTables.OnLootSpawned += BossfallEventHandlers.Instance.BossfallOnTabledLootSpawned;
            SaveLoadManager.OnLoad += BossfallEventHandlers.Instance.BossfallOnLoad;
            DaggerfallUI.Instance.UserInterfaceManager.OnWindowChange += BossfallEventHandlers.Instance.BossfallOnWindowChange;
            EnemyDeath.OnEnemyDeath += BossfallEventHandlers.Instance.BossfallOnEnemyDeath;
            PopulationManager.OnMobileNPCEnable += BossfallEventHandlers.Instance.BossfallOnMobileNPCEnable;
            PopulationManager.OnMobileNPCDisable += BossfallEventHandlers.Instance.BossfallOnMobileNPCDisable;

            if (!QuestListsManager.RegisterQuestList("Bossfall"))
                throw new Exception("Bossfall QuestList could not be loaded.");
        }

        void Awake()
        {
            mod.IsReady = true;
        }

        #endregion

        #region Unity

        void Start()
        {
            Type type = SaveLoadManager.StateManager.GetType();
            Type type1 = typeof(SoulBound);
            Type type2 = typeof(DaggerfallTradeWindow);
            Type type3 = GameManager.Instance.WeaponManager.GetType();

            PropertyInfo property = type.GetProperty("SerializableEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo fieldInfo = type1.GetField("classicParamCosts", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo fieldInfo1 = type2.GetField("storeBuysItemType", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo fieldInfo2 = type3.GetField("swingWeaponFatigueLoss", BindingFlags.Instance | BindingFlags.NonPublic);

            serializableEnemiesGetMethod = property.GetGetMethod(true);
            fieldInfo.SetValue(null, BossfallOverrides.Instance.EnchantmentPointsByEnemyID);
            fieldInfo2.SetValue(GameManager.Instance.WeaponManager, 24);

            shopsBuyTheseItemGroups = (Dictionary<DFLocation.BuildingTypes, List<ItemGroups>>)fieldInfo1.GetValue(type2);
            shopsBuyTheseItemGroups.Remove(DFLocation.BuildingTypes.GeneralStore);
            shopsBuyTheseItemGroups.Add(DFLocation.BuildingTypes.GeneralStore, BossfallOverrides.Instance.BossfallGeneralStoreItemGroupsAccepted);
        }

        #endregion

        #region IHasModSaveData

        public object NewSaveData()
        {
            BossfallSaveData_v1 bossfallSaveData = new BossfallSaveData_v1();
            bossfallSaveData.bossfallEnemySaveData = new Dictionary<ulong, BossfallEnemySaveData>();

            return bossfallSaveData;
        }

        public object GetSaveData()
        {
            BossfallSaveData_v1 bossfallSaveData = new BossfallSaveData_v1();
            bossfallSaveData.bossfallEnemySaveData = new Dictionary<ulong, BossfallEnemySaveData>();

            Dictionary<ulong, ISerializableGameObject> enemyDictionary
                = serializableEnemiesGetMethod.Invoke(SaveLoadManager.StateManager, null) as Dictionary<ulong, ISerializableGameObject>;

            if (enemyDictionary.Count > 0)
            {
                foreach (var kvp in enemyDictionary)
                {
                    if (kvp.Value == null)
                        continue;

                    BossfallEnemySaveData enemyData = new BossfallEnemySaveData();

                    ulong key;
                    DaggerfallEntityBehaviour entityBehaviour;
                    EnemyEntity entity;
                    BossfallEnemySenses enemySenses;
                    SerializableEnemy serializableEnemy = kvp.Value as SerializableEnemy;

                    if (serializableEnemy == null)
                        continue;

                    entityBehaviour = serializableEnemy.GetComponent<DaggerfallEntityBehaviour>();

                    if (entityBehaviour == null)
                        continue;

                    entity = entityBehaviour.Entity as EnemyEntity;
                    enemySenses = serializableEnemy.GetComponent<BossfallEnemySenses>();

                    key = kvp.Key;
                    enemyData.enemyLevel = entity.Level;
                    enemyData.canDetectInvisible = enemySenses.CanDetectInvisible;

                    bossfallSaveData.bossfallEnemySaveData.Add(key, enemyData);
                }
            }

            return bossfallSaveData;
        }

        public void RestoreSaveData(object saveData)
        {
            BossfallSaveData_v1 bossfallSaveData = saveData as BossfallSaveData_v1;

            if (bossfallSaveData.bossfallEnemySaveData.Count > 0)
            {
                foreach (var kvp in bossfallSaveData.bossfallEnemySaveData)
                {
                    ulong keyToRemove = 0;

                    foreach (var kvp1 in BossfallEventHandlers.Instance.AllRestoredEnemies)
                    {
                        if (kvp.Key != kvp1.Key)
                            continue;

                        keyToRemove = kvp1.Key;

                        DaggerfallEntityBehaviour entityBehaviour = kvp1.Value;
                        EnemyEntity enemy = entityBehaviour.Entity as EnemyEntity;
                        BossfallEnemySenses enemySenses = entityBehaviour.GetComponent<BossfallEnemySenses>();

                        enemy.Level = kvp.Value.enemyLevel;
                        enemySenses.CanDetectInvisible = kvp.Value.canDetectInvisible;

                        // Every class enemy needs their armor level restored. This if is mostly from BossfallEventHandlers.
                        if (enemy.EntityType == EntityTypes.EnemyClass)
                        {
                            for (int i = 0; i < enemy.ArmorValues.Length; i++)
                            {
                                enemy.ArmorValues[i] = (sbyte)(60 - (enemy.Level * 2));

                                if (enemy.Level > 20 && enemy.CareerIndex == (int)MobileTypes.Assassin - 128)
                                {
                                    enemy.ArmorValues[i] = 0;
                                }
                            }
                        }

                        // Standard Bossfall skill level formula. From BossfallEventHandlers.
                        short skillsLevel = (short)((enemy.Level * 7) + 30);

                        // Standard Bossfall skillsLevel cap. From BossfallEventHandlers.
                        if (skillsLevel > 180)
                            skillsLevel = 180;

                        // Every enemy needs to have their skill levels restored. This for loop is from BossfallEventHandlers.
                        for (int i = 0; i <= DaggerfallSkills.Count; i++)
                            enemy.Skills.SetPermanentSkillValue(i, skillsLevel);
                    }

                    if (keyToRemove != 0)
                        BossfallEventHandlers.Instance.AllRestoredEnemies.Remove(keyToRemove);
                }
            }

            BossfallEventHandlers.Instance.AllRestoredEnemies.Clear();
        }

        #endregion
    }

    #region Save Data

    [fsObject("v1")]
    public class BossfallSaveData_v1
    {
        public Dictionary<ulong, BossfallEnemySaveData> bossfallEnemySaveData;
    }

    public struct BossfallEnemySaveData
    {
        public int enemyLevel;
        public bool canDetectInvisible;
    }

    #endregion
}
