using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Assertions;
using System;

namespace Assets.BeMoBI.Scripts
{
    public static class ConfigUtil
    {
        public static void SaveAsJson<C>(FileInfo pathToConfig, C objectToSave)
        {
            var configAsJson = JsonUtility.ToJson(objectToSave, true);

            using (var streamWriter = new StreamWriter(pathToConfig.FullName))
            {
                streamWriter.Write(configAsJson);
            }
        }

        public static C LoadConfig<C>(FileInfo expectedConfig, bool writeNewWhenNotFound, Action onLoadFailed) where C : ScriptableObject
        {
            string jsonAsText = String.Empty;

            C result = ScriptableObject.CreateInstance<C>();

            try
            {
                using (var fileStream = new StreamReader(expectedConfig.FullName))
                {
                    jsonAsText = fileStream.ReadToEnd();
                }

                JsonUtility.FromJsonOverwrite(jsonAsText, result);
            }
            catch (Exception)
            {
                if (onLoadFailed != null)
                    onLoadFailed();
            }
            finally
            {
                if (writeNewWhenNotFound)
                {
                    SaveAsJson<C>(expectedConfig, result);
                }
            }

            return result;
        }
    }
}