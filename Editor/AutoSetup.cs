using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.VREF.Editor.Setup;
using Assets.VREF.Scripts.TestAndDebugging;
using System;

namespace Assets.VREF.Editor
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
            Debug.Log("Set api settings to full .net 2.0 compatibility!");
            PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
            Debug.Log("Force assets text serialization - necessary for version control.");
            EditorSettings.serializationMode = SerializationMode.ForceText;
            Debug.Log("Make meta files visible.");
            EditorSettings.externalVersionControl = "Visible Meta Files";
            
        }
    }
}