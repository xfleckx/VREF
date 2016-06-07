using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

namespace Assets.VREF.Application.FogControl
{
    public class FogControl : BaseFogControl
    {
        public CustomGlobalFog fogEffect;
        
        [Range(0.001f, 10.0f)]
        public float HeightDensityWhenRaised = 2.0f;
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
                if (fogEffect.heightDensity < HeightDensityWhenRaised)
                    fogEffect.heightDensity += 1 * RaisingSpeed * Time.deltaTime;
                
                yield return new WaitWhile(() => state);

            } while (!state) ;

            fogEffect.heightDensity = HeightDensityWhenRaised;
        }

        bool FogHasRaisedCompletely()
        {
            return fogEffect.heightDensity >= HeightDensityWhenRaised;
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
                if (fogEffect.heightDensity > DensityWhenDisappeared)
                    fogEffect.heightDensity -= 1 * Disappearingspeed * Time.deltaTime;
                
                yield return new WaitWhile(() => state);

            } while (!state);
             
            fogEffect.heightDensity = DensityWhenDisappeared;
        }

        bool FogHasDisappearedCompletely()
        {  
            return fogEffect.heightDensity <= DensityWhenDisappeared;
        }

        public override void DisappeareImmediately()
        {
            fogEffect.heightDensity = DensityWhenDisappeared;
        }

        public override void RaisedImmediately()
        {
            fogEffect.heightDensity = HeightDensityWhenRaised; 
        }
    }

}
