using System;
using System.Linq;

namespace UniqueWordsLibrary
{
    public static class StringExtensions
    {
        public static string Trim(this string str, Func<char, bool> func) =>
            str.Where(c => !func(c))
                .Aggregate("", (current, c) => current + c);
    }
}