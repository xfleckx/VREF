using System.Collections.Generic;
using System;

namespace Assets.BeMoBI.Paradigms.SearchAndFind
{
    public class BriefStatisticForParadigmRun
    {
        private string subjectId = String.Empty;

        public class TrialStatistic
        {
            private string mazeName;
            private string conditionId;
            private int path;
            private double seconds;
            private string trialType;

            public TrialStatistic(string trialType, string mazeName, int path, double seconds)
            {
                this.trialType = trialType;
                this.mazeName = mazeName;
                this.path = path;
                this.seconds = seconds;
            }

            public string MazeName
            {
                get
                {
                    return mazeName;
                }
            }

            public int Path
            {
                get
                {
                    return path;
                }
            }

            public double DurationInSeconds
            {
                get
                {
                    return seconds;
                }
            }

            public string TrialType
            {
                get
                {
                    return trialType;
                }
            }
                         
            public string ConditionID
            {
                get
                {
                    return conditionId;
                }
            }
        }

        public List<TrialStatistic> Trials = new List<TrialStatistic>();

        public string SubjectId
        {
            get
            {
                return subjectId;
            }
        }

        public void Add(string trialType, string mazeName, int pathId, TrialResult result)
        {
            Trials.Add(new TrialStatistic(trialType, mazeName, pathId, result.Duration.TotalSeconds));
        }
    }
}