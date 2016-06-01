using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class EditorEnvironmentConstants : ScriptableObject
{
    [InitializeOnLoadMethod]
    public static void AssertThatAllDirectoriesExist()
    {
        CreateWhenNotExist(Get_BASE_ASSET_PATH());
        CreateWhenNotExist(Get_PREFAB_DIR_PATH());
        CreateWhenNotExist(Get_PROJECT_PREFAB_DIR_PATH());
        CreateWhenNotExist(Get_PROJECT_MODEL_DIR_PATH());
        CreateWhenNotExist(Get_PACKAGE_MODEL_SUBFOLDER());
        CreateWhenNotExist(Get_PACKAGE_PREFAB_SUBFOLDER());
    }

    public static void CreateWhenNotExist(string expectedPathRelativeToAssetDirectory){

        if (expectedPathRelativeToAssetDirectory == String.Empty)
            return;

        string parentFolder = String.Empty;
        string folderName = String.Empty;

        GetParentFolder(expectedPathRelativeToAssetDirectory, out parentFolder, out folderName);

        if (!AssetDatabase.IsValidFolder(expectedPathRelativeToAssetDirectory))
            AssetDatabase.CreateFolder(parentFolder, folderName);

    }

    private static void GetParentFolder(string path, out string parentFolder, out string folderName)
    {
        int lastSeperator = path.LastIndexOf(Path.AltDirectorySeparatorChar);
        parentFolder = path.Substring(0, lastSeperator);
        folderName = path.Substring(lastSeperator + 1, path.Length - lastSeperator - 1);
    }

    public const string ASSET_PACKAGE_NAME = "SNEED";

    public const string ASSET_CREATION_PREFIX = "CreatedBy_";

    public const string ASSET_DIR = "Assets";

    public const string PREFABS_DIR = "Prefabs";
    public const string PREFAB_EXTENSION = "prefab";
    public const string MODELS_DIR = "Models";

    public static string Get_BASE_ASSET_PATH(){
        return ASSET_DIR + Path.AltDirectorySeparatorChar + ASSET_PACKAGE_NAME;
    }
    public static string Get_BASE_ASSET_MATERIALS_PATH()
    {
        return ASSET_DIR + Path.AltDirectorySeparatorChar + ASSET_PACKAGE_NAME + Path.AltDirectorySeparatorChar + "Materials";
    }
    public static string Get_PREFAB_DIR_PATH()
    {
        return Get_BASE_ASSET_PATH() + Path.AltDirectorySeparatorChar + PREFABS_DIR;
    }

    public static string Get_ASSET_MODEL_DIR_PATH()
    {
        return Get_BASE_ASSET_PATH() + Path.AltDirectorySeparatorChar + MODELS_DIR;
    }

    public static string Get_PROJECT_MODEL_DIR_PATH()
    {
        return ASSET_DIR + Path.AltDirectorySeparatorChar + MODELS_DIR;
    }

    public static string Get_PROJECT_PREFAB_DIR_PATH()
    {
        return ASSET_DIR + Path.AltDirectorySeparatorChar + PREFABS_DIR;
    }

    public static string Get_PACKAGE_MODEL_SUBFOLDER()
    {
        return Get_PROJECT_MODEL_DIR_PATH() + Path.AltDirectorySeparatorChar + ASSET_CREATION_PREFIX + ASSET_PACKAGE_NAME;
    }

    public static string Get_PACKAGE_PREFAB_SUBFOLDER()
    {
        return Get_PROJECT_PREFAB_DIR_PATH() + Path.AltDirectorySeparatorChar + ASSET_CREATION_PREFIX + ASSET_PACKAGE_NAME;
    }

}