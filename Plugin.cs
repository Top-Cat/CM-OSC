using SimpleJSON;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CMOSC
{
    [Plugin("OSC")]
    public class Plugin
    {
        private OSCMessageSender oSCMessageSender;

        [Init]
        private void Init()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var jsonCfg = JSON.Parse(File.ReadAllText(Path.Combine(assemblyFolder, "config.json")));
            var cfg = new Config(jsonCfg["ip"].Value, jsonCfg["port"].AsInt, jsonCfg["chroma"].AsInt);

            oSCMessageSender = new OSCMessageSender(cfg);
        }

        [EventPassedThreshold]
        private void EventPassed(bool init, BeatmapObject data)
        {
            oSCMessageSender.EventPassed(data);
        }

        [Exit]
        private void Exit()
        {
            oSCMessageSender.OnDestroy();
        }
    }
}
