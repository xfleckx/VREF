using UnityEngine;
using System.Collections;
using Assets.VREF.Scripts;
using Assets.VREF.Scripts.Instructions;
using UnityEngine.Assertions;
using Assets.VREF.Scripts.Interfaces;

namespace Assets.VREF.Examples.Paradigms.SimpleButtonPress {

    public class SimpleButtonPress : AExperiment
    {
        public GameObject markerStreamHost;
        public IMarkerStream marker;

        public InstructionHud instructions;

        public bool autoStart = false;

        public int trials;

        private int currentTrial = 0;

        private bool pause = false;

        #region Unity callbacks

        void Start()
        {
            marker = markerStreamHost.GetComponent<IMarkerStream>();

            if(autoStart)
                StartExperiment();

            Assert.IsNotNull(marker, "Add a MarkerStream instance! Implement the IMarkerStream interface on some script and reference it here!");
        }

        #endregion 


        #region experiment logic

        public override void StartExperiment()
        {
            marker.Write("Start Experiment");

            StartCoroutine(RunTrials());
        }

        IEnumerator RunTrials()
        {
            while (currentTrial < trials)
            {
                instructions.Clear();

                yield return new WaitForSeconds(2);

                instructions.ShowInstruction("Please press space bar", "Your task");

                yield return new WaitUntil(SubjectPressesButton);

                marker.Write("Button Press");

                instructions.ShowInstruction("Thank you!");

                yield return new WaitWhile(() => pause);

                yield return new WaitForSeconds(2);
                currentTrial++;
            }

            // end the experiment
            ExperimentHasFinished();

            yield return null;
        }

        private bool SubjectPressesButton()
        {
            return Input.anyKey;
        }

        public override void ForceSaveEndOfExperiment()
        {
            currentTrial = trials;
        }

        public override void InitializeCondition(string conditionName)
        {
            // nothing to do for now
        }

        public override void PauseTheExperiment()
        {
            pause = true;
        }

        public override void Restart()
        {
            currentTrial = 0;
        }

        public override void ReturnFromPause()
        {
            pause = false;
        }

        #endregion
    }
}
