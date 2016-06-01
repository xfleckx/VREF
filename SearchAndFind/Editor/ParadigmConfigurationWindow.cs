using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using NLog;
using NLogger = NLog.Logger;
using Assets.BeMoBI.Paradigms.SearchAndFind;
using Assets.BeMoBI.Scripts;
using Assets.BeMoBI.Scripts.Controls;

namespace Assets.Editor.BeMoBI.Paradigms.SearchAndFind
{
    public class ParadigmModelEditor : EditorWindow
    {
        NLogger log = LogManager.GetCurrentClassLogger();

        private ParadigmController instance;

        private ParadigmModelFactory factory;

        private ConfigurationWindowModel model;
        private VRSubjectController subjectController;

        public String PreDefinedSubjectID = "TestSubject";

        public Action OnPostOnGUICommands;
         

        [SerializeField]
        private ParadigmModel lastGeneratedInstanceDefinition;

        internal void Initialize(ParadigmController target)
        {
            instance = target;

            titleContent = new GUIContent("Paradigm Control");

            InitializeModel(instance);

            log.Info("Initialize Paradigma Control Window");

        }

        private void InitializeModel(ParadigmController instance)
        {
            model = ConfigurationWindowModel.Instance;

            model.hideFlags = HideFlags.HideAndDontSave;

            factory = new ParadigmModelFactory();

            factory.config = instance.Config;

            model.SelectableMazes = new List<ViewOfSelectableMazes>();

            foreach (var maze in factory.mazeInstances)
            {
                var pathController = maze.GetComponent<PathController>();

                var selecteablePaths = pathController.Paths.Select(p => new SelectablePath(p.ID, p.GetDifficultyByCountTJunctions()));

                var mazeView = new ViewOfSelectableMazes()
                {
                    mazeName = maze.name,

                    SelectablePaths = selecteablePaths.ToList()
                };

                model.SelectableMazes.Add(mazeView);
            }

            subjectController = FindObjectOfType<VRSubjectController>();

            model.headControllerNames = subjectController.GetComponents<IHeadMovementController>().Select(hc => hc.Identifier).ToArray();
            model.bodyControllerNames = subjectController.GetComponents<IBodyMovementController>().Select(bc => bc.Identifier).ToArray();
            model.combiControllerNames = subjectController.GetComponents<CombinedController>().Select(cc => cc.Identifier).ToArray();

            OnConditionSelectionChanged();
        }

        private int indexOfSelectedCondition = 0;

