using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Diagnostics;

namespace Assets.VREF.Application.Editor
{
    public class ScriptBatch
    {
        [MenuItem("VREF/Combile SearchAndFind")]
        public static void Build_SearchAndFind()
        {
            /// a prototypical function to test automaticaly build scenes

            // Get filename.
            string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", System.Environment.CurrentDirectory, "");
            string[] levels = new string[] { "Assets/Paradigms/SearchAndFind.unity" };

            var executableName = "SearchAndFind";
            var dataFolderName = executableName + "_Data";
            var targetExecutable = path + "/" + executableName + ".exe";

            // Build player.
            BuildPipeline.BuildPlayer(levels, targetExecutable, BuildTarget.StandaloneWindows64, BuildOptions.None);

            var nlogConfigFile = "NLog.config";
            var configFileName = "SearchAndFind_Config.json";

            // Copy a file from the project folder to the build folder, alongside the built game.
            FileUtil.CopyFileOrDirectory("Assets/" + nlogConfigFile, path + "/" + dataFolderName + "/" + nlogConfigFile);
            FileUtil.CopyFileOrDirectory("Assets/" + configFileName, path + "/" + dataFolderName + "/" + configFileName);

            // open the target directory
            Process.Start(path);

        }
    }
}