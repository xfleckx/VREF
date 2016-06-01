using UnityEngine;
using UnityEditor;

public class MazeDesignerMenuItems : ScriptableObject
{
    [MenuItem("SNEED/Maze/Add new Maze as Prefab")]
    static void CreateNewMazeAsPrefab()
    {
        var filePath = EditorUtility.SaveFilePanelInProject("Save the prefab", "Maze", "prefab","Message");

        if (filePath == string.Empty)
            return;

        var prefab = PrefabUtility.CreateEmptyPrefab(filePath);

        GameObject mazeHost = new GameObject("Maze");
        mazeHost.AddComponent<beMobileMaze>();
        mazeHost.AddComponent<PathController>();
        PrefabUtility.ReplacePrefab(mazeHost, prefab, ReplacePrefabOptions.ConnectToPrefab);

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
    }

    [InitializeOnLoad]
    public class InitializeNecessaryResources
    {
        static InitializeNecessaryResources()
        {
            //Debug.Log("Copying Gizmos");
            //TODO copying gizmos in Gizmos folder
        }
    }

}