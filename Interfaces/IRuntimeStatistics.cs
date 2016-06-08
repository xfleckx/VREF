using System;
using UnityEngine;
using Assets.VREF.Interfaces;
using Assets.VREF.Application.HUD.HUD_Debug;

namespace Assets.VREF.Application
{
    public interface IRuntimeStatistics 
    {
        void EstimateFpsAndAverageFrameTime()
        { 
            get;
        }
        void DisplayStatistics();

    }

    public class Runtimestatistics : MonoBehaviour, IRuntimeStatistics
	{
        private HUD_Debug hud; 

        public void EstimateFpsAndAverageFrameTime()
        {
            frameCount += 1;

            float lastFrameTime = Time.deltaTime;

            timeLeft -= lastFrameTime;

            lastframeTimes.Add(lastFrameTime);

            timePassed += Time.timeScale / Time.deltaTime;

            if (timeLeft <= 0f)
            {
                fps = timePassed / frameCount;
                timeLeft = FpsUpdateInterval;
                timePassed = 0;
                frameCount = 0;
                avgFrameTime = lastframeTimes.Average();
                lastframeTimes.Clear();
            }

        }

        public DisplayStatistics()
        {
            EstimateFpsAndAverageFrameTime()
             
            if (hud != null)
                hud.UpdateFpsAndFTView(fps, avgFrameTime);
        }

}    

