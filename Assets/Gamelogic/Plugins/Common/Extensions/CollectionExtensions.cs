using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Gamelogic
{
	/// <summary>
	/// This class provides useful extension methods for collections, mostly IEnumerable.
	/// </summary>
	[Version(1)]
	public static class CollectionExtensions
	{
		/// <summary>
		/// Returns all elements of the source which are of FilterType.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TFilter"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<TFilter> FilterByType<T, TFilter>(this IEnumerable<T> source)
			where T : class
			where TFilter : class, T
		{
			return source.Where(item => item as TFilter != null).Cast<TFilter>();
		}

		/// <summary>
		/// Removes all the elements in the list that does not satisfy the predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="match"></param>
		public static void RemoveAllBut<T>(this List<T> source, Predicate<T> match)
		{
			Predicate<T> nomatch = item => !match(item);

			source.RemoveAll(nomatch);
		}

		/// <summary>
		/// Returns whether this source is empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static bool IsEmpty<T>(this ICollection<T> collection)
		{
			return collection.Count == 0;
		}
		
		/// <summary>
		/// Add all elements of other to the given source.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="other"></param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
		{
			if (other == null)//nothing to add
			{
				return;
			}

			foreach (var obj in other)
			{
				collection.Add(obj);
			}
		}

		/// <summary>
		/// Returns a pretty string representation of the given list. The resulting string looks something like
		/// <c>{a, b, c}</c>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ListToString<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				return "null";
			}

			if (!source.Any())
			{
				return "[]";
			}

			if (source.Count() == 1)
			{
				return "[" + source.First() + "]";
			}

			var s = "";

			s += source.ButFirst().Aggregate(s, (res, x) => res + ", " + x.ListToString());
			s = "[" + source.First().ListToString() + s + "]";

			return s;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[Version(1, 4)]
		private static string ListToString(this object obj)
		{
			var objAsString = obj as string;

			if (objAsString != null) return objAsString;

			var objAsList = obj as IEnumerable;

			return objAsList == null ? obj.ToString() : objAsList.Cast<object>().ListToString();
		}

		/// <summary>
		/// Returns an enumerable of all elements of the given list	but the first,
		/// keeping them in order.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> ButFirst<T>(this IEnumerable<T> source)
		{
			return source.Skip(1);
		}

		/// <summary>
		/// Returns an enumarable of all elements in the given 
		/// list but the last, keeping them in order.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> ButLast<T>(this IEnumerable<T> source)
		{
			var lastX = default(T);
			var first = true;

			foreach (var x in source)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					yield return lastX;
				}

				lastX = x;
			}
		}

		/// <summary>
		/// Finds the maximum element in the source as scored by the given function.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="score"></param>
		/// <returns></returns>
		public static T MaxBy<T>(this IEnumerable<T> source, Func<T, IComparable> score)
		{
			return source.Aggregate((x, y) => score(x).CompareTo(score(y)) > 0 ? x : y);
		}

		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
	Func<TSource, TKey> selector)
		{
			return source.MinBy(selector, Comparer<TKey>.Default);
		}

		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			source.ThrowIfNull("source");
			selector.ThrowIfNull("selector");
			comparer.ThrowIfNull("comparer");
			
			using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence was empty");
				}

				TSource min = sourceIterator.Current;
				TKey minKey = selector(min);
				
				while (sourceIterator.MoveNext())
				{
					TSource candidate = sourceIterator.Current;
					TKey candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, minKey) < 0)
					{
						min = candidate;
						minKey = candidateProjected;
					}
				}
				return min;
			}
		}

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
	Func<TSource, TKey> selector)
		{
			return source.MaxBy(selector, Comparer<TKey>.Default);
		}

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			source.ThrowIfNull("source");
			selector.ThrowIfNull("selector");
			comparer.ThrowIfNull("comparer");

			using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence was empty");
				}

				TSource max = sourceIterator.Current;
				TKey maxKey = selector(max);

				while (sourceIterator.MoveNext())
				{
					TSource candidate = sourceIterator.Current;
					TKey candidateProjected = selector(candidate);
					
					if (comparer.Compare(candidateProjected, maxKey) > 0)
					{
						max = candidate;
						maxKey = candidateProjected;
					}
				}

				return max;
			}
		}

		//TODO consider changing left to something more universal
		/// <summary>
		/// Returns a enumerable with elements in order, but the first element is moved to the end.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> RotateLeft<T>(this IEnumerable<T> source)
		{
			var enumeratedList = source as IList<T> ?? source.ToList();
			return enumeratedList.ButFirst().Concat(enumeratedList.Take(1));
		}

		/// <summary>
		/// Returns a enumerable with elements in order, but the last element is moved to the front.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> source)
		{
			var enumeratedList = source as IList<T> ?? source.ToList();
			yield return enumeratedList.Last();

			foreach (var item in enumeratedList.ButLast())
			{
				yield return item;
			}
		}

		/// <summary>
		/// Returns a random element from the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T RandomItem<T>(this IEnumerable<T> source)
		{
			return source.SampleRandom(1).First();
		}

		/// <summary>
		/// Returns a random sample from the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="sampleCount"></param>
		/// <returns></returns>
		public static IEnumerable<T> SampleRandom<T>(this IEnumerable<T> source, int sampleCount)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (sampleCount < 0)
			{
				throw new ArgumentOutOfRangeException("sampleCount");
			}

			/* Reservoir sampling. */
			var samples = new List<T>();

			//Must be 1, otherwise we have to use Range(0, i + 1)
			var i = 1;

			foreach (var item in source)
			{
				if (i <= sampleCount)
				{
					samples.Add(item);
				}
				else
				{
					// Randomly replace elements in the reservoir with a decreasing probability.
					var r = GLRandom.Range(i);

					if (r < sampleCount)
					{
						samples[r] = item;
					}
				}

				i++;
			}

			return samples;
		}
		
		/// <summary>
		/// Shuffles a list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		public static void Shuffle<T>(this IList<T> source)  
		{  
		    var n = source.Count;  
		    
			while (n > 1) 
			{  
		        n--;  
		        var k = Random.Range(0, n + 1);  
		        var value = source[k];  
		        source[k] = source[n];  
		        source[n] = value;  
		    }  
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		[Version(1, 2)]
		public static IEnumerable<T> TakeHalf<T>(this IEnumerable<T> source)
		{
			int count = source.Count();

			return source.Take(count/2);
		}

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		/// <typeparam name="TCollection"></typeparam>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="source"></param>
		/// <param name="value"></param>
		/// <param name="getSubElement"></param>
		/// <returns></returns>
		public static int BinarySearch<TCollection, TElement>(this ICollection<TCollection> source, TElement value, Func<TCollection, TElement> getSubElement)
		{
			return BinarySearch(source, value, getSubElement, 0, source.Count, null);
		}

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		/// <typeparam name="TCollection"></typeparam>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="source"></param>
		/// <param name="value"></param>
		/// <param name="getSubElement"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int BinarySearch<TCollection, TElement>(this ICollection<TCollection> source, TElement value, Func<TCollection, TElement> getSubElement, IComparer<TElement> comparer)
		{
			return BinarySearch(source, value, getSubElement, 0, source.Count, comparer);
		}

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		/// <typeparam name="TCollection"></typeparam>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="source"></param>
		/// <param name="value"></param>
		/// <param name="getSubElement"></param>
		/// <param name="index"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static int BinarySearch<TCollection, TElement>(this ICollection<TCollection> source, TElement value, Func<TCollection, TElement> getSubElement, int index, int length)
		{
			return BinarySearch(source, value, getSubElement, index, length, null);
		}

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		/// <typeparam name="TCollection"></typeparam>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="source"></param>
		/// <param name="value"></param>
		/// <param name="getSubElement"></param>
		/// <param name="index"></param>
		/// <param name="length"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int BinarySearch<TCollection, TElement>(this ICollection<TCollection> source, TElement value, Func<TCollection, TElement> getSubElement, int index, int length, IComparer<TElement> comparer)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", 
					"index is less than the lower bound of array.");
			}

			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", 
					"Value has to be >= 0.");
			}

			// re-ordered to avoid possible integer overflow
			if (index > source.Count - length)
			{
				throw new ArgumentException(
					"index and length do not specify a valid range in array.");
			}
			if (comparer == null)
			{
				comparer = Comparer<TElement>.Default;
			}

			int min = index;
			int max = index + length - 1;
			int cmp;
			int mid;

			while (min <= max)
			{
				mid = (min + ((max - min) >> 1));

				cmp = comparer.Compare(
					getSubElement(source.ElementAt(mid)), value);

				if (cmp == 0) return mid;

				if (cmp > 0)
				{
					max = mid - 1;
				}
				else
				{
					min = mid + 1;
				}
			}

			return ~min;
		}

		[Version(1, 3)]
		public static bool AreSequencesEqual<T>(IEnumerable<T> s1, IEnumerable<T> s2)
			where T:IComparable
		{
			ObjectExtensions.ThrowIfNull(s1, "s1");
			ObjectExtensions.ThrowIfNull(s2, "s2");

			var list1 = s1.ToList();
			var list2 = s2.ToList();

			if (list1.Count != list2.Count) return false;

			for (int i = 0; i < list1.Count; i++)
			{
				if (list1[i].CompareTo(list2[i]) != 0) return false;
			}

			return true;
		}
	}
}