using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// A response curve with outputs of Vector2.
	/// </summary>
	[Version(1, 2)]
	public class ResponseCurveVector2 : ResponseCurveBase<Vector2>
	{
		public ResponseCurveVector2(IEnumerable<float> inputSamples, IEnumerable<Vector2> outputSamples)
			: base(inputSamples, outputSamples)
		{
		}

		protected override Vector2 Lerp(Vector2 outputSampleMin, Vector2 outputSampleMax, float t)
		{
			return Vector2.Lerp(outputSampleMin, outputSampleMax, t);
		}
	}
}