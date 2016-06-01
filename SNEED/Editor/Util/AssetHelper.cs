using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
public static class AssetHelper {

    public static string GetOrCreateCompanionFolderForPrefab(string prefabPath)
    {
        int indexOfLastSlash = prefabPath.LastIndexOf('/');

        string folderForMazeContents = prefabPath.Substring(0, indexOfLastSlash);

        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);

        var resultingPath = string.Format("{0}/{1}", folderForMazeContents, prefabName);

        if (!AssetDatabase.IsValidFolder(resultingPath))
        {
            string guid = AssetDatabase.CreateFolder(folderForMazeContents, prefabName);
            
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        return resultingPath;

    }
     
}
