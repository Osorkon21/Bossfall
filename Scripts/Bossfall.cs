// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Lypyl (lypyl@dfworkshop.net), Allofich, Numidium, TheLacus
// 
// Notes: This script uses code from several vanilla scripts. Comments indicate authorship, please verify
//        authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using BossfallMod.Formulas;
using BossfallMod.Utility;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using System;


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
            // Bossfall subscribes to many vanilla events.
            PlayerEnterExit.OnRespawnerComplete += BossfallOnRespawnerComplete;
            PlayerEnterExit.OnTransitionDungeonInterior += BossfallOnTransitionDungeonInterior;

            // DELETE WHEN IMPLEMENTED - u will end up using this event to write CanSeeInvisible value to restored enemies,
            // uncomment below line when u actually have a method that does this
            // SaveLoadManager.OnLoad += BossfallOnLoad;
        }

        void Update()
        {

        }

        #endregion

        #region Public Methods



        #endregion

        #region Events

        // DELETE WHEN IMPLEMENTED
        // U don't need to run anything in OnTransition or OnRespawner events if u don't need to respawn enemies, the enemies will be
        // processed from the EnemyEntity event while they're spawning and components will be added there. If u do need to respawn
        // enemies (i.e. transitioning/teleporting into a fresh dungeon, or if player is outside and OnEnemySpawn event fires)
        // then u do need to use the appropriate event handler. If player is outside and OnEnemySpawn event fires only respawn enemies
        // if enemies are not Quest foes. If they are, they'll already have been processed by EnemyEntity event & u don't need to
        // do anything. If player is outside & OnEnemySpawn event fires & foes are not Quest foes, then destroy enemies as they spawn
        // or destroy FoeSpawner (only if this always works before any foes are placed) and respawn enemies w/appropriate event handler.
        // You'll probably have to create another FoeSpawner with the same amount of enemies to place, but enemies will be different
        // as they'll use different encounter tables.

        // DELETE WHEN IMPLEMENTED
        // In ur EnemyEntity event handler make sure EnemyAttack is present b4 u try to remove it (test for NRE), if it's not present
        // at event firing time just add ur three Bossfall AI scripts & everything should work like it does now. Test this ofc

        // DELETE WHEN IMPLEMENTED
        // Use OnEnemySpawn event in GameManager to detect if enemies are Quest foes. Check DaggerfallEnemy.QuestSpawn = true

        // DELETE WHEN IMPLEMENTED
        // OnEnemySpawn event fires before OnEncounterEvent fires, not sure if this is important

        // DELETE WHEN IMPLEMENTED
        // For EnemyEntity reset event, there's a method in DaggerfallEntity called SpellbookCount, call that to get spellbook.Count,
        // then delete entire spellbook using for loop and DeleteSpell method in DaggerfallEntity, do this b4 adding new Bossfall spells

        /// <summary>
        /// This method adds necessary components to the PlayerObject. If successful doing so, it never runs again.
        /// </summary>
        void BossfallOnRespawnerComplete()
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
        void BossfallOnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
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
                                        // Spawns an underwater enemy using custom Bossfall encounter tables.
                                        GameObject[] waterEnemy = GameObjectHelper.CreateFoeGameObjects(
                                            adjustedPosition, BossfallEncounterTables.ChooseRandomEnemy(true));

                                        // Use already created "Random Enemies" node as parent transform.
                                        waterEnemy[0].transform.parent = randomEnemiesNode.transform;

                                        // Activate enemy.
                                        waterEnemy[0].SetActive(true);
                                    }
                                    else
                                    {
                                        // Spawns a non-water enemy using custom Bossfall encounter tables.
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
                // DELETE AFTER ADDING THIS CODE SECTION TO ENEMYENTITY EVENT HANDLER
                // Unity executes component scripts in the order they are attached to a given GameObject. By destroying
                // EnemyAttack and then re-adding it BossfallEnemyAttack's Update will always run before EnemyAttack's
                // Update, which is necessary for Bossfall to function correctly.
                // Destroy(entityBehaviour.gameObject.GetComponent<EnemyAttack>());
                // entityBehaviour.gameObject.AddComponent<BossfallEnemyAttack>();
                // entityBehaviour.gameObject.AddComponent<BossfallEnemyMotor>();
                // entityBehaviour.gameObject.AddComponent<BossfallEnemySenses>();
                // entityBehaviour.gameObject.AddComponent<EnemyAttack>();
            }

        /// <summary>
        /// This method is entirely vanilla's, pulled from PlayerActivate. It checks if struck object is an enemy.
        /// </summary>
        /// <param name="hitInfo">RaycastHit info.</param>
        /// <param name="mobileEnemy">The object being checked.</param>
        /// <returns>True if struck object is an enemy.</returns>
        bool MobileEnemyCheck(RaycastHit hitInfo, out DaggerfallEntityBehaviour mobileEnemy)
        {
            mobileEnemy = hitInfo.transform.GetComponent<DaggerfallEntityBehaviour>();

            return mobileEnemy != null;
        }

        #endregion
    }
}
