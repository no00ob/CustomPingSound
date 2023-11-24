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

            // wait for it to load and set the property
            pingSound = await LoadClip(path);

            // replace the specified audio clip with our new one using LC Sound Tool
            SoundTool.ReplaceAudioClip("GhostDevicePing", pingSound);
        }

        async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                uwr.SendWebRequest();

                // we have to wrap tasks in try/catch, otherwise it will just fail silently
                try
                {
                    while (!uwr.isDone)
                        await Task.Delay(5);

                    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.DataProcessingError)
                        logger.LogError($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception err)
                {
                    logger.LogError($"{err.Message}, {err.StackTrace}");
                }
            }

            return clip;
        }

        /*[HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]
        public static void HookPlayerControllerBUpdate()
        {
            if (toggleAudioSourceDebugLog.IsDown() && !wasKeyDown)
            {
                wasKeyDown = true;
            }
            if (toggleAudioSourceDebugLog.IsUp() && wasKeyDown)
            {
                wasKeyDown = false;
                debugAudioSources = !debugAudioSources;
                Instance.logger.LogDebug($"Toggling AudioSource debug logs {debugAudioSources}!");            }
        }*/
    }
}