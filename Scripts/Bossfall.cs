// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
// 
// Notes: This script uses code from several vanilla scripts. Comments indicate authorship, please verify
//        authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using BossfallMod.EnemyAI;
using BossfallMod.Formulas;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace BossfallMod
{
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
            // Bossfall subscribes to many vanilla events.
            SaveLoadManager.OnLoad += BossfallOnLoad;
        }

        void Update()
        {

        }

        #endregion

        #region Public Methods



        #endregion

        #region Events

        /// <summary>
        /// This method runs after every load and sets up enemies.
        /// </summary>
        /// <param name="saveData">Save data, unused in current method.</param>
        void BossfallOnLoad(SaveData_v1 saveData)
        {
            // This begins a section of code copied from vanilla's GameManager script, modified for Bossfall.
            DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            for (int i = 0; i < entityBehaviours.Length; i++)
            {
                DaggerfallEntityBehaviour entityBehaviour = entityBehaviours[i];
                if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                {
                    // Unity scripts that inherit from MonoBehaviour are executed in the order they are attached to a given
                    // game object. By destroying EnemyAttack and then re-attaching it BossfallEnemyAttack's Update will
                    // run before EnemyAttack's Update, which is essential to Bossfall functioning correctly.
                    Destroy(entityBehaviour.gameObject.GetComponent<EnemyAttack>());
                    entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();
                    entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
                    entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
                    entityBehaviour.gameObject.AddComponent<EnemyAttack>();
                }
            }
            // This ends the section of code from vanilla's GameManager script.
        }

        #endregion
    }
}
