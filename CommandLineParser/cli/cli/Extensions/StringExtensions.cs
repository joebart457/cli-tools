using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli.Extensions
{
    internal static class StringExtensions
    {
        public static string? Nest(this string src, int nestingLevel, char nestingChar = '\t')
        {
            if (src == null) return null;
            return $"{new string(nestingChar, nestingLevel)}{src}";
        }

        public static Ty ToType<Ty>(this string src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            try
            {
                return (Ty)Convert.ChangeType(src, typeof(Ty));
            }
            catch (Exception)
            {
                throw new ArgumentException($"unable to convert value {src} to type {typeof(Ty).FullName}");
            }
        }
    }
}