        private ConditionConfiguration selectedConditionConfig;

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            if (instance == null && (instance = TryGetInstance()) == null)
            {
                UnityEngine.Debug.Log("Try get instance for Configuration controller");

                EditorGUILayout.HelpBox("No Paradigm Controller available! \n Open another scene or create a paradigm controller instance!", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            if (instance != null && instance.Config == null)
            {
                EditorGUILayout.HelpBox("No Configuration at the paradigm controller available! \n Load or create one!", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            if (model == null || factory == null)
            {
                InitializeModel(instance);
            }

            EditorGUILayout.LabelField("Configure a condition configuration", EditorStyles.largeLabel);

            EditorGUILayout.BeginHorizontal();

            var conditionNames = instance.Config.conditionConfigurations.Select(cc => cc.ConditionID).ToArray();

            if (conditionNames.Count() > indexOfSelectedCondition)
            {
                indexOfSelectedCondition = EditorGUILayout.Popup(indexOfSelectedCondition, conditionNames);

                selectedConditionConfig = instance.Config.conditionConfigurations[indexOfSelectedCondition];

                if (indexOfSelectedCondition != model.lastSelectedCondition)
                {
                    OnConditionSelectionChanged();
                    model.lastSelectedCondition = indexOfSelectedCondition;
                }

                if (GUILayout.Button("Copy"))
                {
                    OnPostOnGUICommands += () =>
                    {
                        var cloneOfSelectedConfig = selectedConditionConfig.Clone() as ConditionConfiguration;
                        cloneOfSelectedConfig.ConditionID = cloneOfSelectedConfig.ConditionID + "_copy";
                        instance.Config.conditionConfigurations.Add(cloneOfSelectedConfig);
                    };
                }

                if (GUILayout.Button("Remove"))
                {
                    OnPostOnGUICommands += () =>
                    {
                        instance.Config.conditionConfigurations.Remove(selectedConditionConfig);
                        selectedConditionConfig = null;

                        // correct the index - should not point to non existing element
                        var middleIndex = instance.Config.conditionConfigurations.Count / 2;

                        if (indexOfSelectedCondition > middleIndex)
                            indexOfSelectedCondition -= 1;

                    };
                }

                RenderAddDefaultConfigOption();

                EditorGUILayout.EndHorizontal();

                selectedConditionConfig.ConditionID = EditorGUILayout.TextField("Name of Condition Configuration:", selectedConditionConfig.ConditionID);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));

                EditorGUILayout.BeginVertical();

                RenderConfigurationOptions();

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                RenderInstanceDefinitionOptions();

                RenderPreviewGUI();

                EditorGUILayout.EndVertical();

            }
            else
            {
                EditorGUILayout.HelpBox("No Conditions available", MessageType.Info);
                RenderAddDefaultConfigOption();
            }

            if (OnPostOnGUICommands != null)
            {
                OnPostOnGUICommands();
                OnPostOnGUICommands = null;
            }
        }

        private void OnConditionSelectionChanged()
        {
            if (selectedConditionConfig == null)
                selectedConditionConfig = instance.Config.conditionConfigurations[0];

            model.indexOfSelectedBodyController = model.bodyControllerNames.ToList().IndexOf(selectedConditionConfig.BodyControllerName);
            model.indexOfSelectedHeadController = model.headControllerNames.ToList().IndexOf(selectedConditionConfig.HeadControllerName);

            model.SelectableMazes.ApplyToAll(selectableMaze =>
            {
                selectableMaze.selected = selectedConditionConfig.ExpectedMazes.Any((expected) => expected.Name.Equals(selectableMaze.mazeName));
            });

            model.SelectableMazes.Where(m => m.selected).ApplyToAll(selectedMaze => {
                selectedMaze.SelectablePaths.ApplyToAll(pathToSelect => {

                    var maze = selectedConditionConfig.ExpectedMazes.First(expectedMaze => expectedMaze.Name == selectedMaze.mazeName);
                    pathToSelect.selected = maze.pathIds.Contains(pathToSelect.Id);

                });

            });
        }

        private void RenderAddDefaultConfigOption()
        {
            if (GUILayout.Button("Add default"))
            {
                OnPostOnGUICommands += () =>
                {
                    var newDefaultCondition = ConditionConfiguration.GetDefault();
                    instance.Config.conditionConfigurations.Add(newDefaultCondition);
                };
            }
        }

        private ParadigmController TryGetInstance()
        {
            return FindObjectOfType<ParadigmController>();
        }

        private void RenderRunVariables()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Subject ID:");
            PreDefinedSubjectID = EditorGUILayout.TextField(PreDefinedSubjectID);

            EditorGUILayout.EndVertical();
        }

        private void InjectCmdArgs()
        {
            instance.SubjectID = PreDefinedSubjectID;
        }
        
        private int selectedCombiControllerIndex = 0;

