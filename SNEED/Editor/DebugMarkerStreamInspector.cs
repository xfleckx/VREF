using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DebugMarkerStream))]
public class DebugMarkerStreamInspector : Editor {

    DebugMarkerStream targetMarkerStream;

    public void OnEnable()
    {
        targetMarkerStream = (DebugMarkerStream)target;

    }

    public override void OnInspectorGUI()
    { 
        GUILayout.BeginVertical();
        GUILayout.Label(targetMarkerStream.StreamName);  
        GUILayout.EndVertical();
    }
}
 