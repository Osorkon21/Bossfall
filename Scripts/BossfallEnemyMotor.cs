// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich
// 
// Notes: This script uses code from vanilla DFU's EnemyMotor script. Comments indicate authorship, please verify authorship
//        before crediting. When in doubt compare to vanilla DFU's source code.
//

using BossfallMod.Formulas;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BossfallMod.EnemyAI
{
    /// <summary>
    /// Bossfall enemy AI.
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
        // I changed this region name to Fields.
        #region Fields

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
        float originalHeight;

        // I added the following 8 lines.
        bool prefersBow;
        bool alwaysCharges;
        bool isBoss;
        bool showBossWarning;
        bool showPowerfulBossWarning;
        EnemyMotor motor;
        MethodInfo setAccessor;
        FieldInfo lastGroundedY;

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

        // I added this entire region.
        #region Properties

        /// <summary>
        /// Used to make each enemy spell cost a flat 40 Magicka, regardless of actual spell cost.
        /// </summary>
        public int EnemyMagickaBeforeCastingSpell { get; set; }

        /// <summary>
        /// This is used to display a custom enemy weakness/resistance/immunity HUD message.
        /// </summary>
        public bool ShownMsg { get; set; }

        /// <summary>
        /// This is used to display a custom enemy weakness/resistance/immunity HUD message.
        /// </summary>
        public bool ShownMsgTwo { get; set; }

        /// <summary>
        /// This is used to display a custom enemy weakness/resistance/immunity HUD message.
        /// </summary>
        public bool ShownMsgThree { get; set; }

        /// <summary>
        /// This is used to display a custom enemy weakness/resistance/immunity HUD message.
        /// </summary>
        public bool ShownMsgFour { get; set; }

        /// <summary>
        /// This is used to display a custom enemy weakness/resistance/immunity HUD message.
        /// </summary>
        public bool ShownMsgFive { get; set; }

        /// <summary>
        /// This is used to display a custom enemy weakness/resistance/immunity HUD message.
        /// </summary>
        public bool ShownMsgSix { get; set; }

        #endregion

        #region Unity Methods

        void Start()
        {
            // I added the next 5 lines.
            motor = GetComponent<EnemyMotor>();
            Type type = motor.GetType();
            PropertyInfo property = type.GetProperty("Bashing");
            lastGroundedY = type.GetField("lastGroundedY", BindingFlags.NonPublic | BindingFlags.Instance);
            setAccessor = property.GetSetMethod(true);

            senses = GetComponent<EnemySenses>();
            controller = GetComponent<CharacterController>();
            mobile = GetComponentInChildren<MobileUnit>();
            myCollider = gameObject.GetComponent<Collider>();

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            motor.IsHostile = mobile.Enemy.Reactions == MobileReactions.Hostile;

            // I reroute the method call to a method in this script.
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

            // Using Reflection I assign to vanilla EnemyMotor's lastGroundedY field.
            lastGroundedY.SetValue(motor, transform.position.y);

            originalHeight = controller.height;

            // If enemy is an Archer or Ranger prefersBow is true.
            if (mobile.Enemy.ID == 141 || mobile.Enemy.ID == 142)
                prefersBow = true;

            // If enemy is non-sentient or very stupid alwaysCharges is true.
            if (mobile.Enemy.ID == 32 || mobile.Enemy.ID == 33)
                alwaysCharges = false;
            else if (mobile.Enemy.Affinity == MobileAffinity.Animal || mobile.Enemy.Affinity == MobileAffinity.Undead)
                alwaysCharges = true;
            else if (mobile.Enemy.ID == 11 || mobile.Enemy.ID == 16 || mobile.Enemy.ID == 22 || mobile.Enemy.ID == 143)
                alwaysCharges = true;

            // If enemy is a boss isBoss is true. If enemy is an OrcWarlord/Vampire/Lich/Dragonling_Alternate
            // showBossWarning is true. If enemy is a VampireAncient/DaedraLord/AncientLich showPowerfulBossWarning is true.
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

            // I reroute this method call to a method in this script.
            flies = CanFly();

            canAct = true;
            flyerFalls = false;
            falls = false;

            // I reroute these method calls to methods in this script.
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

            // If the Boss Proximity Warning setting is on, this new check checks for nearby bosses and creates warning
            // HUD messages if there are any. No message will display for stealthy Assassins.
            if (Bossfall.Instance.BossProximityWarning && isBoss)
            {
                if (showBossWarning)
                {
                    if (senses.DistanceToPlayer < 25.6f)
                    {
                        DaggerfallUI.AddHUDText("You sense a boss nearby.");
                        showBossWarning = false;
                    }
                }
                else if (showPowerfulBossWarning)
                {
                    if (senses.DistanceToPlayer < 25.6f)
                    {
                        DaggerfallUI.AddHUDText("You sense a powerful boss nearby.");
                        showPowerfulBossWarning = false;
                    }
                }
            }
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

                // I redirect property calls to vanilla properties, using Reflection if necessary.
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

                    // I redirect property calls to vanilla properties, using Reflection if necessary.
                    motion = motor.KnockbackDirection * motor.KnockbackSpeed;
                else
                    // I redirect property calls to vanilla properties, using Reflection if necessary.
                    motion = motor.KnockbackDirection * (20 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (swims)

                    // I reroute the method call to a method in this script.
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

                    // I redirect property calls to vanilla properties, using Reflection if necessary.
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
                    // I reroute the method call to a method in this script.
                    EvaluateMoveInForAttack();
                }

                canAct = false;
                flyerFalls = true;
            }
        }

        void ApplyGravity()
        {
            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (!flies && !swims && !motor.IsLevitating && !controller.isGrounded)
            {
                controller.SimpleMove(Vector3.zero);
                falls = true;

                if (lastPosition != transform.position)
                    canAct = false;
            }

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (flyerFalls && flies && !motor.IsLevitating)
            {
                controller.SimpleMove(Vector3.zero);
                falls = true;
            }
        }

        void HandleNoAction()
        {
            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (senses.Target == null || motor.GiveUpTimer <= 0 || senses.PredictedTargetPos == EnemySenses.ResetPlayerPos)
            {
                // I reroute the method call to a method in this script.
                SetChangeStateTimer();
                searchMult = 0;

                canAct = false;
            }
        }

        void HandleBashing()
        {
            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (motor.Bashing)
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

                // I redirect property calls to vanilla properties, using Reflection if necessary.
                motor.GiveUpTimer = 200;

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (GameManager.ClassicUpdate && !senses.DetectedTarget && motor.GiveUpTimer > 0)

                // I redirect property calls to vanilla properties, using Reflection if necessary.
                motor.GiveUpTimer--;
        }

        void TakeAction()
        {
            // I changed the declaration of this variable to avoid unnecessary moveSpeed calculations.
            float moveSpeed;

            // This sets enemy movement speed based on which "Enemy Move Speed" setting is used.
            if (Bossfall.Instance.EnemyMoveSpeed == 1)
            {
                moveSpeed = BossfallOverrides.Instance.FastMoveSpeeds[mobile.Enemy.ID];
            }
            else if (Bossfall.Instance.EnemyMoveSpeed == 2)
            {
                moveSpeed = BossfallOverrides.Instance.VeryFastMoveSpeeds[mobile.Enemy.ID];
            }
            else
            {
                // This is vanilla's moveSpeed formula.
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

            // I reroute the method call to a method in this script.
            GetDestination();

            Vector3 direction = (destination - transform.position).normalized;

            float distance;

            if (avoidObstaclesTimer <= 0 && senses.TargetInSight)

                distance = senses.DistanceToTarget;
            else
                distance = (destination - transform.position).magnitude;

            if (isPlayingOneShot && mobile.OneShotPauseActionsWhilePlaying())
                return;

            // I reroute the method call to a method in this script.
            if (DoRangedAttack(direction, moveSpeed, distance, isPlayingOneShot))
                return;

            // I reroute the method call to a method in this script.
            if (DoTouchSpell())
                return;

            if (moveInForAttackTimer <= 0 && avoidObstaclesTimer <= 0)

                // I reroute the method call to a method in this script.
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
                // I reroute the method call to a method in this script.
                AttemptMove(direction, moveSpeed);
            }
            else if ((!retreating && distance >= (stopDistance * 2.75)) || (distance > stopDistance && moveInForAttack))
            {
                if (changeStateTimer <= 0 || pursuing)

                    // I reroute the method call to a method in this script.
                    AttemptMove(direction, moveSpeed);

                else if (!senses.TargetIsWithinYawAngle(22.5f, destination))

                    // I reroute the method call to a method in this script.
                    TurnToTarget(direction);
            }
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && strafeTimer <= 0)
            {
                // I reroute the method call to a method in this script.
                StrafeDecision();
            }
            else if (doStrafe && strafeTimer > 0 && (distance >= stopDistance * .8f))
            {
                // I reroute the method call to a method in this script.
                AttemptMove(direction, moveSpeed / 4, false, true, distance);
            }
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && senses.TargetInSight && (distance < stopDistance * .8f ||
                !moveInForAttack && distance < stopDistance * retreatDistanceMultiplier && (changeStateTimer <= 0 || retreating)))
            {
                if (changeStateTimer <= 0 || retreating)

                    // Vanilla has moveSpeed / 2 here, I removed the 2. This makes enemies back up at full speed.
                    // The player can run backwards at full speed, I wanted enemies to do so as well. It's only fair.
                    // I also reroute the method call to a method in this script.
                    AttemptMove(direction, moveSpeed, true);
            }
            else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
            {
                // I reroute the method call to a method in this script.
                TurnToTarget(direction);
            }
            else
            {
                // I reroute the method call to a method in this script.
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
            // I reroute the method call to a method in this script.
            else if (ClearPathToPosition(senses.PredictedTargetPos, (destination - transform.position).magnitude) || (senses.TargetInSight && (hasBowAttack || entity.CurrentMagicka > 0)))
            {
                destination = senses.PredictedTargetPos;

                // I redirect property calls to vanilla properties, using Reflection if necessary.
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

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (avoidObstaclesTimer <= 0 && !flies && !motor.IsLevitating && !swims && senses.Target)
            {
                float deltaHeight = (targetController.height - originalHeight) / 2;
                destination.y -= deltaHeight;
            }
        }

        bool DoRangedAttack(Vector3 direction, float moveSpeed, float distance, bool isPlayingOneShot)
        {
            // I use different min/max range values here. Enemies now fire bows and spells at closer and longer ranges.
            bool inRange = senses.DistanceToTarget > BossfallEnemyAttack.minRangedDistance && senses.DistanceToTarget < BossfallEnemyAttack.maxRangedDistance;

            // I reroute these method calls to methods in this script.
            if (inRange && senses.TargetInSight && senses.DetectedTarget && (CanShootBow() || CanCastRangedSpell()))
            {
                if (DaggerfallUnity.Settings.EnhancedCombatAI && senses.TargetIsWithinYawAngle(22.5f, destination) && strafeTimer <= 0)
                {
                    // I reroute the method call to a method in this script.
                    StrafeDecision();
                }

                if (doStrafe && strafeTimer > 0)
                {
                    // Vanilla has moveSpeed / 4 here, I changed that to 2 so enemies at range strafe faster.
                    // I got annoyed at how easy it was to hit enemies with arrows at long distances.
                    // I also reroute the method call to a method in this script.
                    AttemptMove(direction, moveSpeed / 2, false, true, distance);
                }

                if (GameManager.ClassicUpdate && senses.TargetIsWithinYawAngle(22.5f, destination))
                {
                    if (!isPlayingOneShot)
                    {
                        if (hasBowAttack)
                        {
                            // Vanilla has 1/32f here. I thought bow attacks were too infrequent. I also added "UnityEngine."
                            // to the below line.
                            if (UnityEngine.Random.value < 1 / 18f)
                            {
                                if (mobile.Enemy.HasRangedAttack1 && !mobile.Enemy.HasRangedAttack2)
                                    mobile.ChangeEnemyState(MobileStates.RangedAttack1);
                                else if (mobile.Enemy.HasRangedAttack2)
                                    mobile.ChangeEnemyState(MobileStates.RangedAttack2);
                            }
                        }

                        // Vanilla has 1/40f here. Bossfall enemies are magical machine guns. I also added "UnityEngine."
                        // to the below line.
                        else if (UnityEngine.Random.value < 1 / 15f && entityEffectManager.SetReadySpell(selectedSpell))
                        {
                            mobile.ChangeEnemyState(MobileStates.Spell);
                        }
                    }
                }
                else
                    // I reroute the method call to a method in this script.
                    TurnToTarget(direction);

                return true;
            }

            return false;
        }

        bool DoTouchSpell()
        {
            // I reroute the method call to a method in this script.
            if (senses.TargetInSight && senses.DetectedTarget && attack.MeleeTimer == 0
                && senses.DistanceToTarget <= attack.MeleeDistance + senses.TargetRateOfApproach
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
            // I changed the maximum range from 4 to 5, reducing strafe frequency. I also added "UnityEngine." to the
            // following two lines.
            doStrafe = UnityEngine.Random.Range(0, 5) == 0;
            strafeTimer = UnityEngine.Random.Range(1f, 2f);
            if (doStrafe)
            {
                // I added "UnityEngine." to the below line.
                if (UnityEngine.Random.Range(0, 2) == 0)
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

            // I reroute these method calls to methods in this script.
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
            Vector3 sphereCastDir = senses.PredictNextTargetPos(speed);
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

            // I reroute the method call to a method in this script.
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

            // I added "UnityEngine." to the below line.
            EffectBundleSettings selectedSpellSettings = rangeSpells[UnityEngine.Random.Range(0, count)];
            selectedSpell = new EntityEffectBundle(selectedSpellSettings, entityBehaviour);

            // I added this conditional. It prevents enemy from casting Heal if enemy is at full health.
            if (selectedSpell.Settings.StandardSpellIndex == 0x40 && entity.CurrentHealth == entity.MaxHealth)
                return false;


            // I reroute the method call to a method in this script.
            if (EffectsAlreadyOnTarget(selectedSpell))
                return false;

            // I reroute the method call to a method in this script.
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

            // I added "UnityEngine." to the below line.
            EffectBundleSettings selectedSpellSettings = rangeSpells[UnityEngine.Random.Range(0, count)];
            selectedSpell = new EntityEffectBundle(selectedSpellSettings, entityBehaviour);

            // I added this conditional. It prevents enemy from casting Heal if enemy is at full health.
            if (selectedSpell.Settings.StandardSpellIndex == 0x40 && entity.CurrentHealth == entity.MaxHealth)
                return false;

            // I reroute the method call to a method in this script.
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
            // I added "spell.Settings.TargetType != TargetTypes.CasterOnly" to the following line. 
            if (senses.Target && spell.Settings.TargetType != TargetTypes.CasterOnly)
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
            // I added this else if. It prevents enemies from re-casting Caster Only spells unnecessarily. Most of the code
            // within this block is vanilla's from the preceding if block. Comments precede changes or additions I made.
            else if (spell.Settings.TargetType == TargetTypes.CasterOnly)
            {
                // If enemy wants to cast Heal (0x40) again while still under the effects of a previous Heal spell,
                // I don't want to stop them. If they're trying to cast Heal, this code can only be reached if
                // enemy isn't at full health.
                if (spell.Settings.StandardSpellIndex == 0x40)
                    return false;

                for (int i = 0; i < spell.Settings.Effects.Length; i++)
                {
                    bool foundEffect = false;
                    IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(spell.Settings.Effects[i].Key);

                    // I replaced "bundles" with "entityEffectManager.EffectBundles" in the following line.
                    for (int j = 0; j < entityEffectManager.EffectBundles.Length && !foundEffect; j++)
                    {
                        // I replaced "bundles" with "entityEffectManager.EffectBundles" in the following line.
                        for (int k = 0; k < entityEffectManager.EffectBundles[j].liveEffects.Count && !foundEffect; k++)
                        {
                            // I replaced "bundles" with "entityEffectManager.EffectBundles" in the following line.
                            if (entityEffectManager.EffectBundles[j].liveEffects[k].GetType() == effectTemplate.GetType())
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
                // I reroute the method call to a method in this script.
                TurnToTarget(direction);

                if (!DaggerfallUnity.Settings.EnhancedCombatAI || !senses.TargetInSight)
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

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (!flies && !swims && !motor.IsLevitating && controller.isGrounded)
                direction.y = -2f;

            // I reroute the FindGroundPosition method call to vanilla's method.
            if (flies && avoidObstaclesTimer <= 0 && direction.y < 0 && motor.FindGroundPosition((originalHeight / 2) + 1f) != transform.position)
                direction.y = 0.1f;

            Vector3 motion = direction * moveSpeed;

            if (!backAway && DaggerfallUnity.Settings.EnhancedCombatAI && avoidObstaclesTimer <= 0)
            {
                bool withinPitch = senses.TargetIsWithinPitchAngle(45.0f);
                if (!pausePursuit && !withinPitch)
                {
                    // I redirect property calls to vanilla properties, using Reflection if necessary.
                    if (flies || motor.IsLevitating || swims)
                    {
                        if (!senses.TargetIsAbove())
                            motion = -transform.up * moveSpeed / 2;
                        else
                            motion = transform.up * moveSpeed;
                    }
                    else if (senses.TargetIsAbove() && changeStateTimer <= 0)
                    {
                        // I reroute the method call to a method in this script.
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

            // I reroute the method call to a method in this script.
            SetChangeStateTimer();

            Vector3 direction2d = direction;

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (!flies && !swims && !motor.IsLevitating)
                direction2d.y = 0;

            // I reroute these method calls to methods in this script.
            ObstacleCheck(direction2d);
            FallCheck(direction2d);

            if (fallDetected || obstacleDetected)
            {
                if (!strafe && !backAway)

                    // I reroute the method call to a method in this script.
                    FindDetour(direction2d);
            }
            else
            {
                if (swims)

                    // I reroute the method call to a method in this script.
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

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (flies || swims || motor.IsLevitating)
            {
                float multiplier = 0.3f;

                // I added "UnityEngine." to the below line.
                if (UnityEngine.Random.Range(0, 2) == 0)
                    multiplier = -0.3f;

                Vector3 upOrDown = new Vector3(0, 1, 0);
                upOrDown.y *= multiplier;

                testMove = (direction2d + upOrDown).normalized;

                // I reroute the method call to a method in this script.
                ObstacleCheck(testMove);
                if (obstacleDetected)
                {
                    upOrDown.y *= -1;
                    testMove = (direction2d + upOrDown).normalized;

                    // I reroute the method call to a method in this script.
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
                    // I added "UnityEngine." to the below line.
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        angle = 45;
                    else
                        angle = -45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;

                    // I reroute these method calls to methods in this script.
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

                        // I reroute these method calls to methods in this script.
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

                    // I reroute these method calls to methods in this script.
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

                // I redirect property calls to vanilla properties, using Reflection if necessary.
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
            // I redirect property calls to vanilla properties, using Reflection if necessary.
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

            // I redirect property calls to vanilla properties, using Reflection if necessary.
            if (!senses.TargetInSight)
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

            // I added "UnityEngine." to the below line.
            moveInForAttackTimer = UnityEngine.Random.Range(1, 3);
            int levelMod = (entity.Level - senses.Target.Entity.Level) / 2;

            if (levelMod > 4)
                levelMod = 4;

            // Vanilla has levelMod < -4 here. I find enemies constantly retreating really annoying,
            // so I greatly reduced the likelihood of that occurring.
            if (levelMod < 0)
                levelMod = 0;

            // I added "UnityEngine." to the below line.
            int roll = UnityEngine.Random.Range(0 + levelMod, 10 + levelMod);

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

                // I added "UnityEngine." to the below line.
                if (UnityEngine.Random.Range(0, 2) == 0)
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

                // I added "UnityEngine." to the below line.
                changeStateTimer = UnityEngine.Random.Range(0.2f, 0.8f);
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
                if (falls)
                {
                    // Using Reflection I read from vanilla EnemyMotor's lastGroundedY field.
                    float fallDistance = (float)lastGroundedY.GetValue(motor) - transform.position.y;
                    if (fallDistance > fallingDamageThreshold)
                    {
                        int damage = (int)(HPPerMetre * (fallDistance - fallingDamageThreshold));

                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;
                        enemyEntity.DecreaseHealth(damage);

                        if (entityBlood)
                        {
                            entityBlood.ShowBloodSplash(0, transform.position);
                        }

                        // I call vanilla DFU's FindGroundPosition method here.
                        DaggerfallUI.Instance.DaggerfallAudioSource.PlayClipAtPoint((int)SoundClips.FallDamage, motor.FindGroundPosition());
                    }
                }

                // Using Reflection I assign to vanilla EnemyMotor's lastGroundedY field.
                lastGroundedY.SetValue(motor, transform.position.y);
            }
        }

        void OpenDoors()
        {
            if (mobile.Enemy.CanOpenDoors)
            {
                // I redirect property calls to vanilla properties, using Reflection if necessary.
                if (senses.LastKnownDoor != null && senses.DistanceToDoor < motor.OpenDoorDistance && !senses.LastKnownDoor.IsOpen
                    && !senses.LastKnownDoor.IsLocked)
                {
                    senses.LastKnownDoor.ToggleDoor();
                    return;
                }

                // I added this variable declaration - the assignment value calculation is all vanilla's, from the EnemyMotor script.
                bool bashing = DaggerfallUnity.Settings.EnhancedCombatAI && !senses.TargetInSight && moveInForAttack
                    && senses.LastKnownDoor != null && senses.DistanceToDoor <= attack.MeleeDistance && senses.LastKnownDoor.IsLocked;

                // Using Reflection, I set the value of vanilla EnemyMotor's Bashing property.
                setAccessor.Invoke(motor, new object[] { bashing } );
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

        #endregion
    }
}
