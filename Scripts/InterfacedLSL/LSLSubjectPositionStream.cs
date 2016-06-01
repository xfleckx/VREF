using UnityEngine;
using System.Collections;
using Assets.BeMoBI.Scripts.HWinterfaces;

public class LSLSubjectPositionStream : MonoBehaviour {

    private const string unique_source_id = "B286CF7591134C7B8BD54EBC7B8DC840";

    private IStreamOutlet outlet;
    private IStreamInfo streamInfo;
    private float[] currentSample;

    public string StreamName = "BeMoBI.Unity3D.SubjectPosition";
    public string StreamType = "Unity3D.GlobalPositionVector";
    public int ChannelCount = 3;

    public Transform Subject;

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

    public void LateUpdate()
    {
        if (outlet == null)
            return;

        var position = Subject.position;

        currentSample[0] = position.x;
        currentSample[1] = position.y;
        currentSample[2] = position.z;
        outlet.push_sample(currentSample, LSLTimeSync.Instance.UpdateTimeStamp);
    }
}
