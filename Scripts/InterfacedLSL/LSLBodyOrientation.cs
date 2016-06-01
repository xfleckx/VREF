using UnityEngine;
using System.Collections;
using Assets.BeMoBI.Scripts.HWinterfaces;

public class LSLBodyOrientation : MonoBehaviour {

    private const string unique_source_id = "63CE5B03731944F6AC30DBB04B451A94";

    private IStreamOutlet outlet;
    private IStreamInfo streamInfo;
    private float[] currentSample;

    public string StreamName = "BeMoBI.Unity.BodyOrientation";
    public string StreamType = "Unity.Quarternion";
    public int ChannelCount = 4;

    public Transform body;

    void Start()
    {
        currentSample = new float[ChannelCount];

        streamInfo = new liblsl.StreamInfo(StreamName, StreamType, ChannelCount, Time.fixedDeltaTime, liblsl.channel_format_t.cf_float32, unique_source_id);

        outlet = new liblsl.StreamOutlet(streamInfo);
    }

    void LateUpdate()
    {
        if (outlet == null)
            return;

        var rotation = body.rotation;

        currentSample[0] = rotation.x;
        currentSample[1] = rotation.y;
        currentSample[2] = rotation.z;
        currentSample[3] = rotation.w;

        outlet.push_sample(currentSample, LSLTimeSync.Instance.UpdateTimeStamp);
    }
}
