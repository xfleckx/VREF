
using Assets.VREF.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.VREF.Scripts.BaseParadigm
{
    public class Condition<TConditionDefinition> : MonoBehaviour, IConditionController<TConditionDefinition>
    {
        public event Action OnLastConditionFinished;
        public event Action<string> OnConditionFinished;

        private bool isRunning = false;
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }

            private set
            {
                isRunning = value;
            }
        }
        
        public IEnumerable<TConditionDefinition> PendingConditions
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public object InstanceDefinition { get; private set; }

        public void InjectPauseTrialAfterCurrentTrial()
        {
            throw new NotImplementedException();
        }

        public void ReturnFromPauseTrial()
        {
            throw new NotImplementedException();
        }

        public void SetNextConditionPending()
        {
            throw new NotImplementedException();
        }

        public void InitializeFirstOrDefaultCondition()
        {
            // TODO
            //PendingConditions = InstanceDefinition.Conditions;
            //FinishedConditions = new List<ConditionDefinition>();
        }

        public bool IsAConditionBeenInitialized()
        {
            throw new NotImplementedException("TODO");

           //return PendingConditions.Count == Config.ConditionConfigurations.Count
        }
    }
}
