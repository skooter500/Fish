using System.Collections.Generic;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Represents a response curve for HSL values.
	/// </summary>
	[Version(1)]
	public class ResponseCurveHSL : ResponseCurveBase<ColorHSL>
	{

		public ResponseCurveHSL(IEnumerable<float> inputSamples, IEnumerable<ColorHSL> outputSamples)
			: base(inputSamples, outputSamples)
		{
		}

		protected override ColorHSL Lerp(ColorHSL outputSampleMin, ColorHSL outputSampleMax, float t)
		{
			ColorHSL output = ColorHSL.Lerp(outputSampleMin, outputSampleMax, t);
			return output;
		}

		/// <summary>
		/// Returns a response cuve that represents a linear interpolation of the given output values between the given 
		/// input values.
		/// </summary>
		/// <param name="input0"></param>
		/// <param name="input1"></param>
		/// <param name="output0"></param>
		/// <param name="output1"></param>
		/// <returns></returns>
		public static ResponseCurveHSL GetLerp(float input0, float input1, ColorHSL output0, ColorHSL output1)
		{
			var input = new List<float>();
			var output = new List<ColorHSL>();

			input.Add(input0);
			input.Add(input1);

			output.Add(output0);
			output.Add(output1);

			var responseCurve = new ResponseCurveHSL(input, output);

			return responseCurve;
		}
	}
}