using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.VREF.Scripts.Interfaces;
using System;
using System.Collections;

namespace Assets.VREF.Scripts
{
    public abstract class beMoBIBase : MonoBehaviour
    {

        protected IMarkerStream markerStreamInstance;
        private List<IMarkerStream> markerStreams;

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
                    markerStreams = new List<IMarkerStream>();


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

    public abstract class AMarkerStream : Singleton<AMarkerStream>, IMarkerStream
    {
        public virtual string StreamName
        {
            get { return string.Empty; }
        }

        public abstract void Write(string name, float customTimeStamp);

        public abstract void Write(string name);

        public void WriteAtTheEndOfThisFrame(string marker)
        {
            StartCoroutine(WriteAtEndOfFrame(marker));
        }

        private IEnumerator WriteAtEndOfFrame(string marker)
        {
            yield return new WaitForEndOfFrame();

            Write(name);
        }
    }
    
}