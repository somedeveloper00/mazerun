using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MazeRun.Utils {
    public static class LinqExtensions {

        [CanBeNull]
        public static T Random<T>(this IEnumerable<T> enumerable) {
            var lenum = enumerable as List<T> ?? enumerable.ToList();
            return lenum[UnityEngine.Random.Range( 0, lenum.Count )];
        }

        public static bool Any<T>(this IEnumerable<T> enumerable, Func<T, int, bool> predicate) {
            var i = 0;
            foreach (var item in enumerable) {
                if (predicate( item, i )) return true;
                i++;
            }
            return false;
        }
    }
}