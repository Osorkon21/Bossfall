// Project:         Bossfall
// Copyright:       Copyright (C) 2022 Osorkon, vanilla DFU code Copyright (C) 2009-2022 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Osorkon21/Bossfall, vanilla DFU code https://github.com/Interkarma/daggerfall-unity
// Original Author: Osorkon, vanilla DFU code Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    vanilla DFU code Allofich, Numidium, TheLacus
// 
// Notes: This script uses code from vanilla DFU's PlayerActivate script. Comments indicate authorship, please verify
//        authorship before crediting. When in doubt compare to vanilla DFU's source code.
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
    /// Custom player activations.
    /// </summary>
    public class BossfallPlayerActivate : MonoBehaviour
    {
        #region Fields

        Camera mainCamera;
        int playerLayerMask = 0;
        bool castPending = false;

        // I doubled vanilla's RayDistance.
        const float RayDistance = 6144 * MeshReader.GlobalScale;

        // I added these following 7 declarations.
        PlayerActivate activate;
        bool clearPopupText;
        bool clearMidScreenText;
        LinkedList<TextLabel> list;
        TextLabel text;
        DaggerfallEntityBehaviour storedEntity;
        DaggerfallEntityBehaviour storedEntityTwo;

        #endregion

        #region Unity

        /// <summary>
        /// I changed vanilla's method name from "Start" to "Awake".
        /// </summary>
        void Awake()
        {
            mainCamera = GameManager.Instance.MainCamera;
            playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));

            // I added the rest of this method.
            activate = GameManager.Instance.PlayerActivate;

            PopupText dfHUDText = DaggerfallUI.Instance.DaggerfallHUD.PopupText;
            DaggerfallHUD dfHUD = DaggerfallUI.Instance.DaggerfallHUD;

            Type type = dfHUDText.GetType();
            Type type1 = dfHUD.GetType();

            FieldInfo fieldInfo = type.GetField("textRows", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fieldInfo1 = type1.GetField("midScreenTextLabel", BindingFlags.NonPublic | BindingFlags.Instance);

            list = (LinkedList<TextLabel>)fieldInfo.GetValue(dfHUDText);
            text = (TextLabel)fieldInfo1.GetValue(dfHUD);
        }

        void Update()
        {
            if (mainCamera == null)
                return;

            // If clearMidScreenText is true I check for PlayerActivate midScreenTextLabel's presence every
            // frame. Once I find it, I delete it and add a custom HUD message.
            if (clearMidScreenText && !string.IsNullOrEmpty(text.Text))
            {
                text.Text = string.Empty;

                ActivateMobileEnemy(storedEntity);

                storedEntity = null;
                clearMidScreenText = false;
            }

            // If clearPopupText is true I check for popupText every frame. Once I find it I delete the most recent
            // message and add a custom HUD message.
            if (clearPopupText && list.Count > 0)
            {
                list.RemoveLast();

                ActivateMobileEnemy(storedEntityTwo);

                storedEntityTwo = null;
                clearPopupText = false;
            }

            if (GameManager.Instance.PlayerMouseLook.cursorActive &&
                DaggerfallUI.Instance.DaggerfallHUD != null &&
                DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ActiveMouseOverLargeHUD)
                return;

            bool touchCastPending = false;
            if (GameManager.Instance.PlayerEffectManager)
            {
                if (GameManager.Instance.PlayerEffectManager.HasReadySpell)
                {
                    // I removed "MagicAndEffects." from the two lines below.
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

                if (castPending)
                {
                    castPending = false;
                    return;
                }
            }

            if (InputManager.Instance.ActionComplete(InputManager.Actions.ActivateCenterObject))
            {
                Ray ray = new Ray();
                if (GameManager.Instance.PlayerMouseLook.cursorActive)
                {
                    if (DaggerfallUnity.Settings.RetroRenderingMode > 0)
                    {
                        float largeHUDHeight = 0;
                        if (DaggerfallUI.Instance.DaggerfallHUD != null && DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled && DaggerfallUnity.Settings.LargeHUDDocked)
                            largeHUDHeight = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ScreenHeight;
                        float xm = Input.mousePosition.x / Screen.width;
                        float ym = (Input.mousePosition.y - largeHUDHeight) / (Screen.height - largeHUDHeight);
                        Vector2 retroMousePos = new Vector2(mainCamera.targetTexture.width * xm, mainCamera.targetTexture.height * ym);
                        ray = mainCamera.ScreenPointToRay(retroMousePos);
                    }
                    else
                    {
                        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    }
                }
                else
                {
                    ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                }

                RaycastHit hit;
                bool hitSomething = Physics.Raycast(ray, out hit, RayDistance, playerLayerMask);
                if (hitSomething)
                {
                    bool buildingUnlocked = false;
                    DFLocation.BuildingTypes buildingType = DFLocation.BuildingTypes.AllValid;
                    StaticBuilding building = new StaticBuilding();
                    QuestResourceBehaviour questResourceBehaviour;
                    if (QuestResourceBehaviourCheck(hit, out questResourceBehaviour) && !(questResourceBehaviour.TargetResource is Person))
                    {
                        // This displays a custom HUD message if the Display Enemy Level setting is on.
                        if (activate.CurrentMode == PlayerActivateModes.Info && Bossfall.Instance.DisplayEnemyLevel)
                        {
                            if (!touchCastPending)
                            {
                                DaggerfallEntityBehaviour mobileEnemyBehaviour;
                                if (MobileEnemyCheck(hit, out mobileEnemyBehaviour))
                                {
                                    // If distance from player is greater than vanilla's RayDistance, activate
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

                    Transform buildingOwner;
                    DaggerfallStaticBuildings buildings = activate.GetBuildings(hit.transform, out buildingOwner);
                    if (buildings && buildings.HasHit(hit.point, out building))
                    {
                        BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
                        if (!buildingDirectory)
                            return;

                        BuildingSummary buildingSummary;
                        if (!buildingDirectory.GetBuildingSummary(building.buildingKey, out buildingSummary))
                            return;

                        buildingType = buildingSummary.BuildingType;

                        // I only want my added building identification messages to appear if distance from player
                        // is greater than vanilla's maximum ray distance.
                        if (hit.distance > 3072 * MeshReader.GlobalScale)
                        {
                            ActivateBuilding(building, buildingType, buildingUnlocked);
                        }
                    }

                    // I added the DisplayEnemyLevel condition.
                    if (!touchCastPending && Bossfall.Instance.DisplayEnemyLevel)
                    {
                        DaggerfallEntityBehaviour mobileEnemyBehaviour;
                        if (MobileEnemyCheck(hit, out mobileEnemyBehaviour))
                        {
                            // If distance from player is greater than vanilla's RayDistance, activate
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

        /// <summary>
        /// This method is entirely vanilla's, from PlayerActivate.
        /// </summary>
        void ActivateBuilding(
        StaticBuilding building,
        DFLocation.BuildingTypes buildingType,
        bool buildingUnlocked)
        {
            if (activate.CurrentMode == PlayerActivateModes.Info)
            {
                GameManager.Instance.PlayerGPS.DiscoverBuilding(building.buildingKey);
                PlayerGPS.DiscoveredBuilding db;
                if (GameManager.Instance.PlayerGPS.GetDiscoveredBuilding(building.buildingKey, out db))
                {
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
        /// This is vanilla's method from PlayerActivate, modified for Bossfall. It displays a custom HUD message if the
        /// Display Enemy Level setting is on. I removed vanilla's RaycastHit parameter as I didn't use it. Comments
        /// indicate changes or additions I made.
        /// </summary>
        /// <param name="mobileEnemyBehaviour">The mobile enemy hit by the ray.</param>
        void ActivateMobileEnemy(DaggerfallEntityBehaviour mobileEnemyBehaviour)
        {
            EnemyEntity enemyEntity = mobileEnemyBehaviour.Entity as EnemyEntity;

            // I changed "currentMode" to "activate.CurrentMode" in the below line.
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

                        // I added the following two lines.
                        int enemyLevel = enemyEntity.Level;
                        string enemyLevelAndName = string.Format("Level {0} {1}", enemyLevel.ToString(), enemyName);

                        message = TextManager.Instance.GetLocalizedText("youSeeA");

                        // Vanilla's line, but I changed the second parameter from "enemyName" to "enemyLevelAndName".
                        message = message.Replace("%s", enemyLevelAndName);

                        // I changed vanilla's PopupMessage call to an AddHUDText call.
                        DaggerfallUI.AddHUDText(message);
                    }
                    break;
            }
        }

        /// <summary>
        /// This method is entirely vanilla's, from PlayerActivate.
        /// </summary>
        bool QuestResourceBehaviourCheck(RaycastHit hitInfo, out QuestResourceBehaviour questResourceBehaviour)
        {
            questResourceBehaviour = hitInfo.transform.GetComponent<QuestResourceBehaviour>();

            return questResourceBehaviour != null;
        }

        /// <summary>
        /// This method is entirely vanilla's, from PlayerActivate.
        /// </summary>
        bool MobileEnemyCheck(RaycastHit hitInfo, out DaggerfallEntityBehaviour mobileEnemy)
        {
            mobileEnemy = hitInfo.transform.GetComponent<DaggerfallEntityBehaviour>();

            return mobileEnemy != null;
        }

        #endregion
    }
}
