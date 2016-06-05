using UnityEngine;
using System.Collections;

namespace Assets.VREF.Application
{
    public class Pause : Trial
    {
        public override void Initialize(string mazeName, int pathID, string category, string objectName)
        {
            // does actual nothing
        }

        public override void SetReady()
        {
            OnBeforeStart();
            var marker = MarkerPattern.FormatBeginTrial(GetType().Name, string.Empty, -1, string.Empty, string.Empty);
            paradigm.marker.Write(marker);
            stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            paradigm.fading.StartFadeOut();
        }

        public override void ForceTrialEnd()
        {
            paradigm.fading.StartFadeIn();

            base.ForceTrialEnd();
        }
        
    }
}
