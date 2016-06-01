using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.BeMoBI.Scripts.Controls
{
    public class CombinedController : MonoBehaviour, ICombinedControl
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

        protected VRSubjectController subject;
          
        public virtual string Identifier
        {
            get
            {
                return "CombinedController";
            }
        }
        
        void Awake()
        {
            subject = GetComponentInParent<VRSubjectController>();
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }

        #region configuration

        #region head

        public float headRotation_X_Sensitivity = 2f;
        public float headRotation_Y_Sensitivity = 2f;

        public bool clampVerticalRotation = true;
        public bool clampHorizontalRotation = true;

        public float headRotationClampMinX = -90F;
        public float headRotationClampMaxX = 90F;

        public float headRotationClampMinY = -90F;
        public float headRotationClampMaxY = 90F;

        public bool UseSmoothHeadRotation;
        public float HeadRotsmoothTime = 2f;

        #endregion

        #region body 

        public AnimationCurve BodyAccelerationCurve;

        [Range(1, 100)]
        public float BodyRotationSpeed = 1f;

        public bool RotateBodySmooth = true;
        public float SmoothBodyTime = 1;

        [Range(0.1f, 2)]
        public float MaxWalkingSpeed = 1.4f;

        #endregion

        #endregion

        #region state 

        [HideInInspector]
        public float body_raw_X = 0f;
        [HideInInspector]
        public float body_raw_Y = 0f;

        [HideInInspector]
        public float head_raw_X = 0f;
        [HideInInspector]
        public float head_raw_Y = 0f;

        [HideInInspector]
        public Vector3 desiredMove;

        [HideInInspector]
        public Quaternion targetRotation;

        [HideInInspector]
        public Quaternion headTargetRotation = Quaternion.identity;

        [HideInInspector]
        public Quaternion sourceLocalRotation = Quaternion.identity;

        [HideInInspector]
        public Quaternion sourceRotation = Quaternion.identity;

        #endregion

    }
}
