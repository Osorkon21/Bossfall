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

    /// <summary>
    /// There may be an easier way to do this, but I couldn't figure out how to vary vanilla shield armor values based
    /// on shield material, so this creates a Buckler that acts exactly like vanilla's except its armor value varies
    /// based on shield's material. I substitute this new Buckler wherever a vanilla loot function would normally
    /// generate a vanilla Buckler.
    /// </summary>
    public class Buckler : DaggerfallUnityItem
    {
        /// <summary>
        /// I tried registering this Buckler type as a custom item but I ran into a serious problem. Vanilla has shield
        /// detection hard coded into read-only properties and non-virtual methods so I couldn't make this new Buckler
        /// act like a vanilla shield. As a rather desperate workaround, I then tried running the Buckler through a
        /// base constructor that resets its item template to that of a vanilla Buckler. I was very surprised when it worked.
        /// </summary>
        public Buckler() : base(ItemGroups.Armor, (int)Armor.Buckler - 102)
        { }

        /// <summary>
        /// Alternate constructor, used to restore this shield on game load.
        /// </summary>
        /// <param name="itemData">Item save data.</param>
        public Buckler(ItemData_v1 itemData) : base(itemData)
        { }

        /// <summary>
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion

    #region Round Shield

    /// <summary>
    /// There may be an easier way to do this, but I couldn't figure out how to vary vanilla shield armor values based
    /// on shield material, so this creates a Round Shield that acts exactly like vanilla's except its armor value varies
    /// based on shield's material. I substitute this new shield wherever a vanilla loot function would normally
    /// generate a vanilla Round Shield.
    /// </summary>
    public class RoundShield : DaggerfallUnityItem
    {
        /// <summary>
        /// Vanilla has shield detection hard coded into read-only properties and non-virtual methods, so as a workaround
        /// I run this shield through a base constructor that resets its item template to the vanilla shield value.
        /// </summary>
        public RoundShield() : base(ItemGroups.Armor, (int)Armor.Round_Shield - 102)
        { }

        /// <summary>
        /// Alternate constructor, used to restore this shield on game load.
        /// </summary>
        /// <param name="itemData">Item save data.</param>
        public RoundShield(ItemData_v1 itemData) : base(itemData)
        { }

        /// <summary>
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion

    #region Kite Shield

    /// <summary>
    /// There may be an easier way to do this, but I couldn't figure out how to vary vanilla shield armor values based
    /// on shield material, so this creates a Kite Shield that acts exactly like vanilla's except its armor value varies
    /// based on shield's material. I substitute this new shield wherever a vanilla loot function would normally
    /// generate a vanilla Kite Shield.
    /// </summary>
    public class KiteShield : DaggerfallUnityItem
    {
        /// <summary>
        /// Vanilla has shield detection hard coded into read-only properties and non-virtual methods, so as a workaround
        /// I run this shield through a base constructor that resets its item template to the vanilla shield value.
        /// </summary>
        public KiteShield() : base(ItemGroups.Armor, (int)Armor.Kite_Shield - 102)
        { }

        /// <summary>
        /// Alternate constructor, used to restore this shield on game load.
        /// </summary>
        /// <param name="itemData">Item save data.</param>
        public KiteShield(ItemData_v1 itemData) : base(itemData)
        { }

        /// <summary>
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion

    #region Tower Shield

    /// <summary>
    /// There may be an easier way to do this, but I couldn't figure out how to vary vanilla shield armor values based
    /// on shield material, so this creates a Tower Shield that acts exactly like vanilla's except its armor value varies
    /// based on shield's material. I substitute this new shield wherever a vanilla loot function would normally
    /// generate a vanilla Tower Shield.
    /// </summary>
    public class TowerShield : DaggerfallUnityItem
    {
        /// <summary>
        /// Vanilla has shield detection hard coded into read-only properties and non-virtual methods, so as a workaround
        /// I run this shield through a base constructor that resets its item template to the vanilla shield value.
        /// </summary>
        public TowerShield() : base(ItemGroups.Armor, (int)Armor.Tower_Shield - 102)
        { }

        /// <summary>
        /// Alternate constructor, used to restore this shield on game load.
        /// </summary>
        /// <param name="itemData">Item save data.</param>
        public TowerShield(ItemData_v1 itemData) : base(itemData)
        { }

        /// <summary>
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return BossfallOverrides.Instance.BossfallShieldArmorValue(this);
        }
    }

    #endregion
}
