using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.VREF.Application;

namespace Assets.VREF.Application
{
    public class ParadigmModelFactory
    { 
        public ParadigmConfiguration config;
        
        public ParadigmModelFactory()
        {
            objectPool = UnityEngine.Object.FindObjectOfType<ObjectPool>();

            vrManager = UnityEngine.Object.FindObjectOfType<VirtualRealityManager>();

            mazeInstances = vrManager.transform.AllChildren().Where(c => c.GetComponents<beMobileMaze>() != null).Select(c => c.GetComponent<beMobileMaze>()).ToList();
        }

        public int atLeastAvailblePathsPerMaze = 0;

        public ObjectPool objectPool;
        public List<beMobileMaze> mazeInstances;
        private VirtualRealityManager vrManager; 
        
        // use stack for asserting that every category will be used once
        private Stack<Category> availableCategories;
        
        public ParadigmModel Generate(string subjectID, List<ConditionConfiguration> availableConfigurations)
        {
            var newModel = ScriptableObject.CreateInstance<ParadigmModel>();

            newModel.Subject = subjectID;

            newModel.name = string.Format("Model_VP_{0}", subjectID);

            newModel.Conditions = new List<ConditionDefinition>();

            foreach (var conditionConfig in availableConfigurations)
            {
                List<TrialConfig> possibleTrials = GetAllPossibleTrialConfigurations(conditionConfig);
                
                #region now create the actual Paradigma instance defintion by duplicating the possible configurations for trianing and experiment

                var newCondition = new ConditionDefinition();

                newCondition.Identifier = conditionConfig.ConditionID;

                newCondition.Trials = new List<TrialDefinition>();

                var trainingTrials = new List<TrialDefinition>();
                var experimentalTrials = new List<TrialDefinition>();

                foreach (var trialDefinition in possibleTrials)
                {
                    for (int i = 0; i < conditionConfig.objectVisitationsInTraining; i++)
                    {
                        var newTrainingsTrialDefinition = new TrialDefinition()
                        {
                            TrialType = typeof(Training).Name,
                            Category = trialDefinition.Category,
                            MazeName = trialDefinition.MazeName,
                            Path = trialDefinition.Path,
                            ObjectName = trialDefinition.ObjectName
                        };

                        trainingTrials.Add(newTrainingsTrialDefinition);
                    }

                    for (int i = 0; i < conditionConfig.objectVisitationsInExperiment; i++)
                    {
                        var newExperimentTrialDefinition = new TrialDefinition()
                        {
                            TrialType = typeof(Experiment).Name,
                            Category = trialDefinition.Category,
                            MazeName = trialDefinition.MazeName,
                            Path = trialDefinition.Path,
                            ObjectName = trialDefinition.ObjectName
                        };

                        experimentalTrials.Add(newExperimentTrialDefinition);

                    }
                }

                #endregion

                if (conditionConfig.groupByMazes)
                {
                    GroupTrialsPerMaze(newCondition, trainingTrials, experimentalTrials);
                }
                else
                {
                    newCondition.Trials.AddRange(trainingTrials);

                    var shuffledExperimentalTrials = experimentalTrials.Shuffle();

                    newCondition.Trials.AddRange(shuffledExperimentalTrials);
                }

                newCondition.Config = conditionConfig;
                newModel.Conditions.Add(newCondition);
            }
            GC.Collect();
            return newModel;
        }

        private List<TrialConfig> GetAllPossibleTrialConfigurations(ConditionConfiguration conditionConfig)
        {
            var shuffledCategories = objectPool.Categories.Shuffle().ToList();

            availableCategories = new Stack<Category>(shuffledCategories);

            var selectedMazes = mazeInstances.Where(m => conditionConfig.ExpectedMazes.Exists(v => v.Name.Equals(m.name)));

            var shuffledMazes = selectedMazes.Shuffle().ToList();

            var mazeCategoryMap = new Dictionary<beMobileMaze, Category>();

            foreach (var maze in shuffledMazes)
            {
                ChooseCategoryFor(maze, mazeCategoryMap);
            }

            var possibleTrials = new List<TrialConfig>();

            foreach (var association in mazeCategoryMap)
            {
                var maze = association.Key;
                var category = association.Value;

                var trialConfigs = MapPathsToObjects(maze, category, conditionConfig).ToList();
                possibleTrials.AddRange(trialConfigs);
            }

            return possibleTrials;
        }

