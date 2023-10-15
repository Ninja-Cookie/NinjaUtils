﻿using Reptile;
using System.Reflection;
using UnityEngine;

namespace NinjaUtils
{
    internal class NinjaUpdater : MonoBehaviour
    {
        public static NinjaUpdater Instance;

        private NinjaFunction ninjaFunction;
        private NinjaCalls ninjaCalls;
        private NinjaGUI ninjaGUI;

        public NinjaUpdater()
        {
            Instance = this;
            ninjaFunction = NinjaFunction.Instance;
            ninjaCalls = NinjaCalls.Instance;
            ninjaGUI = NinjaGUI.Instance;
        }
        public void Update()
        {
            if (ninjaCalls.timescaleEnabled && Time.timeScale != ninjaCalls.timescale) { Time.timeScale = ninjaCalls.timescale; }
            else if (Time.timeScale != 1f) { Time.timeScale = 1f; }

            if (Core.Instance != null)
            {
                ninjaCalls.corePuased = Core.Instance.IsCorePaused;

                if (ninjaCalls.loadedBaseModule == null) { ninjaCalls.loadedBaseModule = FindObjectOfType<BaseModule>(); }

                if (ninjaCalls.limitFPS)
                {
                    if (UnityEngine.Application.targetFrameRate != ninjaCalls.fpsLimit)
                    {
                        UnityEngine.Application.targetFrameRate = ninjaCalls.fpsLimit;
                    }
                }
            }
            else
            {
                ninjaCalls.corePuased = true;
            }

            if (WorldHandler.instance != null)
            {
                if (ninjaCalls.player != WorldHandler.instance.GetCurrentPlayer()) { ninjaCalls.player = WorldHandler.instance.GetCurrentPlayer(); }
            }

            if (ninjaCalls.player != null)
            {
                if ((Characters)typeof(Player).GetField("character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ninjaCalls.player) != ninjaCalls.currentCharIndex)
                {
                    ninjaCalls.currentCharIndex = (Characters)typeof(Player).GetField("character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ninjaCalls.player);
                    ninjaCalls.currentChar = (int)ninjaCalls.currentCharIndex;
                }
                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ninjaCalls.player) != ninjaCalls.currentStyleIndex && (MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ninjaCalls.player) != MoveStyle.ON_FOOT)
                {
                    ninjaCalls.player.SetCurrentMoveStyleEquipped(ninjaCalls.currentStyleIndex, true, true);
                    ninjaCalls.player.SwitchToEquippedMovestyle(true);
                }
                ninjaCalls.wallrunLineAbility = (WallrunLineAbility)typeof(Player).GetField("wallrunAbility", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ninjaCalls.player);

                if (ninjaCalls.wallrunLineAbility != null)
                {
                    ninjaCalls.storageSpeed = (float)typeof(WallrunLineAbility).GetField("lastSpeed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ninjaCalls.wallrunLineAbility);
                }
                else
                {
                    ninjaCalls.storageSpeed = 0;
                }

                ninjaCalls.playerSpeed = ninjaCalls.player.GetTotalSpeed();
                if (ninjaCalls.playerSpeedMax < ninjaCalls.playerSpeed) { ninjaCalls.playerSpeedMax = ninjaCalls.playerSpeed; }
            }
            else
            {
                if (ninjaCalls.playerSpeed != 0) { ninjaCalls.playerSpeed = 0; }
                if (ninjaCalls.storageSpeed != 0) { ninjaCalls.storageSpeed = 0f; }
            }

            if (ninjaCalls.invul)
            {
                if (ninjaCalls.player != null)
                {
                    ninjaCalls.player.ResetHP();
                    if (ninjaCalls.player.AmountOfCuffs() > 0)
                    {
                        ninjaCalls.player.RemoveAllCuffs();
                    }
                }
            }
            ninjaCalls.wantedManager = WantedManager.instance;
            if (ninjaCalls.wantedManager != null) { ninjaCalls.isWanted = ninjaCalls.wantedManager.Wanted; } else { ninjaCalls.isWanted = false; }

            if (ninjaCalls.currentStage != Reptile.Utility.GetCurrentStage() && (Reptile.Utility.GetIsCurrentSceneStage() && WorldHandler.instance.GetCurrentPlayer() != null && ninjaCalls.loadedBaseModule != null))
            {
                if (!ninjaCalls.loadedBaseModule.IsLoading)
                {
                    ninjaCalls.currentStage = Reptile.Utility.GetCurrentStage();

                    ninjaCalls.currentRespawner = 0;
                    ninjaCalls.respawners.Clear();

                    ninjaCalls.currentDreamRespawner = 0;
                    ninjaCalls.dreamRespawners.Clear();

                    foreach (var item in WorldHandler.instance.SceneObjectsRegister.playerSpawners.FindAll((PlayerSpawner candidate) => candidate.isRespawner))
                    {
                        ninjaCalls.respawners.Add(item.transform.position);
                    };

                    if (WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter() != null)
                    {
                        foreach (var item in WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter().checkpoints)
                        {
                            ninjaCalls.dreamRespawners.Add(item.spawnLocation.position);
                        }
                    }
                }
            }

            if (ninjaCalls.isMenuing && Core.Instance != null)
            {
                if (ninjaCalls.corePuased)
                {
                    if (ninjaCalls.GetGameplayCamera(ninjaCalls.player) != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ninjaCalls.GetGameplayCamera(ninjaCalls.player));
                        cameraMode.inputEnabled = true;
                        ninjaCalls.isMenuing = false;
                    }
                }
                else if (!Cursor.visible && ninjaCalls.GetGameInput() != null && Reptile.Utility.GetIsCurrentSceneStage())
                {
                    ninjaCalls.GetGameInput().SetUICursorMode();
                }
            }

            if (Core.Instance != null)
            {
                if (ninjaCalls.loadedBaseModule != null && ninjaCalls.corePuased)
                {
                    if (ninjaCalls.loadedBaseModule.IsInGamePaused)
                    { ninjaCalls.isPaused = true; }
                    else { ninjaCalls.isPaused = false; }
                }
                else
                {
                    ninjaCalls.isPaused = false;
                }
            }

            if (ninjaCalls.noclip)
            {
                if (ninjaCalls.player != null)
                {
                    ninjaCalls.fly = false;
                    ninjaCalls.flyOff = true;
                    ninjaCalls.noclipOff = false;

                    Camera.main.farClipPlane = 20000f;

                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(ninjaCalls.player, false);

                    Transform cameraMode = (Transform)typeof(WorldHandler).GetField("currentCameraTransform", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(WorldHandler.instance);

                    Vector3 velocity = Vector3.zero;

                    float deadzone = 0.01f;

                    float finalNoclipSpeedForward = ninjaCalls.noclipSpeed;
                    float finalNoclipSpeedRight = ninjaCalls.noclipSpeed;

                    if (UnityEngine.Input.GetAxis("Vertical") > deadzone) { finalNoclipSpeedForward = finalNoclipSpeedForward * UnityEngine.Input.GetAxis("Vertical"); }
                    else if (UnityEngine.Input.GetAxis("Vertical") < -deadzone) { finalNoclipSpeedForward = finalNoclipSpeedForward * (UnityEngine.Input.GetAxis("Vertical") * -1); }

                    if (UnityEngine.Input.GetAxis("Horizontal") > deadzone) { finalNoclipSpeedRight = finalNoclipSpeedRight * UnityEngine.Input.GetAxis("Horizontal"); }
                    else if (UnityEngine.Input.GetAxis("Horizontal") < -deadzone) { finalNoclipSpeedRight = finalNoclipSpeedRight * (UnityEngine.Input.GetAxis("Horizontal") * -1); }

                    Vector3 forward = (finalNoclipSpeedForward * Time.deltaTime) * cameraMode.forward;
                    Vector3 right = (finalNoclipSpeedRight * Time.deltaTime) * cameraMode.right;
                    forward.y = 0f;
                    right.y = 0f;

                    ninjaCalls.player.CompletelyStop();
                    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetAxis("Vertical") > deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += forward;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetAxis("Vertical") < -deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += forward * -1;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetAxis("Horizontal") < -deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += right * -1;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetAxis("Horizontal") > deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += right;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetKey(KeyCode.JoystickButton0))
                    {
                        velocity.y = (25f * Time.deltaTime);
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.JoystickButton1))
                    {
                        velocity.y = (25f * Time.deltaTime) * -1;
                    }
                    ninjaCalls.player.transform.Translate(velocity, Space.World);
                    ninjaCalls.player.SetVelocity(new Vector3(ninjaCalls.player.GetPracticalWorldVelocity().x, 0.24f, ninjaCalls.player.GetPracticalWorldVelocity().z));
                    ninjaCalls.noclipPos = ninjaCalls.player.transform.position;
                }
                else
                {
                    ninjaCalls.noclip = false;
                }
            }
            else
            {
                if (ninjaCalls.player != null)
                {
                    ninjaCalls.noclipPos = ninjaCalls.player.transform.position;
                }

                if (!ninjaCalls.noclipOff && ninjaCalls.player != null)
                {
                    if (!ninjaCalls.fly)
                    {
                        Camera.main.farClipPlane = 1000f;
                        FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                        userInputEnabled.SetValue(ninjaCalls.player, true);
                        ninjaCalls.player.CompletelyStop();
                    }
                    ninjaCalls.noclipOff = true;
                }
            }

            if (ninjaCalls.fly)
            {
                if (ninjaCalls.player != null)
                {
                    ninjaCalls.noclip = false;
                    ninjaCalls.noclipOff = true;
                    ninjaCalls.flyOff = false;

                    Camera.main.farClipPlane = 20000f;

                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(ninjaCalls.player, false);

                    Transform cameraMode = (Transform)typeof(WorldHandler).GetField("currentCameraTransform", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(WorldHandler.instance);

                    Vector3 velocity = Vector3.zero;

                    float deadzone = 0.01f;

                    float finalFlySpeedForward = ninjaCalls.flySpeed;
                    float finalFlySpeedRight = ninjaCalls.flySpeed;

                    if (UnityEngine.Input.GetAxis("Vertical") > deadzone) { finalFlySpeedForward = finalFlySpeedForward * UnityEngine.Input.GetAxis("Vertical"); }
                    else if (UnityEngine.Input.GetAxis("Vertical") < -deadzone) { finalFlySpeedForward = finalFlySpeedForward * (UnityEngine.Input.GetAxis("Vertical") * -1); }

                    if (UnityEngine.Input.GetAxis("Horizontal") > deadzone) { finalFlySpeedRight = finalFlySpeedRight * UnityEngine.Input.GetAxis("Horizontal"); }
                    else if (UnityEngine.Input.GetAxis("Horizontal") < -deadzone) { finalFlySpeedRight = finalFlySpeedRight * (UnityEngine.Input.GetAxis("Horizontal") * -1); }

                    Vector3 forward = finalFlySpeedForward * cameraMode.forward;
                    Vector3 right = finalFlySpeedRight * cameraMode.right;
                    forward.y = 0f;
                    right.y = 0f;

                    ninjaCalls.player.CompletelyStop();
                    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetAxis("Vertical") > deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += forward;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetAxis("Vertical") < -deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += forward * -1;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetAxis("Horizontal") < -deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += right * -1;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetAxis("Horizontal") > deadzone)
                    {
                        ninjaCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += right;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetKey(KeyCode.JoystickButton0))
                    {
                        if (ninjaCalls.player.IsGrounded())
                        {
                            ninjaCalls.player.motor.ForcedUnground();
                            ninjaCalls.player.Jump();
                        }
                        velocity.y = 20;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.JoystickButton1))
                    {
                        velocity.y = -20;
                    }
                    else
                    {
                        velocity.y = 0.24f;
                    }

                    ninjaCalls.player.SetVelocity(velocity);
                }
                else
                {
                    ninjaCalls.fly = false;
                }
            }
            else
            {
                if (!ninjaCalls.flyOff && ninjaCalls.player != null)
                {
                    if (!ninjaCalls.noclip)
                    {
                        Camera.main.farClipPlane = 1000f;
                        FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                        userInputEnabled.SetValue(ninjaCalls.player, true);
                        ninjaCalls.player.CompletelyStop();
                    }
                    ninjaCalls.flyOff = true;
                }
            }
        }

    }
}