// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich, Numidium
// 
// Notes: This script uses code from several vanilla DFU scripts. Comments indicate authorship, please verify authorship
//        before crediting. When in doubt compare to vanilla DFU's source code.
//

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using System.Collections.Generic;
using UnityEngine;

namespace BossfallMod.EnemyAI
{
    /// <summary>
    /// Counterpart to vanilla's EnemyMotor. Used for Bossfall enemy AI.
    /// </summary>
    [RequireComponent(typeof(BossfallEnemySenses))]
    [RequireComponent(typeof(BossfallEnemyAttack))]
    [RequireComponent(typeof(EnemySenses))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(EnemyBlood))]
    [RequireComponent(typeof(EnemySounds))]
    [RequireComponent(typeof(CharacterController))]
    public class BossfallEnemyMotor : MonoBehaviour
    {
        // I changed this region name to Fields. Seemed more appropriate.
        #region Fields

        public float OpenDoorDistance = 2f;
        float stopDistance = 1.7f;
        const float doorCrouchingHeight = 1.65f;
        bool flies;
        bool swims;
        bool pausePursuit;
        float moveInForAttackTimer;
        bool moveInForAttack;
        float retreatDistanceMultiplier;
        float changeStateTimer;
        bool doStrafe;
        float strafeTimer;
        bool pursuing;
        bool retreating;
        bool backingUp;
        bool fallDetected;
        bool obstacleDetected;
        bool foundUpwardSlope;
        bool foundDoor;
        Vector3 lastPosition;
        Vector3 lastDirection;
        bool rotating;
        float avoidObstaclesTimer;
        bool checkingClockwise;
        float checkingClockwiseTimer;
        bool didClockwiseCheck;
        float lastTimeWasStuck;
        bool hasBowAttack;
        float centerChange;
        bool resetHeight;
        float heightChangeTimer;
        bool strafeLeft;
        float strafeAngle;
        int searchMult;
        int ignoreMaskForShooting;
        int ignoreMaskForObstacles;
        bool canAct;
        bool falls;
        bool flyerFalls;
        float lastGroundedY;
        float originalHeight;

        // These bools are used for Bossfall enemy AI and boss proximity warning HUD messages.
        bool prefersBow;
        bool alwaysCharges;
        bool isBoss;
        bool showBossWarning;
        bool showPowerfulBossWarning;
        bool runRangedSpellCorrection;

        /// <summary>
        /// This monstrosity represents enemy move speeds by enemy ID and covers IDs from 0-146. This array is used
        /// if the "Enemy Move Speed" setting is "Fast", and these speeds are Bossfall v1.3 enemy move speeds. I use this array to
        /// make the enemy movespeed selection process much faster than the giant if/else if tree I had in TakeAction in versions
        /// v1.2.1 and earlier. Most of this array is unused filler as enemies with IDs 43-127 don't exist, but it's more efficient
        /// to declare this whole thing and then search by ID without modification than it would be to declare a 62-element array
        /// (to match the 62 enemies in DFU) and then subtract 85 from every enemy ID above 127 to get the correct index number.
        /// </summary>
        public static readonly float[] fastMoveSpeeds = { 7f, 6f, 3f, 8.5f, 7.5f, 8f, 9f, 5f, 7.5f, 9f, 4.5f, 4.5f, 5.75f, 8f,
        7.5f, 6f, 7f, 3f, 3.5f, 3.5f, 7.5f, 5.75f, 4.5f, 4f, 6.5f, 5f, 7.5f, 6.5f, 10f, 7.5f, 12f, 7.5f, 4f, 4.5f, 8f, 7.5f, 3f, 3f,
        3f, 0, 10f, 3.5f, 4f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4f, 7.5f, 6f, 4f, 4f, 8f, 7f, 8f, 8f, 8.5f, 8f, 10f, 7.5f, 4.5f, 4.5f, 7f, 5.75f, 5f, 8.5f };

        /// <summary>
        /// This monstrosity represents enemy move speeds by enemy ID and covers IDs from 0-146. This array is used if the
        /// "Enemy Move Speed" setting is "Very Fast", and these speeds are Bossfall v1.2.1 enemy move speeds. I use this array to
        /// make the enemy movespeed selection process much faster than the giant if/else if tree I had in TakeAction in versions
        /// v1.2.1 and earlier. Most of this array is unused filler as enemies with IDs 43-127 don't exist, but it's more efficient
        /// to declare this whole thing and then search by ID without modification than it would be to declare a 62-element array
        /// (to match the 62 enemies in DFU) and then subtract 85 from every enemy ID above 127 to get the correct index number.
        /// </summary>
        public static readonly float[] veryFastMoveSpeeds = { 7f, 6f, 3f, 8.5f, 7.5f, 8f, 9f, 5f, 7.5f, 9f, 4.5f, 4.5f, 5.75f, 8f,
        7.5f, 6f, 7f, 3f, 3.5f, 3.5f, 7.5f, 5.75f, 4.5f, 4f, 6.5f, 5f, 7.5f, 6.5f, 10f, 7.5f, 12f, 7.5f, 4f, 4.5f, 8f, 7.5f, 3f, 3f,
        3f, 0, 10f, 3.5f, 4f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4f, 7.5f, 6f, 4f, 4f, 8f, 7f, 8f, 8f, 8.5f, 8f, 10f, 7.5f, 4.5f, 4.5f, 7f, 5.75f, 5f, 8.5f };

        // I added the next three lines.
        BossfallEnemySenses bossfallSenses;
        EnemyMotor motor;
        EntityEffectManager manager;

        EnemySenses senses;
        Vector3 destination;
        Vector3 detourDestination;
        CharacterController controller;
        MobileUnit mobile;
        Collider myCollider;
        DaggerfallEntityBehaviour entityBehaviour;
        EnemyBlood entityBlood;
        EntityEffectManager entityEffectManager;
        EntityEffectBundle selectedSpell;
        EnemyAttack attack;
        EnemyEntity entity;
        #endregion

        #region Auto Properties

        public bool Bashing { get; private set; }

        #endregion

        #region Unity Methods

        void Start()
        {
            // I added the next two lines.
            bossfallSenses = GetComponent<BossfallEnemySenses>();
            motor = GetComponent<EnemyMotor>();

            // Bossfall AI breaks EnhancedCombatAI aiming. This event fixes spell aiming.
            if (DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                manager = GetComponent<EntityEffectManager>();
                manager.OnCastReadySpell += BossfallOnCastReadySpell;
            }

            senses = GetComponent<EnemySenses>();
            controller = GetComponent<CharacterController>();
            mobile = GetComponentInChildren<MobileUnit>();
            myCollider = gameObject.GetComponent<Collider>();

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            motor.IsHostile = mobile.Enemy.Reactions == MobileReactions.Hostile;
            flies = CanFly();
            swims = mobile.Enemy.Behaviour == MobileBehaviour.Aquatic;
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBlood = GetComponent<EnemyBlood>();
            entityEffectManager = GetComponent<EntityEffectManager>();
            entity = entityBehaviour.Entity as EnemyEntity;
            attack = GetComponent<EnemyAttack>();
            hasBowAttack = mobile.Enemy.HasRangedAttack1 && mobile.Enemy.ID > 129 && mobile.Enemy.ID != 132;
            ignoreMaskForShooting = ~(1 << LayerMask.NameToLayer("SpellMissiles") | 1 << LayerMask.NameToLayer("Ignore Raycast"));
            ignoreMaskForObstacles = ~(1 << LayerMask.NameToLayer("SpellMissiles") | 1 << LayerMask.NameToLayer("Ignore Raycast"));
            lastGroundedY = transform.position.y;
            originalHeight = controller.height;

            // If enemy is an Archer or Ranger prefersBow is true. Only needs to be checked once.
            if (mobile.Enemy.ID == 141 || mobile.Enemy.ID == 142)
            {
                prefersBow = true;
            }

            // If enemy is non-sentient or very stupid alwaysCharges is true. Only needs to be checked once.
            if (mobile.Enemy.ID == 32 || mobile.Enemy.ID == 33)
            {
                alwaysCharges = false;
            }
            else if (mobile.Enemy.Affinity == MobileAffinity.Animal || mobile.Enemy.Affinity == MobileAffinity.Undead)
            {
                alwaysCharges = true;
            }
            else if (mobile.Enemy.ID == 11 || mobile.Enemy.ID == 16 || mobile.Enemy.ID == 22 || mobile.Enemy.ID == 143)
            {
                alwaysCharges = true;
            }

            // If enemy is a boss isBoss is true. If enemy is an OrcWarlord/Vampire/Lich/Dragonling_Alternate
            // showBossWarning is true. If enemy is a VampireAncient/DaedraLord/AncientLich showPowerfulBossWarning is
            // true. Only needs to be checked once.
            if (mobile.Enemy.ID == 24 || mobile.Enemy.ID == 28 || mobile.Enemy.ID == 32 || mobile.Enemy.ID == 40)
            {
                isBoss = true;
                showBossWarning = true;
            }
            else if (mobile.Enemy.ID == 30 || mobile.Enemy.ID == 31 || mobile.Enemy.ID == 33)
            {
                isBoss = true;
                showPowerfulBossWarning = true;
            }
        }

        void FixedUpdate()
        {
            // I reversed the DisableAI check so player can swap between Bossfall & vanilla AI with the console.
            if (!GameManager.Instance.DisableAI)
                return;

            flies = CanFly();
            canAct = true;
            flyerFalls = false;
            falls = false;

            HandleParalysis();
            KnockbackMovement();
            ApplyGravity();
            HandleNoAction();
            HandleBashing();
            UpdateTimers();
            if (canAct)
                TakeAction();
            ApplyFallDamage();
            UpdateToIdleOrMoveAnim();
            OpenDoors();
            HeightAdjust();

            // If the Boss Proximity Warning setting is ON, this checks for nearby bosses and creates warning
            // HUD messages. Detection radius is roughly half a dungeon block. Assassins won't trigger warning messages,
            // they're too stealthy to detect. The three toughest bosses (Daedra Lord, Vampire Ancient, Ancient Lich)
            // get a unique HUD message. The HUD message appears once per boss.
            if (Bossfall.BossProximityWarning && isBoss)
            {
                if (showBossWarning)
                {
                    if (bossfallSenses.DistanceToPlayer < 25.6f)
                    {
                        DaggerfallUI.AddHUDText("You sense a boss nearby.");
                        showBossWarning = false;
                    }
                }
                else if (showPowerfulBossWarning)
                {
                    if (bossfallSenses.DistanceToPlayer < 25.6f)
                    {
                        DaggerfallUI.AddHUDText("You sense a powerful boss nearby.");
                        showPowerfulBossWarning = false;
                    }
                }
            }

            // Bossfall AI breaks EnhancedCombat AI aiming. This fixes ranged spell aiming.
            if (runRangedSpellCorrection)
                RangedSpellCorrection();
        }

        #endregion

        #region Public Methods

        public Vector3 FindGroundPosition(float distance = 16)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, distance))
                return hit.point;

            return transform.position;
        }

        #endregion

        #region Private Methods

        void HandleParalysis()
        {
            if (entityBehaviour.Entity.IsParalyzed)
            {
                mobile.FreezeAnims = true;
                canAct = false;
                flyerFalls = true;
            }
            mobile.FreezeAnims = false;
        }

        void KnockbackMovement()
        {
            if (mobile.EnemyState == MobileStates.SeducerTransform1 || mobile.EnemyState == MobileStates.SeducerTransform2)
                return;

            // I redirect calls to EnemyMotor's KnockbackSpeed as that is what other (non-Bossfall) scripts are setting.
            if (motor.KnockbackSpeed > 0)
            {
                // I capped KnockbackSpeed at 20 rather than 40. This reduces time spent flying backwards,
                // so enemy can resume smacking away at the player. I redirect calls to EnemyMotor's KnockbackSpeed as
                // that is what other (non-Bossfall) scripts are setting.
                if (motor.KnockbackSpeed > (20 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    motor.KnockbackSpeed = (20 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (motor.KnockbackSpeed > (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                    mobile.EnemyState != MobileStates.PrimaryAttack)
                {
                    mobile.ChangeEnemyState(MobileStates.Hurt);
                }

                Vector3 motion;

                // I reduced maximum KnockbackSpeed from 25 to 20 so knockbacks don't last so long. I redirect
                // calls to EnemyMotor's KnockbackSpeed and KnockbackDirection as that is what other (non-Bossfall)
                // scripts are setting.
                if (motor.KnockbackSpeed <= (20 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    motion = motor.KnockbackDirection * motor.KnockbackSpeed;
                else
                    motion = motor.KnockbackDirection * (20 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (swims)
                    WaterMove(motion);

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                else if (flies || motor.IsLevitating)
                    controller.Move(motion * Time.deltaTime);
                else
                    controller.SimpleMove(motion);

                if (GameManager.ClassicUpdate)
                {
                    // I set KnockbackSpeed to decrement at 7 rather than 5, and I capped minimum KnockbackSpeed
                    // at 7 rather than 5. With changes to enemy weight I made in FormulaHelper, this greatly reduces
                    // knockback stunlocks with high damage/SPD attacks. I redirect calls to EnemyMotor's KnockbackSpeed as
                    // that is what other (non-Bossfall) scripts are setting.
                    motor.KnockbackSpeed -= (7 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                    if (motor.KnockbackSpeed <= (7 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10))
                        && mobile.EnemyState != MobileStates.PrimaryAttack)
                    {
                        mobile.ChangeEnemyState(MobileStates.Move);
                    }
                }

                // I redirect calls to EnemyMotor's KnockbackSpeed as that is what other (non-Bossfall) scripts
                // are setting.
                if (motor.KnockbackSpeed > (10 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                {
                    EvaluateMoveInForAttack();
                }

                canAct = false;
                flyerFalls = true;
            }
        }

        void ApplyGravity()
        {
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!flies && !swims && !motor.IsLevitating && !controller.isGrounded)
            {
                controller.SimpleMove(Vector3.zero);
                falls = true;

                if (lastPosition != transform.position)
                    canAct = false;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (flyerFalls && flies && !motor.IsLevitating)
            {
                controller.SimpleMove(Vector3.zero);
                falls = true;
            }
        }

        void HandleNoAction()
        {
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target == null || motor.GiveUpTimer <= 0 || senses.PredictedTargetPos == EnemySenses.ResetPlayerPos)
            {
                SetChangeStateTimer();
                searchMult = 0;

                canAct = false;
            }
        }

        void HandleBashing()
        {
            if (Bashing)
            {
                int speed = entity.Stats.LiveSpeed;
                if (GameManager.ClassicUpdate && DFRandom.rand() % speed >= (speed >> 3) + 6 && attack.MeleeTimer == 0)
                {
                    mobile.ChangeEnemyState(MobileStates.PrimaryAttack);
                    attack.ResetMeleeTimer();
                }

                canAct = false;
            }
        }

        void UpdateTimers()
        {
            if (moveInForAttackTimer > 0)
                moveInForAttackTimer -= Time.deltaTime;

            if (avoidObstaclesTimer > 0)
                avoidObstaclesTimer -= Time.deltaTime;

            if (avoidObstaclesTimer > 0 && canAct)
            {
                Vector3 detourDestination2D = detourDestination;
                detourDestination2D.y = transform.position.y;
                if ((detourDestination2D - transform.position).magnitude <= 0.3f)
                {
                    avoidObstaclesTimer = 0;
                }
            }

            if (checkingClockwiseTimer > 0)
                checkingClockwiseTimer -= Time.deltaTime;

            if (changeStateTimer > 0)
                changeStateTimer -= Time.deltaTime;

            if (strafeTimer > 0)
                strafeTimer -= Time.deltaTime;

            if (senses.DetectedTarget)

                // [If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                motor.GiveUpTimer = 200;

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (GameManager.ClassicUpdate && !senses.DetectedTarget && motor.GiveUpTimer > 0)
                motor.GiveUpTimer--;
        }

        void TakeAction()
        {
            // I changed the declaration of this variable to avoid unnecessary moveSpeed calculations.
            float moveSpeed;

            // This sets enemy movement speed based on which "Enemy Move Speed" setting is used. The first option
            // is the recommended "Fast" setting, which is rebalanced speeds for Bossfall v1.3. The second option is the
            // "Very Fast" setting - which is v1.2.1 speeds - and the last option is the "Vanilla" setting, which is vanilla
            // DFU speed set using the vanilla formula. Exact enemy moveSpeed numbers are in the declared arrays up top.
            if (Bossfall.EnemyMoveSpeed == 1)
            {
                moveSpeed = fastMoveSpeeds[mobile.Enemy.ID];
            }
            else if (Bossfall.EnemyMoveSpeed == 2)
            {
                moveSpeed = veryFastMoveSpeeds[mobile.Enemy.ID];
            }
            else
            {
                moveSpeed = (entity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) * MeshReader.GlobalScale;
            }

            bool isPlayingOneShot = mobile.IsPlayingOneShot();

            // I commented out the vanilla check below. Now Vampire Ancients can run down the player and
            // hit them instead of slowing down when attacking. To balance this out, I added Pacification, Dispel,
            // and Teleport magical items to give player a fighting chance.

            // if (isPlayingOneShot && DaggerfallUnity.Settings.EnhancedCombatAI)
            //    moveSpeed /= attackSpeedDivisor;

            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                if (senses.Target == GameManager.Instance.PlayerEntityBehaviour)
                    stopDistance = attack.MeleeDistance;
                else
                    stopDistance = attack.ClassicMeleeDistanceVsAI;
            }

            GetDestination();

            Vector3 direction = (destination - transform.position).normalized;

            float distance;

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (avoidObstaclesTimer <= 0 && bossfallSenses.TargetInSight)
                distance = bossfallSenses.DistanceToTarget;
            else
                distance = (destination - transform.position).magnitude;

            if (isPlayingOneShot && mobile.OneShotPauseActionsWhilePlaying())
                return;

            if (DoRangedAttack(direction, moveSpeed, distance, isPlayingOneShot))
                return;

            if (DoTouchSpell())
                return;

            if (moveInForAttackTimer <= 0 && avoidObstaclesTimer <= 0)
                EvaluateMoveInForAttack();

            // If enemy is an Archer or Ranger, I never want them to move in to attack. I put these bool
            // settings here, as that influences what decisions are made a few lines down.
            if (prefersBow)
            {
                retreating = true;
                pursuing = false;
            }

            if (avoidObstaclesTimer > 0)
            {
                AttemptMove(direction, moveSpeed);
            }
            else if ((!retreating && distance >= (stopDistance * 2.75)) || (distance > stopDistance && moveInForAttack))
            {
                if (changeStateTimer <= 0 || pursuing)
                    AttemptMove(direction, moveSpeed);
                else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
                    TurnToTarget(direction);
            }
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && strafeTimer <= 0)
            {
                StrafeDecision();
            }
            else if (doStrafe && strafeTimer > 0 && (distance >= stopDistance * .8f))
            {
                AttemptMove(direction, moveSpeed / 4, false, true, distance);
            }
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && bossfallSenses.TargetInSight && (distance < stopDistance * .8f ||
                !moveInForAttack && distance < stopDistance * retreatDistanceMultiplier && (changeStateTimer <= 0 || retreating)))
            {
                // Vanilla has moveSpeed / 2 here, I removed the 2. This makes enemies back up at full speed.
                // The player can run backwards at full speed, I wanted enemies to do so as well. It's only fair.
                if (changeStateTimer <= 0 || retreating)
                    AttemptMove(direction, moveSpeed, true);
            }
            else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
            {
                TurnToTarget(direction);
            }
            else
            {
                SetChangeStateTimer();
                pursuing = false;
                retreating = false;
            }
        }

        void GetDestination()
        {
            CharacterController targetController = senses.Target.GetComponent<CharacterController>();

            if (avoidObstaclesTimer > 0)
            {
                destination = detourDestination;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            else if (ClearPathToPosition(senses.PredictedTargetPos, (destination - transform.position).magnitude) || (bossfallSenses.TargetInSight && (hasBowAttack || entity.CurrentMagicka > 0)))
            {
                destination = senses.PredictedTargetPos;

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (flies || motor.IsLevitating || (swims && mobile.Enemy.ID == (int)MonsterCareers.Slaughterfish))
                    destination.y += targetController.height * 0.5f;

                searchMult = 0;
            }
            else
            {
                Vector3 searchPosition = senses.LastKnownTargetPos + (senses.LastPositionDiff.normalized * searchMult);
                if (searchMult <= 10 && (searchPosition - transform.position).magnitude <= stopDistance)
                    searchMult++;

                destination = searchPosition;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (avoidObstaclesTimer <= 0 && !flies && !motor.IsLevitating && !swims && senses.Target)
            {
                float deltaHeight = (targetController.height - originalHeight) / 2;
                destination.y -= deltaHeight;
            }
        }

        bool DoRangedAttack(Vector3 direction, float moveSpeed, float distance, bool isPlayingOneShot)
        {
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            bool inRange = bossfallSenses.DistanceToTarget > BossfallEnemyAttack.minRangedDistance && bossfallSenses.DistanceToTarget < BossfallEnemyAttack.maxRangedDistance;
            if (inRange && bossfallSenses.TargetInSight && senses.DetectedTarget && (CanShootBow() || CanCastRangedSpell()))
            {
                if (DaggerfallUnity.Settings.EnhancedCombatAI && senses.TargetIsWithinYawAngle(22.5f, destination) && strafeTimer <= 0)
                {
                    StrafeDecision();
                }

                if (doStrafe && strafeTimer > 0)
                {
                    // Vanilla has moveSpeed / 4 here, I changed that to 2 so enemies at range strafe faster.
                    // I got annoyed at how easy it was to hit enemies with arrows at long distances.
                    AttemptMove(direction, moveSpeed / 2, false, true, distance);
                }

                if (GameManager.ClassicUpdate && senses.TargetIsWithinYawAngle(22.5f, destination))
                {
                    if (!isPlayingOneShot)
                    {
                        if (hasBowAttack)
                        {
                            // Vanilla has 1/32f here. I thought bow attacks were too infrequent.
                            if (Random.value < 1 / 18f)
                            {
                                if (mobile.Enemy.HasRangedAttack1 && !mobile.Enemy.HasRangedAttack2)
                                    mobile.ChangeEnemyState(MobileStates.RangedAttack1);
                                else if (mobile.Enemy.HasRangedAttack2)
                                    mobile.ChangeEnemyState(MobileStates.RangedAttack2);
                            }
                        }

                        // Vanilla has 1/40f here. Bossfall enemies are magical machine guns.
                        else if (Random.value < 1 / 15f && entityEffectManager.SetReadySpell(selectedSpell))
                        {
                            mobile.ChangeEnemyState(MobileStates.Spell);
                        }
                    }
                }
                else
                    TurnToTarget(direction);

                return true;
            }

            return false;
        }

        bool DoTouchSpell()
        {
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (bossfallSenses.TargetInSight && senses.DetectedTarget && attack.MeleeTimer == 0
                && bossfallSenses.DistanceToTarget <= attack.MeleeDistance + senses.TargetRateOfApproach
                && CanCastTouchSpell() && entityEffectManager.SetReadySpell(selectedSpell))
            {
                if (mobile.EnemyState != MobileStates.Spell)
                    mobile.ChangeEnemyState(MobileStates.Spell);

                attack.ResetMeleeTimer();
                return true;
            }

            return false;
        }

        void StrafeDecision()
        {
            // I changed the maximum range from 4 to 5, reducing strafe frequency.
            doStrafe = Random.Range(0, 5) == 0;
            strafeTimer = Random.Range(1f, 2f);
            if (doStrafe)
            {
                if (Random.Range(0, 2) == 0)
                    strafeLeft = true;
                else
                    strafeLeft = false;

                Vector3 north = destination;
                north.z++;
                strafeAngle = Vector3.SignedAngle(destination - north, destination - transform.position, Vector3.up);
                if (strafeAngle < 0)
                    strafeAngle = 360 + strafeAngle;

                strafeAngle *= Mathf.PI / 180;
            }
        }

        bool ClearPathToPosition(Vector3 location, float dist = 30)
        {
            Vector3 sphereCastDir = (location - transform.position).normalized;
            Vector3 sphereCastDir2d = sphereCastDir;
            sphereCastDir2d.y = 0;
            ObstacleCheck(sphereCastDir2d);
            FallCheck(sphereCastDir2d);

            if (obstacleDetected || fallDetected)
                return false;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, controller.radius / 2, sphereCastDir, out hit, dist, ignoreMaskForShooting))
            {
                DaggerfallEntityBehaviour hitTarget = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (hitTarget == senses.Target)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        bool HasClearPathToShootProjectile(float speed, float radius)
        {
            // I call the BossfallEnemySenses method instead of vanilla's as Bossfall's will be more accurate.
            Vector3 sphereCastDir = bossfallSenses.PredictNextTargetPos(speed);
            if (sphereCastDir == EnemySenses.ResetPlayerPos)
                return false;

            bool myColliderWasEnabled = false;
            if (myCollider)
            {
                myColliderWasEnabled = myCollider.enabled;
                myCollider.enabled = false;
            }
            bool isSpaceInsufficient = Physics.CheckSphere(transform.position, radius, ignoreMaskForShooting);
            if (myCollider)
                myCollider.enabled = myColliderWasEnabled;

            if (isSpaceInsufficient)
                return false;

            float sphereCastDist = (sphereCastDir - transform.position).magnitude;
            sphereCastDir = (sphereCastDir - transform.position).normalized;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, radius, sphereCastDir, out hit, sphereCastDist, ignoreMaskForShooting))
            {
                DaggerfallEntityBehaviour hitTarget = hit.transform.GetComponent<DaggerfallEntityBehaviour>();

                if (hitTarget == senses.Target)
                    return true;

                return false;
            }

            return true;
        }

        bool CanShootBow()
        {
            if (!hasBowAttack)
                return false;

            return HasClearPathToShootProjectile(35f, 0.15f);
        }

        bool CanCastRangedSpell()
        {
            if (entity.CurrentMagicka <= 0)
                return false;

            EffectBundleSettings[] spells = entity.GetSpells();
            List<EffectBundleSettings> rangeSpells = new List<EffectBundleSettings>();
            int count = 0;
            foreach (EffectBundleSettings spell in spells)
            {
                // I added CasterOnly, enemies now cast self-protection/healing spells at range.
                // If they didn't, they would never cast Spell Resistance, Shalidor's Mirror, and Heal unless
                // player is within melee range, and by then it'd likely be too late.
                if (spell.TargetType == TargetTypes.SingleTargetAtRange
                    || spell.TargetType == TargetTypes.AreaAtRange
                    || spell.TargetType == TargetTypes.CasterOnly)
                {
                    rangeSpells.Add(spell);
                    count++;
                }
            }

            if (count == 0)
                return false;

            EffectBundleSettings selectedSpellSettings = rangeSpells[Random.Range(0, count)];
            selectedSpell = new EntityEffectBundle(selectedSpellSettings, entityBehaviour);

            if (EffectsAlreadyOnTarget(selectedSpell))
                return false;

            return HasClearPathToShootProjectile(25f, 0.45f);
        }

        bool CanCastTouchSpell()
        {
            if (entity.CurrentMagicka <= 0)
                return false;

            EffectBundleSettings[] spells = entity.GetSpells();
            List<EffectBundleSettings> rangeSpells = new List<EffectBundleSettings>();
            int count = 0;
            foreach (EffectBundleSettings spell in spells)
            {

                // I condensed this bool as I didn't want to differentiate between classic
                // and Enhanced Combat AI. Enemies cast any type of melee spell within melee range.
                if (spell.TargetType == TargetTypes.ByTouch
                    || spell.TargetType == TargetTypes.CasterOnly
                    || spell.TargetType == TargetTypes.AreaAroundCaster)
                {
                    rangeSpells.Add(spell);
                    count++;
                }

            }

            if (count == 0)
                return false;

            EffectBundleSettings selectedSpellSettings = rangeSpells[Random.Range(0, count)];
            selectedSpell = new EntityEffectBundle(selectedSpellSettings, entityBehaviour);

            if (EffectsAlreadyOnTarget(selectedSpell))
                return false;

            return true;
        }

        bool CanFly()
        {
            return mobile.Enemy.Behaviour == MobileBehaviour.Flying || mobile.Enemy.Behaviour == MobileBehaviour.Spectral;
        }

        bool EffectsAlreadyOnTarget(EntityEffectBundle spell)
        {
            if (senses.Target)
            {
                EntityEffectManager targetEffectManager = senses.Target.GetComponent<EntityEffectManager>();
                LiveEffectBundle[] bundles = targetEffectManager.EffectBundles;

                for (int i = 0; i < spell.Settings.Effects.Length; i++)
                {
                    bool foundEffect = false;
                    IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(spell.Settings.Effects[i].Key);
                    for (int j = 0; j < bundles.Length && !foundEffect; j++)
                    {
                        for (int k = 0; k < bundles[j].liveEffects.Count && !foundEffect; k++)
                        {
                            if (bundles[j].liveEffects[k].GetType() == effectTemplate.GetType())
                                foundEffect = true;
                        }
                    }

                    if (!foundEffect)
                        return false;
                }
            }

            return true;
        }

        void AttemptMove(Vector3 direction, float moveSpeed, bool backAway = false, bool strafe = false, float strafeDist = 0)
        {
            if (!backAway && !strafe)
            {
                pursuing = true;
                retreating = false;
            }
            else
            {
                retreating = true;
                pursuing = false;
            }

            if (!senses.TargetIsWithinYawAngle(5.625f, destination))
            {
                TurnToTarget(direction);

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (!DaggerfallUnity.Settings.EnhancedCombatAI || !bossfallSenses.TargetInSight)
                    return;
            }

            if (backAway)
                direction *= -1;

            if (strafe)
            {
                Vector3 strafeDest = new Vector3(destination.x + (Mathf.Sin(strafeAngle) * strafeDist), transform.position.y, destination.z + (Mathf.Cos(strafeAngle) * strafeDist));
                direction = (strafeDest - transform.position).normalized;

                if ((strafeDest - transform.position).magnitude <= 0.2f)
                {
                    if (strafeLeft)
                        strafeAngle++;
                    else
                        strafeAngle--;
                }
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!flies && !swims && !motor.IsLevitating && controller.isGrounded)
                direction.y = -2f;

            if (flies && avoidObstaclesTimer <= 0 && direction.y < 0 && FindGroundPosition((originalHeight / 2) + 1f) != transform.position)
                direction.y = 0.1f;

            Vector3 motion = direction * moveSpeed;

            if (!backAway && DaggerfallUnity.Settings.EnhancedCombatAI && avoidObstaclesTimer <= 0)
            {
                bool withinPitch = senses.TargetIsWithinPitchAngle(45.0f);
                if (!pausePursuit && !withinPitch)
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                    // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (flies || motor.IsLevitating || swims)
                    {
                        if (!senses.TargetIsAbove())
                            motion = -transform.up * moveSpeed / 2;
                        else
                            motion = transform.up * moveSpeed;
                    }
                    else if (senses.TargetIsAbove() && changeStateTimer <= 0)
                    {
                        SetChangeStateTimer();
                        pausePursuit = true;
                    }
                }
                else if (withinPitch)
                {
                    pausePursuit = false;
                    backingUp = false;
                }

                if (pausePursuit)
                {
                    if (senses.TargetIsAbove() && !senses.TargetIsWithinPitchAngle(55.0f) && (changeStateTimer <= 0 || backingUp))
                    {
                        // Vanilla has moveSpeed * 0.75 here. I want enemies to back up faster.
                        motion = -transform.forward * moveSpeed;
                        backingUp = true;
                    }
                    else
                    {
                        backingUp = false;
                        return;
                    }
                }
            }

            SetChangeStateTimer();

            Vector3 direction2d = direction;

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!flies && !swims && !motor.IsLevitating)
                direction2d.y = 0;
            ObstacleCheck(direction2d);
            FallCheck(direction2d);

            if (fallDetected || obstacleDetected)
            {
                if (!strafe && !backAway)
                    FindDetour(direction2d);
            }
            else
            {
                if (swims)
                    WaterMove(motion);
                else
                    controller.Move(motion * Time.deltaTime);
            }
        }

        void FindDetour(Vector3 direction2d)
        {
            float angle;
            Vector3 testMove = Vector3.zero;
            bool foundUpDown = false;

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (flies || swims || motor.IsLevitating)
            {
                float multiplier = 0.3f;
                if (Random.Range(0, 2) == 0)
                    multiplier = -0.3f;

                Vector3 upOrDown = new Vector3(0, 1, 0);
                upOrDown.y *= multiplier;

                testMove = (direction2d + upOrDown).normalized;

                ObstacleCheck(testMove);
                if (obstacleDetected)
                {
                    upOrDown.y *= -1;
                    testMove = (direction2d + upOrDown).normalized;
                    ObstacleCheck(testMove);
                }
                if (!obstacleDetected)
                    foundUpDown = true;
            }

            if (!foundUpDown && Time.time - lastTimeWasStuck > 2f)
            {
                checkingClockwiseTimer = 0;
                didClockwiseCheck = false;
            }

            if (!foundUpDown && checkingClockwiseTimer <= 0)
            {
                if (!didClockwiseCheck)
                {
                    if (Random.Range(0, 2) == 0)
                        angle = 45;
                    else
                        angle = -45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                    ObstacleCheck(testMove);
                    FallCheck(testMove);

                    if (!obstacleDetected && !fallDetected)
                    {
                        if (angle == 45)
                        {
                            checkingClockwise = true;
                        }
                        else
                            checkingClockwise = false;
                    }
                    else
                    {
                        angle *= -1;
                        testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                        ObstacleCheck(testMove);
                        FallCheck(testMove);

                        if (!obstacleDetected && !fallDetected)
                        {
                            if (angle == 45)
                            {
                                checkingClockwise = true;
                            }
                            else
                                checkingClockwise = false;
                        }
                        else
                        {
                            Vector3 toTarget = destination - transform.position;
                            Vector3 directionToTarget = toTarget.normalized;
                            angle = Vector3.SignedAngle(directionToTarget, direction2d, Vector3.up);

                            if (angle > 0)
                            {
                                checkingClockwise = true;
                            }
                            else
                                checkingClockwise = false;
                        }
                    }
                    checkingClockwiseTimer = 5;
                    didClockwiseCheck = true;
                }
                else
                {
                    didClockwiseCheck = false;
                    checkingClockwise = !checkingClockwise;
                    checkingClockwiseTimer = 5;
                }
            }

            angle = 0;
            int count = 0;

            if (!foundUpDown)
            {
                do
                {
                    if (checkingClockwise)
                        angle += 45;
                    else
                        angle -= 45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                    ObstacleCheck(testMove);
                    FallCheck(testMove);
                    count++;

                    if (count > 7)
                    {
                        break;
                    }
                }
                while (obstacleDetected || fallDetected);
            }

            detourDestination = transform.position + testMove * 2;

            if (avoidObstaclesTimer <= 0)
                avoidObstaclesTimer = 0.75f;
            lastTimeWasStuck = Time.time;
        }

        void ObstacleCheck(Vector3 direction)
        {
            obstacleDetected = false;
            float checkDistance = controller.radius / Mathf.Sqrt(2f);
            foundUpwardSlope = false;
            foundDoor = false;
            RaycastHit hit;
            Vector3 p1 = transform.position + (Vector3.up * -originalHeight * 0.1388F);
            Vector3 p2 = p1 + (Vector3.up * Mathf.Min(originalHeight, doorCrouchingHeight) / 2);

            if (Physics.CapsuleCast(p1, p2, controller.radius / 2, direction, out hit, checkDistance, ignoreMaskForObstacles))
            {
                obstacleDetected = true;
                DaggerfallEntityBehaviour entityBehaviour2 = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                DaggerfallActionDoor door = hit.transform.GetComponent<DaggerfallActionDoor>();
                DaggerfallLoot loot = hit.transform.GetComponent<DaggerfallLoot>();

                if (entityBehaviour2)
                {
                    if (entityBehaviour2 == senses.Target)
                        obstacleDetected = false;
                }
                else if (door)
                {
                    obstacleDetected = false;
                    foundDoor = true;
                    if (senses.TargetIsWithinYawAngle(22.5f, door.transform.position))
                    {
                        senses.LastKnownDoor = door;
                        senses.DistanceToDoor = Vector3.Distance(transform.position, door.transform.position);
                    }
                }
                else if (loot)
                {
                    obstacleDetected = false;
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                else if (!swims && !flies && !motor.IsLevitating)
                {
                    Vector3 checkUp = transform.position + direction;
                    checkUp.y++;
                    direction = (checkUp - transform.position).normalized;
                    p1 = transform.position + (Vector3.up * -originalHeight * 0.25f);
                    p2 = p1 + (Vector3.up * originalHeight * 0.75f);

                    if (!Physics.CapsuleCast(p1, p2, controller.radius / 2, direction, checkDistance))
                    {
                        obstacleDetected = false;
                        foundUpwardSlope = true;
                    }
                }
            }
            // There was an empty "else" statement here in EnemyMotor's method. I removed it.
        }

        void FallCheck(Vector3 direction)
        {
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (flies || motor.IsLevitating || swims || obstacleDetected || foundUpwardSlope || foundDoor)
            {
                fallDetected = false;
                return;
            }

            int checkDistance = 1;
            Vector3 rayOrigin = transform.position;

            direction *= checkDistance;
            Ray ray = new Ray(rayOrigin + direction, Vector3.down);
            RaycastHit hit;

            fallDetected = !Physics.Raycast(ray, out hit, (originalHeight * 0.5f) + 1.5f);
        }

        void EvaluateMoveInForAttack()
        {
            // If enemy is the type to always charge, they'll always move in to attack.
            if (!DaggerfallUnity.Settings.EnhancedCombatAI || alwaysCharges)
            {
                moveInForAttack = true;
                return;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!bossfallSenses.TargetInSight)
            {
                moveInForAttack = true;
                return;
            }

            if (senses.Target != null)
            {
                // The new !prefersBow check ensures Archers/Rangers never move in to melee range, even
                // if player is incapacitated or unaware.
                if (!prefersBow)
                {
                    EntityEffectManager targetEffectManager = senses.Target.GetComponent<EntityEffectManager>();

                    // I removed "MagicAndEffects.MagicEffects" from the line below.
                    if (targetEffectManager.FindIncumbentEffect<Paralyze>() != null)
                    {
                        moveInForAttack = true;
                        return;
                    }

                    if (senses.TargetHasBackTurned())
                    {
                        moveInForAttack = true;
                        return;
                    }

                    if (senses.Target == GameManager.Instance.PlayerEntityBehaviour
                        && GameManager.Instance.WeaponManager.ScreenWeapon
                        && (GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType == WeaponTypes.Bow
                        || !GameManager.Instance.WeaponManager.ScreenWeapon.ShowWeapon))
                    {
                        moveInForAttack = true;
                        return;
                    }
                }
            }
            else
            {
                return;
            }

            const float retreatDistanceBaseMult = 2.25f;
            moveInForAttackTimer = Random.Range(1, 3);
            int levelMod = (entity.Level - senses.Target.Entity.Level) / 2;

            if (levelMod > 4)
                levelMod = 4;

            // Vanilla has levelMod < -4 here. I find enemies constantly retreating really annoying,
            // so I greatly reduced the likelihood of that occurring.
            if (levelMod < 0)
                levelMod = 0;

            int roll = Random.Range(0 + levelMod, 10 + levelMod);

            // If enemy is not an Archer or Ranger, proceed as normal. If they are, moveInForAttack will always be false.
            if (!prefersBow)
            {
                moveInForAttack = roll > 4;
            }
            else
            {
                moveInForAttack = false;
            }

            if (!moveInForAttack)
            {
                retreatDistanceMultiplier = (float)(retreatDistanceBaseMult + (retreatDistanceBaseMult * (0.25 * (2 - roll))));

                // This check is necessary for TakeAction to do what I want. Without this custom multiplier, Archers
                // and Rangers usually won't begin retreating until player is right in front of them.
                if (prefersBow)
                    retreatDistanceMultiplier = 2.75f;

                if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                    return;

                if (Random.Range(0, 2) == 0)
                    strafeLeft = true;
                else
                    strafeLeft = false;

                Vector3 north = destination;
                north.z++;
                strafeAngle = Vector3.SignedAngle(destination - north, destination - transform.position, Vector3.up);

                if (strafeAngle < 0)
                    strafeAngle = 360 + strafeAngle;

                strafeAngle *= Mathf.PI / 180;
            }
        }

        void SetChangeStateTimer()
        {
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                return;

            if (changeStateTimer <= 0)
                changeStateTimer = Random.Range(0.2f, 0.8f);
        }

        void WaterMove(Vector3 motion)
        {
            if (GameManager.Instance.PlayerEnterExit.blockWaterLevel != 10000
                    && controller.transform.position.y
                    < GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
            {
                if (motion.y > 0 && controller.transform.position.y + (100 * MeshReader.GlobalScale)
                        >= GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
                {
                    motion.y = 0;
                }

                controller.Move(motion * Time.deltaTime);
            }
        }

        void TurnToTarget(Vector3 targetDirection)
        {
            // Vanilla has 20f here. I want enemies to turn faster.
            const float turnSpeed = 30f;

            if (GameManager.ClassicUpdate)
            {
                transform.forward = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Mathf.Deg2Rad, 0.0f);
            }
        }

        void UpdateToIdleOrMoveAnim()
        {
            if (!mobile.IsPlayingOneShot())
            {
                if (GameManager.ClassicUpdate)
                {
                    Vector3 currentDirection = transform.forward;
                    currentDirection.y = 0;
                    rotating = lastDirection != currentDirection;
                    lastDirection = currentDirection;
                }

                if (!rotating && lastPosition == transform.position)
                    mobile.ChangeEnemyState(MobileStates.Idle);
                else
                    mobile.ChangeEnemyState(MobileStates.Move);
            }

            lastPosition = transform.position;
        }

        void ApplyFallDamage()
        {
            // Bossfall doubles vanilla falling damage and starts damaging at a lower threshold.
            // Enemies are also affected by this change.
            const float fallingDamageThreshold = 3.8f;
            const float HPPerMetre = 10f;

            if (controller.isGrounded)
            {
                // If player is outside enemies will never suffer fall damage. I do this as BossfallEnemyMotor
                // can't access EnemyMotor's lastGroundedY and thus I would re-introduce enemies dying from fall damage when
                // FloatingOrigin ticks on player (this was fixed in a previous DFU version). I obviously don't want to
                // re-create already fixed bugs, so Bossfall enemies don't suffer fall damage outside as a crude workaround.
                if (falls && GameManager.Instance.PlayerEnterExit.IsPlayerInside)
                {
                    float fallDistance = lastGroundedY - transform.position.y;
                    if (fallDistance > fallingDamageThreshold)
                    {
                        int damage = (int)(HPPerMetre * (fallDistance - fallingDamageThreshold));

                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;
                        enemyEntity.DecreaseHealth(damage);

                        if (entityBlood)
                        {
                            entityBlood.ShowBloodSplash(0, transform.position);
                        }

                        DaggerfallUI.Instance.DaggerfallAudioSource.PlayClipAtPoint((int)SoundClips.FallDamage, FindGroundPosition());
                    }
                }

                lastGroundedY = transform.position.y;
            }
        }

        void OpenDoors()
        {
            if (mobile.Enemy.CanOpenDoors)
            {
                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.LastKnownDoor != null && senses.DistanceToDoor < motor.OpenDoorDistance && !senses.LastKnownDoor.IsOpen
                    && !senses.LastKnownDoor.IsLocked)
                {
                    senses.LastKnownDoor.ToggleDoor();
                    return;
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                Bashing = DaggerfallUnity.Settings.EnhancedCombatAI && !bossfallSenses.TargetInSight && moveInForAttack
                    && senses.LastKnownDoor != null && senses.DistanceToDoor <= attack.MeleeDistance && senses.LastKnownDoor.IsLocked;
            }
        }

        void HeightAdjust()
        {
            if (!resetHeight && controller && ((controller.collisionFlags & CollisionFlags.CollidedSides) != 0) && originalHeight > doorCrouchingHeight)
            {
                centerChange = (doorCrouchingHeight - controller.height) / 2;
                Vector3 newCenter = controller.center;
                newCenter.y += centerChange;
                controller.center = newCenter;
                controller.height = doorCrouchingHeight;
                resetHeight = true;
                heightChangeTimer = 0.5f;
            }
            else if (resetHeight && heightChangeTimer <= 0)
            {
                Vector3 newCenter = controller.center;
                newCenter.y -= centerChange;
                controller.center = newCenter;
                controller.height = originalHeight;
                resetHeight = false;
            }

            if (resetHeight && heightChangeTimer > 0)
            {
                heightChangeTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Bossfall AI breaks EnhancedCombatAI aiming. Arrow aiming is easily fixed in BossfallEnemyAttack, but fixing
        /// spell aiming turned out to be far more complicated. This method searches for spells that have been cast by this enemy,
        /// destroys them if they are very close to this enemy as their aim direction is always wrong, instantiates an identical
        /// spell, and fires it in the correct direction. Only active if EnhancedCombatAI is on. This method uses code from
        /// vanilla's DaggerfallMissile and EntityEffectManager scripts.
        /// </summary>
        void RangedSpellCorrection()
        {
            DaggerfallMissile[] missiles = FindObjectsOfType<DaggerfallMissile>();

            for (int i = 0; i < missiles.Length; i++)
            {
                DaggerfallMissile missile = missiles[i];

                var distance = Vector3.Distance(entityBehaviour.transform.position, missile.transform.position) * MeshReader.GlobalScale;
                DaggerfallUI.AddHUDText($"Distance is {0}.", distance);

                if ((Vector3.Distance(entityBehaviour.transform.position, missile.transform.position) * MeshReader.GlobalScale) > 0.06f)
                {
                    continue;
                }
                else if (missile.Payload.CasterEntityBehaviour == entityBehaviour)
                {
                    DaggerfallUI.AddHUDText("Test2.");
                    Destroy(missile.gameObject);
                    DaggerfallMissile newMissile = manager.InstantiateSpellMissile(missile.Payload.Settings.ElementType);
                    if (newMissile)
                    {
                        newMissile.Payload = missile.Payload;
                        Vector3 newPredictedPosition = bossfallSenses.PredictNextTargetPos(25.0f);
                        if (newPredictedPosition == EnemySenses.ResetPlayerPos)
                            newMissile.CustomAimDirection = entityBehaviour.transform.forward;
                        else
                            newMissile.CustomAimDirection = (newPredictedPosition - entityBehaviour.transform.position).normalized;

                        runRangedSpellCorrection = false;
                    }
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// If using EnhancedCombatAI this runs every time this enemy casts a spell. It activates a method that
        /// fixes vanilla ranged spell aiming and directly fixes vanilla touch spell aiming (Bossfall AI breaks both). This
        /// method uses code from DaggerfallMissile.
        /// </summary>
        /// <param name="spell">The spell to be re-targeted.</param>
        void BossfallOnCastReadySpell(EntityEffectBundle spell)
        {
            if (spell.Settings.TargetType == TargetTypes.SingleTargetAtRange || spell.Settings.TargetType == TargetTypes.AreaAtRange)
                runRangedSpellCorrection = true;
            if (spell.Settings.TargetType == TargetTypes.ByTouch)
            {
                DaggerfallMissile[] missiles = FindObjectsOfType<DaggerfallMissile>();

                for (int i = 0; i < missiles.Length; i++)
                {
                    DaggerfallMissile missile = missiles[i];
                    if (missile.Payload.CasterEntityBehaviour == entityBehaviour &&
                        missile.Payload.Settings.TargetType == TargetTypes.ByTouch)
                    {
                        Destroy(missile.gameObject);
                        DaggerfallUI.AddHUDText("Destroyed.");
                    }
                }
                Vector3 aimPosition = entityBehaviour.transform.position;
                Vector3 aimDirection;
                Vector3 predictedPosition = bossfallSenses.PredictNextTargetPos(25.0f);

                DaggerfallUI.AddHUDText("Activated.");

                if (predictedPosition == EnemySenses.ResetPlayerPos)
                    aimDirection = entityBehaviour.transform.forward;
                else
                    aimDirection = (predictedPosition - entityBehaviour.transform.position).normalized;
                
                DaggerfallEntityBehaviour targetEntity = DaggerfallMissile.GetEntityTargetInTouchRange(aimPosition, aimDirection);

                if (targetEntity && targetEntity != entityBehaviour)
                {
                    EntityEffectManager targetManager = targetEntity.GetComponent<EntityEffectManager>();
                    if (targetManager)
                        targetManager.AssignBundle(spell, AssignBundleFlags.ShowNonPlayerFailures);
                    DaggerfallUI.AddHUDText("Assigned.");
                }
            }
        }

        #endregion
    }
}
