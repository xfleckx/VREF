using UnityEngine;
using System.Collections;
using Assets.VREF.Interfaces;
using System;


namespace Assets.VREF.Controls
{
    /// <summary>
    /// Custom Control Implementation for Rotating the body with Joystick x achses and movement with its y axes 
    /// </summary>
    public class JoystickControl : MonoBehaviour, IBodyMovementController
    {
        private const string X_AXIS_NAME = "Horizontal";
        private const string Y_AXIS_NAME = "Vertical";

        [Range(0.1f, 2)]
        public float MaxWalkingSpeed = 1.4f;

        public AnimationCurve accelerationCurve;

        [Range(10, 50)]
        public float RotationSpeed = 40f;

        public bool RotateSmooth = true;
        public float SmoothTime = 1;

        private Quaternion targetRotation;

        public string Identifier { get { return "Joystick"; } }

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
        
        [HideInInspector]
        public float body_raw_X = 0f;
        [HideInInspector]
        public float body_raw_Y = 0f;

        void Update() { 

            targetRotation = body.transform.rotation;

            body_raw_X = Input.GetAxis(X_AXIS_NAME); // use as body rotation

            body_raw_Y = Input.GetAxis(Y_AXIS_NAME); // use as movement to forward / backwards

            Vector3 desiredMove = transform.forward * accelerationCurve.Evaluate(body_raw_Y) * Time.deltaTime * MaxWalkingSpeed;

            targetRotation *= Quaternion.Euler(0f, body_raw_X * RotationSpeed, 0f);

            if (RotateSmooth)
            {
                body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation,
                    SmoothTime * Time.deltaTime);
            }
            else {
                body.transform.rotation = targetRotation;
            }

            body.Move(desiredMove);
        }

        public void Enable()
        {
            this.enabled = true;
        }

        public void Disable()
        {
            this.enabled = false;
        }
    }
}