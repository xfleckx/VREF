

namespace Assets.VREF.Scripts.Interfaces
{

    /// <summary>
    /// A predefinition interface for Remote Controlling your experiment.
    /// Implement these methods and reference them in a ControlProxy instance.
    /// </summary>
    public interface IControlExperiment
    {
        void StartExperiment();

        void InitializeCondition(string conditionName);

        void ReturnFromPause();

        void PauseTheExperiment();

        void ForceSaveEndOfExperiment();

        void Restart();

    }
}