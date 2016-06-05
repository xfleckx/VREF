using UnityEngine;
using System.Collections;
using Assets.VREF.Interfaces;

namespace Assets.VREF.Interfaces
{
    public interface IParadigmControl
    {
        IConditionController ConditionController { get; }

        void StartExperimentFromBeginning(); 

        void StartExperimentWithCurrentPendingCondition();

        void InitializeCondition(string conditionName);

        void ForceSaveEndOfExperiment();

        void ForceABreakInstantly();

        void Restart(string condition = "");

    }
}

