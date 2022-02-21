// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich
// 
// Notes: This script uses code from vanilla's EnemyAttack and DaggerfallMissile scripts. Comments indicate authorship,
//        please verify authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility;
using UnityEngine;

namespace BossfallMod.EnemyAI
{
    /// <summary>
    /// Counterpart to vanilla's EnemyAttack. Used for Bossfall enemy AI.
    /// </summary>
    [RequireComponent(typeof(BossfallEnemySenses))]
    [RequireComponent(typeof(EnemySenses))]
    public class BossfallEnemyAttack : MonoBehaviour
    {
        // I reduced the minRangedDistance by 60 and doubled the maxRangedDistance. I got annoyed at
        // how easy it was to kite enemies at long range without them ever shooting back. Enemies will also fire
        // arrows and spells at the normal Enhanced AI "stopDistance" instead of standing there staring at you.
        public const float minRangedDistance = 180 * MeshReader.GlobalScale;
        public const float maxRangedDistance = 4096 * MeshReader.GlobalScale;
        public float MeleeDistance = 2.25f;
        public float ClassicMeleeDistanceVsAI = 1.5f;
        public float MeleeTimer = 0;
        public DaggerfallMissile ArrowMissilePrefab;

        // I added the bossfallSenses, bossfallMotor, and attack fields.
        BossfallEnemyMotor bossfallMotor;
        BossfallEnemySenses bossfallSenses;
        EnemyAttack attack;

        EnemySenses senses;
        EnemySounds sounds;
        MobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        int damage = 0;

        void Start()
        {
            // I added the bossfallMotor, bossfallSenses, and attack lines.
            bossfallMotor = GetComponent<BossfallEnemyMotor>();
            bossfallSenses = GetComponent<BossfallEnemySenses>();
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

            // I changed the call to EnemyAttack's MeleeTimer instead of Bossfall's.
            attack.MeleeTimer -= Time.deltaTime;

            // I changed the call to EnemyAttack's MeleeTimer instead of Bossfall's.
            if (attack.MeleeTimer < 0)
                attack.MeleeTimer = 0;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            int speed = entity.Stats.LiveSpeed;

            if (speed < speedFloor)
                speed = speedFloor;

            mobile.FrameSpeedDivisor = entity.Stats.PermanentSpeed / speed;

            // I changed the call to EnemyAttack's MeleeTimer instead of Bossfall's.
            if (GameManager.ClassicUpdate && (DFRandom.rand() % speed >= (speed >> 3) + 6 && attack.MeleeTimer == 0))
            {
                if (!MeleeAnimation())
                    return;

                ResetMeleeTimer();
            }
        }

        void Update()
        {
            // I added the !DisableAI check. I don't want Bossfall AI to run if vanilla AI is on.
            if (!GameManager.Instance.DisableAI || entityBehaviour.Entity.IsParalyzed)
                return;

            if (mobile.DoMeleeDamage)
            {
                MeleeDamage();
                mobile.DoMeleeDamage = false;
            }
            else if (mobile.ShootArrow)
            {
                ShootBow();
                mobile.ShootArrow = false;
                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();

                if (dfAudioSource)
                    dfAudioSource.PlayOneShot((int)SoundClips.ArrowShoot, 1, 1.0f);
            }
        }

        public void ResetMeleeTimer()
        {
            // I increased the range up and down by 500 and removed MeleeTimer variation by player level. Enemies
            // attack on average as fast as they do at a vanilla player level of 10, but with greater random variation. I
            // also changed the call to EnemyAttack's MeleeTimer instead of Bossfall's. This was done so in case other mods
            // are changing the MeleeTimer Bossfall will properly take these changes into account.
            attack.MeleeTimer = Random.Range(1000, 3500 + 1);

            // I changed the call to EnemyAttack's MeleeTimer instead of Bossfall's.
            attack.MeleeTimer += 450 * ((int)GameManager.Instance.PlayerEntity.Reflexes - 2);

            // I changed the calls to EnemyAttack's MeleeTimer instead of Bossfall's.
            if (attack.MeleeTimer < 0)
                attack.MeleeTimer = 0;

            // I changed the call to EnemyAttack's MeleeTimer instead of Bossfall's.
            attack.MeleeTimer /= 980;
        }

        #region Private Methods

