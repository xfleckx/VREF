using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VREF.Scripts.Subject
{
    /// <summary>
    /// Model of the subject.
    /// In some cases important for VR experiments.
    /// </summary>
    public class SubjectDescription : ScriptableObject
    {
        public float HeightFromFeetToEyes = 1.76f;

        public float IPD = 64f;

        public float ShoulderWidth = 1f;

        public static SubjectDescription GetDefault()
        {
            return CreateInstance<SubjectDescription>();
        }
    }
}
