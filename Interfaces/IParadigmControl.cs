using UnityEngine;
using System.Collections;
using Assets.BeMoBI.Scripts.Paradigm;

namespace Assets.BeMoBI.Scripts
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

