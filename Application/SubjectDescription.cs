using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.VREF.Application
{
    [Serializable]
    public class SubjectDescription
    {
        public float HeightFromFeetToEyes = 1.76f;

        public float IPD = 67.8f;

        public static SubjectDescription GetDefault()
        {
            return new SubjectDescription();
        }
    }
}
