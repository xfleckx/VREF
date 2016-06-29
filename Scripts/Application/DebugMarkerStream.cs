using UnityEngine;
using System.Collections;
using System;

namespace Assets.VREF.Scripts { 
    /// <summary>
    /// Example basic implementation of an marker stream
    /// </summary>
    public class DebugMarkerStream : MonoBehaviour, IMarkerStream
    {
        private const string streamName = "DebugMarkerStream";
        private const string logWithTimeStampPattern = "Marker {0} at {1}";

        public string StreamName
        {
            get
            {
                return streamName;
            }
        }
        
        void Awake()
        {

        }

        public void Write(string name, float customTimeStamp)
        {
            Debug.Log(string.Format(logWithTimeStampPattern, name, customTimeStamp));
        }

        public void Write(string name)
        {
            Debug.Log(string.Format(logWithTimeStampPattern, name, Time.realtimeSinceStartup));
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
}