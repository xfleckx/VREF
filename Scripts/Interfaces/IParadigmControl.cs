using UnityEngine;
using System.Collections;
using Assets.VREF.Interfaces;

namespace Assets.VREF.Scripts
{
    public interface IParadigmControl
    {
        void StartExperimentFromBeginning();

        void StartExperimentWithCurrentPendingCondition();

        void InitializeCondition(string conditionName);

        void ForceSaveEndOfExperiment();

        void ForceABreakInstantly();

        void Restart(string condition = "");
    }
}

