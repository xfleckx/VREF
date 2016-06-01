using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.BeMoBI.Scripts.Paradigm
{
    public interface IConditionController
    {
        void InjectPauseTrialAfterCurrentTrial();

        void ReturnFromPauseTrial();
    }
}
