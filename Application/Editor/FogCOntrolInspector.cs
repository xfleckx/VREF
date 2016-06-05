using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Assets.VREF.Application.FogEffects;

namespace Assets.VREF.Application.Editor
{
    [CustomEditor(typeof(FogControl))]
    public class FogCOntrolInspector : UnityEditor.Editor 
    {
        public override void OnInspectorGUI()
        {
            var instance = target as FogControl;

            base.OnInspectorGUI();

            if (GUILayout.Button("Raise"))
            {
                instance.RaiseFog();
            }

            if (GUILayout.Button("Let disappear"))
            {
                instance.LetFogDisappeare();
            }

            if(instance.fogEffect != null)
            {

            EditorGUILayout.FloatField("Current Height = ", instance.fogEffect.height);

            EditorGUILayout.FloatField("Current Desity = ", instance.fogEffect.heightDensity);
            
            EditorGUILayout.FloatField("Current Distance = ", instance.fogEffect.startDistance);
            }

        }
    }
}
