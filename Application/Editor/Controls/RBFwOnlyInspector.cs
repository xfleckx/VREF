using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.VREF.Controls;

namespace Assets.VREF.Application.Editor
{

    [CustomEditor(typeof(PSRigidBodyForwardOnlyController))]
    public class RBFwOnlyInspector : UnityEditor.Editor {

        PSRigidBodyForwardOnlyController instance;

        public override void OnInspectorGUI()
        {
            instance = target as PSRigidBodyForwardOnlyController;

            base.OnInspectorGUI();

            if(GUILayout.Button("Set Offset"))
            {
                instance.ResetForward();
            }
        }

    }
}