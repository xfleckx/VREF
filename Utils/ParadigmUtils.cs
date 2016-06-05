using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.VREF.Utils
{
    public static class ParadigmUtils
    {
        public static string GetRandomSubjectName()
        {
            var result = string.Empty;

            var guid = Guid.NewGuid();

            result = guid.ToString();

            return result;
        }

    }
}
