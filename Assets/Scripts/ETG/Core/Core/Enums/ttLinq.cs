using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

    public static class ttLinq
    {
        public static TSource ttAggregate<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func)
        {
            TSource source1 = default (TSource);
            for (int index = 0; index < source.Count<TSource>(); ++index)
                source1 = func(source1, source.ElementAt<TSource>(index));
            return source1;
        }

        public static TAccumulate ttAggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            TAccumulate accumulate = seed;
            for (int index = 0; index < source.Count<TSource>(); ++index)
                accumulate = func(accumulate, source.ElementAt<TSource>(index));
            return accumulate;
        }

        public static TResult ttAggregate<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector)
        {
            TAccumulate accumulate = seed;
            for (int index = 0; index < source.Count<TSource>(); ++index)
                accumulate = func(accumulate, source.ElementAt<TSource>(index));
            return resultSelector(accumulate);
        }

        public static TSource ttLast<TSource>(this IEnumerable<TSource> source)
        {
            int index = source.Count<TSource>() - 1;
            return source.ElementAt<TSource>(index);
        }

        public static List<TSource> ttOrderBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : IComparable
        {
            TSource[] array = source.ToArray<TSource>();
            Array.Sort<TKey, TSource>(((IEnumerable<TSource>) array).Select<TSource, TKey>(keySelector).ToArray<TKey>(), array);
            return ((IEnumerable<TSource>) array).ToList<TSource>();
        }

        public static List<TSource> ttThenBy<TSource, TKey>(
            this IOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.CreateOrderedEnumerable<TKey>(keySelector, (IComparer<TKey>) Comparer<TKey>.Default, false).ToList<TSource>();
        }

        public static List<TSource> ttOrderByDescending<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : IComparable
        {
            TSource[] array = source.ToArray<TSource>();
            Array.Sort<TKey, TSource>(((IEnumerable<TSource>) array).Select<TSource, TKey>(keySelector).ToArray<TKey>(), array);
            return ((IEnumerable<TSource>) array).Reverse<TSource>().ToList<TSource>();
        }

        public static TResult[] ttSelect<TSource, TResult>(
            this IList<TSource> source,
            Func<TSource, TResult> selector)
        {
            TResult[] resultArray = new TResult[source.Count];
            for (int index = 0; index < source.Count; ++index)
                resultArray[index] = selector(source[index]);
            return resultArray;
        }

        public static int ttSum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> func)
        {
            int num = 0;
            for (int index = 0; index < source.Count<TSource>(); ++index)
                num += func(source.ElementAt<TSource>(index));
            return num;
        }

        public static float ttSum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> func)
        {
            float num = 0.0f;
            for (int index = 0; index < source.Count<TSource>(); ++index)
                num += func(source.ElementAt<TSource>(index));
            return num;
        }

        public static double ttSum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> func)
        {
            double num = 0.0;
            for (int index = 0; index < source.Count<TSource>(); ++index)
                num += func(source.ElementAt<TSource>(index));
            return num;
        }

        public static List<TSource> ttWhere<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> func)
        {
            List<TSource> sourceList = new List<TSource>();
            foreach (TSource source1 in source)
            {
                if (func(source1))
                    sourceList.Add(source1);
            }
            return sourceList;
        }
    }

