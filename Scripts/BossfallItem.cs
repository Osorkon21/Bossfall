// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall
// Original Author: Osorkon
// Contributors:
//
// 
// Notes: 
//

using BossfallMod.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;

namespace BossfallMod.Items
{
    #region Buckler

    public class Buckler : DaggerfallUnityItem
    {
        /// <summary>
        /// This creates an item that acts like a vanilla Buckler except its armor varies depending on material type.
        /// </summary>
        public Buckler() : base(ItemGroups.Armor, (int)Armor.Buckler - 102) { }

        public Buckler(ItemData_v1 itemData) : base(itemData) { }

        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion

    #region Round Shield

    public class RoundShield : DaggerfallUnityItem
    {
        /// <summary>
        /// This creates an item that acts like a vanilla Round Shield except its armor varies depending on material type.
        /// </summary>
        public RoundShield() : base(ItemGroups.Armor, (int)Armor.Round_Shield - 102) { }

        public RoundShield(ItemData_v1 itemData) : base(itemData) { }

        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion

    #region Kite Shield

    public class KiteShield : DaggerfallUnityItem
    {
        /// <summary>
        /// This creates an item that acts like a vanilla Kite Shield except its armor varies depending on material type.
        /// </summary>
        public KiteShield() : base(ItemGroups.Armor, (int)Armor.Kite_Shield - 102) { }

        public KiteShield(ItemData_v1 itemData) : base(itemData) { }

        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion

    #region Tower Shield

    public class TowerShield : DaggerfallUnityItem
    {
        /// <summary>
        /// This creates an item that acts like a vanilla Tower Shield except its armor varies depending on material type.
        /// </summary>
        public TowerShield() : base(ItemGroups.Armor, (int)Armor.Tower_Shield - 102) { }

        public TowerShield(ItemData_v1 itemData) : base(itemData) { }

        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion
}
