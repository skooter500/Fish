using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Diagnostics;
using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// Combinatorial algorithms, including generating tuples, combinations, permutations and
	/// partitions.
	/// 
	/// Except for PowerSet, all methods are implemented after Knuth, described in The Art of 
	/// Computer Programming Volume 4.
	/// </summary>
	[Version(1, 4)]
	public static class Combinatorial
	{
		#region Tuples
		/// <summary>
		/// Generate all possible tuplets of length n with digits 0 to n-1.
		/// </summary>
		/// <param name="n">The length of tuplets to geneate. All digits are also between 0 and n-1.</param>
		/// <returns></returns>
		public static IEnumerable<int[]> Tuples(int n)
		{
			if (n < 0) throw new ArgumentOutOfRangeException("n", n, "must be non negative");

			var radices = RangeArray(n);

			return MultiRadixTuplesImpl(radices, e => e);
		}

		/// <summary>
		/// Generares all tuples with mixed radices.
		/// </summary>
		/// <param name="radices">The array of radices for each position in the tuple.</param>
		/// <returns></returns>
		public static IEnumerable<int[]> MultiRadixTuples(int[] radices)
		{
			radices.ThrowIfNull("radices");

			return MultiRadixTuplesImpl(radices, e => e);
		}

		public static IEnumerable<T[]> MultiRadixTuples<T>(this IEnumerable<IEnumerable<T>> elements)
		{
			var elementArray = elements.Select(l => (IList<T>)l.ToArray()).ToArray();
			var indices = MultiRadixTuples(elementArray.Select(l => l.Count));

			return indices.Select(index => AccessByIndex(elementArray, index));
		}

		//This is really n of n combinations
		public static IEnumerable<T[]> Tuples<T>(this IEnumerable<T> list)
		{
			list.ThrowIfNull("list");

			var objList = list.ToArray();
			var indices = Tuples(objList.Length);

			return indices.Select(index => index.Select(i => objList[i]));
		}

		//n of n combinations with selector
		public static IEnumerable<T> Tuples<T>(int n, Func<int[], T> select)
		{
			if (n < 0) throw new ArgumentOutOfRangeException("n", n, "must be non negative");
		
			select.ThrowIfNull("select");
			var radices = new int[n].Init(n);

			return MultiRadixTuplesImpl(radices, select);
		}
		#endregion

		#region Permutations

		/// <summary>
		/// Generates all permuations of the numbers 0 to n - 1.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static IEnumerable<int[]> Permutations(int n)
		{
			if (n < 0) throw new ArgumentOutOfRangeException("n", n, "must be non negative");

			return PermutationsImpl(n, e => e);
		}



		/// <summary>
		/// Gehenrates all permuations of the list of elements.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static IEnumerable<T[]> Permutations<T>(this IEnumerable<T> list)
		{
			list.ThrowIfNull("list");
		
			var items = list.ToArray();
			var permuationIndices = Permutations(items.Length);

			return permuationIndices.Select(
				permuationIndex => permuationIndex.Select(i => items[i]));
		}
		#endregion

		#region Partitions
		public static IEnumerable<IEnumerable<IEnumerable<T>>> Partitions<T>(this IEnumerable<T> list)
		{
			var listArray = list.ToArray();
			int elementCount = listArray.Length;
			var indices = RestrictedGrowthStrings(elementCount);

			return indices.Select(index => AccessByRestrictedGrowthString(listArray, index));
		}
		#endregion

		#region Combinations
		public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> list, int m)
		{
			list.ThrowIfNull("list");

			var listArray = list.ToArray();

			if (m < 0) throw new ArgumentOutOfRangeException("m", m, "must be nonnegative");
			if (m > listArray.Length) throw new ArgumentOutOfRangeException("m", m, "must be smaller than n");

			var combinationIndices = CombinationsImpl(m, listArray.Length, e => e);

			return combinationIndices.Select(
				combinationIndex => combinationIndex.Select(i => listArray[i]));
		}

		public static IEnumerable<T> Combinations<T>(int m, int n, Func<int[], T> select)
		{
			if (m < 0) throw new ArgumentOutOfRangeException("m", m, "must be nonnegative");
			if(n < 0) throw new ArgumentOutOfRangeException("n", n, "must be nonnegative");
			if (m > n) throw new ArgumentOutOfRangeException("m", m, "must be smaller than n");

			select.ThrowIfNull("select");

			return CombinationsImpl(m, n, select);
		}

		#endregion

		#region PowerSet
		public static IEnumerable<IEnumerable<T>> PowerSet<T>(this IEnumerable<T> input)
		{
			input.ThrowIfNull("input");

			return PowerSetImpl(input.ToList());
		}
		#endregion

		#region Implementation
		public static IEnumerable<T> CombinationsImpl<T>(int m, int n, Func<int[], T> select)
		{
			var combination = RangeArray(m + 2);

			combination[m] = n;
			combination[m + 1] = 0;

			while (true)
			{
				yield return select(combination.Take(m));
				int j = 0;

				while (combination[j] + 1 == combination[j + 1])
				{
					combination[j] = j;
					j++;

					if (j >= m) yield break;
				}

				combination[j]++;
			}
		}

		private static IEnumerable<T> MultiRadixTuplesImpl<T>(IList<int> radices, Func<int[], T> select)
		{
			var tuple = new int[radices.Count].Init(0);

			while (true)
			{
				yield return select(tuple.Copy());

				int currentIndex = radices.Count - 1;
				tuple[currentIndex]++;

				while (tuple[currentIndex] >= radices[currentIndex]) //while we overflow, we carry over
				{
					currentIndex--;

					if (currentIndex < 0) yield break;

					tuple[currentIndex]++;
					tuple[currentIndex + 1] = 0;
				}
			}
		}

		private static IEnumerable<T> PermutationsImpl<T>(int n, Func<int[], T> select)
		{
			if (n == 0)
			{
				yield break;
			}

			var permuation = RangeArray(n);

			if (n == 1)
			{
				yield return select(permuation.Copy());	
				yield break;
			}

			while (true)
			{
				int index = n - 2;

				GLDebug.Assert(index >= 0, "index must be greater than or equal to 0");
				GLDebug.Assert(index + 1 < permuation.Length, "index + 1 must be smaller than permution length, but index == " + index);

				while (permuation[index] >= permuation[index + 1]) //while we overflow, we carry over
				{
					index--;

					if (index < 0) yield break;
				}

				// Find the right most index where the value
				// is smaller than at the current index 
				int index2 = n - 1;

				while (permuation[index] >= permuation[index2])
				{
					index2--;
				}

				Swap(permuation, index, index2);

				ReverseRange(permuation, index + 1, n);

				yield return select(permuation.Copy());	
			}
		}


		//P417
		public static IEnumerable<int[]> RestrictedGrowthStrings(int n)
		{
			//H1
			var a = new int[n].Init(0);
			var b = new int[n - 1].Init(1);

			int m = 1;

			while (true)
			{
				//H2
				yield return a;

				if (a[n - 1] == m)
				{
					//H4
					int j = n - 2;

					while (a[j] == b[j])
					{
						j--;
					}

					//H5
					if (j == 0) yield break;
		
					a[j]++;

					//H6
					m = b[j] + ((a[j] == b[j]) ? 1 : 0);
					j++;

					while (j < n - 1)
					{
						a[j] = 0;
						b[j] = m;
						j++;
					}

					a[n - 1] = 0;
				}
				else
				{
					//H3
					a[n - 1]++;
				}
			}
		}
	
		//indices[m][n] means object at m should be in set n in the result
		private static IEnumerable<IEnumerable<T>> AccessByRestrictedGrowthString<T>(IList<T> values, IList<int> indices)
		{
			var setCount = indices.Max() + 1;
			var sets = new IList<T>[setCount];

			for (int i = 0; i < setCount; i++)
			{
				sets[i] = new List<T>();
			}

			for (int i = 0; i < indices.Count; i++)
			{
				sets[indices[i]].Add(values[i]);
			}

			return sets;
		}

		//indices
		private static T[] AccessByIndex<T>(IList<IList<T>> values, IList<int> indices)
		{
			var set = new T[values.Count];

			for (int i = 0; i < values.Count; i++)
			{
				set[i] = values[i][indices[i]];
			}

			return set;
		}

		//From http://stackoverflow.com/questions/19890781/creating-a-power-set-of-a-sequence
		private static IEnumerable<IEnumerable<T>> PowerSetImpl<T>(IList<T> input)
		{
			int n = input.Count;

			// Power set contains 2^N subsets.
			int powerSetCount = 1 << n;

			for (int setMask = 0; setMask < powerSetCount; setMask++)
			{
				var s = new List<T>();

				for (int i = 0; i < n; i++)
				{
					// Checking whether i'th element of input collection should go to the current subset.
					if ((setMask & (1 << i)) > 0)
					{
						s.Add(input[i]);
					}
				}

				yield return s;
			}
		}
		#endregion

		#region Array Extensions
		private static T[] Init<T>(this T[] array, T item)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = item;
			}

			return array;
		}

		public static T[] Take<T>(this T[] array, int m)
		{
			var newArray = new T[Mathf.Min(array.Length, m)];

			for (int i = 0; i < newArray.Length; i++)
			{
				newArray[i] = array[i];
			}

			return newArray;
		}

		private static U[] Select<T, U>(this T[] array, Func<T, U> select)
		{
			var newArray = new U[array.Length];

			for (int i = 0; i < array.Length; i++)
			{
				newArray[i] = select(array[i]);
			}

			return newArray;
		}

		private static int[] RangeArray(int n)
		{
			int[] newArray = new int[n];

			for (int i = 0; i < n; i++)
			{
				newArray[i] = i;
			}

			return newArray;
		}

		private static T[] Copy<T>(this T[] array)
		{
			return array.ToArray();
		}

		private static void Swap<T>(IList<T> permuation, int index1, int index2)
		{
			var tmp = permuation[index1];
			permuation[index1] = permuation[index2];
			permuation[index2] = tmp;
		}

		//reverses the elements in range [startIndex endIndex)
		private static void ReverseRange<T>(T[] permuation, int startIndex, int endIndex)
		{
			int leftIndex = startIndex;
			int rightIndex = endIndex - 1;

			while (leftIndex < rightIndex)
			{
				Swap(permuation, rightIndex, leftIndex);

				rightIndex--;
				leftIndex++;
			}
		}
		#endregion
	}
}