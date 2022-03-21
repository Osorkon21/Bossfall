// Project:         Bossfall
// Copyright:       All additions or modifications to vanilla or classic data copyright (C) 2022 Osorkon
// License:         Osorkon's additions or modifications use the MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU's MagicItemTemplates.txt https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, whoever wrote vanilla DFU's MagicItemTemplates.txt
// Contributors:    Whoever contributed to vanilla DFU's MagicItemTemplates.txt
// 
// Notes: According to vanilla DFU, MagicItemTemplates.txt is a JSON dump of classic's fixed MAGIC.DEF file. I think
//        the copyright on that is (C) 1996 Bethesda Softworks, but I'm only guessing. Vanilla DFU redistributes this
//        file so I assume I can do so as well.
//

using DaggerfallConnect.FallExe;
using UnityEngine;

namespace BossfallMod.Items
{
    /// <summary>
    /// Custom Magic Item templates.
    /// </summary>
    public class BossfallMagicItemTemplates : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Regular magic item templates only, no artifacts. Some are vanilla/classic templates with different "index"
        /// numbers and reduced "uses". Comments indicate authorship.
        /// </summary>
        readonly MagicItemTemplate[] customMagicItemTemplates =
        {
            // I added this. Casts Quiet Undead on use.
            new MagicItemTemplate
            {
                index = 0,
                name = "Spirit Soothing %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 98
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 700,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 1,
                name = "%it of the Wise",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenHeld,
                        param = 83
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1000,
                value = 400,
                material = 0
            },

            // I added this. Casts Spell Resistance on use.
            new MagicItemTemplate
            {
                index = 2,
                name = "%it of Arcane Ward",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 39
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 800,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 3,
                name = "%it of Force Bolts",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 36
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 850,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 4,
                name = "Shining %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 5
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 350,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 5,
                name = "%it of the Sealed Door",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 19
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 300,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 6,
                name = "%it of Undeniable Access",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 18
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 500,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 7,
                name = "%it of Venom Antidote",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 15
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 450,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 8,
                name = "%it of Life Stealing",
                type = MagicItemTypes.RegularMagicItem,
                group = 2,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.VampiricEffect,
                        param = 1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 2500,
                value = 1500,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 9,
                name = "%it of Wizard's Fire",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 7
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 900,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 10,
                name = "Leaping %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 4
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 300,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 11,
                name = "%it of Water-walking",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenHeld,
                        param = 41
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1000,
                value = 300,
                material = 0
            },

            // I added this. Casts Shalidor's Mirror on use.
            new MagicItemTemplate
            {
                index = 12,
                name = "%it of Rebounding",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 30
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 600,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 13,
                name = "Never Tiring %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 40
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 250,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 14,
                name = "%it of Featherweight",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 37
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 550,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 15,
                name = "%it, the Protector",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 17
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 650,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 16,
                name = "%it of Shocking",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 8
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 650,
                material = 0
            },

            // I added this. Casts Tame on use.
            new MagicItemTemplate
            {
                index = 17,
                name = "%it of Domestication",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 99
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 500,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 18,
                name = "%it of Far Silence",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 28
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 850,
                material = 0
            },

            // I added this. Casts Recall on use.
            new MagicItemTemplate
            {
                index = 19,
                name = "%it of Teleportation",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 94
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 500,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 20,
                name = "Blazing %it of Fireballs",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 14
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 1000,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 21,
                name = "Frosty %it of Ice Storms",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 20
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 900,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 22,
                name = "%it of Lightning",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 31
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 900,
                material = 0
            },

            // Vanilla/classic template, but I changed the item name. "Deadman's Item" casting Fire Storm made no sense to me.
            new MagicItemTemplate
            {
                index = 23,
                name = "%it of Pyromania",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 25
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 800,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 24,
                name = "Healing %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 64
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 550,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 25,
                name = "%it of the Orc Lord",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenHeld,
                        param = 82
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1000,
                value = 750,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 26,
                name = "Unrestrainable %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 10
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 500,
                material = 0
            },

            // I added this. Casts Calm Humanoid on use.
            new MagicItemTemplate
            {
                index = 27,
                name = "Calming %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 91
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 400,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 28,
                name = "%it of Nimbleness",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenHeld,
                        param = 85
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1000,
                value = 400,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 29,
                name = "Torgo's %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenHeld,
                        param = 86
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1000,
                value = 700,
                material = 0
            },

            // I added this. Casts Charm Mortal on use.
            new MagicItemTemplate
            {
                index = 30,
                name = "Beguiling %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 90
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 400,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 31,
                name = "%it of Friendship",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenHeld,
                        param = 88
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1000,
                value = 350,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 32,
                name = "%it of Good Luck",
                type = MagicItemTypes.RegularMagicItem,
                group = 1,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenHeld,
                        param = 89
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1000,
                value = 400,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 33,
                name = "%it of Oblivion",
                type = MagicItemTypes.RegularMagicItem,
                group = 2,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenStrikes,
                        param = 55
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 1200,
                value = 700,
                material = 0
            },

            // Vanilla/classic template.
            new MagicItemTemplate
            {
                index = 34,
                name = "%it of Toxic Clouds",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 29
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 650,
                material = 0
            },

            // I added this. Casts Medusa's Gaze on use.
            new MagicItemTemplate
            {
                index = 35,
                name = "Medusa's %it",
                type = MagicItemTypes.RegularMagicItem,
                group = 0,
                groupIndex = 0,
                enchantments = new DaggerfallEnchantment[]
                {
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.CastWhenUsed,
                        param = 35
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    },
                    new DaggerfallEnchantment
                    {
                        type = EnchantmentTypes.None,
                        param = -1
                    }
                },
                uses = 500,
                value = 600,
                material = 0
            }
        };

        #endregion

        #region Properties

        public static BossfallMagicItemTemplates Instance { get { return Bossfall.Instance.GetComponent<BossfallMagicItemTemplates>(); } }

        public MagicItemTemplate[] CustomMagicItemTemplates { get { return customMagicItemTemplates; } }

        #endregion
    }
}
