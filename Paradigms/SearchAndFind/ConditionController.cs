using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using NLog;
using Assets.VREF.Scripts.Paradigm;
using Assets.VREF.Scripts.Controls;
using Assets.VREF.Scripts.HWinterfaces;
using UnityEngine.SceneManagement;

namespace Assets.VREF.Paradigms.SearchAndFind
{
    public class ConditionController : MonoBehaviour, IConditionController
    {
        private static NLog.Logger statistic = LogManager.GetLogger("Statistics");

        private static NLog.Logger appLog = LogManager.GetLogger("App");

        public ParadigmController paradigm;

        public Action OnLastConditionFinished;

        public Action<string> OnConditionFinished;

        #region Condition state

        private LinkedList<TrialDefinition> currentLoadedTrialDefinitions;
        public LinkedListNode<TrialDefinition> currentTrialDefinition;
        public Trial currentTrial;

        public ConditionDefinition currentCondition;
        public ConditionConfiguration conditionConfig;

        private Dictionary<ITrial, int> runCounter = new Dictionary<ITrial, int>();

        private int currentTrialIndex = 0;

        private bool currentRunShouldEndAfterTrialFinished;
        private bool pauseActive = false;

        private bool isTrialRunning = false;
        public bool IsRunning
        {
            get
            {
                return isTrialRunning;
            }
        }

        public List<ConditionDefinition> PendingConditions { get; internal set; }
        public List<ConditionDefinition> FinishedConditions { get; internal set; }

        private bool isConditionRunning;
        public bool IsConditionRunning
        {
            get
            {
                return isConditionRunning;
            }
        }
        
        private bool resetTheLastTrial = false;

        #endregion

        public void Initialize(ConditionDefinition requestedCondition, bool andStart = false)
        {
            if (IsConditionRunning)
                throw new InvalidOperationException("A condition is already running!");
            
            if (PendingConditions.Any(c => c.Equals(requestedCondition))){
                currentCondition = requestedCondition;
                PendingConditions.Remove(requestedCondition);
                FinishedConditions.Add(requestedCondition);
            }
            else
            {
                throw new ArgumentException(string.Format("Requested Condition '{0}' not available - maybe already done?", requestedCondition.Identifier));
            }

            ApplyConditionSpecificConfiguration(currentCondition.Config);

            if (!currentCondition.Trials.Any())
                throw new InvalidOperationException("Selected condition doesn't contain trials!");

            currentLoadedTrialDefinitions = new LinkedList<TrialDefinition>(currentCondition.Trials);

            var statusMessage = string.Format("Initialize Condition: \'{0}\'", currentCondition.Identifier);

            appLog.Info(statusMessage);

            if (andStart)
                StartCurrentConditionWithFirstTrial();
        }
        
        public void StartCurrentConditionWithFirstTrial()
        {
            isConditionRunning = true;

            SetNextTrialPending();
        }

        public void ResetCurrentCondition()
        {
            isConditionRunning = false;
            PendingConditions.Add(currentCondition);
            Initialize(currentCondition);
        }

        #region Trial Management
        
        /// <summary>
        /// Bad code here... semantic is paradigm specific... :/ not abstract enough on this point.. should be moved to trials??
        /// </summary>
        /// <param name="config"></param>
        private void ApplyConditionSpecificConfiguration(ConditionConfiguration config)
        {
            conditionConfig = config;

            paradigm.subject.Change<IBodyMovementController>(config.BodyControllerName);
            paradigm.subject.Change<IHeadMovementController>(config.HeadControllerName);

            var vrHeadSetController = FindObjectOfType<IOculusRiftController>();

            vrHeadSetController.UseMonoscopigRendering = conditionConfig.UseMonoscopicViewOnVRHeadset;
        }

        /// <summary>
        /// Use this for regular breaks
        /// </summary>
        public void InjectPauseTrialAfterCurrentTrial()
        {
            var pauseTrial = new TrialDefinition()
            {
                TrialType = typeof(Pause).Name
            };

            if(currentTrialDefinition != null)
                currentLoadedTrialDefinitions.AddAfter(currentTrialDefinition, new LinkedListNode<TrialDefinition>(pauseTrial));
        }

        /// <summary>
        /// use this for instant breaks
        /// </summary>
        public void InjectPauseTrialBeforeCurrentTrial()
        {
            var pauseTrial = new TrialDefinition()
            {
                TrialType = typeof(Pause).Name
            };

            currentLoadedTrialDefinitions.AddBefore(currentTrialDefinition, new LinkedListNode<TrialDefinition>(pauseTrial));
        }
        
        public void ReturnFromPauseTrial()
        {
            if (currentTrial.Equals(paradigm.pause))
                currentTrial.ForceTrialEnd();
        }

