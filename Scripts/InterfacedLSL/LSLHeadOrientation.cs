using UnityEngine;
using System.Collections;
using Assets.BeMoBI.Scripts.HWinterfaces;

public class LSLHeadOrientation : MonoBehaviour {

    private const string unique_source_id = "0405AF5C24B04416B61684CA9F5D8F0E";

    private IStreamOutlet outlet;
    private IStreamInfo streamInfo;
    private float[] currentSample;

    public string StreamName = "BeMoBI.Unity.HeadOrientation";
    public string StreamType = "Unity.Quaternion";
    public int ChannelCount = 4;

    public Transform head;

    void Start()
    {
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

    void LateUpdate()
    {
        if (outlet == null)
            return;

        var rotation = head.rotation;

        currentSample[0] = rotation.x;
        currentSample[1] = rotation.y;
        currentSample[2] = rotation.z;
        currentSample[3] = rotation.w;

        outlet.push_sample(currentSample, LSLTimeSync.Instance.UpdateTimeStamp);
    }
}
