using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// A generator that generates floating values between 0 and 1 with a uniform distribution.
	/// </summary>
	[Version(1, 4)]
	public class UniformFloatGenerator : IGenerator<float>
	{
		public float Next()
		{
			return Random.value;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}