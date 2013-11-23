using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wp8.Framework.Utils.Utils;

namespace Wp8.Framework.Utils.Collections
{
    public static class Extensions
    {
        /// <summary>
        /// Examines each item in the double collection 
        /// and returns the index of the greatest value.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static int GetIndexOfGreatest(this IEnumerable<double> values)
        {
            var enumerable = values as IList<double> ?? values.ToList();
            ArgumentValidator.AssertNotNull(enumerable, "values");

            var result = -1;
            int count = 0;
            double highest = -1;
            foreach (var d in enumerable)
            {
                if (d > highest)
                {
                    highest = d;
                    result = count;
                }
                count++;
            }
            return result;
        }

        /// <summary>
        /// Removes all items from the list that do not pass the filter condition.
        /// </summary>
        /// <typeparam name="T">The generic type of the list.</typeparam>
        /// <param name="list">The list to remove from.</param>
        /// <param name="filter">The filter to evaluate each item with.</param>
        /// <returns>The removed items.</returns>
        public static IEnumerable<T> RemoveAllAndReturnItems<T>(this IList<T> list, Func<T, bool> filter)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (filter(list[i]))
                {
                    var item = list[i];
                    list.Remove(list[i]);
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Adds the items from one set of items to the specified list.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="toList">The destination list.</param>
        /// <param name="fromList">The source list.</param>
        public static void AddRange<T>(this IList<T> toList, IEnumerable<T> fromList)
        {
            ArgumentValidator.AssertNotNull(toList, "toList");
            var enumerable = fromList as IList<T> ?? fromList.ToList();
            ArgumentValidator.AssertNotNull(enumerable, "fromList");

            foreach (var item in enumerable)
            {
                toList.Add(item);
            }
        }

        /// <summary>
        /// Adds the items from one set of items to the specified list.
        /// </summary>
        /// <param name="toList">The destination list.</param>
        /// <param name="fromList">The source list.</param>
        public static void AddRange(this IList toList, IEnumerable fromList)
        {
            ArgumentValidator.AssertNotNull(toList, "toList");
            var enumerable = fromList as IList<object> ?? fromList.Cast<object>().ToList();
            ArgumentValidator.AssertNotNull(enumerable, "fromList");

            foreach (var item in enumerable)
            {
                toList.Add(item);
            }
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        /// 	<c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return enumerable.Any();
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="collection">The collection, which may be null or empty.</param>
        /// <returns>
        /// 	<c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection == null)
            {
                return true;
            }
            return collection.Count < 1;
        }
    }
}
