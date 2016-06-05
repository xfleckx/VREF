using UnityEngine;
using System.Collections;

namespace Assets.VREF.Controls
{
    public class InputUtils : MonoBehaviour {

        public static Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, min, max);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public static Quaternion ClampRotationAroundYAxis(Quaternion q, float min, float max)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

            angleY = Mathf.Clamp(angleY, min, max);

            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

            return q;
        }
    }
}