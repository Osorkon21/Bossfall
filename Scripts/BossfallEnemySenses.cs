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

using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace BossfallMod.EnemyAI
{
    /// <summary>
    /// Counterpart to vanilla's EnemySenses. Used for Bossfall enemy AI.
    /// </summary>
    public class BossfallEnemySenses : MonoBehaviour
    {
        public static readonly Vector3 ResetPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public float SightRadius = 4096 * MeshReader.GlobalScale;
        public float HearingRadius = 25f;
        public float FieldOfView = 180f;

        const float predictionInterval = 0.0625f;

        MobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        QuestResourceBehaviour questBehaviour;

        // I added the bossfallMotor and senses fields.
        BossfallEnemyMotor bossfallMotor;
        EnemySenses senses;

        EnemyMotor motor;
        EnemyEntity enemyEntity;
        bool targetInSight;
        bool playerInSight;
        bool targetInEarshot;
        Vector3 directionToTarget;
        float distanceToPlayer;
        float distanceToTarget;
        DaggerfallEntityBehaviour player;
        DaggerfallEntityBehaviour target;
        DaggerfallEntityBehaviour targetOnLastUpdate;
        DaggerfallEntityBehaviour secondaryTarget;
        bool sawSecondaryTarget;
        Vector3 secondaryTargetPos;
        EnemySenses targetSenses;
        float lastDistanceToTarget;
        float targetRateOfApproach;
        Vector3 lastKnownTargetPos = ResetPlayerPos;
        Vector3 oldLastKnownTargetPos = ResetPlayerPos;
        Vector3 predictedTargetPos = ResetPlayerPos;
        Vector3 predictedTargetPosWithoutLead = ResetPlayerPos;
        Vector3 lastPositionDiff;
        bool awareOfTargetForLastPrediction;
        DaggerfallActionDoor actionDoor;
        float distanceToActionDoor;
        bool hasEncounteredPlayer = false;
        bool wouldBeSpawnedInClassic = false;
        bool detectedTarget = false;
        uint timeOfLastStealthCheck = 0;
        bool blockedByIllusionEffect = false;
        float lastHadLOSTimer = 0f;

        float targetPosPredictTimer = 0f;
        bool targetPosPredict = false;

        float classicTargetUpdateTimer = 0f;
        const float systemTimerUpdatesDivisor = .0549254f;

        const float classicSpawnDespawnExterior = 4096 * MeshReader.GlobalScale;
        float classicSpawnXZDist = 0f;
        float classicSpawnYDistUpper = 0f;
        float classicSpawnYDistLower = 0f;
        float classicDespawnXZDist = 0f;
        float classicDespawnYDist = 0f;

        /// <summary>
        /// This bool is true if enemy is a level 20 Mage, Sorcerer, or Nightblade. 
        /// </summary>
        bool canDetectInvisible;

        public DaggerfallEntityBehaviour Target
        {
            get { return target; }
            set { target = value; }
        }

        public DaggerfallEntityBehaviour SecondaryTarget
        {
            get { return secondaryTarget; }
            set { secondaryTarget = value; }
        }

        public bool TargetInSight
        {
            get { return targetInSight; }
        }

        public bool DetectedTarget
        {
            get { return detectedTarget; }
            set { detectedTarget = value; }
        }

        public bool TargetInEarshot
        {
            get { return targetInEarshot; }
        }

        public Vector3 DirectionToTarget
        {
            get { return directionToTarget; }
        }

        public float DistanceToPlayer
        {
            get { return distanceToPlayer; }
        }

        public float DistanceToTarget
        {
            get { return distanceToTarget; }
        }

        public Vector3 LastKnownTargetPos
        {
            get { return lastKnownTargetPos; }
            set { lastKnownTargetPos = value; }
        }

        public Vector3 OldLastKnownTargetPos
        {
            get { return oldLastKnownTargetPos; }
            set { oldLastKnownTargetPos = value; }
        }

        public Vector3 LastPositionDiff
        {
            get { return lastPositionDiff; }
            set { lastPositionDiff = value; }
        }

        public Vector3 PredictedTargetPos
        {
            get { return predictedTargetPos; }
            set { predictedTargetPos = value; }
        }

        public DaggerfallActionDoor LastKnownDoor
        {
            get { return actionDoor; }
            set { actionDoor = value; }
        }

        public float DistanceToDoor
        {
            get { return distanceToActionDoor; }
            set { distanceToActionDoor = value; }
        }

        public bool HasEncounteredPlayer
        {
            get { return hasEncounteredPlayer; }
            set { hasEncounteredPlayer = value; }
        }

        public bool WouldBeSpawnedInClassic
        {
            get { return wouldBeSpawnedInClassic; }
            set { wouldBeSpawnedInClassic = value; }
        }

        public QuestResourceBehaviour QuestBehaviour
        {
            get { return questBehaviour; }
            set { questBehaviour = value; }
        }

        public float TargetRateOfApproach
        {
            get { return targetRateOfApproach; }
            set { targetRateOfApproach = value; }
        }

        void Start()
        {
            // I added the bossfallMotor and senses lines.
            bossfallMotor = GetComponent<BossfallEnemyMotor>();
            senses = GetComponent<EnemySenses>();

            mobile = GetComponent<DaggerfallEnemy>().MobileUnit;
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            enemyEntity = entityBehaviour.Entity as EnemyEntity;
            motor = GetComponent<EnemyMotor>();
            questBehaviour = GetComponent<QuestResourceBehaviour>();
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

            if (DaggerfallUnity.Settings.EnhancedCombatAI)
                FieldOfView = 190;

            // Checks if an enemy is a Level 20 Mage, Sorcerer, or Nightblade. Only needs to be checked once.
            if (enemyEntity.Level == 20 && (mobile.Enemy.ID == 128 || mobile.Enemy.ID == 131 || mobile.Enemy.ID == 133))
            {
                canDetectInvisible = true;
            }
        }

        void FixedUpdate()
        {
            // I reversed the DisableAI check so player can swap between Bossfall & vanilla AI with the console.
            if (!GameManager.Instance.DisableAI)
                return;

            targetPosPredictTimer += Time.deltaTime;
            if (targetPosPredictTimer >= predictionInterval)
            {
                targetPosPredictTimer = 0f;
                targetPosPredict = true;
            }
            else
                targetPosPredict = false;

            if (GameManager.ClassicUpdate)
            {
                if (distanceToPlayer < 1094 * MeshReader.GlobalScale)
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
                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
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

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    float YDiffToPlayer = senses.transform.position.y - player.transform.position.y;
                    float YDiffToPlayerAbs = Mathf.Abs(YDiffToPlayer);
                    float distanceToPlayerXZ = Mathf.Sqrt(distanceToPlayer * distanceToPlayer - YDiffToPlayerAbs * YDiffToPlayerAbs);

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.WouldBeSpawnedInClassic = true;

                    if (distanceToPlayerXZ > upperXZ)

                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.WouldBeSpawnedInClassic = false;

                    if (playerInside)
                    {
                        if (lowerY == 0)
                        {
                            if (YDiffToPlayerAbs > upperY)

                                // If the vanilla field or property can be set from outside the script, I call the vanilla
                                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                                senses.WouldBeSpawnedInClassic = false;
                        }
                        else if (YDiffToPlayer < lowerY || YDiffToPlayer > upperY)

                            // If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.WouldBeSpawnedInClassic = false;
                    }
                }
                else
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.WouldBeSpawnedInClassic = false;
            }

            if (GameManager.ClassicUpdate)
            {
                classicTargetUpdateTimer += Time.deltaTime / systemTimerUpdatesDivisor;

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null && senses.Target.Entity.CurrentHealth <= 0)
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.Target = null;
                }

                if (GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile)
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.Target == player)
                        senses.Target = null;
                    if (senses.SecondaryTarget == player)
                        senses.SecondaryTarget = null;
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target == null)
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.LastKnownTargetPos = ResetPlayerPos;
                    senses.PredictedTargetPos = ResetPlayerPos;
                    directionToTarget = ResetPlayerPos;
                    lastDistanceToTarget = 0;
                    senses.TargetRateOfApproach = 0;
                    distanceToTarget = 0;
                    targetSenses = null;

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.SecondaryTarget != null && senses.SecondaryTarget.Entity.CurrentHealth > 0)
                    {
                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.Target = senses.SecondaryTarget;

                        if (sawSecondaryTarget)

                            // If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.LastKnownTargetPos = secondaryTargetPos;
                        awareOfTargetForLastPrediction = false;
                    }
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null && senses.Target == targetOnLastUpdate)
                {
                    if (DaggerfallUnity.Settings.EnhancedCombatAI)

                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.TargetRateOfApproach = lastDistanceToTarget - distanceToTarget;
                }
                else
                {
                    lastDistanceToTarget = 0;

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.TargetRateOfApproach = 0;
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null)
                {
                    lastDistanceToTarget = distanceToTarget;

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    targetOnLastUpdate = senses.Target;
                }
            }

            if (player != null)
            {
                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                Vector3 toPlayer = player.transform.position - senses.transform.position;
                distanceToPlayer = toPlayer.magnitude;

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (!senses.WouldBeSpawnedInClassic)
                {
                    distanceToTarget = distanceToPlayer;
                    directionToTarget = toPlayer.normalized;
                    playerInSight = CanSeeTarget(player);
                }

                if (classicTargetUpdateTimer > 5)
                {
                    classicTargetUpdateTimer = 0f;

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.WouldBeSpawnedInClassic || playerInSight)
                    {
                        GetTargets();

                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        if (senses.Target != null && senses.Target != player)
                            targetSenses = senses.Target.GetComponent<EnemySenses>();
                        else
                            targetSenses = null;
                    }

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.Target != null && targetSenses && targetSenses.Target == null)
                    {
                        targetSenses.Target = entityBehaviour;
                    }
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target == null)
                {
                    targetInSight = false;

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = false;
                    return;
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (!senses.WouldBeSpawnedInClassic && senses.Target == player)
                {
                    distanceToTarget = distanceToPlayer;
                    directionToTarget = toPlayer.normalized;
                    targetInSight = playerInSight;
                }
                else
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    Vector3 toTarget = senses.Target.transform.position - senses.transform.position;
                    distanceToTarget = toTarget.magnitude;
                    directionToTarget = toTarget.normalized;

                    // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                    // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    targetInSight = CanSeeTarget(senses.Target);
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.DetectedTarget && !targetInSight)
                    targetInEarshot = CanHearTarget();
                else
                    targetInEarshot = false;

                if (GameManager.ClassicUpdate)
                {
                    blockedByIllusionEffect = BlockedByIllusionEffect();
                    if (lastHadLOSTimer > 0)
                        lastHadLOSTimer--;
                }

                if (!blockedByIllusionEffect && (targetInSight || targetInEarshot))
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = true;
                    senses.LastKnownTargetPos = senses.Target.transform.position;
                    lastHadLOSTimer = 200f;
                }
                else if (!blockedByIllusionEffect && StealthCheck())
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = true;

                    if (lastHadLOSTimer <= 0)

                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.LastKnownTargetPos = senses.Target.transform.position;
                }
                else
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = false;

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.OldLastKnownTargetPos == EnemySenses.ResetPlayerPos)
                    senses.OldLastKnownTargetPos = senses.LastKnownTargetPos;

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.PredictedTargetPos == EnemySenses.ResetPlayerPos || !DaggerfallUnity.Settings.EnhancedCombatAI)
                    senses.PredictedTargetPos = senses.LastKnownTargetPos;

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (targetPosPredict && senses.LastKnownTargetPos != EnemySenses.ResetPlayerPos)
                {
                    if (!blockedByIllusionEffect && targetInSight)
                    {
                        if (awareOfTargetForLastPrediction)

                            // If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.LastPositionDiff = senses.LastKnownTargetPos - senses.OldLastKnownTargetPos;

                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
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

                        // This predicts target positions based on which Enemy Move Speed setting is active.
                        if (Bossfall.EnemyMoveSpeed == 2)
                        {
                            moveSpeed = BossfallEnemyMotor.veryFastMoveSpeeds[mobile.Enemy.ID];
                        }
                        else if (Bossfall.EnemyMoveSpeed == 1)
                        {
                            moveSpeed = BossfallEnemyMotor.fastMoveSpeeds[mobile.Enemy.ID];
                        }
                        else
                        {
                            moveSpeed = (enemyEntity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) * MeshReader.GlobalScale;
                        }

                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.PredictedTargetPos = PredictNextTargetPos(moveSpeed);
                    }
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.DetectedTarget && !senses.HasEncounteredPlayer && senses.Target == player)
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.HasEncounteredPlayer = true;

                    // I call EnemySenses' QuestBehaviour property rather than Bossfall's. I also check for the
                    // existence of "bossfallMotor" rather than vanilla's counterpart.
                    if (!senses.QuestBehaviour && entityBehaviour && bossfallMotor &&
                        (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass))
                    {
                        // I rerouted this method call to a method I added to this script so I could change which
                        // language skills pacify certain human enemies.
                        DFCareer.Skills languageSkill = GetLanguageSkill(enemyEntity.EntityType, enemyEntity.CareerIndex);
                        if (languageSkill != DFCareer.Skills.None)
                        {
                            PlayerEntity player = GameManager.Instance.PlayerEntity;
                            if (FormulaHelper.CalculateEnemyPacification(player, languageSkill))
                            {
                                motor.IsHostile = false;
                                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("languagePacified").Replace("%e", enemyEntity.Name).Replace("%s", languageSkill.ToString()), 5);

                                // I lowered TallySkill from 3 to 1. As far as I'm aware, no other skills tally more
                                // than 1 on success. Bossfall greatly increases speed at which language skills level anyway.
                                player.TallySkill(languageSkill, 1);
                            }
                            else
                                // I removed the conditions that excluded Etiquette and Streetwise from getting tallies
                                // if enemy pacification wasn't successful. Player will notice Etiquette and Streetwise increasing
                                // at a much faster rate. All language skills now function the same way. I like consistency.
                                player.TallySkill(languageSkill, 1);
                        }
                    }
                }
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target == GameManager.Instance.PlayerEntityBehaviour && targetInSight)
                GameManager.Instance.PlayerEntity.SetEnemyAlert(true);
        }

        #region Public Methods

        public Vector3 PredictNextTargetPos(float interceptSpeed)
        {
            Vector3 assumedCurrentPosition;
            RaycastHit tempHit;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (predictedTargetPosWithoutLead == EnemySenses.ResetPlayerPos)
            {
                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                predictedTargetPosWithoutLead = senses.LastKnownTargetPos;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (targetInSight || targetInEarshot || (senses.PredictedTargetPos - senses.transform.position).magnitude > senses.SightRadius + mobile.Enemy.SightModifier
                || !Physics.Raycast(senses.transform.position, (predictedTargetPosWithoutLead - senses.transform.position).normalized, out tempHit, senses.SightRadius + mobile.Enemy.SightModifier))
            {
                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                assumedCurrentPosition = senses.LastKnownTargetPos;
            }
            else
            {
                assumedCurrentPosition = predictedTargetPosWithoutLead;
            }

            float divisor = predictionInterval;

            if (targetPosPredictTimer != 0)
            {
                divisor = targetPosPredictTimer;

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                senses.LastPositionDiff = senses.LastKnownTargetPos - senses.OldLastKnownTargetPos;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Vector3 d = assumedCurrentPosition - senses.transform.position;
            Vector3 v = senses.LastPositionDiff / divisor;
            float a = v.sqrMagnitude - interceptSpeed * interceptSpeed;
            float b = 2 * Vector3.Dot(d, v);
            float c = d.sqrMagnitude;

            Vector3 prediction = assumedCurrentPosition;

            float t = -1;
            if (Mathf.Abs(a) >= 1e-5)
            {
                float disc = b * b - 4 * a * c;
                if (disc >= 0)
                {
                    float discSqrt = Mathf.Sqrt(disc) * Mathf.Sign(a);
                    t = (-b - discSqrt) / (2 * a);
                    if (t < 0)
                        t = (-b + discSqrt) / (2 * a);
                }
            }
            else
            {
                if (Mathf.Abs(b) >= 1e-5)
                    t = -d.sqrMagnitude / b;
            }

            if (t >= 0)
            {
                prediction = assumedCurrentPosition + v * t;
                RaycastHit hit;
                Ray ray = new Ray(assumedCurrentPosition, (prediction - assumedCurrentPosition).normalized);
                if (Physics.Raycast(ray, out hit, (prediction - assumedCurrentPosition).magnitude))
                    prediction = assumedCurrentPosition;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            predictedTargetPosWithoutLead = assumedCurrentPosition + senses.LastPositionDiff;

            return prediction;
        }

        public bool StealthCheck()
        {
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle && !motor.IsHostile)
                return false;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!senses.WouldBeSpawnedInClassic)
                return false;

            if (distanceToTarget > 1024 * MeshReader.GlobalScale)
                return false;

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (gameMinutes == timeOfLastStealthCheck)

                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                return senses.DetectedTarget;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target == player)
            {
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
                if (playerMotor.IsMovingLessThanHalfSpeed)
                {
                    if ((gameMinutes & 1) == 1)

                        // If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        return senses.DetectedTarget;
                }
                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                else if (senses.HasEncounteredPlayer)
                    return true;

                PlayerEntity player = GameManager.Instance.PlayerEntity;
                if (player.TimeOfLastStealthCheck != gameMinutes)
                {
                    player.TallySkill(DFCareer.Skills.Stealth, 1);
                    player.TimeOfLastStealthCheck = gameMinutes;
                }
            }

            timeOfLastStealthCheck = gameMinutes;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            int stealthChance = FormulaHelper.CalculateStealthChance(distanceToTarget, senses.Target);

            return Dice100.FailedRoll(stealthChance);
        }

        public bool BlockedByIllusionEffect()
        {
            if (mobile.Enemy.SeesThroughInvisibility)
                return false;

            // If enemy is a level 20 Mage, Sorcerer, or Nightblade, they can see Invisible.
            if (canDetectInvisible)
                return false;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target.Entity.IsInvisible)
                return true;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!senses.Target.Entity.IsBlending && !senses.Target.Entity.IsAShade)
                return false;

            int chance;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target.Entity.IsBlending)
                chance = 8;
            else
                chance = 4;

            return Dice100.FailedRoll(chance);
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
            Vector3 directionToTargetHolder = directionToTarget;
            float distanceToTargetHolder = distanceToTarget;

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

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.QuestBehaviour && !senses.QuestBehaviour.IsAttackableByAI && targetBehaviour != player)
                        continue;

                    EnemySenses targetSenses = null;
                    if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass)
                        targetSenses = targetBehaviour.GetComponent<EnemySenses>();

                    if (targetSenses && targetSenses.QuestBehaviour && !targetSenses.QuestBehaviour.IsAttackableByAI)
                        continue;

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    Vector3 toTarget = targetBehaviour.transform.position - senses.transform.position;
                    directionToTarget = toTarget.normalized;
                    distanceToTarget = toTarget.magnitude;

                    bool see = CanSeeTarget(targetBehaviour);

                    if (targetSenses && !targetSenses.WouldBeSpawnedInClassic && !see)
                        continue;

                    float priority = 0;

                    if (targetSenses && targetSenses.Target == null)
                        priority += 5;

                    if (see)
                        priority += 10;

                    float distancePriority = 30 - distanceToTarget;
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
                        directionToTargetHolder = directionToTarget;
                        distanceToTargetHolder = distanceToTarget;
                    }
                    else if (priority > secondHighestPriority)
                    {
                        sawSecondaryTarget = see;
                        secondHighestPriority = priority;
                        secondHighestPriorityTarget = targetBehaviour;
                    }
                }
            }

            directionToTarget = directionToTargetHolder;
            distanceToTarget = distanceToTargetHolder;

            targetInSight = sawSelectedTarget;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            senses.Target = highestPriorityTarget;
            if (DaggerfallUnity.Settings.EnhancedCombatAI && secondHighestPriorityTarget)
            {
                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                senses.SecondaryTarget = secondHighestPriorityTarget;
                if (sawSecondaryTarget)

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    secondaryTargetPos = senses.SecondaryTarget.transform.position;
            }
        }

        bool CanSeeTarget(DaggerfallEntityBehaviour target)
        {
            bool seen = false;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            senses.LastKnownDoor = null;

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (distanceToTarget < senses.SightRadius + mobile.Enemy.SightModifier)
            {
                // If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                float angle = Vector3.Angle(directionToTarget, senses.transform.forward);
                if (angle < senses.FieldOfView * 0.5f)
                {
                    RaycastHit hit;
                    CharacterController controller = entityBehaviour.transform.GetComponent<CharacterController>();

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    Vector3 eyePos = senses.transform.position + controller.center;
                    eyePos.y += controller.height / 3;
                    controller = target.transform.GetComponent<CharacterController>();
                    Vector3 targetEyePos = target.transform.position + controller.center;
                    targetEyePos.y += controller.height / 3;
                    Vector3 eyeToTarget = targetEyePos - eyePos;
                    Vector3 eyeDirectionToTarget = eyeToTarget.normalized;
                    Ray ray = new Ray(eyePos, eyeDirectionToTarget);

                    // If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (Physics.Raycast(ray, out hit, senses.SightRadius))
                    {
                        DaggerfallEntityBehaviour entity = hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                        if (entity == target)
                            seen = true;

                        DaggerfallActionDoor door = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                        if (door != null)
                        {
                            // If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.LastKnownDoor = door;

                            // If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
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

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Ray ray = new Ray(senses.transform.position, directionToTarget);
            if (Physics.Raycast(ray, out hit))
            {
                if (GameObjectHelper.IsStaticGeometry(hit.transform.gameObject))
                    return false;
            }

            // If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            return distanceToTarget < (senses.HearingRadius * hearingScale) + mobile.Enemy.HearingModifier;
        }

        /// <summary>
        /// This method is from EnemyEntity, modified for Bossfall. I added two parameters and changed
        /// which language skill pacifies certain human enemies.
        /// </summary>
        /// <param name="entityType">Is this a human enemy or monster?</param>
        /// <param name="careerIndex">EnemyEntity's CareerIndex.</param>
        /// <returns>Which language skill pacifies the given enemy.</returns>
        DFCareer.Skills GetLanguageSkill(EntityTypes entityType, int careerIndex)
        {
            if (entityType == EntityTypes.EnemyClass)
            {
                switch (careerIndex)
                {
                    // I added Sorcerers, Barbarians, and Rangers to the Streetwise list. Now class
                    // enemies are half pacified by Streetwise and the other half Etiquette. I like balance.
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
