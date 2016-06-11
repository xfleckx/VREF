using UnityEngine;
using System.Collections;

using Assets.BeMoBI.Scripts.HWinterfaces;


using System;
using System.Linq;
using Assets.BeMoBI.Scripts.PhaseSpaceExtensions;
using Assets.BeMoBI.Paradigms.AbstractParadigm;
using UnityEngine.Assertions;

namespace Assets.BeMoBI.Scripts.Controls
{
    public class PSRigidBodyForwardOnlyController : MonoBehaviour, IBodyMovementController, IInputCanCalibrate
    {
        public Transform Head;

        public VRSubjectController subject;

        NLog.Logger appLog = NLog.LogManager.GetLogger("App");

        string VERTICAL = "FW_Vertical";

        string VERTICAL_WiiMote = "FW_WiiM_Vertical";
         
        public float ForwardSpeed = 1;

		IOWLTracker tracker= FindObjectofType<IOWLinterface> ();

        public int expectedRigidID;

        public int TimeOffsetTilRBcreation = 1;

        public int AvailableRigidBodies
        {
            get { return tracker.NumRigids; }
        }
        /// <summary>
        /// The number of frames to interpolate the rigid body over.  Values over 8 are clamped.
        /// </summary>
        public int Interp = 0;

        /// <summary>
        /// The most recent rigid acquired from Tracker
        /// </summary>
        public PhaseSpace.Rigid lastRigid = new PhaseSpace.Rigid();

        private OVRCameraRig rig;

        protected Vector3 prevPos = new Vector3(0, 0, 0);
        protected Quaternion prevRot = new Quaternion(0, 0, 0, 1);

        // interpolation
        protected int interp_index = 0;
        protected Quaternion[] quaternions = new Quaternion[8];
        protected Vector3[] rotations = new Vector3[8];

        private CharacterController body;
        public CharacterController Body
        {
            get
            {
                return body;
            }

            set
            {
                body = value;
            }
        }

        public string Identifier
        {
            get
            {
                return "PS_RigidBody";
            }
        }

        [Range(-360, 360)]
        public float y_rotation_correction = 0;

        void Awake()
        {
            subject = GetComponent<VRSubjectController>();
            rig = FindObjectOfType<OVRCameraRig>();
            
            Assert.IsNotNull(rig);

            body = subject.GetComponent<CharacterController>();

        }
        
        private void Rig_UpdatedAnchors(OVRCameraRig obj)
        {
            OVRPose pose = rig.centerEyeAnchor.ToOVRPose(true);
            
            pose.orientation = Quaternion.Euler(0, -body.transform.rotation.eulerAngles.y, 0) * pose.Inverse().orientation * pose.orientation;
            
            rig.trackingSpace.FromOVRPose(pose, true);
        }
        
        public void Disable()
        {
            this.enabled = false;


            rig.UpdatedAnchors -= Rig_UpdatedAnchors;
        }

        private void CloseTrackerConnection()
        {
            if (tracker != null && tracker.Connected())
            {
                tracker.Disconnect();
            } 
        }

        public void Enable()
        {
            this.enabled = true;
            TryConnectToTracker();

            rig.UpdatedAnchors += Rig_UpdatedAnchors;
        }

        private void TryConnectToTracker()
        {
            tracker = OWLTracker.Instance;

            var availableServers = tracker.GetServers();

            if (tracker == null)
            {
                Debug.Log("No tracker found!");
                this.enabled = false;
                return;
            }

            var firstServer = availableServers.FirstOrDefault();

            if (firstServer != null)
            {
                var result = tracker.Connect(firstServer.address, false, false);

                if (result == false)
                {
                    Debug.Log("Establishing connection failed...");
                }
            }
            this.enabled = true;

            StartCoroutine(WaitSecondsBeforeCreateRigidbody());
        }

        private void TryCreateRigidBodyTracker()
        {
            Debug.Log("Try create rigid body");

            var fileProvider = FindObjectOfType<ParadigmController>();

            var fileInfo = fileProvider.GetRigidBodyDefinition();

            if (fileInfo == null)
            {
                appLog.Fatal("An expected rigidbody file for configuring the phasespace server could not be found!");

                Debug.Log("Missing RigidBodyFile!");
                this.enabled = false;

            }

            try
            {

                if (tracker != null)
                    tracker.CreateRigidTracker(expectedRigidID, fileInfo.FullName);
            }
            catch (OWLException owle)
            {
                appLog.Fatal(owle.Message);

                this.enabled = false;
            }

        }

        IEnumerator WaitSecondsBeforeCreateRigidbody()
        {
            yield return new WaitForSeconds(TimeOffsetTilRBcreation);
            TryCreateRigidBodyTracker();
            tracker.StartStreaming();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateFromTracker();

            var y_rotation = prevRot.eulerAngles.y + y_rotation_correction;

            Body.transform.rotation = Quaternion.Euler(0, y_rotation, 0);
            // using the rotation of the rigid body at this point is not valid... causes strange woobling behavior
            // Body.transform.rotation = Quaternion.Euler(prevRot.eulerAngles.x, y_rotation, prevRot.eulerAngles.z);

            // forward only - only take positive value ignoring left or right button!
            var forwardMovement = Math.Abs(Input.GetAxis(VERTICAL_WiiMote));

            var movementVector = Body.transform.forward * forwardMovement * ForwardSpeed * Time.deltaTime;

            movementVector = new Vector3(movementVector.x, 0, movementVector.z);

            subject.Move(movementVector);

        }

        protected void UpdateFromTracker()
        {
            // get data from tracker
            lastRigid = tracker.GetRigid(expectedRigidID);

            if (lastRigid == null || lastRigid.cond < 0)
                return;

            prevRot = lastRigid.rotation;
            prevPos = lastRigid.position;

            // interpolate if required
            if (Interp > 1)
            {
                quaternions[interp_index] = prevRot;
                rotations[interp_index] = prevPos;

                int n = Math.Min(Interp, quaternions.Length);
                float f = 1.0f / n;
                Quaternion q = quaternions[interp_index];
                Vector3 r = rotations[interp_index];
                for (int i = 1; i < n; i++)
                {
                    int index = ((interp_index - i) + quaternions.Length) % quaternions.Length;
                    q = Quaternion.Slerp(q, quaternions[index], f);
                    r += rotations[index];
                }
                r *= f;

                prevRot = q;
                prevPos = r;

                interp_index = (interp_index + 1) % quaternions.Length;
            }
        }

        public void ResetForward()
        { 
        }

        public void OnDestroy()
        {
            CloseTrackerConnection();
        }

        public void OnDisable()
        {
            CloseTrackerConnection();
        }

        public void Calibrate()
        {
            var delta = Quaternion.FromToRotation(subject.Head.forward, Body.transform.forward);

            y_rotation_correction = delta.eulerAngles.y;
        }
    }
}