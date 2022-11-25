﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Extensions
{
    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static void TryAddOrUpdateOnTagList(this IList<string> smgtags, IList<string> tagsToAdd)
        {
            if (smgtags == null)
                smgtags = new List<string>();

            foreach (var tag in smgtags)
            {
                tagsToAdd.TryAddOrUpdateOnTagList(tag);
            }
        }

        public static void TryAddOrUpdateOnTagList(this IList<string> smgtags, string tagToAdd)
        {
            if (smgtags == null)
                smgtags = new List<string>();


            if (!smgtags.Contains(tagToAdd))
                smgtags.Add(tagToAdd);
        }
    }
}