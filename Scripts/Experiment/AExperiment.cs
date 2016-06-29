using Assets.VREF.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.VREF.Scripts
{
    /// <summary>
    /// Implement this base class for simple experiments
    /// </summary>
    public abstract class AExperiment : MonoBehaviour, IControlExperiment
    {

        public abstract void StartExperiment();

        /// <summary>
        /// If the experiment needs an shutdown before it's ended.
        /// </summary>
        public abstract void ForceSaveEndOfExperiment();

        public abstract void InitializeCondition(string conditionName);

        public abstract void PauseTheExperiment();

        public abstract void Restart();

        public abstract void ReturnFromPause();

        public UnityEvent OnExperimentFinished;

        protected void ExperimentHasFinished()
        {
            if (OnExperimentFinished.GetPersistentEventCount() > 0)
                OnExperimentFinished.Invoke();
        }
    }
}