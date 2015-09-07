using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// A generator that generates floats given an aribitrary distribution.
	/// </summary>
	[Version(1, 4)]
	public class FrequencyFloatGenerator : IGenerator<float>
	{
		private readonly ResponseCurveFloat responseCurve;
		private readonly UniformFloatGenerator floatGenerator;
		private readonly float accumulativeSum;

		/// <summary>
		/// Constructs a new FrequencyFloatGenerator object. The given elements and frequencies
		/// together describe a piecewise linear distribution. 
		/// </summary>
		/// <param name="elements">Samples of elements to generate.</param>
		/// <param name="frequencies">The (relative) frequency to generate the sample at.</param>
		public FrequencyFloatGenerator(IEnumerable<float> elements, IEnumerable<float> frequencies)
		{
			var accumulutiveProbability = new List<float>();

			accumulativeSum = 0f;

			foreach (var frequency in frequencies)
			{
				accumulativeSum += frequency;
				accumulutiveProbability.Add(accumulativeSum);
			}

			responseCurve = new ResponseCurveFloat(accumulutiveProbability, elements);
			floatGenerator = new UniformFloatGenerator();
		}

		public float Next()
		{
			return responseCurve[floatGenerator.Next()*accumulativeSum];
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}