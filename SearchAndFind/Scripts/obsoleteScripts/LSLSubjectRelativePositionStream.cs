using UnityEngine;
using System.Collections;
using Assets.VREF.Interfaces;

public class LSLSubjectRelativePositionStream : MonoBehaviour {
    
    private const string unique_source_id = "F949DAC8CBAB4BF691D0DFE9D8BFD1CB";

    private IStreamOutlet outlet;
    private IStreamInfo streamInfo;
    private float[] currentSample;

    public string StreamName = "BeMoBI.Unity3D.SubjectPositionRelative";
    public string StreamType = "Unity3D.LocalPositionVector";
    public int ChannelCount = 3;

    public Transform Subject;

    [HideInInspector] // is set per trial!
    public beMobileMaze currentMaze;

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
        if (outlet == null || currentMaze == null)
            return;

        var positionInMaze = currentMaze.transform.InverseTransformPoint(Subject.position);

        currentSample[0] = positionInMaze.x;
        currentSample[1] = positionInMaze.y;
        currentSample[2] = positionInMaze.z;

        outlet.push_sample(currentSample, LSLTimeSync.Instance.UpdateTimeStamp);
    }
}
