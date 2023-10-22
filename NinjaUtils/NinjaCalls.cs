using Reptile;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NinjaUtils
{
    internal class NinjaCalls : MonoBehaviour
    {
        public static NinjaCalls Instance;

        public NinjaCalls()
        {
            Instance = this;
        }

        // SAVE / LOAD
        public Vector3 savedPos = Vector3.zero;
        public Quaternion savedAng = new Quaternion(0.00000f, 0.70926f, 0.00000f, -0.70495f);
        public Vector3 savedVel = Vector3.zero;

        public bool shouldSaveVel = true;

        // STAGE SELECT
        public Stage selectedStage = Stage.hideout;
        public int selectedStageV = 0;

        // CORE
        public bool corePuased = true;

        // NOCLIP
        public float noclipSpeed = 50f;
        public bool noclip = false;
        public bool noclipOff = true;
        public Vector3 noclipPos = Vector3.zero;

        // FLY
        public float flySpeed = 25f;
        public bool fly = false;
        public bool flyOff = true;

        // INVULNERABILITY
        public bool invul = false;

        // CHAR SELECT
        public int currentChar = 0;
        public Characters currentCharIndex = Characters.NONE;

        // STYLE SELECT
        public int currentStyle = 2;
        public MoveStyle currentStyleIndex = MoveStyle.SKATEBOARD;

        // OUTFIT SELECT
        public int currentOutfit = 0;

        // FPS CAP
        public bool limitFPS = false;
        public int fpsLimit = 30;

        // PLAYER SPEED
        public float playerSpeed = 0;
        public float playerSpeedMax = 0;

        // WANTED
        public WantedManager wantedManager;
        public bool isWanted = false;

        // STORAGE
        public WallrunLineAbility wallrunLineAbility;
        public float storageSpeed = 0f;
        public float savedStorage = 0f;

        // MENU
        public bool isMenuing = false;
        public bool isPaused = false;

        // TIMESCALE
        public bool timescaleEnabled = false;
        public float timescale = 0.1f;

        // STAGE SELECT
        public Stage currentStage = Stage.NONE;

        // SPAWNS
        public List<Vector3> respawners = new List<Vector3>();
        public int currentRespawner = 0;

        // DREAM SPAWNS
        public List<Vector3> dreamRespawners = new List<Vector3>();
        public int currentDreamRespawner = 0;

        // OBJECTS
        public Player player;
        public BaseModule loadedBaseModule;
        public GameplayCamera gameplayCamera;
        public GameInput gameInput;

        // GUI
        public String savePosX = "";
        public String savePosY = "";
        public String savePosZ = "";

        public String saveVelX = "";
        public String saveVelY = "";
        public String saveVelZ = "";

        public String savedStorageS = "";

        public String noclipSpeedS = "";
        public String flySpeedS = "";

        public String fpsLimitS = "";

        public String timescaleS = "";

        // SAVES
        

        public GameplayCamera GetGameplayCamera(Player player)
        {
            if (player != null && loadedBaseModule != null)
            {
                if (!loadedBaseModule.IsLoading)
                {
                    gameplayCamera = (GameplayCamera)typeof(Player).GetField("cam", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);
                }
            }
            return gameplayCamera != null ? gameplayCamera : null;
        }

        public GameInput GetGameInput()
        {
            if (loadedBaseModule != null)
            {
                gameInput = (GameInput)typeof(BaseModule).GetField("gameInput", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(loadedBaseModule);
            }
            return gameInput != null ? gameInput : null;
        }

        public Player GetPlayer()
        {
            if (WorldHandler.instance != null)
            {
                player = WorldHandler.instance.GetCurrentPlayer();
            }
            return player != null ? player : null;
        }
    }
}