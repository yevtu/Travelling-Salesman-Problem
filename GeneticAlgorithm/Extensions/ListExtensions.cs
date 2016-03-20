using GeneticAlgorithm.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Extensions
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {         
            Int32 n = list.Count;

            while(n > 1)
            {
                --n;
                Int16 k = (Int16)RandomExtension.Instance.Next(n + 1);

                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static void ExchangeParts<T>(this IList<T> list1, IList<T> list2, int lcp, int rcp) where T : ICloneable
        {
            for(int i = lcp; i <= rcp; ++i)
            {
                T a = (T)list1[i].Clone();
                list1[i] = (T)list2[i].Clone();
                list2[i] = a;
            }
        }

        public static IList<int> GenerateRandomPositions(int count)
        {
            HashSet<int> candidates = new HashSet<int>();
            while (candidates.Count < count)
            {
                candidates.Add(RandomExtension.Instance.Next(count));
            }

            List<int> result = new List<int>();
            result.AddRange(candidates);

            return result;
        }
    }
}
