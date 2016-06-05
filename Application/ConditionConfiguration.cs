using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.VREF.Interfaces;

namespace Assets.VREF.Application
{
    [Serializable]
    public class ConditionConfiguration : ICloneable
    {
        public static ConditionConfiguration GetDefault()
        {
            var config = new ConditionConfiguration();

            config.ConditionID = ParadigmConfiguration.NAME_FOR_DEFAULT_CONFIG;
             
            return config;
        }

        public object Clone()
        {
            var originalProperties = this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var clone = new ConditionConfiguration();
            var cloneProperties = clone.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            for (int i = 0; i < originalProperties.Length; i++)
            {
                var originalValue = originalProperties[i].GetValue(this, null);
                cloneProperties[i].SetValue(clone, originalValue, null);
            }

            var originalFields = this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            var cloneFields = clone.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            for (int i = 0; i < originalFields.Length; i++)
            {
                var originalFieldValue = originalFields[i].GetValue(this);
                cloneFields[i].SetValue(clone, originalFieldValue);
            }
            
            return clone;
        }

        [SerializeField]
        public string ConditionID = ParadigmConfiguration.NAME_FOR_DEFAULT_CONFIG;
        
        [SerializeField]
        public bool useTeleportation = false;

        [SerializeField]
        public string BodyControllerName = "KeyboardCombi";

        [SerializeField]
        public string HeadControllerName = "KeyboardCombi";

        [SerializeField]
        public float TimeToDisplayObjectToRememberInSeconds = 3;

        [SerializeField]
        public float TimeToDisplayObjectWhenFoundInSeconds = 2;

        [SerializeField]
        public float offsetToTeleportation = 2;

        [SerializeField]
        public int categoriesPerMaze = 1;
        
        [SerializeField]
        public int objectVisitationsInTraining = 1; // how often an object should be visisted while trainings trial

        [SerializeField]
        public int objectVisitationsInExperiment = 1; // " while Experiment

        [SerializeField]
        public bool useExactOnCategoryPerMaze = true;
        
        [SerializeField]
        public bool groupByMazes = true;

        [SerializeField]
        public bool UseShortWayBack = false;

        [SerializeField]
        public bool UseMonoscopicViewOnVRHeadset = false;

        [SerializeField]
        public List<ExpectedMazeWithPaths> ExpectedMazes = new List<ExpectedMazeWithPaths>();
    }


    [Serializable]
    public class ExpectedMazeWithPaths : ICloneable  
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public List<int> pathIds;

        public object Clone()
        {
            return new ExpectedMazeWithPaths()
            {
                Name = this.Name,
                pathIds = new List<int>(this.pathIds)
            };
        }
    }
}