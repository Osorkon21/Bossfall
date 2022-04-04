Project:         Bossfall
Copyright:       Copyright (C) 2022 Osorkon
License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
Source Code:     https://github.com/Osorkon21/Bossfall
Original Author: Osorkon
Contact Info:    https://forums.dfworkshop.net/viewtopic.php?f=27&t=5221, https://www.nexusmods.com/daggerfallunity/mods/260


Thank you for downloading Bossfall! I hope you enjoy your time spent in the Iliac Bay. You will face daunting challenges, but 
with great trials come great rewards. Bossfall's new features will either aid or hinder you in your struggle to stay alive.


README SECTIONS:

    RECOMMENDED READING:

        SETUP INSTRUCTIONS
        MOD COMPATIBILITY
        SUMMARY OF CHANGES


    ADDITIONAL INFO:

        PROJECT HISTORY
        v1.3.1 CHANGELOG
        v1.3.1 FORMULAS & TABLES
        v1.3.1 ITEM TABLES
        CREDITS & THANKS
        LEGAL INFO


SETUP INSTRUCTIONS:

    - These instructions are for Windows, they may vary slightly depending on your OS.


        REQUIRED:

            1. Extract contents of this folder, its ending location doesn't matter.
            2. Put the bossfall.dfmod file in DaggerfallUnity's StreamingAssets/Mods folder.
            3. Check out Bossfall's Settings page when you start up DFU, I suggest using the Recommended preset.


            OPTIONAL:

                - This will remove Assassin bosses from easy non-dungeon vanilla quests. It won't change main storyline quests.

                1. Right-click on the StreamingAssets folder included in this folder, select COPY.
                2. Right-click on DaggerfallUnity's StreamingAssets folder, PASTE Bossfall's StreamingAssets folder here.
                3. Select YES when prompted to overwrite files.


MOD COMPATIBILITY:

    - I recommend placing Bossfall at the bottom of your mod list.

    - If Bossfall is at the bottom of your mod list it may override some features of the following mods, and vice versa. Mods
      with the most features that may be overridden are listed first:

        Meaner Monsters
        Unleveled Loot
        Physical Combat & Armor Overhaul
        Roleplay & Realism: Items
        Unleveled Mobs and Quests
        Roleplay & Realism
        Better Default Classes
        Shield Module?          (untested)
        Combat Overhaul Alpha?  (untested)
        Dodge Mod?              (untested)


SUMMARY OF CHANGES:

    BOSSES:

        - Bosses are rare. They are extremely dangerous and usually drop excellent loot.

        - Orc Warlords, Vampires, Ancient Vampires, Daedra Lords, Liches, Ancient Liches, Assassins, and a rare type of
          Dragonling are bosses.

        - Boss Dragonlings, Assassins, Vampires, and Ancient Vampires move incredibly fast.

        - Assassins become bosses once you hit level 7 - at this point, their poisoned weapon will pierce your Poison Immunity.

        - Enable the "Boss Proximity Warning" setting to be shown a HUD message whenever a boss is nearby. You won't get a
          warning message for stealthy Assassins.

        - The "Powerful Enemies Are" setting determines how often bosses and other tough enemies spawn.


    CHARACTER CREATION:

        - Most new characters gain 20 HP per level by default and they all start with Steel weapons rather than Iron.


    COMBAT:

        - Your Hand-to-Hand accuracy is buffed by equipped glove/boot material. This scales like vanilla weapon material bonuses.


    ENEMIES:

        - Most enemies are tougher, faster, hit harder, and are harder to hit.

        - Vanilla's weapon material requirements to damage certain monsters have been removed.

        - Enemy spell variety greatly increased.

        - The "Enemy Move Speed" setting determines how fast enemies move.

        - Enable the "Display Enemy Level" setting and activate enemies to be shown their level. By default this is left click.


    LEVEL SCALING:

        - Monsters, loot, and human enemies unleveled.

        - Enemy difficulty and loot quality scales with their level. Bosses have greatly increased chances to drop good loot.

        - Once you are Level 7 or higher human enemies are unleveled.


    ITEMS:

        - Daedric and other valuable materials are rare but can appear anytime, anywhere. Your level and Luck do not change material
          generation chances.

        - All weapon/armor durabilities have been decreased. Durability scales down as material tier increases.

        - Shield armor values increase as material tier increases.

        - Enable the "Alternate Loot Piles" setting to use an expanded vanilla sprite list for variety in loot pile appearance.


    MAGIC ITEMS:

        - All magic items have been modified. They have different names, enchantments, and/or durabilities.


    SHOPS:

        - Horses and Wagons are expensive.

        - Store quality ("Rusty Relics", "Sturdy Shelves", etc.) determines what items are available. Best-in-slot weapons and
          armor and fancy or expensive-looking items are not in low quality shops.


PROJECT HISTORY:

    Bossfall's origin can be traced back to early 2021, when I was on a mission to beat every single game that I currently owned.
    This goal was at first a tedious process, as all the games that were lying around my hard drive were the ones I was avoiding.
    As I had purchased a copy of the original DOOM from GOG.com, I was gifted a little game called TES Chapter I: Arena...

    My first experience with an Elder Scrolls game was not positive. I slogged my way through Arena's opening dungeon, traveled
    to a random city, and wandered around purposeless for some time. I had no idea where to go and what to do. I wandered around
    outside the city's walls and stumbled across a tiny dungeon, which contained nothing interesting. I quit in disgust.

    After trying (and failing) to enjoy Outlast 2, I returned, grudgingly, to Arena. This time, I looked up information on where
    to go to progress the main quest. I had some success with my Battlemage character and found I was enjoying myself a bit, which
    was more than I expected. I explored the first few main quest dungeons thoroughly and began assembling the Staff of Chaos.

    Unfortunately, I soon found out my character was ridiculously overpowered. I crafted some extremely powerful spells and blasted
    my way through the rest of the game, which was a breeze. I couldn't stop looking forwards to trying Daggerfall, though. I
    assumed it was better than Arena, which I thought had lots of potential but ultimately didn't deliver.

    I first fired up GOG.com's version of Daggerfall in mid-2021 and I instantly fell in love. From the moody dungeon music to the
    intricately complicated dungeon designs, I loved it all. I spent hours upon hours in the Iliac Bay. However, I coudn't avoid
    the feeling I was missing something. I did a lot of reading on UESP and found out that Daggerfall was riddled with bugs, which
    bothered me to no end. I wanted my Dodging skill to actually help! Eventually, I found my way to DaggerfallUnity.

    After beating the main quest in vanilla DFU without any mods, I began another playthrough using all the cool mods I wanted. As
    I played, I found myself thinking "what would I change if I could?" I began writing detailed lists of mod ideas that I was unable
    to do anything with, as I had no idea how to implement them. I gradually did more research on what I needed to being modding,
    but it was a frustratingly slow learning process.

    After reading through the codebase trying to decipher the mysteries of C#, I finally started changing vanilla's code in
    mid-October 2021. At that time I had no background or prior experience in programming or coding, so my initial work was
    limited to changing vanilla's formula variables. After about a month of steady work, I released Bossfall v1.1 on the
    DFWorkshop forums in mid-November 2021. It wasn't a .dfmod and was not user-friendly, so it wasn't exactly a hit. 
    
    The non-event of Bossfall's initial release helped me come to a healthy conclusion. Since this was my first programming project,
    I shouldn't expect to be immediately successful. As with anything worth doing in life, success takes lots of time, and lots of
    hard work. Bolstered by that knowledge I began work on v1.2, while also learning the basics of programming in C#. I should've
    learned those fundamentals 'before' trying to write code, but with the helpful comments in vanilla's codebase and lots of
    studying I slowly expanded my programming knowledge. I also discovered, to my dismay, that many of the "improvements" I made in
    v1.1 didn't work, and in the worst cases completely broke the game. Once I got the errors resolved and lots more changes
    finalized, I released v1.2 on the DFWorkshop forums in mid-December 2021.

    Shortly afterwards, DFU 0.13.4 was released. With the lighting changes done, I felt it was time to release Bossfall on NexusMods.
    I uploaded v1.2.1 on Christmas Day, but I ran into yet another problem. Because I was distributing a build of the whole game,
    NexusMods flagged the .exe files in the mod package as suspicious and quarantined Bossfall. What a headache that was! Once
    NexusMods staff got back on the following Monday, I got them to release Bossfall from quarantine.

    Completely unsurprisingly, Bossfall was not very popular on NexusMods, either. While at that point I had dramatically improved
    the quality of my documentation, I couldn't escape the fact that my distribution package was complicated, difficult to deal
    with and to upgrade, and often didn't play nicely with other mods.

    Regardless of this fact I soon resumed work on v1.3, which I released in mid-January 2022. By this point, my Bossfall project
    had grown to an enormous scope, altering nearly one hundred vanilla files and over ten thousand lines of code. My programming
    knowledge had likewise increased. I had also discovered something unexpected - as I learned how to code, I realized that I
    had found my passion in life. I wanted to be a game developer! 

    Due to this realization, I changed how I mentally approached the Bossfall project. Now viewing it as a learning and discovery
    process, I figured it was time to bite the bullet and begin work on a .dfmod. After weeks of more frustration trying to learn
    an entirely new programming system, I began to make significant progress. And finally, I released the Bossfall .dfmod.

    You've read my story, now it's time to make your own. Tamriel is waiting...


