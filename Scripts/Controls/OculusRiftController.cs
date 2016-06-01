using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using UnityEngine.VR;

namespace Assets.BeMoBI.Scripts.Controls
{
    public class OculusRiftController : MonoBehaviour, IHeadMovementController, IInputCanCalibrate
    {
        [SerializeField]
        private Transform head;
        public Transform Head
        {
            get
            {
                return head;
            }

            set
            {
                head = value;
            }
        }

        public OVRManager ovrManager;

        public OVRDisplay ovrDisplay;


        public OVRCameraRig ovrRig;
        
        public string Identifier
        {
            get
            {
               return "Oculus Headset";
            }
        }

        private bool monoscopicRendering = false;

        private float originalIpd = 0;

        public float currentIPD = 0;

        public void Recenter()
        {
            InputTracking.Recenter();
        }

        public void SetIPDValue(float newIpdValue)
        {
            currentIPD = newIpdValue;

        }

        public bool UseMonoscopigRendering {

            get {
                return monoscopicRendering;
            }
            set {

                if (value != monoscopicRendering) {
                    monoscopicRendering = value;
                    OnMonoscopicRenderingChanged();
                }
            }
        }
        
        public float Latency
        {
            get
            {
                return ovrDisplay.latency.postPresent;
            }
        }

        public void ChangeIPDValue(float value)
        {
            originalIpd = OVRPlugin.ipd;

            OVRPlugin.ipd = value;

            currentIPD = OVRPlugin.ipd;
        }

        public void RestoreOriginalIpd()
        {
            OVRPlugin.ipd = originalIpd;
            currentIPD = OVRPlugin.ipd;
        }

        public void RequestConfigValues()
        {
            currentIPD = OVRPlugin.ipd;
        }

        private void OnMonoscopicRenderingChanged()
        {
            ovrManager.monoscopic = monoscopicRendering;
        }

        void Awake()
        {
            ovrManager = OVRManager.instance;
            ovrDisplay = OVRManager.display;
            ovrRig = FindObjectOfType<OVRCameraRig>();
        }

        void Start()
        {
        }
        
        void OnEnable()
        {
            StartCoroutine(LookUpRift());
        }
         
        IEnumerator LookUpRift()
        {
            yield return new WaitWhile(() => !OVRPlugin.hmdPresent);

            VRSettings.enabled = true;

            yield return new WaitForSeconds(0.1f);
                
            ovrRig.enabled = true;

            ovrManager.enabled = true;
            
            yield return null;
        }

        void OnDisable()
        {
            if(ovrRig != null)
                ovrRig.enabled = false;

            if(ovrRig != null)
                ovrManager.enabled = false;

            VRSettings.enabled = false;
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }

        public void Calibrate()
        {
            Recenter();
        }
    }
}
