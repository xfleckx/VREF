using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

using Assets.BeMoBI.Scripts.HWinterfaces;


public class AppRuntimeStatistics : MonoBehaviour {

    public HUD_DEBUG hud;



    #region FPS Counter

    private int frameCount = 0;
    private float fps = 0;
    private float avgFrameTime;
    private float timeSpendToCompleteLastFrame;

    private float timeLeft = 0.5f;
    private float timePassed = 0;
    public float FpsUpdateInterval = 0.5f;

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
            fps = timePassed / frameCount;
            timeLeft = FpsUpdateInterval;
            timePassed = 0;
            frameCount = 0;
            avgFrameTime = lastframeTimes.Average();
            lastframeTimes.Clear();
        }

    }

    #endregion
    
    void Start()
    {

    }

    void Update () {

        EstimateFpsAndAverageFrameTime();

        if (hud != null)
            hud.UpdateFpsAndFTView(fps, avgFrameTime);
    }
    
    
}
