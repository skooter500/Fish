using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// A response curve with outputs of Vector4.
	/// </summary>
	[Version(1, 2)]
	public class ResponseCurveVector4 : ResponseCurveBase<Vector4>
	{
		public ResponseCurveVector4(IEnumerable<float> inputSamples, IEnumerable<Vector4> outputSamples)
			: base(inputSamples, outputSamples)
		{
		}

		protected override Vector4 Lerp(Vector4 outputSampleMin, Vector4 outputSampleMax, float t)
		{
			return Vector4.Lerp(outputSampleMin, outputSampleMax, t);
		}
	}
}