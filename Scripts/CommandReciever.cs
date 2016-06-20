using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Assertions;
using Assets.VREF.Scripts;

namespace Assets.BeMoBI.Scripts
{
    public class CommandReciever : MonoBehaviour
    {
        public IParadigmControl paradigm;
        //public VRSubjectController subject;

        // Use this for initialization
        void Start()
        {
            paradigm = GetComponent<IParadigmControl>();
           // subject = FindObjectOfType<ISubject>();
           // Assert.IsNotNull(subject);
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
                // TODO here write commands -> command pattern
                // var new ParadigmPauseCommand().Execute();
                //paradigm.ConditionController.InjectPauseTrialAfterCurrentTrial();
                return;
            }

            if(command.Equals("pause end"))
            { 
                // TODO here write commands -> command pattern
                // var new ParadigmPauseEndCommand().Execute();
                // paradigm.ConditionController.ReturnFromPauseTrial();
                return;
            }

            if (command.Equals("recalibrate_SubjectsOrientation"))
            {
                //if (subject != null)
                //    subject.Recalibrate();

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