        private void RenderConfigurationOptions()
        {
            if (factory.config == null)
            {
                EditorGUILayout.HelpBox("Please Load or generate a Paradigm configuration first!", MessageType.Info);

                return;
            }

            EditorGUILayout.LabelField(new GUIContent("Expetected Controller Names", "See VR Subject Controller for possible options"), EditorStyles.largeLabel);

            if (subjectController != null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Available HeadController:");

                model.indexOfSelectedHeadController = EditorGUILayout.Popup(model.indexOfSelectedHeadController, model.headControllerNames);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Available BodyController:");

                model.indexOfSelectedBodyController = EditorGUILayout.Popup(model.indexOfSelectedBodyController, model.bodyControllerNames);

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Available Combined Controller:");

                selectedCombiControllerIndex = EditorGUILayout.Popup(selectedCombiControllerIndex, model.combiControllerNames.ToArray());

                EditorGUILayout.EndHorizontal();

            }

            selectedConditionConfig.HeadControllerName = EditorGUILayout.TextField("Head Controller", selectedConditionConfig.HeadControllerName);

            selectedConditionConfig.BodyControllerName = EditorGUILayout.TextField("Body Controller", selectedConditionConfig.BodyControllerName);

            EditorGUILayout.BeginHorizontal();

            factory.config.nameOfRigidBodyDefinition = EditorGUILayout.TextField("Name of Rigidbody file", factory.config.nameOfRigidBodyDefinition);

            if (GUILayout.Button("Select"))
            {
                var filePath = EditorUtility.OpenFilePanelWithFilters("Choose rigidbody file", "Assets", new string[] { "PS_RigidBody", "rb" });

                if (filePath != null && File.Exists(filePath))
                {
                    var fileName = Path.GetFileName(filePath);

                    factory.config.nameOfRigidBodyDefinition = fileName;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(250));

            RenderMazeSelection();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("General Options:", EditorStyles.boldLabel);

            selectedConditionConfig.useExactOnCategoryPerMaze = EditorGUILayout.Toggle(
                new GUIContent("Use category exclusive", "A category will never be shared within multiple mazes"),
                selectedConditionConfig.useExactOnCategoryPerMaze);

            selectedConditionConfig.groupByMazes = EditorGUILayout.Toggle(
                new GUIContent("Group by Mazes and Paths", "Trials are set as tuples of training and experiment trials per Maze and Path"),
                selectedConditionConfig.groupByMazes);

            selectedConditionConfig.useTeleportation = EditorGUILayout.Toggle(
                new GUIContent("use Teleportaiton", "Use Teleportation to bring the subject back to start after Trial ends"),
                selectedConditionConfig.useTeleportation);

            selectedConditionConfig.UseShortWayBack = EditorGUILayout.Toggle(
                new GUIContent("use short way back", "Disables the maze and let the subject move directly to the entrance area."),
                selectedConditionConfig.UseShortWayBack);

            selectedConditionConfig.UseMonoscopicViewOnVRHeadset = EditorGUILayout.Toggle(
               new GUIContent("Use monoscopic perspective", "An option for the vr headset to get monoscopic perspective instead of stereo perspective"),
               selectedConditionConfig.UseMonoscopicViewOnVRHeadset);

            if (!selectedConditionConfig.useExactOnCategoryPerMaze)
            {
                selectedConditionConfig.categoriesPerMaze = EditorGUILayout.IntField(
                    new GUIContent("Categories per Maze", "Declares the amount of categories \n from which objects are choosen."),
                    selectedConditionConfig.categoriesPerMaze);
            }

            EditorGUILayout.LabelField("Count of object visitations");

            selectedConditionConfig.objectVisitationsInTraining = EditorGUILayout.IntField("Training", selectedConditionConfig.objectVisitationsInTraining);

            selectedConditionConfig.objectVisitationsInExperiment = EditorGUILayout.IntField("Experiment", selectedConditionConfig.objectVisitationsInExperiment);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(string.Format("Apply Settings to Condition '{0}'", selectedConditionConfig.ConditionID)))
            {
                ApplySettings(model, selectedConditionConfig);
            }

            if (GUILayout.Button("Save Configuration"))
            {


                if (!File.Exists(model.PathToCurrentConfig))
                {
                    //model.PathToCurrentConfig = EditorUtility.OpenFilePanelWithFilters("Select a config file", "Assets", new string[] { "Json", "json" });
                    model.PathToCurrentConfig = EditorUtility.SaveFilePanelInProject("Select a config file", "config", "json", "Save a new config file" );

                    if (model == null)
                    {
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                }

                if(model.PathToCurrentConfig != null)
                    ConfigUtil.SaveAsJson<ParadigmConfiguration>(new FileInfo(model.PathToCurrentConfig), instance.Config);

                EditorApplication.delayCall += () =>
                {
                    AssetDatabase.SaveAssets();
                };

            }

            EditorGUILayout.EndHorizontal();
        }

        private void ApplySettings(ConfigurationWindowModel model, ConditionConfiguration selectedConditionConfig)
        {
            selectedConditionConfig.ExpectedMazes = model.SelectableMazes.Where(m => m.selected).Select(
                vm => new ExpectedMazeWithPaths()
                {
                    Name = vm.mazeName,
                    pathIds = vm.SelectablePaths.Where(p => p.selected).Select(p => p.Id).ToList()

                }).ToList();

            selectedConditionConfig.HeadControllerName = model.headControllerNames[model.indexOfSelectedHeadController];

            selectedConditionConfig.BodyControllerName = model.bodyControllerNames[model.indexOfSelectedBodyController];
        }

        private void RenderMazeSelection()
        {
            EditorGUILayout.LabelField("Select the mazes to use:", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            foreach (var selectableMaze in model.SelectableMazes)
            {
                selectableMaze.selected = EditorGUILayout.ToggleLeft(selectableMaze.mazeName, selectableMaze.selected);

                if (selectableMaze.selected)
                {
                    EditorGUI.indentLevel++;
                    foreach (var selectedAblePath in selectableMaze.SelectablePaths)
                    {
                        selectedAblePath.selected = EditorGUILayout.ToggleLeft(String.Format("Path {0} -> Difficulty {1}", selectedAblePath.Id, selectedAblePath.Difficulty), selectedAblePath.selected);
                    }
                    EditorGUI.indentLevel--;
                }

            }
            EditorGUI.indentLevel--;
        }

        private void RenderInstanceDefinitionOptions()
        {
            EditorGUILayout.LabelField("Predefine a Instance Definition:", EditorStyles.boldLabel);

            RenderRunVariables();

            if (GUILayout.Button("Generate Instance Definition", GUILayout.Height(35)))
            {
                if (instance.SubjectID == null)
                {
                    instance.SubjectID = this.PreDefinedSubjectID;
                }

                lastGeneratedInstanceDefinition = factory.Generate(this.PreDefinedSubjectID, factory.config.conditionConfigurations);
                lastGeneratedInstanceDefinition.Configuration = instance.Config;
            }

            lastGeneratedInstanceDefinition = EditorGUILayout.ObjectField("Last Generated Definion", lastGeneratedInstanceDefinition, typeof(ParadigmModel), false) as ParadigmModel;

            if (lastGeneratedInstanceDefinition == null)
            {
                lastGeneratedInstanceDefinition = instance.InstanceDefinition;
            }

            if (GUILayout.Button("Save Instance Definition"))
            {
                var fileNameWoExt = string.Format("Assets/Paradigms/SearchAndFind/PreDefinitions/VP_{0}_Definition", lastGeneratedInstanceDefinition.Subject);

                var jsonString = JsonUtility.ToJson(lastGeneratedInstanceDefinition, true);

                AssetDatabase.CreateAsset(lastGeneratedInstanceDefinition, fileNameWoExt + ".asset");

                using (var file = new StreamWriter(fileNameWoExt + ".json"))
                {
                    file.Write(jsonString);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Use this definition"))
            {
                instance.InstanceDefinition = lastGeneratedInstanceDefinition;
            }
        }
           
        private void RenderPreviewGUI()
        {
            if (lastGeneratedInstanceDefinition == null)
                return;
             
            if (GUILayout.Button("Preview"))
            {
                ParadigmDefinitionPreview.ShowFor(lastGeneratedInstanceDefinition);
            }

        }


        void OnDestroy()
        {
            DestroyImmediate(model);
        }


    }
}