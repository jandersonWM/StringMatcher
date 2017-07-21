using System;
using System.Collections.Generic;
using System.Linq;

namespace StringMatcher
{
    public static class StringMatcher
    {
        public static int ComputeLevDistance(string source, string target)
        {
            //Source of logic: https://people.cs.pitt.edu/~kirk/cs1501/Pruhs/Spring2006/assignments/editdistance/Levenshtein%20Distance.htm
            int n = source.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];

            //shortcut to save time - if one is empty the difference is obviously the length of the other
            if (n == 0) { return m; }
            if (m == 0) { return n; }

            //fill array with 2 vectors - one of source length and one of target length
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            //loop over the source
            for (int i = 1; i <= n; i++)
            {
                //loop through each letter in target to compare with source
                for (int j = 1; j <= m; j++)
                {
                    //calculate cost, either 0 (letters match) or 1 (they don't match)
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            //return final cell in array
            return d[n, m];
        }

        public static string FindBestMatch(string source, List<string> targets)
        {
            int bestMatch = source.Length; string bestMatchStr = "";
            for (var i = 0; i < targets.Count(); i++)
            {
                //Compute how similar the entity is to the matched race
                int thisMatch = ComputeLevDistance(source, targets[i]);
                //if thismatch is a better match, set the bestmatch str and int values
                if (bestMatch > thisMatch) { bestMatch = thisMatch; bestMatchStr = targets[i]; }
                //If the distance is 0, the strings are identical and we've found our best match
                if (bestMatch == 0) break;
            }
            return bestMatchStr;
        }

        public static List<string> FindAllMatches(string location, List<string> targets, int? difference = 2)
        {
            List<string> returnMatches = new List<string>();
            foreach (var target in targets)
            {
                IEnumerable<string> substrings;
                string longestSub = string.Empty;
                if (target.Length <= location.Length)
                {
                    substrings = getSubstrings(target);
                    foreach (string sub in substrings)
                    {
                        if (location.Contains(sub)) longestSub = sub; break;
                    }
                }
                else
                {
                    substrings = getSubstrings(location);
                    foreach (string sub in substrings)
                    {
                        if (target.Contains(sub)) { longestSub = sub; break; }
                    }
                }
                if (ComputeLevDistance(location, longestSub) < difference) returnMatches.Add(target);
            }
            return returnMatches;
        }

        public static IEnumerable<string> getSubstrings(string source)
        {
            return from firstChar in Enumerable.Range(0, source.Length)
                   from secondChar in Enumerable.Range(0, source.Length - firstChar + 1)
                   where secondChar > 0
                   let myString = source.Substring(firstChar, secondChar)
                   orderby myString.Length descending
                   select source.Substring(firstChar, secondChar);
        }
    }
}