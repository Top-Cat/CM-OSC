using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CMOSC
{
    public class OSCMessageSender : MonoBehaviour
    {
        public static Config config;
        private readonly OSC osc = new OSC();
        private bool readyToGo;
        public static bool IsActive { get; private set; }

        public OSCMessageSender(Config config)
        {
            OSCMessageSender.config = config;
            ReloadOSCStats();
            osc.Awake();
        }

        public void ReloadOSCStats()
        {
            osc.ReloadOSC();
            readyToGo = ValidateOSCStatus(osc, out _, out _);
        }

        public bool ValidateOSCStatus(OSC osc, out bool ipMatches, out bool portsMatches)
        {
            //Create two regex expressions to make sure OSC is ready to use.
            Regex ipRegex = new Regex(@"\b(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))\b");
            Regex tcpIpPortRegex = new Regex(@"^([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$");
            //Probably dont need these checks, but its safe to have 'em.
            ipMatches = ipRegex.IsMatch(osc.outIP);
            portsMatches = tcpIpPortRegex.IsMatch(osc.inPort.ToString()) && tcpIpPortRegex.IsMatch(osc.outPort.ToString());
            return ipMatches && portsMatches;
        }

        public void EventPassed(BeatmapObject data)
        {
            if (!readyToGo) return;
            Debug.Log("Validate A");
            MapEvent e = data as MapEvent;
            if (!e.IsUtilityEvent) //Filter out Ring Spin, Ring Zoom, and Laser Speeds
            {
                List<OscMessage> messages = new List<OscMessage>(); //Collection of messages to mass send
                OscMessage mainMessage = new OscMessage();
                mainMessage.address = $"/pb/{e._type}/{e._value}";
                messages.Add(mainMessage);
                if (e._value >= ColourManager.RGB_INT_OFFSET) //If we have a Chroma event in our hands...
                {
                    Color color = ColourManager.ColourFromInt(e._value); //Grab Chroma color from data
                    OscMessage r = new OscMessage();
                    OscMessage g = new OscMessage(); //We hvae to make a new message for each RGB value, yay!
                    OscMessage b = new OscMessage();
                    r.address = $"/exec/\"R\"/{color.r}"; //Color values are floats from 0-1
                    g.address = $"/exec/\"G\"/{color.g}";
                    b.address = $"/exec/\"B\"/{color.b}";
                    messages.AddRange(new List<OscMessage>() { r, g, b }); //Smack these guys into our messages list
                }
                Debug.Log("Validate B");
                foreach (OscMessage message in messages) osc?.Send(message); //Send those guys down the pipe.
            }
        }

        public void OnDestroy()
        {
            osc.OnDestroy();
        }
    }
}
