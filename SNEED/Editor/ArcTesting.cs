using UnityEngine;
using UnityEditor;
using System.Collections;

public class ArcTesting : MonoBehaviour {

    public Transform Last;
    public Transform Current;
    public Transform Next;

    public Vector3 LastPosition;
    public Vector3 CurrentPosition;
    public Vector3 NextPosition;

    public float Angle= 0f;
    public float signedAngle = 0f;
    public float AbsoluteAngle = 0f;

    void OnDrawGizmos()
    {
        if (!(Last && Current && Next))
            return;

        LastPosition = Last.localPosition;
        CurrentPosition = Current.localPosition;
        NextPosition = Next.localPosition;

        var gizmos_temp = Gizmos.matrix;
        var handles_temp = Handles.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

        Handles.matrix = Gizmos.matrix;

        Gizmos.DrawLine(Vector3.zero, Last.localPosition);
        Gizmos.DrawLine(Vector3.zero, Next.localPosition);

        var a = LastPosition - CurrentPosition;
        var b = CurrentPosition - NextPosition;
    
        Angle = Vector3.Angle(a,b);

        signedAngle = a.SignedAngle(b, Vector3.up);

        AbsoluteAngle = (signedAngle + 180) % 360;

        Handles.DrawSolidArc(Vector3.zero, Vector3.up, LastPosition, signedAngle, 0.5f);

        Handles.Label(Vector3.zero + new Vector3(0,0.1f,0), signedAngle.ToString());

        Gizmos.matrix = gizmos_temp;
        Handles.matrix = handles_temp;
    }
}
