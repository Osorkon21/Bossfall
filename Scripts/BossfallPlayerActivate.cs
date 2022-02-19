// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich, Numidium, TheLacus
// 
// Notes: This script uses code from vanilla DFU's PlayerActivate script. // [OSORKON] comments precede changes or
//        additions I made, please verify authorship before crediting. When in doubt compare to vanilla DFU's source code.
//

using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BossfallMod.Utility
{
    /// <summary>
    /// Counterpart to vanilla's PlayerActivate. Used to double vanilla activation distance and for custom HUD messages.
    /// </summary>
    public class BossfallPlayerActivate : MonoBehaviour
    {
        #region Fields

        Camera mainCamera;
        int playerLayerMask = 0;
        bool castPending = false;

        // [OSORKON] I doubled vanilla's RayDistance.
        const float RayDistance = 6144 * MeshReader.GlobalScale;

        // [OSORKON] I added these declarations.
        PlayerActivate activate;
        bool clearPopupText;
        bool clearMidScreenText;
        LinkedList<TextLabel> list;
        TextLabel text;
        DaggerfallEntityBehaviour storedEntity;
        DaggerfallEntityBehaviour storedEntityTwo;

        #endregion

        #region Unity

        void Start()
        {
            mainCamera = GameManager.Instance.MainCamera;
            playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));

            // [OSORKON] I added this line.
            activate = GameManager.Instance.PlayerActivate;

            // [OSORKON] I added this. Using Reflection I access the private "textRows" field in PopupText.
            list = new LinkedList<TextLabel>();
            PopupText dfHUDText = DaggerfallUI.Instance.DaggerfallHUD.PopupText;
            Type type = dfHUDText.GetType();
            FieldInfo fieldInfo = type.GetField("textRows", BindingFlags.NonPublic | BindingFlags.Instance);
            object fieldValue = fieldInfo.GetValue(dfHUDText);
            list = (LinkedList<TextLabel>)fieldValue;

            // [OSORKON] I added this. Using Reflection I access the private "midScreenTextLabel" field in DaggerfallHUD.
            text = new TextLabel();
            DaggerfallHUD dfHUD = DaggerfallUI.Instance.DaggerfallHUD;
            Type type1 = dfHUD.GetType();
            FieldInfo fieldInfo1 = type1.GetField("midScreenTextLabel", BindingFlags.NonPublic | BindingFlags.Instance);
            object fieldValue1 = fieldInfo1.GetValue(dfHUD);
            text = (TextLabel)fieldValue1;
        }

        void Update()
        {
            if (mainCamera == null)
                return;

            // [OSORKON] If clearMidScreenText is true I check for PlayerActivate midScreenTextLabel's presence every
            // frame. Once I find it, I delete it and add a custom HUD message.
            if (clearMidScreenText && !string.IsNullOrEmpty(text.Text))
            {
                text.Text = string.Empty;
                ActivateMobileEnemy(storedEntity);
                clearMidScreenText = false;
            }

            // [OSORKON] PlayerActivate's enemy activation HUD message is always added to the "textRows" field in
            // PopupText after BossfallPlayerActivate executes and I can't figure out why. The scripts have the same
            // execution order so that's not the source of the timing issue. Even if I change this method to LateUpdate
            // PlayerActivate's HUD message still is added to "textRows" after this script executes. I'm at a loss as
            // to why this is happening, so I'm resorting to brute force. If clearPopupText is true I check for
            // popupText every frame. Once I find it I delete the most recent message and add a custom HUD message.
            if (clearPopupText && list.Count > 0)
            {
                list.RemoveLast();
                ActivateMobileEnemy(storedEntityTwo);
                clearPopupText = false;
            }

            // Do not do scene activation if player has cursor active over large HUD
            if (GameManager.Instance.PlayerMouseLook.cursorActive &&
                DaggerfallUI.Instance.DaggerfallHUD != null &&
                DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ActiveMouseOverLargeHUD)
                return;

            // Do nothing further if player has spell ready to cast as activate button is now used to fire spell
            // The exception is a readied touch spell where player can activate doors, etc.
            // Touch spells only fire once a target entity is in range
            bool touchCastPending = false;
            if (GameManager.Instance.PlayerEffectManager)
            {
                // Handle pending spell cast
                if (GameManager.Instance.PlayerEffectManager.HasReadySpell)
                {
                    // Exclude touch spells from this check

                    // [OSORKON] I removed "MagicAndEffects." from the two lines below.
                    EntityEffectBundle spell = GameManager.Instance.PlayerEffectManager.ReadySpell;
                    if (spell.Settings.TargetType != TargetTypes.ByTouch)
                    {
                        castPending = true;
                        return;
                    }
                    else
                    {
                        touchCastPending = true;
                    }
                }

                // Prevents last spell cast click from falling through to normal click handling this frame
                if (castPending)
                {
                    castPending = false;
                    return;
                }
            }

            // Player activates object
            if (InputManager.Instance.ActionComplete(InputManager.Actions.ActivateCenterObject))
            {
                // Fire ray into scene from active mouse cursor or camera
                Ray ray = new Ray();
                if (GameManager.Instance.PlayerMouseLook.cursorActive)
                {
                    if (DaggerfallUnity.Settings.RetroRenderingMode > 0)
                    {
                        // Need to scale screen mouse position to match actual viewport area when retro rendering enabled
                        // Also need to account for when large HUD is enabled and docked as this changes the retro viewport area
                        // Undocked large HUD does not change retro viewport area
                        float largeHUDHeight = 0;
                        if (DaggerfallUI.Instance.DaggerfallHUD != null && DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled && DaggerfallUnity.Settings.LargeHUDDocked)
                            largeHUDHeight = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ScreenHeight;
                        float xm = Input.mousePosition.x / Screen.width;
                        float ym = (Input.mousePosition.y - largeHUDHeight) / (Screen.height - largeHUDHeight);
                        Vector2 retroMousePos = new Vector2(mainCamera.targetTexture.width * xm, mainCamera.targetTexture.height * ym);
                        ray = mainCamera.ScreenPointToRay(retroMousePos);
                        //Debug.Log(retroMousePos);
                    }
                    else
                    {
                        // Ray from mouse position into viewport
                        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    }
                }
                else
                {
                    // Ray from camera crosshair position
                    ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                }

                // Test ray against scene
                RaycastHit hit;
                bool hitSomething = Physics.Raycast(ray, out hit, RayDistance, playerLayerMask);
                if (hitSomething)
                {
                    bool buildingUnlocked = false;
                    DFLocation.BuildingTypes buildingType = DFLocation.BuildingTypes.AllValid;
                    StaticBuilding building = new StaticBuilding();

                    // Trigger quest resource behaviour click on anything but NPCs
                    QuestResourceBehaviour questResourceBehaviour;
                    if (QuestResourceBehaviourCheck(hit, out questResourceBehaviour) && !(questResourceBehaviour.TargetResource is Person))
                    {
                        // [OSORKON] This displays a custom HUD message if the Display Enemy Level setting is on.
                        if (activate.CurrentMode == PlayerActivateModes.Info && Bossfall.DisplayEnemyLevel)
                        {
                            if (!touchCastPending)
                            {
                                // Check for mobile enemy hit
                                DaggerfallEntityBehaviour mobileEnemyBehaviour;
                                if (MobileEnemyCheck(hit, out mobileEnemyBehaviour))
                                {
                                    // [OSORKON] If distance from player is greater than vanilla's RayDistance, activate
                                    // enemy immediately. If not, I add an extra step before messages are displayed.
                                    if (hit.distance > 3072 * MeshReader.GlobalScale)
                                    {
                                        ActivateMobileEnemy(mobileEnemyBehaviour);
                                    }
                                    else
                                    {
                                        storedEntity = mobileEnemyBehaviour;
                                        clearMidScreenText = true;
                                    }
                                }
                            }
                        }
                        return;
                    }

                    // Check for a static building hit
                    Transform buildingOwner;
                    DaggerfallStaticBuildings buildings = activate.GetBuildings(hit.transform, out buildingOwner);
                    if (buildings && buildings.HasHit(hit.point, out building))
                    {
                        // Get building directory for location
                        BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
                        if (!buildingDirectory)
                            return;

                        // Get detailed building data from directory
                        BuildingSummary buildingSummary;
                        if (!buildingDirectory.GetBuildingSummary(building.buildingKey, out buildingSummary))
                            return;

                        buildingType = buildingSummary.BuildingType;

                        // [OSORKON] I only want my added building identification messages to appear if distance from player
                        // is greater than vanilla's maximum ray distance.
                        if (hit.distance > 3072 * MeshReader.GlobalScale)
                        {
                            ActivateBuilding(building, buildingType, buildingUnlocked);
                        }
                    }

                    // Avoid non-action interactions while a Touch cast is readied

                    // [OSORKON] I added the DisplayEnemyLevel condition.
                    if (!touchCastPending && Bossfall.DisplayEnemyLevel)
                    {
                        // Check for mobile enemy hit
                        DaggerfallEntityBehaviour mobileEnemyBehaviour;
                        if (MobileEnemyCheck(hit, out mobileEnemyBehaviour))
                        {
                            // [OSORKON] If distance from player is greater than vanilla's RayDistance, activate
                            // enemy immediately. If not, I add an extra step before messages are displayed.
                            if (hit.distance > 3072 * MeshReader.GlobalScale)
                            {
                                ActivateMobileEnemy(mobileEnemyBehaviour);
                            }
                            else
                            {
                                storedEntityTwo = mobileEnemyBehaviour;
                                clearPopupText = true;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Activation Methods

        void ActivateBuilding(
        StaticBuilding building,
        DFLocation.BuildingTypes buildingType,
        bool buildingUnlocked)
        {
            if (activate.CurrentMode == PlayerActivateModes.Info)
            {
                // Discover building
                GameManager.Instance.PlayerGPS.DiscoverBuilding(building.buildingKey);

                // Get discovered building
                PlayerGPS.DiscoveredBuilding db;
                if (GameManager.Instance.PlayerGPS.GetDiscoveredBuilding(building.buildingKey, out db))
                {
                    // Check against quest system for an overriding quest-assigned display name for this building
                    DaggerfallUI.AddHUDText(db.displayName);

                    if (!buildingUnlocked && buildingType < DFLocation.BuildingTypes.Temple
                        && buildingType != DFLocation.BuildingTypes.HouseForSale)
                    {
                        string buildingClosedMessage = (buildingType == DFLocation.BuildingTypes.GuildHall) ? TextManager.Instance.GetLocalizedText("guildClosed") : TextManager.Instance.GetLocalizedText("storeClosed");
                        buildingClosedMessage = buildingClosedMessage.Replace("%d1", PlayerActivate.openHours[(int)buildingType].ToString());
                        buildingClosedMessage = buildingClosedMessage.Replace("%d2", PlayerActivate.closeHours[(int)buildingType].ToString());
                        DaggerfallUI.Instance.PopupMessage(buildingClosedMessage);
                    }
                }
            }
        }

        /// <summary>
        /// [OSORKON] This is vanilla's method, modified for Bossfall. It displays a custom HUD message if the
        /// Display Enemy Level setting is on.
        /// </summary>
        /// <param name="mobileEnemyBehaviour">The mobile enemy hit by the ray.</param>
        void ActivateMobileEnemy(DaggerfallEntityBehaviour mobileEnemyBehaviour)
        {
            EnemyEntity enemyEntity = mobileEnemyBehaviour.Entity as EnemyEntity;
            switch (activate.CurrentMode)
            {
                case PlayerActivateModes.Info:
                case PlayerActivateModes.Grab:
                case PlayerActivateModes.Talk:
                    if (enemyEntity != null)
                    {
                        MobileEnemy mobileEnemy = enemyEntity.MobileEnemy;
                        string enemyName = TextManager.Instance.GetLocalizedEnemyName(mobileEnemy.ID);
                        string message;
                        int enemyLevel = enemyEntity.Level;
                        string enemyLevelAndName = string.Format("Level {0} {1}", enemyLevel.ToString(), enemyName);
                        message = TextManager.Instance.GetLocalizedText("youSeeA");
                        message = message.Replace("%s", enemyLevelAndName);
                        DaggerfallUI.AddHUDText(message);
                    }
                    break;
            }
        }

        private bool QuestResourceBehaviourCheck(RaycastHit hitInfo, out QuestResourceBehaviour questResourceBehaviour)
        {
            questResourceBehaviour = hitInfo.transform.GetComponent<QuestResourceBehaviour>();

            return questResourceBehaviour != null;
        }

        // Check if raycast hit a mobile enemy
        bool MobileEnemyCheck(RaycastHit hitInfo, out DaggerfallEntityBehaviour mobileEnemy)
        {
            mobileEnemy = hitInfo.transform.GetComponent<DaggerfallEntityBehaviour>();

            return mobileEnemy != null;
        }

        #endregion
    }
}
