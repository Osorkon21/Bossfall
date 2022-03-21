// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich, Hazelnut, ifkopifko, Numidium, TheLacus
// 
// Notes: This script uses code from several vanilla DFU scripts. Comments indicate authorship - please
//        verify original authorship before crediting. When in doubt compare with vanilla DFU's source code.
//

using BossfallMod.EnemyAI;
using BossfallMod.Utility;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using System;
using UnityEngine;

namespace BossfallMod.Formulas
{
    /// <summary>
    /// Bossfall override data.
    /// </summary>
    public class BossfallOverrides : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Most of this array is the Enemies field from vanilla's EnemyBasics script. I changed every monster's level, HP,
        /// damage, and/or armor, SoulPts value, Weight, and MinMetalToHit. I changed the field to be a readonly instance
        /// field and changed which enemies see Invisible. All comments are mine.
        /// </summary>
        readonly MobileEnemy[] bossfallEnemyStats = new MobileEnemy[]
        {
            // Slightly less damage and armor than vanilla, similar HP. They move somewhat fast.
            // Rats are very common in dungeons, rare in the wilderness during the day and fairly common
            // at night. 5% chance per hit of infecting player with the Plague, and they always charge.
            // They never appear in towns at night.
            new MobileEnemy()
            {
                ID = 0,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 255,
                FemaleTexture = 255,
                CorpseTexture = EnemyBasics.CorpseTexture(401, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyRatMove,
                BarkSound = (int)SoundClips.EnemyRatBark,
                AttackSound = (int)SoundClips.EnemyRatAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 3,
                MinHealth = 6,
                MaxHealth = 18,
                Level = 1,
                ArmorValue = 7,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                Team = MobileTeams.Vermin,
            },

            // Much less damage and armor, slightly more HP. Their spells are a lot nastier and
            // they move somewhat slowly. They are never found outside and are only present in a few dungeon
            // types. I figure they are familiars to spellcasters and don't wander around on their own.
            new MobileEnemy()
            {
                ID = 1,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 256,
                FemaleTexture = 256,
                CorpseTexture = EnemyBasics.CorpseTexture(406, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyImpMove,
                BarkSound = (int)SoundClips.EnemyImpBark,
                AttackSound = (int)SoundClips.EnemyImpAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 8,
                MaxHealth = 24,
                Level = 2,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "D",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 1 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 1 },
                Team = MobileTeams.Magic,
            },

            // Much less damage and armor, more HP. Immune to Archery, resistant to Short Blades and
            // Hand-to-Hand, takes double damage from Axes. They move extremely slowly. They are in Natural Cave
            // dungeon types and very common in wilderness during the day anywhere except Deserts. Not as active
            // during the night.
            new MobileEnemy()
            {
                ID = 2,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 257,
                FemaleTexture = 257,
                CorpseTexture = EnemyBasics.CorpseTexture(406, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemySprigganMove,
                BarkSound = (int)SoundClips.EnemySprigganBark,
                AttackSound = (int)SoundClips.EnemySprigganAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 4,
                MinDamage2 = 1,
                MaxDamage2 = 4,
                MinDamage3 = 1,
                MaxDamage3 = 4,
                MinHealth = 14,
                MaxHealth = 42,
                Level = 3,
                ArmorValue = 6,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                LootTableKey = "B",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 3, 3 },
                Team = MobileTeams.Spriggans,
            },

            // Less damage and HP, slightly better armor. They move very fast. Very common in dungeons,
            // outside they prefer warmer climates. They never spawn outside during the day or in towns at night,
            // but are common in nighttime wilderness. 5% chance per hit of transmitting disease. They always charge.
            new MobileEnemy()
            {
                ID = 3,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 258,
                FemaleTexture = 258,
                CorpseTexture = EnemyBasics.CorpseTexture(401, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyGiantBatMove,
                BarkSound = (int)SoundClips.EnemyGiantBatBark,
                AttackSound = (int)SoundClips.EnemyGiantBatAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 7,
                MinHealth = 8,
                MaxHealth = 24,
                Level = 3,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                Team = MobileTeams.Vermin,
            },

            // Less damage, much more HP. They move somewhat fast. They are only in Natural Cave
            // dungeon types. Bears love mountain and woodland wilderness, day or night. They always charge.
            new MobileEnemy()
            {
                ID = 4,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 259,
                FemaleTexture = 259,
                CorpseTexture = EnemyBasics.CorpseTexture(401, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyBearMove,
                BarkSound = (int)SoundClips.EnemyBearBark,
                AttackSound = (int)SoundClips.EnemyBearAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 6,
                MinDamage2 = 1,
                MaxDamage2 = 6,
                MinDamage3 = 1,
                MaxDamage3 = 6,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 4,
                ArmorValue = 6,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 0 },
                Team = MobileTeams.Bears,
            },

            // Much less damage, much more HP and armor. They move fast. Never in any dungeons
            // and only outside in subtropical/rainforest climates. Not as active at night. They always charge.
            new MobileEnemy()
            {
                ID = 5,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 260,
                FemaleTexture = 260,
                CorpseTexture = EnemyBasics.CorpseTexture(401, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyTigerMove,
                BarkSound = (int)SoundClips.EnemyTigerBark,
                AttackSound = (int)SoundClips.EnemyTigerAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 7,
                MinDamage2 = 1,
                MaxDamage2 = 7,
                MinDamage3 = 1,
                MaxDamage3 = 7,
                MinHealth = 25,
                MaxHealth = 75,
                Level = 4,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                Team = MobileTeams.Tigers,
            },

            // Much less damage, more HP. They move very fast. 10% chance/hit to poison, 0.1% chance/hit
            // to inflict Immunity-bypassing poison, doesn't Paralyze. Common in many dungeons. Outside Spiders
            // prefer tropical wilderness, day or night, and are rarely found in woodlands. They always charge.
            new MobileEnemy()
            {
                ID = 6,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 261,
                FemaleTexture = 261,
                CorpseTexture = EnemyBasics.CorpseTexture(401, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemySpiderMove,
                BarkSound = (int)SoundClips.EnemySpiderBark,
                AttackSound = (int)SoundClips.EnemySpiderAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 8,
                MinHealth = 14,
                MaxHealth = 42,
                Level = 4,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                Team = MobileTeams.Spiders,
            },

            // Much more damage and HP. They move somewhat slowly. Orcs don't wield weapons anymore - they
            // carried way too much loot in vanilla. They prefer tropical climates, day or night, and are only in
            // Orc Stronghold dungeon types. They never appear in towns at night.
            new MobileEnemy()
            {
                ID = 7,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 262,
                FemaleTexture = 262,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcMove,
                BarkSound = (int)SoundClips.EnemyOrcBark,
                AttackSound = (int)SoundClips.EnemyOrcAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 12,
                MinHealth = 24,
                MaxHealth = 72,
                Level = 5,
                ArmorValue = 7,
                ParrySounds = true,
                MapChance = 0,
                Weight = 100000,
                LootTableKey = "A",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                Team = MobileTeams.Orcs,
            },

            // Less damage, more HP. They don't wield weapons anymore - they carried too much loot in
            // vanilla. They move fast. They are never in dungeons or towns at night, only appearing in
            // daytime temperate and mountain wilderness.
            new MobileEnemy()
            {
                ID = 8,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 263,
                FemaleTexture = 263,
                CorpseTexture = EnemyBasics.CorpseTexture(406, 0),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyCentaurMove,
                BarkSound = (int)SoundClips.EnemyCentaurBark,
                AttackSound = (int)SoundClips.EnemyCentaurAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 20,
                MaxHealth = 60,
                Level = 5,
                ArmorValue = 6,
                ParrySounds = true,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "C",
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 1, 2, -1, 3, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, 1, 1, 2, -1, 3, 3, 2, 1, 1, -1, 2, 3, 3, 4 },
                Team = MobileTeams.Centaurs,
            },

            // Less damage but way higher Level, HP, and Armor. Non-Silver materials deal half damage.
            // They move very fast and are never in dungeons. They rarely appear outside at night almost anywhere,
            // but never in Desert wilderness. Common in day/night Haunted Woodlands wilderness.
            new MobileEnemy()
            {
                ID = 9,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 264,
                FemaleTexture = 264,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyWerewolfMove,
                BarkSound = (int)SoundClips.EnemyWerewolfBark,
                AttackSound = (int)SoundClips.EnemyWerewolfAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 8,
                MinDamage2 = 1,
                MaxDamage2 = 8,
                MinDamage3 = 1,
                MaxDamage3 = 8,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 12,
                ArmorValue = 0,
                MapChance = 0,
                ParrySounds = false,
                Weight = 100000,
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, -1, 2 },
                Team = MobileTeams.Werecreatures,
            },

            // More damage, less HP and much less Armor. They move slowly. They're only found
            // in Natural Cave dungeon types and are common outside in tropical daytime wilderness. They
            // never spawn outside at night.
            new MobileEnemy()
            {
                ID = 10,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 265,
                FemaleTexture = 265,
                CorpseTexture = EnemyBasics.CorpseTexture(406, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyNymphMove,
                BarkSound = (int)SoundClips.EnemyNymphBark,
                AttackSound = (int)SoundClips.EnemyNymphAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 15,
                MaxHealth = 45,
                Level = 6,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "C",
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, 4, -1, 5 },
                Team = MobileTeams.Nymphs,
            },

            // Slightly less damage, slightly more HP and armor. They swim slowly. Only found underwater. They always charge.
            new MobileEnemy()
            {
                ID = 11,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 266,
                FemaleTexture = 266,
                CorpseTexture = EnemyBasics.CorpseTexture(305, 1),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyEelMove,
                BarkSound = (int)SoundClips.EnemyEelBark,
                AttackSound = (int)SoundClips.EnemyEelAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 12,
                MinHealth = 20,
                MaxHealth = 60,
                Level = 7,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 3, -1, 5, 4, 3, 3, -1, 5, 4, 3, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 3, -1, 5, 0 },
                Team = MobileTeams.Aquatic,
            },

            // More damage, much higher HP, Level, and Armor. They move somewhat slowly. They are
            // only in Orc Stronghold dungeon types. Outside they prefer tropical climates, day or night. They
            // are rarer than standard Orcs. They never appear in towns at night.
            new MobileEnemy()
            {
                ID = 12,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 267,
                FemaleTexture = 267,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcSergeantMove,
                BarkSound = (int)SoundClips.EnemyOrcSergeantBark,
                AttackSound = (int)SoundClips.EnemyOrcSergeantAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 25,
                MinHealth = 50,
                MaxHealth = 150,
                Level = 11,
                ArmorValue = 2,
                ParrySounds = true,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "A",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 5, 4, 3, -1, 2, 1, 0 },
                Team = MobileTeams.Orcs,
            },

            // Slightly more damage, similar HP, much less armor. They move fast. Common
            // in a few dungeon types and are only outside during the day in mountainous regions -
            // I assume mountains make good nesting grounds. They never spawn outside at night.
            new MobileEnemy()
            {
                ID = 13,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 268,
                FemaleTexture = 268,
                CorpseTexture = EnemyBasics.CorpseTexture(406, 4),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHarpyMove,
                BarkSound = (int)SoundClips.EnemyHarpyBark,
                AttackSound = (int)SoundClips.EnemyHarpyAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 23,
                MinHealth = 25,
                MaxHealth = 75,
                Level = 8,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                LootTableKey = "D",
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                Team = MobileTeams.Harpies,
            },

            // More damage and Armor and way higher Level and HP. Non-Silver materials deal half damage.
            // They move fast and are never in dungeons. They rarely appear outside at night almost anywhere, but
            // never in Desert wilderness. Common in day/night Haunted Woodlands wilderness.
            new MobileEnemy()
            {
                ID = 14,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 269,
                FemaleTexture = 269,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyWereboarMove,
                BarkSound = (int)SoundClips.EnemyWereboarBark,
                AttackSound = (int)SoundClips.EnemyWereboarAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 16,
                MinDamage2 = 1,
                MaxDamage2 = 16,
                MinDamage3 = 1,
                MaxDamage3 = 16,
                MinHealth = 44,
                MaxHealth = 132,
                Level = 12,
                ArmorValue = 2,
                MapChance = 0,
                ParrySounds = false,
                Weight = 100000,
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 2 },
                Team = MobileTeams.Werecreatures,
            },

