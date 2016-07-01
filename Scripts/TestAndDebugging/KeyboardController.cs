using UnityEngine;

namespace Assets.VREF.Scripts.TestAndDebugging
{
    /// <summary>
    /// A very simple Keyboard implementation for controlling the subject.
    /// It's intended to be used for testing during the development of an experiment.
    /// It uses WASD QE for moving the subject like a first person shooter,
    /// and the arrow keys for simulating head movements.
    /// </summary>
    [RequireComponent(typeof(Subject.Subject))]
    public class KeyboardController : MonoBehaviour
    {
        #region constants and axis name definitions

        public const string HEAD_UP_DOWN = "HEAD_UP_DOWN";

        public const string HEAD_TURN = "HEAD_TURN";

        public const string BODY_FORWARD_BACKWARDS = "BODY_FORWARD_BACKWARDS";

        public const string BODY_TURN = "BODY_TURN";

        public const string BODY_STRAFE = "BODY_STRAFE";

        #endregion

        /// <summary>
        /// Degree per second
        /// </summary>
        public float bodyRotationSpeed = 1;
        
        public float movementSpeed = 1;

        public float headRotationSpeed = 1;

        public bool allowHeadMovement = true;
        
        private Subject.Subject subject;
        private CharacterController subjectsCharacterController;

        void Awake()
        {
            subject = GetComponent<Subject.Subject>();
            subjectsCharacterController = GetComponent<CharacterController>();
        }
        
        // Update is called once per frame
        void Update()
        {
            if (allowHeadMovement)
                updateHeadMovement();

            updateBodyMovement();
        }

        /// <summary>
        /// this method is virtual so it can be overriden in a subclass.
        /// </summary>
        protected virtual void updateBodyMovement()
        {
            var raw_Y = Input.GetAxis(BODY_FORWARD_BACKWARDS);

            var raw_X = Input.GetAxis(BODY_TURN);

            var currentRotation = subjectsCharacterController.transform.rotation;

            var rotationAngle = raw_X * bodyRotationSpeed * Time.deltaTime;

            var expectedRotation = currentRotation * Quaternion.Euler(0f, rotationAngle, 0f);

            subjectsCharacterController.transform.rotation = expectedRotation;

            var desiredMove = transform.forward * raw_Y * movementSpeed * Time.deltaTime;

            subjectsCharacterController.transform.TransformDirection(desiredMove);

            subjectsCharacterController.Move(desiredMove);
        }

        /// <summary>
        /// this method is virtual so it can be overriden
        /// </summary>
        protected virtual void updateHeadMovement()
        {
            var raw_Y = Input.GetAxis(HEAD_UP_DOWN);

            var raw_X = Input.GetAxis(HEAD_TURN);

            var rotationTargetHorizontalTarget = raw_Y * headRotationSpeed * Time.deltaTime;

            var rotationTargetVerticalTarget = raw_X * headRotationSpeed * Time.deltaTime;

            var clampedHorizontalRotation = Mathf.Clamp(rotationTargetHorizontalTarget, -90, 90);

            var clampedVerticalRotation = Mathf.Clamp(rotationTargetVerticalTarget, -60, 60);

            var currentRotation = subject.Head.rotation;

            var expectedRotation = Quaternion.Euler(clampedHorizontalRotation, clampedVerticalRotation, 0);

            subject.Head.rotation = currentRotation * expectedRotation;
        }
    }

}