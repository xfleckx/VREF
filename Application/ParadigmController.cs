using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.IO;

using Debug = UnityEngine.Debug;
// A logging framework, mainly used to write the log and statistic files. 
// See also the NLog.config within the asset directory 
// Pittfall: You need to copy the NLog.config file to the *_DATA directory after the build!
using NLog;
using Logger = NLog.Logger; // just aliasing
using Assets.VREF.PhaseSpaceExtensions;
using Assets.VREF.Interfaces;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Assets.VREF.Application;

namespace Assets.VREF.Application
{
    public class ParadigmController : MonoBehaviour, IParadigmControl, IProvideRigidBodyFile
    {
        public const string STD_CONFIG_NAME = "SearchAndFind_Config.json";

        private static Logger appLog = LogManager.GetLogger("App");

        #region Constants

        private const string ParadgimConfigDirectoryName = "ParadigmConfig";

        private const string ParadigmConfigNamePattern = "VP_{0}_{1}";

        private const string DateTimeFileNameFormat = "yyyy-MM-dd_hh-mm";

        private const string DetailedDateTimeFileNameFormat = "yyyy-MM-dd_hh-mm-ss-tt";
	
        #endregion

        #region dependencies

        public string SubjectID = String.Empty;

        public AppInit appInit;

        public ConditionController conditionController;
        // TODO abstract from the implementation
        public IConditionController ConditionController
        {
            get
            {
                return conditionController;
            }
        }

        public FileInfo fileToLoadedConfig;
        
        public ParadigmConfiguration Config;

        public ParadigmModel InstanceDefinition;
        
        public ActionWaypoint TrialEndPoint;
        public VirtualRealityManager VRManager;
        public StartPoint startingPoint;
        public HUD_Instruction hud;
        public HUD_DEBUG debug_hud;
        public AudioInstructions audioInstructions;
        public IMarkerStream marker;
        public GameObject objectPresenter;
        public ObjectPool objectPool;
        public Transform objectPositionAtTrialStart;
        public GameObject HidingSpotPrefab;
        public GameObject entrance;
        public FullScreenFade fading;
        public Teleporting teleporter;
        public VRSubjectController subject;
        public BaseFogControl fogControl;
        public ISubjectRelativePositionStream relativePositionStream;
        public Transform FocusPointAtStart;
        
        public Training training;
        public Experiment experiment;
        public Pause pause;
        public InstructionTrial instruction;
        private bool waitingForSignalToStartNextCondition;

        public string PathToLoadedConfig = string.Empty;

        #endregion

        void Awake()
        {
            if (VRManager == null)
                throw new MissingReferenceException("Reference to VirtualRealityManager is missing");
			
			marker = FindObjectOfType<IMarkerStream> ();

            if (marker == null)
                throw new MissingReferenceException("Reference to a MarkerStream instance is missing");

            if (hud == null)
                throw new MissingReferenceException("No HUD available, you are not able to give visual instructions");

            if (subject == null)
                subject = FindObjectOfType<VRSubjectController>();

            Assert.IsNotNull<ConditionController>(conditionController);

            conditionController.OnLastConditionFinished += ParadigmInstanceFinished;
        }

        void Start()
        {
            First_GetTheSubjectName();
            
            appLog.Info("Initializing Paradigm");

            Second_LoadOrGenerateAConfig();
            
            Third_LoadOrGenerateInstanceDefinition();

            conditionController.OnConditionFinished += ConditionFinished;

            Fourth_InitializeFirstOrDefaultCondition();

            hud.Clear();

            fogControl.DisappeareImmediately();

            fading.StartFadeIn();

			marker = FindObjectOfType<IMarkerStream> ();

            marker.LogAlsoToFile = Config.logMarkerToFile;
        }

        private void First_GetTheSubjectName()
        {
            if (SubjectID == string.Empty)
            {
                if (appInit.Options.subjectId != String.Empty)
                    SubjectID = appInit.Options.subjectId;
                else
                    SubjectID = ParadigmUtils.GetRandomSubjectName();
            }
            
            // this is enables access to variables used by the logging framework
            NLog.GlobalDiagnosticsContext.Set("subject_Id", SubjectID);

            appInit.UpdateLoggingConfiguration();

            appLog.Info(string.Format("Using Subject Id: {0}", SubjectID));
        }

