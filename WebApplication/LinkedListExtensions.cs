using System.Collections.Generic;

namespace WebApplication
{
    public static class LinkedListExtensions   
    {
        public static void AppendRange<T>(this LinkedList<T> source,
            IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                source.AddLast(item);
            }
        }

        public static void PrependRange<T>(this LinkedList<T> source,
            IEnumerable<T> items)
        {
            LinkedListNode<T> first = source.First;
            // If the list is empty, we can just append everything.
            if (first is null)
            {
                AppendRange(source, items);
                return;
            }

            // Otherwise, add each item in turn just before the original first item
            foreach (T item in items)
            {
                source.AddBefore(first, item);
            }
        }
    }
}