v1.3.1 CHANGELOG:

    - Documents every change I made in Bossfall v1.3.1. If no entry on a subject exists, its behavior and values are vanilla's.


    BANKING:

        - Bank loans charge 20% interest rather than 10%.


    BOSS INFO:

        - All bosses can be damaged by weapons of any material. Silver deals double damage to undead bosses.

        - Bosses are generally very rare and are the only enemies - besides Guards (HALT!) - that can be above Level 20.

        - All bosses drop loot and its quality scales heavily with their level.

        - Vampire Ancients no longer cast spells.

        - Spellcaster spell variety greatly increased and they all use the same spells.

        - Each climate region and dungeon type has different potential bosses, with some overlap.

        - Two types of Dragonling exist - the non-boss and the boss version. Use "Display Enemy Level" to tell between the two.
          Boss Dragonlings move much faster than the non-boss version.

        - See the FORMULAS & TABLES section of the ReadMe for details on which bosses can spawn where.

        - See the FORMULAS & TABLES section of the ReadMe for spell kits and precise numbers on boss spawn chances and loot scaling
          by boss level.


    BOSS STATS:

        - Assassins, Vampires, Vampire Ancients, and boss Dragonlings will deal much less than stated dmg if player is a Vampire.


        VAMPIRE:

            - Can be damaged by any weapon material.

            - Moves extremely fast.

            - 55-85 average damage per hit, this will vary slightly depending on the Vampire's level.

            - 70 average damage per attack animation.

            - Has 80-240 HP.

            - Has -6 Armor.

            - Level 21-25.

            - Has Magicka for 30 spells.

            - Takes double damage from Silver gloves, boots, and weapons.

            - Soul Gems cost 750,000 gold and are worth 7500 Enchantment Points.

            - 1.4% chance per hit to inflict any disease (in vanilla they can only inflict the Plague).


        LICH:

            - Can be damaged by any weapon material.

            - Moves slowly.

            - 60-90 damage per hit.

            - 75 average damage per attack animation.

            - Has 80-240 HP.

            - Has -7 Armor.

            - Level 21-25.

            - Has Magicka for 30 spells.

            - Takes double damage from Silver gloves, boots, and weapons.

            - Soul Gems cost 750,000 gold and are worth 7500 Enchantment Points.


        ASSASSIN:

            - If player is level 1-6, Assassins have standard human class enemy armor, HP, damage, and loot quality. Their weapon
              poison will not pierce player's Poison Immunity.

            - Moves extremely fast.

            - 39-67 average damage per hit if player is above level 6, this will vary depending on the Assassin's level.

            - 106 average damage per attack animation if player is above level 6, this will vary depending on the Assassin's level.

            - Has 100-300 HP if player is above level 6.

            - Has -8 Armor if player is above level 6.

            - Level 21-30 if player is above level 6.

            - Weapon poison will pierce Poison Immunity if player is above level 6.

            - If player is above level 1, Assassins will always wield a poisoned weapon.

            - Sees through Invisibility at any player level.

            - "Boss Proximity Warning" HUD message will not display for stealthy Assassins.


        DRAGONLING_ALTERNATE:

            - Moves extremely fast.

            - 95-125 average damage per hit, this will vary slightly depending on the Dragonling's level.

            - 110 average damage per attack animation.

            - Has 130-390 HP.

            - Has -9 Armor.

            - Level 21-30.

            - Sees through Invisibility.

            - Soul Gems cost 1,500,000 gold and are worth 15,000 Enchantment Points.


        ORC WARLORD:

            - Moves somewhat fast.

            - 42-73 damage per hit.

            - 115 average damage per attack animation.

            - Has 150-450 HP.

            - Has -10 Armor.

            - Level 21-30.

            - Soul Gems cost 1,500,000 gold and are worth 15,000 Enchantment Points. 


        DAEDRA LORD:

            - Can be damaged by any weapon material.

            - Moves fast.

            - 30-60 damage per hit.

            - 120 average damage per attack animation.

            - Has 170-510 HP.

            - Has -11 Armor.

            - Level 26-30.

            - Infinite Magicka.

            - Soul Gems cost 2,250,000 gold and are worth 22,500 Enchantment Points.


        VAMPIRE ANCIENT:

            - Can be damaged by any weapon material.

            - Moves incredibly fast, will outrun player with 100 Running and SPD.

            - 110-140 average damage per hit, this will vary slightly depending on the Vampire Ancient's level.

            - 125 average damage per attack animation.

            - Has 180-540 HP.

            - Has -12 Armor.

            - Level 26-30.

            - Casts no spells.

            - Takes double damage from Silver gloves, boots, and weapons.

            - Soul Gems cost 2,250,000 gold and are worth 22,500 Enchantment Points.

            - 1.4% chance per hit to inflict any disease (in vanilla they can only inflict the Plague).


        ANCIENT LICH:

            - Can be damaged by any weapon material.

            - Moves slowly.

            - 115-145 damage per hit.

            - 130 average damage per attack animation.

            - Has 200-600 HP.

            - Has -13 Armor.

            - Level 26-30.

            - Infinite Magicka.

            - Takes double damage from Silver gloves, boots, and weapons.

            - Sees through Invisibility.

            - Soul Gems cost 2,250,000 gold and are worth 22,500 Enchantment Points.


    CHARACTER CREATION:

        - All custom classes start with default HP/lvl of 20, default skill advancement difficulty unchanged.

        - All Iron weapons are converted to Steel when starting a new character.

        - Going below default HP/lvl of 20 drops difficulty dagger twice as much as vanilla per point.

        - Rebalanced special advantage/disadvantage costs, this rebalance is slightly different than v1.3's values.

        - Compared to vanilla most advantages are cheaper, most disadvantages drop difficulty dagger much more.

        - All canned classes except for Barbarians start with 45 HP and gain 20 HP/lvl, Barbarian starting HP and HP/lvl unchanged.

        - Full special advantage/disadvantage cost table can be found in the FORMULAS & TABLES section of the ReadMe.


    COMBAT:

        - All landed attacks damage armor or shield of target struck. In vanilla only weapon attacks damaged armor/shields.

        - All Steel weapons have a minimum damage of 1.

        - Knockbacks heavily nerfed to reduce stunlocks.

        - Strength weapon damage bonus halved.

        - Body parts hit with equal likelihood. In vanilla this was weighted towards chest strikes.


    CRIME & PUNISHMENT:

        - Hostile guards use Knight animations, move faster, shoot arrows, and hit much harder. HALT!

        - If player is level 1-6, Guards will be up to 2 levels below or 12 levels above player. HALT! 

        - Once player is at least level 7, Guards are on average around level 15. Their level range is 1-30. HALT!

        - See FORMULAS & TABLES section of the ReadMe for exact Guard (HALT!) level scaling formulas.

        - Guards carry weapons and armor of any material. HALT!


    ENEMY COMBAT:

        - Vanilla weapon material requirements to damage certain monsters removed. Any material will now damage most enemies.

        - Some monsters have new weapon/material immunities, resistances, and/or weaknesses. HUD messages will tell you what works.
          See MONSTER STATS section for detailed information.

        - All enemies have new combat AI. Some AI changes only apply if you are using Enhanced Combat AI.

        - Enemies fire bows and spells much more often. They also fire them at closer range and from greater distances.

        - Enemies back up and turn much faster. They will retreat and strafe less frequently.

        - When not in melee range, enemies strafe much faster. When in melee range, enemies strafe slowly.

        - Enemy accuracy scales heavily with their level. Their accuracy scales up much faster than vanilla. The toughest enemies
          are nearly twice as accurate as vanilla. See FORMULAS & TABLES section of the ReadMe for detailed accuracy formulas.

        - Enemy attack speed unleveled. In vanilla enemies attacked faster at higher player levels, but in Bossfall enemy attack
          speed is always equal to vanilla's enemy attack speed at player level 10, with greater random variation. See
          FORMULAS & TABLES section of the ReadMe for attack speed timings.

        - Enemy move speed is not reduced while enemy is melee attacking.

        - Enemy STR attribute bonus added to their Hand-to-Hand attack damage, scale is identical to that of player's.

        - Most non-boss enemies have a minimum damage of 1. This is not the case for enemies with Bonus to Hit: Humanoids or for
          human class enemies using Hand-To-Hand - they'll start using Hand-to-Hand around enemy level 6 and higher.

        - Enemies use drugs - Aegrotat, Quaesto Vil, Sursum, Indulcet - as weapon poisons, in addition to vanilla's weapon poisons.


    ENEMY SPAWNING:

        - Enemies unleveled - spawn chances and enemies spawned are not influenced by player level.

        - Every enemy spawn is randomly picked from a list of 100 or 140 - the "Powerful Enemies Are" setting determines length.
          Every dungeon type, location type, and climate has a different list. While outside, day and night have different lists.

        - All dungeons have enemy themes. You will find undead in Crypts, Daedra and spellcasters in Covens, and so forth.
          The easiest dungeons are Cemeteries, the toughest are Vampire Haunts.

        - Heat-loving or heat-tolerant enemies are found outside in Desert, Rainforest, Subtropical, and Swamp wilderness.
          Cold-loving or cold-tolerant enemies are found outside in Woodlands, Mountain, and Mountain Woods wilderness.

        - Difficult, high-level enemies are generally rare.

        - Orcs and humans spawn nearly anywhere, but they are usually rarer at night. Orcs and most humans do not spawn in
          Mountain and Mountain Woods wilderness at night or in Desert wilderness during day or night.

        - Rangers and Barbarians are outdoorsy types, they are rarely found in dungeons. They will spawn in Mountain and
          Mountain Woods wilderness at night, where it is too cold for most humans. Rangers can be found in Desert wilderness
          during the day and at night.

        - Werewolves and Wereboars removed from dungeons, they now only appear outside. They are rare.

        - Spawn frequency has been slightly reduced across all dungeons, locations, climates, and times.

        - Dungeon and outdoor locations use the same spawn chance. In vanilla spawn chances varied depending on player's location.

        - See FORMULAS & TABLES section of the ReadMe for spawn details about every dungeon, climate, location and time of day.


    ENEMY SPELLS:

        - Ghosts and Vampire Ancients no longer cast spells.

        - Enemy spell variety increased, every spellcaster uses same kit except for Frost & Fire Daedra (their spell kits are
          only slightly changed from vanilla's). Standard enemy spell kits include Sphere of Negation.

        - Full enemy spell kits can be found in the FORMULAS & TABLES section of the ReadMe.

        - Magicka costs for enemy spells no longer vary based on player spell skill level.

        - Spellcaster Magicka scales with their level. They will cast 2 spells at level 1, this scales up to 8 at level 20.
          Vampires and Liches will cast 30 spells, Daedra Lords and Ancient Liches have infinite Magicka.

        - The exact Magicka/level scale can be found in the FORMULAS & TABLES section of the ReadMe.

        - Enemies cast Caster Only spells at range. They cast Shalidor's Mirror, Spell Resistance, and Heal.

        - Enemies no longer re-cast most Caster Only spells if they are currently affected by them. The only exception to this rule
          is Heal - enemies can re-cast Heal if they are still under the effects of a previous Heal spell and are not at full HP.

        - Enemies will not cast Heal if they're already at full HP. In vanilla they would do so.

        - Enemies cast any type of spell within appropriate ranges. In vanilla enemies wouldn't cast certain types of spells
          depending on the Enhanced Combat AI setting in use.

        - I didn't add Hand of Decay to enemy spell kits as enemies can spam you with it in melee range.


    GAMEPLAY:

        - Climbing nerfed, success depends almost entirely on your skill level. Luck now influences your Climbing check success.

        - With 50 Luck you get no Climbing skill check bonus, with 1 Luck you get a 5% penalty, with 100 Luck you get a 5% bonus.

        - Building identification range in "Info" mode doubled.


    HAND-TO-HAND:

        - Glove/boot material affects player's Hand-to-Hand to-hit roll, scales just like vanilla weapon materials.

        - Glove material affects player's punch to-hit, boot material affects player's kick to-hit.

        - Leather/Chain/Steel/Silver do not alter Hand-to-Hand to-hit rolls.

        - Successful Hand-to-Hand attacks damage player's glove/boot durability based on if attack is punch/kick.

        - Silver gloves/boots will damage Ghosts/Wraiths/Lycanthropes/etc. if player is using Hand-to-Hand.

        - If you're wearing silver gloves you must use punch attacks to damage Ghosts/Wraiths/etc.

        - If you're wearing silver boots you must use kick attacks to damage Ghosts/Wraiths/etc.

        - Full list of Hand-to-Hand material to-hit bonuses can be found in the FORMULAS & TABLES section of the ReadMe.


    HUMAN ENEMIES:

        - If player is level 1-6, all human enemies are within 2 levels of the player and Assassins are not bosses.

        - Human enemies are unleveled (this includes Guards. HALT!) and Assassins become bosses once player hits Level 7.

        - Once player is level 7, human enemies will usually be around level 10. Level 1 and 20 humans are very rare.

        - Human attack damage scales with their level. Low level humans deal slightly more than weapon damage up to an average
          of 48 damage per hit at enemy level 20.

        - Human enemies have 4 armor at Level 1, which is slightly better than Imp armor. They scale up to -4 armor at Level
          20, which is slightly worse than Daedra Seducer armor. Their armor does not change based on their equipment.

        - Human spellcaster spell variety greatly increased, all human spellcasters use the same spells regardless of level.

        - If using Enhanced Combat AI, Archers and Rangers never voluntarily move into melee range and will always retreat when
          approached. They'd rather shoot arrows.

        - If using Enhanced Combat AI, Barbarians will always charge the player. They're not too bright.

        - Level 20 Mages, Sorcerers, and Nightblades can see Invisible.

        - Sorcerers, Barbarians, and Rangers are now pacified by the Streetwise language skill.

        - The exact formulas and tables referenced in this section can be found in the FORMULAS & TABLES section of the ReadMe.


    ITEMS:

        - Weapon, armor, and shield durability heavily nerfed. Durability decreases as material tier increases.

        - Shields have much more durability than armor.

        - Many item weights and values changed. Weapon, armor and shield weights and values unchanged.

        - Enchantment Point capacities of weapons, armor, and shields changed. Staves have the highest capacity.

        - All weapons have the same durability scale. Best-in-weapon-skill weapons last the longest. For example, a Steel
          Warhammer will last exactly as long as a Steel Long Bow or a Steel Wakazashi. A Steel Staff will last exactly as
          long as a Steel Short Bow or Steel Dagger.

        - For exact item weights/values and durability and Enchantment Point values see the ITEM TABLES section of the ReadMe.


    LOOT:

        - All non-enemy loot has been unleveled. This applies to shop shelves, house containers and dungeon and building loot
          piles. The best materials are very rare but possible at any level. Player's level and Luck does not change drop chances.

        - If a level 1-15 enemy drops loot, it is normal unleveled loot (such as from loot piles, shop shelves, etc.).

        - If a level 16+ enemy drops loot, loot quality scales heavily with enemy's level. Level 30 enemies have extremely high
          chances to drop Daedric items. These drop chances are not changed by player's level or Luck.

        - Enemies that spawn with equipped armor and weapons now have a greatly increased chance to drop Gauntlets and any type
          of weapon or shield.

        - All gold generation unleveled, gold is much rarer. Gold dropped by enemies now scales with their level, with significant
          random variation. Player's level and Luck does not change gold generation chances.

        - Ingredient generation unleveled, ingredients are much rarer. All ingredients dropped by enemies now scale with their
          level, with significant random variation. Player's level and Luck does not change ingredient chances.

        - Reduced each armor piece's generation chance by an average of 38% for enemies that spawn with equipped armor.

        - Reduced shield and off-hand Short Blade generation chance by 40% for enemies that spawn with these item types. 

        - Reduced Plate armor generation chance to 10% and increased Leather armor generation chance to 70%.

        - Arrows spawn with a maximum of 30 per stack.

        - The exact formulas and tables referenced in this section can be found in the FORMULAS & TABLES section of the ReadMe.


    LYCANTHROPY:

        - If player is in wereform equipped gloves/boots aren't damaged when player attacks and they don't give to-hit bonuses.

        - If player is in wereform they can damage enemies that are normally immune to Hand-to-Hand.


    MAGIC ITEMS:

        - All non-quest-reward, non-artifact magic items have different names, enchantments, and/or durabilities.

        - Pointless vanilla item powers - for example, "Item of Venom Spitting" - replaced with more useful enchantments.

        - Regular magic items no longer spawn with the "Absorbs Spells" power. Artifacts still can.

        - Holy Water, Holy Daggers, and Holy Tomes cast Dispel Undead/Undead/Daedra on use, have 1/3/3 charges.

        - Empty Soul Gems cost 50,000 gold.

        - All non-boss filled Soul Gem costs increased by 50,000 gold.

        - All boss Soul Gem costs and Enchantment Point values greatly increased (except for Assassins, you can't bind human souls).

        - Daedroth Soul Gem cost and Enchantment Point value increased (this is in addition to the previously mentioned 50,000).

        - The new Holy items appear in high quality Pawn Shops and as random loot from enemies, loot piles and house containers.

        - The new Holy items won't dispel Undead/Daedra 100% of the time, success rate depends on player level.

        - Regular magic items found as loot or in stores will never be Holy Water, Holy Daggers, or Holy Tomes.

        - Bossfall's magic item and Soul Gem Enchantment Point value lists can be found in the ITEM TABLES section of the ReadMe.


    MONSTER STATS:

        - This section assumes you are using the Recommended setting preset, which sets the "Enemy Move Speed" setting to "Fast".
          For exact enemy movement speed values for "Fast" and "Very Fast" settings, see the FORMULAS & TABLES section of the ReadMe.

        - "Always charges" will only matter if you are using Enhanced Combat AI. If you are not, every enemy will always charge.

        - Monster level variation usually only changes their accuracy, dodging, and spell power. If monster is a Dreugh, Lamia,
          or any Atronach, their level variation will also slightly change their damage ranges.

        - Dreughs, Lamia, and all Atronachs will deal much less than stated damage if player is a Vampire.


        RAT:

            - Moves somewhat fast.

            - Always charges.

            - 1-3 damage per hit.

            - 2 average damage per attack animation.

            - Has 6-18 HP.

            - Has 7 Armor.

            - Level 1-3.

            - 5% chance per hit to inflict the Plague. In vanilla they could inflict other diseases.

            - Soul Gems cost 50,000 gold.


        IMP:

            - Can be damaged by any weapon material.

            - Moves somewhat slowly.

            - 1-10 damage per hit.

            - 5.5 average damage per attack animation.

            - Has 8-24 HP.

            - Has 5 Armor.

            - Level 1-4.

            - Has Magicka for 2 spells.

            - Spell variety greatly increased. See FORMULAS & TABLES section for full spell list.

            - Does not see Invisible.

            - Soul Gems cost 51,000 gold.


        SPRIGGAN:

            - Hand-to-Hand attacks deal 1/4 damage and player suffers 1 HP damage every landed attack.

            - Moves extremely slowly.

            - 1-12 damage per hit.

            - 6.5 average damage per attack animation.

            - Has 14-42 HP.

            - Has 6 Armor.

            - Level 1-5.

            - Soul Gems cost 51,000 gold.


            WEAPON EFFECTS:

                - Axes deal x2 damage.

                - Blunt Weapons deal x1 damage.

                - Long Blades deal x1 damage and suffer x2 durability damage.

                - Short Blades deal 1/2 damage and suffer x2 durability damage.

                - Archery Immune.


        GIANT BAT:

            - Moves very fast.

            - Always charges.

            - 1-7 damage per hit.

            - 4 average damage per attack animation.

            - Has 8-24 HP.

            - Has 5 Armor.

            - Level 1-5.

            - 5% chance per hit to inflict disease, increased from vanilla's 2%.

            - Soul Gems cost 50,000 gold.


        GRIZZLY BEAR:

            - Moves somewhat fast.

            - Always charges.

            - 1-18 damage per hit.

            - 9.5 average damage per attack animation.

            - Has 33-99 HP.

            - Has 6 Armor.

            - Level 2-6.

            - Soul Gems cost 50,000 gold.


        SABERTOOTH TIGER:

            - Moves fast.

            - Always charges.

            - 1-21 damage per hit.

            - 11 average damage per attack animation.

            - Has 25-75 HP.

            - Has 4 Armor.

            - Level 2-6.

            - Soul Gems cost 50,000 gold.


        SPIDER:

            - Attacks will not paralyze targets.

            - Has a 9.9% chance per hit to inflict one of eleven vanilla poisons. Poison selection is random. Each poison has the
              same chance to be selected.

            - Has a 0.1% chance per hit to inflict Drothweed. This pierces target's Poison Immunity if target is above level 1.

            - Moves very fast.

            - Always charges.

            - 1-8 damage per hit.

            - 4.5 average damage per attack animation.

            - Has 14-42 HP.

            - Has 5 Armor.

            - Level 2-6.

            - Soul Gems cost 50,000 gold.


        ORC:

            - Moves somewhat slowly.

            - 1-12 damage per hit.

            - 9.75 average damage per attack animation.

            - Has 24-72 HP.

            - Has 7 Armor.

            - Level 3-7.

            - Never wields a weapon, never drops large amounts of armor.

            - Never wields a poisoned weapon.

            - Soul Gems cost 51,000 gold.


        CENTAUR:

            - Moves fast.

            - 1-15 damage per hit.

            - 12 average damage per attack animation.

            - Has 20-60 HP.

            - Has 6 Armor.

            - Level 3-7.

            - Never wields a weapon, never drops large amounts of armor.

            - Never wields a poisoned weapon.

            - Soul Gems cost 53,000 gold.


        WEREWOLF:

            - Can be damaged by any weapon material.

            - Moves very fast.

            - 1-24 damage per hit.

            - 25 average damage per attack animation.

            - Has 33-99 HP.

            - Has 0 Armor.

            - Level 10-14.

            - Soul Gems cost 51,000 gold.


            WEAPON EFFECTS:

                - Silver weapons deal x1 damage.

                - Hand-to-Hand punches deal x1 damage if player is wearing Silver gloves.

                - Hand-to-Hand kicks deal x1 damage if player is wearing Silver boots.

                - All other weapon or Hand-to-Hand attacks deal 1/2 damage.


        NYMPH:

            - Can be damaged by any weapon material.

            - Moves slowly.

            - 1-10 damage per hit.

            - 5.5 average damage per attack animation.

            - Has 15-45 HP.

            - Has 4 Armor.

            - Level 4-8.

            - Soul Gems cost 60,000 gold.


        SLAUGHTERFISH:

            - Swims slowly.

            - Always charges.

            - 1-12 damage per hit.

            - 13 average damage per attack animation.

            - Has 20-60 HP.

            - Has 5 Armor.

            - Level 5-9.

            - Soul Gems cost 50,000 gold.


        ORC SERGEANT:

            - Moves somewhat slowly.

            - 1-25 damage per hit.

            - 19.5 average damage per attack animation.

            - Has 50-150 HP.

            - Has 2 Armor.

            - Level 9-13.

            - Soul Gems cost 51,000 gold.


        HARPY:

            - Can be damaged by any weapon material.

            - Moves fast.

            - 1-23 damage per hit.

            - 12 average damage per attack animation.

            - Has 25-75 HP.

            - Has 5 Armor.

            - Level 6-10.

            - Soul Gems cost 53,000 gold.


        WEREBOAR:

            - Can be damaged by any weapon material.

            - Moves fast.

            - 1-48 damage per hit.

            - 24.5 average damage per attack animation.

            - Has 44-132 HP.

            - Has 2 Armor.

            - Level 10-14.

            - Soul Gems cost 51,000 gold.


            WEAPON EFFECTS:

                - Silver weapons deal x1 damage.

                - Hand-to-Hand punches deal x1 damage if player is wearing Silver gloves.

                - Hand-to-Hand kicks deal x1 damage if player is wearing Silver boots.

                - All other weapon or Hand-to-Hand attacks deal 1/2 damage.


        SKELETAL WARRIOR:

            - Moves somewhat fast.

            - Always charges.

            - 1-19 damage per hit.

            - 10 average damage per attack animation.

            - Has 17-51 HP.

            - Has 4 Armor.

            - Level 6-10.

            - Does not see Invisible.

            - 2% chance per hit to inflict any disease. In vanilla they didn't inflict disease.

            - Soul Gems cost 50,000 gold.


            WEAPON EFFECTS:

                - Blunt Weapons deal x2 damage.

                - Axes or Hand-to-Hand attacks deal x1 damage.

                - Long Blades deal 1/2 dmg and suffer x2 durability damage.

                - Short Blades deal 1/3 dmg and suffer x3 durability damage.

                - Archery Immune.


        GIANT:

            - Moves somewhat fast.

            - Always charges.

            - 1-30 damage per hit.

            - 31 average damage per attack animation.

            - Has 70-210 HP.

            - Has 4 Armor.

            - Level 10-14.

            - Soul Gems cost 53,000 gold.


        ZOMBIE:

            - Moves extremely slowly.

            - Always charges.

            - 1-15 damage per hit.

            - 8 average damage per attack animation.

            - Has 33-99 HP.

            - Has 5 Armor.

            - Level 5-9.

            - 5% chance per hit to inflict disease, increased from vanilla's 2%.

            - Soul Gems cost 50,000 gold.


            WEAPON EFFECTS:

                - Axes deal x2 damage.

                - Blunt Weapons and Long Blades deal x1 damage.

                - Short Blades deal 1/2 damage.

                - Hand-to-Hand attacks deal 1/3 damage.

                - Archery attacks deal 1/4 damage.


        GHOST:

            - Moves extremely slowly.

            - Always charges.

            - 1-30 damage per hit.

            - 15.5 average damage per attack animation.

            - Has 5-15 HP.

            - Has -4 Armor.

            - Level 9-13.

            - Casts no spells.

            - Does not see Invisible.

            - Soul Gems cost 80,000 gold.


            WEAPON EFFECTS:

                - Silver weapons deal x1 damage.

                - Hand-to-Hand punches deal x1 damage if player is wearing Silver gloves.

                - Hand-to-Hand kicks deal x1 damage if player is wearing Silver boots.

                - Immune to all other weapon or Hand-to-Hand attacks.


        MUMMY:

            - Can be damaged by any weapon material.

            - Moves extremely slowly.

            - Always charges.

            - 1-25 damage per hit.

            - 13 average damage per attack animation.

            - Has 45-135 HP.

            - Has 2 Armor.

            - Level 8-12.

            - Does not see Invisible.

            - 2% chance per hit to inflict disease, decreased from vanilla's 5%.

            - Soul Gems cost 60,000 gold.


            WEAPON EFFECTS:

                - Axes and Long Blades deal x2 damage.

                - Blunt Weapons, Short Blades, and Hand-to-Hand attacks deal x1 damage.

                - Archery attacks deal 1/2 damage.


        GIANT SCORPION:

            - Attacks will not paralyze targets.

            - Has a 9.9% chance per hit to inflict one of eleven vanilla poisons. Poison selection is random. Each poison has the
              same chance to be selected.

            - Has a 0.1% chance per hit to inflict Drothweed. This pierces target's Poison Immunity if target is above level 1.

            - Hand-to-Hand attacks deal 1/2 damage and player suffers 1 HP damage every landed attack.

            - Moves fast.

            - Always charges.

            - 1-30 damage per hit.

            - 15.5 average damage per attack animation.

            - Has 33-99 HP.

            - Has 1 Armor.

            - Level 10-14.

            - Soul Gems cost 50,000 gold.


            WEAPON EFFECTS:

                - Axes deal x2 damage.

                - Blunt Weapons, Long Blades, and Short Blades deal x1 damage.

                - Archery attacks deal 1/3 damage.


        ORC SHAMAN:

            - Moves somewhat slowly.

            - 1-35 damage per hit.

            - 28.8 average damage per attack animation.

            - Has 55-165 HP.

            - Has -2 Armor.

            - Level 14-18.

            - Has Magicka for 4-6 spells. See FORMULAS & TABLES section for full list of Magicka capacities by level.

            - Spell variety greatly increased. See FORMULAS & TABLES section for full spell list.

            - Soul Gems cost 53,000 gold.


        GARGOYLE:

            - Hand-to-Hand attacks deal no damage and player suffers 2 HP damage every landed attack.

            - Can be damaged by any weapon material.

            - Moves extremely slowly.

            - Always charges.

            - 1-50 damage per hit.

            - 25.5 average damage per attack animation.

            - Has 50-150 HP.

            - Has -1 Armor.

            - Level 12-16.

            - Soul Gems cost 53,000 gold.


            WEAPON EFFECTS:

                - Blunt Weapons deal x1 damage and suffer x2 durability damage.

                - Axes deal x1 damage and suffer x3 durability damage.

                - Long Blades deal 1/2 damage and suffer x4 durability damage.

                - Short Blades deal 1/3 damage and suffer x4 durability damage.

                - Archery Immune.


        WRAITH:

            - Moves slowly.

            - Always charges.

            - 1-45 damage per hit.

            - 23 average damage per attack animation.

            - Has 10-30 HP.

            - Has -8 Armor.

            - Level 13-17.

            - Has Magicka for 4-5 spells. See FORMULAS & TABLES section for full list of Magicka capacities by level.

            - Spell variety greatly increased. See FORMULAS & TABLES section for full spell list.

            - Does not see Invisible.

            - Soul Gems cost 80,000 gold.


            WEAPON EFFECTS:

                - Silver weapons deal x1 damage.

                - Hand-to-Hand punches deal x1 damage if player is wearing Silver gloves.

                - Hand-to-Hand kicks deal x1 damage if player is wearing Silver boots.

                - Immune to all other weapon or Hand-to-Hand attacks.


        FROST DAEDRA:

            - Can be damaged by any weapon material.

            - Moves slowly.

            - 1-100 damage per hit.

            - 75.75 average damage per attack animation.

            - Has 35-105 HP.

            - Has 0 Armor.

            - Level 15-19.

            - Has Magicka for 4-6 spells. See FORMULAS & TABLES section for full list of Magicka capacities by level.

            - Spell variety slightly increased. See FORMULAS & TABLES section for full spell list.

            - Always drops a weapon, usually drops large amounts of armor.

            - Does not see Invisible.

            - Soul Gems cost 100,000 gold.


        FIRE DAEDRA:

            - Player suffers 4 HP damage every landed Hand-to-Hand attack.

            - Can be damaged by any weapon material.

            - Moves fast.

            - 1-50 damage per hit.

            - 38.25 average damage per attack animation.

            - Has 60-180 HP.

            - Has -3 Armor.

            - Level 15-19.

            - Has Magicka for 4-6 spells. See FORMULAS & TABLES section for full list of Magicka capacities by level.

            - Spell variety slightly increased. See FORMULAS & TABLES section for full spell list.

            - Always drops a weapon, usually drops large amounts of armor.

            - Does not see Invisible.

            - Soul Gems cost 100,000 gold.


        DAEDROTH:

            - Can be damaged by any weapon material.

            - Moves somewhat fast.

            - 1-50 damage per hit.

            - 51 average damage per attack animation.

            - Has 66-198 HP.

            - Has -4 Armor.

            - Level 16-20.

            - Has Magicka for 5-8 spells. See FORMULAS & TABLES section for full list of Magicka capacities by level.

            - Spell variety greatly increased. See FORMULAS & TABLES section for full spell list.

            - Always drops a weapon, usually drops large amounts of armor.

            - Does not see Invisible.

            - Soul Gems cost 150,000 gold and are worth 1,000 Enchantment Points.


        DAEDRA SEDUCER:

            - Can be damaged by any weapon material.

            - Moves fast.

            - 1-90 damage per hit.

            - 45.5 average damage per attack animation.

            - Has 75-225 HP.

            - Has -5 Armor.

            - Level 17-20.

            - Has Magicka for 5-8 spells. See FORMULAS & TABLES section for full list of Magicka capacities by level.

            - Spell variety greatly increased. See FORMULAS & TABLES section for full spell list.

            - Always drops a weapon, usually drops large amounts of armor.

            - Soul Gems cost 200,000 gold.


        DRAGONLING:

            - Moves fast.

            - 1-30 damage per hit.

            - 15.5 average damage per attack animation.

            - Has 40-120 HP.

            - Has 3 Armor.

            - Level 8-12.

            - Soul Gems cost 50,000 gold.


        FIRE ATRONACH:

            - Player suffers 2 HP damage every landed Hand-to-Hand attack.

            - Moves fast.

            - 15-33 average damage per hit, this will vary slightly depending on the Fire Atronach's level.

            - 24 average damage per attack animation, this will vary slightly depending on the Fire Atronach's level.

            - Has 40-120 HP.

            - Has 3 Armor.

            - Level 14-18.

            - Soul Gems cost 80,000 gold.


        IRON ATRONACH:

            - Hand-to-Hand attacks deal no damage and player suffers 3 HP damage every landed attack.

            - Moves extremely slowly.

            - 15-43 average damage per hit, this will vary slightly depending on the Iron Atronach's level.

            - 29 average damage per attack animation, this will vary slightly depending on the Iron Atronach's level.

            - Has 66-198 HP.

            - Has 2 Armor.

            - Level 14-18.

            - Soul Gems cost 80,000 gold.


            WEAPON EFFECTS:

                - Blunt Weapons deal x1 damage and suffer x3 durability damage.

                - Axes deal 1/2 damage and suffer x4 durability damage.

                - Long Blades deal 1/3 damage and suffer x5 durability damage.

                - Short Blades deal 1/4 damage and suffer x5 durability damage.

                - Archery Immune.


        FLESH ATRONACH:

            - Moves extremely slowly.

            - 15-28 average damage per hit, this will vary slightly depending on the Flesh Atronach's level.

            - 21.5 average damage per attack animation, this will vary slightly depending on the Flesh Atronach's level.

            - Has 60-180 HP.

            - Has 4 Armor.

            - Level 14-18.

            - Soul Gems cost 60,000 gold.


            WEAPON EFFECTS:

                - Axes deal x2 damage.

                - Blunt Weapons and Long Blades deal x1 damage.

                - Short Blades deal 1/2 damage.

                - Hand-to-Hand attacks deal 1/3 damage.

                - Archery attacks deal 1/4 damage.


        ICE ATRONACH:

            - Hand-to-Hand attacks deal no damage and player suffers 2 HP damage every landed attack.

            - Moves extremely slowly.

            - 15-38 average damage per hit, this will vary slightly depending on the Ice Atronach's level.

            - 26.5 average damage per attack animation, this will vary slightly depending on the Ice Atronach's level.

            - Has 50-150 HP.

            - Has 3 Armor.

            - Level 14-18.

            - Soul Gems cost 80,000 gold.


            WEAPON EFFECTS:

                - Blunt Weapons deal x1 damage.

                - Axes deal x1 damage and suffer x2 durability damage.

                - Long Blades deal 1/2 damage and suffer x3 durability damage.

                - Short Blades deal 1/3 damage and suffer x4 durability damage.

                - Archery Immune.


        DREUGH:

            - Swims extremely slowly.

            - 7-20 average damage per hit, this will vary slightly depending on the Dreugh's level.

            - 18 average damage per attack animation, this will vary slightly depending on the Dreugh's level.

            - Has 22-66 HP.

            - Has 5 Armor.

            - Level 6-10.

            - Soul Gems cost 60,000 gold.


        LAMIA:

            - Swims slowly.

            - 15-33 average damage per hit, this will vary slightly depending on the Lamia's level.

            - 48 average damage per attack animation, this will vary slightly depending on the Lamia's level.

            - Has 51-153 HP.

            - Has 0 Armor.

            - Level 14-18.

            - Soul Gems cost 60,000 gold.


    POTIONS:

        - Increased potion generation chance by 33% for all enemies that can spawn with them.


    QUESTING:

        - Activating quest enemies in Info mode will display their level and name at greatly increased distances.


    SETTINGS:

        - I suggest using the Recommended preset.

        - Settings default to Bossfall v1.2.1 values so current Bossfall players don't have to change settings unless they want to.


        POWERFUL ENEMIES ARE:

            - Determines how often powerful enemies spawn, also tweaks other enemy rarities.

            - "More Common" is Bossfall v1.2.1 spawn frequency, "Less Common" is rebalance I did for v1.3.


        ENEMY MOVE SPEED:

            - Switch between vanilla enemy movespeed and two faster options.

            - "Very Fast" is Bossfall v1.2.1 speeds, "Fast" is rebalance I did for v1.3.


        BOSS PROXIMITY WARNING:

            - HUD warning message when (non-Assassin) boss nearby, detection radius half a dungeon block.


        DISPLAY ENEMY LEVEL:

            - Activating an enemy in Info/Talk/Grab mode displays enemy's level as well as name.


        ALTERNATE LOOT PILES:

            - Uses expanded vanilla sprite list so loot piles have some visual variety.

            - May not work perfectly with Handpainted Models.


    SHOPS:

        - Horses and Wagons are expensive.

        - Shop quality now determines what items can be sold at a store. "Rusty Relics" and "Sturdy Shelves" quality stores
          have much less selection. "Average" and "Better Appointed" quality stores have slightly less selection. "Incense Burning"
          stores can stock any item available for the store type.

        - Low quality stores will never sell fancy- or expensive-looking items. For example, a "Rusty Relics" store will never
          sell Formal Cloaks. Those are only available in "Average" quality stores and higher. As a second example, "Rusty Relics"
          to "Better Appointed" stores will never stock Diamonds. "Incense Burning" stores are the only shops that can stock them.

        - All shops on average stock fewer items.

        - Cuirasses, gauntlets, tower shields, and best-in-slot weapons are not in low quality shops.

        - Shops of "Average" quality or better that are not libraries or bookstores will stock far fewer books.

        - Shops below "Average" quality that are not libraries or bookstores will never stock books.

        - Booksellers stock lots more books.

        - All shop formulas and shop item quality lists are in the FORMULAS & TABLES and ITEM TABLES sections of the ReadMe.


    SKILLS:

        - Miscellaneous Skill starting levels are 1 to 4 rather than vanilla's 3 to 6. This only applies to new characters.

        - Etiquette and Streetwise are exercised when encountering human enemies even if you don't pacify them. This now matches
          behavior of other language skills.

        - Reduced skill tallies from 3 to 1 for successful language skill pacifications.


v1.3.1 FORMULAS & TABLES:

    BOSS SPAWN CHANCES:

        - If the "Powerful Enemies Are" setting is "More Common":

            - Each spawned enemy has a 1/100 chance (1%) to be a boss.


        - If the "Powerful Enemies Are" setting is "Less Common":

            - Each spawned enemy has a 1/140 chance (0.71%) to be a boss.


        - These numbers can be higher if the environment or dungeon type has more than one boss. However, this is rare. 

        - Now, let's put these numbers into practice. In a large 4-block dungeon, let's say there will be 100 enemies spawned.

            - If the "Powerful Enemies Are" setting is "More Common":

                - There is a 99/100 chance of one enemy to NOT be a boss.

                - For multiple enemies, the chance of them to all NOT be bosses is (99/100)^(number of enemies). ^ means
                  "to the power of".

                - So in this case, calculate (99/100)^(100), which is 0.366, or 36.6%. Remember, this is the chance for all
                  100 enemies to NOT be bosses.

                - Thus, there is a (100% - 36.6%) = 63.40% chance for you to encounter at least one boss in this large 4-block
                  100-enemy dungeon. This calculation does not specify how many bosses you will likely find - it only tells
                  you how likely it is to find at least 1.

                - To calculate the chances of finding at least one boss among a varying number of enemies, use this formula:

                    100 - ((99/100)^(number of enemies) * 100) = your chance of encountering at least 1 boss


            - If the "Powerful Enemies Are" setting is "Less Common":
                    
                - To calculate the chances of finding at least one boss among a varying number of enemies, use this formula:

                    100 - ((139/140)^(number of enemies) * 100) = your chance of encountering at least 1 boss

                - So, for this imaginary 100-enemy dungeon, the chance of encountering at least 1 boss is:

                    100 - ((139/140)^(100) * 100) = 51.2%


    CHARACTER CREATION:

        CUSTOM CLASS SKILL ADVANCEMENT DIFFICULTY:

            - The maximum is 40, which will result in a Skill Advancement Difficulty of 3.0.

            - The minimum is -12, which will result in a Skill Advancement Difficulty of 0.3.

            - Going below Bossfall default HP per level of 20 will reduce the value by -4, going above will increase the value by 1.

            SPECIAL ADVANTAGE COSTS:                                SPECIAL DISADVANTAGE COSTS:

                Acute Hearing:              1                           Critical Weakness, each:        -14
                Adrenaline Rush:            1                           Damage In Holy Places:          -7 
                Athleticism:                2                           Damage In Sunlight:             -28
                Bonus To Hit Animals:       3                           Lowered Magicka in Daylight:    -10
                Bonus To Hit Daedra:        3                           No Magicka in Daylight:         -21
                Bonus To Hit Humanoids:     3                           Forbidden Chain Armor:          -14
                Bonus To Hit Undead:        3                           Forbidden Leather Armor:        -7
                Expertise, each:            2                           Forbidden Plate Armor:          -28
                Immunity, each:             28                          Forbidden Adamantium:           -8
                INT In Spell Points:        2                           Forbidden Daedric:              -28
                x1.5 INT In Spell Points:   5                           Forbidden Dwarven:              -4
                x1.75 INT In Spell Points:  6                           Forbidden Ebony:                -10
                x2 INT In Spell Points:     7                           Forbidden Elven:                -2
                x3 INT In Spell Points:     10                          Forbidden Iron:                 -14
                Rapid Healing, General:     4                           Forbidden Mithril:              -6
                Rapid Healing, Darkness:    3                           Forbidden Orcish:               -12
                Rapid Healing, Light:       2                           Forbidden Silver:               -28
                Regenerate HP, General:     6                           Forbidden Steel:                -42
                Regenerate HP, Darkness:    5                           Forbidden Shield, each:         -3
                Regenerate HP, Light:       3                           Forbidden Weaponry, each:       -7
                Regenerate HP, Water:       1                           Inability to Regen Magicka:     -28
                Resistance, each:           14                          Lowered Magicka in Darkness:    -14
                Spell Absorption, General:  400                         No Magicka in Darkness:         -28
                Spell Absorption, Darkness: 400                         Low Tolerance, each:            -7
                Spell Absorption, Light:    400                         Phobia, each:                   -7


    ENEMY ATTACK SPEED:

        - Attack timing ranges are determined randomly, and one attack's timing has no effect on the following attack's timing. For
          example, with "Very High" Reflexes the enemy may attack in 0.1 seconds and attack for a second time 0.1 seconds later.

        TIMING RANGE FOR ONE ATTACK BY REFLEXES SETTING, IN SECONDS:

            VERY HIGH:              HIGH:               AVERAGE:              LOW:                VERY LOW:

                0.1 - 2.7               0.6 - 3.1           1.0 - 3.6             1.5 - 4.0           1.9 - 4.5


    ENEMY MOVE SPEEDS:

        - For comparison, the player's running speed with 100 Running and 100 SPD is 11.7.

                               VANILLA:   FAST:   VERY FAST:

                         Rat:   4.875     6.5         7
                         Imp:   5.5       5.5         6
                    Spriggan:   4.75      3           3
                   Giant Bat:   5.5       8           8.5
                Grizzly Bear:   5.5       6.75        7.5
            Sabertooth Tiger:   5.5       7.25        8
                      Spider:   5.5       8           9
                         Orc:   5         5           5
                     Centaur:   5.5       7           7.5
                    Werewolf:   5.875     8           9
                       Nymph:   5.5       4.5         4.5
               Slaughterfish:   5.5       4.5         4.5
                Orc Sergeant:   5         5.75        5.75
                       Harpy:   5.5       7.5         8
                    Wereboar:   5.5       7           7.5
            Skeletal Warrior:   5.5       6           6
                       Giant:   5.25      6.5         7
                      Zombie:   5.5       3           3
                       Ghost:   5.5       3.5         3.5
                       Mummy:   5.5       3.5         3.5
              Giant Scorpion:   5.5       7           7.5
                  Orc Shaman:   5         5.25        5.75
                    Gargoyle:   5.5       3.5         4.5
                      Wraith:   5.875     4           4
                 Orc Warlord:   5         6           6.5
                Frost Daedra:   6.125     4           5
                 Fire Daedra:   6.125     7           7.5
                    Daedroth:   6.25      6           6.5
                     Vampire:   6.25      9          10
              Daedra Seducer:   5.5       7.5         7.5
             Vampire Ancient:   6.75     12          12
                 Daedra Lord:   6.75      7           7.5
                        Lich:   5.75      4           4
                Ancient Lich:   6.25      4.5         4.5
                  Dragonling:   6.25      7.5         8
               Fire Atronach:   5.25      7           7.5
               Iron Atronach:   5.125     3           3
              Flesh Atronach:   5.275     3           3
                Ice Atronach:   5.375     3           3
        Dragonling_Alternate:   5.5       9          10
                      Dreugh:   5.5       3.5         3.5
                       Lamia:   5.375     4           4
                        Mage:   4.825     4           4
                  Spellsword:   4.875     7           7.5
                  Battlemage:   5         6           6
                    Sorcerer:   4.875     4           4
                      Healer:   4.9       4           4
                  Nightblade:   5         7.5         8
                        Bard:   5         6.5         7
                     Burglar:   5.2       7.5         8
                       Rogue:   5         7.5         8
                     Acrobat:   5.325     8           8.5
                       Thief:   5.2       7.5         8
                    Assassin:   5         9          10
                        Monk:   5.2       7           7.5
                      Archer:   5.075     4.5         4.5
                      Ranger:   4.925     4.5         4.5
                   Barbarian:   5         6.5         7
                     Warrior:   4.925     5.75        5.75
                      Knight:   4.875     5           5
                       Guard:   4.875     8           8.5


    ENEMY SKILL LEVEL SCALING:

        SKILL FORMULA:

            (enemy level * 7) + 30

            - This scales 40% faster than vanilla. It sets weapon, Dodging, Critical Strike, and all other skill levels.


        SKILL CAP:

            180

            - This is 80% higher than vanilla. Bossfall enemies are more accurate and are harder to hit.


    ENEMY SPAWN CHANCES:

        - Every 2.4 game hours several random numbers from 0 to 40 are generated. If any of the random numbers are 0, and if
          enemies are allowed to spawn given current game conditions, a number of enemies equal to the number of randomly generated
          zeroes are spawned from the location or environment's encounter table. This process happens 10 times per game day.


    ENEMY SPELLS:

        FIRE DAEDRA SPELLS:

            Fireball
            Fire Storm
            God's Fire


        FROST DAEDRA SPELLS:

            Frostbite
            Ice Bolt
            Ice Storm


        GENERIC SPELLS:

            Energy Leech
            Far Silence
            Fireball
            Fire Storm
            Force Bolt
            God's Fire
            Hand of Sleep
            Heal
            Ice Bolt
            Ice Storm
            Lightning
            Magicka Leech
            Medusa's Gaze
            Paralysis
            Shalidor's Mirror
            Shock
            Silence
            Sleep
            Spell Drain
            Spell Resistance
            Sphere of Negation
            Strength Leech
            Toxic Cloud
            Vampiric Touch


        SPELLS CAST PER LEVEL:

            Level 1-7:      2
            Level 8-12:     3
            Level 13-15:    4
            Level 16-17:    5
            Level 18-19:    6
            Level 20:       8
            Level 21-25:    30
            Level 26-30:    Infinite


    EXTERIOR ENCOUNTER TABLES:

        DESERT, IN TOWN, NIGHT:             DESERT, NOT IN TOWN, DAY:               DESERT, NOT IN TOWN, NIGHT:

            COMMON:                             COMMON:                                 COMMON:

                Barbarian                           Dragonling                              Ghost
                Ranger                              Fire Atronach                           Mummy           
                                                    Giant Scorpion                          Ranger
                                                    Nymph                                   Skeletal Warrior
                
                                                
            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Acrobat                             Gargoyle                                Gargoyle
                Archer                              Ranger
                Bard
                Battlemage
                Burglar
                Healer
                Knight
                Mage
                Monk
                Nightblade
                Rogue
                Sorcerer
                Spellsword
                Thief
                Warrior


            RARE:                               RARE:                                   RARE:

                Gargoyle                            Daedra Seducer                          Wraith
                Wereboar                            Fire Daedra
                Werewolf


            BOSS:                               BOSS:                                   BOSSES:

                Vampire                             Dragonling_Alternate                    Ancient Lich
                                                                                            Lich


        HAUNTED WOODLANDS, IN TOWN, NIGHT:  HAUNTED WOODLANDS, NOT IN TOWN, DAY:    HAUNTED WOODLANDS, NOT IN TOWN, NIGHT:

                                                COMMON:                                 COMMON:

                                                    Ghost                                   Ghost
                                                    Spriggan                                Wereboar
                                                    Wraith                                  Werewolf
                                                                                            Wraith  


            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Acrobat                             Flesh Atronach                          Flesh Atronach
                Archer                              Mummy                                   Mummy
                Barbarian                           Skeletal Warrior                        Skeletal Warrior
                Bard                                Wereboar                                Zombie
                Battlemage                          Werewolf        
                Burglar                             Zombie
                Ghost                 
                Healer              
                Knight     
                Mage  
                Monk      
                Nightblade  
                Ranger           
                Rogue                 
                Sorcerer
                Spellsword
                Thief  
                Warrior
                Wraith


            RARE:

                Flesh Atronach
                Wereboar
                Werewolf


            BOSSES:                             BOSSES:                                 BOSSES:

                Vampire                             Ancient Lich                            Ancient Lich
                Vampire Ancient                     Lich                                    Lich
                                                                                            Vampire        
                                                                                            Vampire Ancient


        MOUNTAIN, IN TOWN, NIGHT:           MOUNTAIN, NOT IN TOWN, DAY:             MOUNTAIN, NOT IN TOWN, NIGHT:

            COMMON:                             COMMON:                                 COMMON:

                Barbarian                           Centaur                                 Barbarian    
                Ranger                              Giant                                   Grizzly Bear
                                                    Grizzly Bear                            Ice Atronach
                                                    Harpy                                   Skeletal Warrior
                                                    Spriggan                                Zombie          
                                                             

            UNCOMMON:                                                                   UNCOMMON:

                Acrobat                                                                     Frost Daedra
                Archer                                                                      Gargoyle      
                Bard                                                                        Ghost       
                Battlemage                                                                  Ranger  
                Burglar                                                                     Spriggan
                Healer                                                                      Wereboar
                Knight                                                                      Werewolf
                Mage           
                Monk          
                Nightblade           
                Rogue   
                Sorcerer  
                Spellsword
                Thief
                Warrior


            RARE:                               RARE:                                   RARE:

                Grizzly Bear                        Acrobat                                 Mummy 
                Wereboar                            Archer                                  Wraith
                Werewolf                            Barbarian       
                                                    Bard     
                                                    Battlemage       
                                                    Burglar         
                                                    Healer      
                                                    Knight
                                                    Mage
                                                    Monk           
                                                    Nightblade           
                                                    Orc           
                                                    Orc Sergeant
                                                    Ranger
                                                    Rogue  
                                                    Sorcerer
                                                    Spellsword      
                                                    Thief
                                                    Warrior     


            BOSS:                               BOSS:                                   BOSS:

                Assassin                            Lich                                    Vampire Ancient


        MOUNTAIN WOODS, IN TOWN, NIGHT:     MOUNTAIN WOODS, NOT IN TOWN, DAY:       MOUNTAIN WOODS, NOT IN TOWN, NIGHT:

            COMMON:                             COMMON:                                 COMMON:

                Barbarian                           Centaur                                 Barbarian             
                Ranger                              Giant                                   Grizzly Bear       
                                                    Grizzly Bear                            Ranger      
                                                    Harpy                                   Skeletal Warrior         
                                                    Spriggan                                Spriggan        
                                                                                            Zombie          
                                                             

            UNCOMMON:                                                                   UNCOMMON:

                Acrobat                                                                     Ghost       
                Archer                                                                      Ice Atronach  
                Bard                                                                                    
                Battlemage                                                                          
                Burglar                                                                             
                Healer                                                                              
                Knight                                                                                  
                Mage           
                Monk          
                Nightblade           
                Rogue   
                Sorcerer  
                Spellsword
                Thief
                Warrior


            RARE:                               RARE:                                   RARE:

                Grizzly Bear                        Acrobat                                 Frost Daedra
                Wereboar                            Archer                                  Gargoyle        
                Werewolf                            Barbarian                               Mummy   
                                                    Bard                                    Wereboar
                                                    Battlemage                              Werewolf
                                                    Burglar                                 Wraith
                                                    Healer      
                                                    Knight
                                                    Mage
                                                    Monk           
                                                    Nightblade           
                                                    Orc           
                                                    Orc Sergeant
                                                    Ranger
                                                    Rogue  
                                                    Sorcerer
                                                    Spellsword      
                                                    Thief
                                                    Warrior     


            BOSS:                               BOSS:                                   BOSS:

                Assassin                            Lich                                    Ancient Lich


        RAINFOREST, IN TOWN, NIGHT:         RAINFOREST, NOT IN TOWN, DAY:           RAINFOREST, NOT IN TOWN, NIGHT:

            COMMON:                             COMMON:                                 COMMON:

                Barbarian                           Nymph                                   Orc         
                Ranger                              Orc                                     Rat                 
                                                    Sabertooth Tiger                        Skeletal Warrior            
                                                    Spider                                  Spider          
                                                    Spriggan                                Spriggan
                                                                                            Zombie


            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Acrobat                             Daedroth                                Daedroth                       
                Archer                              Orc Sergeant                            Ghost           
                Bard                                                                        Giant Bat
                Battlemage                                                                  Sabertooth Tiger
                Burglar   
                Healer
                Knight     
                Mage            
                Monk            
                Nightblade              
                Rogue   
                Sorcerer
                Spellsword
                Thief   
                Warrior


            RARE:                               RARE:                                   RARE:

                Daedroth                            Acrobat                                 Acrobat         
                Spider                              Archer                                  Archer          
                Wereboar                            Barbarian                               Barbarian           
                Werewolf                            Bard                                    Bard            
                                                    Battlemage                              Battlemage              
                                                    Burglar                                 Burglar             
                                                    Daedra Seducer                          Healer  
                                                    Healer                                  Knight      
                                                    Knight                                  Mage    
                                                    Mage                                    Monk
                                                    Monk                                    Mummy       
                                                    Nightblade                              Nightblade          
                                                    Orc Shaman                              Orc Sergeant
                                                    Ranger                                  Orc Shaman
                                                    Rogue                                   Ranger
                                                    Sorcerer                                Rogue       
                                                    Spellsword                              Sorcerer
                                                    Thief                                   Spellsword
                                                    Warrior                                 Thief   
                                                                                            Warrior
                                                                                            Wereboar
                                                                                            Werewolf
                                                                                            Wraith


            BOSS:                               BOSS:                                   BOSS:

                Dragonling_Alternate                Orc Warlord                             Daedra Lord


        SUBTROPICAL, IN TOWN, NIGHT:        SUBTROPICAL, NOT IN TOWN, DAY:          SUBTROPICAL, NOT IN TOWN, NIGHT:

            COMMON:                             COMMON:                                 COMMON:

                Barbarian                           Nymph                                   Giant Bat       
                Ranger                              Orc                                     Orc     
                                                    Sabertooth Tiger                        Rat
                                                    Spider                                  Skeletal Warrior
                                                    Spriggan                                Spider       
                                                                                            Zombie


            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Acrobat                             Centaur                                 Ghost                 
                Archer                              Daedroth                                Sabertooth Tiger
                Bard                                Orc Sergeant                            Spriggan
                Battlemage                          Rat         
                Burglar     
                Healer
                Knight     
                Mage            
                Monk            
                Nightblade              
                Rogue   
                Sorcerer
                Spellsword
                Thief
                Warrior


            RARE:                               RARE:                                   RARE:

                Sabertooth Tiger                    Acrobat                                 Acrobat
                Wereboar                            Archer                                  Archer         
                Werewolf                            Barbarian                               Barbarian
                                                    Bard                                    Bard         
                                                    Battlemage                              Battlemage          
                                                    Burglar                                 Burglar
                                                    Daedra Seducer                          Daedroth
                                                    Healer                                  Healer  
                                                    Knight                                  Knight      
                                                    Mage                                    Mage    
                                                    Monk                                    Monk
                                                    Nightblade                              Mummy       
                                                    Orc Shaman                              Nightblade          
                                                    Ranger                                  Orc Sergeant
                                                    Rogue                                   Orc Shaman
                                                    Sorcerer                                Ranger
                                                    Spellsword                              Rogue   
                                                    Thief                                   Sorcerer
                                                    Warrior                                 Spellsword
                                                                                            Thief   
                                                                                            Warrior
                                                                                            Wereboar
                                                                                            Werewolf
                                                                                            Wraith


            BOSS:                               BOSS:                                   BOSS:

                Assassin                            Daedra Lord                             Vampire Ancient


        SWAMP, IN TOWN, NIGHT:              SWAMP, NOT IN TOWN, DAY:                SWAMP, NOT IN TOWN, NIGHT:     

            COMMON:                             COMMON:                                 COMMON:

                Barbarian                           Nymph                                   Orc         
                Ranger                              Orc                                     Rat                 
                                                    Sabertooth Tiger                        Skeletal Warrior            
                                                    Spider                                  Spider          
                                                    Spriggan                                Spriggan
                                                                                            Zombie


            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Acrobat                             Daedroth                                Daedroth                       
                Archer                              Orc Sergeant                            Ghost           
                Bard                                                                        Giant Bat
                Battlemage                                                                  Sabertooth Tiger
                Burglar   
                Healer
                Knight     
                Mage            
                Monk            
                Nightblade              
                Rogue   
                Sorcerer
                Spellsword
                Thief   
                Warrior


            RARE:                               RARE:                                   RARE:

                Daedroth                            Acrobat                                 Acrobat         
                Spider                              Archer                                  Archer          
                Wereboar                            Barbarian                               Barbarian           
                Werewolf                            Bard                                    Bard            
                                                    Battlemage                              Battlemage              
                                                    Burglar                                 Burglar             
                                                    Daedra Seducer                          Healer  
                                                    Healer                                  Knight      
                                                    Knight                                  Mage    
                                                    Mage                                    Monk
                                                    Monk                                    Mummy       
                                                    Nightblade                              Nightblade          
                                                    Orc Shaman                              Orc Sergeant
                                                    Ranger                                  Orc Shaman
                                                    Rogue                                   Ranger
                                                    Sorcerer                                Rogue       
                                                    Spellsword                              Sorcerer
                                                    Thief                                   Spellsword
                                                    Warrior                                 Thief   
                                                                                            Warrior
                                                                                            Wereboar
                                                                                            Werewolf
                                                                                            Wraith


            BOSS:                               BOSS:                                   BOSS:

                Dragonling_Alternate                Orc Warlord                             Daedra Lord


        WOODLANDS, IN TOWN, NIGHT:          WOODLANDS, NOT IN TOWN, DAY:            WOODLANDS, NOT IN TOWN, NIGHT:

            COMMON:                             COMMON:                                 COMMON:
            
                Barbarian                           Centaur                                 Giant Bat    
                Ranger                              Giant                                   Grizzly Bear
                                                    Grizzly Bear                            Rat       
                                                    Orc                                     Skeletal Warrior
                                                    Rat                                     Zombie      
                                                    Spriggan    


            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Acrobat                             Spider                                  Ghost    
                Archer                                                                      Orc  
                Bard                                                                        Spriggan
                Battlemage        
                Burglar   
                Healer
                Knight      
                Mage            
                Monk         
                Nightblade           
                Rogue            
                Sorcerer
                Spellsword
                Thief
                Warrior


            RARE:                               RARE:                                   RARE:

                Spriggan                            Acrobat                                 Acrobat             
                Wereboar                            Archer                                  Archer
                Werewolf                            Barbarian                               Barbarian
                                                    Bard                                    Bard      
                                                    Battlemage                              Battlemage
                                                    Burglar                                 Burglar
                                                    Healer                                  Healer  
                                                    Knight                                  Knight      
                                                    Mage                                    Mage    
                                                    Monk                                    Monk     
                                                    Nightblade                              Mummy
                                                    Orc Sergeant                            Nightblade
                                                    Ranger                                  Orc Sergeant
                                                    Rogue                                   Ranger      
                                                    Sorcerer                                Rogue
                                                    Spellsword                              Sorcerer    
                                                    Thief                                   Spellsword
                                                    Warrior                                 Spider
                                                                                            Thief
                                                                                            Warrior
                                                                                            Wereboar
                                                                                            Werewolf
                                                                                            Wraith


            BOSS:                               BOSS:                                   BOSS:

                Vampire                             Dragonling_Alternate                    Orc Warlord


    HAND-TO-HAND:

        - Punch attacks are buffed by equipped glove material, kick attacks are buffed by equipped boot material.

        MATERIAL TO-HIT BUFFS:

            Iron:       -10
            Elven:      +10
            Dwarven:    +20
            Mithril:    +30
            Adamantium: +30
            Ebony:      +40
            Orcish:     +50
            Daedric:    +60


        Iron, Silver, Elven,                Steel Gauntlets/Boots:                  Mithril, Adamantium, Ebony,
        Dwarven Gauntlets/Boots:                                                    Orcish, Daedric Gauntlets/Boots:

            Gauntlets: Break in 205 hits        Gauntlets: Break in 308 hits            Gauntlets: Break in 103 hits
            Boots: Break in 205 hits            Boots: Break in 308 hits                Boots: Break in 103 hits


    HUMAN ENEMY ARMOR:

        ARMOR RATING = 60 - (human enemy level * 2)


        - Enemy's ARMOR RATING is the base to-hit chance player has against them, but since many other factors influence player's
          to-hit chance, ARMOR RATING only serves as a general indicator of how difficult an enemy is to hit.

        - Human enemy ARMOR RATING has a cap of 0 at level 30, but only Guards (HALT!) and boss Assassins will hit that cap. All
          other human enemies have an ARMOR RATING cap of 20 at enemy level 20.

        - For comparison, Imps have an ARMOR RATING of 65, Daedra Seducers have an ARMOR RATING of 15, and Ancient Liches
          have an ARMOR RATING of -25.

        - Equipped armor does not change enemy's ARMOR RATING.


    HUMAN ENEMY DAMAGE:

        - These are average damages. These tables will not be very accurate when enemies are level 1-5, as that is when enemies
          usually use weapons. Around level 6 enemies switch to Hand-to-Hand and their damage output becomes more consistent.

        - At level 22 and higher damage scales up more slowly. Enemies hit a Hand-to-Hand skill cap at level 22.

        Level 1:  10                            Level 11: 30                            Level 21: 50
        Level 2:  12                            Level 12: 32                            Level 22: 51
        Level 3:  14                            Level 13: 34                            Level 23: 52
        Level 4:  16                            Level 14: 36                            Level 24: 53
        Level 5:  18                            Level 15: 38                            Level 25: 54
        Level 6:  20                            Level 16: 40                            Level 26: 55
        Level 7:  22                            Level 17: 42                            Level 27: 56
        Level 8:  24                            Level 18: 44                            Level 28: 57
        Level 9:  26                            Level 19: 46                            Level 29: 58
        Level 10: 28                            Level 20: 48                            Level 30: 59


    HUMAN ENEMY LEVELS:

        - Guards (HALT!) follow standard level rules, but they also get a random level boost of 0 to 10. This is true at
          any player level.


        If player is level 1-6, all non-Guard (HALT!) human enemy levels will be set using the following formula:

            Player's level + a random integer from -2 to 2


        Once player is level 7 or higher, every non-boss human enemy has the following chance to be the stated level:

            Level 1:  1/3%
            Level 2:  1/3%
            Level 3:  1/3%
            Level 4:    1%
            Level 5:    1%
            Level 6:    3%
            Level 7:    5%
            Level 8:    9%
            Level 9:   13%
            Level 10:  17%
            Level 11:  17%
            Level 12:  13%
            Level 13:   9%
            Level 14:   5%
            Level 15:   3%
            Level 16:   1%
            Level 17:   1%
            Level 18: 1/3%
            Level 19: 1/3%
            Level 20: 1/3% 


    HUMAN ENEMY PACIFICATION:

        BY ETIQUETTE:                       BY STREETWISE:

            Archer                              Acrobat
            Bard                                Assassin
            Battlemage                          Barbarian
            Guard (HALT!)                       Burglar
            Healer                              Nightblade
            Knight                              Ranger
            Mage                                Rogue
            Monk                                Sorcerer
            Spellsword                          Thief
            Warrior


    INTERIOR ENCOUNTER TABLES:

        BARBARIAN STRONGHOLD:               LABORATORY:                             VAMPIRE HAUNT:                

            COMMON:                             COMMON:                                 COMMON:

                Barbarian                           Fire Atronach                           Ghost      
                Giant Bat                           Flesh Atronach                          Vampire                 
                Rat                                 Gargoyle                                Wraith 
                                                    Ice Atronach 
                                                    Imp         
                                                    Iron Atronach
                
                                                
            UNCOMMON:                           UNCOMMON:                                        
                
                Harpy                               Battlemage                                                          
                Healer                              Healer
                                                    Mage      
                                                    Nightblade         
                                                    Sorcerer  
                                                    Spellsword


            BOSS:                               BOSS:                                   BOSS:

                Assassin                            Orc Warlord                             Vampire Ancient


        CEMETERY:                           MINE:                                   VOLCANIC CAVES:             

            COMMON:                             COMMON:                                 COMMON:

                Burglar                             Giant Bat                               Dragonling   
                Giant Bat                           Rat                                     Fire Atronach
                Rat                                 Spider                                  Fire Daedra
                Rogue                                                                       
                Thief  
                Zombie


            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Acrobat                             Gargoyle                                Daedra Seducer
                Bard                                Iron Atronach                           Daedroth
                Skeletal Warrior


            RARE:                                                                            

                Ghost                                                                               
                Mummy                                           


            BOSS:                               BOSS:                                   BOSSES:

                Vampire                             Lich                                    Daedra Lord
                                                                                            Dragonling_Alternate


        COVEN:                              NATURAL CAVE:                           DEFAULT BUILDING:            

            COMMON:                             COMMON:                                 COMMON:

                Daedra Seducer                      Giant Bat                               Rat
                Daedroth                            Grizzly Bear                                                        
                Fire Daedra                         Rat                                             
                Frost Daedra                        Spider                                          
                Zombie
                                                
                                                                                                  
            UNCOMMON:                           UNCOMMON:                               UNCOMMON:
                                                                                                      
                Battlemage                          Nymph                                   Acrobat          
                Ghost                               Ranger                                  Archer       
                Healer                              Spriggan                                Barbarian
                Mage                                                                        Bard       
                Mummy                                                                       Battlemage       
                Nightblade                                                                  Burglar     
                Skeletal Warrior                                                            Giant Bat       
                Sorcerer                                                                    Healer       
                Spellsword                                                                  Knight        
                Wraith                                                                      Mage
                                                                                            Monk           
                                                                                            Nightblade         
                                                                                            Ranger           
                                                                                            Rogue  
                                                                                            Sorcerer
                                                                                            Spellsword
                                                                                            Thief
                                                                                            Warrior


            BOSS:                               BOSS:                                   BOSS:

                Daedra Lord                         Vampire                                 Assassin


        CRYPT:                              ORC STRONGHOLD:                         GUILDHALL:                                  

            COMMON:                             COMMON:                                 COMMON:

                Ghost                               Orc                                     Rat
                Giant Bat                           Orc Sergeant
                Mummy                               Orc Shaman
                Rat
                Skeletal Warrior                                                                    
                Zombie                                                                                                
                                                                                                    
                                                                                                                                              
            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Wraith                              Giant Bat                               Acrobat
                                                    Rat                                     Archer
                                                                                            Barbarian
                                                                                            Bard
                                                                                            Battlemage
                                                                                            Burglar
                                                                                            Giant Bat
                                                                                            Healer
                                                                                            Knight
                                                                                            Mage
                                                                                            Monk
                                                                                            Nightblade
                                                                                            Ranger
                                                                                            Rogue
                                                                                            Sorcerer
                                                                                            Spellsword
                                                                                            Thief
                                                                                            Warrior


            BOSS:                               BOSS:                                   BOSS:

                Vampire Ancient                     Orc Warlord                             Dragonling_Alternate


        DESECRATED TEMPLE:                  PRISON:                                 HOUSE 1:                            

            COMMON:                             COMMON:                                 COMMON:

                Imp                                 Acrobat                                 Rat
                Mage                                Burglar           
                Rat                                 Giant Bat         
                Sorcerer                            Nightblade           
                                                    Rat    
                                                    Rogue  
                                                    Sorcerer
                                                    Spellsword
                                                    Thief    
                
                                                                                                                                             
            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Barbarian                           Spider                                  Acrobat
                Daedra Seducer                                                              Archer
                Giant Bat                                                                   Barbarian
                Harpy                                                                       Bard
                Monk                                                                        Battlemage
                Mummy                                                                       Burglar
                Nightblade                                                                  Giant Bat
                Rogue                                                                       Healer
                Skeletal Warrior                                                            Knight
                Zombie                                                                      Mage
                                                                                            Monk
                                                                                            Nightblade
                                                                                            Ranger
                                                                                            Rogue
                                                                                            Sorcerer
                                                                                            Spellsword
                                                                                            Thief
                                                                                            Warrior
                                                                                          

            BOSS:                               BOSS:                                   BOSS:

                Ancient Lich                        Assassin                                Daedra Lord


        DRAGON'S DEN:                       RUINED CASTLE:                          HOUSE 2:                       

            COMMON:                             COMMON:                                 COMMON:

                Dragonling                          Giant Bat                               Rat
                Knight                              Rat

                                                                                                                                                                                        
            UNCOMMON:                           UNCOMMON:                               UNCOMMON:

                Mage                                Acrobat                                 Acrobat
                                                    Archer                                  Archer
                                                    Bard                                    Barbarian
                                                    Battlemage                              Bard
                                                    Burglar                                 Battlemage
                                                    Healer                                  Burglar
                                                    Knight                                  Giant Bat
                                                    Mage                                    Healer
                                                    Monk                                    Knight
                                                    Nightblade                              Mage
                                                    Rogue                                   Monk
                                                    Sorcerer                                Nightblade
                                                    Spellsword                              Ranger
                                                    Spider                                  Rogue
                                                    Thief                                   Sorcerer
                                                    Warrior                                 Spellsword
                                                                                            Thief
                                                                                            Warrior


            BOSS:                               BOSS:                                   BOSS:

                Dragonling_Alternate                Assassin                                Vampire         


        GIANT STRONGHOLD:                   SCORPION NEST:                          HOUSE 3:                                    

            COMMON:                             COMMON:                                 COMMON:

                Gargoyle                            Giant Bat                               Rat
                Giant                               Giant Scorpion
                Giant Bat
                Rat

                               
                                                UNCOMMON:                               UNCOMMON:

                                                    Rat                                     Acrobat
                                                                                            Archer
                                                                                            Barbarian
                                                                                            Bard
                                                                                            Battlemage
                                                                                            Burglar
                                                                                            Giant Bat
                                                                                            Healer
                                                                                            Knight
                                                                                            Mage
                                                                                            Monk
                                                                                            Nightblade
                                                                                            Ranger
                                                                                            Rogue
                                                                                            Sorcerer
                                                                                            Spellsword
                                                                                            Thief
                                                                                            Warrior
                                                                                          

            BOSS:                               BOSS:                                   BOSS:

                Vampire                             Lich                                    Ancient Lich


        HARPY NEST:                         SPIDER NEST:                            PALACE:                                 

            COMMON:                             COMMON:                                 COMMON:

                Giant Bat                           Skeletal Warrior                        Rat
                Harpy                               Spider                                            
                Rat                                 Zombie
                Spider


                                                UNCOMMON:                               UNCOMMON:

                                                    Ghost                                   Acrobat
                                                    Mummy                                   Archer
                                                                                            Barbarian
                                                                                            Bard
                                                                                            Battlemage
                                                                                            Burglar
                                                                                            Giant Bat
                                                                                            Healer
                                                                                            Knight
                                                                                            Mage
                                                                                            Monk
                                                                                            Nightblade
                                                                                            Ranger
                                                                                            Rogue
                                                                                            Sorcerer
                                                                                            Spellsword
                                                                                            Thief
                                                                                            Warrior
                                                                                            

            BOSS:                               BOSS:                                   BOSS:

                Lich                                Ancient Lich                            Daedra Lord


        HUMAN STRONGHOLD:                   UNDERWATER:                             TEMPLE:                                    

            COMMON:                             COMMON:                                 COMMON:

                Archer                              Dreugh                                  Rat
                Bard                                Slaughterfish
                Battlemage
                Giant Bat
                Healer
                Knight
                Mage
                Monk
                Rat
                Warrior                                                                                                 
                
                                                
                                                UNCOMMON:                               UNCOMMON:

                                                    Skeletal Warrior                        Acrobat
                                                                                            Archer
                                                                                            Barbarian
                                                                                            Bard
                                                                                            Battlemage
                                                                                            Burglar
                                                                                            Giant Bat
                                                                                            Healer
                                                                                            Knight
                                                                                            Mage
                                                                                            Monk
                                                                                            Nightblade
                                                                                            Ranger
                                                                                            Rogue
                                                                                            Sorcerer
                                                                                            Spellsword
                                                                                            Thief
                                                                                            Warrior
                                                                                          

                                                RARE:                                   

                                                    Ghost
                                                    Ice Atronach
                                                    Lamia
                                                    Wraith
                                                    Zombie
                                                                                
                                                
            BOSS:                               BOSS:                                   BOSS:

                Assassin                            Ancient Lich                            Orc Warlord


    ITEM GENERATION:

        - This section contains all item generation formulas I added or modified, and any vanilla data or formulas necessary
          to understand my changes. It is not an exhaustive reference for all vanilla item generation formulas.

        ENEMY EQUIPMENT:

            ENEMIES THAT SPAWN WITH EQUIPMENT:

                Orc Sergeant
                Orc Shaman
                Orc Warlord
                Frost Daedra
                Fire Daedra
                Daedroth
                Vampire
                Daedra Seducer
                Vampire Ancient
                Daedra Lord
                Lich
                Ancient Lich
                Dragonling_Alternate
                Any human


            EQUIPMENT SELECTION:

                - There are three equipment variants. They determine most equipment spawn chances. All enemies that use equipment
                  will randomly use one of these three variants, and each spawned enemy can use a different variant. For example,
                  a given Mage may use equipment variant 1, but the next Mage spawned may use equipment variant 2.


            EQUIPMENT VARIANT 1:                        EQUIPMENT VARIANT 2:                        EQUIPMENT VARIANT 3:

                Randomly choose one:                        Randomly choose one:                        Randomly choose one:

                    Broadsword                                  Claymore                                    Claymore
                    Saber                                       Dai-Katana                                  Dai-Katana
                    Longsword                                   Mace                                        Mace
                    Katana                                      Flail                                       Flail
                                                                Warhammer                                   Warhammer
                                                                Battle Axe                                  Battle Axe
                30% chance for one random:                      War Axe                                     War Axe
                                                                Short Bow                                   Short Bow
                    Buckler                                     Long Bow                                    Long Bow
                    Round Shield
                    Kite Shield
                    Tower Shield

                
                If no shield, 30% chance for
                one random:                      
                                                            
                    Dagger
                    Tanto
                    Staff
                    Shortsword
                    Wakazashi


                30% chance for each item:                   45% chance for each item:                   60% chance for each item:

                    Helm                                        Helm                                        Helm
                    Right Pauldron                              Right Pauldron                              Right Pauldron
                    Left Pauldron                               Left Pauldron                               Left Pauldron
                    Cuirass                                     Cuirass                                     Cuirass
                    Greaves                                     Greaves                                     Greaves
                    Boots                                       Boots                                       Boots
                    Gauntlets                                   Gauntlets                                   Gauntlets


        GOLD GENERATION:

            ENEMY GOLD:

                (
                    Enemy level / 2 rounded down to nearest integer
                    + Random integer from -5 to 5
                    + Random integer from -5 to 5
                    + Random integer from -5 to 5
                )

                * Random integer from 1 to "Gold" maxiumum from LOOT CHANCES

                = Gold


            LOOT PILE GOLD:

                (
                    Random integer from -5 to 5
                    + Random integer from -5 to 5
                    + Random integer from -5 to 5
                    + Random integer from -5 to 5
                )

                * Random integer from 1 to "Gold" maxiumum from LOOT CHANCES

                = Gold


                - If the above gold formula generates nothing and if all other item generation formulas generate nothing,
                  1 to 20 gold pieces will be added, number of pieces is random. This is done to avoid empty loot piles.


        INGREDIENT LISTS:

            Creature Ingredient List 1:             Miscellaneous Ingredient List 1:            Plant Ingredient List 2:
                
                Werewolf's Blood                        Holy Relic                                  Twigs
                Fairy Dragon's Scales                   Big Tooth                                   Green Leaves
                Wraith Essence                          Medium Tooth                                Red Flowers
                Ectoplasm                               Small Tooth                                 Yellow Flowers
                Ghoul's Tongue                          Pure Water                                  Root Tendrils
                Spider's Venom                          Rain Water                                  Root Bulb
                Troll's Blood                           Elixir Vitae                                Green Berries
                Snake Venom                             Nectar                                      Red Berries
                Gorgon Snake                            Ichor                                       Yellow Berries
                Lich Dust                                                                           Black Rose
                Giant's Blood                                                                       White Rose
                Basilisk's Eye                      Miscellaneous Ingredient List 2:                Black Poppy
                Daedra's Heart                                                                      White Poppy
                Saint's Hair                            Ivory                                       Ginkgo Leaves
                Orc's Blood                             Pearl                                       Bamboo
                                                                                                    Palm
                                                                                                    Aloe
            Creature Ingredient List 2:             Plant Ingredient List 1:                        Fig
                                                                                                    Cactus
                Dragon's Scales                         Twigs
                Giant Scorpion Stinger                  Green Leaves
                Small Scorpion Stinger                  Red Flowers
                Mummy Wrappings                         Yellow Flowers
                Gryphon's Feather                       Root Tendrils
                                                        Root Bulb
                                                        Pine Branch
            Creature Ingredient List 3:                 Green Berries
                                                        Red Berries
                Wereboar's Tusk                         Yellow Berries
                Nymph Hair                              Clover
                Unicorn Horn                            Red Rose
                                                        Yellow Rose
                                                        Red Poppy
                                                        Golden Poppy


        LOOT CHANCES:

            LEGEND:

                Enemy Loot Chances

                Dungeon Type Loot Pile Chances

                Location Type Loot Pile Chances

                AM: Armor
                MI: Magic Item
                WP: Weapon

                BK: Book
                CL: Clothing
                RI: Religious Item

                C1: Creature Ingredient List 1
                C2: Creature Ingredient List 2
                C3: Creature Ingredient List 3
                M1: Miscellaneous Ingredient List 1
                M2: Miscellaneous Ingredient List 2
                P1: Plant Ingredient List 1
                P2: Plant Ingredient List 2


            Orc Stronghold                  Crypt                               Daedra Lord                 Frost Daedra
            Human Stronghold                Desecrated Temple                   Lich                        Fire Daedra
            Prison                          Vampire Haunt                       Ancient Lich
            Ruined Castle                                                                                       Gold: 1-150
            Barbarian Stronghold            City (walled town)                  Dragon's Den                    AM:       5
            Cemetery                        Wealthy Home (Manor, etc.)                                          MI:       3
                                                                                    Gold: 1-125                 WP:       5
            Hamlet (medium town)                Gold:  1-10                         AM:      10
            Village (small town)                AM:       5                         MI:       3                 
            Farm (Grange, etc.)                 MI:       3                         WP:      10             Giant
            Poor Home (Hovel, etc.)             WP:       5
                                                                                    BK:       5             Giant Stronghold
                Gold:  1-80                     BK:       5
                AM:       5                     RI:     100                         C1:       5                 Gold:  1-30
                MI:       1                                                         C2:       5                 AM:      50
                WP:       5                     C1:       3                         C3:       2                 MI:       1
                                                C2:       3                         M1:       7                 WP:      50
                BK:       5                     C3:       1                         M2:       2
                CL:      20                     M1:       1                         P1:       5                 C1:       5
                RI:       5                     M2:       1                         P2:       5                 C2:       5
                                                P1:       3                                                     C3:       2
                C1:       5                     P2:       3                                                     M1:       1
                C2:       5                                                     Imp                             M2:       1
                C3:       2                                                     Harpy                           P1:       2
                M1:       2                 Mine                                                                P2:       2
                M2:       1                 Natural Cave                        Harpy Nest
                P1:       5                 Volcanic Caves
                P2:       5                                                         Gold:   1-4             Spider Nest
                                            Temple (light blue dot)                                         Scorpion Nest
                                            Tavern (dark brown dot)                 RI:       4
            Orc Shaman                                                                                          Gold:  1-20
            Mage                                Gold:  1-15                         C1:       6                 AM:      50
            Battlemage                          AM:      10                         C2:       6                 MI:       1
            Sorcerer                            MI:       1                         C3:       3                 WP:      50
            Healer                              WP:      10                         M1:       3
            Nightblade                                                              P1:       6                 CL:      75
                                                BK:       2                         P2:       6                 RI:       3
            Laboratory                          CL:      15
                                                RI:       1                                                     C1:       3
                Gold:  1-30                                                     Orc                             C2:       3
                AM:      10                     C1:       1                     Orc Sergeant                    C3:       1
                MI:       2                     C2:       1                                                     M1:       1
                WP:      10                     C3:       1                         Gold:  1-10                 M2:       2
                                                M1:       1                         AM:       5
                BK:       2                     M2:       1                         MI:       2
                RI:      10                     P1:       1                         WP:       5             Spellsword
                                                P2:       1
                C1:       5                                                         CL:       4                 Gold:  1-20
                C2:       5                                                                                     AM:       5
                C3:       2                 Centaur                                 M2:       1                 MI:       2
                M1:       5                 Nymph                                                               WP:      10
                M2:       1                 Archer
                P1:       5                 Ranger                              Dreugh                          BK:      10
                P2:       5                                                     Lamia
                                                Gold:  1-20                                                     C1:       5
                                                AM:       5                         Gold:  1-20                 C2:       5
            Bard                                MI:       3                         AM:       5                 C3:       2
            Burglar                             WP:      25                         MI:       2                 M1:       2
            Rogue                                                                   WP:      15                 M2:       2
            Acrobat                             BK:       2                                                     P1:       5
            Thief                               RI:       2                         C1:       3                 P2:       5
            Assassin                                                                C2:       3
                                                C1:       5                         C3:       1
                Gold:  1-20                     C2:       5                         M1:       2             Skeletal Warrior
                AM:      10                     C3:       2
                MI:       2                     M1:       2                                                     Gold:  1-10
                WP:      15                     M2:       1                     Ghost                           MI:       1
                                                P1:       5                     Wraith                          WP:     100
                C1:       1                     P2:       5
                C2:       1                                                         MI:       2                 CL:       2
                C3:       1
                M1:       1                 Vampire                                 RI:       5
                P1:       1                 Daedra Seducer                                                  Zombie
                P2:       1                 Vampire Ancient
                                                                                Mummy                           Gold:  1-15
                                            Coven                               Daedroth                        AM:      50
            Orc Warlord                                                                                         MI:       1
            Monk                                Gold:  1-80                         Gold:  1-80                 WP:      50
            Barbarian                           AM:      10                         AM:      10
            Warrior                             MI:       3                         MI:       3                 CL:       5
            Knight                              WP:      25                         WP:      10
                                                                                                                M2:       1
                Gold:  1-80                     BK:       5                         BK:       2
                AM:     100                     CL:      35                         CL:       4
                MI:       1                                                         RI:      15             Spriggan
                WP:     100                     C1:       8
                                                C2:       8                         M2:       1                 P1:      10
                                                C3:       4                                                     P2:      10
                                                M1:       1
                                                M2:       1
                                                P1:       2
                                                P2:       2


        LOOT GENERATION:

            - This covers most loot generation formulas for enemies and loot piles. Enemy equipment generation is covered elsewhere.

            ARMOR GENERATION:

                CHANCE = AM value from LOOT CHANCES

                START
                    NUMBER = generate a random integer from 1 to 100

                IF (NUMBER is equal to or less than CHANCE)
                    Generate one random piece of armor, material selection chances are found in MATERIAL CHANCES
                    Halve CHANCE, round down to nearest integer, then repeat section from START downward
                ELSE
                    End of armor generation process


            BOOK GENERATION:

                CHANCE = BK value from LOOT CHANCES

                START
                    NUMBER = generate a random integer from 1 to 100

                IF (NUMBER is equal to or less than CHANCE)
                    Generate one random book
                    Halve CHANCE, round down to nearest integer, then repeat section from START downward
                ELSE
                    End of book generation process


            CLOTHING GENERATION:

                CHANCE = CL value from LOOT CHANCES

                START
                    NUMBER = generate a random integer from 1 to 100

                IF (NUMBER is equal to or less than CHANCE)
                    Generate one random article of clothing
                    Halve CHANCE, round down to nearest integer, then repeat section from START downward
                ELSE
                    End of clothing generation process


            INGREDIENT GENERATION:

                ENEMY INGREDIENT GENERATION:

                    VALUE = C1, C2, C3, M1, M2, P1, or P2 value from LOOT CHANCES

                    MULTIPLIER = Enemy level / 2 rounded down to nearest integer
                                 + Random integer from -5 to 5
                                 + Random integer from -5 to 5
                                 + Random integer from -5 to 5

                    CHANCE = VALUE * MULTIPLIER

                    START
                        NUMBER = generate a random integer from 1 to 100

                    IF (NUMBER is equal to or less than CHANCE)
                        Generate one random ingredient from current ingredient list
                        Halve CHANCE, round down to nearest integer, then repeat section from START downward
                    ELSE
                        End of ingredient generation process for current ingredient list


                    - The above process is performed for every ingredient list that has a value in LOOT CHANCES.


                LOOT PILE INGREDIENT CHANCES:

                    VALUE = C1, C2, C3, M1, M2, P1, or P2 value from LOOT CHANCES

                    MULTIPLIER = Random integer from -5 to 5
                                 + Random integer from -5 to 5
                                 + Random integer from -5 to 5
                                 + Random integer from -5 to 5

                    CHANCE = VALUE * MULTIPLIER

                    START
                        NUMBER = generate a random integer from 1 to 100

                    IF (NUMBER is equal to or less than CHANCE)
                        Generate one random ingredient from current ingredient list
                        Halve CHANCE, round down to nearest integer, then repeat section from START downward
                    ELSE
                        End of ingredient generation process for current ingredient list


                    - The above process is performed for every ingredient list that has a value in LOOT CHANCES.


            MAGIC ITEM GENERATION:

                CHANCE = MI value from LOOT CHANCES

                START
                    NUMBER = generate a random integer from 1 to 100

                IF (NUMBER is equal to or less than CHANCE)
                    Generate one random magic item, if magic item is armor or a weapon, material selection chances are found
                    in MATERIAL CHANCES
                    Halve CHANCE, round down to nearest integer, then repeat section from START downward
                ELSE
                    End of magic item generation process


            POTION GENERATION:

                ENEMY POTION GENERATION:

                    - If enemy has a section in LOOT CHANCES, they also have a 4% chance to drop a random potion.


            RELIGIOUS ITEM GENERATION:

                CHANCE = RI value from LOOT CHANCES

                START
                    NUMBER = generate a random integer from 1 to 100

                IF (NUMBER is equal to or less than CHANCE)
                    Generate one random religious item
                    Halve CHANCE, round down to nearest integer, then repeat section from START downward
                ELSE
                    End of religious item generation process


            WEAPON GENERATION:

                CHANCE = WP value from LOOT CHANCES

                START
                    NUMBER = generate a random integer from 1 to 100

                IF (NUMBER is equal to or less than CHANCE)
                    Generate one random weapon, material selection chances are found in MATERIAL CHANCES
                    Halve CHANCE, round down to nearest integer, then repeat section from START downward
                ELSE
                    End of weapon generation process


        MATERIAL CHANCES:

            ARMOR MATERIAL CHANCES:

                Leather: 70%
                Chain:   20%
                Plate:   10%


            STANDARD PLATE AND WEAPON MATERIAL CHANCES:

                Iron: 328/1025     32.0%
                Steel: 654/1025    63.8%
                Silver: 8/1025      0.8%
                Elven: 12/1025      1.2%
                Dwarven: 8/1025     0.8%
                Mithril: 5/1025     0.5%
                Adamantium: 4/1025  0.4%
                Ebony: 3/1025       0.3%
                Orcish: 2/1025      0.2%
                Daedric: 1/1025     0.1%


            - For each enemy level above 15, decrement lowest material tier by 50, increment Daedric by 50. For example, enemy
              level 17 material generation chances are:

                Iron: 228/1025     22.2%
                Steel: 654/1025    63.8%
                Silver: 8/1025      0.8%
                Elven: 12/1025      1.2%
                Dwarven: 8/1025     0.8%
                Mithril: 5/1025     0.5%
                Adamantium: 4/1025  0.4%
                Ebony: 3/1025       0.3%
                Orcish: 2/1025      0.2%
                Daedric: 101/1025   9.9%


            - The material generation chance cap is as follows at enemy level 30:

                Iron: 0/1025        0.0%
                Steel: 232/1025    22.6%
                Silver: 8/1025      0.8%
                Elven: 12/1025      1.2%
                Dwarven: 8/1025     0.8%
                Mithril: 5/1025     0.5%
                Adamantium: 4/1025  0.4%
                Ebony: 3/1025       0.3%
                Orcish: 2/1025      0.2%
                Daedric: 751/1025  73.3%


        SHOP ITEM GENERATION:

            - Materials for armor and weapons are generated using STANDARD PLATE AND WEAPON MATERIAL CHANCES in MATERIAL CHANCES.

            GENERAL STORE & PAWN SHOP BOOK GENERATION:

                - These checks run when the player activates any General Store or Pawn Shop shelf.


                If store quality message is "Incense Burning":

                    - 25% chance for one book.

                    - 15% chance (checked separately) for a second book.

                    - 5% chance (checked separately) for a third book.


                If store quality message is "Better Appointed":

                    - 15% chance for one book.

                    - 5% chance (checked separately) for a second book.


                If store quality message is "Average":

                    - 5% chance for one book.


            BOOKSTORE BOOK GENERATION:

                - This formula runs when the player activates any Bookstore shelf.

                - Shop quality is an integer between 1 and 20. I use vanilla's shop quality numbers.


                BOOK COUNT = Shop quality / 2 rounded down to nearest integer
                             + Random integer from -7 to 7

                    - BOOK COUNT random books are then generated.

                    - If BOOK COUNT is zero or negative, only one book will be on the shelf.


    MONSTER LEVELS:

        - When enemy is spawned, a random number within the enemy's LEVEL RANGE is selected. Rats and Imps are more likely to be
          level 1 than any other possible level, and Daedra Seducers are more likely to be level 20 than any other possible level.

        LEVEL RANGE:

            Rat:              1-3                       Wereboar:             10-14
            Imp:              1-4                       Werewolf:             10-14
            Giant Bat:        1-5                       Gargoyle:             12-16
            Spriggan:         1-5                       Wraith:               13-17
            Grizzly Bear:     2-6                       Fire Atronach:        14-18
            Sabertooth Tiger: 2-6                       Flesh Atronach:       14-18
            Spider:           2-6                       Ice Atronach:         14-18
            Centaur:          3-7                       Iron Atronach:        14-18
            Orc:              3-7                       Lamia:                14-18
            Nymph:            4-8                       Orc Shaman:           14-18
            Slaughterfish:    5-9                       Fire Daedra:          15-19
            Zombie:           5-9                       Frost Daedra:         15-19
            Dreugh:           6-10                      Daedroth:             16-20
            Harpy:            6-10                      Daedra Seducer:       17-20
            Skeletal Warrior: 6-10                      Lich:                 21-25
            Dragonling:       8-12                      Vampire:              21-25
            Mummy:            8-12                      Dragonling_Alternate: 21-30
            Ghost:            9-13                      Orc Warlord:          21-30
            Orc Sergeant:     9-13                      Ancient Lich:         26-30
            Giant:           10-14                      Daedra Lord:          26-30
            Giant Scorpion:  10-14                      Vampire Ancient:      26-30


    SPIDER & GIANT SCORPION POISON CHANCES:

        - Each hit has a 10% total chance to inflict a vanilla poison. Only one poison may be inflicted per attack.

        0.9% CHANCE PER HIT FOR EACH OF THE FOLLOWING:                  0.1% CHANCE PER HIT, THIS PIERCES POISON IMMUNITY:

            Aegrotat                                                        Drothweed
            Arsenic
            Indulcet
            Magebane
            Moonseed
            Nux Vomica
            Pyrrhic Acid
            Quaesto Vil
            Somnalius
            Sursum
            Thyrwort


v1.3.1 ITEM TABLES:

    ITEM CHANGES, SUMMARY:

        ITEMS NOT SOLD IN STORES:

            LEGEND:

                V: Modified Value
                W: Modified Weight


            BANKING:                                                    GUILD ITEMS:

                Name                  V     W                               Name                  V     W

                House Deed                 0.03                             Potions, All               0.1
                Letter of Credit           0.03                             Soul Trap, Empty   50000   0.0025
                Ship Deed                  0.03                             Spellbook           5000   0.5


            DRUGS:                                                      QUEST ITEMS:

                Name                  V     W                               Name                  V     W

                Aegrotat            5500   0.0025                           Finger                     0.02
                Indulcet             500   0.0025                           Totem                      2.5
                Quaesto Vil         2500   0.0025                           
                Sursum               800   0.0025


        MAGIC ITEMS:

            REGULAR MAGIC ITEMS:

                LEGEND:

                %it: Replaced with actual item name (Longsword, Cuirass, etc.)
                D: Modified Durability
                E: Modified Enchantment and Enchantment Type


                MODIFIED VANILLA MAGIC ITEMS:

                    Name                        D                   Name                        D

                    %it of Far Silence         500                  %it of Undeniable Access   500
                    %it of Featherweight       500                  %it of Venom Antidote      500
                    %it of Force Bolts         500                  %it of Water-walking      1000
                    %it of Friendship         1000                  %it of Wizard's Fire       500
                    %it of Good Luck          1000                  %it, the Protector         500
                    %it of Life Stealing      2500                  Blazing %it of Fireballs   500
                    %it of Lightning           500                  Frosty %it of Ice Storms   500
                    %it of Nimbleness         1000                  Healing %it                500
                    %it of Oblivion           1200                  Leaping %it                500
                    %it of Shocking            500                  Never Tiring %it           500
                    %it of the Orc Lord       1000                  Shining %it                500
                    %it of the Sealed Door     500                  Torgo's %it               1000
                    %it of the Wise           1000                  Unrestrainable %it         500
                    %it of Toxic Clouds        500


                NEW MAGIC ITEMS:

                    Name                        D      E

                    %it of Arcane Ward         500     Spell Resistance  Cast When Used
                    %it of Domestication       500     Tame              Cast When Used
                    %it of Pyromania           500     Fire Storm        Cast When Used
                    %it of Rebounding          500     Shalidor's Mirror Cast When Used
                    %it of Teleportation       500     Recall            Cast When Used
                    Beguiling %it              500     Charm Mortal      Cast When Used
                    Calming %it                500     Calm Humanoid     Cast When Used
                    Medusa's %it               500     Medusa's Gaze     Cast When Used
                    Spirit Soothing %it        500     Quiet Undead      Cast When Used


            SOUL GEMS:

                LEGEND:

                C: Modified Cost
                EP: Modified Enchantment Points


                Name                       C          EP                    Name                       C          EP

                Ancient Lich           2,250,000   -22,500                  Iron Atronach             80,000
                Centaur                   53,000                            Lamia                     60,000
                Daedra Lord            2,250,000   -22,500                  Lich                     750,000    -7,500
                Daedra Seducer           200,000                            Mummy                     60,000
                Daedroth                 150,000    -1,000                  Nymph                     60,000
                Dragonling                50,000                            Orc                       51,000
                Dragonling_Alternate   1,500,000   -15,000                  Orc Sergeant              51,000
                Dreugh                    60,000                            Orc Shaman                53,000
                Fire Atronach             80,000                            Orc Warlord            1,500,000   -15,000
                Fire Daedra              100,000                            Rat                       50,000
                Flesh Atronach            60,000                            Sabertooth Tiger          50,000
                Frost Daedra             100,000                            Skeletal Warrior          50,000
                Gargoyle                  53,000                            Slaughterfish             50,000
                Ghost                     80,000                            Spider                    50,000
                Giant                     53,000                            Spriggan                  51,000
                Giant Bat                 50,000                            Vampire                  750,000    -7,500
                Giant Scorpion            50,000                            Vampire Ancient        2,250,000   -22,500
                Grizzly Bear              50,000                            Wereboar                  51,000
                Harpy                     53,000                            Werewolf                  51,000
                Ice Atronach              80,000                            Wraith                    80,000
                Imp                       51,000                            Zombie                    50,000


        SHOP ITEMS, BY STORE QUALITY:

            - If an item is listed in a store quality tier, it is also possible to find it in all shops of a higher quality tier,
              and not possible to find it in any shops of a lower quality tier.

            LEGEND:

                RUSTY RELICS:     Store Qualities  1-3
                STURDY SHELVES:   Store Qualities  4-7
                AVERAGE:          Store Qualities  8-13
                BETTER APPOINTED: Store Qualities 14-17
                INCENSE BURNING:  Store Qualities 18-20

                V:  Modified Value
                W:  Modified Weight
                EP: Modified Base Enchantment Points


            RUSTY RELICS:                                                   AVERAGE (CONT'D):

                ARMOR:                                                          GEMS:

                    Name                      V        W       EP                   Name                      V        W       EP

                    Boots                                     200                   Jade                     500      0.01
                    Buckler                                   400 
                    Helm                                      250
                                                                                INGREDIENTS:

                CLOTHING:                                                           Name                      V        W       EP

                    Name                      V        W       EP                   Bamboo                    55
                                                                                    Basilisk's Eye            50
                    Armbands                          0.02                          Black Rose                35      0.02
                    Boots                     35      0.3                           Brass                     54      0.01
                    Brassiere                  8      0.05                          Cactus                    75
                    Breeches                  25      0.25                          Copper                    80      0.01
                    Casual Cloak              60                                    Ectoplasm                 65      0.0025
                    Casual Dress              50      0.4                           Fig                       40      0.1
                    Casual Pants              25      0.25                          Giant Scorpion Stinger    75
                    Challenger Straps                 0.05                          Giant's Blood             65      0.0025
                    Champion Straps                   0.05                          Ginkgo Leaves             45      0.0025
                    Loincloth                  8      0.04                          Mummy Wrappings           80      0.05
                    Long Shirt                36      0.25                          Pearl                   1500      0.0025
                    Long Skirt                35      0.2                           Red Rose                  35      0.02
                    Peasant Blouse            35      0.12                          Silver                   100      0.01
                    Sandals                   10      0.03                          Wereboar's Tusk           85      0.5
                    Sash                              0.04                          Werewolf's Blood          90      0.0025
                    Shoes                     20      0.2                           White Rose                35      0.02
                    Short Shirt               18      0.12                          Yellow Rose               35      0.02
                    Short Skirt               25      0.1
                    Short Tunic               25      0.12
                    Straps                            0.04                      JEWELRY:
                    Toga                      12      0.2
                                                                                    Name                      V        W       EP

                GEMS:                                                               Amulet                   450      0.03

                    Name                      V        W       EP
                                                                                MISCELLANEOUS:
                    Amber                    250      0.01
                    Malachite                150      0.01                          Name                      V        W       EP

                                                                                    Map                     5000      0.02 
                INGREDIENTS:

                    Name                      V        W       EP               RELIGIOUS ITEMS:

                    Green Berries                     0.03                          Name                      V        W       EP
                    Green Leaves                      0.04
                    Iron                      10      0.02                          Bell                      80      0.1
                    Lodestone                  4     10.0                           Religious Item                    1.0
                    Medium Tooth               7      0.03
                    Nectar                     8      0.1
                    Orc's Blood                5      0.0025                    WEAPONS:
                    Pine Branch                       0.75
                    Rain Water                 2      0.1                           Name                      V        W       EP
                    Red Berries                       0.03
                    Red Flowers                       0.03                          Claymore                                  550
                    Root Bulb                         0.05                          Flail                                     500
                    Root Tendrils                     0.1                           Longsword                                 450
                    Small Tooth                3      0.01                          Shortsword                                325
                    Snake Venom                8      0.1
                    Spider's Venom            10      0.1
                    Twigs                             0.05                  BETTER APPOINTED:
                    Yellow Berries                    0.03
                    Yellow Flowers                    0.02                      ARMOR:

                                                                                    Name                      V        W       EP
                JEWELRY:
                                                                                    Gauntlets                                 125
                    Name                      V        W       EP                   Tower Shield                             2500

                    Bracer                    45      0.05
                    Cloth Amulet                      0.02                      CLOTHING:
                    Mark                              0.03
                                                                                    Name                      V        W       EP

                LIGHT SOURCES:                                                      Eodoric                  140      0.08
                                                                                    Evening Gown             250      0.75
                    Name                      V        W       EP                   Fancy Armbands            75      0.03
                                                                                    Formal Brassiere         120      0.08
                    Candle                            0.05                          Formal Tunic             175      0.25
                    Lantern                  200      0.5                           Kimono                   165      0.2
                    Oil                       25      0.1                           Vest                     200      0.07
                    Torch                      5      0.25                          Wrap                     155      0.1


                MISCELLANEOUS:                                                  GEMS:

                    Name                      V        W       EP                   Name                      V        W       EP

                    Bandage                           0.03                          Ruby                    2500      0.0025
                    Book                              0.5                           Sapphire                3750      0.0025
                    Parchment                  2      0.02
                    Potion Recipe                     0.02
                                                                                INGREDIENTS:

                RELIGIOUS ITEMS:                                                    Name                      V        W       EP

                    Name                      V        W       EP                   Aloe                     100      0.5
                                                                                    Fairy Dragon's Scales    200
                    Prayer Beads                      0.03                          Gold                     200      0.03
                                                                                    Gorgon Snake             160
                                                                                    Mercury                  300      0.01
                TRANSPORTATION:                                                     Pure Water               140      0.1
                                                                                    Sulphur                  250      0.01
                    Name                      V        W       EP                   Troll's Blood            135      0.0025
                                                                                    Wraith Essence           155      0.0025
                    Horse                   5000
                    Small Cart              3000
                                                                                JEWELRY:

                WEAPONS:                                                            Name                      V        W       EP

                    Name                      V        W       EP                   Bracelet                 900      0.2
                                                                                    Ring                     750      0.0025
                    Arrow                             0.05                          Torc                    1050      0.08
                    Battle Axe                               1250
                    Broadsword                                350
                    Dagger                                    175               MISCELLANEOUS:
                    Short Bow                                 750
                    Staff                                    4000                   Name                      V        W       EP

                                                                                    Painting               10000      2.0
            STURDY SHELVES:

                ARMOR:                                                          RELIGIOUS ITEMS:

                    Name                      V        W       EP                   Name                      V        W       EP

                    Greaves                                   250                   Holy Candle              300      0.05
                    Left Pauldron                             200                   Holy Water               200
                    Right Pauldron                            200                   Rare Symbol              140      0.05
                    Round Shield                              900                   Scarab                            0.05
                                                                                    Talisman                          0.25

                CLOTHING:
                                                                                WEAPONS:
                    Name                      V        W       EP
                                                                                    Name                      V        W       EP
                    Plain Robes              100      1.5
                    Strapless Dress           90      0.3                           Dai-Katana                                600
                                                                                    Katana                                    500
                                                                                    Long Bow                                 1500
                GEMS:                                                               Wakazashi                                 400
                                                                                    War Axe                                  2500
                    Name                      V        W       EP                   Warhammer                                 600

                    Turquoise                425      0.01
                                                                            INCENSE BURNING:

                INGREDIENTS:                                                    CLOTHING:

                    Name                      V        W       EP                   Name                      V        W       EP

                    Big Tooth                 15      0.05                          Formal Eodoric           300      0.2
                    Black Poppy               15      0.02                          Khajiit Suit             350      0.25
                    Clover                    15      0.0025                        Priest Robes             450      2.0
                    Ghoul's Tongue            20      0.1                           Priestess Robes          450      2.0
                    Golden Poppy              15      0.02
                    Gryphon's Feather         15      0.0025
                    Ichor                     12      0.1                       GEMS:
                    Ivory                    375      0.05 
                    Lead                      24      0.05                          Name                      V        W       EP
                    Nymph Hair                35      0.0025
                    Palm                      20                                    Diamond                 5000      0.0025
                    Red Poppy                 15      0.02                          Emerald                 4250      0.0025
                    Small Scorpion Stinger    30      0.0025
                    Tin                       18      0.01
                    White Poppy               15      0.02                      INGREDIENTS:

                                                                                    Name                      V        W       EP
                RELIGIOUS ITEMS:
                                                                                    Daedra's Heart           750      0.5
                    Name                      V        W       EP                   Dragon's Scales          550
                                                                                    Elixir Vitae            1000      0.1
                    Common Symbol                     0.07                          Holy Relic               500      0.05
                    Icon                                                            Lich Dust                400      0.0025
                                                                                    Platinum                 800      0.01
                                                                                    Saint's Hair             300      0.0025
                WEAPONS:                                                            Unicorn Horn             350

                    Name                      V        W       EP
                                                                                JEWELRY:
                    Mace                                      400
                    Saber                                     400                   Name                      V        W       EP
                    Tanto                                     250
                                                                                    Wand                    1800      0.02   2500

            AVERAGE:
                                                                                RELIGIOUS ITEMS:
                ARMOR:
                                                                                    Name                      V        W       EP
                    Name                      V        W       EP
                                                                                    Holy Dagger                       0.25
                    Cuirass                                  1250                   Holy Tome                         1.5
                    Kite Shield                              1500                   Small Statue             350     10.0


                CLOTHING:

                    Name                      V        W       EP

                    Anticlere Surcoat        115      0.75
                    Day Gown                 150      0.6
                    Dwynnen Surcoat          110      0.75
                    Formal Cloak             180      2.0
                    Open Tunic                65      0.12
                    Reversible Tunic          60      0.25
                    Tall Boots                70      0.75
                    Tights                    85      0.07

 
    WEAPONS & ARMOR:          

        DURABILITY:

            IRON, SILVER, ELVEN,                STEEL:                                  MITHRIL, ADAMANTIUM, EBONY,
            DWARVEN:                                                                    ORCISH, DAEDRIC:             

                ARMOR:                              ARMOR:                                  ARMOR:
                                                    
                    Cuirass:         82 Hits            Cuirass:       123 Hits                 Cuirass:        41 Hits
                    Gauntlets:       41 Hits            Gauntlets:      62 Hits                 Gauntlets:      21 Hits
                    Greaves:         62 Hits            Greaves:        93 Hits                 Greaves:        31 Hits
                    Left Pauldron:   41 Hits            Left Pauldron:  62 Hits                 Left Pauldron:  21 Hits
                    Right Pauldron:  41 Hits            Right Pauldron: 62 Hits                 Right Pauldron: 21 Hits
                    Helm:            56 Hits            Helm:           84 Hits                 Helm:           28 Hits
                    Boots:           62 Hits            Boots:          93 Hits                 Boots:          31 Hits


                SHIELDS:                            SHIELDS:                                SHIELDS:

                    Buckler:         80 Hits            Buckler:       120 Hits                 Buckler:        40 Hits
                    Round Shield:    94 Hits            Round Shield:  140 Hits                 Round Shield:   47 Hits
                    Kite Shield:    107 Hits            Kite Shield:   160 Hits                 Kite Shield:    54 Hits
                    Tower Shield:   120 Hits            Tower Shield:  180 Hits                 Tower Shield:   60 Hits

                                                    
                ARCHERY:                            ARCHERY:                                ARCHERY:

                    Short Bow:      140 Hits            Short Bow:     210 Hits                 Short Bow:      70 Hits
                    Long Bow:       200 Hits            Long Bow:      300 Hits                 Long Bow:      100 Hits


                AXES:                               AXES:                                   AXES:

                    Battle Axe:     140 Hits            Battle Axe:    210 Hits                 Battle Axe:     70 Hits
                    War Axe:        200 Hits            War Axe:       300 Hits                 War Axe:       100 Hits


                BLUNT WEAPONS:                      BLUNT WEAPONS:                          BLUNT WEAPONS:

                    Staff:          140 Hits            Staff:         210 Hits                 Staff:          70 Hits
                    Mace:           160 Hits            Mace:          240 Hits                 Mace:           80 Hits
                    Flail:          180 Hits            Flail:         270 Hits                 Flail:          90 Hits
                    Warhammer:      200 Hits            Warhammer:     300 Hits                 Warhammer:     100 Hits


                LONG BLADES:                        LONG BLADES:                            LONG BLADES:

                    Broadsword:     140 Hits            Broadsword:    210 Hits                 Broadsword:     70 Hits
                    Saber:          160 Hits            Saber:         240 Hits                 Saber:          80 Hits
                    Longsword:      180 Hits            Longsword:     270 Hits                 Longsword:      90 Hits
                    Katana:         200 Hits            Katana:        300 Hits                 Katana:        100 Hits
                    Claymore:       180 Hits            Claymore:      270 Hits                 Claymore:       90 Hits
                    Dai-Katana:     200 Hits            Dai-Katana:    300 Hits                 Dai-Katana:    100 Hits


                SHORT BLADES:                       SHORT BLADES:                           SHORT BLADES:

                    Dagger:         140 Hits            Dagger:        210 Hits                 Dagger:         70 Hits
                    Tanto:          160 Hits            Tanto:         240 Hits                 Tanto:          80 Hits
                    Shortsword:     180 Hits            Shortsword:    270 Hits                 Shortsword:     90 Hits
                    Wakazashi:      200 Hits            Wakazashi:     300 Hits                 Wakazashi:     100 Hits


        ENCHANTMENT POINTS, BASE:

                ARMOR:                              ARCHERY:                                LONG BLADES:

                    Cuirass:       1250                 Short Bow:     750                      Broadsword:    350
                    Gauntlets:      125                 Long Bow:     1500                      Saber:         400
                    Greaves:        250                                                         Longsword:     450
                    Left Pauldron:  200                                                         Katana:        500
                    Right Pauldron: 200             AXES:                                       Claymore:      550
                    Helm:           250                                                         Dai-Katana:    600
                    Boots:          200                 Battle Axe:   1250
                                                        War Axe:      2500
                                                                                            SHORT BLADES:
                SHIELDS:
                                                    BLUNT WEAPONS:                              Dagger:        175
                    Buckler:        400                                                         Tanto:         250
                    Round Shield:   900                 Staff:        4000                      Shortsword:    325
                    Kite Shield:   1500                 Mace:          400                      Wakazashi:     400
                    Tower Shield:  2500                 Flail:         500
                                                        Warhammer:     600


        SHIELD ARMOR:

                LEATHER:                            SILVER:                                 ADAMANTIUM:

                    Buckler:          1                 Buckler:         1                      Buckler:         4     
                    Round Shield:     2                 Round Shield:    2                      Round Shield:    5
                    Kite Shield:      3                 Kite Shield:     3                      Kite Shield:     6
                    Tower Shield:     4                 Tower Shield:    4                      Tower Shield:    7    


                CHAIN:                              ELVEN:                                  EBONY:

                    Buckler:          1                 Buckler:         2                      Buckler:         5  
                    Round Shield:     2                 Round Shield:    3                      Round Shield:    6
                    Kite Shield:      3                 Kite Shield:     4                      Kite Shield:     7
                    Tower Shield:     4                 Tower Shield:    5                      Tower Shield:    8


                IRON:                               DWARVEN:                                ORCISH:    

                    Buckler:          0                 Buckler:         3                      Buckler:         6     
                    Round Shield:     1                 Round Shield:    4                      Round Shield:    7
                    Kite Shield:      2                 Kite Shield:     5                      Kite Shield:     8 
                    Tower Shield:     3                 Tower Shield:    6                      Tower Shield:    9


                STEEL:                              MITHRIL:                                DAEDRIC:

                    Buckler:          1                 Buckler:         4                      Buckler:         7 
                    Round Shield:     2                 Round Shield:    5                      Round Shield:    8
                    Kite Shield:      3                 Kite Shield:     6                      Kite Shield:     9
                    Tower Shield:     4                 Tower Shield:    7                      Tower Shield:   10 


CREDITS & THANKS:

    CREDITS:

        Osorkon:

            Bossfall author
            Bossfall ideas
            Bossfall documentation


        Interkarma and the DFWorkshop team:

            I use some vanilla DFU code in Bossfall, see https://github.com/Osorkon21/Bossfall to read Bossfall's source code.
            Every use of vanilla code is clearly marked by comments stating original authorship, please verify authorship if you
            re-use any code from Bossfall.


    THANKS:

        Interkarma and the DFWorkshop team:

            Bossfall wouldn't exist without their years of hard work on DFU. Interkarma deserves a special thanks.


        Osorkon's Dad:

            For tech support, patience, explanations, and the books.


        Osorkon's Older Brother:

            Your initial balance ideas were good and they're still in the mod!


LEGAL INFO:

    - Bossfall - and all vanilla DFU code - is licensed under the MIT license (http://www.opensource.org/licenses/mit-license.php).

    - Under the MIT license, the Bossfall .dfmod is provided as-is without any warranty and I can't be held liable for Bossfall
      not working as intended.

    - You can use Bossfall's work and/or code however you like, but if you do you must include a copy of the MIT license in your
      work and credit the author who holds the code's copyright. In most cases, that is me (Osorkon).

    - Some of Bossfall's code was written by DFWorkshop. To determine who wrote the code, you can view Bossfall's source code at
      https://github.com/Osorkon21/Bossfall. I clearly mark all code I didn't write with comments.