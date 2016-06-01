using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;

[InitializeOnLoad]
public class ContextMenuExtensions 
{
    private const string COPY_PREFAB_ASSET_HERE = "Assets/Copy Prefab";

    [MenuItem(COPY_PREFAB_ASSET_HERE, isValidateFunction: true)]
    static bool ValidateCopyHere(MenuCommand command)
    {
        var allPrefabs = Selection.objects.All((o) => o.IsPrefab());

        return allPrefabs;
    }

    [MenuItem(COPY_PREFAB_ASSET_HERE, false, 1)]
    static void CopyHere(MenuCommand command)
    {
        var selection = Selection.objects;

        if(selection.Length > 1 && selection.All((o) => o.IsPrefab())){

            Selection.objects.ApplyToAll(
                (o) => CopyPrefab(o, false));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return;
        }

        var originalPrefab = Selection.activeObject as GameObject;

        if (originalPrefab != null)
            CopyPrefab(originalPrefab);
    }

    public static void CopyPrefab(UnityEngine.Object prefab, bool saveAndRefresh = true){

        string originalFileName = AssetDatabase.GetAssetPath(prefab.GetInstanceID());

        string folderPath = Path.GetDirectoryName(originalFileName);

        var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        PrefabUtility.DisconnectPrefabInstance(instance);

        var cloneName = instance.name + " (Clone)";

        var prefabCloneFilePath = string.Format("{0}{1}{2}.{3}", folderPath, Path.AltDirectorySeparatorChar, cloneName, EditorEnvironmentConstants.PREFAB_EXTENSION);

        PrefabUtility.CreatePrefab(prefabCloneFilePath, instance);

        Editor.DestroyImmediate(instance);

        if (saveAndRefresh) { 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
