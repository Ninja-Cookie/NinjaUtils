using BepInEx;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NinjaUtils
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Utils : BaseUnityPlugin
    {
        public const string pluginGuid = "ninjacookie.brc.ninjautils";
        public const string pluginName = "NinjaUtils";
        public const string pluginVersion = "0.0.1";

        public void Awake()
        {

        }

        public bool open = true;

        private Rect winRect = new Rect(20, 20, 275, 725);

        public void OnGUI()
        {
            if (open)
            {
                winRect = GUI.Window(0, winRect, NinjaUtilsGUI, pluginName + " (" + pluginVersion + ")");
            }
        }

        public int sidePadding = 10;
        public int elementSizeW = 100;
        public int elementSizeH = 20;
        public int lineSpacing = 2;
        public float buttonSpacing = 1.5f;
        public int linePos;

        public Vector3 savedPos = Vector3.zero;
        public Quaternion savedAng = new Quaternion(0.00000f, 0.70926f, 0.00000f, -0.70495f);
        public Vector3 savedVel = Vector3.zero;

        String savePosX = "";
        String savePosY = "";
        String savePosZ = "";

        bool shouldSaveVel = true;

        String saveVelX = "";
        String saveVelY = "";
        String saveVelZ = "";

        Stage selectedStage = Stage.hideout;
        int selectedStageV = 0;

        bool corePuased = true;

        public float noclipSpeed = 50f;
        public float flySpeed = 20f;

        String noclipSpeedS = "";
        String flySpeedS = "";

        bool invul = false;

        public int currentChar = 0;
        public Characters currentCharIndex = Characters.NONE;

        public int currentStyle = 2;
        public MoveStyle currentStyleIndex = MoveStyle.SKATEBOARD;

        public int currentOutfit = 0;

        public bool limitFPS = false;
        public int fpsLimit = 30;
        String fpsLimitS = "";

        public float playerSpeed = 0;
        public float playerSpeedMax = 0;

        WantedManager wantedManager;
        bool isWanted = false;

        public WallrunLineAbility wallrunLineAbility;
        public float storageSpeed = 0f;
        public float savedStorage = 0f;
        public String savedStorageS = "";

        public bool isMenuing = false;
        public bool isPaused = false;

        public bool timescaleEnabled = false;
        public float timescale = 0.1f;
        public String timescaleS = "";

        void NinjaUtilsGUI(int windowID)
        {
            GUIStyle colorWhite = new GUIStyle();
            colorWhite.normal.textColor = Color.white;
            colorWhite.alignment = TextAnchor.MiddleCenter;

            GUIStyle colorBlack = new GUIStyle();
            colorBlack.normal.textColor = Color.black;
            colorBlack.alignment = TextAnchor.MiddleCenter;

            GUIStyle colorRed = new GUIStyle();
            colorRed.normal.textColor = Color.red;
            colorRed.alignment = TextAnchor.MiddleCenter;

            linePos = 20;

            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Toggle Mouse Input (P)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, $"Core ({(corePuased ? "<color=red>Paused</color>" : "<color=green>Running</color>")})", colorWhite);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Speed: " + (int)playerSpeed, colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Highest Speed: " + (int)playerSpeedMax, colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Storage Speed: " + (int)storageSpeed, colorWhite, colorBlack);

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), "Fill Boost (R)") && (isMenuing || isPaused))
            {
                FillBoostMax(GetPlayer());
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Invulnerable ({(invul ? "<color=green>On</color>" : "<color=red>Off</color>")}) (i)") && (isMenuing || isPaused))
            {
                invul = !invul;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"End Wanted ({(isWanted ? "<color=red>Wanted</color>" : "<color=green>Safe</color>")}) (K)") && (isMenuing || isPaused))
            {
                EndWanted();
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Position (X, Y, Z)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            savePosX = GUI.TextArea(new Rect(sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), savePosX);
            savePosY = GUI.TextArea(new Rect((winRect.width / 2) - (((winRect.width / 3) - (sidePadding * buttonSpacing)) / 2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), savePosY);
            savePosZ = GUI.TextArea(new Rect(winRect.width - ((winRect.width / 3) - (sidePadding * buttonSpacing)) - sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), savePosZ);

            if (!float.TryParse(savePosX, out _)) { savePosX = savedPos.x.ToString(); }
            if (!float.TryParse(savePosY, out _)) { savePosY = savedPos.y.ToString(); }
            if (!float.TryParse(savePosZ, out _)) { savePosZ = savedPos.z.ToString(); }

            savedPos.x = float.Parse(savePosX);
            savedPos.y = float.Parse(savePosY);
            savedPos.z = float.Parse(savePosZ);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Velocity (X, Y, Z)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            saveVelX = GUI.TextArea(new Rect(sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), saveVelX);
            saveVelY = GUI.TextArea(new Rect((winRect.width / 2) - (((winRect.width / 3) - (sidePadding * buttonSpacing)) / 2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), saveVelY);
            saveVelZ = GUI.TextArea(new Rect(winRect.width - ((winRect.width / 3) - (sidePadding * buttonSpacing)) - sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), saveVelZ);

            if (!float.TryParse(saveVelX, out _)) { saveVelX = savedVel.x.ToString(); }
            if (!float.TryParse(saveVelY, out _)) { saveVelY = savedVel.y.ToString(); }
            if (!float.TryParse(saveVelZ, out _)) { saveVelZ = savedVel.z.ToString(); }

            savedVel.x = float.Parse(saveVelX);
            savedVel.y = float.Parse(saveVelY);
            savedVel.z = float.Parse(saveVelZ);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Saved Storage", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            savedStorageS = GUI.TextArea(new Rect((winRect.width/2) - (((winRect.width / 3) - (sidePadding * buttonSpacing))/2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), savedStorageS);

            if (!float.TryParse(savedStorageS, out _)) { savedStorageS = savedStorage.ToString(); }

            savedStorage = float.Parse(savedStorageS);

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), "Set Storage Speed (O)") && isMenuing || isPaused)
            {
                SetStorage(GetPlayer(), savedStorage);
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Saving Velocity ({(shouldSaveVel ? "<color=green>On</color>" : "<color=red>Off</color>")})") && (isMenuing || isPaused))
            {
                shouldSaveVel = !shouldSaveVel;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Save Position (H)") && (isMenuing || isPaused))
            {
                SaveLoad(GetPlayer(), true);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Load Position (J)") && (isMenuing || isPaused))
            {
                SaveLoad(GetPlayer(), false);
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Spawns (N)") && (isMenuing || isPaused))
            {
                GoToNextSpawn(GetPlayer());
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Dream Spawns (M)") && (isMenuing || isPaused))
            {
                GoToNextDreamSpawn(GetPlayer());
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Current Stage: " + currentStage.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Selected Stage: " + selectedStage.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), $"Go To Stage {(isPaused ? "<color=red>(Off)</color>" : "")} (1)") && isMenuing)
            {
                GoToStage(loadedBaseModule);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Select Stage (2)") && (isMenuing || isPaused))
            {
                SelectNextStage();
            }

            linePos = linePos + ((elementSizeH) + lineSpacing);
            if (fly) { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: Flying", colorWhite, colorBlack); }
            else if (noclip) { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: Noclip", colorWhite, colorBlack); }
            else { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: Normal", colorWhite, colorBlack); }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Toggle Noclip (\\)") && (isMenuing || isPaused))
            {
                fly = false;
                noclip = !noclip;
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Enable Fly (/)") && (isMenuing || isPaused))
            {
                noclip = false;
                fly = !fly;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            noclipSpeedS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), noclipSpeedS);
            flySpeedS = GUI.TextArea(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), flySpeedS);

            if (!float.TryParse(noclipSpeedS, out _)) { noclipSpeedS = noclipSpeed.ToString(); }
            if (!float.TryParse(flySpeedS, out _)) { flySpeedS = flySpeed.ToString(); }

            noclipSpeed = float.Parse(noclipSpeedS);
            flySpeed = float.Parse(flySpeedS);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Character: " + currentCharIndex.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Style: " + currentStyleIndex.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Char ([)") && (isMenuing || isPaused))
            {
                NextChar(GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Char (])") && (isMenuing || isPaused))
            {
                NextChar(GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Style (-)") && (isMenuing || isPaused))
            {
                NextStyle(GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Style (+)") && (isMenuing || isPaused))
            {
                NextStyle(GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Outfit (,)") && (isMenuing || isPaused))
            {
                NextStyle(GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Outfit (.)") && (isMenuing || isPaused))
            {
                NextStyle(GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH * 2);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Limit FPS ({(limitFPS ? "<color=green>On</color>" : "<color=red>Off</color>")}) (L)") && (isMenuing || isPaused))
            {
                LimitFPS();
            }

            linePos = linePos + (elementSizeH);
            fpsLimitS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), fpsLimitS);

            if (!int.TryParse(fpsLimitS, out _)) { fpsLimitS = fpsLimit.ToString(); }
            fpsLimit = int.Parse(fpsLimitS);



            linePos = linePos + (elementSizeH * 2);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Timescale ({(timescaleEnabled ? "<color=green>On</color>" : "<color=red>Off</color>")}) (T)") && (isMenuing || isPaused))
            {
                timescaleEnabled = !timescaleEnabled;
            }

            linePos = linePos + (elementSizeH);
            timescaleS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), timescaleS);

            if (!float.TryParse(timescaleS, out _)) { timescaleS = timescale.ToString(); }
            timescale = float.Parse(timescaleS);
        }

        void DrawText(float x, float y, float w, float h, string text, GUIStyle textColor, GUIStyle shadowColor = null)
        {
            if (shadowColor != null)
            {
                GUI.Label(new Rect(x + 1, y + 1, w, h), text, shadowColor);
                GUI.Label(new Rect(x + 2, y + 2, w, h), text, shadowColor);
            }

            GUI.Label(new Rect(x, y, w, h), text, textColor);
        }

        // ---------- GET ----------
        
        //public BaseModule loadedBaseModule
        //{
            //if (FindObjectOfType<BaseModule>() != null)
            //{
            //    return FindObjectOfType<BaseModule>();
            //}
            //return null;
        //}

        public GameplayCamera GetGameplayCamera(Player player, BaseModule baseModule)
        {
            if (player != null && baseModule != null)
            {
                if (!baseModule.IsLoading)
                {
                    GameplayCamera gameplayCamera = (GameplayCamera)typeof(Player).GetField("cam", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);
                    if (gameplayCamera != null) { return gameplayCamera; }
                }
            }
            return null;
        }

        public GameInput GetGameInput()
        {
            if (loadedBaseModule != null)
            {
                GameInput gameInput = (GameInput)typeof(BaseModule).GetField("gameInput", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(loadedBaseModule);
                if (gameInput != null) { return gameInput; }
            }
            return null;
        }

        public Player GetPlayer()
        {
            if (WorldHandler.instance != null)
            {
                if (WorldHandler.instance.GetCurrentPlayer() != null)
                {
                    return WorldHandler.instance.GetCurrentPlayer();
                }
            }
            return null;
        }

        // ---------- UPDATE ----------
        public Stage currentStage = Stage.NONE;
        public List<Vector3> respawners = new List<Vector3>();
        public List<Vector3> dreamRespawners = new List<Vector3>();
        public int currentRespawner = 0;
        public int currentDreamRespawner = 0;
        bool noclip = false;
        bool noclipOff = true;
        public Vector3 noclipPos = Vector3.zero;

        bool fly = false;
        bool flyOff = true;
        Player player;
        BaseModule loadedBaseModule;

        public void Update()
        {
            if (timescaleEnabled && Time.timeScale != timescale) { Time.timeScale = timescale; }
            else if (Time.timeScale != 1f) { Time.timeScale = 1f; }

            if (Core.Instance != null)
            {
                corePuased = Core.Instance.IsCorePaused;

                if (loadedBaseModule == null) { loadedBaseModule = FindObjectOfType<BaseModule>(); }

                if (limitFPS)
                {
                    if (UnityEngine.Application.targetFrameRate != fpsLimit)
                    {
                        UnityEngine.Application.targetFrameRate = fpsLimit;
                    }
                }
            }
            else
            {
                corePuased = true;
            }

            if (WorldHandler.instance != null)
            {
                if (player != WorldHandler.instance.GetCurrentPlayer()) { player = WorldHandler.instance.GetCurrentPlayer(); }
            }
            
            if (player != null)
            {
                if ((Characters)typeof(Player).GetField("character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != currentCharIndex)
                {
                    currentCharIndex = (Characters)typeof(Player).GetField("character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);
                    currentChar = (int)currentCharIndex;
                }

                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != currentStyleIndex && (MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != MoveStyle.ON_FOOT)
                {
                    player.SetCurrentMoveStyleEquipped(currentStyleIndex, true, true);
                    player.SwitchToEquippedMovestyle(true);
                }

                wallrunLineAbility = (WallrunLineAbility)typeof(Player).GetField("wallrunAbility", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);

                if (wallrunLineAbility != null)
                {
                    storageSpeed = (float)typeof(WallrunLineAbility).GetField("lastSpeed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(wallrunLineAbility);
                }
                else 
                {
                    storageSpeed = 0;
                }

                playerSpeed = player.GetTotalSpeed();
                if (playerSpeedMax < playerSpeed) { playerSpeedMax = playerSpeed; }
            }
            else
            {
                if (playerSpeed != 0) { playerSpeed = 0; }
                if (storageSpeed != 0) { storageSpeed = 0f; }
            }

            if (invul)
            {
                if (player != null)
                {
                    player.ResetHP();
                    if (player.AmountOfCuffs() > 0)
                    {
                        player.RemoveAllCuffs();
                    }
                }
            }

            wantedManager = WantedManager.instance;
            if (wantedManager != null) { isWanted = wantedManager.Wanted; } else { isWanted = false; }

            if (currentStage != Reptile.Utility.GetCurrentStage() && (Reptile.Utility.GetIsCurrentSceneStage() && WorldHandler.instance.GetCurrentPlayer() != null && loadedBaseModule != null))
            {
                if (!loadedBaseModule.IsLoading)
                {
                    currentStage = Reptile.Utility.GetCurrentStage();

                    currentRespawner = 0;
                    respawners.Clear();

                    currentDreamRespawner = 0;
                    dreamRespawners.Clear();

                    foreach (var item in WorldHandler.instance.SceneObjectsRegister.playerSpawners.FindAll((PlayerSpawner candidate) => candidate.isRespawner))
                    {
                        respawners.Add(item.transform.position);
                    };

                    if (WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter() != null)
                    {
                        foreach (var item in WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter().checkpoints)
                        {
                            dreamRespawners.Add(item.spawnLocation.position);
                        }
                    }
                }
            }

            if (isMenuing && Core.Instance != null)
            {
                if (corePuased)
                {
                    if (GetGameplayCamera(player, loadedBaseModule) != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GetGameplayCamera(player, loadedBaseModule));
                        cameraMode.inputEnabled = true;
                        isMenuing = false;
                    }
                }
                else if (!Cursor.visible && GetGameInput() != null && Reptile.Utility.GetIsCurrentSceneStage())
                {
                    GetGameInput().SetUICursorMode();
                }
            }

            if (Core.Instance != null)
            {
                if (loadedBaseModule != null && corePuased)
                {
                    if (loadedBaseModule.IsInGamePaused) 
                    { isPaused = true; } else { isPaused = false; }
                }
                else
                {
                    isPaused = false;
                }
            }

            if (noclip)
            {
                if (player != null)
                {
                    fly = false;
                    flyOff = true;
                    noclipOff = false;
                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(player, false);

                    Transform cameraMode = (Transform)typeof(WorldHandler).GetField("currentCameraTransform", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(WorldHandler.instance);

                    Vector3 velocity = Vector3.zero;

                    float deadzone = 0.01f;

                    float finalNoclipSpeedForward = noclipSpeed;
                    float finalNoclipSpeedRight = noclipSpeed;

                    if (UnityEngine.Input.GetAxis("Vertical") > deadzone) { finalNoclipSpeedForward = finalNoclipSpeedForward * UnityEngine.Input.GetAxis("Vertical"); }
                    else if (UnityEngine.Input.GetAxis("Vertical") < -deadzone) { finalNoclipSpeedForward = finalNoclipSpeedForward * (UnityEngine.Input.GetAxis("Vertical") * -1); }

                    if (UnityEngine.Input.GetAxis("Horizontal") > deadzone) { finalNoclipSpeedRight = finalNoclipSpeedRight * UnityEngine.Input.GetAxis("Horizontal"); }
                    else if (UnityEngine.Input.GetAxis("Horizontal") < -deadzone) { finalNoclipSpeedRight = finalNoclipSpeedRight * (UnityEngine.Input.GetAxis("Horizontal") * -1); }

                    Vector3 forward = (finalNoclipSpeedForward * Time.deltaTime) * cameraMode.forward;
                    Vector3 right = (finalNoclipSpeedRight * Time.deltaTime) * cameraMode.right;
                    forward.y = 0f;
                    right.y = 0f;

                    player.CompletelyStop();
                    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetAxis("Vertical") > deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        velocity += forward;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetAxis("Vertical") < -deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        velocity += forward * -1;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetAxis("Horizontal") < -deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        velocity += right * -1;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetAxis("Horizontal") > deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        //player.transform.Translate(right, Space.World);
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
                    player.transform.Translate(velocity, Space.World);
                    player.SetVelocity(new Vector3(player.GetPracticalWorldVelocity().x, 0.24f, player.GetPracticalWorldVelocity().z));
                    noclipPos = player.transform.position;
                }
                else
                {
                    noclip = false;
                    noclipOff = true;
                }
            }
            else
            {
                if (player != null)
                {
                    noclipPos = player.transform.position;
                }

                if (!noclipOff && player != null)
                {
                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(player, true);
                    player.CompletelyStop();
                    noclipOff = true;
                }
            }

            if (fly)
            {
                if (player != null)
                {
                    noclip = false;
                    noclipOff = true;
                    flyOff = false;

                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(player, false);

                    Transform cameraMode = (Transform)typeof(WorldHandler).GetField("currentCameraTransform", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(WorldHandler.instance);

                    Vector3 velocity = Vector3.zero;

                    float deadzone = 0.01f;

                    float finalFlySpeedForward = flySpeed;
                    float finalFlySpeedRight = flySpeed;

                    if (UnityEngine.Input.GetAxis("Vertical") > deadzone) { finalFlySpeedForward = finalFlySpeedForward * UnityEngine.Input.GetAxis("Vertical"); }
                    else if (UnityEngine.Input.GetAxis("Vertical") < -deadzone) { finalFlySpeedForward = finalFlySpeedForward * (UnityEngine.Input.GetAxis("Vertical") * -1); }

                    if (UnityEngine.Input.GetAxis("Horizontal") > deadzone) { finalFlySpeedRight = finalFlySpeedRight * UnityEngine.Input.GetAxis("Horizontal"); }
                    else if (UnityEngine.Input.GetAxis("Horizontal") < -deadzone) { finalFlySpeedRight = finalFlySpeedRight * (UnityEngine.Input.GetAxis("Horizontal") * -1); }

                    Vector3 forward = finalFlySpeedForward * cameraMode.forward;
                    Vector3 right = finalFlySpeedRight * cameraMode.right;
                    forward.y = 0f;
                    right.y = 0f;

                    player.CompletelyStop();
                    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetAxis("Vertical") > deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        velocity += forward;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetAxis("Vertical") < -deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        velocity += forward * -1;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetAxis("Horizontal") < -deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        velocity += right * -1;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetAxis("Horizontal") > deadzone)
                    {
                        player.motor.rotation = cameraMode.rotation;
                        velocity += right;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetKey(KeyCode.JoystickButton0))
                    {
                        if (player.IsGrounded())
                        {
                            player.motor.ForcedUnground();
                            player.Jump();
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

                    player.SetVelocity(velocity);
                }
                else
                {
                    fly = false;
                    flyOff = true;
                }
            }
            else
            {
                if (!flyOff && player != null)
                {
                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(player, true);
                    player.CompletelyStop();
                    flyOff = true;
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.P)) { ToggleCursor(GetGameInput(), GetGameplayCamera(player, loadedBaseModule)); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.R)) { FillBoostMax(player); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.N)) { GoToNextSpawn(player); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.M)) { GoToNextDreamSpawn(player); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.H)) { SaveLoad(GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.J)) { SaveLoad(GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) { GoToStage(loadedBaseModule); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) { SelectNextStage(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Backslash)) { fly = false; noclip = !noclip; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Slash)) { noclip = false; fly = !fly; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.I)) { invul = !invul; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightBracket)) { NextChar(player, true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftBracket)) { NextChar(player, false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Minus)) { NextStyle(player, true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals)) { NextStyle(player, false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Period)) { NextOutfit(player, true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Comma)) { NextOutfit(player, false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.L)) { LimitFPS(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.K)) { EndWanted(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.O)) { SetStorage(player, savedStorage); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.T)) { timescaleEnabled = !timescaleEnabled; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Quote)) { open = !open; }
        }

        // ---------- FUNCTIONS ----------
        public void ToggleCursor(GameInput gameInput, GameplayCamera gameplayCamera)
        {
            if (gameInput != null && !corePuased && Reptile.Utility.GetIsCurrentSceneStage() && Reptile.Utility.GetCurrentStage() != Stage.NONE)
            {
                if (!Cursor.visible && !Core.Instance.IsCorePaused)
                {
                    if (gameplayCamera != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gameplayCamera);
                        cameraMode.inputEnabled = false;
                    }
                    isMenuing = true;
                    gameInput.SetUICursorMode();
                }
                else
                {
                    if (gameplayCamera != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gameplayCamera);
                        cameraMode.inputEnabled = true;
                    }
                    isMenuing = false;
                    gameInput.SetGameCursorMode();
                }
            }
        }

        public void FillBoostMax(Player player)
        {
            if (player != null)
            {
                player.AddBoostCharge((float)typeof(Player).GetField("maxBoostCharge", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GetPlayer()));
            }
        }

        public void GoToNextSpawn(Player player)
        {
            if (player != null)
            {
                player.transform.position = respawners.ToArray()[currentRespawner];
                if (currentRespawner + 1 < respawners.Count()) { currentRespawner++; } else { currentRespawner = 0; }
            }
        }

        public void GoToNextDreamSpawn(Player player)
        {
            if (player != null && WorldHandler.instance != null)
            {
                if (WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter() != null)
                {
                    player.transform.position = dreamRespawners.ToArray()[currentDreamRespawner];
                    if (currentDreamRespawner + 1 < dreamRespawners.Count()) { currentDreamRespawner++; } else { currentDreamRespawner = 0; }
                }
            }
        }

        public void SaveLoad(Player player, bool save)
        {
            if (player != null)
            {
                if (save)
                {
                    savedPos = player.tf.position;
                    savedAng = player.tf.rotation;

                    savePosX = savedPos.x.ToString();
                    savePosY = savedPos.y.ToString();
                    savePosZ = savedPos.z.ToString();

                    savedStorage = storageSpeed;
                    savedStorageS = savedStorage.ToString();

                    if (shouldSaveVel)
                    {
                        savedVel = player.GetPracticalWorldVelocity();
                        saveVelX = savedVel.x.ToString();
                        saveVelY = savedVel.y.ToString();
                        saveVelZ = savedVel.z.ToString();
                    }
                }
                else
                {
                    if (float.TryParse(savePosX, out _) && float.TryParse(savePosY, out _) && float.TryParse(savePosZ, out _))
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(new Vector3(float.Parse(savePosX), float.Parse(savePosY), float.Parse(savePosZ)), savedAng, true);
                    }
                    else
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(savedPos, savedAng, true);
                    }
                    SetStorage(player, savedStorage);
                    player.SetVelocity(savedVel);
                }
            }
        }

        public void SelectNextStage()
        {
            selectedStageV++;
            if (selectedStageV > 7) { selectedStageV = 0; }
            switch (selectedStageV)
            {
                case 0:
                    selectedStage = Stage.hideout;
                    break;
                case 1:
                    selectedStage = Stage.downhill;
                    break;
                case 2:
                    selectedStage = Stage.square;
                    break;
                case 3:
                    selectedStage = Stage.tower;
                    break;
                case 4:
                    selectedStage = Stage.Mall;
                    break;
                case 5:
                    selectedStage = Stage.pyramid;
                    break;
                case 6:
                    selectedStage = Stage.osaka;
                    break;
                case 7:
                    selectedStage = Stage.Prelude;
                    break;
                default:
                    break;
            }
        }

        public void GoToStage(BaseModule baseModule)
        {
            if (baseModule != null)
            {
                if (!baseModule.IsLoading && !corePuased && baseModule.StageManager != null && Reptile.Utility.GetCurrentStage() != Stage.NONE && Reptile.Utility.GetIsCurrentSceneStage())
                {
                    if (!baseModule.StageManager.IsExtendingLoadingScreen)
                    {
                        isMenuing = false;
                        baseModule.StageManager.ExitCurrentStage(selectedStage, Stage.NONE);
                    }
                }
            }
        }

        public void NextChar(Player player, bool nextChar = true)
        {
            if (player != null)
            {
                if (nextChar) { currentChar++; } else { currentChar--; }
                if (currentChar > (int)Characters.MAX - 1) { currentChar = 0; }
                else if (currentChar < 0) { currentChar = (int)Characters.MAX - 1; }

                bool shouldChange = false;
                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != MoveStyle.ON_FOOT) { shouldChange = true; }

                player.SetCharacter((Characters)currentChar);
                currentCharIndex = (Characters)currentChar;

                var initHitboxes = player.GetType().GetMethod("InitHitboxes", BindingFlags.NonPublic | BindingFlags.Instance);
                initHitboxes?.Invoke(player, new object[] { });

                var initCuffs = player.GetType().GetMethod("initCuffs", BindingFlags.NonPublic | BindingFlags.Instance);
                initCuffs?.Invoke(player, new object[] { });

                player.SetCurrentMoveStyleEquipped((MoveStyle)currentStyle, true, true);
                if (shouldChange) { player.SwitchToEquippedMovestyle(true); }
            }
        }

        public void NextStyle(Player player, bool nextStyle = true)
        {
            if (player != null)
            {
                if (nextStyle) { currentStyle++; } else { currentStyle--; }
                if (currentStyle > (int)MoveStyle.MAX - 1) { currentStyle = 1; }
                else if (currentStyle < 1) { currentStyle = (int)MoveStyle.MAX - 1; }

                bool shouldChange = false;
                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != MoveStyle.ON_FOOT) { shouldChange = true; }

                player.SetCurrentMoveStyleEquipped((MoveStyle)currentStyle, true, true);
                if (shouldChange) { player.SwitchToEquippedMovestyle(true); }
                currentStyleIndex = (MoveStyle)currentStyle;
            }
        }

        public void NextOutfit(Player player, bool nextOutfit = true)
        {
            if (player != null)
            {
                if (nextOutfit) { currentOutfit++; } else { currentOutfit--; }
                if (currentOutfit > 3) { currentOutfit = 0; }
                else if (currentOutfit < 0) { currentOutfit = 3; }

                player.SetOutfit(currentOutfit);
            }
        }

        public void LimitFPS()
        {
            if (Core.Instance != null)
            {
                limitFPS = !limitFPS;
                if (limitFPS)
                {
                    UnityEngine.Application.targetFrameRate = fpsLimit;
                }
                else
                {
                    UnityEngine.Application.targetFrameRate = -1;
                }
            }
        }

        public void EndWanted()
        {
            if (wantedManager != null)
            {
                wantedManager.StopPlayerWantedStatus(true);
            }
        }

        public void SetStorage(Player player, float storage)
        {
            if (wallrunLineAbility != null && player != null)
            {
                FieldInfo lastSpeed = typeof(WallrunLineAbility).GetField("lastSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
                lastSpeed.SetValue(wallrunLineAbility, storage);
            }
        }

    }
}