        private bool MeleeAnimation()
        {
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (bossfallSenses.TargetInSight && senses.TargetIsWithinYawAngle(22.5f, senses.LastKnownTargetPos))
            {
                // I changed the call to EnemyAttack's MeleeDistance instead of Bossfall's.
                float distance = attack.MeleeDistance;

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (!DaggerfallUnity.Settings.EnhancedCombatAI && senses.Target != GameManager.Instance.PlayerEntityBehaviour)
                    distance = attack.ClassicMeleeDistanceVsAI;

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (bossfallSenses.DistanceToTarget > distance + senses.TargetRateOfApproach)
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

        private void MeleeDamage()
        {
            if (entityBehaviour)
            {
                EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                EnemyEntity targetEntity = null;

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null && senses.Target != GameManager.Instance.PlayerEntityBehaviour)
                    targetEntity = senses.Target.Entity as EnemyEntity;

                // I removed the "Item" prefixes in the below lines.
                DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(EquipSlots.RightHand);
                if (weapon != null && targetEntity != null && targetEntity.MobileEnemy.MinMetalToHit > (WeaponMaterialTypes)weapon.NativeMaterialValue)
                    weapon = null;

                damage = 0;

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (senses.Target != null && bossfallSenses.TargetInSight && (bossfallSenses.DistanceToTarget <= 0.25f
                    || (bossfallSenses.DistanceToTarget <= attack.MeleeDistance && senses.TargetIsWithinYawAngle(35.156f, senses.Target.transform.position))))
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                    // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    if (senses.Target == GameManager.Instance.PlayerEntityBehaviour)
                        damage = ApplyDamageToPlayer(weapon);
                    else
                        // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                        // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                        damage = ApplyDamageToNonPlayer(weapon, senses.transform.forward);
                }
                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                else if (bossfallMotor.Bashing && senses.LastKnownDoor != null && senses.DistanceToDoor <= attack.MeleeDistance && !senses.LastKnownDoor.IsOpen)
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                    // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    senses.LastKnownDoor.AttemptBash(false);
                }
                else
                {
                    sounds.PlayMissSound(weapon);
                }
                
                if (DaggerfallUnity.Settings.CombatVoices && entity.EntityType == EntityTypes.EnemyClass && Dice100.SuccessRoll(20))
                {
                    Genders gender;
                    if (mobile.Enemy.Gender == MobileGender.Male || entity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                        gender = Genders.Male;
                    else
                        gender = Genders.Female;
                
                    sounds.PlayCombatVoice(gender, true);
                }
            }
        }

        private void ShootBow()
        {
            if (entityBehaviour)
            {
                DaggerfallMissile missile = Instantiate(ArrowMissilePrefab);
                if (missile)
                {
                    missile.Caster = entityBehaviour;
                    missile.TargetType = TargetTypes.SingleTargetAtRange;
                    missile.ElementType = ElementTypes.None;
                    missile.IsArrow = true;

                    // EnhancedCombatAI aiming doesn't work with Bossfall AI. This fixes arrow aiming.
                    // I copied code from DaggerfallMissile and modified it to work with Bossfall.
                    if (DaggerfallUnity.Settings.EnhancedCombatAI)
                    {
                        Vector3 predictedPosition = bossfallSenses.PredictNextTargetPos(25.0f);

                        if (predictedPosition == EnemySenses.ResetPlayerPos)
                            missile.CustomAimDirection = entityBehaviour.transform.forward;
                        else
                            missile.CustomAimDirection = (predictedPosition - entityBehaviour.transform.position).normalized;
                    }
                }
            }
        }

        // I removed the "Item" prefix in the line below.
        private int ApplyDamageToPlayer(DaggerfallUnityItem weapon)
        {
            const int doYouSurrenderToGuardsTextID = 15;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            damage = FormulaHelper.CalculateAttackDamage(entity, playerEntity, false, 0, weapon);

            if (entity.IsMagicallyConcealedNormalPower && damage > 0)
                EntityEffectManager.BreakNormalPowerConcealmentEffects(entityBehaviour);

            playerEntity.TallySkill(DFCareer.Skills.Dodging, 1);

            if (damage > 0 && weapon != null && weapon.IsEnchanted)
            {
                EntityEffectManager effectManager = GetComponent<EntityEffectManager>();
                if (effectManager)
                    damage = effectManager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Strikes, weapon, entity.Items, playerEntity.EntityBehaviour, damage);
            }

            if (damage > 0)
            {
                if (entity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                {
                    if (!playerEntity.HaveShownSurrenderToGuardsDialogue && playerEntity.CrimeCommitted != PlayerEntity.Crimes.None)
                    {
                        playerEntity.LowerRepForCrime();

                        DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
                        messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(doYouSurrenderToGuardsTextID));
                        messageBox.ParentPanel.BackgroundColor = Color.clear;
                        messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                        messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                        messageBox.OnButtonClick += SurrenderToGuardsDialogue_OnButtonClick;
                        messageBox.Show();

                        playerEntity.HaveShownSurrenderToGuardsDialogue = true;
                    }
                    else if (playerEntity.CurrentHealth > damage || !playerEntity.SurrenderToCityGuards(false))
                        SendDamageToPlayer();
                }
                else
                    SendDamageToPlayer();
            }
            else
                sounds.PlayMissSound(weapon);

            return damage;
        }

