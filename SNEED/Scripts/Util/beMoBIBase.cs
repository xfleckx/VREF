using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class beMoBIBase : MonoBehaviour {

    protected AMarkerStream markerStreamInstance;
    private List<AMarkerStream> markerStreams;

    protected void Initialize()
    {
        var availableMarkerStreams = (IEnumerable<AMarkerStream>)GameObject.FindObjectsOfType<AMarkerStream>();

        if (availableMarkerStreams.Any())
        {
            markerStreamInstance = availableMarkerStreams.First();
        }
        else
        {
            Debug.LogWarning("No instance implementing IMarkerStream found! \n creating Debug.Log MarkerStream instance");
            GameObject DebugMarkerStreamHost = new GameObject();
            DebugMarkerStreamHost.AddComponent(typeof(DebugMarkerStream));
            DebugMarkerStreamHost.name = DebugMarkerStream.Instance.StreamName;
            markerStreamInstance = DebugMarkerStream.Instance;
            
            if (markerStreams == null) 
                markerStreams = new List<AMarkerStream>();

            
            markerStreams.Add(markerStreamInstance);
        }
    }

    protected void WriteMarker(string name) 
    {
        markerStreamInstance.Write(name);
    }

    protected void WriteMarker(string name, float customTimeStamp) 
    {
        markerStreamInstance.Write(name, customTimeStamp);
    }

    public void Reset()
    {
        Initialize();
    }

}
 
public interface IMarkerStream
{
    string StreamName { get; } 

    void Write(string name, float customTimeStamp);

    void Write(string name);
}

public abstract class AMarkerStream : Singleton<AMarkerStream>, IMarkerStream
{ 
    public virtual string StreamName
    {
        get { return string.Empty; }
    }

    public abstract void Write(string name, float customTimeStamp);

    public abstract void Write(string name);
}

/// <summary>
/// Example implementation of an marker stream
/// </summary>
public class DebugMarkerStream : AMarkerStream
{
    private const string streamName = "DebugMarkerStream";
    private const string logWithTimeStampPattern = "Marker {0} at {1}";

    public override string StreamName
    {
        get
        {
            return streamName;
        }
    } 

    public override void Write(string name, float customTimeStamp)
    {
        Debug.Log(string.Format(logWithTimeStampPattern, name, customTimeStamp));
    }

    public override void Write(string name)
    {
        Debug.Log(string.Format(logWithTimeStampPattern, name, Time.realtimeSinceStartup));
    }
}