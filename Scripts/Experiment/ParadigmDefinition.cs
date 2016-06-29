using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VREF.Scripts.BaseParadigm
{
    public interface IParadigmDefinition
    {
       string SubjectId { get; }
    }

    public abstract class ParadigmDefinition<TParadigmConfiguration, TConditionDefinition> : IParadigmDefinition
        where TConditionDefinition : IConditionDefinition
    {
        protected string subjectId;

        public TParadigmConfiguration Configuration;

        [SerializeField]
        public List<TConditionDefinition> Conditions;

        public string SubjectId
        {
            get
            {
                return subjectId;
            }
        }

        public TConditionDefinition Get(string condition)
        {
            if (Conditions.Any(c => c.Identifier.SequenceEqual(condition)))
            {
                return Conditions.Where(c => c.Identifier.Contains(condition)).FirstOrDefault();
            }

            throw new ArgumentException(string.Format("Expected condition '{0}' not found in paradigm model!", condition));
        }
    }
}