        // I removed the "Item" prefix in the line below.
        private int ApplyDamageToNonPlayer(DaggerfallUnityItem weapon, Vector3 direction, bool bowAttack = false)
        {
            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            if (senses.Target == null)
                return 0;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            EnemyEntity targetEntity = senses.Target.Entity as EnemyEntity;
            EnemySounds targetSounds = senses.Target.GetComponent<EnemySounds>();

            // If the vanilla field or property can be set from outside the script, I call the vanilla version.
            // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
            EnemyMotor targetMotor = senses.Target.transform.GetComponent<EnemyMotor>();
            damage = FormulaHelper.CalculateAttackDamage(entity, targetEntity, false, 0, weapon);

            if (entity.IsMagicallyConcealedNormalPower && damage > 0)
                EntityEffectManager.BreakNormalPowerConcealmentEffects(entityBehaviour);

            if (damage > 0)
            {
                targetSounds.PlayHitSound(weapon);

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                EnemyBlood blood = senses.Target.transform.GetComponent<EnemyBlood>();
                CharacterController targetController = senses.Target.transform.GetComponent<CharacterController>();
                Vector3 bloodPos = senses.Target.transform.position + targetController.center;
                bloodPos.y += targetController.height / 8;

                if (blood)
                {
                    blood.ShowBloodSplash(targetEntity.MobileEnemy.BloodIndex, bloodPos);
                }

                if (targetMotor && (targetMotor.KnockbackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10))
                        && (entityBehaviour.EntityType == EntityTypes.EnemyClass || targetEntity.MobileEnemy.Weight > 0)))
                {
                    // I changed enemyWeight to 100,000 to reduce knockback stunlocks.
                    float enemyWeight = 100000;
                    float tenTimesDamage = damage * 10;
                    float twoTimesDamage = damage * 2;

                    float knockBackAmount = ((tenTimesDamage - enemyWeight) * 256) / (enemyWeight + tenTimesDamage) * twoTimesDamage;
                    float KnockbackSpeed = (tenTimesDamage / enemyWeight) * (twoTimesDamage - (knockBackAmount / 256));
                    KnockbackSpeed /= (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10);

                    // I changed KnockbackSpeed from 15 to 12 here. This lowers the minimum speed at which enemies
                    // fly backwards. I changed this very early on when I didn't really know what I was doing, and I am not
                    // sure even now if this change was necessary. Regardless, knockbacks are now at a level I'm comfortable
                    // with, so there's no need for me to change this further.
                    if (KnockbackSpeed < (12 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                        KnockbackSpeed = (12 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                    targetMotor.KnockbackSpeed = KnockbackSpeed;
                    targetMotor.KnockbackDirection = direction;
                }

                // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                if (DaggerfallUnity.Settings.CombatVoices && senses.Target.EntityType == EntityTypes.EnemyClass && Dice100.SuccessRoll(40))
                {
                    // If the vanilla field or property can be set from outside the script, I call the vanilla version.
                    // Otherwise, I redirect calls to the field or property's Bossfall counterpart.
                    var targetMobileUnit = senses.Target.GetComponentInChildren<MobileUnit>();
                    Genders gender;
                    if (targetMobileUnit.Enemy.Gender == MobileGender.Male || targetEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                        gender = Genders.Male;
                    else
                        gender = Genders.Female;

                    targetSounds.PlayCombatVoice(gender, false, damage >= targetEntity.MaxHealth / 4);
                }
            }
            else
            {
                WeaponTypes weaponType = WeaponTypes.Melee;
                if (weapon != null)
                    weaponType = DaggerfallUnity.Instance.ItemHelper.ConvertItemToAPIWeaponType(weapon);

                if ((!bowAttack && !targetEntity.MobileEnemy.ParrySounds) || weaponType == WeaponTypes.Melee)
                    sounds.PlayMissSound(weapon);
                else if (targetEntity.MobileEnemy.ParrySounds)
                    targetSounds.PlayParrySound();
            }

            if (weapon != null && weapon.IsEnchanted)
            {
                EntityEffectManager effectManager = GetComponent<EntityEffectManager>();
                if (effectManager)
                    damage = effectManager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Strikes, weapon, entity.Items, targetEntity.EntityBehaviour, damage);
            }

            targetEntity.DecreaseHealth(damage);

            if (targetMotor)
            {
                targetMotor.MakeEnemyHostileToAttacker(entityBehaviour);
            }

            return damage;
        }

        private void SurrenderToGuardsDialogue_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                GameManager.Instance.PlayerEntity.SurrenderToCityGuards(true);
            else
                SendDamageToPlayer();
        }

        private void SendDamageToPlayer()
        {
            GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", damage);

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;

            // I removed the "Item" prefixes in the below lines.
            DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(EquipSlots.RightHand);
            if (weapon == null)
                weapon = entity.ItemEquipTable.GetItem(EquipSlots.LeftHand);
            if (weapon != null)
                GameManager.Instance.PlayerObject.SendMessage("PlayWeaponHitSound");
            else
                GameManager.Instance.PlayerObject.SendMessage("PlayWeaponlessHitSound");
        }

        #endregion
    }
}
