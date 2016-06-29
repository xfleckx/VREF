using Assets.VREF.Scripts.Interfaces;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Assets.VREF.Scripts.BaseParadigm
{

    /// <summary>
    /// This class is intended to be an base class for more complex experiments depending on several conditions
    /// using several conditions and so one....
    /// Not finished yet
    /// </summary>
    /// <typeparam name="TParadigmConfig"></typeparam>
    /// <typeparam name="TParadigmDefinition"></typeparam>
    /// <typeparam name="TConditionConfiguration"></typeparam>
    /// <typeparam name="TConditionDefinition"></typeparam>
    public class Paradigm<TParadigmConfig, TParadigmDefinition, TConditionConfiguration, TConditionDefinition> : MonoBehaviour, IControlExperiment 
        where TParadigmConfig : BaseConfiguration<TParadigmConfig>
        where TParadigmDefinition : ScriptableObject, IParadigmDefinition
    {
        private const string STD_CONFIG_NAME = "StdConfigName";

        #region Constants

        private const string ParadgimConfigDirectoryName = "ParadigmConfig";

        private const string ParadigmConfigNamePattern = "VP_{0}_{1}";

        private const string DateTimeFileNameFormat = "yyyy-MM-dd_hh-mm";

        private const string DetailedDateTimeFileNameFormat = "yyyy-MM-dd_hh-mm-ss-tt";

        #endregion

        #region dependencies
        NLog.Logger appLog = NLog.LogManager.GetLogger("AppLog");
        
        public AppInit appInit;

        private IConditionController<TConditionDefinition> conditionController;
        // TODO abstract from the implementation
        public IConditionController<TConditionDefinition> ConditionController
        {
            get
            {
                return conditionController;
            }
        }
        
        public TParadigmConfig Config;

        public TParadigmDefinition InstanceDefinition;

        #endregion

        void Awake()
        {
            conditionController = GetComponentInChildren<IConditionController<TConditionDefinition>>();

            Assert.IsNotNull(conditionController);

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
            
            // marker.LogAlsoToFile = Config.logMarkerToFile;
        }

        private void First_GetTheSubjectName()
        {
            //if (SubjectID == string.Empty)
            //{
            //    if (appInit.Options.subjectId != String.Empty)
            //        SubjectID = appInit.Options.subjectId;
            //    //else
            //    //    SubjectID = ParadigmUtils.GetRandomSubjectName();
            //}

            // this is enables access to variables used by the logging framework
            //NLog.GlobalDiagnosticsContext.Set("subject_Id", SubjectID);

            //appInit.UpdateLoggingConfiguration();

           //appLog.Info(string.Format("Using Subject Id: {0}", SubjectID));
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

                    Config = ConfigUtil.LoadConfig<TParadigmConfig>(configFile, true,
                        () => Debug.LogError("Loading config failed, using default config + writing a default config"));

                    //PathToLoadedConfig = configFile.FullName;
                }
                else if (pathOfDefaultConfig.Exists)
                {
                    //appLog.Info(string.Format("Found default config at {0}", pathOfDefaultConfig.Name));

                    //Config = ConfigUtil.LoadConfig<ParadigmConfiguration>(pathOfDefaultConfig, false, () => {
                    //    appLog.Error(string.Format("Load default config at {0} failed!", pathOfDefaultConfig.Name));
                    //});

                    //PathToLoadedConfig = pathOfDefaultConfig.FullName;
                }
                else
                {
                    //Config = Config.GetDefault();

                    //var customPath = pathOfDefaultConfig;

                    //if (appInit.HasOptions && appInit.Options.fileNameOfCustomConfig != string.Empty)
                    //{
                    //    customPath = new FileInfo(Application.dataPath + Path.AltDirectorySeparatorChar + appInit.Options.fileNameOfCustomConfig);
                    //}

                    //appLog.Info(string.Format("New Config created will be saved to: {0}! Reason: No config file found!", customPath.FullName));

                    //try
                    //{
                    //    //ConfigUtil.SaveAsJson<ParadigmConfiguration>(pathOfDefaultConfig, Config);
                    //}
                    //catch (Exception e)
                    //{
                    //    //appLog.Info(string.Format("Config could not be saved to: {0}! Reason: {1}", pathOfDefaultConfig.FullName, e.Message));
                    //}

                   // PathToLoadedConfig = customPath.FullName;
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

                    //appLog.Info(logMsg);

                    //var fileContainingDefinition = new FileInfo(appInit.Options.fileNameOfParadigmDefinition);

                    //LoadInstanceDefinitionFrom(fileContainingDefinition);
                }
                else
                {
                    throw new NotImplementedException("TODO implement creation logic for definitions");

                    //UnityEngine.Debug.Log("Create instance definition.");

                    //var factory = new ParadigmModelFactory();

                    //factory.config = Config;

                    //try
                    //{
                    //    InstanceDefinition = factory.Generate(SubjectID, Config.conditionConfigurations);

                    //    Save(InstanceDefinition);
                    //}
                    //catch (Exception e)
                    //{
                    //    Debug.LogException(e, this);

                    //    appLog.Fatal(e, "Incorrect configuration! - Try to configure the paradigm in the editor!");

                    //    appLog.Fatal("Not able to create an instance definition based on the given configuration! Check the paradigm using the UnityEditor and rebuild the paradigm or change the expected configuration!");

                    //    UnityEngine.Application.Quit();
                    //}
                }
            }

        }

        private void Fourth_InitializeFirstOrDefaultCondition()
        {
            conditionController.InitializeFirstOrDefaultCondition();

        }

        private void Save(TParadigmDefinition instanceDefinition)
        {
            var fileNameWoExt = string.Format("{1}{0}PreDefinitions{0}VP_{2}_Definition", Path.AltDirectorySeparatorChar, Application.dataPath, instanceDefinition.SubjectId);

            var jsonString = JsonUtility.ToJson(InstanceDefinition, true);

            var targetFileName = fileNameWoExt + ".json";

            //appLog.Info(string.Format("Saving new definition at: {0}", targetFileName));

            using (var file = new StreamWriter(targetFileName))
            {
                file.Write(jsonString);
            }
        }

        public void StartExperimentOrNextCondition()
        {
            bool noConditionHasBeenInitialized = conditionController.IsAConditionBeenInitialized();
            //if (waitingForSignalToStartNextCondition == true)
            //    waitingForSignalToStartNextCondition = false;

            if (!conditionController.IsRunning && noConditionHasBeenInitialized)
                StartExperimentFromBeginning();

            if (!conditionController.IsRunning)
                conditionController.SetNextConditionPending();
        }

        private IEnumerator WaitForCommandToStartNextCondition()
        {
            //yield return new WaitWhile(() => waitingForSignalToStartNextCondition);

            //waitingForSignalToStartNextCondition = false; // reset

            //conditionController.SetNextConditionPending(true);

            yield return null;
        }

        private void ParadigmInstanceFinished()
        {
            //marker.Write("End Experiment");

            //appLog.Info("Paradigma run finished");
        }
         
        #region Public interface for controlling the paradigm remotely

        public void StartExperimentFromBeginning()
        {
            appLog.Info(string.Format("Run complete paradigma as defined in {0}!", InstanceDefinition.name));

            //marker.Write("Start Experiment");

            conditionController.SetNextConditionPending();

            //conditionController.StartCurrentConditionWithFirstTrial();
        }

        public void InitializeCondition(string condition)
        {
            //appLog.Info(string.Format("Condition {0} requested", condition));

            //try
            //{
            //    if (conditionController.currentCondition != null && conditionController.currentCondition.Identifier.Equals(condition))
            //    {
            //        var msg = string.Format("Requested condition '{0}' already initialized! Just start it.", condition);
            //       // appLog.Info(msg);

            //        return;
            //    }


            //    var requestedCondition = InstanceDefinition.Get(condition);

            //    conditionController.Initialize(requestedCondition);

            //}
            //catch (InvalidOperationException ioe)
            //{
            //    //appLog.Error(string.Format("Initialize condition {0} failed! {1}", condition, ioe.Message));
            //}
            //catch (ArgumentException ae)
            //{
            //    //appLog.Error(ae, "Expected Condition could not be started - maybe not implemented or has a wrong name?!");
            //}

        }

        public void SubjectTriesToSubmit()
        {
            //if (conditionController.currentTrial != null && conditionController.currentTrial.acceptsASubmit)
            //{
            //    conditionController.currentTrial.RecieveSubmit();
            //}
        }

        public void ForceABreakInstantly()
        {
            // TODO
            //conditionController.InjectPauseTrial();
            //conditionController.ResetCurrentTrial();
            //hud.ShowInstruction("Press the Submit Button to continue!\n Close your eyes and talk to the supervisor!", "Break");
        }

        public void StartExperimentWithCurrentPendingCondition()
        {
            //if (conditionController.HasConditionPending())
            //    conditionController.StartCurrentConditionWithFirstTrial();
            //else
            //    appLog.Error("Try starting a condition but none has been configured");
        }

        public bool AutoStartNextCondition = false;

        private void ConditionFinished(string conditionId)
        {
             appLog.Info(string.Format("Condition {0} has finished!", conditionId));

            if (!conditionController.PendingConditions.Any())
            {
                ParadigmInstanceFinished();
                return;
            }

            // TODO
            //if (Config.waitForCommandOnConditionEnd)
            //{
            //    appLog.Info(string.Format("Waiting for signal to start next condition...", conditionId));

            //    //hud.ShowInstruction("You made it through one part of the experiment!", "Congrats!");

            //    waitingForSignalToStartNextCondition = true;

            //    StartCoroutine(WaitForCommandToStartNextCondition());

            //    return;
            //}

            if (AutoStartNextCondition)
                conditionController.SetNextConditionPending();
        }

        /// <summary>
        /// It ends the complete experiment!
        /// </summary>
        public void ForceSaveEndOfExperiment()
        {
            // TODO
            //conditionController.PendingConditions.Clear();
            //conditionController.ForceASaveEndOfCurrentCondition();
        }

        /// <summary>
        /// It ends just the current condition, so another condition could be started.
        /// </summary>
        public void TryToPerformSaveInterruption()
        {
            throw new NotImplementedException("TODO");
            //appLog.Info("Try performing save interruption of current condition.");

            //if (conditionController.IsConditionRunning)
            //{
            //    AutoStartNextCondition = false;
            //    conditionController.ForceASaveEndOfCurrentCondition();
            //}
        }

        public void Restart(string condition = "")
        {
            if (condition == "")
            {

                var currentScene = SceneManager.GetActiveScene();

                SceneManager.LoadScene(currentScene.name, LoadSceneMode.Single);
            }
        }

        public void StartExperiment()
        {
            throw new NotImplementedException();
        }

        public void ReturnFromPause()
        {
            throw new NotImplementedException();
        }

        public void PauseTheExperiment()
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads a predefined definition from a file
        /// May override already loaded config!
        /// </summary>
        /// <param name="file"></param>
        //public void LoadInstanceDefinitionFrom(FileInfo file)
        //{
        //    using (var reader = new StreamReader(file.FullName))
        //    {
        //        var jsonFromFile = reader.ReadToEnd();

        //        var loadedDefinition = JsonUtility.FromJson<ParadigmModel>(jsonFromFile);


        //        if (InstanceDefinition == null)
        //        {
        //            appLog.Fatal(string.Format("Loading {0} as Instance Definition failed!", file.FullName));
        //            return;
        //        }

        //        InstanceDefinition = loadedDefinition;
        //        Config = InstanceDefinition.Configuration;
        //    }
        //}

        #endregion
    }

}
