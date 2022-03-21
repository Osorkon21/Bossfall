// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich
// 
// Notes: This script uses code from vanilla DFU's EnemyAttack script. Comments indicate authorship,
//        please verify authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace BossfallMod.EnemyAI
{
    /// <summary>
    /// Bossfall enemy AI.
    /// </summary>
    [RequireComponent(typeof(BossfallEnemySenses))]
    [RequireComponent(typeof(EnemySenses))]
    public class BossfallEnemyAttack : MonoBehaviour
    {
        #region Fields

        // I reduced the minRangedDistance by 60 and doubled the maxRangedDistance.
        public const float minRangedDistance = 180 * MeshReader.GlobalScale;
        public const float maxRangedDistance = 4096 * MeshReader.GlobalScale;

        // I added the following line.
        EnemyAttack attack;

        EnemySenses senses;
        EnemySounds sounds;
        MobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;

        #endregion

        #region Unity

        void Start()
        {
            // I added the following line.
            attack = GetComponent<EnemyAttack>();

            senses = GetComponent<EnemySenses>();
            sounds = GetComponent<EnemySounds>();
            mobile = GetComponent<DaggerfallEnemy>().MobileUnit;
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
        }

        void FixedUpdate()
        {
            const int speedFloor = 8;

            // I reversed the DisableAI check so player can swap between Bossfall & vanilla AI with the console.
            if (!GameManager.Instance.DisableAI || entityBehaviour.Entity.IsParalyzed)
                return;

            if (mobile && mobile.IsPlayingOneShot() && mobile.OneShotPauseActionsWhilePlaying())
                return;

            // I changed the call to vanilla's MeleeTimer field.
            attack.MeleeTimer -= Time.deltaTime;

            // I changed the call to vanilla's MeleeTimer field.
            if (attack.MeleeTimer < 0)

                // I changed the call to vanilla's MeleeTimer field.
                attack.MeleeTimer = 0;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            int speed = entity.Stats.LiveSpeed;

            if (speed < speedFloor)
                speed = speedFloor;

            mobile.FrameSpeedDivisor = entity.Stats.PermanentSpeed / speed;

            // I changed the call to vanilla's MeleeTimer field.
            if (GameManager.ClassicUpdate && (DFRandom.rand() % speed >= (speed >> 3) + 6 && attack.MeleeTimer == 0))
            {
                // I reroute the method call to a method in this script.
                if (!MeleeAnimation())
                    return;

                // I reroute the method call to a method in this script.
                ResetMeleeTimer();
            }
        }

        #endregion

        #region Public Methods

        public void ResetMeleeTimer()
        {
            // I increased the range up and down by 500 and removed MeleeTimer variation by player level. Enemies
            // attack on average as fast as they do at a vanilla player level of 10, but with greater random variation. I
            // also changed the call to EnemyAttack's MeleeTimer instead of Bossfall's. This was done so in case other mods
            // are changing the MeleeTimer Bossfall will properly take these changes into account.
            attack.MeleeTimer = Random.Range(1000, 3500 + 1);

            // I changed the call to vanilla's MeleeTimer field.
            attack.MeleeTimer += 450 * ((int)GameManager.Instance.PlayerEntity.Reflexes - 2);

            // I changed the call to vanilla's MeleeTimer field.
            if (attack.MeleeTimer < 0)

                // I changed the call to vanilla's MeleeTimer field.
                attack.MeleeTimer = 0;

            // I changed the call to vanilla's MeleeTimer field.
            attack.MeleeTimer /= 980;
        }

        #endregion

        #region Private Methods

        private bool MeleeAnimation()
        {
            if (senses.TargetInSight && senses.TargetIsWithinYawAngle(22.5f, senses.LastKnownTargetPos))
            {
                // I changed the call to vanilla's MeleeDistance field.
                float distance = attack.MeleeDistance;

                if (!DaggerfallUnity.Settings.EnhancedCombatAI && senses.Target != GameManager.Instance.PlayerEntityBehaviour)

                    // I changed the call to vanilla's ClassicMeleeDistanceVsAI field.
                    distance = attack.ClassicMeleeDistanceVsAI;

                if (senses.DistanceToTarget > distance + senses.TargetRateOfApproach)
                    return false;

                mobile.ChangeEnemyState(MobileStates.PrimaryAttack);

                if (sounds)
                {
                    sounds.PlayAttackSound();
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}
