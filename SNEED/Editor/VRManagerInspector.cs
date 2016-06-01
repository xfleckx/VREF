using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[CustomEditor(typeof(VirtualRealityManager))]
public class VRManagerInspector : Editor
{
    private VirtualRealityManager vrcontroller;

    void OnEnable()
    {
        LookUpEnvironments();
    }

    private void LookUpEnvironments()
    {
        vrcontroller = (VirtualRealityManager) target;

        var environments = vrcontroller.transform.AllChildren().Where(
            (e) => e.GetComponent<EnvironmentController>() != null)
            .Select((e) => e.GetComponent<EnvironmentController>());

        vrcontroller.AvailableEnvironments.Clear();
        vrcontroller.AvailableEnvironments.AddRange(environments);
    }

    public override void OnInspectorGUI()
    {
        if(vrcontroller == null)
            vrcontroller = (VirtualRealityManager)target;

        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        vrcontroller = (VirtualRealityManager)target;

        if (!vrcontroller.AvailableEnvironments.Any((c) => c != null))
            return;

        Handles.BeginGUI();

        GUILayout.Space(25);

        GUILayout.BeginVertical(GUILayout.MaxWidth(75));
        
        GUILayout.Label("Choose Environment", EditorStyles.whiteLabel);
        GUILayout.Space(10);


        foreach (var item in vrcontroller.AvailableEnvironments)
        {
            GUILayout.BeginHorizontal();

            var state = item.gameObject.activeSelf;

            state = GUILayout.Toggle(state, "");

            item.gameObject.SetActive(state);

            if (GUILayout.Button(item.Title, GUILayout.Width(75)))
            {
                vrcontroller.ChangeWorld(item.Title);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();


        Handles.EndGUI();
    }

}
