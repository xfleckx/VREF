using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Assertions;
using Assets.VREF.Interfaces;
using Assets.VREF.Controls;

namespace Assets.VREF.Application
{
    public class CommandReciever : MonoBehaviour
    {
        public IParadigmControl paradigm;
        public VRSubjectController subject;

        // Use this for initialization
        void Start()
        {
            paradigm = GetComponent<IParadigmControl>();
            subject = FindObjectOfType<VRSubjectController>();

            Assert.IsNotNull(subject);
        }

        public void RecieveAndApply(string command)
        {
            if (command.Contains("config"))
            {
                var parts = command.Split(' ');
                var conditionName = parts[1].Replace("\n","").Trim();
                
                paradigm.InitializeCondition(conditionName);
                return;
            }

            if (command.Equals("start"))
            {
                paradigm.StartExperimentWithCurrentPendingCondition();
                return;
            }

            if (command.Equals("pause"))
            {
                paradigm.ConditionController.InjectPauseTrialAfterCurrentTrial();
                return;
            }

            if(command.Equals("pause end"))
            {
                paradigm.ConditionController.ReturnFromPauseTrial();
                return;
            }

            if (command.Equals("recalibrate_SubjectsOrientation"))
            {
                if (subject != null)
                    subject.Recalibrate();

                return;
            }

            if (command.Equals("force_end_of_experiment"))
            {
                paradigm.ForceSaveEndOfExperiment();
                return;
            }

            if (command.Contains("restart"))
            {
                paradigm.Restart();
                return;
            }
        }

    }

}

