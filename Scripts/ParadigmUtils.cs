using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.BeMoBI.Scripts
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
