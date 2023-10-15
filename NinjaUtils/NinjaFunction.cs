using Reptile;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NinjaUtils
{
    internal class NinjaFunction : MonoBehaviour
    {
        public static NinjaFunction Instance;

        private NinjaFunction ninjaFunction;
        private NinjaCalls ninjaCalls;
        private NinjaGUI ninjaGUI;

        public NinjaFunction()
        {
            Instance = this;
            ninjaFunction = NinjaFunction.Instance;
            ninjaCalls = NinjaCalls.Instance;
            ninjaGUI = NinjaGUI.Instance;
        }

        public void ToggleCursor(GameInput gameInput, GameplayCamera gameplayCamera)
        {
            if (gameInput != null && !ninjaCalls.corePuased && Reptile.Utility.GetIsCurrentSceneStage() && Reptile.Utility.GetCurrentStage() != Stage.NONE)
            {
                if (!Cursor.visible && !Core.Instance.IsCorePaused)
                {
                    if (gameplayCamera != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gameplayCamera);
                        cameraMode.inputEnabled = false;
                    }
                    ninjaCalls.isMenuing = true;
                    gameInput.SetUICursorMode();
                }
                else
                {
                    if (gameplayCamera != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gameplayCamera);
                        cameraMode.inputEnabled = true;
                    }
                    ninjaCalls.isMenuing = false;
                    gameInput.SetGameCursorMode();
                }
            }
        }

        public void FillBoostMax(Player player)
        {
            if (player != null)
            {
                player.AddBoostCharge((float)typeof(Player).GetField("maxBoostCharge", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player));
            }
        }

        public void GoToNextSpawn(Player player)
        {
            if (player != null)
            {
                player.transform.position = ninjaCalls.respawners.ToArray()[ninjaCalls.currentRespawner];
                if (ninjaCalls.currentRespawner + 1 < ninjaCalls.respawners.Count()) { ninjaCalls.currentRespawner++; } else { ninjaCalls.currentRespawner = 0; }
            }
        }

        public void GoToNextDreamSpawn(Player player)
        {
            if (player != null && WorldHandler.instance != null)
            {
                if (WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter() != null)
                {
                    player.transform.position = ninjaCalls.dreamRespawners.ToArray()[ninjaCalls.currentDreamRespawner];
                    if (ninjaCalls.currentDreamRespawner + 1 < ninjaCalls.dreamRespawners.Count()) { ninjaCalls.currentDreamRespawner++; } else { ninjaCalls.currentDreamRespawner = 0; }
                }
            }
        }

        public void SaveLoad(Player player, bool save)
        {
            if (player != null)
            {
                if (save)
                {
                    ninjaCalls.savedPos = player.tf.position;
                    ninjaCalls.savedAng = player.tf.rotation;

                    ninjaCalls.savePosX = ninjaCalls.savedPos.x.ToString();
                    ninjaCalls.savePosY = ninjaCalls.savedPos.y.ToString();
                    ninjaCalls.savePosZ = ninjaCalls.savedPos.z.ToString();

                    ninjaCalls.savedStorage = ninjaCalls.storageSpeed;
                    ninjaCalls.savedStorageS = ninjaCalls.savedStorage.ToString();

                    if (ninjaCalls.shouldSaveVel)
                    {
                        ninjaCalls.savedVel = player.GetPracticalWorldVelocity();
                        ninjaCalls.saveVelX = ninjaCalls.savedVel.x.ToString();
                        ninjaCalls.saveVelY = ninjaCalls.savedVel.y.ToString();
                        ninjaCalls.saveVelZ = ninjaCalls.savedVel.z.ToString();
                    }
                }
                else
                {
                    if (float.TryParse(ninjaCalls.savePosX, out _) && float.TryParse(ninjaCalls.savePosY, out _) && float.TryParse(ninjaCalls.savePosZ, out _))
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(new Vector3(float.Parse(ninjaCalls.savePosX), float.Parse(ninjaCalls.savePosY), float.Parse(ninjaCalls.savePosZ)), ninjaCalls.savedAng, true);
                    }
                    else
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(ninjaCalls.savedPos, ninjaCalls.savedAng, true);
                    }
                    SetStorage(player, ninjaCalls.savedStorage);
                    player.SetVelocity(ninjaCalls.savedVel);
                }
            }
        }

        public void SelectNextStage()
        {
            ninjaCalls.selectedStageV++;
            if (ninjaCalls.selectedStageV > 7) { ninjaCalls.selectedStageV = 0; }
            switch (ninjaCalls.selectedStageV)
            {
                case 0:
                    ninjaCalls.selectedStage = Stage.hideout;
                    break;
                case 1:
                    ninjaCalls.selectedStage = Stage.downhill;
                    break;
                case 2:
                    ninjaCalls.selectedStage = Stage.square;
                    break;
                case 3:
                    ninjaCalls.selectedStage = Stage.tower;
                    break;
                case 4:
                    ninjaCalls.selectedStage = Stage.Mall;
                    break;
                case 5:
                    ninjaCalls.selectedStage = Stage.pyramid;
                    break;
                case 6:
                    ninjaCalls.selectedStage = Stage.osaka;
                    break;
                case 7:
                    ninjaCalls.selectedStage = Stage.Prelude;
                    break;
                default:
                    break;
            }
        }

        public void GoToStage(BaseModule baseModule)
        {
            if (baseModule != null)
            {
                if (!baseModule.IsLoading && !ninjaCalls.corePuased && baseModule.StageManager != null && Reptile.Utility.GetCurrentStage() != Stage.NONE && Reptile.Utility.GetIsCurrentSceneStage())
                {
                    if (!baseModule.StageManager.IsExtendingLoadingScreen)
                    {
                        ninjaCalls.isMenuing = false;
                        baseModule.StageManager.ExitCurrentStage(ninjaCalls.selectedStage, Stage.NONE);
                    }
                }
            }
        }

        public void NextChar(Player player, bool nextChar = true)
        {
            if (player != null)
            {
                if (nextChar) { ninjaCalls.currentChar++; } else { ninjaCalls.currentChar--; }
                if (ninjaCalls.currentChar > (int)Characters.MAX - 1) { ninjaCalls.currentChar = 0; }
                else if (ninjaCalls.currentChar < 0) { ninjaCalls.currentChar = (int)Characters.MAX - 1; }

                bool shouldChange = false;
                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != MoveStyle.ON_FOOT) { shouldChange = true; }

                player.SetCharacter((Characters)ninjaCalls.currentChar);
                ninjaCalls.currentCharIndex = (Characters)ninjaCalls.currentChar;

                var initHitboxes = player.GetType().GetMethod("InitHitboxes", BindingFlags.NonPublic | BindingFlags.Instance);
                initHitboxes?.Invoke(player, new object[] { });

                var initCuffs = player.GetType().GetMethod("initCuffs", BindingFlags.NonPublic | BindingFlags.Instance);
                initCuffs?.Invoke(player, new object[] { });

                player.SetCurrentMoveStyleEquipped((MoveStyle)ninjaCalls.currentStyle, true, true);
                if (shouldChange) { player.SwitchToEquippedMovestyle(true); }
            }
        }

        public void NextStyle(Player player, bool nextStyle = true)
        {
            if (player != null)
            {
                if (nextStyle) { ninjaCalls.currentStyle++; } else { ninjaCalls.currentStyle--; }
                if (ninjaCalls.currentStyle > (int)MoveStyle.MAX - 1) { ninjaCalls.currentStyle = 1; }
                else if (ninjaCalls.currentStyle < 1) { ninjaCalls.currentStyle = (int)MoveStyle.MAX - 1; }

                bool shouldChange = false;
                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != MoveStyle.ON_FOOT) { shouldChange = true; }

                player.SetCurrentMoveStyleEquipped((MoveStyle)ninjaCalls.currentStyle, true, true);
                if (shouldChange) { player.SwitchToEquippedMovestyle(true); }
                ninjaCalls.currentStyleIndex = (MoveStyle)ninjaCalls.currentStyle;
            }
        }

        public void NextOutfit(Player player, bool nextOutfit = true)
        {
            if (player != null)
            {
                if (nextOutfit) { ninjaCalls.currentOutfit++; } else { ninjaCalls.currentOutfit--; }
                if (ninjaCalls.currentOutfit > 3) { ninjaCalls.currentOutfit = 0; }
                else if (ninjaCalls.currentOutfit < 0) { ninjaCalls.currentOutfit = 3; }

                player.SetOutfit(ninjaCalls.currentOutfit);
            }
        }

        public void LimitFPS()
        {
            if (Core.Instance != null)
            {
                ninjaCalls.limitFPS = !ninjaCalls.limitFPS;
                if (ninjaCalls.limitFPS)
                {
                    UnityEngine.Application.targetFrameRate = ninjaCalls.fpsLimit;
                }
                else
                {
                    UnityEngine.Application.targetFrameRate = -1;
                }
            }
        }

        public void EndWanted()
        {
            if (ninjaCalls.wantedManager != null)
            {
                ninjaCalls.wantedManager.StopPlayerWantedStatus(true);
            }
        }

        public void SetStorage(Player player, float storage)
        {
            if (ninjaCalls.wallrunLineAbility != null && player != null)
            {
                FieldInfo lastSpeed = typeof(WallrunLineAbility).GetField("lastSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
                lastSpeed.SetValue(ninjaCalls.wallrunLineAbility, storage);
            }
        }
    }
}