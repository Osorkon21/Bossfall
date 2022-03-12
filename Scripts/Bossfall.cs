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
    /// Bossfall's command center.
    /// </summary>
    public class Bossfall : MonoBehaviour, IHasModSaveData
    {
        #region Fields

        // A mod (Bossfall in this case).
        static Mod mod;

        // Used to get a private enemy list.
        MethodInfo getMethod;

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
        public int PowerfulEnemiesAre { get; private set; }
        public int EnemyMoveSpeed { get; private set; }
        public int SkillAdvancementDifficulty { get; private set; }
        public bool BossProximityWarning { get; private set; }
        public bool DisplayEnemyLevel { get; private set; }
        public bool AlternateLootPiles { get; private set; }

        /// <summary>
        /// Returns the only instance of Bossfall.
        /// </summary>
        public static Bossfall Instance { get; private set; }

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
            Instance = go.GetComponent<Bossfall>();
            mod.SaveDataInterface = Instance;

            // Gets mod settings.
            var settings = mod.GetSettings();

            // Sets properties based on setting values.
            Instance.PowerfulEnemiesAre = settings.GetValue<int>("Difficulty", "PowerfulEnemiesAre");
            Instance.EnemyMoveSpeed = settings.GetValue<int>("Difficulty", "EnemyMoveSpeed");
            Instance.SkillAdvancementDifficulty = settings.GetValue<int>("Difficulty", "SkillAdvancementDifficulty");
            Instance.BossProximityWarning = settings.GetValue<bool>("Difficulty", "BossProximityWarning");
            Instance.DisplayEnemyLevel = settings.GetValue<bool>("Miscellaneous", "DisplayEnemyLevel");
            Instance.AlternateLootPiles = settings.GetValue<bool>("Miscellaneous", "AlternateLootPiles");

            // Add remaining Bossfall components after Bossfall component setup completes.
            go.AddComponent<BossfallEventHandlers>();
            go.AddComponent<BossfallItemBuilder>();
            go.AddComponent<BossfallOverrides>();
            go.AddComponent<BossfallEncounterTables>();

            // This gives enemies Bossfall stats.
            int index = 0;
            while (index < BossfallOverrides.Instance.BossfallEnemyStats.Length)
            {
                EnemyBasics.Enemies[index] = BossfallOverrides.Instance.BossfallEnemyStats[index];
                index++;
            }

            // This replaces vanilla FormulaHelper methods with Bossfall methods.
            BossfallOverrides.Instance.RegisterOverrides(mod);

            // This disables parts of EnemyMotor, EnemySenses, and EnemyAttack. It also enables Bossfall AI.
            GameManager.Instance.DisableAI = true;

            // This expands the loot pile sprite list if the Alternate Loot Piles setting is on.
            if (Instance.AlternateLootPiles)
            {
                DaggerfallLootDataTables.randomTreasureIconIndices = BossfallOverrides.Instance.BossfallAlternateRandomTreasureIconIndices;
            }

            // Bossfall subscribes to many vanilla events.
            PlayerEnterExit.OnRespawnerComplete += BossfallEventHandlers.Instance.BossfallOnRespawnerComplete;
            PlayerEnterExit.OnTransitionDungeonInterior += BossfallEventHandlers.Instance.BossfallOnTransitionDungeonInterior;
            EnemyEntity.OnLootSpawned += BossfallEventHandlers.Instance.BossfallOnEnemyLootSpawned;
            PlayerActivate.OnLootSpawned += BossfallEventHandlers.Instance.BossfallOnContainerLootSpawned;
            LootTables.OnLootSpawned += BossfallEventHandlers.Instance.BossfallOnTabledLootSpawned;
            SaveLoadManager.OnLoad += BossfallEventHandlers.Instance.BossfallOnLoad;
            DaggerfallUI.Instance.UserInterfaceManager.OnWindowChange += BossfallEventHandlers.Instance.BossfallOnWindowChange;
        }

        void Awake()
        {
            // I assume this tells DFU Bossfall has loaded everything it needs to.
            mod.IsReady = true;
        }

        #endregion

        void Start()
        {
            // I access a SerializableStateManager property's private get accessor here. It will return a list of every
            // instantiated enemy. It's easier than calling GetObjectsOfType<DaggerfallEntityBehaviour> and then searching
            // the permanent scene cache for enemies. I can't call GetEnemyData in SerializableStateManager as that method
            // doesn't return enemy levels or DaggerfallEntityBehaviour I can then search for enemy levels, so it wouldn't help me.
            Type type = SaveLoadManager.StateManager.GetType();
            PropertyInfo property = type.GetProperty("SerializableEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
            getMethod = property.GetGetMethod(true);
        }

            // DELETE WHEN IMPLEMENTED

            // Add ItemGroups.Armor to static Dictionary storeBuysItemType in DaggerfallTradeWindow
            // Erase old List entry first, should end up looking like:
            // { DFLocation.BuildingTypes.GeneralStore, new List<ItemGroups>()
            // { ItemGroups.Armor, ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Transportation, ItemGroups.Jewellery, ItemGroups.Weapons, ItemGroups.UselessItems2 } }
            // w/Reflection - this will make General Stores buy armor, but don't make 'em sell Armor. Follow v1.4 to-do "shops"
            // section 4 guidance

            // DWI

            // Can u handle all this in BFOnWindowChange?
            // I think this would be a good place to start

            // U could call CreateCharCustomClass.AdvantageAdjust (that fires that scripts' UpdateDifficulty method) then
            // immediately after that access that script's difficultyPoints w/Reflection & change it to whatever u want, once u set
            // difficultyPoints then set dagger position 2 whatever it should be (use lines 463-477 for code for where to place
            // dagger dependent on difficultyPoints, remember to reset createdClass.AdvancementMultiplier if necessary every time
            // as I think you'll have to), you mite have to manually check so dagger position doesn't go past hard caps in vanilla
            // code, don't call AnimateDagger as I think that'll look silly

            // in CreateCharCustomClass method createdClass.HitPointsPerLevel is what u wanna change, vanilla inits to const int 8
            // in above script hpLabel contains actual text of HitPointsPerLevel, u will wanna change that too, see if difficulty
            // reads from that TextLabel or from createdClass.HitPointsPerLevel so u find out what u all need to set to 20

            // u prolly have to register 2 HitPointsUpButton and HitPointsDownButton events 2 override vanilla's UpdateDifficulty()
            // call from resetting ur custom difficultyPoints setting (u will have to run ur difficultyPoints resets every time
            // either button click event fires)

            // NOTE: There are two different instances of CreateCharSpecialAdvantageWindow created in CreateCharCustomClass -
            // one of createCharSpecialAdvantageWindow and one of createCharSpecialDisadvantageWindow - if u can read whenever an
            // instance of the type is created and always run ur SpecialAdvantage number resets, this is a moot point as ur resets
            // will catch both instances of the type

            // U can change player's DFCareer.HitPointsPerLevel on game start (for custom classes this is ur method of last resort),
            // this will not notify player of this change if player is creating custom class, see if there's "OnGameStart" event
            // u can register to to find out precise timing to do this, if u do this 4 custom classes notify player when u do somehow
            // U should only use this for canned class gen as player never sees their HP/Level in that case anyway
            // Remember to NOT change HitPointsPerLevel to 20 if player is creating canned Barbarian class, that class has 25 HP/lvl
            // Change to 20 HP/lvl as default for all other canned classes tho

            // these methods in CreateCharSpecialAdvantageWindow may be what u need to refresh HUD after u change initially numbers 
            // UpdateLabels();
            // UpdateDifficultyAdjustment();
            // or just call Draw() again 2 refresh? In same script
            // Do u even need to manually refresh? Update is called every frame anyway w/out ur input... test this ofc

            // DaggerfallPopupWindow.PreviousWindow may be handy? dunno

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

            // I invoke a SerializableStateManager private property's get accessor here. It returns every instantiated enemy.
            Dictionary<ulong, ISerializableGameObject> enemyDictionary =
                getMethod.Invoke(SaveLoadManager.StateManager, null) as Dictionary<ulong, ISerializableGameObject>;

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
                BossfallEventHandlers.Instance.EntityDictionary.Clear();
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
                BossfallEventHandlers.Instance.EntityDictionary.Clear();
                return;
            }
            // If this is reached and is true, something has gone wrong. This dictionary should never be null.
            else if (BossfallEventHandlers.Instance.EntityDictionary == null)
            {
                Debug.Log("BossfallEventHandlers.EntityDictionary was somehow null. Doing nothing and returning.");
                return;
            }

            // Processes save data by matching it to a dictionary contained in another script.
            foreach (var kvp in bossfallSaveData.bossfallEnemySaveData)
            {
                // I use this variable to remove entries from the nested dictionary as I go.
                ulong keyToRemove = 0;

                // This dictionary was built earlier in the load process and contains references to every instantiated enemy.
                foreach (var kvp1 in BossfallEventHandlers.Instance.EntityDictionary)
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
                    BossfallEventHandlers.Instance.EntityDictionary.Remove(keyToRemove);
                }
            }

            // Clear dictionary as final step. It likely doesn't contain anything as I removed entries while processing,
            // but it never hurts to be careful.
            BossfallEventHandlers.Instance.EntityDictionary.Clear();
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
