using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.VREF.Interfaces
{
    public interface IConditionController
    {
        void InjectPauseTrialAfterCurrentTrial();

        void ReturnFromPauseTrial();
    }
}
