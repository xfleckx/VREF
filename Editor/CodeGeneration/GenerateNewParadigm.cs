using UnityEngine;
using System.Collections;
using UnityEditor;

public class GenerateNewParadigm : EditorWindow {

    [MenuItem("SNEED/Maze/Unit Creator")]
    static void OpenUnitCreator()
    {
        var window = EditorWindow.GetWindow<EditorWindow>();

        window.titleContent = new GUIContent("Paradigm Creator");

        window.Show();
    }
}
