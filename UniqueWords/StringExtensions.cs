using System;
using System.Linq;

namespace UniqueWords
{
    public static class StringExtensions
    {
        public static string Trim(this string str, Func<char, bool> func) =>
            str.Where(c => !func(c))
                .Aggregate("", (current, c) => current + c);
    }
}