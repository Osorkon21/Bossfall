// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall
// Original Author: Osorkon
// Contributors:    
// 
// Notes: 
//

using BossfallMod.Events;
using BossfallMod.Formulas;
using BossfallMod.Items;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace BossfallMod
{
    /// <summary>
    /// Acts as Bossfall's command center.
    /// </summary>
    public class Bossfall : MonoBehaviour
    {
        #region Fields

        // A mod (Bossfall in this case).
        static Mod mod;

        #endregion

        #region Properties

        // Bossfall properties, set by mod settings.
        public static int PowerfulEnemiesAre { get; private set; }
        public static int EnemyMoveSpeed { get; private set; }
        public static int SkillAdvancementDifficulty { get; private set; }
        public static bool BossProximityWarning { get; private set; }
        public static bool DisplayEnemyLevel { get; private set; }
        public static bool AlternateLootPiles { get; private set; }

        #endregion

        #region Mod Init

        // This is the magic method to invoke to start up a mod. No idea what is going on behind the scenes.
        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            // This initializes the mod and adds the Bossfall component to the new mod GameObject.
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<Bossfall>();

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
            // DELETE WHEN IMPLEMENTED - Add ItemGroups.Armor to static Dictionary storeBuysItemType in DaggerfallTradeWindow
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
            SaveLoadManager.OnStartLoad += BossfallEventHandlers.BossfallOnStartLoad;
            SaveLoadManager.OnLoad += BossfallEventHandlers.BossfallOnLoad;
        }

        #endregion
    }
}
