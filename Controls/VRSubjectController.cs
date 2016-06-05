using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Assertions;
using VRStandardAssets.Utils;
using Assets.VREF.Application.FogEffects;
using Assets.VREF.Controls;
using Assets.VREF.Scripts;
using Assets.VREF.Interface.SubjectRepresentation;

using NLog;

namespace Assets.VREF.Controls
{

    public class VRSubjectController : MonoBehaviour, ISubject
    {
        private static NLog.ILogger appLog = LogManager.GetLogger("App");    

        public CharacterController Body;
        public Transform Head;
        public Camera HeadPerspective;
        public Reticle reticle;
        public HUD_DEBUG debug_hud;
        public HUD_Instruction instruction_hud;
        
        public event Action<float> ItemValueRequested;

        [SerializeField]
        public string HeadController;
        [SerializeField]
        public string BodyController;

        private SubjectDescription description;

        public void Start()
        {
            Assert.IsNotNull<Camera>(HeadPerspective);
            //Body = GetComponentInChildren<CharacterController>();

            if (description == null)
            {
                appLog.Info("Subject Description not set! Using default...");
                description = SubjectDescription.GetDefault();
            }

            SetSubjectProperties(description);
        }

        public void SetSubjectProperties(SubjectDescription description)
        {
            this.description = description;
            Body.height = this.description.HeightFromFeetToEyes;
            Body.center = new Vector3(0, this.description.HeightFromFeetToEyes / 2, 0);
            instruction_hud.transform.localPosition = new Vector3(0, this.description.HeightFromFeetToEyes, 0);
        }

        public void EnableSubjectBehaviorControl()
        {
            Change<IHeadMovementController>(HeadController);
            Change<IBodyMovementController>(BodyController);
        }
        
        private void Enable<C>(string ControllerName) where C : IInputController
        {
            var possibleController = GetComponents<C>();

            Func<C, bool> withTheExpectedName = controller => controller.Identifier.Equals(ControllerName);

            if (possibleController.Any(withTheExpectedName))
            {
                possibleController.FirstOrDefault(withTheExpectedName).Enable();
            }
            
        }
        
        public void Change<C>(string controllerName) where C : IInputController
        {
            DisableAll<C>();

            Enable<C>(controllerName);

            var controller = Get<C>(controllerName);

            if (controller == null)
                Debug.Log(string.Format("Expected {0} not found!", controllerName));

            if(controller is IBodyMovementController)
            {
                var bodyController = controller as IBodyMovementController;

                if (bodyController == null)
                    Debug.Log(string.Format("Expected {0} not found as BodyMovementController!", controllerName));

                bodyController.Body = this.Body;
            }

            if(controller is IHeadMovementController)
            {
                var headController = controller as IHeadMovementController;

                if (headController == null)
                    Debug.Log(string.Format("Expected {0} not found as HeadMovementController!", controllerName));

                headController.Head = this.Head;
            }
            
        }
        
        public IInputController Get<C>(string ControllerName) where C : IInputController
        {
            var possibleController = GetComponents<C>();

            IInputController expectedController = null;

            Func<C, bool> withTheExpectedName = controller => controller.Identifier.Equals(ControllerName);

            if (possibleController.Any(withTheExpectedName))
            {
                expectedController = possibleController.FirstOrDefault(withTheExpectedName);
            } 

            return expectedController;
        }

        private void DisableAll<C>() where C : IInputController
        {
            var controller = GetComponents<C>();

            foreach (var c in controller)
            {
                c.Disable();
            }

        }
        
        public void ResetController()
        {
            BodyController = String.Empty; 
            HeadController = String.Empty;

            DisableAll<IInputController>();

            Head.rotation = Quaternion.identity;
            Body.transform.rotation = Quaternion.identity;
        }
        
        #region Options

        public void ToggleRectile()
        {
            if (reticle == null)
                return;

            var state = reticle.ReticleTransform.gameObject.activeSelf;

            reticle.ReticleTransform.gameObject.SetActive(!state);
        }

        public void ToggleFog()
        {
            var fog = GetComponentInChildren<CustomGlobalFog>();

            if (fog == null)
            {
                Debug.Log("No CustomGlobalFog instance found");
                return;
            }

            fog.enabled = !fog.enabled;

        }

        public void RecenterHeading()
        {
            InputTracking.Recenter();
        }

        #endregion
        
        void OnDrawGizmos()
        {
            var bodyCenter = Body.transform.localPosition + new Vector3(0, 1, 0);

            var temp = Gizmos.color;

            Gizmos.color = new Color(0.2f, 0.3f, 0.7f);

            Gizmos.DrawWireSphere(Head.position, 0.2f);

            Gizmos.DrawLine(bodyCenter, Head.position);

            Gizmos.DrawRay(HeadPerspective.transform.position, HeadPerspective.transform.forward);

            Gizmos.DrawRay(bodyCenter, Body.transform.forward * 0.5f);

            Gizmos.DrawCube(bodyCenter, new Vector3(0.1f, 0.4f, 0.1f));
            
            Gizmos.DrawWireCube(Body.transform.localPosition, new Vector3(0.4f, 0.001f, 0.4f));

            Gizmos.color = temp;
        }

        public void Move(Vector3 movementVector)
        {
            Body.Move(movementVector);
        }

        public void SetPosition(Transform target)
        {
            Body.transform.position = target.position;
        }

        public void Rotate(Quaternion resultRotation)
        {
            Body.transform.rotation = resultRotation;
            Head.transform.rotation = resultRotation;
        }

        public void Recalibrate()
        {
            var inputController = GetComponents<IInputCanCalibrate>();

            foreach (var controller in inputController)
            {
                controller.Calibrate();
            }
        }
    }
}