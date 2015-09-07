using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// A response curve with outputs of float.
	/// </summary>
	[Version(1, 2)]
	public class ResponseCurveFloat : ResponseCurveBase<float>
	{
		public ResponseCurveFloat(IEnumerable<float> inputSamples, IEnumerable<float> outputSamples)
			: base(inputSamples, outputSamples)
		{}
		
		public static ResponseCurveFloat GetLerp(float x0, float x1, float y0, float y1)
		{
			var input = new List<float>();
			var output = new List<float>();

			input.Add(x0);
			input.Add(x1);

			output.Add(y0);
			output.Add(y1);

			var responseCurve = new ResponseCurveFloat(input, output);

			return responseCurve;
		}

		protected override float Lerp(float outputSampleMin, float outputSampleMax, float t)
		{
			return outputSampleMin + (outputSampleMax - outputSampleMin) * Mathf.Clamp01(t);
		}
	}

	
}
