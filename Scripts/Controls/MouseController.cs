using UnityEngine;
using System.Collections;
using System;

namespace Assets.BeMoBI.Scripts.Controls
{
    [RequireComponent(typeof(VRSubjectController))]
    public class MouseController : MonoBehaviour, ICombinedControl
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


        private Quaternion m_CharacterTargetRot = Quaternion.identity;
        private Quaternion m_CameraTargetRot = Quaternion.identity;

        public float X_Sensitivity = 2f;
        public float Y_Sensitivity = 2f;

        public bool clampVerticalRotation = true;
        public bool clampHorizontalRotation = true;

        public float MinimumX = -90F;
        public float MaximumX = 90F;

        public bool smooth;
        public float smoothTime = 5f;

        public string Identifier
        {
            get
            {
                return "Mouse Head Controller";
            }
        }
        

        // Use this for initialization
        void Start()
        {
            var subject = GetComponent<VRSubjectController>();
            body = subject.Body;
            var head = subject.Head;
            m_CharacterTargetRot = body.transform.rotation;
           // m_CameraTargetRot = head.transform.localRotation;
        }

        public void ApplyMovement(Transform head)
        {
            float yRot = Input.GetAxis("Mouse X") * X_Sensitivity;
            float xRot = Input.GetAxis("Mouse Y") * Y_Sensitivity;

            //m_CharacterTargetRot = body.transform.rotation;
            //m_CameraTargetRot = head.transform.localRotation;

            m_CharacterTargetRot *= Quaternion.Euler(0, yRot, 0);

            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = InputUtils.ClampRotationAroundXAxis(m_CameraTargetRot, -90, 90);

            if (clampHorizontalRotation)
                m_CharacterTargetRot = InputUtils.ClampRotationAroundYAxis(m_CharacterTargetRot, -90, 90);


            if (smooth)
            {

                head.localRotation *= Quaternion.Slerp(head.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);

                body.transform.rotation = Quaternion.Slerp(body.transform.rotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                head.localRotation *= m_CameraTargetRot;
                body.transform.rotation = m_CharacterTargetRot;
            }
        }

        public void ApplyMovement(CharacterController controller)
        {

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