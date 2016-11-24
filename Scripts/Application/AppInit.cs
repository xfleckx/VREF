using CommandLine;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.VREF.Scripts.Application
{
    public class AppInit : MonoBehaviour
    {
        void Awake()
        {
            var otherInstances = FindObjectsOfType<AppInit>();
            Assert.IsTrue(otherInstances.Count() == 1);
        }

        void Start()
        {
            var args = Environment.GetCommandLineArgs();

            options = new StartUpOptions();

            hasOptions = Parser.Default.ParseArguments(args, options);

            Debug.Log(string.Format("Starting with Args: {0} {1}", options.subjectId, options.fileNameOfCustomConfig));

            var currentLevelIndex = QualitySettings.GetQualityLevel();
            var allLevels = QualitySettings.names;
            var currentLevel = allLevels[currentLevelIndex];

            Debug.Log("Using quality level " + currentLevel);
        }

        private bool hasOptions;
        public bool HasOptions
        {
            get
            {
                return hasOptions;
            }
        }

        private StartUpOptions options;
        public StartUpOptions Options
        {
            get
            {
                return options;
            }
        }
    }

    /// <summary>
    /// See usage scenarios
    /// https://github.com/gsscoder/commandline/wiki/Latest-Version
    /// </summary>
    public class StartUpOptions
    {
        [Option('s', "subject", DefaultValue = "defaultSubject", Required = true, HelpText = "Subject Identification - should be a unique string!")]
        public string subjectId { get; set; }

        [Option('c', "config", DefaultValue = "", HelpText = "A file name of customn config file", Required = false)]
        public string fileNameOfCustomConfig { get; set; }

    }
}
