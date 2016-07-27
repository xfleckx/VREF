using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.VREF.EditorExtensions.Setup;
using Assets.VREF.Scripts.TestAndDebugging;

namespace Assets.VREF.EditorExtensions
{
    /// <summary>
    /// This class is intented to setup some pre-conditions for the VREF framework components.
    /// </summary>
    [InitializeOnLoad]
    public class AutoSetup
    {
        static AutoSetup()
        {
            SetupDedicatedControllsForTheTestSubject();

            ConfigureProjectSettings();
        }


        public static void SetupDedicatedControllsForTheTestSubject()
        {
            var axisToAdd = new List<InputAxis>();

            if (!InputSetup.AxisDefined(KeyboardController.BODY_FORWARD_BACKWARDS))
            {
                axisToAdd.Add(new InputAxis()
                {
                    name = KeyboardController.BODY_FORWARD_BACKWARDS,
                    type = AxisType.KeyOrMouseButton,
                    positiveButton = "w",
                    negativeButton = "s",
                    gravity = 10,
                    dead = 0.001f,
                    sensitivity = 3
                });
            }

            if (!InputSetup.AxisDefined(KeyboardController.BODY_TURN))
            {
                axisToAdd.Add(new InputAxis()
                {
                    name = KeyboardController.BODY_TURN,
                    type = AxisType.KeyOrMouseButton,
                    positiveButton = "d",
                    negativeButton = "a",
                    gravity = 10,
                    dead = 0.001f,
                    sensitivity = 3
                });

            }

            if (!InputSetup.AxisDefined(KeyboardController.BODY_STRAFE))
            {
                axisToAdd.Add(new InputAxis()
                {
                    name = KeyboardController.BODY_STRAFE,
                    type = AxisType.KeyOrMouseButton,
                    positiveButton = "q",
                    negativeButton = "e",
                    gravity = 10,
                    dead = 0.001f,
                    sensitivity = 3,
                });
            }


            if (!InputSetup.AxisDefined(KeyboardController.HEAD_UP_DOWN))
            {
                axisToAdd.Add(new InputAxis()
                {
                    name = KeyboardController.HEAD_UP_DOWN,
                    type = AxisType.KeyOrMouseButton,
                    positiveButton = "up",
                    negativeButton = "down",
                    gravity = 100,
                    invert = true,
                    dead = 0.001f,
                    sensitivity = 3
                });
            }

            if (!InputSetup.AxisDefined(KeyboardController.HEAD_TURN))
            {
                axisToAdd.Add(new InputAxis()
                {
                    name = KeyboardController.HEAD_TURN,
                    type = AxisType.KeyOrMouseButton,
                    positiveButton = "right",
                    negativeButton = "left",
                    gravity = 100,
                    dead = 0.001f,
                    sensitivity = 3
                });
            }

            foreach (var axis in axisToAdd)
            {
                InputSetup.AddAxis(axis);
            }
        }

        private static void ConfigureProjectSettings()
        {
            if(PlayerSettings.apiCompatibilityLevel != ApiCompatibilityLevel.NET_2_0) { 

                    Debug.Log("Set api settings to full .net 2.0 compatibility!");
                    PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
                
            }

            if(EditorSettings.serializationMode != SerializationMode.ForceText) { 
            
                if(EditorUtility.DisplayDialog("VREF: Use Text serialization", "If you intend to use git or svn you might like that!", "Ok (recommended)", "No"))
                {
                    Debug.Log("Force assets text serialization - necessary for version control.");
                    EditorSettings.serializationMode = SerializationMode.ForceText;
                }
            }
            
            if(EditorSettings.externalVersionControl != Expected_External_Source_Control) {

                if (EditorUtility.DisplayDialog("VREF: Use visible meta files", "If you intend to use git or svn you might like that!", "Ok (recommended)", "No - (Warning)"))
                {
                    Debug.Log("Make meta files visible.");
                    EditorSettings.externalVersionControl = Expected_External_Source_Control;
                }
            }

        }

        const string Expected_External_Source_Control = "Visible Meta Files";
    }
}