using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VREF.Application
{
    [Serializable]
    public class ParadigmConfiguration : ScriptableObject
    {
        public const string NAME_FOR_DEFAULT_CONFIG = "default";

        public static ParadigmConfiguration GetDefault()
        {
            var config = CreateInstance<ParadigmConfiguration>();

            config.conditionConfigurations.Add(ConditionConfiguration.GetDefault());

            config.hideFlags = HideFlags.HideAndDontSave;

            return config;
        }

        [SerializeField]
        public bool ifNoInstanceDefinitionCreateOne = true;

        [SerializeField]
        public bool writeStatistics = true;

        [SerializeField]
        public bool logMarkerToFile = true;

        [SerializeField]
        public string nameOfRigidBodyDefinition = "";

        [SerializeField]
        public bool waitForCommandOnConditionEnd = true;
        
        [SerializeField]
        public List<ConditionConfiguration> conditionConfigurations = new List<ConditionConfiguration>();
    }
}
