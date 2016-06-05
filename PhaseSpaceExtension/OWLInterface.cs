using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System;
using Assets.VREF.Interfaces;


namespace Assets.VREF.PhaseSpaceExtensions
{

    public enum OWLUpdateStratgy { FixedUpdate, Update, OnPreRender }

    public delegate void OnPostOwlUpdate();
    public delegate void OnOwlConnected();

    public class OWLInterface : MonoBehaviour
    {
        public OWLUpdateStratgy updateMoment = OWLUpdateStratgy.FixedUpdate;

        public string OWLHost;
        public bool isSlave = false;
        public bool broadcast = false;

        public bool autoConnectOnStart = false;

		public IOWLTracker Tracker = FindObjectofType<IOWLinterface> ();

        private bool connected = false;

        public bool IsConnected { get { return connected; } }

        private Stopwatch stopWatch = new Stopwatch();

        public float OWLUpdateTook = 0f;

        public OnPostOwlUpdate OwlUpdateCallbacks;

        public OnOwlConnected OwlConnectedCallbacks;

        protected string message = String.Empty;

        public bool showDeprecatedOnGUI = false;

        //
        void Awake()
        {
            isSlave = PlayerPrefs.GetInt("owlInSlaveMode", 0) == 1;
        }

        // Use this for initialization
        void Start()
        {

            if (!OWLHost.Equals(string.Empty) && autoConnectOnStart)
            {
                ConnectToOWLInstance();
                connected = Tracker.Connected();
            }
        }

        public void ConnectToOWLInstance()
        {
            if (Tracker.Connect(OWLHost, isSlave, broadcast))
            {

                UnityEngine.Debug.Log(string.Format("OWL connected to {0}", OWLHost), this);

                if (!isSlave)
                {
                    if (OwlConnectedCallbacks != null)
                    {
                        OwlConnectedCallbacks.Invoke();
                    }
                    else
                    {
                        // create default point tracker
                        int n = 128;
                        int[] leds = new int[n];
                        for (int i = 0; i < n; i++)
                            leds[i] = i;
                        Tracker.CreatePointTracker(0, leds);
                    }

                }

                // start streaming
               //Tracker.Start();

            }
            else
            {
                UnityEngine.Debug.LogWarning(string.Format("Connection to OWL Host {0} failed!", OWLHost), this);
            }
        }

        public void DisconnectFromOWLInstance()
        {
            Tracker.Disconnect();
        }

        public bool HasConfigurationAvaiable()
        {
            return false;
        }

        private void PerformOwlUpdate()
        {
            stopWatch.Start();

           // Tracker.Update();

            stopWatch.Stop();

            OWLUpdateTook = stopWatch.ElapsedMilliseconds;

            stopWatch.Reset();

            if (OwlUpdateCallbacks != null)
            {
                OwlUpdateCallbacks.Invoke();
            }
        }

        void FixedUpdate()
        {
            if (Tracker.Connected() && updateMoment == OWLUpdateStratgy.FixedUpdate)
            {
                PerformOwlUpdate();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Tracker.Connected() && updateMoment == OWLUpdateStratgy.Update)
                PerformOwlUpdate();
        }

        void OnPreRender()
        {
            if (Tracker.Connected() && updateMoment == OWLUpdateStratgy.OnPreRender)
                PerformOwlUpdate();
        }

      

        //
        void OnDestroy()
        {

            // save user settings
            PlayerPrefs.SetString("OWLHost", OWLHost);
            PlayerPrefs.SetInt("owlInSlaveMode", Convert.ToInt32(isSlave));

            // disconnect from OWL server
            Tracker.Disconnect();
        }
    }
}