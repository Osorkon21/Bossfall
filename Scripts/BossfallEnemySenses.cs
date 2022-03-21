// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich
// 
// Notes: This script uses code from vanilla DFU's EnemySenses and EnemyEntity scripts. Comments indicate authorship,
//        please verify authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using BossfallMod.Formulas;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using System;
using System.Reflection;
using UnityEngine;

namespace BossfallMod.EnemyAI
{
    /// <summary>
    /// Bossfall enemy AI.
    /// </summary>
    public class BossfallEnemySenses : MonoBehaviour
    {
        #region Fields

        const float predictionInterval = 0.0625f;
        const float systemTimerUpdatesDivisor = .0549254f;
        const float classicSpawnDespawnExterior = 4096 * MeshReader.GlobalScale;

        MobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        EnemyMotor motor;
        EnemyEntity enemyEntity;
        bool playerInSight;
        DaggerfallEntityBehaviour player;
        DaggerfallEntityBehaviour targetOnLastUpdate;
        bool sawSecondaryTarget;
        Vector3 secondaryTargetPos;
        EnemySenses targetSenses;
        float lastDistanceToTarget;
        bool awareOfTargetForLastPrediction;
        bool blockedByIllusionEffect = false;
        float lastHadLOSTimer = 0f;
        bool targetPosPredict = false;
        float classicTargetUpdateTimer = 0f;
        float classicSpawnXZDist = 0f;
        float classicSpawnYDistUpper = 0f;
        float classicSpawnYDistLower = 0f;
        float classicDespawnXZDist = 0f;
        float classicDespawnYDist = 0f;

        // I added the following 7 fields.
        EnemySenses senses;
        FieldInfo distanceToPlayer;
        FieldInfo targetInSight;
        FieldInfo targetInEarshot;
        FieldInfo targetPosPredictTimer;
        FieldInfo distanceToTarget;
        FieldInfo directionToTarget;

        #endregion

        #region Properties

        /// <summary>
        /// I added this property.
        /// </summary>
        public bool CanDetectInvisible { get; set; }

        #endregion

        #region Unity

