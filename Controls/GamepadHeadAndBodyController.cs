using UnityEngine;
using System.Collections;
using System;
using Assets.VREF.Interfaces;


namespace Assets.VREF.Controls
{
    public class GamepadHeadAndBodyController : CombinedController
    {
        private const string X_AXIS_BODY_NAME = "XBX_Horizontal_Body";
        private const string Y_AXIS_BODY_NAME = "XBX_Vertical_Body";

        private const string X_AXIS_HEAD_NAME = "XBX_Horizontal_Head_X";
        private const string Y_AXIS_HEAD_NAME = "XBX_Vertical_Head_Y";

        public override string Identifier
        {
            get
            {
                return "GamePad";
            }
        }
        
        void Update() {

            head_raw_X = Input.GetAxis(X_AXIS_HEAD_NAME);
            head_raw_Y = Input.GetAxis(Y_AXIS_HEAD_NAME);

            float xRot = head_raw_X * headRotation_X_Sensitivity;
            float yRot = head_raw_Y * headRotation_Y_Sensitivity;

            headTargetRotation *= Quaternion.Euler(-yRot, xRot, 0f);

            if (clampVerticalRotation)
                headTargetRotation = InputUtils.ClampRotationAroundXAxis(headTargetRotation, headRotationClampMinX, headRotationClampMaxX);

            if (clampHorizontalRotation)
                headTargetRotation = InputUtils.ClampRotationAroundYAxis(headTargetRotation, headRotationClampMinY, headRotationClampMaxY);
            
            sourceRotation = Head.rotation;
            sourceLocalRotation = Head.localRotation;

            if (UseSmoothHeadRotation)
            {
                Head.localRotation *= Quaternion.Slerp(sourceLocalRotation, headTargetRotation,
                    HeadRotsmoothTime * Time.deltaTime);
            }
            else
            {
                Head.rotation *= headTargetRotation;
            }

            var euler = Head.rotation.eulerAngles;

            euler.z = 0;

            Head.rotation = Quaternion.Euler(euler);

            targetRotation = Body.transform.rotation;

            body_raw_X = Input.GetAxis(X_AXIS_BODY_NAME); // use as body rotation

            body_raw_Y = Input.GetAxis(Y_AXIS_BODY_NAME); // use as movement to forward / backwards

            Vector3 desiredMove = Body.transform.forward * BodyAccelerationCurve.Evaluate(body_raw_Y) * Time.deltaTime * MaxWalkingSpeed;

            targetRotation *= Quaternion.Euler(0f, body_raw_X * BodyRotationSpeed, 0f);

            if (RotateBodySmooth)
            {
                Body.transform.rotation = Quaternion.Slerp(Body.transform.rotation, targetRotation, SmoothBodyTime * Time.deltaTime);
            }
            else {
                Body.transform.rotation = targetRotation;
            }

            Body.Move(desiredMove);
        }

    }
}