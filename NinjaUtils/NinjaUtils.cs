using BepInEx;
using UnityEngine;

namespace NinjaUtils
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Utils : BaseUnityPlugin
    {
        public const string pluginGuid = "ninjacookie.brc.ninjautils";
        public const string pluginName = "NinjaUtils";
        public const string pluginVersion = "0.0.1";

        private GameObject _mod;
        private NinjaGUI _ninjaGUI;
        private NinjaCalls _ninjaCalls;
        private NinjaFunction _ninjaFunction;
        private NinjaUpdater _ninjaUpdater;
        private TriggerTools _triggerTools;
        private void Awake()
        {
            _ninjaCalls = new NinjaCalls();
            _ninjaFunction = new NinjaFunction();
            _ninjaUpdater = new NinjaUpdater();
            _triggerTools = new TriggerTools();
            _ninjaGUI = new NinjaGUI();

            _mod = new GameObject();
            _mod.AddComponent<NinjaCalls>();
            _mod.AddComponent<NinjaFunction>();
            _mod.AddComponent<NinjaUpdater>();
            _mod.AddComponent<TriggerTools>();
            _mod.AddComponent<NinjaGUI>();
            GameObject.DontDestroyOnLoad(_mod);
        }
    }
}