        /// <summary>
        /// I moved everything in vanilla DFU's EnemySenses Start method to this Awake method.
        /// </summary>
        void Awake()
        {
            // I added this line.
            senses = GetComponent<EnemySenses>();

            // I added the next 7 lines. They're mostly not necessary for Bossfall. I added them so I don't break other mods.
            Type type = senses.GetType();
            distanceToPlayer = type.GetField("distanceToPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
            targetInSight = type.GetField("targetInSight", BindingFlags.NonPublic | BindingFlags.Instance);
            targetInEarshot = type.GetField("targetInEarshot", BindingFlags.NonPublic | BindingFlags.Instance);
            targetPosPredictTimer = type.GetField("targetPosPredictTimer", BindingFlags.NonPublic | BindingFlags.Instance);
            distanceToTarget = type.GetField("distanceToTarget", BindingFlags.NonPublic | BindingFlags.Instance);
            directionToTarget = type.GetField("directionToTarget", BindingFlags.NonPublic | BindingFlags.Instance);

            mobile = GetComponent<DaggerfallEnemy>().MobileUnit;
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            enemyEntity = entityBehaviour.Entity as EnemyEntity;
            motor = GetComponent<EnemyMotor>();
            player = GameManager.Instance.PlayerEntityBehaviour;

            short[] classicSpawnXZDistArray = { 1024, 384, 640, 768, 768, 768, 768 };
            short[] classicSpawnYDistUpperArray = { 128, 128, 128, 384, 768, 128, 256 };
            short[] classicSpawnYDistLowerArray = { 0, 0, 0, 0, -128, -768, 0 };
            short[] classicDespawnXZDistArray = { 1024, 1024, 1024, 1024, 768, 768, 768 };
            short[] classicDespawnYDistArray = { 384, 384, 384, 384, 768, 768, 768 };

            byte index = mobile.ClassicSpawnDistanceType;

            classicSpawnXZDist = classicSpawnXZDistArray[index] * MeshReader.GlobalScale;
            classicSpawnYDistUpper = classicSpawnYDistUpperArray[index] * MeshReader.GlobalScale;
            classicSpawnYDistLower = classicSpawnYDistLowerArray[index] * MeshReader.GlobalScale;
            classicDespawnXZDist = classicDespawnXZDistArray[index] * MeshReader.GlobalScale;
            classicDespawnYDist = classicDespawnYDistArray[index] * MeshReader.GlobalScale;

            // I added this. It checks if enemy is a Level 20 Mage, Sorcerer, or Nightblade.
            if (enemyEntity.Level == 20 && (mobile.Enemy.ID == 128 || mobile.Enemy.ID == 131 || mobile.Enemy.ID == 133))
                CanDetectInvisible = true;
        }

        void FixedUpdate()
        {
            // I reversed the DisableAI check so player can swap between Bossfall & vanilla AI with the console.
            if (!GameManager.Instance.DisableAI)
                return;

            // I read from/assign to vanilla DFU's EnemySenses targetPosPredictTimer field here using Reflection.
            targetPosPredictTimer.SetValue(senses, (float)targetPosPredictTimer.GetValue(senses) + Time.deltaTime);

            // I read from vanilla DFU's EnemySenses targetPosPredictTimer field here using Reflection.
            if ((float)targetPosPredictTimer.GetValue(senses) >= predictionInterval)
            {
                // I assign to vanilla DFU's EnemySenses targetPosPredictTimer field here using Reflection.
                targetPosPredictTimer.SetValue(senses, 0f);

                targetPosPredict = true;
            }
            else
                targetPosPredict = false;

            if (GameManager.ClassicUpdate)
            {
                // I call vanilla's field, property, or method here, using Reflection if necessary.
                if (senses.DistanceToPlayer < 1094 * MeshReader.GlobalScale)
                {
                    float upperXZ;
                    float upperY = 0;
                    float lowerY = 0;
                    bool playerInside = GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>().IsPlayerInside;

                    if (!playerInside)
                    {
                        upperXZ = classicSpawnDespawnExterior;
                    }
                    else
                    {
                        // I call vanilla's field, property, or method here, using Reflection if necessary.
                        if (!senses.WouldBeSpawnedInClassic)
                        {
                            upperXZ = classicSpawnXZDist;
                            upperY = classicSpawnYDistUpper;
                            lowerY = classicSpawnYDistLower;
                        }
                        else
                        {
                            upperXZ = classicDespawnXZDist;
                            upperY = classicDespawnYDist;
                        }
                    }

                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    float YDiffToPlayer = senses.transform.position.y - player.transform.position.y;
                    float YDiffToPlayerAbs = Mathf.Abs(YDiffToPlayer);
                    float distanceToPlayerXZ = Mathf.Sqrt(senses.DistanceToPlayer * senses.DistanceToPlayer - YDiffToPlayerAbs * YDiffToPlayerAbs);

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.WouldBeSpawnedInClassic = true;

                    if (distanceToPlayerXZ > upperXZ)

                        // I call vanilla's field, property, or method here, using Reflection if necessary.
                        senses.WouldBeSpawnedInClassic = false;

                    if (playerInside)
                    {
                        if (lowerY == 0)
                        {
                            if (YDiffToPlayerAbs > upperY)

                                // I call vanilla's field, property, or method here, using Reflection if necessary.
                                senses.WouldBeSpawnedInClassic = false;
                        }
                        else if (YDiffToPlayer < lowerY || YDiffToPlayer > upperY)

                            // I call vanilla's field, property, or method here, using Reflection if necessary.
                            senses.WouldBeSpawnedInClassic = false;
                    }
                }
                else
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.WouldBeSpawnedInClassic = false;
            }

            if (GameManager.ClassicUpdate)
            {
                classicTargetUpdateTimer += Time.deltaTime / systemTimerUpdatesDivisor;

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (senses.Target != null && senses.Target.Entity.CurrentHealth <= 0)
                {
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.Target = null;
                }

                if (GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile)
                {
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    if (senses.Target == player)

                        // I call vanilla's field, property, or method here, using Reflection if necessary.
                        senses.Target = null;

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    if (senses.SecondaryTarget == player)

                        // I call vanilla's field, property, or method here, using Reflection if necessary.
                        senses.SecondaryTarget = null;
                }

                // I call vanilla's field, property, or method here, using Reflection if necessary.
                if (senses.Target == null)
                {
                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    senses.LastKnownTargetPos = EnemySenses.ResetPlayerPos;
                    senses.PredictedTargetPos = EnemySenses.ResetPlayerPos;

                    // I assign to vanilla DFU's EnemySenses directionToTarget field here using Reflection.
                    directionToTarget.SetValue(senses, EnemySenses.ResetPlayerPos);

                    lastDistanceToTarget = 0;
                    senses.TargetRateOfApproach = 0;

                    // I assign to vanilla DFU's EnemySenses distanceToTarget field here using Reflection.
                    distanceToTarget.SetValue(senses, 0f);

                    targetSenses = null;

                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    if (senses.SecondaryTarget != null && senses.SecondaryTarget.Entity.CurrentHealth > 0)
                    {
                        // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                        senses.Target = senses.SecondaryTarget;

                        if (sawSecondaryTarget)

                            // I call vanilla's field, property, or method here, using Reflection if necessary.
                            senses.LastKnownTargetPos = secondaryTargetPos;
                        awareOfTargetForLastPrediction = false;
                    }
                }

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (senses.Target != null && senses.Target == targetOnLastUpdate)
                {
                    if (DaggerfallUnity.Settings.EnhancedCombatAI)

                        // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                        senses.TargetRateOfApproach = lastDistanceToTarget - senses.DistanceToTarget;
                }
                else
                {
                    lastDistanceToTarget = 0;

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.TargetRateOfApproach = 0;
                }

                // I call vanilla's field, property, or method here, using Reflection if necessary.
                if (senses.Target != null)
                {
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    lastDistanceToTarget = senses.DistanceToTarget;

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    targetOnLastUpdate = senses.Target;
                }
            }

            if (player != null)
            {
                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                Vector3 toPlayer = player.transform.position - senses.transform.position;

                // I assign to vanilla DFU's EnemySenses distanceToPlayer field here using Reflection.
                distanceToPlayer.SetValue(senses, toPlayer.magnitude);

                // I call vanilla's field, property, or method here, using Reflection if necessary.
                if (!senses.WouldBeSpawnedInClassic)
                {
                    // I assign to vanilla DFU's EnemySenses distanceToTarget field here using Reflection.
                    distanceToTarget.SetValue(senses, senses.DistanceToPlayer);

                    // I assign to vanilla DFU's EnemySenses directionToTarget field here using Reflection.
                    directionToTarget.SetValue(senses, toPlayer.normalized);

                    // I reroute the method call to a method in this script.
                    playerInSight = CanSeeTarget(player);
                }

                if (classicTargetUpdateTimer > 5)
                {
                    classicTargetUpdateTimer = 0f;

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    if (senses.WouldBeSpawnedInClassic || playerInSight)
                    {
                        // I reroute the method call to a method in this script.
                        GetTargets();

                        // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                        if (senses.Target != null && senses.Target != player)

                            // I call vanilla's field, property, or method here, using Reflection if necessary.
                            targetSenses = senses.Target.GetComponent<EnemySenses>();
                        else
                            targetSenses = null;
                    }

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    if (senses.Target != null && targetSenses && targetSenses.Target == null)
                    {
                        targetSenses.Target = entityBehaviour;
                    }
                }

                // I call vanilla's field, property, or method here, using Reflection if necessary.
                if (senses.Target == null)
                {
                    // I assign to vanilla DFU's EnemySenses targetInSight field here using Reflection.
                    targetInSight.SetValue(senses, false);

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.DetectedTarget = false;
                    return;
                }

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (!senses.WouldBeSpawnedInClassic && senses.Target == player)
                {
                    // I assign to vanilla DFU's EnemySenses distanceToTarget field here using Reflection.
                    distanceToTarget.SetValue(senses, senses.DistanceToPlayer);

                    // I assign to vanilla DFU's EnemySenses directionToTarget field here using Reflection.
                    directionToTarget.SetValue(senses, toPlayer.normalized);

                    // I assign to vanilla DFU's EnemySenses targetInSight field here using Reflection.
                    targetInSight.SetValue(senses, playerInSight);
                }
                else
                {
                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    Vector3 toTarget = senses.Target.transform.position - senses.transform.position;

                    // I assign to vanilla DFU's EnemySenses distanceToTarget field here using Reflection.
                    distanceToTarget.SetValue(senses, toTarget.magnitude);

                    // I assign to vanilla DFU's EnemySenses directionToTarget field here using Reflection.
                    directionToTarget.SetValue(senses, toTarget.normalized);

                    // I assign to vanilla DFU's EnemySenses targetInSight field here using Reflection, but
                    // I reroute the method call to a method in this script.
                    targetInSight.SetValue(senses, CanSeeTarget(senses.Target));
                }

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (senses.DetectedTarget && !senses.TargetInSight)

                    // I assign to vanilla DFU's EnemySenses targetInEarshot field here using Reflection, but
                    // I reroute the method call to a method in this script.
                    targetInEarshot.SetValue(senses, CanHearTarget());
                else
                    // I assign to vanilla DFU's EnemySenses targetInEarshot field here using Reflection.
                    targetInEarshot.SetValue(senses, false);

                if (GameManager.ClassicUpdate)
                {
                    // I reroute the method call to a method in this script.
                    blockedByIllusionEffect = BossfallBlockedByIllusionEffect();
                    if (lastHadLOSTimer > 0)
                        lastHadLOSTimer--;
                }

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (!blockedByIllusionEffect && (senses.TargetInSight || senses.TargetInEarshot))
                {
                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    senses.DetectedTarget = true;
                    senses.LastKnownTargetPos = senses.Target.transform.position;
                    lastHadLOSTimer = 200f;
                }
                // I call vanilla's field, property, or method here, using Reflection if necessary.
                else if (!blockedByIllusionEffect && senses.StealthCheck())
                {
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.DetectedTarget = true;

                    if (lastHadLOSTimer <= 0)

                        // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                        senses.LastKnownTargetPos = senses.Target.transform.position;
                }
                else
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.DetectedTarget = false;

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (senses.OldLastKnownTargetPos == EnemySenses.ResetPlayerPos)

                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    senses.OldLastKnownTargetPos = senses.LastKnownTargetPos;

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (senses.PredictedTargetPos == EnemySenses.ResetPlayerPos || !DaggerfallUnity.Settings.EnhancedCombatAI)

                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    senses.PredictedTargetPos = senses.LastKnownTargetPos;

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (targetPosPredict && senses.LastKnownTargetPos != EnemySenses.ResetPlayerPos)
                {
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    if (!blockedByIllusionEffect && senses.TargetInSight)
                    {
                        if (awareOfTargetForLastPrediction)

                            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                            senses.LastPositionDiff = senses.LastKnownTargetPos - senses.OldLastKnownTargetPos;

                        // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                        senses.OldLastKnownTargetPos = senses.LastKnownTargetPos;

                        awareOfTargetForLastPrediction = true;
                    }
                    else
                    {
                        awareOfTargetForLastPrediction = false;
                    }

                    if (DaggerfallUnity.Settings.EnhancedCombatAI)
                    {
                        // I changed the declaration of this variable.
                        float moveSpeed;

                        // This predicts target positions if player is using the "Very Fast" Enemy Move Speed setting.
                        if (Bossfall.Instance.EnemyMoveSpeed == 2)
                        {
                            moveSpeed = BossfallOverrides.Instance.VeryFastMoveSpeeds[mobile.Enemy.ID];
                        }
                        // This predicts target positions if player is using the "Fast" Enemy Move Speed setting.
                        else if (Bossfall.Instance.EnemyMoveSpeed == 1)
                        {
                            moveSpeed = BossfallOverrides.Instance.FastMoveSpeeds[mobile.Enemy.ID];
                        }
                        // This predicts target positions if player is using the "Vanilla" Enemy Move Speed setting.
                        // The move speed calculation formula is vanilla DFU's.
                        else
                        {
                            moveSpeed = (enemyEntity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) * MeshReader.GlobalScale;
                        }

                        // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                        senses.PredictedTargetPos = senses.PredictNextTargetPos(moveSpeed);
                    }
                }

                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                if (senses.DetectedTarget && !senses.HasEncounteredPlayer && senses.Target == player)
                {
                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    senses.HasEncounteredPlayer = true;

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    if (!senses.QuestBehaviour && entityBehaviour && motor &&
                        (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass))
                    {
                        // I rerouted this method call to a method I added to this script.
                        DFCareer.Skills languageSkill = GetBossfallLanguageSkill(enemyEntity.EntityType, enemyEntity.CareerIndex);
                        if (languageSkill != DFCareer.Skills.None)
                        {
                            PlayerEntity player = GameManager.Instance.PlayerEntity;
                            if (FormulaHelper.CalculateEnemyPacification(player, languageSkill))
                            {
                                motor.IsHostile = false;
                                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("languagePacified").Replace("%e", enemyEntity.Name).Replace("%s", languageSkill.ToString()), 5);

                                // I lowered TallySkill from 3 to 1.
                                player.TallySkill(languageSkill, 1);
                            }
                            else
                                // I removed the conditions that excluded Etiquette and Streetwise from getting tallies
                                // if enemy pacification wasn't successful.
                                player.TallySkill(languageSkill, 1);
                        }
                    }
                }
            }

            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
            if (senses.Target == GameManager.Instance.PlayerEntityBehaviour && senses.TargetInSight)
                GameManager.Instance.PlayerEntity.SetEnemyAlert(true);
        }

