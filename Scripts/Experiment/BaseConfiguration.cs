using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VREF.Scripts.BaseParadigm
{
    public abstract class BaseConfiguration<TConfig> : ScriptableObject
    {
        public abstract TConfig GetDefault();
    }
}
