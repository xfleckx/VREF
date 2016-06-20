using System;
using UnityEngine;

namespace Assets.VREF.Scripts.BaseParadigm
{
    [Serializable]
    public abstract class BaseParadigmDefinition : ScriptableObject, IParadigmDefinition
    {
        [SerializeField]
        protected string subjectId = "";
        public string SubjectId
        {
            get
            {
                return subjectId;
            }
        }
    }
}