        private static void GroupTrialsPerMaze(ConditionDefinition newCondition, List<TrialDefinition> trainingTrials, List<TrialDefinition> experimentalTrials)
        {
            var tempAllTrials = new List<TrialDefinition>();
            tempAllTrials.AddRange(trainingTrials);
            tempAllTrials.AddRange(experimentalTrials);

            var groupedByMaze = tempAllTrials.GroupBy((td) => td.MazeName);

            foreach (var group in groupedByMaze)
            {
                var groupedByPath = group.GroupBy(td => td.Path).Shuffle();

                List<TrialDefinition> trainingPerMaze = new List<TrialDefinition>();
                List<TrialDefinition> experimentPerMaze = new List<TrialDefinition>();

                foreach (var pathGroup in groupedByPath)
                {
                    var pathGroupTraining = pathGroup.Where(td => td.TrialType.Equals(typeof(Training).Name));
                    trainingPerMaze.AddRange(pathGroupTraining);

                    var pathGroupExperiment = pathGroup.Where(td => td.TrialType.Equals(typeof(Experiment).Name));
                    experimentPerMaze.AddRange(pathGroupExperiment);
                }

                var shuffledTrainingPerMaze = trainingPerMaze.Shuffle().ToList();
                var shuffledExperimentPerMaze = experimentPerMaze.Shuffle().ToList();

                newCondition.Trials.AddRange(shuffledTrainingPerMaze);
                newCondition.Trials.AddRange(shuffledExperimentPerMaze);
            }
        }

        private IEnumerable<TrialConfig> MapPathsToObjects(beMobileMaze maze, Category category, ConditionConfiguration config)
        {
            var expectedMaze = config.ExpectedMazes.Single(m => m.Name.Equals(maze.name));

            var availablePaths = maze.GetComponent<PathController>().Paths.Where(p => expectedMaze.pathIds.Exists( id => id == p.ID ));
            var shuffledPaths = availablePaths.Shuffle().ToList();

            var resultConfigs = new List<TrialConfig>();

            category.AutoResetSequence = false;

            foreach (var path in shuffledPaths)
            {
                var objectFromCategory = category.SampleWithoutReplacement();
                
                var trialConfig = new TrialConfig()
                {
                    Category = category.name,
                    MazeName = maze.name,
                    Path = path.ID,
                    ObjectName = objectFromCategory.name
                };

                resultConfigs.Add(trialConfig);
            }

            category.ResetSamplingSequence();

            return resultConfigs;
        }

        private void ChooseCategoryFor(beMobileMaze m, Dictionary<beMobileMaze, Category> mazeCategoryMap)
        {
            if (!mazeCategoryMap.ContainsKey(m))
            {
                //TODO first.. apply sample extension to categories
                mazeCategoryMap.Add(m, availableCategories.Pop());
            }
        }
        
    }


    /// <summary>
    /// A temporary configuration of values describing the configuration of a trial
    /// this is used during the generation process
    /// </summary>
    [DebuggerDisplay("{MazeName} {Path} {Category} {ObjectName}")]
    public struct TrialConfig : ICloneable
    {
        public string MazeName;
        public int Path;
        public string Category;
        public string ObjectName;

        public object Clone()
        {
            return new TrialConfig()
            {
                MazeName = this.MazeName,
                Path = this.Path,
                Category = this.Category,
                ObjectName = this.ObjectName
            };
        }
    }
}
