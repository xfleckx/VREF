using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.BeMoBI.Scripts.Controls;

namespace Assets.Editor.BeMoBI.Paradigms.SearchAndFind
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