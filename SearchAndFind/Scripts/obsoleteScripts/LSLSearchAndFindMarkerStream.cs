using UnityEngine;
using System.Collections;
using NLog;
using NLogger = NLog.Logger;
using Assets.VREF.Scripts.Interfaces;



public class LSLSearchAndFindMarkerStream : MonoBehaviour {

	NLogger markerLog = LogManager.GetLogger("MarkerLog");

	private const string unique_source_id = "D3F83BB699EB49AB94A9FA44B88882AB";
	
	public string lslStreamName = "Unity_Paradigma";
	public string lslStreamType = "markers"; // be aware that EEGLab expects 'markers' as name for the marker stream...

	public bool LogAlsoToFile = true;

	private IStreamInfo lslStreamInfo;
	private IStreamOutlet lslOutlet;
	private int lslChannelCount = 1;
	private double nominalRate = 0;
	//private const LSL.liblsl.channel_format_t lslChannelFormat = LSL.liblsl.channel_format_t.cf_string;

	private string[] sample; 

	// Use this for initialization
	void Start () {
		sample = new string[lslChannelCount];

		lslStreamInfo = new IStreamInfo(
				lslStreamName,
				lslStreamType,
				lslChannelCount,
				nominalRate,
				lslChannelFormat,
				unique_source_id);

		lslOutlet = new LSL.liblsl.StreamOutlet(lslStreamInfo);
	}
	  
	public void Write(string marker, double customTimeStamp)
	{
		sample[0] = marker;

		lslOutlet.push_sample(sample, customTimeStamp);

		if (LogAlsoToFile)
			markerLog.Info(string.Format("{0}\t{1}", customTimeStamp, marker));
	}

	public void Write(string marker)
	{
		sample[0] = marker;

        var timestamp = LSL.liblsl.local_clock();

		lslOutlet.push_sample(sample, timestamp);

		if (LogAlsoToFile)
			markerLog.Info(string.Format("{0}\t{1}", timestamp, marker));
	}


    #region Write Marker at the end of a frame

    private string pendingMarker;

    IEnumerator WriteAfterPostPresent()
    {
        yield return new WaitForEndOfFrame();

        Write(pendingMarker);

        yield return null;
    }

    public void WriteAtTheEndOfThisFrame(string marker)
    {
        pendingMarker = marker;
        StartCoroutine(WriteAfterPostPresent());
    }

    #endregion
}
