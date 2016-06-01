using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class Extensions
{
    public static void ApplyTarget(this Transform origin, Transform targetTransform)
    {
        origin.parent = targetTransform.parent;

        origin.localPosition = targetTransform.localPosition;

        origin.localRotation = targetTransform.localRotation;

        origin.transform.localScale = targetTransform.localScale;
    }

    public static string AsPartFileName(this Vector3 v){
       
        var fileNameCompatibleString = string.Format("{0}x{1}x{2}", v.x, v.y, v.z);

        return fileNameCompatibleString.Replace('.', '_');
    }

    public static string AsPartFileName(this Vector2 v)
    {
        var fileNameCompatibleString = string.Format("{0}x{1}", v.x, v.y);

        return fileNameCompatibleString.Replace('.', '_');
    }

    public static List<GameObject> AllChildren(this Transform transform)
    {
        var childCount = transform.childCount;

        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            result.Add(child);
        }

        return result;
    }
    
    public static float SignedAngle(this Vector3 a, Vector3 b, Vector3 n)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        //float angle360 =  (signed_angle + 180) % 360;

        return signed_angle;
    }
    
}