        #endregion

        #region Private Methods

        void GetTargets()
        {
            DaggerfallEntityBehaviour highestPriorityTarget = null;
            DaggerfallEntityBehaviour secondHighestPriorityTarget = null;
            float highestPriority = -1;
            float secondHighestPriority = -1;
            bool sawSelectedTarget = false;

            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
            Vector3 directionToTargetHolder = senses.DirectionToTarget;
            float distanceToTargetHolder = senses.DistanceToTarget;

            DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            for (int i = 0; i < entityBehaviours.Length; i++)
            {
                DaggerfallEntityBehaviour targetBehaviour = entityBehaviours[i];
                EnemyEntity targetEntity = null;
                if (targetBehaviour != player)
                    targetEntity = targetBehaviour.Entity as EnemyEntity;

                if (targetBehaviour == entityBehaviour)
                    continue;

                if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass
                    || targetBehaviour.EntityType == EntityTypes.Player)
                {
                    if ((GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile || enemyEntity.MobileEnemy.Team == MobileTeams.PlayerAlly) && targetBehaviour == player)
                        continue;

                    if (targetBehaviour == player && enemyEntity.Team == MobileTeams.PlayerAlly)
                        continue;
                    else if (DaggerfallUnity.Settings.EnemyInfighting && !enemyEntity.SuppressInfighting && targetEntity != null && !targetEntity.SuppressInfighting)
                    {
                        if (targetEntity.Team == enemyEntity.Team)
                            continue;
                    }
                    else
                    {
                        if (targetBehaviour != player && enemyEntity.MobileEnemy.Team != MobileTeams.PlayerAlly)
                            continue;
                    }

                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    if (senses.QuestBehaviour && !senses.QuestBehaviour.IsAttackableByAI && targetBehaviour != player)
                        continue;

                    EnemySenses targetSenses = null;
                    if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass)
                        targetSenses = targetBehaviour.GetComponent<EnemySenses>();

                    if (targetSenses && targetSenses.QuestBehaviour && !targetSenses.QuestBehaviour.IsAttackableByAI)
                        continue;

                    // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                    Vector3 toTarget = targetBehaviour.transform.position - senses.transform.position;

                    // I assign to vanilla DFU's EnemySenses directionToTarget field here using Reflection.
                    directionToTarget.SetValue(senses, toTarget.normalized);

                    // I assign to vanilla DFU's EnemySenses distanceToTarget field here using Reflection.
                    distanceToTarget.SetValue(senses, toTarget.magnitude);

                    // I reroute the method call to a method in this script.
                    bool see = CanSeeTarget(targetBehaviour);

                    if (targetSenses && !targetSenses.WouldBeSpawnedInClassic && !see)
                        continue;

                    float priority = 0;

                    if (targetSenses && targetSenses.Target == null)
                        priority += 5;

                    if (see)
                        priority += 10;

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    float distancePriority = 30 - senses.DistanceToTarget;
                    if (distancePriority < 0)
                        distancePriority = 0;

                    priority += distancePriority;
                    if (priority > highestPriority)
                    {
                        secondHighestPriority = highestPriority;
                        highestPriority = priority;
                        secondHighestPriorityTarget = highestPriorityTarget;
                        highestPriorityTarget = targetBehaviour;
                        sawSecondaryTarget = sawSelectedTarget;
                        sawSelectedTarget = see;

                        // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                        directionToTargetHolder = senses.DirectionToTarget;
                        distanceToTargetHolder = senses.DistanceToTarget;
                    }
                    else if (priority > secondHighestPriority)
                    {
                        sawSecondaryTarget = see;
                        secondHighestPriority = priority;
                        secondHighestPriorityTarget = targetBehaviour;
                    }
                }
            }

