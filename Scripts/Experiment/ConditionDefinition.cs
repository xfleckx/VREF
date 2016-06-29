using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VREF.Scripts.BaseParadigm
{
    public interface IConditionDefinition
    {
        string Identifier { get; set; }
    }
    
    [Serializable]
    public class ConditionDefinition<TConditionConfiguration, TTrialDefinition> : IConditionDefinition
    {
        [SerializeField]
        private string identifier = "default";

        [SerializeField]
        public TConditionConfiguration Config;

        [SerializeField]
        public List<TTrialDefinition> Trials;

        string IConditionDefinition.Identifier
        {
            get
            {
                return identifier;
            }

            set
            {
                identifier = value;
            }
        }
    }

}
