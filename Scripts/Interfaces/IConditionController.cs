using System;
using System.Collections.Generic;

namespace Assets.VREF.Scripts.Interfaces
{
    public interface IConditionController<TConditionDefinition>
    {
        bool IsRunning { get; }

        IEnumerable<TConditionDefinition> PendingConditions { get; set; }

        void InitializeFirstOrDefaultCondition();

        void InjectPauseTrialAfterCurrentTrial();

        void ReturnFromPauseTrial();

        void SetNextConditionPending();

        event Action<string> OnConditionFinished;

        event Action OnLastConditionFinished;

        bool IsAConditionBeenInitialized();
    }
}
