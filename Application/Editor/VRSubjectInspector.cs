using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

using Assets.VREF.Controls;
using Assets.VREF..Utils;


namespace Assets.VREF.Applications.Editor
{
    [CustomEditor(typeof(VRSubjectController))]
    public class VRSubjectInspector : UnityEditor.Editor
    {
        VRSubjectController instance;

        public override void OnInspectorGUI()
        {
            instance = target as VRSubjectController;

            base.OnInspectorGUI();

            GUILayout.BeginVertical();


            if (GUILayout.Button("Toggle Rectile"))
            {
                instance.ToggleRectile();
            }

            if (GUILayout.Button("Toogle Fog"))
            {
                instance.ToggleFog();
            }

            EditorGUILayout.Space();

            var availableBodyController = instance.GetComponents<IBodyMovementController>().Where(c => !(c is CombinedController));
            var availableHeadController = instance.GetComponents<IHeadMovementController>().Where(c => !(c is CombinedController));
            var availableCombinedController = instance.GetComponents<CombinedController>();

            EditorGUILayout.LabelField("Available Combi Controller");

            if (!availableCombinedController.Any())
                EditorGUILayout.HelpBox("No Combined Controller Implementations found! \n Attache them to this GameObject!", MessageType.Info);

            foreach (var combiController in availableCombinedController)
            {
                var nameOfController = combiController.Identifier;

                if (GUILayout.Button(nameOfController))
                {
                    instance.HeadController = nameOfController;
                    instance.BodyController = nameOfController;
                    instance.Change<CombinedController>(nameOfController);
                }
            }

            EditorGUILayout.LabelField("Available Head Controller");

            if (!availableHeadController.Any())
                EditorGUILayout.HelpBox("No Head Controller Implementations found! \n Attache them to this GameObject!", MessageType.Info);

            foreach (var headController in availableHeadController)
            {
                var nameOfController = headController.Identifier;

                if (GUILayout.Button(nameOfController))
                {
                    instance.HeadController = nameOfController;
                    instance.Change<IHeadMovementController>(nameOfController);
                }
            }

            EditorGUILayout.LabelField("Available Body Controller");

            if (!availableBodyController.Any())
                EditorGUILayout.HelpBox("No Body Controller Implementations found! \n Attache them to this GameObject!", MessageType.Info);

            foreach (var bodyController in availableBodyController)
            {
                var nameOfController = bodyController.Identifier;

                if (GUILayout.Button(nameOfController))
                {
                    instance.BodyController = nameOfController;
                    instance.Change<IBodyMovementController>(nameOfController);
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset Controller"))
            {
                instance.ResetController();
            }

            GUILayout.EndVertical();

        }
    }
}
