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

using DaggerfallWorkshop.Game.Items;

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
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return GetBossfallShieldArmorValue(this);
        }

        /// <summary>
        /// This method makes shields of high tier materials actually useful. Leather/Chain/Steel/Silver shield armor
        /// unchanged from vanilla. An Iron shield grants 1 less armor than vanilla, an Elven shield grants 1 more
        /// armor than vanilla, a Dwarven shield grants 2 more armor than vanilla, etc.
        /// </summary>
        /// <returns>Shield armor value, adjusted for shield material.</returns>
        public static int GetBossfallShieldArmorValue(DaggerfallUnityItem item)
        {
            int shieldMaterialModifier;

            switch (item.nativeMaterialValue)
            {
                case (int)ArmorMaterialTypes.Iron:
                    shieldMaterialModifier = -1;
                    break;
                case (int)ArmorMaterialTypes.Elven:
                    shieldMaterialModifier = 1;
                    break;
                case (int)ArmorMaterialTypes.Dwarven:
                    shieldMaterialModifier = 2;
                    break;
                case (int)ArmorMaterialTypes.Mithril:
                case (int)ArmorMaterialTypes.Adamantium:
                    shieldMaterialModifier = 3;
                    break;
                case (int)ArmorMaterialTypes.Ebony:
                    shieldMaterialModifier = 4;
                    break;
                case (int)ArmorMaterialTypes.Orcish:
                    shieldMaterialModifier = 5;
                    break;
                case (int)ArmorMaterialTypes.Daedric:
                    shieldMaterialModifier = 6;
                    break;

                default:
                    shieldMaterialModifier = 0;
                    break;
            }

            switch (item.TemplateIndex)
            {
                case (int)Armor.Buckler:
                    return 1 + shieldMaterialModifier;
                case (int)Armor.Round_Shield:
                    return 2 + shieldMaterialModifier;
                case (int)Armor.Kite_Shield:
                    return 3 + shieldMaterialModifier;
                case (int)Armor.Tower_Shield:
                    return 4 + shieldMaterialModifier;

                default:
                    return 0;
            }
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
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return Buckler.GetBossfallShieldArmorValue(this);
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
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return Buckler.GetBossfallShieldArmorValue(this);
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
        /// Reroutes armor calculation to my new method.
        /// </summary>
        /// <returns>Shield armor, adjusted for shield material.</returns>
        public override int GetShieldArmorValue()
        {
            return Buckler.GetBossfallShieldArmorValue(this);
        }
    }

    #endregion
}
