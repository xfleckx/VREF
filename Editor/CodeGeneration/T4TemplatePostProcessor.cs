using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Original Source Code from
/// https://issuetracker.unity3d.com/issues/t4-templates-disappear-from-monodevelop-project-after-refreshing-solution
/// http://pastebin.com/vmGQJeLk
/// </summary>

namespace UnityEditor.T4Templating
{
    public sealed class T4TemplatePostProcessor : AssetPostprocessor
    {
        [MenuItem("Assets/Force T4 Detection", priority = 1000)]
        public static void OnGeneratedCSProjectFiles()
        {
            string[] Starts = { "<Compile Include=\"", "<None Include=\"" };
            const string End = "\" />";
            string[] GeneratedLines = {
@"<AutoGen>True</AutoGen>",
@"<DesignTime>True</DesignTime>",
@"<DependentUpon>{0}</DependentUpon>",
                                      };
            string[] TemplateLines = {
"<None Include=\"{0}\">",
"<Generator>TextTemplatingFileGenerator</Generator>",
"<LastGenOutput>{1}</LastGenOutput>",
"</None>",
                                     };
            const string ItemGroupEnd = "  </ItemGroup>";
            List<string> TemplatesFiles = null;
            List<string> TemplateTargets = null;
            bool AnyTemplates = false;
            bool AnyModifications = false;
            string projectDirectory = Directory.GetCurrentDirectory();
            const string searchPattern = "Assembly-CSharp*-vs.csproj";
            const SearchOption searchOption = SearchOption.TopDirectoryOnly;
            string[] projectFiles;
            try
            {
                projectFiles = Directory.GetFiles(projectDirectory, searchPattern, SearchOption.TopDirectoryOnly);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                UnityEngine.Debug.LogError(projectDirectory + " " + searchPattern + " " + searchOption);
                return;
            }
            foreach (var file in projectFiles)
            {
                string tempProjectFile = FileUtil.GetUniqueTempPathInProject() + " " + Path.GetFileName(file);
                using (var reader = new StreamReader(file))
                using (var writer = new StreamWriter(tempProjectFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.EndsWith(End))
                        {
                            bool ActedOn = false;
                            foreach (var Start in Starts)
                            {
                                int startIndex = line.IndexOf(Start);
                                if (startIndex != -1)
                                {
                                    startIndex += Start.Length;

                                    int lengthFromIndex = line.Length - (startIndex + End.Length);
                                    // there should be no quotes in here..
                                    if (line.IndexOf('\"', startIndex, lengthFromIndex) == -1)
                                    {
                                        string path = line.Substring(startIndex, lengthFromIndex);

                                        string withTT = Path.Combine(
                                            Path.GetDirectoryName(path),
                                            Path.GetFileNameWithoutExtension(path) + ".tt");

                                        if (File.Exists(withTT))
                                        {
                                            string line_stripped_exec = line.Remove(line.Length - 2, 1);

                                            int starterPos = Start.IndexOf('<') + 1;
                                            string manual_exec =
                                                string.Concat(
                                                    Start.Remove(starterPos),
                                                    "/",
                                                    Start.Substring(starterPos, Start.IndexOf(' ', starterPos) - starterPos),
                                                    ">");

                                            object ttFileName = Path.GetFileName(withTT);

                                            writer.WriteLine(line_stripped_exec);

                                            AnyModifications = true;

                                            foreach (var generatedLineInsert in GeneratedLines)
                                            {
                                                writer.WriteLine(generatedLineInsert, ttFileName);
                                            }
                                            writer.WriteLine(manual_exec);

                                            if (!AnyTemplates)
                                            {
                                                TemplatesFiles = new List<string>();
                                                TemplateTargets = new List<string>();
                                                AnyTemplates = true;
                                            }
                                            TemplatesFiles.Add(withTT);
                                            TemplateTargets.Add(Path.GetFileName(path));
                                            ActedOn = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (ActedOn)
                                continue;
                        }
                        else if (AnyTemplates && line == ItemGroupEnd)
                        {
                            object[] args = new object[2];

                            for (int c = TemplatesFiles.Count, i = 0; i < c; i++)
                            {
                                args[0] = TemplatesFiles[i];
                                args[1] = TemplateTargets[i];

                                foreach (var templateLine in TemplateLines)
                                    writer.WriteLine(templateLine, args);
                                AnyModifications = true;
                            }
                            AnyTemplates = false;
                        }
                        writer.WriteLine(line);
                    }
                }
                if (AnyModifications)
                {
                    File.Copy(tempProjectFile, file, true);
                }
                File.Delete(tempProjectFile);
            }
        }
    }
}