            // I assign to vanilla DFU's EnemySenses directionToTarget field here using Reflection.
            directionToTarget.SetValue(senses, directionToTargetHolder);

            // I assign to vanilla DFU's EnemySenses distanceToTarget field here using Reflection.
            distanceToTarget.SetValue(senses, distanceToTargetHolder);

            // I assign to vanilla DFU's EnemySenses targetInSight field here using Reflection.
            targetInSight.SetValue(senses, sawSelectedTarget);

            senses.Target = highestPriorityTarget;

            if (DaggerfallUnity.Settings.EnhancedCombatAI && secondHighestPriorityTarget)
            {
                // I call vanilla's field, property, or method here, using Reflection if necessary.
                senses.SecondaryTarget = secondHighestPriorityTarget;

                if (sawSecondaryTarget)

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    secondaryTargetPos = senses.SecondaryTarget.transform.position;
            }
        }

        bool CanSeeTarget(DaggerfallEntityBehaviour target)
        {
            bool seen = false;

            // I call vanilla's field, property, or method here, using Reflection if necessary.
            senses.LastKnownDoor = null;

            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
            if (senses.DistanceToTarget < senses.SightRadius + mobile.Enemy.SightModifier)
            {
                // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                float angle = Vector3.Angle(senses.DirectionToTarget, senses.transform.forward);

                // I call vanilla's field, property, or method here, using Reflection if necessary.
                if (angle < senses.FieldOfView * 0.5f)
                {
                    RaycastHit hit;
                    CharacterController controller = entityBehaviour.transform.GetComponent<CharacterController>();

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    Vector3 eyePos = senses.transform.position + controller.center;
                    eyePos.y += controller.height / 3;
                    controller = target.transform.GetComponent<CharacterController>();
                    Vector3 targetEyePos = target.transform.position + controller.center;
                    targetEyePos.y += controller.height / 3;
                    Vector3 eyeToTarget = targetEyePos - eyePos;
                    Vector3 eyeDirectionToTarget = eyeToTarget.normalized;
                    Ray ray = new Ray(eyePos, eyeDirectionToTarget);

                    // I call vanilla's field, property, or method here, using Reflection if necessary.
                    if (Physics.Raycast(ray, out hit, senses.SightRadius))
                    {
                        DaggerfallEntityBehaviour entity = hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                        if (entity == target)
                            seen = true;

                        DaggerfallActionDoor door = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                        if (door != null)
                        {
                            // I call vanilla's field, property, or method here, using Reflection if necessary.
                            senses.LastKnownDoor = door;

                            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
                            senses.DistanceToDoor = Vector3.Distance(senses.transform.position, senses.LastKnownDoor.transform.position);
                        }
                    }
                }
            }

            return seen;
        }

        bool CanHearTarget()
        {
            float hearingScale = 1f;
            RaycastHit hit;

            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
            Ray ray = new Ray(senses.transform.position, senses.DirectionToTarget);
            if (Physics.Raycast(ray, out hit))
            {
                if (GameObjectHelper.IsStaticGeometry(hit.transform.gameObject))
                    return false;
            }

            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
            return senses.DistanceToTarget < (senses.HearingRadius * hearingScale) + mobile.Enemy.HearingModifier;
        }

        /// <summary>
        /// This method is based on vanilla DFU's BlockedByIllusionEffect method from EnemySenses, modified for Bossfall.
        /// Comments indicate changes or additions I made.
        /// </summary>
        /// <returns>True if the target is undetectable.</returns>
        bool BossfallBlockedByIllusionEffect()
        {
            if (mobile.Enemy.SeesThroughInvisibility)
                return false;

            // If enemy is a level 20 Mage, Sorcerer, or Nightblade, they can see Invisible.
            if (CanDetectInvisible)
                return false;

            // I call vanilla's field, property, or method here, using Reflection if necessary.
            if (senses.Target.Entity.IsInvisible)
                return true;

            // I call vanilla fields, properties, or methods here, using Reflection if necessary.
            if (!senses.Target.Entity.IsBlending && !senses.Target.Entity.IsAShade)
                return false;

            int chance;

            // I call vanilla's field, property, or method here, using Reflection if necessary.
            if (senses.Target.Entity.IsBlending)
                chance = 8;
            else
                chance = 4;

            return Dice100.FailedRoll(chance);
        }

        /// <summary>
        /// This method is based on vanilla DFU's GetLanguageSkill method from EnemyEntity, modified for Bossfall.
        /// Comments indicate changes or additions I made.
        /// </summary>
        /// <param name="entityType">Is this a human enemy or monster?</param>
        /// <param name="careerIndex">EnemyEntity's CareerIndex.</param>
        /// <returns>Which language skill pacifies the given enemy.</returns>
        DFCareer.Skills GetBossfallLanguageSkill(EntityTypes entityType, int careerIndex)
        {
            if (entityType == EntityTypes.EnemyClass)
            {
                switch (careerIndex)
                {
                    // I added Sorcerers, Barbarians, and Rangers to the Streetwise list.
                    case (int)ClassCareers.Burglar:
                    case (int)ClassCareers.Rogue:
                    case (int)ClassCareers.Acrobat:
                    case (int)ClassCareers.Thief:
                    case (int)ClassCareers.Assassin:
                    case (int)ClassCareers.Nightblade:
                    case (int)ClassCareers.Sorcerer:
                    case (int)ClassCareers.Barbarian:
                    case (int)ClassCareers.Ranger:
                        return DFCareer.Skills.Streetwise;
                    default:
                        return DFCareer.Skills.Etiquette;
                }
            }

            switch (careerIndex)
            {
                case (int)MonsterCareers.Orc:
                case (int)MonsterCareers.OrcSergeant:
                case (int)MonsterCareers.OrcShaman:
                case (int)MonsterCareers.OrcWarlord:
                    return DFCareer.Skills.Orcish;

                case (int)MonsterCareers.Harpy:
                    return DFCareer.Skills.Harpy;

                case (int)MonsterCareers.Giant:
                case (int)MonsterCareers.Gargoyle:
                    return DFCareer.Skills.Giantish;

                case (int)MonsterCareers.Dragonling:
                case (int)MonsterCareers.Dragonling_Alternate:
                    return DFCareer.Skills.Dragonish;

                case (int)MonsterCareers.Nymph:
                case (int)MonsterCareers.Lamia:
                    return DFCareer.Skills.Nymph;

                case (int)MonsterCareers.FrostDaedra:
                case (int)MonsterCareers.FireDaedra:
                case (int)MonsterCareers.Daedroth:
                case (int)MonsterCareers.DaedraSeducer:
                case (int)MonsterCareers.DaedraLord:
                    return DFCareer.Skills.Daedric;

                case (int)MonsterCareers.Spriggan:
                    return DFCareer.Skills.Spriggan;

                case (int)MonsterCareers.Centaur:
                    return DFCareer.Skills.Centaurian;

                case (int)MonsterCareers.Imp:
                case (int)MonsterCareers.Dreugh:
                    return DFCareer.Skills.Impish;

                case (int)MonsterCareers.Vampire:
                case (int)MonsterCareers.VampireAncient:
                case (int)MonsterCareers.Lich:
                case (int)MonsterCareers.AncientLich:
                    return DFCareer.Skills.Etiquette;

                default:
                    return DFCareer.Skills.None;
            }
        }

        #endregion
    }
}