        private void Second_LoadOrGenerateAConfig()
        {
            var pathOfDefaultConfig = new FileInfo(Application.dataPath + Path.AltDirectorySeparatorChar + STD_CONFIG_NAME);

            if (Config == null)
            {
                appLog.Trace("Try Loading Config...");

                appLog.Trace("Custom Config: " + appInit.Options.fileNameOfCustomConfig);

                var pathToCustomConfig = Application.dataPath + Path.AltDirectorySeparatorChar + appInit.Options.fileNameOfCustomConfig;

                appLog.Trace("Custom Config Path: " + pathToCustomConfig.Trim());

                bool customConfigFileExists = File.Exists(pathToCustomConfig);

                if (customConfigFileExists)
                {
                    var configFile = new FileInfo(Application.dataPath + Path.AltDirectorySeparatorChar + appInit.Options.fileNameOfCustomConfig);

                    appLog.Info(string.Format("Load specific config: {0}!", configFile.FullName));

                    Config = ConfigUtil.LoadConfig<ParadigmConfiguration>(configFile, true, 
                        () => appLog.Error("Loading config failed, using default config + writing a default config"));

                    PathToLoadedConfig = configFile.FullName;
                }
                else if (pathOfDefaultConfig.Exists) 
                {
                    appLog.Info(string.Format("Found default config at {0}", pathOfDefaultConfig.Name));

                    Config = ConfigUtil.LoadConfig<ParadigmConfiguration>(pathOfDefaultConfig, false, () => {
                        appLog.Error (string.Format("Load default config at {0} failed!", pathOfDefaultConfig.Name));
                    });

                    PathToLoadedConfig = pathOfDefaultConfig.FullName;
                }
                else
                {
                    Config = ParadigmConfiguration.GetDefault();

                    var customPath = pathOfDefaultConfig;

                    if (appInit.HasOptions && appInit.Options.fileNameOfCustomConfig != string.Empty)
                    {
                        customPath = new FileInfo(Application.dataPath + Path.AltDirectorySeparatorChar + appInit.Options.fileNameOfCustomConfig);
                    }
                    
                    appLog.Info(string.Format("New Config created will be saved to: {0}! Reason: No config file found!", customPath.FullName));

                    try
                    {
                        ConfigUtil.SaveAsJson<ParadigmConfiguration>(pathOfDefaultConfig, Config);
                    }
                    catch (Exception e)
                    {
                        appLog.Info(string.Format("Config could not be saved to: {0}! Reason: {1}", pathOfDefaultConfig.FullName, e.Message));
                    }

                    PathToLoadedConfig = customPath.FullName;
                }

            }
            else
            {
                appLog.Warn("A configuration instance already exist before loading one!");
            }

        }
        
        private void Third_LoadOrGenerateInstanceDefinition()
        {
            if (InstanceDefinition == null)
            {
                UnityEngine.Debug.Log("No instance definition found.");

                if (appInit.HasOptions && File.Exists(appInit.Options.fileNameOfParadigmDefinition))
                {
                    var logMsg = string.Format("Load instance definition from {0}", appInit.Options.fileNameOfParadigmDefinition);

                    UnityEngine.Debug.Log(logMsg);

                    appLog.Info(logMsg);

                    var fileContainingDefinition = new FileInfo(appInit.Options.fileNameOfParadigmDefinition);

                    LoadInstanceDefinitionFrom(fileContainingDefinition);
                }
                else
                {
                    UnityEngine.Debug.Log("Create instance definition.");

                    var factory = new ParadigmModelFactory();

                    factory.config = Config;

                    try
                    {
                        InstanceDefinition = factory.Generate(SubjectID, Config.conditionConfigurations);

                        Save(InstanceDefinition);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);

                        appLog.Fatal(e, "Incorrect configuration! - Try to configure the paradigm in the editor!");

                        appLog.Fatal("Not able to create an instance definition based on the given configuration! Check the paradigm using the UnityEditor and rebuild the paradigm or change the expected configuration!");

                        Application.Quit();
                    }
                }
            }

        }

        private void Fourth_InitializeFirstOrDefaultCondition()
        {
            conditionController.PendingConditions = InstanceDefinition.Conditions;
            conditionController.FinishedConditions = new List<ConditionDefinition>();
        }

        private void Save(ParadigmModel instanceDefinition)
        {
            var fileNameWoExt = string.Format("{1}{0}PreDefinitions{0}VP_{2}_Definition", Path.AltDirectorySeparatorChar, Application.dataPath, instanceDefinition.Subject);

            var jsonString = JsonUtility.ToJson(InstanceDefinition, true);

            var targetFileName = fileNameWoExt + ".json";

            appLog.Info(string.Format("Saving new definition at: {0}", targetFileName));

            using (var file = new StreamWriter(targetFileName))
            {
                file.Write(jsonString);
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.F5))
            {
                StartExperimentOrNextCondition();
            }

