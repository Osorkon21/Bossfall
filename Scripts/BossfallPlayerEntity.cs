// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall
// Original Author: Osorkon
// Contributors:    
// 
// Notes: 
//

using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace BossfallMod.Player
{
    /// <summary>
    /// Custom player-specific effects.
    /// </summary>
    public class BossfallPlayerEntity : MonoBehaviour
    {
        #region Fields

        PlayerEntity player;
        PlayerMotor motor;

        int vanillaRunningTally;
        int bossfallRunningTally;

        #endregion

        #region Unity

        void Start()
        {
            player = GameManager.Instance.PlayerEntity;
            motor = GameManager.Instance.PlayerMotor;
        }

        void FixedUpdate()
        {
            if (GameManager.ClassicUpdate && motor && motor.IsRunning && !motor.IsRiding && player != null)
            {
                if (++vanillaRunningTally == 4)
                {
                    player.TallySkill(DFCareer.Skills.Running, -1);
                    vanillaRunningTally = 0;
                }

                // Extremely Hard skill advancement difficulty. Running will level up 5 times slower than vanilla. 
                if (Bossfall.Instance.SkillAdvancementDifficulty == 2)
                {
                    if (++bossfallRunningTally == 30)
                    {
                        player.TallySkill(DFCareer.Skills.Running, 1);
                        bossfallRunningTally = 0;
                    }
                }
                // Hard skill advancement difficulty. Running will level up 50% more slowly than vanilla.
                else if (Bossfall.Instance.SkillAdvancementDifficulty == 1)
                {
                    if (++bossfallRunningTally == 9)
                    {
                        player.TallySkill(DFCareer.Skills.Running, 1);
                        bossfallRunningTally = 0;
                    }
                }
                // Vanilla skill advancement difficulty. Running levels up at the vanilla rate despite the 50% increase
                // to the running tally - Running's Skill Advancement Multiplier has been decreased from 50 to 34 (a 33%
                // decrease). This was done to ensure Running correctly levels up at very high player levels, very high
                // Running skill levels, a CareerAdvancementMultiplier of 3.0, and VeryLow Reflexes. In vanilla Running would
                // never increase from 99 to 100 if these (very improbable, but possible) conditions were met.
                else if (++bossfallRunningTally == 6)
                {
                    player.TallySkill(DFCareer.Skills.Running, 1);
                    bossfallRunningTally = 0;
                }
            }
        }

        #endregion
    }
}
