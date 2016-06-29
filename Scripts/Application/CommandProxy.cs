using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.VREF.Scripts
{
    /// <summary>
    /// A proxy for several commands. It's intended to be used together with remote commands coming from SNAPEmulator or other
    /// kinds of Remote 
    /// </summary>
    public class CommandProxy : MonoBehaviour
    {
        public InitializeConditionEvent InitializeCondition;

        public UnityEvent StartRequested;
        public UnityEvent PauseRequested;
        public UnityEvent PauseEndRequested;
        public UnityEvent SaveEndRequested;
        public UnityEvent RestartConditionRequested;
        public UnityEvent RecalibrationRequested;


        /// <summary>
        /// A call back which could be used on the point where the reciever got messages
        /// </summary>
        /// <param name="command"></param>
        public void RecieveAndApply(string command)
        {
            if (command.Contains("config"))
            {
                var parts = command.Split(' ');
                var conditionName = parts[1].Replace("\n", "").Trim();

                if (InitializeCondition.GetPersistentEventCount() > 0)
                    InitializeCondition.Invoke(conditionName);

                return;
            }

            if (command.Equals("start"))
            {
                if (StartRequested.GetPersistentEventCount() > 0)
                    StartRequested.Invoke();

                return;
            }

            if (command.Equals("pause"))
            {
                if (PauseRequested.GetPersistentEventCount() > 0)
                    PauseRequested.Invoke();

                return;
            }

            if (command.Equals("pause end"))
            {
                if (PauseEndRequested.GetPersistentEventCount() > 0)
                    PauseEndRequested.Invoke();

                return;
            }

            if (command.Equals("force_end_of_experiment"))
            {
                if (SaveEndRequested.GetPersistentEventCount() > 0)
                    SaveEndRequested.Invoke();

                return;
            }

            if (command.Contains("restart"))
            {
                if (RestartConditionRequested.GetPersistentEventCount() > 0)
                    RestartConditionRequested.Invoke();

                return;
            }

            if (command.Equals("recalibrate_SubjectsOrientation"))
            {
                if (RecalibrationRequested.GetPersistentEventCount() > 0)
                    RecalibrationRequested.Invoke();

                return;
            }
        }

    }

    /// <summary>
    /// Unity specific event implementation to make instance of this event serializable.
    /// It will be serialized and displayed in the inspector.
    /// </summary>
    [Serializable]
    public class InitializeConditionEvent : UnityEvent<String> { }

}