            if (Input.GetKeyUp(KeyCode.F1))
                ToogleDebugHUD();
        }

        public void StartExperimentOrNextCondition()
        {
            bool noConditionHasBeenInitialized = conditionController.PendingConditions.Count == Config.conditionConfigurations.Count;

            if (waitingForSignalToStartNextCondition == true)
                waitingForSignalToStartNextCondition = false;

            if (!conditionController.IsRunning && noConditionHasBeenInitialized)
                StartExperimentFromBeginning();

            if (!conditionController.IsRunning)
                conditionController.SetNextConditionPending();
        }

        private void ToogleDebugHUD()
        {
            if (debug_hud != null)
                debug_hud.gameObject.SetActive(!debug_hud.gameObject.activeSelf);
        }

        public void AfterTeleportingToEndPoint()
        { 
            subject.transform.LookAt(FocusPointAtStart);
            var resultRotation = Quaternion.Euler(0, subject.transform.rotation.eulerAngles.y, 0);
            subject.Rotate(resultRotation);
        }

        private void ConditionFinished(string conditionId)
        {
            appLog.Info(string.Format("Condition {0} has finished!", conditionId));

            if (!conditionController.PendingConditions.Any()) { 

                ParadigmInstanceFinished();

                return;
            }

            if (Config.waitForCommandOnConditionEnd)
            {
                appLog.Info(string.Format("Waiting for signal to start next condition...", conditionId));

                //hud.ShowInstruction("You made it through one part of the experiment!", "Congrats!");

                waitingForSignalToStartNextCondition = true;

                StartCoroutine(WaitForCommandToStartNextCondition());

                return;
            }

            conditionController.SetNextConditionPending(true);
        }

        private IEnumerator WaitForCommandToStartNextCondition()
        {
            yield return new WaitWhile(() => waitingForSignalToStartNextCondition);

            waitingForSignalToStartNextCondition = false; // reset

            conditionController.SetNextConditionPending(true);

            yield return null;
        }

        private void ParadigmInstanceFinished()
        {
            //hud.ShowInstruction("You made it!\nThx for participation!", "Experiment finished!");

            marker.Write("End Experiment");

            appLog.Info("Paradigma run finished");
        }

        public void OnRotationEvent(RotationEventArgs args)
        {
            if (args.state == RotationEventArgs.State.Begin)

                marker.Write("Begin Rotation", LSLTimeSync.Instance.UpdateTimeStamp);
            
            if (args.state == RotationEventArgs.State.End)
                marker.Write("End Rotation", LSLTimeSync.Instance.UpdateTimeStamp);
        }

        #region Public interface for controlling the paradigm remotely
        
        public void StartExperimentFromBeginning()
        {
            appLog.Info(string.Format("Run complete paradigma as defined in {0}!", InstanceDefinition.name));

            marker.Write("Start Experiment");

            conditionController.SetNextConditionPending();

            conditionController.StartCurrentConditionWithFirstTrial();
        }
        
        public void InitializeCondition(string condition)
        {
            appLog.Info(string.Format("Condition {0} requested", condition));

            try
            {
                var requestedCondition =  InstanceDefinition.Get(condition);

                conditionController.Initialize(requestedCondition);

            }catch(InvalidOperationException ioe)
            {
                appLog.Error(string.Format("Initialize condition {0} failed! {1}", condition, ioe.Message));
            }
            catch (ArgumentException ae)
            {
                appLog.Error(ae, "Expected Condition could not be started - maybe not implemented or has a wrong name?!");
            }

        }

        public void SubjectTriesToSubmit()
        {
            if (conditionController.currentTrial != null && conditionController.currentTrial.acceptsASubmit)
            {
                conditionController.currentTrial.RecieveSubmit();
            }

        }

        public void ForceABreakInstantly()
        {
            //conditionController.InjectPauseTrial();
            conditionController.ResetCurrentTrial();
            hud.ShowInstruction("Press the Submit Button to continue!\n Close your eyes and talk to the supervisor!", "Break");
        }
          
        public void StartExperimentWithCurrentPendingCondition()
        {
            conditionController.StartCurrentConditionWithFirstTrial();
        }

        public void Restart(string condition = "")
        {
            if(condition == "") {

                var currentScene = SceneManager.GetActiveScene();

                SceneManager.LoadScene(currentScene.name, LoadSceneMode.Single);
            }
        }
        
        /// <summary>
        /// Loads a predefined definition from a file
        /// May override already loaded config!
        /// </summary>
        /// <param name="file"></param>
        public void LoadInstanceDefinitionFrom(FileInfo file)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                var jsonFromFile = reader.ReadToEnd();

                var loadedDefinition = JsonUtility.FromJson<ParadigmModel>(jsonFromFile);
              

                if (InstanceDefinition == null)
                {
                    appLog.Fatal(string.Format("Loading {0} as Instance Definition failed!", file.FullName));
                    return;
                }

                InstanceDefinition = loadedDefinition;
                Config = InstanceDefinition.Configuration;
            }
        }

        public FileInfo GetRigidBodyDefinition()
        {
            var fileName = Config.nameOfRigidBodyDefinition;

            var expectedFilePath = Path.Combine(Application.dataPath, fileName);

            if (File.Exists(expectedFilePath))
            {
                return new FileInfo(expectedFilePath);
            }

            return null;
        }

        public void SaveCurrentState()
        {
            // TODO serialize to JSON string...  JsonUtility.ToJson()
            throw new NotImplementedException("TODO");
        }

        public void LoadState()
        {
            throw new NotImplementedException("TODO");
        }

        public void ForceSaveEndOfExperiment()
        {
            conditionController.PendingConditions.Clear();
            conditionController.ForceASaveEndOfCurrentCondition();
        }

        #endregion
    }

    #region TODO: Save instance state (SaveGames)
    //https://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/persistence-data-saving-loading
    [Serializable]
    public class InstanceState
    {
        public string Subject;

        public string InstanceDefinition;

        public TrialDefinition LastFinishedTrial;
    }

    #endregion
}