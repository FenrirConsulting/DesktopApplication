
using System.Collections.Generic;
using System.Windows;

namespace IAMHeimdall.Resources.Commands
{
    public class SmallestMissingNumber
    {
        #region Functions
        public static int FindSmallestMissingNumber(int[] A, int rangeStart, int rangeEnd)
        {
            // the minimum possible answer is 1
            int result = rangeStart;
            // let's keep track of what we find
            Dictionary<int, bool> found = new();

            // loop through the given array  
            for (int i = 0; i < A.Length; i++)
            {
                // if we have a positive integer that we haven't found before
                if (A[i] >= result && A[i] < rangeEnd && !found.ContainsKey(A[i]))
                {
                    // record the fact that we found it
                    found.Add(A[i], true);
                }
            }

            // crawl through what we found starting at 1
            while (found.ContainsKey(result))
            {
                // look for the next number
                result++;
            }

            // return the smallest positive number that we couldn't find.
            return result;
        }
        #endregion
    }
}
