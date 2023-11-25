using System.IO;
using System.Threading.Tasks;
using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.Networking;
using LCSoundTool;

namespace no00ob.Mod.LethalCompany.CustomPingSound
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "CustomPingSound";
        private const string PLUGIN_NAME = "Custom Ping Sound";
        private const string PLUGIN_VERSION = "1.0.0";

        public static Plugin Instance;

        internal ManualLogSource logger;

        private AudioClip pingSound;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                logger = BepInEx.Logging.Logger.CreateLogSource(PLUGIN_GUID);

                logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

                //harmony.PatchAll(typeof(Plugin));
            }
        }

        async void Start()
        {
            // build absolute path
            var path = Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "CustomPingSound", "ping.wav");//Path.Combine(Application.dataPath, "BepInEx", "plugins", "CustomPingSound", "ping.wav");

            //path.Replace("/Lethal Company_Data", "");

            logger.LogDebug(path);

            WWW w = new WWW(path);

            while (!w.isDone)
            {
            }

            pingSound = w.GetAudioClip();

            // replace the specified audio clip with our new one using LC Sound Tool
            SoundTool.ReplaceAudioClip("Button2", pingSound);
            SoundTool.ReplaceAudioClip("Button3", pingSound);
        }
    }
}