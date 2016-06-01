using UnityEngine;
using System.Collections;
using Assets.BeMoBI.Scripts.HWinterfaces;


public class LSLEulerBodyOrientation : MonoBehaviour {

    private const string unique_source_id = "FE420F14A8F3402FA76099168521FDE5";

    private IStreamOutlet outlet;
    private IStreamInfo streamInfo;
    private float[] currentSample;

    public string StreamName = "BeMoBI.Unity3D.BodyOrientation";
    public string StreamType = "Unity3D.Euler";
    public int ChannelCount = 3;

    public Transform body;

    void Start()
    {
        currentSample = new float[ChannelCount];

        streamInfo = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount, Time.fixedDeltaTime, liblsl.channel_format_t.cf_float32, unique_source_id);

        outlet = new liblsl.StreamOutlet(streamInfo);
    }

    public void LateUpdate()
    {
        if (outlet == null)
            return;

        var rotation = body.rotation.eulerAngles;

        currentSample[0] = rotation.x;
        currentSample[1] = rotation.y;
        currentSample[2] = rotation.z; 
        outlet.push_sample(currentSample, LSLTimeSync.Instance.UpdateTimeStamp);
    }
}
