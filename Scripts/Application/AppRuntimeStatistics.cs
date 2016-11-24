using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.VREF.Application
{
    public class AppRuntimeStatistics : MonoBehaviour
    {

        public float FpsUpdateInterval = 0.5f;

        public float FPS = 0;
        public float AvgFrameTime = 0;

        #region FPS Counter

        private int frameCount = 0;
        private float timeSpendToCompleteLastFrame;

        private float timeLeft = 0.5f;
        private float timePassed = 0;

        List<float> lastframeTimes = new List<float>();

        private void EstimateFpsAndAverageFrameTime()
        {
            frameCount += 1;

            float lastFrameTime = Time.deltaTime;

            timeLeft -= lastFrameTime;

            lastframeTimes.Add(lastFrameTime);

            timePassed += Time.timeScale / Time.deltaTime;

            if (timeLeft <= 0f)
            {
                FPS = timePassed / frameCount;
                timeLeft = FpsUpdateInterval;
                timePassed = 0;
                frameCount = 0;
                AvgFrameTime = lastframeTimes.Average();
                lastframeTimes.Clear();
            }

        }

        #endregion

        void Update()
        {

            EstimateFpsAndAverageFrameTime();
        }

    }
}