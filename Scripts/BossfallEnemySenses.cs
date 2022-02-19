// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich
// 
// Notes: This script uses code from vanilla's EnemySenses script. // [OSORKON] comments precede changes or
//        additions I made - please verify original authorship before crediting. When in doubt compare with
//        vanilla DFU's source code.
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

        public float SightRadius = 4096 * MeshReader.GlobalScale;       // Range of enemy sight
        public float HearingRadius = 25f;                               // Range of enemy hearing
        public float FieldOfView = 180f;                                // Enemy field of view

        const float predictionInterval = 0.0625f;

        MobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        QuestResourceBehaviour questBehaviour;

        // [OSORKON] I added the bossfallMotor and senses fields.
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
        const float systemTimerUpdatesDivisor = .0549254f;  // Divisor for updates per second by the system timer at memory location 0x46C.

        const float classicSpawnDespawnExterior = 4096 * MeshReader.GlobalScale;
        float classicSpawnXZDist = 0f;
        float classicSpawnYDistUpper = 0f;
        float classicSpawnYDistLower = 0f;
        float classicDespawnXZDist = 0f;
        float classicDespawnYDist = 0f;

        /// <summary>
        /// [OSORKON] This bool is true if enemy is a level 20 Mage, Sorcerer, or Nightblade. 
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
            // [OSORKON] I added the bossfallMotor and senses lines.
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

            // 180 degrees is classic's value. 190 degrees is actual human FOV according to online sources.
            if (DaggerfallUnity.Settings.EnhancedCombatAI)
                FieldOfView = 190;

            ///<summary>
            /// [OSORKON] Checks if an enemy is a Level 20 Mage, Sorcerer, or Nightblade. Only needs to be checked once.
            /// </summary>
            if (enemyEntity.Level == 20 && (mobile.Enemy.ID == 128 || mobile.Enemy.ID == 131 || mobile.Enemy.ID == 133))
            {
                canDetectInvisible = true;
            }
        }

        void FixedUpdate()
        {
            // [OSORKON] I reversed the DisableAI check so player can swap between Bossfall & vanilla AI with the console.
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

            // Update whether enemy would be spawned or not in classic.
            // Only check if within the maximum possible distance (Just under 1094 classic units)
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
                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
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

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    float YDiffToPlayer = senses.transform.position.y - player.transform.position.y;
                    float YDiffToPlayerAbs = Mathf.Abs(YDiffToPlayer);
                    float distanceToPlayerXZ = Mathf.Sqrt(distanceToPlayer * distanceToPlayer - YDiffToPlayerAbs * YDiffToPlayerAbs);

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.WouldBeSpawnedInClassic = true;

                    if (distanceToPlayerXZ > upperXZ)

                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.WouldBeSpawnedInClassic = false;

                    if (playerInside)
                    {
                        if (lowerY == 0)
                        {
                            if (YDiffToPlayerAbs > upperY)

                                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                                senses.WouldBeSpawnedInClassic = false;
                        }
                        else if (YDiffToPlayer < lowerY || YDiffToPlayer > upperY)

                            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.WouldBeSpawnedInClassic = false;
                    }
                }
                else
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.WouldBeSpawnedInClassic = false;
            }

            if (GameManager.ClassicUpdate)
            {
                classicTargetUpdateTimer += Time.deltaTime / systemTimerUpdatesDivisor;

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null && senses.Target.Entity.CurrentHealth <= 0)
                {
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.Target = null;
                }

                // Non-hostile mode
                if (GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile)
                {
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.Target == player)
                        senses.Target = null;
                    if (senses.SecondaryTarget == player)
                        senses.SecondaryTarget = null;
                }

                // Reset these values if no target

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target == null)
                {
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.LastKnownTargetPos = ResetPlayerPos;
                    senses.PredictedTargetPos = ResetPlayerPos;
                    directionToTarget = ResetPlayerPos;
                    lastDistanceToTarget = 0;
                    senses.TargetRateOfApproach = 0;
                    distanceToTarget = 0;
                    targetSenses = null;

                    // If we have a valid secondary target that we acquired when we got the primary, switch to it.
                    // There will only be a secondary target if using enhanced combat AI.

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.SecondaryTarget != null && senses.SecondaryTarget.Entity.CurrentHealth > 0)
                    {
                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.Target = senses.SecondaryTarget;

                        // If the secondary target was actually seen, use the last place we saw it to begin pursuit.
                        if (sawSecondaryTarget)

                            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.LastKnownTargetPos = secondaryTargetPos;
                        awareOfTargetForLastPrediction = false;
                    }
                }

                // Compare change in target position to give AI some ability to read opponent's movements

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null && senses.Target == targetOnLastUpdate)
                {
                    if (DaggerfallUnity.Settings.EnhancedCombatAI)

                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.TargetRateOfApproach = lastDistanceToTarget - distanceToTarget;
                }
                else
                {
                    lastDistanceToTarget = 0;

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.TargetRateOfApproach = 0;
                }

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null)
                {
                    lastDistanceToTarget = distanceToTarget;

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    targetOnLastUpdate = senses.Target;
                }
            }

            if (player != null)
            {
                // Get distance to player

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                Vector3 toPlayer = player.transform.position - senses.transform.position;
                distanceToPlayer = toPlayer.magnitude;

                // If out of classic spawn range, still check for direct LOS to player so that enemies who see player will
                // try to attack.

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
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

                    // Is enemy in area around player or can see player?

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.WouldBeSpawnedInClassic || playerInSight)
                    {
                        GetTargets();

                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        if (senses.Target != null && senses.Target != player)
                            targetSenses = senses.Target.GetComponent<EnemySenses>();
                        else
                            targetSenses = null;
                    }

                    // Make targeted character also target this character if it doesn't have a target yet.

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.Target != null && targetSenses && targetSenses.Target == null)
                    {
                        targetSenses.Target = entityBehaviour;
                    }
                }

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target == null)
                {
                    targetInSight = false;

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = false;
                    return;
                }

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (!senses.WouldBeSpawnedInClassic && senses.Target == player)
                {
                    distanceToTarget = distanceToPlayer;
                    directionToTarget = toPlayer.normalized;
                    targetInSight = playerInSight;
                }
                else
                {
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    Vector3 toTarget = senses.Target.transform.position - senses.transform.position;
                    distanceToTarget = toTarget.magnitude;
                    directionToTarget = toTarget.normalized;

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla version.
                    // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    targetInSight = CanSeeTarget(senses.Target);
                }

                // Classic stealth mechanics would be interfered with by hearing, so only enable
                // hearing if the enemy has detected the target. If target is visible we can omit hearing.

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.DetectedTarget && !targetInSight)
                    targetInEarshot = CanHearTarget();
                else
                    targetInEarshot = false;

                // Note: In classic an enemy can continue to track the player as long as their
                // giveUpTimer is > 0. Since the timer is reset to 200 on every detection this
                // would make chameleon and shade essentially useless, since the enemy is sure
                // to detect the player during one of the many AI updates. Here, the enemy has to
                // successfully see through the illusion spell each classic update to continue
                // to know where the player is.
                if (GameManager.ClassicUpdate)
                {
                    blockedByIllusionEffect = BlockedByIllusionEffect();
                    if (lastHadLOSTimer > 0)
                        lastHadLOSTimer--;
                }

                if (!blockedByIllusionEffect && (targetInSight || targetInEarshot))
                {
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = true;
                    senses.LastKnownTargetPos = senses.Target.transform.position;
                    lastHadLOSTimer = 200f;
                }
                else if (!blockedByIllusionEffect && StealthCheck())
                {
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = true;

                    // Only get the target's location from the stealth check if we haven't had
                    // actual LOS for a while. This gives better pursuit behavior since enemies
                    // will go to the last spot they saw the player instead of walking into walls.
                    if (lastHadLOSTimer <= 0)

                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.LastKnownTargetPos = senses.Target.transform.position;
                }
                else
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.DetectedTarget = false;

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.OldLastKnownTargetPos == EnemySenses.ResetPlayerPos)
                    senses.OldLastKnownTargetPos = senses.LastKnownTargetPos;

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.PredictedTargetPos == EnemySenses.ResetPlayerPos || !DaggerfallUnity.Settings.EnhancedCombatAI)
                    senses.PredictedTargetPos = senses.LastKnownTargetPos;

                // Predict target's next position

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (targetPosPredict && senses.LastKnownTargetPos != EnemySenses.ResetPlayerPos)
                {
                    // Be sure to only take difference of movement if we've seen the target for two consecutive prediction updates
                    if (!blockedByIllusionEffect && targetInSight)
                    {
                        if (awareOfTargetForLastPrediction)

                            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.LastPositionDiff = senses.LastKnownTargetPos - senses.OldLastKnownTargetPos;

                        // Store current last known target position for next prediction update

                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
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
                        // [OSORKON] I changed the declaration of this variable.
                        float moveSpeed;

                        // [OSORKON] This predicts target positions based on which Enemy Move Speed setting is active.
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

                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        senses.PredictedTargetPos = PredictNextTargetPos(moveSpeed);
                    }
                }

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.DetectedTarget && !senses.HasEncounteredPlayer && senses.Target == player)
                {
                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.HasEncounteredPlayer = true;

                    // Check appropriate language skill to see if player can pacify enemy

                    // [OSORKON] I call EnemySenses' QuestBehaviour property rather than Bossfall's. I also check for the
                    // existence of "bossfallMotor" rather than vanilla's counterpart.
                    if (!senses.QuestBehaviour && entityBehaviour && bossfallMotor &&
                        (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass))
                    {
                        DFCareer.Skills languageSkill = enemyEntity.GetLanguageSkill();
                        if (languageSkill != DFCareer.Skills.None)
                        {
                            PlayerEntity player = GameManager.Instance.PlayerEntity;
                            if (FormulaHelper.CalculateEnemyPacification(player, languageSkill))
                            {
                                motor.IsHostile = false;
                                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("languagePacified").Replace("%e", enemyEntity.Name).Replace("%s", languageSkill.ToString()), 5);

                                // [OSORKON] I lowered TallySkill from 3 to 1. As far as I'm aware, no other skills tally more
                                // than 1 on success. Bossfall greatly increases speed at which language skills level anyway.
                                player.TallySkill(languageSkill, 1);    // BCHG: increased skill uses from 1 in classic on success to make raising language skills easier
                            }
                            else
                                // [OSORKON] I removed the conditions that excluded Etiquette and Streetwise from getting tallies
                                // if enemy pacification wasn't successful. Player will notice Etiquette and Streetwise increasing
                                // at a much faster rate. All language skills now function the same way. I like consistency.
                                player.TallySkill(languageSkill, 1);
                        }
                    }
                }
            }

            // If target is player and in sight then raise enemy alert on player
            // This can only be lowered again by killing an enemy or escaping for some amount of time
            // Any enemies actively targeting player will continue to raise alert state

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target == GameManager.Instance.PlayerEntityBehaviour && targetInSight)
                GameManager.Instance.PlayerEntity.SetEnemyAlert(true);
        }

        #region Public Methods

        public Vector3 PredictNextTargetPos(float interceptSpeed)
        {
            Vector3 assumedCurrentPosition;
            RaycastHit tempHit;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (predictedTargetPosWithoutLead == EnemySenses.ResetPlayerPos)
            {
                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                predictedTargetPosWithoutLead = senses.LastKnownTargetPos;
            }

            // If aware of target, if distance is too far or can see nothing is there, use last known position as assumed current position

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (targetInSight || targetInEarshot || (senses.PredictedTargetPos - senses.transform.position).magnitude > senses.SightRadius + mobile.Enemy.SightModifier
                || !Physics.Raycast(senses.transform.position, (predictedTargetPosWithoutLead - senses.transform.position).normalized, out tempHit, senses.SightRadius + mobile.Enemy.SightModifier))
            {
                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                assumedCurrentPosition = senses.LastKnownTargetPos;
            }
            // If not aware of target and predicted position may still be good, use predicted position
            else
            {
                assumedCurrentPosition = predictedTargetPosWithoutLead;
            }

            float divisor = predictionInterval;

            // Account for mid-interval call by DaggerfallMissile
            if (targetPosPredictTimer != 0)
            {
                divisor = targetPosPredictTimer;

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                senses.LastPositionDiff = senses.LastKnownTargetPos - senses.OldLastKnownTargetPos;
            }

            // Let's solve cone / line intersection (quadratic equation)

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
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
                    // find the minimal positive solution
                    float discSqrt = Mathf.Sqrt(disc) * Mathf.Sign(a);
                    t = (-b - discSqrt) / (2 * a);
                    if (t < 0)
                        t = (-b + discSqrt) / (2 * a);
                }
            }
            else
            {
                // degenerated cases
                if (Mathf.Abs(b) >= 1e-5)
                    t = -d.sqrMagnitude / b;
            }

            if (t >= 0)
            {
                prediction = assumedCurrentPosition + v * t;

                // Don't predict target will move through obstacles (prevent predicting movement through walls)
                RaycastHit hit;
                Ray ray = new Ray(assumedCurrentPosition, (prediction - assumedCurrentPosition).normalized);
                if (Physics.Raycast(ray, out hit, (prediction - assumedCurrentPosition).magnitude))
                    prediction = assumedCurrentPosition;
            }

            // Store prediction minus lead for next prediction update

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            predictedTargetPosWithoutLead = assumedCurrentPosition + senses.LastPositionDiff;

            return prediction;
        }

        public bool StealthCheck()
        {
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle && !motor.IsHostile)
                return false;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!senses.WouldBeSpawnedInClassic)
                return false;

            if (distanceToTarget > 1024 * MeshReader.GlobalScale)
                return false;

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (gameMinutes == timeOfLastStealthCheck)

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                return senses.DetectedTarget;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target == player)
            {
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
                if (playerMotor.IsMovingLessThanHalfSpeed)
                {
                    if ((gameMinutes & 1) == 1)

                        // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                        // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        return senses.DetectedTarget;
                }
                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
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

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            int stealthChance = FormulaHelper.CalculateStealthChance(distanceToTarget, senses.Target);

            return Dice100.FailedRoll(stealthChance);
        }

        public bool BlockedByIllusionEffect()
        {
            // In classic if the target is another AI character true is always returned.

            // Some enemy types can see through these effects.
            if (mobile.Enemy.SeesThroughInvisibility)
                return false;

            // [OSORKON] If enemy is a level 20 Mage, Sorcerer, or Nightblade, they can see Invisible.
            if (canDetectInvisible)
                return false;

            // If not one of the above enemy types, and target has invisibility,
            // detection is always blocked.

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target.Entity.IsInvisible)
                return true;

            // If target doesn't have any illusion effect, detection is not blocked.

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (!senses.Target.Entity.IsBlending && !senses.Target.Entity.IsAShade)
                return false;

            // Target has either chameleon or shade. Try to see through it.
            int chance;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target.Entity.IsBlending)
                chance = 8;
            else // is a shade
                chance = 4;

            return Dice100.FailedRoll(chance);
        }

        public bool TargetIsWithinYawAngle(float targetAngle, Vector3 targetPos)
        {
            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Vector3 toTarget = targetPos - senses.transform.position;
            toTarget.y = 0;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Vector3 enemyDirection2D = senses.transform.forward;
            enemyDirection2D.y = 0;

            return Vector3.Angle(toTarget, enemyDirection2D) < targetAngle;
        }

        public bool TargetHasBackTurned()
        {
            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Vector3 toTarget = senses.PredictedTargetPos - senses.transform.position;
            toTarget.y = 0;

            Vector3 targetDirection2D;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target == player)
            {
                Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                targetDirection2D = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
            }
            else
                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                targetDirection2D = -new Vector3(senses.Target.transform.forward.x, 0, senses.Target.transform.forward.z);

            return Vector3.Angle(toTarget, targetDirection2D) > 157.5f;
        }

        public bool TargetIsWithinPitchAngle(float targetAngle)
        {
            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Vector3 toTarget = senses.PredictedTargetPos - senses.transform.position;
            Vector3 directionToLastKnownTarget2D = toTarget.normalized;
            Plane verticalTransformToLastKnownPos = new Plane(senses.PredictedTargetPos, senses.transform.position, senses.transform.position + Vector3.up);
            // first project enemy direction to horizontal plane.

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Vector3 enemyDirection2D = Vector3.ProjectOnPlane(senses.transform.forward, Vector3.up);
            // next project enemy direction to vertical plane intersecting with last known position
            enemyDirection2D = Vector3.ProjectOnPlane(enemyDirection2D, verticalTransformToLastKnownPos.normal);

            float angle = Vector3.Angle(directionToLastKnownTarget2D, enemyDirection2D);

            return angle < targetAngle;
        }

        public bool TargetIsAbove()
        {
            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            return senses.PredictedTargetPos.y > senses.transform.position.y;
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

                // Can't target self
                if (targetBehaviour == entityBehaviour)
                    continue;

                // Evaluate potential targets
                if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass
                    || targetBehaviour.EntityType == EntityTypes.Player)
                {
                    // NoTarget mode
                    if ((GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile || enemyEntity.MobileEnemy.Team == MobileTeams.PlayerAlly) && targetBehaviour == player)
                        continue;

                    // Can't target ally
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

                    // Quest enemy AI only targets player by default unless explicitly marked as attackable by a mod/quest.

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.QuestBehaviour && !senses.QuestBehaviour.IsAttackableByAI && targetBehaviour != player)
                        continue;

                    EnemySenses targetSenses = null;
                    if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass)
                        targetSenses = targetBehaviour.GetComponent<EnemySenses>();

                    // For now, quest AI can't be targeted
                    if (targetSenses && targetSenses.QuestBehaviour && !targetSenses.QuestBehaviour.IsAttackableByAI)
                        continue;

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    Vector3 toTarget = targetBehaviour.transform.position - senses.transform.position;
                    directionToTarget = toTarget.normalized;
                    distanceToTarget = toTarget.magnitude;

                    bool see = CanSeeTarget(targetBehaviour);

                    // Is potential target neither visible nor in area around player? If so, reject as target.
                    if (targetSenses && !targetSenses.WouldBeSpawnedInClassic && !see)
                        continue;

                    float priority = 0;

                    // Add 5 priority if this potential target isn't already targeting someone
                    if (targetSenses && targetSenses.Target == null)
                        priority += 5;

                    if (see)
                        priority += 10;

                    // Add distance priority
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

            // Restore direction and distance values
            directionToTarget = directionToTargetHolder;
            distanceToTarget = distanceToTargetHolder;

            targetInSight = sawSelectedTarget;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            senses.Target = highestPriorityTarget;
            if (DaggerfallUnity.Settings.EnhancedCombatAI && secondHighestPriorityTarget)
            {
                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                senses.SecondaryTarget = secondHighestPriorityTarget;
                if (sawSecondaryTarget)

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    secondaryTargetPos = senses.SecondaryTarget.transform.position;
            }
        }

        bool CanSeeTarget(DaggerfallEntityBehaviour target)
        {
            bool seen = false;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            senses.LastKnownDoor = null;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (distanceToTarget < senses.SightRadius + mobile.Enemy.SightModifier)
            {
                // Check if target in field of view

                // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                float angle = Vector3.Angle(directionToTarget, senses.transform.forward);
                if (angle < senses.FieldOfView * 0.5f)
                {
                    // Check if line of sight to target
                    RaycastHit hit;

                    // Set origin of ray to approximate eye position
                    CharacterController controller = entityBehaviour.transform.GetComponent<CharacterController>();

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    Vector3 eyePos = senses.transform.position + controller.center;
                    eyePos.y += controller.height / 3;

                    // Set destination to the target's approximate eye position
                    controller = target.transform.GetComponent<CharacterController>();
                    Vector3 targetEyePos = target.transform.position + controller.center;
                    targetEyePos.y += controller.height / 3;

                    // Check if can see.
                    Vector3 eyeToTarget = targetEyePos - eyePos;
                    Vector3 eyeDirectionToTarget = eyeToTarget.normalized;
                    Ray ray = new Ray(eyePos, eyeDirectionToTarget);

                    // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                    // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (Physics.Raycast(ray, out hit, senses.SightRadius))
                    {
                        // Check if hit was target
                        DaggerfallEntityBehaviour entity = hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                        if (entity == target)
                            seen = true;

                        // Check if hit was an action door
                        DaggerfallActionDoor door = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                        if (door != null)
                        {
                            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
                            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                            senses.LastKnownDoor = door;

                            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
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

            // If something is between enemy and target then return false (was reduce hearingScale by half), to minimize
            // enemies walking against walls.
            // Hearing is not impeded by doors or other non-static objects
            RaycastHit hit;

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            Ray ray = new Ray(senses.transform.position, directionToTarget);
            if (Physics.Raycast(ray, out hit))
            {
                //DaggerfallEntityBehaviour entity = hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                if (GameObjectHelper.IsStaticGeometry(hit.transform.gameObject))
                    return false;
            }

            // TODO: Modify this by how much noise the target is making

            // [OSORKON] If the vanilla field or property can be set from outside the script, I call the vanilla
            // version. Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            return distanceToTarget < (senses.HearingRadius * hearingScale) + mobile.Enemy.HearingModifier;
        }

        #endregion
    }
}
