// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall
// Original Author: Osorkon
// Contributors:
//
// 
// Notes: I used Hazelnut & Ralzar's "Roleplay and Realism: Items" item generation scripts as templates for this one. Comments
//        indicate specific methods I used.
//

using DaggerfallConnect.Save;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;

namespace BossfallMod.Items
{
    // DWI
    // Clear script using standard citing methods once completed

    // DWI
    // Add contributors using standard procedures once script completed

    /// <summary>
    /// There may be an easier way to do this, but I couldn't figure out how to vary vanilla shield armor values based
    /// on shield material, so this creates a Buckler that acts exactly like vanilla's except its armor value varies
    /// based on shield's material. I substitute this new Buckler for whenever a vanilla loot function would normally
    /// generate a vanilla Buckler. I used the item creation/registration scripts in Hazelnut & Ralzar's excellent
    /// "Roleplay and Realism: Items" mod as templates for this script.
    /// </summary>
    public class Buckler : DaggerfallUnityItem
    {
        public Buckler() : base(ItemGroups.Armor, (int)Armor.Buckler - 102)
        {
        }

        /// <summary>
        /// Use standard vanilla shield equip sounds.
        /// </summary>
        /// <returns>Sound Clip to play on equip.</returns>
        public override SoundClips GetEquipSound()
        {
            return SoundClips.EquipPlate;
        }

        /// <summary>
        /// Not sure if this override is necessary, but I included it anyway.
        /// </summary>
        /// <returns>Redirects to my shield armor method.</returns>
        public override int GetMaterialArmorValue()
        {
            return GetShieldArmorValue();
        }

        /// <summary>
        /// I added this override method to make shields of high tier materials actually useful.
        /// Leather/Chain/Steel/Silver shield armor unchanged from vanilla. An Iron shield grants 1 less armor
        /// than vanilla, an Elven shield grants 1 more armor than vanilla, a Dwarven shield grants 2 more armor
        /// than vanilla,... a Daedric shield grants 6 more armor than vanilla.
        /// </summary>
        /// <returns>Armor value for the shield.</returns>
        public override int GetShieldArmorValue()
        {
            switch (nativeMaterialValue)
            {
                case (int)ArmorMaterialTypes.Leather:
                    return 1;
                case (int)ArmorMaterialTypes.Chain:
                case (int)ArmorMaterialTypes.Chain2:
                    return 1;
                case (int)ArmorMaterialTypes.Iron:
                    return 0;
                case (int)ArmorMaterialTypes.Steel:
                case (int)ArmorMaterialTypes.Silver:
                    return 1;
                case (int)ArmorMaterialTypes.Elven:
                    return 2;
                case (int)ArmorMaterialTypes.Dwarven:
                    return 3;
                case (int)ArmorMaterialTypes.Mithril:
                case (int)ArmorMaterialTypes.Adamantium:
                    return 4;
                case (int)ArmorMaterialTypes.Ebony:
                    return 5;
                case (int)ArmorMaterialTypes.Orcish:
                    return 6;
                case (int)ArmorMaterialTypes.Daedric:
                    return 7;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Copied from vanilla's DaggerfallUnityItem script, modified for Bossfall.
        /// </summary>
        /// <returns>BodyParts[] protected by the shield.</returns>
        public override BodyParts[] GetShieldProtectedBodyParts()
        {
            return new BodyParts[] { BodyParts.LeftArm, BodyParts.Hands };
        }

        /// <summary>
        /// Equips new shields to standard vanilla shield slot.
        /// </summary>
        /// <returns>Equipment Slot for the shield.</returns>
        public override EquipSlots GetEquipSlot()
        {
            return EquipSlots.LeftHand;
        }

        /// <summary>
        /// Use standard vanilla left-hand-only shield rules.
        /// </summary>
        /// <returns>Which hand to equip the item in.</returns>
        public override ItemHands GetItemHands()
        {
            return ItemHands.LeftOnly;
        }

        /// <summary>
        /// This method override is straight out of RPR:I's item scripts, so I don't know what exactly this method is
        /// doing. I assume this ensures the item gets saved/loaded correctly.
        /// </summary>
        /// <returns>Save data.</returns>
        public override ItemData_v1 GetSaveData()
        {
            // DWI

            // Perhaps this is the place to re-create ur custom shields? U'll have to transplant durability
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(Buckler).ToString();
            return data;
        }
    }
}
