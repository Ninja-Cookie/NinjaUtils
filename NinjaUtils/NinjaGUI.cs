using Reptile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace NinjaUtils
{
    internal class NinjaGUI : MonoBehaviour
    {
        public static NinjaGUI Instance;

        private NinjaFunction ninjaFunction;
        private NinjaCalls ninjaCalls;
        private TriggerTools triggerTools;

        public NinjaGUI()
        {
            Instance = this;
            ninjaFunction = NinjaFunction.Instance;
            ninjaCalls = NinjaCalls.Instance;
            triggerTools = TriggerTools.Instance;
        }

        private bool open = true;

        private Rect winRect = new Rect(20, 20, 275, 769);

        void OnGUI()
        {
            if (open)
            {
                winRect = GUI.Window(0, winRect, NinjaUtilsGUI, Utils.pluginName + " (" + Utils.pluginVersion + ")");
            }
        }

        private int sidePadding = 10;
        private int elementSizeW = 100;
        private int elementSizeH = 20;
        private int lineSpacing = 2;
        private float buttonSpacing = 1.5f;
        private int linePos;

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
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, $"Core ({(ninjaCalls.corePuased ? "<color=red>Paused</color>" : "<color=green>Running</color>")})", colorWhite);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Speed: " + ninjaCalls.playerSpeed.ToString("0.00"), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Highest Speed: " + ninjaCalls.playerSpeedMax.ToString("0.00"), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Storage Speed: " + ninjaCalls.storageSpeed.ToString("0.00"), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), "Fill Boost (R)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.FillBoostMax(ninjaCalls.GetPlayer());
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle invulnerable ({(ninjaCalls.invul ? "<color=green>On</color>" : "<color=red>Off</color>")}) (i)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaCalls.invul = !ninjaCalls.invul;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"End Wanted ({(ninjaCalls.isWanted ? "<color=red>Wanted</color>" : "<color=green>Safe</color>")}) (K)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.EndWanted();
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Position (X, Y, Z)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            ninjaCalls.savePosX = GUI.TextArea(new Rect(sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), ninjaCalls.savePosX);
            ninjaCalls.savePosY = GUI.TextArea(new Rect((winRect.width / 2) - (((winRect.width / 3) - (sidePadding * buttonSpacing)) / 2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), ninjaCalls.savePosY);
            ninjaCalls.savePosZ = GUI.TextArea(new Rect(winRect.width - ((winRect.width / 3) - (sidePadding * buttonSpacing)) - sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), ninjaCalls.savePosZ);

            if (!float.TryParse(ninjaCalls.savePosX, out _)) { ninjaCalls.savePosX = ninjaCalls.savedPos.x.ToString(); }
            if (!float.TryParse(ninjaCalls.savePosY, out _)) { ninjaCalls.savePosY = ninjaCalls.savedPos.y.ToString(); }
            if (!float.TryParse(ninjaCalls.savePosZ, out _)) { ninjaCalls.savePosZ = ninjaCalls.savedPos.z.ToString(); }

            ninjaCalls.savedPos.x = float.Parse(ninjaCalls.savePosX);
            ninjaCalls.savedPos.y = float.Parse(ninjaCalls.savePosY);
            ninjaCalls.savedPos.z = float.Parse(ninjaCalls.savePosZ);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Velocity (X, Y, Z)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            ninjaCalls.saveVelX = GUI.TextArea(new Rect(sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), ninjaCalls.saveVelX);
            ninjaCalls.saveVelY = GUI.TextArea(new Rect((winRect.width / 2) - (((winRect.width / 3) - (sidePadding * buttonSpacing)) / 2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), ninjaCalls.saveVelY);
            ninjaCalls.saveVelZ = GUI.TextArea(new Rect(winRect.width - ((winRect.width / 3) - (sidePadding * buttonSpacing)) - sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), ninjaCalls.saveVelZ);

            if (!float.TryParse(ninjaCalls.saveVelX, out _)) { ninjaCalls.saveVelX = ninjaCalls.savedVel.x.ToString(); }
            if (!float.TryParse(ninjaCalls.saveVelY, out _)) { ninjaCalls.saveVelY = ninjaCalls.savedVel.y.ToString(); }
            if (!float.TryParse(ninjaCalls.saveVelZ, out _)) { ninjaCalls.saveVelZ = ninjaCalls.savedVel.z.ToString(); }

            ninjaCalls.savedVel.x = float.Parse(ninjaCalls.saveVelX);
            ninjaCalls.savedVel.y = float.Parse(ninjaCalls.saveVelY);
            ninjaCalls.savedVel.z = float.Parse(ninjaCalls.saveVelZ);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Saved Storage", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            ninjaCalls.savedStorageS = GUI.TextArea(new Rect((winRect.width / 2) - (((winRect.width / 3) - (sidePadding * buttonSpacing)) / 2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), ninjaCalls.savedStorageS);

            if (!float.TryParse(ninjaCalls.savedStorageS, out _)) { ninjaCalls.savedStorageS = ninjaCalls.savedStorage.ToString(); }

            ninjaCalls.savedStorage = float.Parse(ninjaCalls.savedStorageS);

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), "Set Storage Speed (O)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.SetStorage(ninjaCalls.GetPlayer(), ninjaCalls.savedStorage);
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Saving Velocity ({(ninjaCalls.shouldSaveVel ? "<color=green>On</color>" : "<color=red>Off</color>")})") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaCalls.shouldSaveVel = !ninjaCalls.shouldSaveVel;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Save Position (H)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.SaveLoad(ninjaCalls.GetPlayer(), true);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Load Position (J)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.SaveLoad(ninjaCalls.GetPlayer(), false);
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Spawns (N)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.GoToNextSpawn(ninjaCalls.GetPlayer());
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Dream Spawns (M)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.GoToNextDreamSpawn(ninjaCalls.GetPlayer());
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Current Stage: " + ninjaCalls.currentStage.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Selected Stage: " + ninjaCalls.selectedStage.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), $"Go To Stage {(ninjaCalls.isPaused ? "<color=red>(Off)</color>" : "")} (1)") && ninjaCalls.isMenuing)
            {
                ninjaFunction.GoToStage(ninjaCalls.loadedBaseModule);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Select Stage (2)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.SelectNextStage();
            }

            linePos = linePos + ((elementSizeH) + lineSpacing);
            if (ninjaCalls.fly) { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: flying", colorWhite, colorBlack); }
            else if (ninjaCalls.noclip) { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: noclip", colorWhite, colorBlack); }
            else { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: Normal", colorWhite, colorBlack); }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Toggle noclip (\\)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaCalls.fly = false;
                ninjaCalls.noclip = !ninjaCalls.noclip;
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Enable fly (/)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaCalls.noclip = false;
                ninjaCalls.fly = !ninjaCalls.fly;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            ninjaCalls.noclipSpeedS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), ninjaCalls.noclipSpeedS);
            ninjaCalls.flySpeedS = GUI.TextArea(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), ninjaCalls.flySpeedS);

            if (!float.TryParse(ninjaCalls.noclipSpeedS, out _)) { ninjaCalls.noclipSpeedS = ninjaCalls.noclipSpeed.ToString(); }
            if (!float.TryParse(ninjaCalls.flySpeedS, out _)) { ninjaCalls.flySpeedS = ninjaCalls.flySpeed.ToString(); }

            ninjaCalls.noclipSpeed = float.Parse(ninjaCalls.noclipSpeedS);
            ninjaCalls.flySpeed = float.Parse(ninjaCalls.flySpeedS);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Character: " + ninjaCalls.currentCharIndex.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Style: " + ninjaCalls.currentStyleIndex.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Char ([)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.NextChar(ninjaCalls.GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Char (])") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.NextChar(ninjaCalls.GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Style (-)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.NextStyle(ninjaCalls.GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Style (+)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.NextStyle(ninjaCalls.GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Outfit (,)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.NextOutfit(ninjaCalls.GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Outfit (.)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.NextOutfit(ninjaCalls.GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH * 2);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Limit FPS ({(ninjaCalls.limitFPS ? "<color=green>On</color>" : "<color=red>Off</color>")}) (L)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaFunction.LimitFPS();
            }

            linePos = linePos + (elementSizeH);
            ninjaCalls.fpsLimitS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), ninjaCalls.fpsLimitS);

            if (!int.TryParse(ninjaCalls.fpsLimitS, out _)) { ninjaCalls.fpsLimitS = ninjaCalls.fpsLimit.ToString(); }
            ninjaCalls.fpsLimit = int.Parse(ninjaCalls.fpsLimitS);



            linePos = linePos + (elementSizeH * 2);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Timescale ({(ninjaCalls.timescaleEnabled ? "<color=green>On</color>" : "<color=red>Off</color>")}) (T)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                ninjaCalls.timescaleEnabled = !ninjaCalls.timescaleEnabled;
            }

            linePos = linePos + (elementSizeH);
            ninjaCalls.timescaleS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), ninjaCalls.timescaleS);

            if (!float.TryParse(ninjaCalls.timescaleS, out _)) { ninjaCalls.timescaleS = ninjaCalls.timescale.ToString(); }
            ninjaCalls.timescale = float.Parse(ninjaCalls.timescaleS);

            linePos = linePos + (elementSizeH*2 + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Triggers ({(triggerTools.DisplayTriggerZones ? "<color=green>On</color>" : "<color=red>Off</color>")}) (X)") && (ninjaCalls.isMenuing || ninjaCalls.isPaused))
            {
                triggerTools.DisplayTriggerZones = !triggerTools.DisplayTriggerZones;
            }
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

        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P)) { ninjaFunction.ToggleCursor(ninjaCalls.GetGameInput(), ninjaCalls.GetGameplayCamera(ninjaCalls.GetPlayer())); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.R)) { ninjaFunction.FillBoostMax(ninjaCalls.GetPlayer()); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.N)) { ninjaFunction.GoToNextSpawn(ninjaCalls.GetPlayer()); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.M)) { ninjaFunction.GoToNextDreamSpawn(ninjaCalls.GetPlayer()); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.H)) { ninjaFunction.SaveLoad(ninjaCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.J)) { ninjaFunction.SaveLoad(ninjaCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) { ninjaFunction.GoToStage(ninjaCalls.loadedBaseModule); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) { ninjaFunction.SelectNextStage(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Backslash)) { ninjaCalls.fly = false; ninjaCalls.noclip = !ninjaCalls.noclip; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Slash)) { ninjaCalls.noclip = false; ninjaCalls.fly = !ninjaCalls.fly; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.I)) { ninjaCalls.invul = !ninjaCalls.invul; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightBracket)) { ninjaFunction.NextChar(ninjaCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftBracket)) { ninjaFunction.NextChar(ninjaCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Minus)) { ninjaFunction.NextStyle(ninjaCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals)) { ninjaFunction.NextStyle(ninjaCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Period)) { ninjaFunction.NextOutfit(ninjaCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Comma)) { ninjaFunction.NextOutfit(ninjaCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.L)) { ninjaFunction.LimitFPS(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.K)) { ninjaFunction.EndWanted(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.O)) { ninjaFunction.SetStorage(ninjaCalls.GetPlayer(), ninjaCalls.savedStorage); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.T)) { ninjaCalls.timescaleEnabled = !ninjaCalls.timescaleEnabled; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Quote)) { open = !open; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.X)) { triggerTools.DisplayTriggerZones = !triggerTools.DisplayTriggerZones; }

            //if (UnityEngine.Input.GetKeyDown(KeyCode.V)) { ninjaFunction.VisualizeZip(); }
            //if (UnityEngine.Input.GetKeyDown(KeyCode.B)) { ninjaFunction.HighlightWalls(); }
        }
    }
}
