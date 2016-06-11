using UnityEngine;
using System.Collections;
using Assets.Paradigms.SearchAndFind.ImageEffects;

namespace Assets.BeMoBI.Paradigms.AbstractParadigm
{
    public class SceneLightingFogControl : BaseFogControl
    {
        [Range(0.001f, 10.0f)]
        public float DensityWhenRaised = 2.0f;
        public float RaisingSpeed = 0.1f;
        
        [Range(0.001f, 10.0f)]
        public float DensityWhenDisappeared = 2.0f;
        public float Disappearingspeed = 0.1f;

        public override void RaiseFog()
        {
            StopAllCoroutines();
            StartCoroutine(RaiseToTargetValues());
        }

        IEnumerator RaiseToTargetValues()
        {
            var state = FogHasRaisedCompletely();

            do
            {
                if (RenderSettings.fogDensity < DensityWhenRaised)
                    RenderSettings.fogDensity += 1 * RaisingSpeed * Time.deltaTime;
                
                yield return new WaitWhile(() => state);

            } while (!state) ;

            RenderSettings.fogDensity = DensityWhenRaised;
        }

        bool FogHasRaisedCompletely()
        {
            return RenderSettings.fogDensity >= DensityWhenRaised;
        }

        public override void LetFogDisappeare()
        {
            StopAllCoroutines();
            StartCoroutine(ReduceFogToTargetValues());
        }

        IEnumerator ReduceFogToTargetValues()
        {
            var state = FogHasDisappearedCompletely();

            do
            {
                if (RenderSettings.fogDensity > DensityWhenDisappeared)
                    RenderSettings.fogDensity -= 1 * Disappearingspeed * Time.deltaTime;
                
                yield return new WaitWhile(() => state);

            } while (!state);

            RenderSettings.fogDensity = DensityWhenDisappeared;
        }

        bool FogHasDisappearedCompletely()
        {
            return RenderSettings.fogDensity <= DensityWhenDisappeared;
        }

        public override void DisappeareImmediately()
        {
            RenderSettings.fogDensity = DensityWhenDisappeared;
        }

        public override void RaisedImmediately()
        {
            RenderSettings.fogDensity = DensityWhenRaised; 
        }
    }

}
