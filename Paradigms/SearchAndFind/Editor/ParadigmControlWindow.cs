using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;
using NLog;
using NLogger = NLog.Logger;
using Assets.BeMoBI.Paradigms.SearchAndFind;

namespace Assets.Editor.BeMoBI.Paradigms.SearchAndFind
{
    public class ParadigmControlWindow : EditorWindow
    {
        NLogger log = LogManager.GetCurrentClassLogger();

        private ParadigmController instance;

        [SerializeField]
        private string subject_ID = "TestSubject";
        
        private ParadigmModel lastGeneratedInstanceConfig;

        internal void Initialize(ParadigmController target)
        {
            instance = target;

            titleContent = new GUIContent("Paradigm Control");
            
            log.Info("Initialize Paradigma Control Window");
        }

        GUILayoutOption windowWidth;

        void OnGUI()
        {
            windowWidth = GUILayout.MinWidth(this.position.size.x - 10);

            EditorGUILayout.BeginVertical(windowWidth);

            if (instance == null && (instance = TryGetInstance()) == null) { 
                EditorGUILayout.HelpBox("No Paradigm Controller available! \n Open another scene or create a paradigm controller instance!", MessageType.Info);
                return;
            }
            
            EditorGUILayout.BeginVertical();

            RenderControlGUI(); 

            EditorGUILayout.EndVertical();
        }

        private ParadigmController TryGetInstance()
        {
            return FindObjectOfType<ParadigmController>();
        }

        private void RenderControlGUI()
        {
            GUILayout.Label("Control", EditorStyles.largeLabel);

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Please start the playmode through this start button!", MessageType.Info);

                RenderRunVariables();

                if (GUILayout.Button("Start playmode", GUILayout.Height(30)))
                { 
                    InjectCmdArgs();

                    EditorApplication.ExecuteMenuItem("Edit/Play");
                }


                if(GUILayout.Button("Open Survey"))
                {
                    var httpRequest = FormatSurveyRequest();

                    Process.Start("explorer", httpRequest);
                }


                if (GUILayout.Button("Open logs"))
                {
                    Process.Start("explorer", ".");
                }

                return;
            }

            if (!instance.conditionController.IsRunning) {
                
                if (GUILayout.Button("Start First Trial", GUILayout.Height(25)))
                {
                    instance.StartExperimentFromBeginning();
                }

            }
            else
            {
                EditorGUILayout.HelpBox("Paradigma is already running", MessageType.Info);
                RenderOptionsForRunningState();

            }

        }

        private void RenderOptionsForRunningState()
        {
            if(instance.conditionController.currentTrial != null)
            {
                var currentTrial = instance.conditionController.currentTrial;
                var trialName = currentTrial.GetType().Name;

                EditorGUILayout.LabelField("Current Trial: ", trialName, windowWidth);

                var currentMaze = currentTrial.currentMazeName != null && currentTrial.currentMazeName != String.Empty ? currentTrial.currentMazeName : "none";
                EditorGUILayout.LabelField("Maze: ", currentMaze);

                var currentObject = currentTrial.objectToRemember != null ? currentTrial.objectToRemember.name : "none";
                EditorGUILayout.LabelField("Object: ", currentObject, windowWidth);

                var currentPath = currentTrial.currentPathID != -1 ? currentTrial.currentPathID.ToString() : "none";
                EditorGUILayout.LabelField("Path: ", currentPath);
            }


            if (instance.conditionController.currentTrial != instance.pause && GUILayout.Button("Inject Pause Trial"))
            {
                instance.conditionController.InjectPauseTrialAfterCurrentTrial();
            }

            if (instance.conditionController.currentTrial == instance.pause && GUILayout.Button("End Pause"))
            {
                instance.conditionController.currentTrial.ForceTrialEnd();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("End current run \n (stop playmode)", GUILayout.Height(25)))
            {
                // TODO force immediately safe shutdown

                instance.conditionController.currentTrial.Finished += (t, ts) => {
                    EditorApplication.ExecuteMenuItem("Edit/Play"); // call Play a second time to stop it
                };
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Paradigma State \n and Quit"))
            {
                instance.ForceABreakInstantly();
            }
        }

        private string FormatSurveyRequest()
        {
            return @"http:\\localhost\limesurvey\index.php\197498?lang=en" + "?subject=test?pose=bla";
        }

        private void RenderRunVariables()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Subject ID:");
            subject_ID = EditorGUILayout.TextField(subject_ID);
            
            GUILayout.Label("Run definition:");
            
            instance.InstanceDefinition = EditorGUILayout.ObjectField(instance.InstanceDefinition, typeof(ParadigmModel), false) as ParadigmModel;

            if (instance.Config != null) { 
                instance.Config.writeStatistics =  GUILayout.Toggle(instance.Config.writeStatistics, "Write statistics?");
            }

            EditorGUILayout.EndVertical();
        }

        private void InjectCmdArgs()
        {
            instance.SubjectID = subject_ID;
            instance.appInit.UpdateLoggingConfiguration();
        }
        
        const string DEFINITION_PREVIEW_PATTERN = "{0}: {1} -> {2} = {3} from {4}";
        private Vector2 configPreviewScrollState;
        
    }
}