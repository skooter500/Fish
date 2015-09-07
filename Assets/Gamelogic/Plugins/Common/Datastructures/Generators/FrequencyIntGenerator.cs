using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic
{
	/// <summary>
	/// A generator that generates ints given an aribitrary distribution.
	/// </summary>
	[Version(1, 4)]
	public class FrequencyIntGenerator : IIntGenerator
	{
		private readonly float[] buckets;
		private readonly int[] indices0;
		private readonly int[] indices1;
		private Random Random;

		/// <summary>
		/// Constructs a new FrequencyIntGenerator object. The given elements and frequencies
		/// together describe a piecewise linear distribution. 
		/// </summary>
		/// <param name="relativeFrequencies">The (relative) frequency to generate integers at. The size of this
		///	sequence determines which frequencies are being generated. If the size is n, then integers from 0 
		/// to n - 1 are generated.</param>
		public FrequencyIntGenerator(IEnumerable<float> relativeFrequencies)
		{
			float[] frequencies = relativeFrequencies as float[] ?? relativeFrequencies.ToArray();

			if (frequencies.Length == 0)
			{
				throw new ArgumentException("Array cannot be empty");
			}

			if (frequencies.Length == 1)
			{
				if (frequencies[0] <= 0)
				{
					throw new ArgumentException("Sum of frequencies cannot be 0");
				}

				buckets = new[]{1f}; 
				indices0 = new[]{0};
				indices1 = new[]{0};
			}

			float sum = frequencies.Sum();

			if (sum <= 0)
			{
				throw new ArgumentException("Sum of frequencies cannot be 0");
			}

			if (frequencies.Any(x => x < 0))
			{
				throw new Exception("Frequencies must be non-negative");
			}

			float[] absoluteProbabilities = frequencies.Select(x => x/sum*frequencies.Length).ToArray();

			buckets = new float[absoluteProbabilities.Length];
			indices0 = Enumerable.Range(0, absoluteProbabilities.Length).ToArray();
			indices0 = indices0.OrderBy(i => absoluteProbabilities[i]).ToArray();

			int leftIndex = 0;
			int rightIndex = absoluteProbabilities.Length - 1;

			indices1 = new int[indices0.Length];

			while (leftIndex <= rightIndex)
			{	
				buckets[leftIndex] = absoluteProbabilities[indices0[leftIndex]];

				absoluteProbabilities[indices0[leftIndex]] = 0;
				absoluteProbabilities[indices0[rightIndex]] -= (1 - buckets[leftIndex]); 

				indices1[leftIndex] = indices0[rightIndex];

				leftIndex++;
				indices0 = indices0
					.Take(leftIndex)
					.Concat(
						indices0.Skip(leftIndex).OrderBy(i => absoluteProbabilities[i]))
					.ToArray();
			
			}

			Random = new Random();
		}

		public int Next()
		{
			if (buckets == null)
			{
				return 0;
			}

			float r = (float)Random.NextDouble()*buckets.Length;

			int i = (int)Math.Floor(r);
			float x = r - i;

			return x < buckets[i] ? indices0[i] : indices1[i];
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}