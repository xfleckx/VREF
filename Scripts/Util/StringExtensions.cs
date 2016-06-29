using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.VREF.Scripts
{
    public static class StringExtensions
    {
        /// <summary>
        /// Usefull when getting strings from network messages. When the stirng is smaller than the buffer it may contain a lot of zero bytes.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>the same string without zero bytes</returns>
        public static string RemoveZeroBytes(this String input)
        {
            return input.Replace("\0", String.Empty);
        }
    }
}
