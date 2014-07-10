using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FritzBot.Core
{
    public static class Linq
    {
        public static IEnumerable<T> TryLogEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (T item in list)
            {
                try
                {
                    action(item);
                }
                catch (Exception ex)
                {
                    toolbox.Logging(ex);
                }
            }
            return list;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            Contract.Requires(list != null);

            foreach (T item in list)
            {
                action(item);
            }
            return list;
        }

        public static IEnumerable<T> HasAttribute<T, A>(this IEnumerable<T> source)
        {
            return source.Where(x => toolbox.GetAttribute<A>(x) != null);
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> list)
        {
            Contract.Requires(list != null);

            return list.Where(x => x != null);
        }

        public static IEnumerable<TSource> NotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            foreach (TSource item in source)
            {
                bool notNull = false;
                try
                {
                    TResult result = selector(item);
                    notNull = result != null;
                }
                catch (NullReferenceException)
                {
                }
                if (notNull)
                {
                    yield return item;
                }
            }
        }

        public static bool Contains<T>(this IEnumerable<T> list, T item, Func<T, object> bedingung)
        {
            return list.Contains(item, new KeyEqualityComparer<T>(bedingung));
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> list, Func<T, object> bedingung)
        {
            Contract.Requires(list != null);

            return list.Distinct<T>(new KeyEqualityComparer<T>(bedingung));
        }

        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> list, IEnumerable<T> second, Func<T, object> bedingung)
        {
            return list.Intersect<T>(second, new KeyEqualityComparer<T>(bedingung));
        }

        public static List<T> Steal<T>(this List<T> list, Func<T, bool> bedingung)
        {
            List<T> extracted = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (bedingung(list[i]))
                {
                    extracted.Add(list[i]);
                    list.RemoveAt(i);
                    i--;
                }
            }
            return extracted;
        }

        public static IEnumerable<T> JoinMany<T>(params IEnumerable<T>[] items)
        {
            foreach (IEnumerable<T> item in items)
            {
                foreach (T subitem in item)
                {
                    yield return subitem;
                }
            }
        }

        public static string Join(this IEnumerable<string> source, string seperator)
        {
            return String.Join(seperator, source);
        }
    }

    public static class XMLExtensions
    {
        public static T AddSingle<T>(this XContainer obj, T element)
        {
            obj.Add(element);
            return element;
        }

        public static XElement AddIfHasElements(this XElement target, XElement element)
        {
            if (element.HasElements)
            {
                target.Add(element);
            }
            return element;
        }

        public static XElement GetElementOrCreate(this XElement storage, string name)
        {
            XElement el = storage.Element(name);
            if (el == null)
            {
                el = new XElement(name);
                storage.Add(el);
            }
            return el;
        }

        public static string AttributeValueOrEmpty(this XElement element, string name)
        {
            XAttribute a = element.Attribute(name);
            if (a != null)
            {
                return element.Attribute(name).Value;
            }
            return String.Empty;
        }

        public static string ToStringWithDeclaration(this XDocument doc)
        {
            Contract.Requires(doc != null);
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new Utf8StringWriter(builder))
            {
                doc.Save(writer);
            }
            return builder.ToString();
        }

        public sealed class Utf8StringWriter : StringWriter
        {
            public Utf8StringWriter(StringBuilder builder) : base(builder) { }
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
    }

    public static class PluginExtensions
    {
        public static T As<T>(this PluginInfo info) where T : class
        {
            if (info == null)
            {
                return null;
            }
            return info.Plugin as T;
        }
    }

    public static class OtherExtensions
    {
        public static bool In<T>(this T source, params T[] values)
        {
            Contract.Requires(values != null);
            Contract.Requires(values.Length > 0);

            return values.Contains(source);
        }
    }

    public class KeyEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> comparer;
        private readonly Func<T, object> keyExtractor;

        // Allows us to simply specify the key to compare with: y => y.CustomerID
        public KeyEqualityComparer(Func<T, object> keyExtractor) : this(keyExtractor, null) { }
        // Allows us to tell if two objects are equal: (x, y) => y.CustomerID == x.CustomerID
        public KeyEqualityComparer(Func<T, T, bool> comparer) : this(null, comparer) { }

        public KeyEqualityComparer(Func<T, object> keyExtractor, Func<T, T, bool> comparer)
        {
            this.keyExtractor = keyExtractor;
            this.comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            if (comparer != null)
                return comparer(x, y);
            var valX = keyExtractor(x);
            if (valX is IEnumerable<object>) // The special case where we pass a list of keys
                return ((IEnumerable<object>)valX).SequenceEqual((IEnumerable<object>)keyExtractor(y));

            return valX.Equals(keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            if (keyExtractor == null)
                return obj.ToString().ToLower().GetHashCode();
            var val = keyExtractor(obj);
            if (val is IEnumerable<object>) // The special case where we pass a list of keys
                return (int)((IEnumerable<object>)val).Aggregate((x, y) => x.GetHashCode() ^ y.GetHashCode());

            return val.GetHashCode();
        }
    }
}