// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/BossfallDFMod
// Original Author: Osorkon
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using Bossfall.Formulas;

namespace Bossfall
{
    public class Bossfall : MonoBehaviour
    {
        #region Fields

        static Mod mod;

        // Mod settings, set at mod start-up. 
        public static int PowerfulEnemiesAre { get; private set; }
        public static int EnemyMoveSpeed { get; private set; }
        public static int SkillAdvancementDifficulty { get; private set; }
        public static bool BossProximityWarning { get; private set; }
        public static bool DisplayEnemyLevel { get; private set; }
        public static bool AlternateLootPiles { get; private set; }

        #endregion

        #region Mod Init

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
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
        }

        void Awake()
        {
            var settings = mod.GetSettings();

            // Gets mod settings and sets appropriate properties.
            PowerfulEnemiesAre = settings.GetValue<int>("Difficulty", "PowerfulEnemiesAre");
            EnemyMoveSpeed = settings.GetValue<int>("Difficulty", "EnemyMoveSpeed");
            SkillAdvancementDifficulty = settings.GetValue<int>("Difficulty", "SkillAdvancementDifficulty");
            BossProximityWarning = settings.GetValue<bool>("Difficulty", "BossProximityWarning");
            DisplayEnemyLevel = settings.GetValue<bool>("Miscellaneous", "DisplayEnemyLevel");
            AlternateLootPiles = settings.GetValue<bool>("Miscellaneous", "AlternateLootPiles");

            mod.IsReady = true;
        }

        #endregion

        #region Unity

        void Start()
        {

        }

        void Update()
        {

        }

        #endregion
    }
}