            // Lower Level, HP, and Armor. They move somewhat fast. Immune to Archery, resistant to
            // Long/Short Blades, takes double damage from Blunt Weapons. Ubiquitous at night in the wilderness
            // and common in many dungeons. Can appear during the day in Haunted Woodlands wilderness. They
            // never spawn in towns at night. 2% chance/hit of transmitting any disease. They always charge
            // and are occasionally found underwater.
            new MobileEnemy()
            {
                ID = 15,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 270,
                FemaleTexture = 270,
                CorpseTexture = EnemyBasics.CorpseTexture(306, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemySkeletonMove,
                BarkSound = (int)SoundClips.EnemySkeletonBark,
                AttackSound = (int)SoundClips.EnemySkeletonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 19,
                MinHealth = 17,
                MaxHealth = 51,
                Level = 8,
                ArmorValue = 4,
                ParrySounds = true,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "H",
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                Team = MobileTeams.Undead,
            },

            // Less damage and Armor, higher Level and way more HP. They move somewhat fast. Only in
            // Giant Stronghold dungeon types. Common outside in woodland and mountain daytime wilderness. They
            // never spawn outside at night. They always charge.
            new MobileEnemy()
            {
                ID = 16,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 271,
                FemaleTexture = 271,
                CorpseTexture = EnemyBasics.CorpseTexture(406, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyGiantMove,
                BarkSound = (int)SoundClips.EnemyGiantBark,
                AttackSound = (int)SoundClips.EnemyGiantAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 70,
                MaxHealth = 210,
                Level = 12,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 1,
                LootTableKey = "F",
                Weight = 100000,
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                Team = MobileTeams.Giants,
            },

            // Much lower Level, damage, and Armor, more HP. They move extremely slowly. Resistant to Short
            // Blades, Hand-to-Hand, and Archery, takes double damage from Axes. 5% chance/hit of transmitting any disease.
            // Common in many dungeons and outside in wilderness at night. They never spawn in towns at night. Can
            // appear during the day in Haunted Woodlands wilderness. They always charge and are rarely found underwater.
            new MobileEnemy()
            {
                ID = 17,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 272,
                FemaleTexture = 272,
                CorpseTexture = EnemyBasics.CorpseTexture(306, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyZombieMove,
                BarkSound = (int)SoundClips.EnemyZombieBark,
                AttackSound = (int)SoundClips.EnemyZombieAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 7,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "G",
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, 2, -1, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // Less damage, much less HP, much more Armor. Casts no spells. Can only be damaged by
            // Silver. Moves extremely slowly. Extremely fragile but hard to hit. Holy Water or Holy Daggers
            // recommended if player isn't a spellcaster. Common in many dungeons and outside in wilderness
            // at night. They can spawn in Haunted Woodland towns at night, and very common in Haunted
            // Woodlands wilderness, day or night. They always charge and are very rarely found underwater.
            new MobileEnemy()
            {
                ID = 18,
                Behaviour = MobileBehaviour.Spectral,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 273,
                FemaleTexture = 273,
                CorpseTexture = EnemyBasics.CorpseTexture(306, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyGhostMove,
                BarkSound = (int)SoundClips.EnemyGhostBark,
                AttackSound = (int)SoundClips.EnemyGhostAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 5,
                MaxHealth = 15,
                Level = 11,
                ArmorValue = -4,
                ParrySounds = false,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "I",
                NoShadow = true,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                SpellAnimFrames = new int[] { 0, 0, 0, 0, 0, 0 },
                Team = MobileTeams.Undead,
            },

            // More damage, lower Level, much more HP. Moves extremely slowly. Resistant to Archery,
            // takes double damage from Axes and Long Blades. Common in a few undead-themed dungeon types, rare
            // outside in wilderness at night. Very common at night in Desert wilderness - the environment suits
            // them. They never spawn in towns at night. Can appear during the day in Haunted Woodlands
            // wilderness. 2% chance/hit of transmitting disease, and they always charge.
            new MobileEnemy()
            {
                ID = 19,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 274,
                FemaleTexture = 274,
                CorpseTexture = EnemyBasics.CorpseTexture(306, 5),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyMummyMove,
                BarkSound = (int)SoundClips.EnemyMummyBark,
                AttackSound = (int)SoundClips.EnemyMummyAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 25,
                MinHealth = 45,
                MaxHealth = 135,
                Level = 10,
                ArmorValue = 2,
                ParrySounds = false,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "E",
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // Less damage and Armor, more HP. Moves fast. Resistant to Hand-to-Hand and Archery,
            // takes double damage from Axes. 10% chance/hit to poison, 0.1% chance/hit to inflict
            // Immunity-bypassing poison, doesn't Paralyze. Very common in Scorpion Nest dungeon types and
            // outside in daytime Desert wilderness. They never spawn outside at night. They always charge.
            new MobileEnemy()
            {
                ID = 20,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 275,
                FemaleTexture = 275,
                CorpseTexture = EnemyBasics.CorpseTexture(401, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyScorpionMove,
                BarkSound = (int)SoundClips.EnemyScorpionBark,
                AttackSound = (int)SoundClips.EnemyScorpionAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 12,
                ParrySounds = false,
                ArmorValue = 1,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                Team = MobileTeams.Scorpions,
            },

            // Much higher damage, HP, Level, and Armor. They move somewhat slowly. Greatly increased
            // spell variety, increased chance to carry good loot. Only in Orc Stronghold dungeon type. Outside
            // they prefer tropical climates, day or night. They never appear in towns at night. Shamans are very
            // rare compared to standard Orcs.
            new MobileEnemy()
            {
                ID = 21,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 276,
                FemaleTexture = 276,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcShamanMove,
                BarkSound = (int)SoundClips.EnemyOrcShamanBark,
                AttackSound = (int)SoundClips.EnemyOrcShamanAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 35,
                MinHealth = 55,
                MaxHealth = 165,
                Level = 16,
                ArmorValue = -2,
                ParrySounds = true,
                MapChance = 3,
                Weight = 100000,
                LootTableKey = "U",
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack2 = 20,
                PrimaryAttackAnimFrames2 = new int[] { 0, -1, 4, 5, 0 },
                ChanceForAttack3 = 20,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 4, 5, 0 },
                ChanceForAttack4 = 20,
                PrimaryAttackAnimFrames4 = new int[] { 0, 1, -1, 3, 2, -1, 3, 2, 1, 0 },
                ChanceForAttack5 = 20,
                PrimaryAttackAnimFrames5 = new int[] { 0, -1, 4, 5, -1, 4, 5, 0 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 0, 1, 2, 3, 3, 3 },
                Team = MobileTeams.Orcs,
            },

            // Much more damage and HP, slightly better Armor. They move extremely slowly. Immune to
            // Hand-to-Hand and Archery, resistant to Long/Short Blades. Common in Mines, Laboratories, and
            // Giant Strongholds (they speak Giantish). They rarely appear outside at night and rarely spawn
            // in daytime Desert wilderness. They always charge.
            new MobileEnemy()
            {
                ID = 22,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 277,
                FemaleTexture = 277,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyGargoyleMove,
                BarkSound = (int)SoundClips.EnemyGargoyleBark,
                AttackSound = (int)SoundClips.EnemyGargoyleAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 50,
                MinHealth = 50,
                MaxHealth = 150,
                Level = 14,
                ArmorValue = -1,
                MapChance = 0,
                ParrySounds = false,
                Weight = 100000,
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 2, 1, 2, 3, -1, 4, 0 },
                Team = MobileTeams.Magic,
            },

            // Much less damage and HP, much more Armor. Immune to all materials except Silver. They move
            // slowly. Greatly increased spell variety. Very fragile but incredibly hard to hit. Holy Water or Holy
            // Daggers recommended if player isn't a spellcaster. Common in Vampire Haunts, somewhat common in a few
            // other dungeon types. Rarely spawns in outside wilderness at night. Can spawn in Haunted Woodland towns
            // at night. Very common in Haunted Woodland wilderness, day or night. They always charge, and are very
            // rarely found underwater.
            new MobileEnemy()
            {
                ID = 23,
                Behaviour = MobileBehaviour.Spectral,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 278,
                FemaleTexture = 278,
                CorpseTexture = EnemyBasics.CorpseTexture(306, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyWraithMove,
                BarkSound = (int)SoundClips.EnemyWraithBark,
                AttackSound = (int)SoundClips.EnemyWraithAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 45,
                MinHealth = 10,
                MaxHealth = 30,
                Level = 15,
                ArmorValue = -8,
                ParrySounds = false,
                MapChance = 1,
                Weight = 100000,
                LootTableKey = "I",
                NoShadow = true,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                SpellAnimFrames = new int[] { 0, 0, 0, 0, 0 },
                Team = MobileTeams.Undead,
            },

            // The fourth most difficult boss of the game, tougher than Alternate Dragonlings but not as
            // tough as a Daedra Lord. Damage, HP, Level, and Armor greatly increased. Moves somewhat fast. Has much
            // greater chance of dropping good loot. They are the only boss who don't See Invisible. They are very rare.
            new MobileEnemy()
            {
                ID = 24,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 279,
                FemaleTexture = 279,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcWarlordMove,
                BarkSound = (int)SoundClips.EnemyOrcWarlordBark,
                AttackSound = (int)SoundClips.EnemyOrcWarlordAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 42,
                MaxDamage = 73,
                MinHealth = 150,
                MaxHealth = 450,
                Level = 25,
                ArmorValue = -10,
                ParrySounds = true,
                MapChance = 2,
                Weight = 100000,
                LootTableKey = "T",
                SoulPts = 1500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 3, 4 -1, 5, 0, 4, -1, 5, 0 },
                Team = MobileTeams.Orcs,
            },

            // Much less damage and Armor, less HP. Moves slowly. Frostbite added to spell kit.
            // Common in Covens and outside in Mountain wilderness at night. Will very rarely spawn in Mountain
            // Woods wilderness at night. Never spawns in towns at night. Greater chance to drop good loot. 
            new MobileEnemy()
            {
                ID = 25,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 280,
                FemaleTexture = 280,
                CorpseTexture = EnemyBasics.CorpseTexture(400, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFrostDaedraMove,
                BarkSound = (int)SoundClips.EnemyFrostDaedraBark,
                AttackSound = (int)SoundClips.EnemyFrostDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 100,
                MinHealth = 35,
                MaxHealth = 105,
                Level = 17,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 100000,
                LootTableKey = "J",
                NoShadow = true,
                GlowColor = new Color(18, 68, 88) * 0.1f,
                SoulPts = 100000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { -1, 4, 5, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // Less damage, much more HP and Armor. Moves fast. God's Fire added to spell kit. Common
            // in a handful of dungeon types and outside in Desert daytime wilderness. Never spawns outside at
            // night. I don't recommend using Hand-to-Hand on them - doing so will damage you 4 HP every landed
            // attack. They have a greater chance to drop good loot.
            new MobileEnemy()
            {
                ID = 26,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 281,
                FemaleTexture = 281,
                CorpseTexture = EnemyBasics.CorpseTexture(400, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFireDaedraMove,
                BarkSound = (int)SoundClips.EnemyFireDaedraBark,
                AttackSound = (int)SoundClips.EnemyFireDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 50,
                MinHealth = 60,
                MaxHealth = 180,
                Level = 17,
                ArmorValue = -3,
                ParrySounds = true,
                MapChance = 0,
                Weight = 100000,
                LootTableKey = "J",
                NoShadow = true,
                GlowColor = new Color(243, 239, 44) * 0.05f,
                SoulPts = 100000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, -1, 4 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // Less damage, much more HP and Armor. Moves somewhat fast. Greatly increased spell
            // variety. Somewhat common in a handful of dungeon types, can rarely appear outside in tropical
            // wilderness, day or night. They look like crocodiles, so I would think they prefer hot climates.
            // Can rarely appear in Rainforest towns at night. Greater chance to drop good loot.
            new MobileEnemy()
            {
                ID = 27,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 282,
                FemaleTexture = 282,
                CorpseTexture = EnemyBasics.CorpseTexture(400, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyLesserDaedraMove,
                BarkSound = (int)SoundClips.EnemyLesserDaedraBark,
                AttackSound = (int)SoundClips.EnemyLesserDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 50,
                MinHealth = 66,
                MaxHealth = 198,
                Level = 18,
                ArmorValue = -4,
                ParrySounds = true,
                MapChance = 0,
                Weight = 100000,
                LootTableKey = "E",
                SoulPts = 150000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0, 4, -1, 5, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // The easiest boss of the game. Greatly increased Level, damage, HP, and Armor. They move extremely
            // fast and take double damage from Silver. Spell variety greatly increased, they have Magicka for 30 spells.
            // 1.4% chance/hit of transmitting any disease, 0.6% chance/hit of transmitting stage one Vampirism. High
            // chance to drop good loot. Very common in Vampire Haunt dungeon types, very rare otherwise.
            new MobileEnemy()
            {
                ID = 28,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 283,
                FemaleTexture = 283,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFemaleVampireMove,
                BarkSound = (int)SoundClips.EnemyFemaleVampireBark,
                AttackSound = (int)SoundClips.EnemyFemaleVampireAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 32,
                MaxDamage = 62,
                MinHealth = 80,
                MaxHealth = 240,
                Level = 23,
                ArmorValue = -6,
                ParrySounds = false,
                MapChance = 3,
                Weight = 100000,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                NoShadow = true,
                SoulPts = 750000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                SpellAnimFrames = new int[] { 1, 1, 5, 5 },
                Team = MobileTeams.Undead,
            },

            // The toughest non-boss enemy of the game. Much more damage, HP, and Armor. They move fast.
            // Spell variety greatly increased. They are relatively common in a handful of the toughest dungeon types.
            // Also, they will very rarely spawn outside in daytime wilderness wherever the player normally finds Nymphs.
            // My thought was every now and then a Daedra Seducer would pretend to be a Nymph and try to trick foolish
            // mortals. They have a higher chance of dropping good loot.
            new MobileEnemy()
            {
                ID = 29,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 284,
                FemaleTexture = 284,
                CorpseTexture = EnemyBasics.CorpseTexture(400, 6),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                HasSeducerTransform1 = true,
                HasSeducerTransform2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemySeducerMove,
                BarkSound = (int)SoundClips.EnemySeducerBark,
                AttackSound = (int)SoundClips.EnemySeducerAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 90,
                MinHealth = 75,
                MaxHealth = 225,
                Level = 19,
                ArmorValue = -5,
                ParrySounds = false,
                MapChance = 1,
                Weight = 100000,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                SoulPts = 200000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2 },
                SpellAnimFrames = new int[] { 0, 1, 2 },
                SeducerTransform1Frames = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                SeducerTransform2Frames = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                Team = MobileTeams.Daedra,
            },

            // The second toughest boss, tougher than a Daedra Lord but not as tough as an Ancient Lich. All
            // stats heavily buffed. They are incredibly fast and will outrun player with 100 Running and SPD. They take
            // double damage from Silver. Casts no spells. 1.4% chance/hit of transmitting any disease, 0.6% chance/hit
            // of transmitting stage one Vampirism. Incredibly high chance to drop good loot. They are very rare.
            new MobileEnemy()
            {
                ID = 30,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 285,
                FemaleTexture = 285,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyVampireMove,
                BarkSound = (int)SoundClips.EnemyVampireBark,
                AttackSound = (int)SoundClips.EnemyVampireAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 82,
                MaxDamage = 112,
                MinHealth = 180,
                MaxHealth = 540,
                Level = 28,
                ArmorValue = -12,
                ParrySounds = false,
                MapChance = 3,
                Weight = 100000,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                NoShadow = true,
                SoulPts = 2250000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                SpellAnimFrames = new int[] { 1, 1, 5, 5 },
                Team = MobileTeams.Undead,
            },

            // The third toughest boss, tougher than Orc Warlords but not as tough as an Ancient Vampire.
            // All stats buffed. They move fast. Spell variety greatly increased, they have infinite Magicka.
            // Incredibly high chance to drop good loot. They are very rare.
            new MobileEnemy()
            {
                ID = 31,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 286,
                FemaleTexture = 286,
                CorpseTexture = EnemyBasics.CorpseTexture(400, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyDaedraLordMove,
                BarkSound = (int)SoundClips.EnemyDaedraLordBark,
                AttackSound = (int)SoundClips.EnemyDaedraLordAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 30,
                MaxDamage = 60,
                MinHealth = 170,
                MaxHealth = 510,
                Level = 28,
                ArmorValue = -11,
                ParrySounds = true,
                MapChance = 0,
                Weight = 100000,
                SeesThroughInvisibility = true,
                LootTableKey = "S",
                SoulPts = 2250000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 3, -1, 4, 0, -1, 4, 3, -1, 4, 0, -1, 4, 3 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 1, 0, 1, -1, 2, 1, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // The second easiest boss, tougher than a Vampire but not as tough as an Assassin. Slightly
            // less damage than vanilla, higher Level, much more HP, much less Armor. They take double damage from
            // Silver. Spell variety greatly increased, has Magicka for 30 spells. Moves slowly. High chance of
            // dropping good loot. They are very rare.
            new MobileEnemy()
            {
                ID = 32,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 287,
                FemaleTexture = 287,
                CorpseTexture = EnemyBasics.CorpseTexture(306, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyLichMove,
                BarkSound = (int)SoundClips.EnemyLichBark,
                AttackSound = (int)SoundClips.EnemyLichAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 60,
                MaxDamage = 90,
                MinHealth = 80,
                MaxHealth = 240,
                Level = 23,
                ArmorValue = -7,
                ParrySounds = false,
                MapChance = 4,
                Weight = 100000,
                SeesThroughInvisibility = true,
                LootTableKey = "S",
                SoulPts = 750000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 2, -1, 3, 4, 4 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // The toughest boss in the game. They have the highest HP, Armor, and damage, but they take double
            // damage from Silver (you probably won't be able to hit them with anything Silver - you generally need Daedric's
            // to-hit buffs to land any attacks). Dealing devastating damage with melee attacks and massive spell variety,
            // they are any adventurer's worst nightmare. To reliably defeat them, you need a Daedric high-damage weapon,
            // 300+ HP, plenty of Healing, and a very high weapon skill. Chipping away at them over time won't work, as they
            // regularly cast Heal on themselves and have infinite Magicka. To top it off, they will likely reflect or resist
            // any spell you throw at them. Their greatest weakness is their speed - they'll never chase you down as they move
            // slowly. They almost always drop excellent loot. They are very rare and can be found underwater.
            new MobileEnemy()
            {
                ID = 33,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 288,
                FemaleTexture = 288,
                CorpseTexture = EnemyBasics.CorpseTexture(306, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyLichKingMove,
                BarkSound = (int)SoundClips.EnemyLichKingBark,
                AttackSound = (int)SoundClips.EnemyLichKingAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 115,
                MaxDamage = 145,
                MinHealth = 200,
                MaxHealth = 600,
                Level = 28,
                ArmorValue = -13,
                ParrySounds = false,
                MapChance = 4,
                Weight = 100000,
                SeesThroughInvisibility = true,
                LootTableKey = "S",
                SoulPts = 2250000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 2, -1, 3, 4, 4 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // Much more damage, HP, and Armor, but much lower Level. Moves fast. Common in a handful
            // of dungeon types, they only spawn outside in Desert wilderness during the day. They never spawn
            // outside at night. 
            new MobileEnemy()
            {
                ID = 34,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 289,
                FemaleTexture = 289,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyFaeryDragonMove,
                BarkSound = (int)SoundClips.EnemyFaeryDragonBark,
                AttackSound = (int)SoundClips.EnemyFaeryDragonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 40,
                MaxHealth = 120,
                Level = 10,
                ArmorValue = 3,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                Team = MobileTeams.Dragonlings,
            },

            // Less damage, similar HP, much more Armor than vanilla. They move fast.  Punching or kicking
            // a Fire Atronach inflicts 2 HP damage on player every hit. They are common in Laboratory and Volcanic
            // Cave dungeon types, and are very common in Desert daytime wilderness. They never spawn outside at night.
            new MobileEnemy()
            {
                ID = 35,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 290,
                FemaleTexture = 290,
                CorpseTexture = EnemyBasics.CorpseTexture(405, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFireAtronachMove,
                BarkSound = (int)SoundClips.EnemyFireAtronachBark,
                AttackSound = (int)SoundClips.EnemyFireAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 40,
                MaxHealth = 120,
                Level = 16,
                ArmorValue = 3,
                ParrySounds = false,
                MapChance = 0,
                NoShadow = true,
                GlowColor = new Color(243, 150, 44) * 0.05f,
                Weight = 100000,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4 },
                Team = MobileTeams.Magic,
            },

            // More damage, much more HP and Armor. They move extremely slowly. Immune to Hand-to-Hand
            // and Archery, resistant to Long/Short Blades and Axes. They are common in Laboratory and Mine
            // dungeon types but they never spawn outside.
            new MobileEnemy()
            {
                ID = 36,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 291,
                FemaleTexture = 291,
                CorpseTexture = EnemyBasics.CorpseTexture(405, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyIronAtronachMove,
                BarkSound = (int)SoundClips.EnemyIronAtronachBark,
                AttackSound = (int)SoundClips.EnemyIronAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 25,
                MinHealth = 66,
                MaxHealth = 198,
                Level = 16,
                ArmorValue = 2,
                ParrySounds = true,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Magic,
            },

            // Much less damage, more Armor, much more HP. They move extremely slowly. Resistant to Short
            // Blades, Hand-to-Hand, and Archery, takes double damage from Axes. Like all Atronachs, they are common
            // in Laboratory dungeon types. They also appear in Haunted Woodland towns at night and Haunted Woodland
            // wilderness, day or night. 
            new MobileEnemy()
            {
                ID = 37,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 292,
                FemaleTexture = 292,
                CorpseTexture = EnemyBasics.CorpseTexture(405, 0),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFleshAtronachMove,
                BarkSound = (int)SoundClips.EnemyFleshAtronachBark,
                AttackSound = (int)SoundClips.EnemyFleshAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 60,
                MaxHealth = 180,
                Level = 16,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Magic,
            },

            // Much more HP and Armor. They move extremely slowly. Immune to Hand-to-Hand and Archery,
            // resistant to Long/Short Blades. Ice Atronachs are common in Laboratory dungeon types and outside in
            // Mountain nighttime wilderness. They rarely appear in Mountain Woods nighttime wilderness and underwater.
            new MobileEnemy()
            {
                ID = 38,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 293,
                FemaleTexture = 293,
                CorpseTexture = EnemyBasics.CorpseTexture(405, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyIceAtronachMove,
                BarkSound = (int)SoundClips.EnemyIceAtronachBark,
                AttackSound = (int)SoundClips.EnemyIceAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 20,
                MinHealth = 50,
                MaxHealth = 150,
                Level = 16,
                ArmorValue = 3,
                ParrySounds = true,
                MapChance = 0,
                Weight = 100000,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, -1, 3, 4 },
                Team = MobileTeams.Magic,
            },

            new MobileEnemy()
            {
                ID = 39,
                SoulPts = 50000,
            },

            // The fifth toughest boss, tougher than an Assassin but not as tough as an Orc Warlord.
            // All stats greatly buffed. They move extremely fast. They are very rare and usually drop good loot.
            new MobileEnemy()
            {
                ID = 40,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 295,
                FemaleTexture = 295,
                CorpseTexture = EnemyBasics.CorpseTexture(96, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyFaeryDragonMove,
                BarkSound = (int)SoundClips.EnemyFaeryDragonBark,
                AttackSound = (int)SoundClips.EnemyFaeryDragonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 70,
                MaxDamage = 100,
                MinHealth = 130,
                MaxHealth = 390,
                Level = 25,
                ArmorValue = -9,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                SeesThroughInvisibility = true,
                SoulPts = 1500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                Team = MobileTeams.Dragonlings,
            },

            // Slightly better Armor, much lower damage and Level, much more HP. They swim extremely
            // slowly and are very common, but only appear underwater.
            new MobileEnemy()
            {
                ID = 41,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 296,
                FemaleTexture = 296,
                CorpseTexture = EnemyBasics.CorpseTexture(305, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyDreughMove,
                BarkSound = (int)SoundClips.EnemyDreughBark,
                AttackSound = (int)SoundClips.EnemyDreughAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 22,
                MaxHealth = 66,
                Level = 8,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 100000,
                LootTableKey = "R",
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5, -1, 6, 7 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, 2, 3, -1, 4 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 5, -1, 6, 7 },
                Team = MobileTeams.Aquatic,
            },

            // Less damage, much more HP and Armor. They swim slowly and only appear underwater. They are very rare.
            new MobileEnemy()
            {
                ID = 42,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 297,
                FemaleTexture = 297,
                CorpseTexture = EnemyBasics.CorpseTexture(305, 2),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyLamiaMove,
                BarkSound = (int)SoundClips.EnemyLamiaBark,
                AttackSound = (int)SoundClips.EnemyLamiaAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 51,
                MaxHealth = 153,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                LootTableKey = "R",
                Weight = 100000,
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 3, -1, 5, 4, 3, 3, -1, 5, 4, 3, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 3, -1, 5, 0 },
                Team = MobileTeams.Aquatic,
            },

            // Mages move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their melee damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more melee damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types. Level 20 Mages see Invisible.
            new MobileEnemy()
            {
                ID = 128,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 486,
                FemaleTexture = 485,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 3,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 5, 4, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, -1, 5, 4, 0 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Spellswords move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 129,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 476,
                FemaleTexture = 475,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "P",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 5, 4, 3, -1, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Battlemages move somewhat fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 130,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 490,
                FemaleTexture = 489,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Sorcerers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            // Level 20 Sorcerers see Invisible.
            new MobileEnemy()
            {
                ID = 131,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 478,
                FemaleTexture = 477,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 3,
                LootTableKey = "U",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 4, 5, -1, 3, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Healers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their melee damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more melee damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 132,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 486,
                FemaleTexture = 485,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 1,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 5, 4, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, -1, 5, 4, 0 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Nightblades move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types. Level 20 Nightblades see Invisible.
            new MobileEnemy()
            {
                ID = 133,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 490,
                FemaleTexture = 489,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.Criminals,
            },

            // Bards move somewhat fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 134,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Burglars move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 135,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Rogues move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 136,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 480,
                FemaleTexture = 479,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Acrobats move very fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 137,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Thieves move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 138,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // The third easiest boss of the game, tougher than a Lich but not as tough as an Alternate Dragonling.
            // Assassins move extremely fast. Privateer's Hold is a Human Stronghold dungeon type, and Assassins are the bosses
            // of that dungeon type. Thus, Assassins can rarely spawn in Privateer's Hold, and I don't want an unlucky player
            // running into a boss that early on. To avoid that extremely frustrating scenario, Assassins follow standard class
            // enemy unleveling rules. If player is level 1-6, Assassins don't have boss stats and their level, armor, HP,
            // and damage scales with player's level. At player level 7+ Assassins will be Level 21-30, have -8 Armor, 100-300 HP,
            // and deal around 39-67 damage. Always wields a poisoned weapon unless player is level 1. Once player is level 7
            // Assassins will likely drop good loot and their poison will bypass player's Poison Immunity. They are very rare.
            new MobileEnemy()
            {
                ID = 139,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 480,
                FemaleTexture = 479,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                SeesThroughInvisibility = true,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Monks move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 140,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Archers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            // If using Enhanced Combat AI Archers will never voluntarily move into melee range and will always retreat if player
            // charges them, preferring to stay at range and shoot arrows.
            new MobileEnemy()
            {
                ID = 141,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 482,
                FemaleTexture = 481,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                PrefersRanged = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "C",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Rangers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night. Rangers are the only humans
            // brave (or stupid) enough to wander through Desert wilderness. They also spawn in Mountain and Mountain Woods
            // wilderness at night, where it is too cold for most humans. They are only in Natural Cave dungeon types. If using
            // Enhanced Combat AI Rangers will never voluntarily move into melee range and will always retreat if player charges
            // them, preferring to stay at range and shoot arrows.
            new MobileEnemy()
            {
                ID = 142,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 482,
                FemaleTexture = 481,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "C",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Barbarians move somewhat fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside in towns
            // at night and are fairly common in most wilderness areas during day or night. They will frequently spawn in Mountain
            // and Mountain Woods wilderness at night, where it is too cold for most humans. They spawn in a handful of dungeon
            // types. Barbarians always charge.
            new MobileEnemy()
            {
                ID = 143,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Warriors move somewhat slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 144,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Knights move somewhat slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 145,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Guards move very fast. Their level sort of scales with player's level until player is level 7 - I say
            // "sort of" because they always get a random level boost of 0 to 10 at any player level, so "scales" is not very
            // accurate as their level will vary wildly. Once player is level 7 Guards can be levels 1-30 but on average will
            // be around level 15. Guard equipment scales with their level, which means Guards will likely drop a ton of
            // Daedric once player is level 7. Their armor isn't changed by their equipment - it scales with their level - and
            // they can do incredible damage at higher levels. HP unchanged from vanilla. They will also happily riddle you
            // with arrows. Lawbreakers beware! And by the way... HALT!
            new MobileEnemy()
            {
                ID = 146,
                Behaviour = MobileBehaviour.Guard,
                Affinity = MobileAffinity.Human,

                // I changed hostile Guards to use Male Knight textures and animations so they could shoot arrows.
                // Non-hostile Guards use vanilla textures. HALT!
                MaleTexture = 488,
                FemaleTexture = 488,
                CorpseTexture = EnemyBasics.CorpseTexture(380, 1),
                HasIdle = true,

                // I gave them RangedAttack1, same as Knights. HALT!
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.None,
                BarkSound = (int)SoundClips.Halt,
                AttackSound = (int)SoundClips.None,
                ParrySounds = true,
                MapChance = 0,
                CastsMagic = false,

                // I copied and pasted all Knight PrimaryAttack and RangedAttack animations here. Now Guards shoot
                // arrows, which removes the old exploit of getting them stuck on something and mowing them down. Ranged attacks
                // from Guards also trigger the "do you surrender" pop-up, which makes escaping justice much tougher. HALT!
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.CityWatch,
            },
        };

        /// <summary>
        /// This is the classicParamCosts field from vanilla DFU's SoulBound script, changed to a readonly instance array.
        /// Comments indicate changes I made.
        /// </summary>
        readonly short[] enchantmentPointsByEnemyID = new short[]
        {
            -0, -10, -20, -0, -0, -0, -0, -10, -30, -90, -100, -0, -10, -30, -140, -0, -30, -0, -300, -100, -0, -30, -30, -300,

            -15000, // Modified Orc Warlord value

            -500, -500,

            -1000, // Modified Daedroth value
            -7500, // Modified Vampire value

            -1500,

            -22500, -22500, -7500, -22500, // Modified Ancient Vampire, Daedra Lord, Lich, Ancient Lich values

            -0, -300, -300, -300, -300, -0,

            -15000, // Modified Dragonling_Alternate value

            -100, -100
        };

        /// <summary>
        /// Vanilla treasure pile sprites.
        /// </summary>
        readonly int[] bossfallAlternateRandomTreasureIconIndices = new int[]
        {
            0, 1, 3, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 36, 37, 38, 39, 40, 43, 44, 45, 46, 47
        };

        /// <summary>
        /// Covers enemy IDs 0-146. Player heals when they kill an enemy.
        /// </summary>
        readonly byte[] bossfallVampireHealAmounts = new byte[]
        {
            3, 2, 0, 3, 75, 50, 9, 40, 50, 40, 15, 5, 50, 15, 50, 0, 150, 1, 0, 0, 12, 45, 0, 0, 200, 0, 0, 100, 175, 150, 255,
            255, 0, 0, 15, 0, 0, 1, 0, 0, 200, 20, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15, 20, 25, 15, 15, 20, 20, 20, 25, 15, 20, 200,
            35, 30, 35, 50, 45, 40, 50
        };

        readonly string[] bossfallVampireHUDMessages = new string[]
        {
            "You discard the drained %s.", "Barely any blood. Worthless %s.", "You gorge on %s blood.",
            "You feast on %s blood.", "Not enough blood in this %s.", "You greedily lap up the %s's blood.",
            "Fresh %s blood, and plenty of it!", "You ache for more %s blood.", "There's not much blood in the %s.",
            "You guzzle the %s's blood.", "You yearn for more %s blood.", "%s blood. Almost enough to satisfy you.",
            "Not much blood in this %s.", "You savor the %s's blood.", "%s blood revitalizes you!", "%s blood. So sweet...",
            "This %s's blood is now yours.", "%s blood mends your wounds.", "%s blood sates your hunger. For now.",
            "You feel the heady rush of %s blood!", "You crave more %s blood.", "Such powerful %s blood!",
            "%s blood is marvelous. You want more.", "The %s succumbs to your fangs.", "You gulp down the %s's blood.",
            "%s's blood cannot assuage your hunger.", "The %s's blood gushes into your mouth.", "You lust for more %s blood.",
            "You rob the %s of blood.", "This %s's blood is delicious.", "You imbibe the %s's powerful blood!",
            "You drain the %s of blood.", "You sup on the %s's vital fluids.", "You eagerly drink the %s's blood.",
            "Hearty %s blood!", "This %s's blood fortifies you.", "The %s's blood fills your belly.",
            "You consume the %s's blood. You want more.", "You exsanguinate the %s.", "You delight in the %s's blood.",
            "You revel in %s blood.", "You desire more %s blood.", "You'd love more %s blood.",
            "%s blood quenches your thirst. For now.", "You don't waste a drop of the %s's blood."
        };

        /// <summary>
        /// Used when Vampire player characters kill a Zombie or Flesh Atronach.
        /// </summary>
        readonly string[] bossfallOldBloodHUDMessages = new string[]
        {
            "You gag on old %s blood.", "You retch on foul %s blood.", "You regurgitate the %s's blood.",
            "Your stomach rebels at %s blood.", "You consume disgusting %s blood.", "There's no good blood in this %s.",
            "You can't stand %s blood.", "Worthless %s blood.", "This %s blood tastes awful.",
            "Even you can't drink the %s's blood.", "You crave anything other than %s blood.",
            "%s blood. What a waste of time.", "%s blood tastes as vile as it smells.", "Old %s blood fails to satisfy you."
        };

        /// <summary>
        /// Used when player with Lycanthropy kills an innocent.
        /// </summary>
        readonly string[] bossfallInnocentHUDMessages = new string[]
        {
            "Your urge to kill subsides.", "The innocent collapses.", "The innocent breathes their last.",
            "One final twitch and the innocent goes still.", "A pool of blood spreads from the innocent.",
            "You shed innocent blood.", "You are soaked with innocent blood.", "Your urge to hunt fades.",
            "You tear innocent flesh from bone.", "You spill innocent blood.", "The innocent shrieks, then goes silent.",
            "You lock eyes with the dying innocent.", "Blood fountains from the innocent's neck.",
            "Innocent blood soaks the ground.", "Life fades from the innocent's eyes.", "Your urge to hunt recedes.",
            "The innocent recoils, then falls.", "You end an innocent life.", "Innocent screams fill the air.",
            "The innocent gasps, then goes limp.", "You cut short an innocent life.", "The innocent lies in a broken heap.",
            "Blood gushes from the innocent's chest.", "Eyes wide with shock, the innocent topples.",
            "Silently, the innocent expires.", "You wipe innocent blood from your face.", "You stop an innocent heart.",
            "The innocent crumples to the ground.", "Blood spouts from the innocent's mouth.",
            "Writhing, the innocent drops to the earth.", "An innocent heart beats no more.",
            "With a pitiful moan, the innocent dies.", "The innocent's mangled body lies still.",
            "Your face is covered with innocent blood.", "Your urge to hunt dissipates.", "You snuff out an innocent life.",
            "An innocent corpse lies at your feet.", "Blood spurts from the innocent's belly."
        };

        /// <summary>
        /// Covers enemy IDs from 0-146.
        /// </summary>
        readonly float[] fastMoveSpeeds = new float[]
        {
            6.5f, 5.5f, 3f, 8f, 6.75f, 7.25f, 8f, 5f, 7f, 8f, 4.5f, 4.5f, 5.75f, 7.5f, 7f, 6f, 6.5f, 3f, 3.5f, 3.5f, 7f, 5.25f,
            3.5f, 4f, 6f, 4f, 7f, 6f, 9f, 7.5f, 12f, 7f, 4f, 4.5f, 7.5f, 7f, 3f, 3f, 3f, 0, 9f, 3.5f, 4f, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            4f, 7f, 6f, 4f, 4f, 7.5f, 6.5f, 7.5f, 7.5f, 8f, 7.5f, 9f, 7f, 4.5f, 4.5f, 6.5f, 5.75f, 5f, 8f
        };

        /// <summary>
        /// Covers enemy IDs from 0-146.
        /// </summary>
        readonly float[] veryFastMoveSpeeds = new float[]
        {
            7f, 6f, 3f, 8.5f, 7.5f, 8f, 9f, 5f, 7.5f, 9f, 4.5f, 4.5f, 5.75f, 8f, 7.5f, 6f, 7f, 3f, 3.5f, 3.5f, 7.5f, 5.75f,
            4.5f, 4f, 6.5f, 5f, 7.5f, 6.5f, 10f, 7.5f, 12f, 7.5f, 4f, 4.5f, 8f, 7.5f, 3f, 3f, 3f, 0, 10f, 3.5f, 4f, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 4f, 7.5f, 6f, 4f, 4f, 8f, 7f, 8f, 8f, 8.5f, 8f, 10f, 7.5f, 4.5f, 4.5f, 7f, 5.75f, 5f, 8.5f
        };

        /// <summary>
        /// Covers enemy IDs from 0-38 (Rat to IceAtronach).
        /// </summary> 
        readonly byte[] bossfallMonsterSpecialHandling = new byte[]
        {
            0, 0, 1, 0, 0, 0, 0, 0, 0, 13, 0, 0, 0, 0, 13, 3, 0, 2, 5, 6, 4, 0, 7, 5, 0, 0, 11, 0, 12, 0, 12, 0, 12, 12, 0, 9,
            8, 2, 10
        };

        /// <summary>
        /// Represents Iron through Daedric.
        /// </summary>
        readonly short[] bossfallMaterialProbabilities = new short[]
        {
            327, 654, 8, 12, 8, 5, 4, 3, 2, 1
        };

        #endregion

        #region Properties

        public static BossfallOverrides Instance { get { return Bossfall.Instance.GetComponent<BossfallOverrides>(); } }

        public MobileEnemy[] BossfallEnemyStats { get { return bossfallEnemyStats; } }
        public short[] EnchantmentPointsByEnemyID { get { return enchantmentPointsByEnemyID; } }
        public int[] BossfallAlternateRandomTreasureIconIndices { get { return bossfallAlternateRandomTreasureIconIndices; } }

        public byte[] BossfallVampireHealAmounts { get { return bossfallVampireHealAmounts; } }
        public string[] BossfallVampireHUDMessages { get { return bossfallVampireHUDMessages; } }
        public string[] BossfallOldBloodHUDMessages { get { return bossfallOldBloodHUDMessages; } }

        public string[] BossfallInnocentHUDMessages { get { return bossfallInnocentHUDMessages; } }

        public float[] FastMoveSpeeds { get { return fastMoveSpeeds; } }
        public float[] VeryFastMoveSpeeds { get { return veryFastMoveSpeeds; } }

        #endregion

        #region Public Methods

        public void RegisterOverrides(Mod mod)
        {
            FormulaHelper.RegisterOverride<Func<int, int>>(mod, "DamageModifier", DamageModifier);
            FormulaHelper.RegisterOverride<Func<PlayerEntity, int, int>>(mod, "CalculateClimbingChance", CalculateClimbingChance);
            FormulaHelper.RegisterOverride<Func<Weapons, int>>(mod, "CalculateWeaponMinDamage", CalculateWeaponMinDamage);
            FormulaHelper.RegisterOverride<Func<DaggerfallEntity, DaggerfallEntity, bool, int, DaggerfallUnityItem, int>>
                (mod, "CalculateAttackDamage", CalculateAttackDamage);
            FormulaHelper.RegisterOverride<Func<DaggerfallEntity, DaggerfallEntity, int, int, DaggerfallUnityItem, int>>
                (mod, "CalculateWeaponAttackDamage", CalculateWeaponAttackDamage);
            FormulaHelper.RegisterOverride<Func<DaggerfallEntity, DaggerfallEntity, int, int>>
                (mod, "CalculateHandToHandAttackDamage", CalculateHandToHandAttackDamage);
            FormulaHelper.RegisterOverride<Func<int>>(mod, "CalculateStruckBodyPart", CalculateStruckBodyPart);
            FormulaHelper.RegisterOverride<Func<int, int, int>>(mod, "CalculateBackstabDamage", CalculateBackstabDamage);
            FormulaHelper.RegisterOverride<Func<DaggerfallEntity, DaggerfallEntity, int, DaggerfallUnityItem, int, bool>>
                (mod, "DamageEquipment", DamageEquipment);
            FormulaHelper.RegisterOverride<Func<DaggerfallUnityItem, DaggerfallEntity, int, bool>>
                (mod, "ApplyConditionDamageThroughPhysicalHit", ApplyConditionDamageThroughPhysicalHit);
            FormulaHelper.RegisterOverride<Action<EnemyEntity, DaggerfallEntity, int>>(mod, "OnMonsterHit", OnMonsterHit);
            FormulaHelper.RegisterOverride<Action<DaggerfallEntity, DaggerfallEntity, Poisons, bool>>
                (mod, "InflictPoison", InflictPoison);
            FormulaHelper.RegisterOverride<Func<int>>(mod, "RollRandomSpawn_LocationNight", RollRandomSpawn_LocationNight);
            FormulaHelper.RegisterOverride<Func<int>>(mod, "RollRandomSpawn_WildernessDay", RollRandomSpawn_WildernessDay);
            FormulaHelper.RegisterOverride<Func<int>>(mod, "RollRandomSpawn_WildernessNight", RollRandomSpawn_WildernessNight);
            FormulaHelper.RegisterOverride<Func<int>>(mod, "RollRandomSpawn_Dungeon", RollRandomSpawn_Dungeon);
            FormulaHelper.RegisterOverride<Func<int, int, int>>(mod, "CalculateBankLoanRepayment", CalculateBankLoanRepayment);
            FormulaHelper.RegisterOverride<Func<int, WeaponMaterialTypes>>(mod, "RandomMaterial", RandomMaterial);
            FormulaHelper.RegisterOverride<Func<int, ArmorMaterialTypes>>(mod, "RandomArmorMaterial", RandomArmorMaterial);
        }

        public int BossfallShieldArmorValue(DaggerfallUnityItem shield)
        {
            int shieldMaterialModifier;

            switch (shield.nativeMaterialValue)
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

            switch (shield.TemplateIndex)
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

        #endregion

        #region Private Methods

        bool IsPunch(FPSWeapon onscreenWeapon)
        {
            if (onscreenWeapon != null)
            {
                if (onscreenWeapon.WeaponState == WeaponStates.StrikeRight || onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft
                    || onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region FormulaHelper Method Overrides

        /// <summary>
        /// Formula is vanilla's, but I changed it to be an instance method and also replaced "5f" with "10f" to match
        /// scaling of other stats.
        /// </summary>
        /// <param name="strength">Entity's STR attribute.</param>
        /// <returns>Weapon attack damage modifier from -5 to +5.</returns>
        public int DamageModifier(int strength)
        {
            return (int)Mathf.Floor((float)(strength - 50) / 10f);
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="basePercentSuccess">I didn't use this in my new climbing skill checks.</param>
        /// <returns>Climbing chance.</returns>
        public int CalculateClimbingChance(PlayerEntity player, int basePercentSuccess)
        {
            int skill = player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing);
            int luck = player.Stats.GetLiveStatValue(DFCareer.Stats.Luck);
            if (player.Race == Races.Khajiit)
                skill += 30;
            if (player.IsEnhancedClimbing)
                skill *= 2;

            // I changed how luck affects the skill check and the clamp range.
            skill = Mathf.Clamp(skill, 1, 100);
            float luckFactor = Mathf.Lerp(-5, 5, luck * 0.01f);

            // Climbing success now depends almost entirely on your skill level.
            int chance = (int)(Mathf.Lerp(15, 100, skill * .01f) + luckFactor);

            return chance;
        }

        /// <summary>
        /// Formula is vanilla's, but I changed it to be an instance method and set all minimum damages to 1.
        /// </summary>
        /// <param name="weapon">Weapon type.</param>
        /// <returns>1 for every weapon.</returns>
        public int CalculateWeaponMinDamage(Weapons weapon)
        {
            switch (weapon)
            {
                case Weapons.Dagger:
                case Weapons.Tanto:
                case Weapons.Wakazashi:
                case Weapons.Shortsword:
                case Weapons.Broadsword:
                case Weapons.Staff:
                case Weapons.Mace:
                case Weapons.Longsword:
                case Weapons.Claymore:
                case Weapons.Battle_Axe:
                case Weapons.War_Axe:
                case Weapons.Flail:
                case Weapons.Saber:
                case Weapons.Katana:
                case Weapons.Dai_Katana:
                case Weapons.Warhammer:
                case Weapons.Short_Bow:
                case Weapons.Long_Bow:
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="attacker">Attacker.</param>
        /// <param name="target">Target.</param>
        /// <param name="isEnemyFacingAwayFromPlayer">If player is behind the target. False for all enemy attacks.</param>
        /// <param name="weaponAnimTime">How long the weapon animation lasted, in milliseconds.</param>
        /// <param name="weapon">The attacker's weapon, null if no weapon is being used.</param>
        /// <returns>Damage done to target, will be 0 if attack misses.</returns>
        public int CalculateAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, bool isEnemyFacingAwayFromPlayer, int weaponAnimTime, DaggerfallUnityItem weapon)
        {
            if (attacker == null || target == null)
                return 0;

            int damageModifiers = 0;
            int damage = 0;
            int chanceToHitMod = 0;
            int backstabChance = 0;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            short skillID = 0;

            // I added these variables.
            DaggerfallUnityItem gloves = player.ItemEquipTable.GetItem(EquipSlots.Gloves);
            DaggerfallUnityItem boots = player.ItemEquipTable.GetItem(EquipSlots.Feet);
            DaggerfallUnityItem poisonWeapon = attacker.ItemEquipTable.GetItem(EquipSlots.RightHand);
            EnemyEntity enemyTarget = target as EnemyEntity;
            EnemyEntity AIAttacker = attacker as EnemyEntity;

            // I modified this check to make human class enemies use Hand-to-Hand if that would be more
            // effective. This makes humans far more dangerous at higher levels.
            if (AIAttacker != null && weapon != null)
            {
                int weaponAverage = (weapon.GetBaseDamageMin() + weapon.GetBaseDamageMax()) / 2;
                int noWeaponAverage = (AIAttacker.MobileEnemy.MinDamage + AIAttacker.MobileEnemy.MaxDamage) / 2;
                int classNoWeaponAverage = (FormulaHelper.CalculateHandToHandMinDamage(AIAttacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand))
                    + FormulaHelper.CalculateHandToHandMaxDamage(AIAttacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand))) / 2;

                if (AIAttacker.EntityType == EntityTypes.EnemyMonster)
                {
                    if (noWeaponAverage > weaponAverage)
                    {
                        weapon = null;
                    }
                }
                else if (classNoWeaponAverage > weaponAverage)
                {
                    weapon = null;
                }
            }

            if (weapon != null)
            {
                skillID = weapon.GetWeaponSkillIDAsShort();
            }
            else
            {
                skillID = (short)DFCareer.Skills.HandToHand;
            }

            chanceToHitMod = attacker.Skills.GetLiveSkillValue(skillID);

            if (attacker == player)
            {
                FormulaHelper.ToHitAndDamageMods swingMods = FormulaHelper.CalculateSwingModifiers(GameManager.Instance.WeaponManager.ScreenWeapon);
                damageModifiers += swingMods.damageMod;
                chanceToHitMod += swingMods.toHitMod;
                FormulaHelper.ToHitAndDamageMods proficiencyMods = FormulaHelper.CalculateProficiencyModifiers(attacker, weapon);
                damageModifiers += proficiencyMods.damageMod;
                chanceToHitMod += proficiencyMods.toHitMod;
                FormulaHelper.ToHitAndDamageMods racialMods = FormulaHelper.CalculateRacialModifiers(attacker, weapon, player);
                damageModifiers += racialMods.damageMod;
                chanceToHitMod += racialMods.toHitMod;
                backstabChance = FormulaHelper.CalculateBackstabChance(player, null, isEnemyFacingAwayFromPlayer);
                chanceToHitMod += backstabChance;

                // This long list checks whether player's H2H attacks are punches or kicks and buffs
                // to-hit rolls according to what armor material is on player's hands or feet.
                if (skillID == (short)DFCareer.Skills.HandToHand
                    && GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType != WeaponTypes.Werecreature)
                {
                    if (IsPunch(GameManager.Instance.WeaponManager.ScreenWeapon))
                    {
                        if (gloves != null)
                        {
                            switch (gloves.nativeMaterialValue)
                            {
                                case (int)ArmorMaterialTypes.Iron:
                                    chanceToHitMod -= 10;
                                    break;
                                case (int)ArmorMaterialTypes.Elven:
                                    chanceToHitMod += 10;
                                    break;
                                case (int)ArmorMaterialTypes.Dwarven:
                                    chanceToHitMod += 20;
                                    break;
                                case (int)ArmorMaterialTypes.Mithril:
                                case (int)ArmorMaterialTypes.Adamantium:
                                    chanceToHitMod += 30;
                                    break;
                                case (int)ArmorMaterialTypes.Ebony:
                                    chanceToHitMod += 40;
                                    break;
                                case (int)ArmorMaterialTypes.Orcish:
                                    chanceToHitMod += 50;
                                    break;
                                case (int)ArmorMaterialTypes.Daedric:
                                    chanceToHitMod += 60;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    else if (boots != null)
                    {
                        switch (boots.nativeMaterialValue)
                        {
                            case (int)ArmorMaterialTypes.Iron:
                                chanceToHitMod -= 10;
                                break;
                            case (int)ArmorMaterialTypes.Elven:
                                chanceToHitMod += 10;
                                break;
                            case (int)ArmorMaterialTypes.Dwarven:
                                chanceToHitMod += 20;
                                break;
                            case (int)ArmorMaterialTypes.Mithril:
                            case (int)ArmorMaterialTypes.Adamantium:
                                chanceToHitMod += 30;
                                break;
                            case (int)ArmorMaterialTypes.Ebony:
                                chanceToHitMod += 40;
                                break;
                            case (int)ArmorMaterialTypes.Orcish:
                                chanceToHitMod += 50;
                                break;
                            case (int)ArmorMaterialTypes.Daedric:
                                chanceToHitMod += 60;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            int struckBodyPart = CalculateStruckBodyPart();

            if (skillID == (short)DFCareer.Skills.HandToHand)
            {
                if (attacker == player || (AIAttacker != null && AIAttacker.EntityType == EntityTypes.EnemyClass))
                {
                    if (FormulaHelper.CalculateSuccessfulHit(attacker, target, chanceToHitMod, struckBodyPart))
                    {
                        damage = CalculateHandToHandAttackDamage(attacker, target, damageModifiers);
                        damage = CalculateBackstabDamage(damage, backstabChance);
                    }
                }
                else if (AIAttacker != null)
                {
                    int minBaseDamage = 0;
                    int maxBaseDamage = 0;
                    int attackNumber = 0;
                    while (attackNumber < 3)
                    {
                        if (attackNumber == 0)
                        {
                            minBaseDamage = AIAttacker.MobileEnemy.MinDamage;
                            maxBaseDamage = AIAttacker.MobileEnemy.MaxDamage;
                        }
                        else if (attackNumber == 1)
                        {
                            minBaseDamage = AIAttacker.MobileEnemy.MinDamage2;
                            maxBaseDamage = AIAttacker.MobileEnemy.MaxDamage2;
                        }
                        else if (attackNumber == 2)
                        {
                            minBaseDamage = AIAttacker.MobileEnemy.MinDamage3;
                            maxBaseDamage = AIAttacker.MobileEnemy.MaxDamage3;
                        }

                        int reflexesChance = 50 - (10 * ((int)player.Reflexes - 2));
                        int hitDamage = 0;

                        if (DFRandom.rand() % 100 < reflexesChance && minBaseDamage > 0 && FormulaHelper.CalculateSuccessfulHit(attacker, target, chanceToHitMod, struckBodyPart))
                        {
                            hitDamage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + 1);

                            if (hitDamage > 0)
                                OnMonsterHit(AIAttacker, target, hitDamage);

                            damage += hitDamage;
                        }

                        if (hitDamage > 0)
                            damage += FormulaHelper.GetBonusOrPenaltyByEnemyType(attacker, target);

                        ++attackNumber;
                    }
                }
            }
            else if (weapon != null)
            {
                chanceToHitMod += FormulaHelper.CalculateWeaponToHit(weapon);
                chanceToHitMod = FormulaHelper.AdjustWeaponHitChanceMod(attacker, target, chanceToHitMod, weaponAnimTime, weapon);

                if (FormulaHelper.CalculateSuccessfulHit(attacker, target, chanceToHitMod, struckBodyPart))
                {
                    damage = CalculateWeaponAttackDamage(attacker, target, damageModifiers, weaponAnimTime, weapon);
                    damage = CalculateBackstabDamage(damage, backstabChance);
                }
            }

            // I moved poison formulas out of the weapon != null check. Enemies with poisoned weapons
            // now usually use Hand-to-Hand and I want their poison to be inflicted anyway, so I check
            // right-handed items for poison and apply that poison to the target.
            if (damage > 0 && poisonWeapon != null && poisonWeapon.poisonType != Poisons.None)
            {
                if (AIAttacker.MobileEnemy.ID == (int)MobileTypes.Assassin && GameManager.Instance.PlayerEntity.Level > 6)
                {
                    InflictPoison(attacker, target, poisonWeapon.poisonType, true);
                    poisonWeapon.poisonType = Poisons.None;
                }
                else
                {
                    InflictPoison(attacker, target, poisonWeapon.poisonType, false);
                    poisonWeapon.poisonType = Poisons.None;
                }
            }

            damage = Mathf.Max(0, damage);

            DamageEquipment(attacker, target, damage, weapon, struckBodyPart);

            // This damages player's gloves or boots dependent on whether H2H attack is a punch or kick.
            if (attacker == player && skillID == (short)DFCareer.Skills.HandToHand && damage > 0
             && GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType != WeaponTypes.Werecreature)
            {
                if (IsPunch(GameManager.Instance.WeaponManager.ScreenWeapon))
                {
                    if (gloves != null)
                    {
                        if (gloves.IsEnchanted)
                            gloves.LowerCondition(10, player, player.Items);
                        else
                            gloves.LowerCondition(10, player);
                    }
                }
                else if (boots != null)
                {
                    if (boots.IsEnchanted)
                        boots.LowerCondition(15, player, player.Items);
                    else
                        boots.LowerCondition(15, player);
                }
            }

            // Implements Bossfall's custom enemy weapon immunities, resistances, and weaknesses.
            if (attacker == player && damage > 0 && enemyTarget.MobileEnemy.ID < 39
             && GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType != WeaponTypes.Werecreature)
            {
                if (bossfallMonsterSpecialHandling[enemyTarget.MobileEnemy.ID] != 0)
                {
                    BossfallEnemyMotor motor = enemyTarget.EntityBehaviour.GetComponent<BossfallEnemyMotor>();

                    // This enormous switch goes through all possible enemy/weapon combinations.
                    switch (bossfallMonsterSpecialHandling[enemyTarget.MobileEnemy.ID])
                    {
                        // Spriggan
                        case 1:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.Axe:
                                    damage *= 2;
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Very effective!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.HandToHand:
                                    damage /= 4;
                                    GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 1);
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("Ow! Not very effective...");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.ShortBlade:
                                    damage /= 2;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(1, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(1, player);
                                    }
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You dull your blade.");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.LongBlade:
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(6, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(6, player);
                                    }
                                    if (!motor.ShownMsgFour)
                                    {
                                        DaggerfallUI.AddHUDText("You dull your blade.");
                                        motor.ShownMsgFour = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.BluntWeapon:
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage = 0;
                                    if (!motor.ShownMsgFive)
                                    {
                                        DaggerfallUI.AddHUDText("Ineffective.");
                                        motor.ShownMsgFive = true;
                                    }
                                    break;
                            }
                            break;

                        // Zombie, Flesh Atronach
                        case 2:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.Axe:
                                    damage *= 2;
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Very effective!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.LongBlade:
                                case (short)DFCareer.Skills.BluntWeapon:
                                    break;
                                case (short)DFCareer.Skills.HandToHand:
                                    damage /= 3;
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective...");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.ShortBlade:
                                    damage /= 2;
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective...");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage /= 4;
                                    if (!motor.ShownMsgFour)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective...");
                                        motor.ShownMsgFour = true;
                                    }
                                    break;
                            }
                            break;

                        // Skeletal Warrior
                        case 3:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.BluntWeapon:
                                    damage *= 2;
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Very effective!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Axe:
                                case (short)DFCareer.Skills.HandToHand:
                                    break;
                                case (short)DFCareer.Skills.LongBlade:
                                    damage /= 2;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(6, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(6, player);
                                    }
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You nick your blade.");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.ShortBlade:
                                    damage /= 3;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(2, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(2, player);
                                    }
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You chip your blade.");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage = 0;
                                    if (!motor.ShownMsgFour)
                                    {
                                        DaggerfallUI.AddHUDText("Ineffective.");
                                        motor.ShownMsgFour = true;
                                    }
                                    break;
                            }
                            break;

                        // Giant Scorpion
                        case 4:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.Axe:
                                    damage *= 2;
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Very effective!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.BluntWeapon:
                                case (short)DFCareer.Skills.LongBlade:
                                case (short)DFCareer.Skills.ShortBlade:
                                    break;
                                case (short)DFCareer.Skills.HandToHand:
                                    damage /= 2;
                                    GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 1);
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("Ow! Not very effective...");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage /= 3;
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective...");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                            }
                            break;

                        // Ghost, Wraith
                        case 5:
                            if (skillID != (short)DFCareer.Skills.HandToHand)
                            {
                                if (weapon.nativeMaterialValue == (int)WeaponMaterialTypes.Silver)
                                {
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Your Silver weapon strikes true!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                }
                                damage = 0;
                                if (!motor.ShownMsgTwo)
                                {
                                    DaggerfallUI.AddHUDText("Ineffective. Use Silver.");
                                    motor.ShownMsgTwo = true;
                                }
                                break;
                            }
                            else if (IsPunch(GameManager.Instance.WeaponManager.ScreenWeapon))
                            {
                                if (gloves != null && gloves.nativeMaterialValue == (int)ArmorMaterialTypes.Silver)
                                {
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Your Silver gauntlet strikes true!");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                }
                                damage = 0;
                                if (!motor.ShownMsgFour)
                                {
                                    DaggerfallUI.AddHUDText("Ineffective. Use Silver.");
                                    motor.ShownMsgFour = true;
                                }
                                break;
                            }
                            else if (boots != null && boots.nativeMaterialValue == (int)ArmorMaterialTypes.Silver)
                            {
                                if (!motor.ShownMsgFive)
                                {
                                    DaggerfallUI.AddHUDText("Your Silver boot strikes true!");
                                    motor.ShownMsgFive = true;
                                }
                                break;
                            }
                            damage = 0;
                            if (!motor.ShownMsgSix)
                            {
                                DaggerfallUI.AddHUDText("Ineffective. Use Silver.");
                                motor.ShownMsgSix = true;
                            }
                            break;

                        // Mummy
                        case 6:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.Axe:
                                case (short)DFCareer.Skills.LongBlade:
                                    damage *= 2;
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Very effective!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.BluntWeapon:
                                case (short)DFCareer.Skills.HandToHand:
                                case (short)DFCareer.Skills.ShortBlade:
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage /= 2;
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective...");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                            }
                            break;

                        // Gargoyle
                        case 7:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.BluntWeapon:
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(6, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(6, player);
                                    }
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("You dent your weapon.");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.HandToHand:
                                    damage = 0;
                                    GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 2);
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("OW! Ineffective.");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Axe:
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(12, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(12, player);
                                    }
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("You chip your axe.");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.LongBlade:
                                    damage /= 2;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(18, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(18, player);
                                    }
                                    if (!motor.ShownMsgFour)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You crack your blade.");
                                        motor.ShownMsgFour = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.ShortBlade:
                                    damage /= 3;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(3, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(3, player);
                                    }
                                    if (!motor.ShownMsgFive)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You crack your blade.");
                                        motor.ShownMsgFive = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage = 0;
                                    if (!motor.ShownMsgSix)
                                    {
                                        DaggerfallUI.AddHUDText("Ineffective.");
                                        motor.ShownMsgSix = true;
                                    }
                                    break;
                            }
                            break;

                        // Iron Atronach
                        case 8:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.BluntWeapon:
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(12, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(12, player);
                                    }
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("You chip your weapon.");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.HandToHand:
                                    damage = 0;
                                    GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 3);
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("OW!! Ineffective.");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Axe:
                                    damage /= 2;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(18, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(18, player);
                                    }
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You crack your axe.");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.LongBlade:
                                    damage /= 3;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(24, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(24, player);
                                    }
                                    if (!motor.ShownMsgFour)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You badly damage your blade!");
                                        motor.ShownMsgFour = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.ShortBlade:
                                    damage /= 4;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(4, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(4, player);
                                    }
                                    if (!motor.ShownMsgFive)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You badly damage your blade!");
                                        motor.ShownMsgFive = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage = 0;
                                    if (!motor.ShownMsgSix)
                                    {
                                        DaggerfallUI.AddHUDText("Ineffective.");
                                        motor.ShownMsgSix = true;
                                    }
                                    break;
                            }
                            break;

                        // Fire Atronach
                        case 9:
                            if (skillID == (short)DFCareer.Skills.HandToHand)
                            {
                                if (IsPunch(GameManager.Instance.WeaponManager.ScreenWeapon))
                                {
                                    GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 2);
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("You burn your hand.");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                }
                                GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 2);
                                if (!motor.ShownMsgTwo)
                                {
                                    DaggerfallUI.AddHUDText("You burn your foot.");
                                    motor.ShownMsgTwo = true;
                                }
                                break;
                            }
                            break;

                        // Ice Atronach
                        case 10:
                            switch (skillID)
                            {
                                case (short)DFCareer.Skills.Axe:
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(6, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(6, player);
                                    }
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("You nick your axe.");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.BluntWeapon:
                                    break;
                                case (short)DFCareer.Skills.HandToHand:
                                    damage = 0;
                                    GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 2);
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("OW! Ineffective.");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.LongBlade:
                                    damage /= 2;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(12, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(12, player);
                                    }
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You chip your blade.");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.ShortBlade:
                                    damage /= 3;
                                    if (weapon.IsEnchanted)
                                    {
                                        weapon.LowerCondition(2, player, player.Items);
                                    }
                                    else
                                    {
                                        weapon.LowerCondition(2, player);
                                    }
                                    if (!motor.ShownMsgFour)
                                    {
                                        DaggerfallUI.AddHUDText("Not very effective... You chip your blade.");
                                        motor.ShownMsgFour = true;
                                    }
                                    break;
                                case (short)DFCareer.Skills.Archery:
                                    damage = 0;
                                    if (!motor.ShownMsgFive)
                                    {
                                        DaggerfallUI.AddHUDText("Ineffective.");
                                        motor.ShownMsgFive = true;
                                    }
                                    break;
                            }
                            break;

                        // Fire Daedra
                        case 11:
                            if (skillID == (short)DFCareer.Skills.HandToHand)
                            {
                                if (IsPunch(GameManager.Instance.WeaponManager.ScreenWeapon))
                                {
                                    GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 4);
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("You scorch your hand!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                }
                                GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", 4);
                                if (!motor.ShownMsgTwo)
                                {
                                    DaggerfallUI.AddHUDText("You scorch your foot!");
                                    motor.ShownMsgTwo = true;
                                }
                                break;
                            }
                            break;

                        // Vampire, Vampire Ancient, Lich, Ancient Lich
                        case 12:
                            if (skillID != (short)DFCareer.Skills.HandToHand)
                            {
                                if (weapon.nativeMaterialValue == (int)WeaponMaterialTypes.Silver)
                                {
                                    damage *= 2;
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Your Silver weapon strikes true!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                }
                                break;
                            }
                            else if (IsPunch(GameManager.Instance.WeaponManager.ScreenWeapon))
                            {
                                if (gloves != null && gloves.nativeMaterialValue == (int)ArmorMaterialTypes.Silver)
                                {
                                    damage *= 2;
                                    if (!motor.ShownMsgTwo)
                                    {
                                        DaggerfallUI.AddHUDText("Your Silver gauntlet strikes true!");
                                        motor.ShownMsgTwo = true;
                                    }
                                    break;
                                }
                                break;
                            }
                            else if (boots != null && boots.nativeMaterialValue == (int)ArmorMaterialTypes.Silver)
                            {
                                damage *= 2;
                                if (!motor.ShownMsgThree)
                                {
                                    DaggerfallUI.AddHUDText("Your Silver boot strikes true!");
                                    motor.ShownMsgThree = true;
                                }
                                break;
                            }
                            break;

                        // Werewolf, Wereboar
                        case 13:
                            if (skillID != (short)DFCareer.Skills.HandToHand)
                            {
                                if (weapon.nativeMaterialValue == (int)WeaponMaterialTypes.Silver)
                                {
                                    if (!motor.ShownMsg)
                                    {
                                        DaggerfallUI.AddHUDText("Your Silver weapon strikes true!");
                                        motor.ShownMsg = true;
                                    }
                                    break;
                                }
                                damage /= 2;
                                if (!motor.ShownMsgTwo)
                                {
                                    DaggerfallUI.AddHUDText("Not very effective... Use Silver.");
                                    motor.ShownMsgTwo = true;
                                }
                                break;
                            }
                            else if (IsPunch(GameManager.Instance.WeaponManager.ScreenWeapon))
                            {
                                if (gloves != null && gloves.nativeMaterialValue == (int)ArmorMaterialTypes.Silver)
                                {
                                    if (!motor.ShownMsgThree)
                                    {
                                        DaggerfallUI.AddHUDText("Your Silver gauntlet strikes true!");
                                        motor.ShownMsgThree = true;
                                    }
                                    break;
                                }
                                damage /= 2;
                                if (!motor.ShownMsgFour)
                                {
                                    DaggerfallUI.AddHUDText("Not very effective... Use Silver.");
                                    motor.ShownMsgFour = true;
                                }
                                break;
                            }
                            else if (boots != null && boots.nativeMaterialValue == (int)ArmorMaterialTypes.Silver)
                            {
                                if (!motor.ShownMsgFive)
                                {
                                    DaggerfallUI.AddHUDText("Your Silver boot strikes true!");
                                    motor.ShownMsgFive = true;
                                }
                                break;
                            }
                            damage /= 2;
                            if (!motor.ShownMsgSix)
                            {
                                DaggerfallUI.AddHUDText("Not very effective... Use Silver.");
                                motor.ShownMsgSix = true;
                            }
                            break;
                    }
                }
            }

            if (target == player)
            {
                DaggerfallUnityItem[] equippedItems = target.ItemEquipTable.EquipTable;
                DaggerfallUnityItem item = null;

                if (equippedItems.Length != 0)
                {
                    if (IsRingOfNamira(equippedItems[(int)EquipSlots.Ring0]) || IsRingOfNamira(equippedItems[(int)EquipSlots.Ring1]))
                    {
                        IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(RingOfNamiraEffect.EffectKey);
                        effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.None,
                            targetEntity: AIAttacker.EntityBehaviour,
                            sourceItem: item,
                            sourceDamage: damage);
                    }
                }
            }

            return damage;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. It's private in FormulaHelper which doesn't make sense to me
        /// considering the formula that calls it (CalculateAttackDamage) is overridable. So, I copied the formula here.
        /// </summary>
        /// <param name="item">The ring being checked.</param>
        /// <returns>True if item is the Ring Of Namira.</returns>
        bool IsRingOfNamira(DaggerfallUnityItem item)
        {
            return item != null && item.ContainsEnchantment(DaggerfallConnect.FallExe.EnchantmentTypes.SpecialArtifactEffect, (int)ArtifactsSubTypes.Ring_of_Namira);
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="attacker">Attacking entity.</param>
        /// <param name="target">Target entity.</param>
        /// <param name="damageModifier">Damage modifiers.</param>
        /// <param name="weaponAnimTime">Weapon animation time.</param>
        /// <param name="weapon">The weapon used.</param>
        /// <returns>Adjusted weapon attack damage.</returns>
        public int CalculateWeaponAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, int damageModifier, int weaponAnimTime, DaggerfallUnityItem weapon)
        {
            // I added this variable.
            EnemyEntity AIAttacker = attacker as EnemyEntity;

            int damage = UnityEngine.Random.Range(weapon.GetBaseDamageMin(), weapon.GetBaseDamageMax() + 1) + damageModifier;

            // This addition is a part of Bossfall's increased difficulty. Enemy classes deal
            // more damage as long as they're using a weapon (class enemies switch to H2H around level 5).
            if (AIAttacker != null && AIAttacker.EntityType == EntityTypes.EnemyClass)
            {
                damage = UnityEngine.Random.Range(weapon.GetBaseDamageMin(), weapon.GetBaseDamageMax() + (attacker.Level * 2) + 1) + damageModifier;
            }

            damage += DamageModifier(attacker.Stats.LiveStrength);
            damage += weapon.GetWeaponMaterialModifier();

            if (damage < 1)
                damage = 0;

            damage += FormulaHelper.GetBonusOrPenaltyByEnemyType(attacker, target);
            damage = FormulaHelper.AdjustWeaponAttackDamage(attacker, target, damage, weaponAnimTime, weapon);

            return damage;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made. The original
        /// formula had an extra "bool player" parameter but the TryGetOverride method didn't include a bool, so I deleted
        /// the "bool player" parameter to make sure this formula correctly overrides vanilla's.
        /// </summary>
        /// <param name="attacker">Attacking entity.</param>
        /// <param name="target">Target entity.</param>
        /// <param name="damageModifier">Damage modifiers.</param>
        /// <returns>Adjusted Hand-to-Hand damage.</returns>
        public int CalculateHandToHandAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, int damageModifier)
        {
            // I added this variable.
            EnemyEntity AIAttacker = attacker as EnemyEntity;

            int minBaseDamage = FormulaHelper.CalculateHandToHandMinDamage(attacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
            int maxBaseDamage = FormulaHelper.CalculateHandToHandMaxDamage(attacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
            int damage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + 1);

            // This addition is a key part of Bossfall's increased difficulty. Enemy classes dish out
            // much more damage at high levels. Note the Assassin exclusion - their damage would be ridiculous.
            if (AIAttacker != null && AIAttacker.EntityType == EntityTypes.EnemyClass
                && AIAttacker.MobileEnemy.ID != (int)MobileTypes.Assassin)
            {
                damage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + (attacker.Level * 2) + 1);
            }

            damage += damageModifier;

            // I removed the player-only check, now high STR benefits both player and enemies.
            damage += DamageModifier(attacker.Stats.LiveStrength);

            damage += FormulaHelper.GetBonusOrPenaltyByEnemyType(attacker, target);

            return damage;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <returns>Which body part was struck.</returns>
        public int CalculateStruckBodyPart()
        {
            // Vanilla DFU weights landed attacks toward chest armor, which causes body armor to wear out at a
            // roughly uniform rate as chest armor has much greater durability than other armor pieces. I want Pauldrons,
            // Boots, and Helms to wear out quicker but I don't want to change armor durability values to do this
            // (to maintain continuity with "Roleplay & Realism: Items" armor), so a workaround is this non-weighted array.
            int[] bodyParts = { 0, 1, 2, 3, 4, 5, 6 };
            return bodyParts[UnityEngine.Random.Range(0, bodyParts.Length)];
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="damage">Attack damage.</param>
        /// <param name="backstabbingLevel">Backstabbing skill level.</param>
        /// <returns>Attack damage *3 if Backstabbing check succeeds.</returns>
        public int CalculateBackstabDamage(int damage, int backstabbingLevel)
        {
            // I moved the backstabbingLevel check from > 1 to > 0 as I set miscellaneous
            // skills to start at 1-4. I want a Backstab (however unlikely) to be possible at a 
            // Backstabbing skill level of 1.
            if (backstabbingLevel > 0 && Dice100.SuccessRoll(backstabbingLevel))
            {
                damage *= 3;
                string backstabMessage = TextManager.Instance.GetLocalizedText("successfulBackstab");
                DaggerfallUI.Instance.PopupMessage(backstabMessage);
            }
            return damage;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="attacker">Attacking entity.</param>
        /// <param name="target">Target entity.</param>
        /// <param name="damage">Attack damage.</param>
        /// <param name="weapon">Weapon used.</param>
        /// <param name="struckBodyPart">Body part struck.</param>
        /// <returns>True. Vanilla's method is void but override must be a bool and return true to function properly.</returns>
        public bool DamageEquipment(DaggerfallEntity attacker, DaggerfallEntity target, int damage, DaggerfallUnityItem weapon, int struckBodyPart)
        {
            // I moved the weapon != null check - armor is now damaged by weaponless attacks.
            if (damage > 0)
            {
                if (weapon != null)
                {
                    ApplyConditionDamageThroughPhysicalHit(weapon, attacker, damage);
                }

                DaggerfallUnityItem shield = target.ItemEquipTable.GetItem(EquipSlots.LeftHand);
                bool shieldTakesDamage = false;

                if (shield != null)
                {
                    BodyParts[] protectedBodyParts = shield.GetShieldProtectedBodyParts();

                    for (int i = 0; (i < protectedBodyParts.Length) && !shieldTakesDamage; i++)
                    {
                        if (protectedBodyParts[i] == (BodyParts)struckBodyPart)
                            shieldTakesDamage = true;
                    }
                }

                if (shieldTakesDamage)
                    ApplyConditionDamageThroughPhysicalHit(shield, target, damage);
                else
                {
                    EquipSlots hitSlot = DaggerfallUnityItem.GetEquipSlotForBodyPart((BodyParts)struckBodyPart);
                    DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
                    if (armor != null)
                        ApplyConditionDamageThroughPhysicalHit(armor, target, damage);
                }
            }

            // The TryGetOverride method requires this method to be a bool and to return true for this method to
            // properly override vanilla's. I assume the delegate Func used doesn't support void methods, but I'm only guessing.
            // Regardless, I added the return true statement to make this override function properly.
            return true;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="item">The item being damaged.</param>
        /// <param name="owner">Used for removing broken magic items.</param>
        /// <param name="damage">Attack damage. I don't use this.</param>
        /// <returns>True. Vanilla's method is void but override must be a bool and return true to function properly.</returns>
        public bool ApplyConditionDamageThroughPhysicalHit(DaggerfallUnityItem item, DaggerfallEntity owner, int damage)
        {
            // I added these variables.
            short skillID = item.GetWeaponSkillIDAsShort();
            int amount;

            // I want Bossfall to work well with popular existing mods, which is why I make such oddly
            // specific changes to this method. Roleplay & Realism: Items sets its added armor item durabilities
            // (this I can't change), so in order to work well with RPR:I I need to vary equipment damage if I want
            // my nerfs to equipment durability to be consistent with RPR:I's item durabilities. I can't nerf vanilla
            // vanilla armor durabilities as then regular armor would have far less durability than RPR:I's added armor,
            // so as a workaround I vary damage based on weapon skill used.
            if (skillID == (short)DFCareer.Skills.ShortBlade || skillID == (short)DFCareer.Skills.Archery)
                amount = 1;
            else if (skillID == (short)DFCareer.Skills.LongBlade || skillID == (short)DFCareer.Skills.Axe
                     || skillID == (short)DFCareer.Skills.BluntWeapon)
                amount = 6;
            else
                amount = 50;

            if (item.IsEnchanted && owner is PlayerEntity)
                item.LowerCondition(amount, owner, (owner as PlayerEntity).Items);
            else
                item.LowerCondition(amount, owner);

            // The TryGetOverride method requires this method to be a bool and to return true for this method to
            // properly override vanilla's. I assume the delegate Func used doesn't support void methods, but I'm only guessing.
            // Regardless, I added the return true statement to make this override function properly.
            return true; 
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="attacker">Attacker.</param>
        /// <param name="target">Target.</param>
        /// <param name="damage">Weapon damage from the attack.</param>
        public void OnMonsterHit(EnemyEntity attacker, DaggerfallEntity target, int damage)
        {
            Diseases[] diseaseListA = { Diseases.Plague };
            Diseases[] diseaseListB = { Diseases.Plague, Diseases.StomachRot, Diseases.BrainFever };
            Diseases[] diseaseListC = {
                Diseases.Plague, Diseases.YellowFever, Diseases.StomachRot, Diseases.Consumption,
                Diseases.BrainFever, Diseases.SwampRot, Diseases.Cholera, Diseases.Leprosy, Diseases.RedDeath,
                Diseases.TyphoidFever, Diseases.Dementia
            };

            float random;

            switch (attacker.CareerIndex)
            {

                case (int)MonsterCareers.Rat:

                    // I liked the idea of Rats only transmitting the Plague.
                    if (Dice100.SuccessRoll(5))
                        FormulaHelper.InflictDisease(attacker, target, diseaseListA);
                    break;
                case (int)MonsterCareers.GiantBat:

                    // I liked the 5% chance of disease. I doubt Giant Bats brush their teeth.
                    if (Dice100.SuccessRoll(5))
                        FormulaHelper.InflictDisease(attacker, target, diseaseListB);
                    break;

                // On UESP's Skeletal Warrior page it says they have a 2% chance of transmitting disease.
                // I liked that idea. As a side note, this is where Bossfall began - this new disease chance for
                // Skeletal Warriors was the first thing I added.
                case (int)MonsterCareers.SkeletalWarrior:
                    if (Dice100.SuccessRoll(2))
                        FormulaHelper.InflictDisease(attacker, target, diseaseListC);
                    break;

                // I never understood why Spiders and Scorpions paralyzed rather than poisoned. I know
                // that's consistent with TES I: Arena but it still didn't make sense... On every hit from a Spider
                // or Scorpion there's a 10% chance you'll get poisoned. There is a tiny 0.1% chance/hit of poison
                // being Drothweed that bypasses Poison Immunity. This accomplishes two goals. First, Drothweed will
                // likely kill you unless you have a Cure Poison spell or potion. This encourages the player to always
                // be prepared, and you must be to survive Bossfall. Second, it makes Poison Immunity a bit less OP.
                case (int)MonsterCareers.Spider:
                case (int)MonsterCareers.GiantScorpion:
                    if (Dice100.SuccessRoll(10))
                    {
                        int roll = Dice100.Roll();

                        if (roll > 0)
                        {
                            if (roll > 9)
                            {
                                if (roll > 18)
                                {
                                    if (roll > 27)
                                    {
                                        if (roll > 36)
                                        {
                                            if (roll > 45)
                                            {
                                                if (roll > 54)
                                                {
                                                    if (roll > 63)
                                                    {
                                                        if (roll > 72)
                                                        {
                                                            if (roll > 81)
                                                            {
                                                                if (roll > 90)
                                                                {
                                                                    if (roll > 99)
                                                                    {
                                                                        InflictPoison(attacker, target, Poisons.Drothweed, true);
                                                                    }
                                                                    else
                                                                    {
                                                                        InflictPoison(attacker, target, Poisons.Arsenic, false);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    InflictPoison(attacker, target, Poisons.Moonseed, false);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                InflictPoison(attacker, target, Poisons.Nux_Vomica, false);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            InflictPoison(attacker, target, Poisons.Somnalius, false);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        InflictPoison(attacker, target, Poisons.Pyrrhic_Acid, false);
                                                    }
                                                }
                                                else
                                                {
                                                    InflictPoison(attacker, target, Poisons.Magebane, false);
                                                }
                                            }
                                            else
                                            {
                                                InflictPoison(attacker, target, Poisons.Thyrwort, false);
                                            }
                                        }
                                        else
                                        {
                                            InflictPoison(attacker, target, Poisons.Indulcet, false);
                                        }
                                    }
                                    else
                                    {
                                        InflictPoison(attacker, target, Poisons.Sursum, false);
                                    }
                                }
                                else
                                {
                                    InflictPoison(attacker, target, Poisons.Quaesto_Vil, false);
                                }
                            }
                            else
                            {
                                InflictPoison(attacker, target, Poisons.Aegrotat, false);
                            }
                        }
                    }
                    break;
                case (int)MonsterCareers.Werewolf:
                    random = UnityEngine.Random.Range(0f, 100f);
                    if (random <= FormulaHelper.specialInfectionChance && target.EntityBehaviour.EntityType == EntityTypes.Player)
                    {
                        EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Werewolf);
                        GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                        Debug.Log("Player infected by werewolf.");
                    }
                    break;
                case (int)MonsterCareers.Nymph:
                    FormulaHelper.FatigueDamage(attacker, target, damage);
                    break;
                case (int)MonsterCareers.Wereboar:
                    random = UnityEngine.Random.Range(0f, 100f);
                    if (random <= FormulaHelper.specialInfectionChance && target.EntityBehaviour.EntityType == EntityTypes.Player)
                    {
                        EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Wereboar);
                        GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                        Debug.Log("Player infected by wereboar.");
                    }
                    break;
                case (int)MonsterCareers.Zombie:

                    // I bumped up Zombie disease chance to 5%. Zombies don't look very clean.
                    if (Dice100.SuccessRoll(5))
                        FormulaHelper.InflictDisease(attacker, target, diseaseListC);
                    break;

                // Why would a dusty old Mummy be more infectious than a dirty Zombie? I reduced disease chance to 2%.
                case (int)MonsterCareers.Mummy:
                    if (Dice100.SuccessRoll(2))
                        FormulaHelper.InflictDisease(attacker, target, diseaseListC);
                    break;
                case (int)MonsterCareers.Vampire:
                case (int)MonsterCareers.VampireAncient:
                    random = UnityEngine.Random.Range(0f, 100f);
                    if (random <= FormulaHelper.specialInfectionChance && target.EntityBehaviour.EntityType == EntityTypes.Player)
                    {
                        EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismDisease();
                        GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                        Debug.Log("Player infected by vampire.");
                    }
                    else if (random <= 2.0f)
                    {
                        // In vanilla DFU Vampires/Vampire Ancients could only transmit the Plague. I don't know if that's
                        // how it worked in classic or if it was a mistake, but either way they now give you any disease.
                        FormulaHelper.InflictDisease(attacker, target, diseaseListC);
                    }
                    break;
                case (int)MonsterCareers.Lamia:
                    FormulaHelper.FatigueDamage(attacker, target, damage);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="attacker">Source of poison.</param>
        /// <param name="target">Entity to be poisoned.</param>
        /// <param name="poisonType">What poison will be inflicted.</param>
        /// <param name="bypassImmunity">Whether poison should bypass Poison Immunity.</param>
        public void InflictPoison(DaggerfallEntity attacker, DaggerfallEntity target, Poisons poisonType, bool bypassImmunity)
        {
            EntityEffectManager effectManager = null;
            if (target.EntityBehaviour != null)
            {
                effectManager = target.EntityBehaviour.GetComponent<EntityEffectManager>();

                if (effectManager == null)
                    return;
            }
            else
            {
                return;
            }

            // If bypassImmunity is true the immunity checks are skipped, ensuring the poison will pierce
            // Poison Immunity. If bypassImmunity is false, Poison Immunity will block every poison, just like vanilla DFU.
            if (!bypassImmunity)
            {
                DFCareer.Tolerance toleranceFlags = target.Career.Poison;
                if (toleranceFlags == DFCareer.Tolerance.Immune)
                    return;
                if (target is PlayerEntity)
                {
                    RaceTemplate raceTemplate = (target as PlayerEntity).GetLiveRaceTemplate();
                    if ((raceTemplate.ImmunityFlags & DFCareer.EffectFlags.Poison) == DFCareer.EffectFlags.Poison)
                        return;
                }
            }
            if (bypassImmunity || FormulaHelper.SavingThrow(DFCareer.Elements.DiseaseOrPoison, DFCareer.EffectFlags.Poison, target, 0) != 0)
            {
                if (target.Level != 1)
                {
                    EntityEffectBundle bundle = effectManager.CreatePoison(poisonType);
                    effectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);
                }
            }
            else
            {
                Debug.LogFormat("Poison resisted by {0}.", target.EntityBehaviour.name);
            }
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <returns>1, so it doesn't generate a vanilla random spawn.</returns>
        public int RollRandomSpawn_LocationNight()
        {
            // I don't want to use vanilla's random spawn methods. By returning 1, vanilla will never create
            // a FoeSpawner for random encounters. Instead, if I roll a 0 I create a FoeSpawner that mimics vanilla spawn
            // behavior but uses Bossfall's expanded encounter tables.
            if (UnityEngine.Random.Range(0, 40 + 1) == 0)
            {
                GameObjectHelper.CreateFoeSpawner(true, BossfallEncounterTables.Instance.ChooseRandomEnemy(false), 1, 10);
            }

            return 1;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <returns>1, so it doesn't generate a vanilla random spawn.</returns>
        public int RollRandomSpawn_WildernessDay()
        {
            // I don't want to use vanilla's random spawn methods. By returning 1, vanilla will never create
            // a FoeSpawner for random encounters. Instead, if I roll a 0 I create a FoeSpawner that mimics vanilla spawn
            // behavior but uses Bossfall's expanded encounter tables.
            if (UnityEngine.Random.Range(0, 40 + 1) == 0)
            {
                GameObjectHelper.CreateFoeSpawner(true, BossfallEncounterTables.Instance.ChooseRandomEnemy(false), 1, 10);
            }

            return 1;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <returns>1, so it doesn't generate a vanilla random spawn.</returns>
        public int RollRandomSpawn_WildernessNight()
        {
            // I don't want to use vanilla's random spawn methods. By returning 1, vanilla will never create
            // a FoeSpawner for random encounters. Instead, if I roll a 0 I create a FoeSpawner that mimics vanilla spawn
            // behavior but uses Bossfall's expanded encounter tables.
            if (UnityEngine.Random.Range(0, 40 + 1) == 0)
            {
                GameObjectHelper.CreateFoeSpawner(true, BossfallEncounterTables.Instance.ChooseRandomEnemy(false), 1, 10);
            }

            return 1;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <returns>1, so it doesn't generate a vanilla random spawn.</returns>
        public int RollRandomSpawn_Dungeon()
        {
            if (GameManager.Instance.PlayerEntity.EnemyAlertActive)
            {
                // I don't want to use vanilla's random spawn methods. By returning 1, vanilla will never create
                // a FoeSpawner for random encounters. Instead, if I roll a 0 I create a FoeSpawner that mimics vanilla spawn
                // behavior but uses Bossfall's expanded encounter tables.
                if (UnityEngine.Random.Range(0, 40 + 1) == 0)
                {
                    GameObjectHelper.CreateFoeSpawner(false, BossfallEncounterTables.Instance.ChooseRandomEnemy(false), 1, 8);
                }
            }

            return 1;
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="amount">Amount of the loan.</param>
        /// <param name="regionIndex">The current region index.</param>
        /// <returns>The amount of the loan plus interest.</returns>
        public int CalculateBankLoanRepayment(int amount, int regionIndex)
        {
            // I bumped loan interest from 10% to 20%. Seemed more reasonable.
            return (int)(amount + amount * .2);
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="enemyLevelModifier">Enemy level * 50</param>
        /// <returns>WeaponMaterialTypes value of material selected.</returns>
        public WeaponMaterialTypes RandomMaterial(int enemyLevelModifier)
        {
            // I changed this line. The enemyLevelModifier is (enemy level * 50).
            int levelModifier = enemyLevelModifier - 750;

            // Without this check, enemies below level 15 would never drop anything but Iron or Steel.
            if (levelModifier < 0)
                levelModifier = 0;

            // I increased the maximum range from vanilla's 256 to 1024.
            int randomModifier = UnityEngine.Random.Range(0, 1024 + 1);

            int combinedModifiers = levelModifier + randomModifier;

            // I increased the clamp range from 256 to 1024.
            combinedModifiers = Mathf.Clamp(combinedModifiers, 0, 1024);

            int material = 0;

            // I changed this "while" to use an array from this script.
            while (bossfallMaterialProbabilities[material] < combinedModifiers)
            {
                combinedModifiers -= bossfallMaterialProbabilities[material++];
            }

            return (WeaponMaterialTypes)(material);
        }

        /// <summary>
        /// Formula is vanilla's, changed to an instance method. Comments precede changes or additions I made.
        /// </summary>
        /// <param name="playerLevel">Enemy level * 50</param>
        /// <returns>ArmorMaterialTypes value of material selected.</returns>
        public ArmorMaterialTypes RandomArmorMaterial(int playerLevel)
        {
            int roll = Dice100.Roll();
            if (roll > 70)
            {
                // In vanilla DFU if (roll >= 90) it returns plate, which means an 11% chance for
                // plate. 10% is the chance I've seen in various sources - I don't know if 11% is what classic
                // actually uses or if 11% is a mistake. Regardless, Bossfall has a 10% chance for plate.
                if (roll > 90)
                {
                    WeaponMaterialTypes plateMaterial = FormulaHelper.RandomMaterial(playerLevel);
                    return (ArmorMaterialTypes)(0x0200 + plateMaterial);
                }
                else
                    return ArmorMaterialTypes.Chain;
            }
            else
                return ArmorMaterialTypes.Leather;
        }

        #endregion
    }
}
