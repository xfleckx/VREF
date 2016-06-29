using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Represents the logik to display LogMessages in a dedicated HUD - this is important to debug and test HMD VR scenes
/// </summary>
namespace Assets.VREF.Scripts.TestAndDebugging
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class LogMessageDisplay : MonoBehaviour {

        private const string LOG_SLOT_PATTERN = "Log_Slot_{0}";

        private Queue<String> messages;

        [Range(1,7)]
        public int MessageBufferSize = 7;

        void Start()
        {
            messages = new Queue<string>(MessageBufferSize);
        }

        public void WriteLog(string logMessage)
        {
            if (messages.Count == MessageBufferSize)
                messages.Dequeue();

            messages.Enqueue(logMessage);

            RenderLogs();
        }

        private void RenderLogs()
        {
            var messagesAsArray = messages.Reverse().ToArray();

            for (int i = 0; i < messages.Count; i++)
            {
                var slotHost = transform.FindChild(string.Format(LOG_SLOT_PATTERN, i));
                var textComponent = slotHost.GetComponent<Text>();
                textComponent.text = messagesAsArray[i];
            }
        }

        public void CleanLogs()
        {
            messages.Clear();

            var logSlots = transform.GetComponentsInChildren<Text>();

            foreach (var item in logSlots)
            {
                item.text = string.Empty;
            }
        }
    }

}