using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

using Assets.BeMoBI.Scripts.HWinterfaces;


public class AppRuntimeStatistics : MonoBehaviour {

    public HUD_DEBUG hud;

    public bool publishToLSL = true;

    #region lsl
    
    private const string unique_source_id = "E493C423896E4783A004E93AA3D81051";

    private IStreamOutlet outlet;
    private IStreamInfo streamInfo;
    private float[] currentSample;

    public string StreamName = "BeMoBI.Unity3D.AppStatistics";
    public string StreamType = "Unity3D.FPS.FT";
    public int ChannelCount = 2;

    #endregion

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
        if (!publishToLSL)
            return;

        currentSample = new float[ChannelCount];

        streamInfo = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount, Time.fixedDeltaTime, liblsl.channel_format_t.cf_float32, unique_source_id);

        try
        {
            outlet = new liblsl.StreamOutlet(streamInfo);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void Update () {

        EstimateFpsAndAverageFrameTime();

        if (hud != null)
            hud.UpdateFpsAndFTView(fps, avgFrameTime);
    }
    
    public void LateUpdate()
    {
        if (outlet == null)
            return;
        
        currentSample[0] = fps;
        currentSample[1] = timeSpendToCompleteLastFrame;
        outlet.push_sample(currentSample, LSLTimeSync.Instance.UpdateTimeStamp);
    }
    
}
