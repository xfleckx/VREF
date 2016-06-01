using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.BeMoBI.Scripts.Controls
{
    public class KeyboardCombined : CombinedController
    {
        private const string X_AXIS_NAME = "KBC_Horizontal";
        private const string Y_AXIS_NAME = "KBC_Vertical";
        
        public override string Identifier
        {
            get
            {
                return "KeyboardCombi";
            }
        }

        void OnEnable()
        {
            UnityEngine.VR.VRSettings.enabled = false;

            targetRotation = Body.transform.rotation;
        }


        public AnimationCurve BodyRotationAccelerationCurve;

        public Vector3 currentForward;

        void Update()
        {
            targetRotation = Body.transform.rotation;

            body_raw_X = Input.GetAxis(X_AXIS_NAME);

            var sign = Math.Sign(body_raw_X);
            
            var abs_body_raw_X = Math.Abs(body_raw_X);
            
            body_raw_Y = Input.GetAxis(Y_AXIS_NAME);

            currentForward = Body.transform.forward;

            desiredMove = Body.transform.forward * BodyAccelerationCurve.Evaluate(body_raw_Y) * Time.deltaTime * MaxWalkingSpeed;

            // Problem here... Acceleration for rotation doesn't work :/ always the same direction

            var evaluated = BodyRotationAccelerationCurve.Evaluate(abs_body_raw_X);

            var bodyRotation = evaluated * sign * Time.deltaTime;

            targetRotation *= Quaternion.Euler(0f, bodyRotation * BodyRotationSpeed, 0f);

            var resultRotation = Quaternion.identity;

            if (RotateBodySmooth)
            {
                resultRotation = Quaternion.Slerp(Body.transform.rotation, targetRotation, SmoothBodyTime * Time.deltaTime);
            }
            else {
                resultRotation = targetRotation;
            }

            subject.Rotate(resultRotation);

            subject.Move(desiredMove);
        }
    }
}
