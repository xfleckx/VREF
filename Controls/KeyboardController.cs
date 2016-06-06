using UnityEngine;
using System.Collections;
using System;
using Assets.VREF.Interfaces;

namespace Assets.VREF.Controls
{ 

    [RequireComponent(typeof(VRSubjectController))]
    public class KeyboardController : MonoBehaviour, IBodyMovementController
    {
        private const string X_AXIS_NAME = "KBC_Horizontal";
        private const string Y_AXIS_NAME = "KBC_Vertical";

        private Transform heading;

        public bool useStrafing = false;

        public AnimationCurve BodyRotationAccelerationCurve;

        public bool rotateSmooth = true;

        public float SmoothBodyTime = 2f;


        public float ForwardSpeed = 10f;

        void Start()
        {
            var subject = GetComponent<VRSubjectController>();
            heading = subject.Head;
        }

        public string Identifier
        {
            get
            {
                return "Keyboard";
            }
        }
        
        

        [SerializeField]
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
        
        public float speed = 2.0f;

        private Quaternion expectedRotation;

        public void Update()
        {
            var rawX = Input.GetAxis(X_AXIS_NAME);

            var rawY = Input.GetAxis(Y_AXIS_NAME);

            var signX = Math.Sign(rawX);

            var abs_body_raw_X = Math.Abs(rawX);
            
            // set to the last state
            expectedRotation = Body.transform.rotation;

            Vector3 desiredMove = Vector3.zero;

            if (useStrafing)
                desiredMove = transform.forward * rawX + transform.right;
            else
                desiredMove = transform.forward * rawY * ForwardSpeed * Time.deltaTime;
            
            var evaluated = BodyRotationAccelerationCurve.Evaluate(abs_body_raw_X);

            var rotationDirection = evaluated * signX;

            expectedRotation *= Quaternion.Euler(0f, rotationDirection * speed, 0f);

            if (rotateSmooth)
            {
                Body.transform.rotation = Quaternion.Slerp(Body.transform.rotation, expectedRotation, SmoothBodyTime * Time.deltaTime);
            }
            else
            {
                Body.transform.rotation = expectedRotation;
            }

            heading.transform.TransformDirection(desiredMove);

            Body.Move(desiredMove);
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}