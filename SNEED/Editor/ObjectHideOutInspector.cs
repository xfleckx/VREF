using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(ObjectHideOut))]
public class ObjectHideOutInspector : Editor {

    ObjectHideOut instance;

    public override void OnInspectorGUI()
    {
        instance = target as ObjectHideOut;

        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open"))
        {
            instance.Open();
        }

        if (GUILayout.Button("Close"))
        {
            instance.Close();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        var options = Enum.GetValues(typeof(OpenDirections));

        foreach (var item in options)
        {
            var buttonText = Enum.GetName(typeof(OpenDirections),item);

            if (GUILayout.Button(buttonText))
            {
                Toogle(instance, (OpenDirections) item);
            }
        }

        //EditorGUILayout.BeginVertical();

        //if (GUILayout.Button(MazeUnit.NORTH))
        //{
        //    Toogle(instance, MazeUnit.NORTH);
        //}

        //if (GUILayout.Button(MazeUnit.SOUTH))
        //{
        //    Toogle(instance, MazeUnit.SOUTH);
        //}

        //EditorGUILayout.EndVertical();

        //EditorGUILayout.BeginVertical();

        //if (GUILayout.Button(MazeUnit.WEST))
        //{
        //    Toogle(instance, MazeUnit.WEST);
        //}

        //if (GUILayout.Button(MazeUnit.EAST))
        //{
        //    Toogle(instance, MazeUnit.EAST);
        //}
        //EditorGUILayout.EndVertical();


        EditorGUILayout.EndHorizontal();

    }

    private void Toogle(ObjectHideOut hideOut, OpenDirections direction)
    {
        if (hideOut.IsOpen(Enum.GetName(typeof(OpenDirections), direction)))
            hideOut.Close(direction);
        else
            hideOut.Open(direction);

    }
}