        /// <summary>
        /// Setup the next trial but wait until subject enters startpoint
        /// 
        /// Take into a account that the paradigm definition is a linked list!
        /// </summary>
        void SetNextTrialPending()
        {
            isTrialRunning = true;

            if (currentTrialDefinition == null)
            {
                // Special case: First Trial after condition start
                paradigm.marker.Write(string.Format("Begin Condition '{0}'", currentCondition.Identifier));

                currentTrialDefinition = currentLoadedTrialDefinitions.First;
            }
            else if (currentRunShouldEndAfterTrialFinished || currentTrialDefinition.Next == null)
            {
                // Special case: Last Trial either the run was canceld or all trials done

                // Special case: First Trial after condition start
                paradigm.marker.Write(string.Format("End Condition '{0}'", currentCondition.Identifier));
                ConditionFinished();
                return;
            }
            else
            {
                if (!resetTheLastTrial)
                {
                    // normal case the next trial is the follower of the current trial due to the definition
                    currentTrialDefinition = currentTrialDefinition.Next;
                }
                else
                {
                    // current definition stays - e.g. when a pause was requested
                    resetTheLastTrial = false;
                }
            }

            var definitionForNextTrial = currentTrialDefinition.Value;

            if (definitionForNextTrial.TrialType.Equals(typeof(Training).Name))
            {
                Begin(paradigm.training, definitionForNextTrial);
            }
            else if (definitionForNextTrial.TrialType.Equals(typeof(Experiment).Name))
            {
                Begin(paradigm.experiment, definitionForNextTrial);

            }
            else if (definitionForNextTrial.TrialType.Equals(typeof(Pause).Name))
            {
                Begin(paradigm.pause, definitionForNextTrial);
            }
        }
        
        public void Begin<T>(T trial, TrialDefinition trialDefinition) where T : Trial
        {
            if (!(trial is Pause))
                currentTrialIndex++;

            appLog.Info(string.Format("Starting Trial {0} of {1}", currentTrialIndex, currentLoadedTrialDefinitions.Count));

            if (!runCounter.ContainsKey(trial))
                runCounter.Add(trial, 0);

            currentTrial = trial;

            Prepare(currentTrial);

            currentTrial.SetReady();
        }

        private void Prepare(Trial currentTrial)
        {
            currentTrial.conditionConfig = currentCondition.Config;

            currentTrial.gameObject.SetActive(true);

            currentTrial.enabled = true;

            currentTrial.paradigm = this.paradigm;

            var def = currentTrialDefinition.Value;

            currentTrial.Initialize(def.MazeName, def.Path, def.Category, def.ObjectName);

            // this sets a callback to Trials Finished event (it's an pointer to a function)
            currentTrial.Finished += CallWhenCurrentTrialFinished;
        }

        /// <summary>
        /// Callback method, should be called by the trial itself - cause only the trial knows when it's finished
        /// </summary>
        /// <param name="trial">the trial instance which has finished</param>
        /// <param name="result">A collection of informations on the trial run</param>
        void CallWhenCurrentTrialFinished(Trial trial, TrialResult result)
        {
            runCounter[trial]++;

            currentTrial.CleanUp();

            currentTrial.enabled = false;

            currentTrial.gameObject.SetActive(false);
            
            if (!pauseActive)
                SetNextTrialPending();
        }

        internal void ResetCurrentTrial()
        {
            if (currentTrial != null)
            {
                currentTrial.ForceTrialEnd();
            }
        }

        public void SetNextConditionPending(bool andStart = false)
        {
            if (PendingConditions.Count > 0)
                Initialize(PendingConditions.First(), andStart);
            else
                OnLastConditionFinished();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionName"></param>
        /// <exception cref="ArgumentException">Thrown If requested condition not available</exception>
        public void SetSpecificConditionPending(string conditionName, bool attempReRun = false)
        {
            if(PendingConditions.Any(c => c.Identifier.Equals(conditionName)))
            {
                var requestedCondition = PendingConditions.First(c => c.Identifier.Equals(conditionName));
                
                Initialize(requestedCondition);
                return;
            }


            if(FinishedConditions.Any(c => c.Identifier.Equals(conditionName)) && attempReRun)
            {
                Initialize(FinishedConditions.First(c => c.Identifier.Equals(conditionName)));
                return;
            }else
            {
                appLog.Error(string.Format("Requested condition '{0}' has been done already - force by using rerun=true!", conditionName));
                return;
            }
            
            throw new ArgumentException(string.Format("Requested Condition '{0}' not found!", conditionName), conditionName);
        }
        
        private void ConditionFinished()
        {
            isConditionRunning = false;

            currentTrialDefinition = null;

            currentTrialIndex = 0;

            GC.Collect();

            if (OnConditionFinished != null)
                OnConditionFinished(currentCondition.Identifier);
        }

        internal void ForceASaveEndOfCurrentCondition()
        {
            currentRunShouldEndAfterTrialFinished = true;
        }

        #endregion

    }
}