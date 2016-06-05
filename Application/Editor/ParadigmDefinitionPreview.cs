using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.VREF.Application;
using System;
using System.Linq;

namespace Assets.VREF.Application.Editor
{
    public class ParadigmDefinitionPreview : EditorWindow
    {
        const string DEFINITION_PREVIEW_PATTERN = "{0}: {1} -> {2} = {3} from {4}";

        private Vector2 configPreviewScrollState;
        private int indexOfSelectedDefintionPreview;
        private ConditionDefinition selectedConditionForPreview;
        private ParadigmModel instanceToPreview;

        public static void ShowFor(ParadigmModel definition)
        {
            var window = EditorWindow.GetWindow<ParadigmDefinitionPreview>();

            window.SetModelToPreview(definition);

            window.Show();
        }

        private void SetModelToPreview(ParadigmModel definition)
        {
            this.instanceToPreview = definition;
        }


        void OnGUI() {
            RenderPreviewFor(instanceToPreview);
        }

        private void RenderPreviewFor(ParadigmModel definition)
        {
            EditorGUILayout.LabelField("Preview:");

            var definitionNames = definition.Conditions.Select(cc => cc.Identifier).ToArray();

            indexOfSelectedDefintionPreview = EditorGUILayout.Popup(indexOfSelectedDefintionPreview, definitionNames);

            selectedConditionForPreview = definition.Conditions[indexOfSelectedDefintionPreview];

            EditorGUILayout.LabelField(string.Format("Condition {0} with {1} Trials", selectedConditionForPreview.Identifier, selectedConditionForPreview.Trials.Count));

            configPreviewScrollState = EditorGUILayout.BeginScrollView(configPreviewScrollState);

            if (selectedConditionForPreview.Trials != null)
            {
                string lastMazeName = string.Empty;

                foreach (var tdef in selectedConditionForPreview.Trials)
                {
                    if (!lastMazeName.Equals(tdef.MazeName))
                        EditorGUILayout.Space();

                    EditorGUILayout.LabelField(
                        string.Format(DEFINITION_PREVIEW_PATTERN,
                        tdef.TrialType,
                        tdef.MazeName,
                        tdef.Path,
                        tdef.ObjectName,
                        tdef.Category));
                    lastMazeName = tdef.MazeName;
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

}